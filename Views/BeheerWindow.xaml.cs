using Dossier_Registratie.ViewModels;
using System.Windows;

namespace Dossier_Registratie.Views
{
    public partial class BeheerWindow : Window
    {
        public BeheerWindow()
        {
            InitializeComponent();
            this.DataContext = new ConfigurationBeheerViewModel();
        }
    }
}
