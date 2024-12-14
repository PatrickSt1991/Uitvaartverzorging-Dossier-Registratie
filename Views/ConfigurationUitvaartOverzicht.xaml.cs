using Dossier_Registratie.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class ConfigurationUitvaartOverzicht : UserControl
    {
        public ConfigurationUitvaartOverzicht()
        {
            InitializeComponent();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                CbVoorregeling.Items.Add(c.ToString());
                CbVoornaam.Items.Add(c.ToString());
            }
        }
        private void ReloadOverzicht(object sender, RoutedEventArgs e)
        {
            var viewModel = (ConfigurationUitvaartOverzichtViewModel)this.DataContext;
            viewModel.LoadAllItems();
        }
    }
}
