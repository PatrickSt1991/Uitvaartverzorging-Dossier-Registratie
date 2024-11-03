using System;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class ConfigurationStartView : UserControl
    {
        public ConfigurationStartView()
        {
            InitializeComponent();
            lbl_Intro.Content = "Welkom " + Environment.UserName + ",\r\n\r\n" + "Kies uit het menu aan de linker kant om verder te gaan.";
        }
    }
}
