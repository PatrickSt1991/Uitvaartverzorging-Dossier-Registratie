using Dossier_Registratie.Models;
using Dossier_Registratie.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneOpbarenView : UserControl
    {
        public OverledeneOpbarenView()
        {
            InitializeComponent();

            if (Globals.PermissionLevelName == "Gebruiker")
                OpbarenGrid.IsEnabled = false;
        }
        private void ReloadDynamicElements(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneOpbarenViewModel)this.DataContext;
            viewModel.ReloadDynamicElements();
        }
    }
}
