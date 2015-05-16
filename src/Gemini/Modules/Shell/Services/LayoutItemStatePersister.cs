using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace Gemini.Modules.Shell.Services
{
    [Export(typeof(ILayoutItemStatePersister))]
    public class LayoutItemStatePersister : ILayoutItemStatePersister
    {
        public void SaveState(IShell shell, IShellView shellView, string fileName)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                using (var writer = new BinaryWriter(stream))
                {
                    stream = null;

                    IEnumerable<ILayoutItem> itemStates = shell.Documents.Concat(shell.Tools.Cast<ILayoutItem>());

                    int itemCount = 0;
                    // reserve some space for items count, it'll be updated later
                    writer.Write(itemCount);

                    foreach (var item in itemStates)
                    {
                        if (!item.ShouldReopenOnStart)
                            continue;

                        // simply save by concrete type name allways. Then in LoadState we need to match this type up with 
                        // one of our known export types (Tools or Documents). 
                        var selectedTypeName = item.GetType().AssemblyQualifiedName;

                        writer.Write(selectedTypeName);
                        writer.Write(item.ContentId);

                        // Here's the tricky part. Because some items might fail to save their state, or they might be removed (a plug-in assembly deleted and etc.)
                        // we need to save the item's state size to be able to skip the data during deserialization.
                        // Save current stream position. We'll need it later.
                        long stateSizePosition = writer.BaseStream.Position;

                        // Reserve some space for item state size
                        writer.Write(0L);

                        long stateSize;

                        try
                        {
                            long stateStartPosition = writer.BaseStream.Position;
                            item.SaveState(writer);
                            stateSize = writer.BaseStream.Position - stateStartPosition;
                        }
                        catch
                        {
                            stateSize = 0;
                        }

                        // Go back to the position before item's state and write the actual value.
                        writer.BaseStream.Seek(stateSizePosition, SeekOrigin.Begin);
                        writer.Write(stateSize);

                        if (stateSize > 0)
                        {
                            // Got to the end of the stream
                            writer.BaseStream.Seek(0, SeekOrigin.End);
                        }

                        itemCount++;
                    }

                    writer.BaseStream.Seek(0, SeekOrigin.Begin);
                    writer.Write(itemCount);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    shellView.SaveLayout(writer.BaseStream);
                }
            }
            catch
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        Type GetTypeFromContractNameAsILayoutItem(ExportAttribute attribute)
        {
            if (attribute == null)
                return null;

            string typeName;
            if ((typeName = attribute.ContractName) == null)
                return null;

            var type = Type.GetType(typeName);
            if (type == null || !typeof(ILayoutItem).IsInstanceOfType(type))
                return null;
            return type;
        }

        // If we make these Lazy, we speed startup time, and reduce dependency issues
        [ImportMany(typeof(ITool))]
        private IEnumerable<Lazy<ITool, IToolMetadata>> AllExportedTools { get; set; }

        [ImportMany(typeof(IDocument))]
        private IEnumerable<ExportFactory<IDocument, IDocumentMetadata>> AllExportedDocuments { get; set; }

        public void LoadState(IShell shell, IShellView shellView, string fileName)
        {
            var layoutItems = new Dictionary<string, ILayoutItem>();

            if (!File.Exists(fileName))
            {
                return;
            }

            FileStream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                using (var reader = new BinaryReader(stream))
                {
                    stream = null;

                    int count = reader.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        string typeName = reader.ReadString();
                        string contentId = reader.ReadString();
                        long stateEndPosition = reader.ReadInt64();
                        stateEndPosition += reader.BaseStream.Position;

                        var contentType = Type.GetType(typeName);
                        bool skipStateData = true;

                        if (contentType != null)
                        {
                            // lookup the content within exported tools and document factories
                            ILayoutItem contentInstance = AllExportedTools.Where(x => x.Metadata.ToolType == contentType).Select(x => x.Value).FirstOrDefault();
                            if (contentInstance == null)
                            {
                                // see if it is a document type, and support instancing multiple times
                                var factory = AllExportedDocuments.Where(x => typeName.Contains(x.Metadata.Type.ToString())).FirstOrDefault();
                                if (factory != null)
                                {
                                    contentInstance = factory.CreateExport().Value;
                                }
                            }

                            if (contentInstance == null)
                            {
                                // fall back to the old way - ELIMINATE THIS SOON!
                                contentInstance = IoC.GetInstance(contentType, null) as ILayoutItem;
                            }

                            if (contentInstance != null)
                            {
                                layoutItems.Add(contentId, contentInstance);

                                try
                                {
                                    contentInstance.LoadState(reader);
                                    skipStateData = false;
                                }
                                catch
                                {
                                    skipStateData = true;
                                }
                            }
                        }

                        // Skip state data block if we couldn't read it.
                        if (skipStateData)
                        {
                            reader.BaseStream.Seek(stateEndPosition, SeekOrigin.Begin);
                        }
                    }

                    shellView.LoadLayout(reader.BaseStream, shell.ShowTool, shell.OpenDocument, layoutItems);
                }
            }
            catch
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
    }
}