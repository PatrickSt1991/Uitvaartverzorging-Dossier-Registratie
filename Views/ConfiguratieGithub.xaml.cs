using Dossier_Registratie.Helper;
using Dossier_Registratie.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class ConfiguratieGithub : UserControl
    {
        public ConfiguratieGithub()
        {
            InitializeComponent();
        }
        private void ReloadHelp(object sender, RoutedEventArgs e)
        {
            var viewModel = (ConfigurationGithubViewModel)this.DataContext;
            if (DataProvider.GithubEnabled)
                viewModel.LoadIssuesAsync();
        }
    }
}
