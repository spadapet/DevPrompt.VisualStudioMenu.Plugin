using System.Windows;

namespace VisualStudioMenu.Plugin
{
    internal partial class InstallBranchDialog : Window
    {
        public InstallBranchDialogVM ViewModel { get; }
        public string BranchName => this.ViewModel.Name;

        public InstallBranchDialog()
        {
            this.ViewModel = new InstallBranchDialogVM(this, "master");

            this.InitializeComponent();
        }

        private void OnClickOk(object sender, RoutedEventArgs args)
        {
            this.DialogResult = true;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            this.editControl.Focus();
            this.editControl.SelectAll();
        }
    }
}
