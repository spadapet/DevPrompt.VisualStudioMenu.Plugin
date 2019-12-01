using System.Windows.Input;
using VisualStudioMenu.Plugin.Utility;

namespace VisualStudioMenu.Plugin
{
    /// <summary>
    /// View model for the dialog to install a VS branch
    /// </summary>
    internal class InstallBranchDialogVM : PropertyNotifier
    {
        private readonly InstallBranchDialog dialog;
        private string name;

        public InstallBranchDialogVM(InstallBranchDialog dialog, string name)
        {
            this.dialog = dialog;
            this.name = name;
        }

        public string Name
        {
            get => this.name;
            set
            {
                if (this.SetPropertyValue(ref this.name, value ?? string.Empty))
                {
                    this.OnPropertyChanged(nameof(this.Hyperlink));
                }
            }
        }

        public string Hyperlink => $"https://aka.ms/vs/16/int.{this.Name}/vs_Enterprise.exe";

        public ICommand InstallCommand => new DelegateCommand(() =>
        {
            this.dialog.DialogResult = true;
        });
    }
}
