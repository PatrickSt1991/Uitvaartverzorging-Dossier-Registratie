﻿using Dossier_Registratie.Interfaces;
using System.Windows;

namespace Dossier_Registratie.Views
{
    public partial class GeneratingDocumentView : Window, IGeneratingDocumentWindow
    {
        public GeneratingDocumentView()
        {
            InitializeComponent();
        }
        public new void Show()
        {
            base.Show(); 
        }

        public new void Hide()
        {
            base.Hide(); 
        }
    }
}
