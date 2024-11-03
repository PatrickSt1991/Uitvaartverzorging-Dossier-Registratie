using Dossier_Registratie.ViewModels;
using System.Windows;
using System.Windows.Controls;
using static Dossier_Registratie.MainWindow;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneSteenhouwerijView : UserControl
    {
        public bool SearchIsUpdate = false;
        public OverledeneSteenhouwerijView()
        {
            InitializeComponent();

            if (Globals.PermissionLevelName == "Gebruiker")
                SteenBloemBon.IsEnabled = false;
        }
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(PreviousClickedEvent));
        }
        private void ReloadDynamicElements(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneSteenhouwerijViewModel)this.DataContext;
            viewModel.ReloadDynamicElements();
        }
    }
}
