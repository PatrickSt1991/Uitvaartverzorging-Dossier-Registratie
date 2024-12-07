using Dossier_Registratie.Models;
using Dossier_Registratie.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneAsbestemmingView : UserControl
    {
        public OverledeneAsbestemmingView()
        {
            InitializeComponent();

            if (Globals.PermissionLevelName == "Gebruiker")
                AsbestemmingGrid.IsEnabled = false;
        }
        private void ReloadDynamicElements(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneAsbestemmingViewModel)this.DataContext;
            viewModel.ReloadDynamicElements();
        }
    }
}
