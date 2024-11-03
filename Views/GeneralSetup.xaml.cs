using Dossier_Registratie.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class GeneralSetup : UserControl
    {
        public GeneralSetup()
        {
            InitializeComponent();
            this.DataContext = new GeneralSetupViewModel();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is GeneralSetupViewModel viewModel)
                viewModel.Password = PasswordBox.Password;
        }
    }
}
