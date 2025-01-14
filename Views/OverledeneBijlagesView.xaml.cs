using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.ViewModels;
using Dossier_Registratie.Interfaces;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Dossier_Registratie.Views
{
    public partial class OverledeneBijlagesView : UserControl
    {
        private readonly ISearchOperations searchOperations;
        public OverledeneBijlagesView()
        {
            InitializeComponent();
            searchOperations = new SearchOperations();
            OverledeneBijlagesViewModel.BijlagesInstance.DataLoaded += ViewModel_DataLoaded;


            if (Globals.PermissionLevelName == "Gebruiker")
                BijlageGrid.IsEnabled = false;
        }
        public void ViewModel_DataLoaded(object? sender, EventArgs e)
        {
            searchOperations
                .GetOverlijdenBijlagesByUitvaartId(Globals.UitvaartCode)
                .Where(ShouldProcessBijlage)
                .ToList()
                .ForEach(ProcessBijlage);
        }
        private static bool ShouldProcessBijlage(OverledeneBijlagesModel bijlage)
        {
            return !bijlage.IsDeleted &&
                   !bijlage.DocumentName.StartsWith("AkteVanCessie_") &&
                   !bijlage.DocumentName.Equals("Bloemen") &&
                   !bijlage.DocumentName.Equals("Verlof") &&
                   !bijlage.DocumentName.Equals("Dossier");
        }
        private void ProcessBijlage(OverledeneBijlagesModel bijlage)
        {
            ComboBox attachmentCb = (ComboBox)this.FindName("cb_Document" + bijlage.DocumentName);

            if (bijlage.DocumentName == "Terugmelding")
                UpdateComboBox(attachmentCb, "Alles");
            else
                UpdateComboBox(attachmentCb, bijlage.DocumentUrl);
        }
        private static void UpdateComboBox(ComboBox comboBox, string tag)
        {
            if (comboBox == null)
            {
                return; // Do nothing if comboBox is null.
            }

            comboBox.Tag = tag;

            foreach (var item in comboBox.Items)
            {
                if (item is ComboBoxItem comboBoxItem && (string)comboBoxItem.Tag == "Opnieuw")
                {
                    comboBoxItem.Visibility = System.Windows.Visibility.Visible;
                    break;
                }
            }
        }
        private void Cb_GeneralDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedIndex != -1)
            {
                ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null && selectedItem.Tag != null)
                {
                    var viewModel = (OverledeneBijlagesViewModel)this.DataContext;

                    switch (comboBox.Name)
                    {
                        case "cb_DocumentAangifte":
                            viewModel.CreateDocumentAangifte(selectedItem.Tag);
                            break;
                        case "cb_DocumentDocument":
                            viewModel.CreateDocumentDocument(selectedItem.Tag);
                            break;
                        case "cb_DocumentChecklist":
                            viewModel.CreateDocumentChecklist(selectedItem.Tag);
                            break;
                        case "cb_DocumentOverdracht":
                            viewModel.CreateDocumentOverdracht(selectedItem.Tag);
                            break;
                        case "cb_DocumentOpdrachtBegrafenis":
                            viewModel.CreateDocumentBegrafenis(selectedItem.Tag);
                            break;
                        case "cb_DocumentOpdrachtCrematie":
                            viewModel.CreateDocumentCrematie(selectedItem.Tag);
                            break;
                        case "cb_DocumentTerugmelding":
                            viewModel.CreateDocumentTerugmelding(selectedItem.Tag);
                            break;
                        case "cb_DocumentKoffiekamer":
                            viewModel.CreateDocumentKoffie(selectedItem.Tag);
                            break;
                        case "cb_DocumentAanvraagDienst":
                            viewModel.CreateDocumentDienst(selectedItem.Tag);
                            break;
                        case "cb_DocumentBezittingen":
                            viewModel.CreateDocumentBezittingen(selectedItem.Tag);
                            break;
                        case "cb_DocumentTevredenheid":
                            viewModel.CreateDocumentTevredenheid(selectedItem.Tag);
                            break;
                    }
                    comboBox.SelectedIndex = -1;
                }
            }
        }
    }
}
