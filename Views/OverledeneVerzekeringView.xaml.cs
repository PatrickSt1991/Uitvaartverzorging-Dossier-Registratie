using Dossier_Registratie.Models;
using Dossier_Registratie.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneVerzekeringView : UserControl
    {
        public bool SearchIsUpdate = false;

        public OverledeneVerzekeringView()
        {
            InitializeComponent();

            if (Globals.PermissionLevelName == "Gebruiker")
                MainGrid.IsEnabled = false;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(MainWindow.PreviousClickedEvent));
        }
        private void ReloadDynamicElements(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneVerzekeringViewModal)this.DataContext;
            viewModel.ReloadDynamicElements();
        }
    }
}
