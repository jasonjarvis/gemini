using Gemini.Modules.UndoRedo;
using System;
using System.ComponentModel.Composition;

namespace Gemini.Framework
{
	public interface IDocument : ILayoutItem
	{
        IUndoRedoManager UndoRedoManager { get; }
	}

    /// <summary>
    /// This is a nice typed interface with which to access the meta data
    /// exported via the ExportTool attribute below
    /// Note: this interface should match the properties of the ExportDocumentAttribute
    /// </summary>
    public interface IDocumentMetadata
    {
        Type Type { get; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportDocumentAttribute : ExportAttribute
    {
        public ExportDocumentAttribute(Type concreteType)
            : base(typeof(IDocument))
        {
            Type = concreteType;
        }

        public Type Type { get; private set; }
    }
}