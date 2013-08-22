using System.Windows;

namespace Gemini.Modules.MainWindow.Views
{
	public partial class MainWindowView
    {
        public MainWindowView()
		{
			InitializeComponent();
		}

        // TODO: this is an attempt at getting the application to save state when quit via the 
        // close button at top right corner. For some reason it does not go through 
        // existing code paths.
        //
        private void OnWindowUnloaded(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as Gemini.Framework.Services.IMainWindow;
            if (vm != null)
            {
                vm.Shell.Close();
            }
        }
    }
}
