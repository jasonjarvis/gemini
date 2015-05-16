using Gemini.Framework.Services;
using System;
using System.ComponentModel.Composition;

namespace Gemini.Framework
{
    public interface ITool : ILayoutItem
    {
        PaneLocation PreferredLocation { get; }
        double PreferredWidth { get; }
        double PreferredHeight { get; }

        bool IsVisible { get; set; }
    }

    /// <summary>
    /// This is a nice typed interface with which to access the meta data
    /// exported via the ExportTool attribute below
    /// Note: this interface should match the properties of the ExportToolAttribute
    /// </summary>
    public interface IToolMetadata
    {
        Type ToolType { get; }
        string DisplayName { get; }
        string Category { get; }
        int SortOrder { get; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportToolAttribute : ExportAttribute
    {
        public ExportToolAttribute(Type concreteType, string displayName)
            : base(typeof(ITool))
        {
            ToolType = concreteType;
            DisplayName = displayName;
        }

        public Type ToolType { get; private set; }
        public string DisplayName { get; private set; }
        public string Category { get; set; }
        public int SortOrder { get; set; }
    }
}