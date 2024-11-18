using Dossier_Registratie.ViewModels;
using System.Windows;
using System.Windows.Controls;
using static Dossier_Registratie.MainWindow;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneUitvaartView : UserControl
    {
        public bool SearchIsUpdate = false;

        public OverledeneUitvaartView()
        {
            InitializeComponent();

            if (Globals.PermissionLevelName == "Gebruiker")
                GridUitvaartView.IsEnabled = false;
        }
        private void ReloadDynamicElements(object sender, RoutedEventArgs e)
        {
            var viewModel = (OverledeneUitvaartViewModel)this.DataContext;
            viewModel.RefresDynamicElements();
        }

        private void input_UitvaartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (OverledeneUitvaartViewModel)this.DataContext;
            if (sender is ComboBox comboBox)
                if (comboBox.SelectedItem is ComboBoxItem selectedItem)
                    viewModel.IsBegrafenisVisable = selectedItem.Name == "begrafenis" ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
