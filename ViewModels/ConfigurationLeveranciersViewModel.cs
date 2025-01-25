using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationLeveranciersViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivateLeverancierCommand { get; }
        public ICommand EditLeverancierCommand { get; }
        public ICommand DisableLeverancierCommand { get; }
        public ICommand CloseEditLeverancierPopupCommand { get; }
        public ICommand SaveLeverancierCommand { get; }
        public ICommand CreateNewLeverancierCommand { get; }
        public ICommand RefreshLeverancierGridCommand { get; set; }
        public ICommand UploadLogoCommand { get; }

        private bool isEditLeverancierPopupOpen;
        private bool newLeverancier;
        private bool _showDeleted;

        private LeveranciersModel selectedLeverancier;
        private LeverancierContactModel selectedLeverancierContact;
        private ObservableCollection<LeveranciersModel> _leveranciers;
        private ICollectionView _filteredLeveranciers;
        public LeveranciersModel SelectedLeverancier
        {
            get { return selectedLeverancier; }
            set
            {
                if (selectedLeverancier != value)
                {
                    selectedLeverancier = value;
                    OnPropertyChanged(nameof(SelectedLeverancier));
                }
            }
        }
        public LeverancierContactModel SelectedLeverancierContact
        {
            get { return selectedLeverancierContact; }
            set
            {
                if (selectedLeverancierContact != value)
                {
                    selectedLeverancierContact = value;
                    OnPropertyChanged(nameof(SelectedLeverancierContact));
                }
            }
        }
        public ObservableCollection<LeveranciersModel> Leveranciers
        {
            get { return _leveranciers; }
            set
            {
                if (_leveranciers != value)
                {
                    _leveranciers = value;
                    OnPropertyChanged(nameof(Leveranciers));
                }
            }
        }
        public ICollectionView FilteredLeveranciers
        {
            get => _filteredLeveranciers;
            set
            {
                _filteredLeveranciers = value;
                OnPropertyChanged(nameof(FilteredLeveranciers));
            }
        }
        public bool IsEditLeverancierPopupOpen
        {
            get { return isEditLeverancierPopupOpen; }
            set
            {
                if (isEditLeverancierPopupOpen != value)
                {
                    isEditLeverancierPopupOpen = value;
                    OnPropertyChanged(nameof(IsEditLeverancierPopupOpen));
                }
            }
        }
        public bool ShowDeleted
        {
            get { return _showDeleted; }
            set
            {
                if (_showDeleted != value)
                {
                    _showDeleted = value;
                    OnPropertyChanged(nameof(ShowDeleted));
                    FilteredLeveranciers.Refresh();
                }
            }
        }
        public ConfigurationLeveranciersViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedLeverancier = new LeveranciersModel();
            SelectedLeverancierContact = new LeverancierContactModel();
            Leveranciers = new ObservableCollection<LeveranciersModel>();

            FilteredLeveranciers = CollectionViewSource.GetDefaultView(Leveranciers);
            FilteredLeveranciers.Filter = FilterLeveranciers;

            ActivateLeverancierCommand = new ViewModelCommand(ExecuteActivateLeverancierCommand);
            EditLeverancierCommand = new ViewModelCommand(ExecuteEditLeverancierCommand);
            DisableLeverancierCommand = new ViewModelCommand(ExecuteDisableLeverancierCommand);
            CloseEditLeverancierPopupCommand = new RelayCommand(() => IsEditLeverancierPopupOpen = false);
            RefreshLeverancierGridCommand = new RelayCommand(LeverancierGridData);
            SaveLeverancierCommand = new ViewModelCommand(ExecuteSaveLeverancierCommand);
            CreateNewLeverancierCommand = new ViewModelCommand(ExecuteCreateNewLeverancier);

            LeverancierGridData();
        }
        public void ExecuteActivateLeverancierCommand(object obj)
        {
            MessageBoxResult ativateLeverancier = MessageBox.Show("Wil je deze leverancier heractiveren?", "Leverancier heractiveren", MessageBoxButton.YesNo);
            if (ativateLeverancier == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid leverancierId))
                {
                    try
                    {
                        commandRepository.ActivateLeverancier(leverancierId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error activating leverancier: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    LeverancierGridData();
                }
            }
        }
        public void ExecuteEditLeverancierCommand(object obj)
        {
            var leverancier = miscellaneousRepository.GetLeverancierBeheer((Guid)obj);

            selectedLeverancier.LeverancierId = leverancier.LeverancierId;
            selectedLeverancier.LeverancierName = leverancier.LeverancierName;
            selectedLeverancier.LeverancierBeschrijving = leverancier.LeverancierBeschrijving;
            selectedLeverancier.Steenhouwer = leverancier.Steenhouwer;
            selectedLeverancier.Bloemist = leverancier.Bloemist;
            selectedLeverancier.Kisten = leverancier.Kisten;
            selectedLeverancier.UrnSieraden = leverancier.UrnSieraden;

            if (!string.IsNullOrEmpty(SelectedLeverancier.LeverancierContactGegevens))
            {
                var deserializedContactInfo = JsonConvert.DeserializeObject<LeverancierContactModel>(SelectedLeverancier.LeverancierContactGegevens);

                selectedLeverancierContact.LeverancierAdres = deserializedContactInfo.LeverancierAdres;
                selectedLeverancierContact.LeverancierHuisnummer = deserializedContactInfo.LeverancierHuisnummer;
                selectedLeverancierContact.LeverancierHuisnummerToevoeging = deserializedContactInfo.LeverancierHuisnummerToevoeging;
                selectedLeverancierContact.LeverancierPostcode = deserializedContactInfo.LeverancierPostcode;
                selectedLeverancierContact.LeverancierPlaats = deserializedContactInfo.LeverancierPlaats;
            }

            newLeverancier = false;
            IsEditLeverancierPopupOpen = true;
        }
        public void ExecuteDisableLeverancierCommand(object obj)
        {
            MessageBoxResult disableLeverancier = MessageBox.Show("Wil je deze leverancier deactiveren?", "Leverancier deactiveren", MessageBoxButton.YesNo);
            if (disableLeverancier == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid leverancierId))
                {
                    try
                    {
                        commandRepository.DisableLeverancier(leverancierId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error disabeling leverancier: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    LeverancierGridData();
                }
                else
                {
                    MessageBox.Show("Invalid employee ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void LeverancierGridData()
        {
            Leveranciers.Clear();
            foreach (var leverancier in miscellaneousRepository.GetLeveranciers())
            {
                Leveranciers.Add(new LeveranciersModel
                {
                    LeverancierId = leverancier.LeverancierId,
                    LeverancierName = leverancier.LeverancierName,
                    LeverancierBeschrijving = leverancier.LeverancierBeschrijving,
                    Steenhouwer = leverancier.Steenhouwer,
                    Bloemist = leverancier.Bloemist,
                    Kisten = leverancier.Kisten,
                    UrnSieraden = leverancier.UrnSieraden,
                    IsDeleted = leverancier.IsDeleted,
                    BtnBrush = leverancier.BtnBrush
                });
            }
        }
        public void ExecuteSaveLeverancierCommand(object obj)
        {
            var contactInfo = new LeverancierContactModel
            {
                LeverancierAdres = SelectedLeverancierContact.LeverancierAdres,
                LeverancierHuisnummer = SelectedLeverancierContact.LeverancierHuisnummer,
                LeverancierHuisnummerToevoeging = SelectedLeverancierContact.LeverancierHuisnummerToevoeging,
                LeverancierPostcode = SelectedLeverancierContact.LeverancierPostcode,
                LeverancierPlaats = SelectedLeverancierContact.LeverancierPlaats
            };

            SelectedLeverancier.LeverancierContactGegevens = JsonConvert.SerializeObject(contactInfo);

            if (!newLeverancier)
            {
                try
                {
                    updateRepository.UpdateLeverancier(SelectedLeverancier);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating leverancier: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                SelectedLeverancier.LeverancierId = Guid.NewGuid();

                try
                {
                    createRepository.CreateLeverancier(SelectedLeverancier);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting leverancier: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            CloseEditLeverancierPopupCommand.Execute(null);
            LeverancierGridData();
        }
        public void ExecuteCreateNewLeverancier(object obj)
        {
            newLeverancier = true;

            selectedLeverancier.LeverancierId = Guid.NewGuid();
            selectedLeverancier.LeverancierName = string.Empty;
            selectedLeverancier.LeverancierBeschrijving = string.Empty;
            selectedLeverancier.Steenhouwer = false;
            selectedLeverancier.Bloemist = false;
            selectedLeverancier.Kisten = false;
            selectedLeverancier.UrnSieraden = false;
            selectedLeverancier.IsDeleted = false;

            IsEditLeverancierPopupOpen = true;
        }
        private bool FilterLeveranciers(object item)
        {
            if (item is LeveranciersModel leverancier)
                return ShowDeleted || !leverancier.IsDeleted;

            return false;
        }
    }
}
