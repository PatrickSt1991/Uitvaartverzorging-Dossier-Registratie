using Dossier_Registratie.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class ConfigurationAgenda : UserControl
    {
        public ConfigurationAgenda()
        {
            InitializeComponent();
        }
        private void ReloadAgenda(object sender, RoutedEventArgs e)
        {
            var viewModel = (ConfigurationAgendaViewModel)this.DataContext;
            viewModel.GetAllAgendaItems();
        }
    }
}
