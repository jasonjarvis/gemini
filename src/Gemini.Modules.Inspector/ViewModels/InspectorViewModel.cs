using System;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Gemini.Modules.Inspector.ViewModels
{
    [Export(typeof(IInspectorTool))]
    [ExportTool(typeof(InspectorViewModel), "Inspector", Category = "View", SortOrder = 5)]
    public class InspectorViewModel : Tool, IInspectorTool
    {
        public event EventHandler SelectedObjectChanged;

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        public override double PreferredWidth
        {
            get { return 300; }
        }

        private IInspectableObject _selectedObject;

        public IInspectableObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                NotifyOfPropertyChange(() => SelectedObject);
                RaiseSelectedObjectChanged();
            }
        }

        public InspectorViewModel()
        {
            DisplayName = "Inspector";
        }

        private void RaiseSelectedObjectChanged()
        {
            EventHandler handler = SelectedObjectChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}