using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationHerkomstenViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivateHerkomstCommand { get; }
        public ICommand EditHerkomstCommand { get; }
        public ICommand DisableHerkomstCommand { get; }
        public ICommand CloseEditHerkomstPopupCommand { get; }
        public ICommand SaveHerkomstCommand { get; }
        public ICommand CreateNewHerkomstCommand { get; }
        public ICommand RefreshHerkomstGridCommand { get; set; }

        private bool isEditHerkomstPopupOpen;
        private bool newHerkomst;

        private VerzekeraarsModel selectedHerkomst;
        private ObservableCollection<VerzekeraarsModel> _herkomsten;

        public VerzekeraarsModel SelectedHerkomst
        {
            get { return selectedHerkomst; }
            set
            {
                if (selectedHerkomst != value)
                {
                    selectedHerkomst = value;
                    OnPropertyChanged(nameof(SelectedHerkomst));
                }
            }
        }
        public ObservableCollection<VerzekeraarsModel> Herkomsten
        {
            get { return _herkomsten; }
            set
            {
                if (_herkomsten != value)
                {
                    _herkomsten = value;
                    OnPropertyChanged(nameof(Herkomsten));
                }
            }
        }
        public bool IsEditHerkomstPopupOpen
        {
            get { return isEditHerkomstPopupOpen; }
            set
            {
                if (isEditHerkomstPopupOpen != value)
                {
                    isEditHerkomstPopupOpen = value;
                    OnPropertyChanged(nameof(IsEditHerkomstPopupOpen));
                }
            }
        }

        public ConfigurationHerkomstenViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedHerkomst = new VerzekeraarsModel();
            Herkomsten = new ObservableCollection<VerzekeraarsModel>();

            ActivateHerkomstCommand = new ViewModelCommand(ExecuteActivateHerkomstCommand);
            EditHerkomstCommand = new ViewModelCommand(ExecuteEditHerkomstCommand);
            DisableHerkomstCommand = new ViewModelCommand(ExecuteDisableHerkomstCommand);
            CloseEditHerkomstPopupCommand = new RelayCommand(() => IsEditHerkomstPopupOpen = false);
            RefreshHerkomstGridCommand = new RelayCommand(HerkomstGridData);
            SaveHerkomstCommand = new ViewModelCommand(ExecuteSaveHerkomstCommand);
            CreateNewHerkomstCommand = new ViewModelCommand(ExecuteCreateNewHerkomst);

            HerkomstGridData();
        }
        public void ExecuteActivateHerkomstCommand(object obj)
        {
            MessageBoxResult disableQuestion = MessageBox.Show("Wil je deze herkomst heractiveren?", "Herkomst heractiveren", MessageBoxButton.YesNo);
            if (disableQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid verzekeraarId))
                {
                    commandRepository.ActivateVerzekeraar(verzekeraarId);
                    HerkomstGridData();
                }
                else
                {
                    MessageBox.Show("Invalid Verzekeraar ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ExecuteEditHerkomstCommand(object obj)
        {
            var herkomst = miscellaneousRepository.GetHerkomstById((Guid)obj);

            selectedHerkomst.Id = herkomst.Id;
            selectedHerkomst.Afkorting = herkomst.Afkorting;
            selectedHerkomst.Name = herkomst.Name;
            selectedHerkomst.HasLidnummer = herkomst.HasLidnummer;
            selectedHerkomst.IsHerkomst = true;
            SelectedHerkomst.IsVerzekeraar = false;
            selectedHerkomst.CorrespondentieType = herkomst.CorrespondentieType;
            selectedHerkomst.FactuurType = System.Net.WebUtility.HtmlDecode(herkomst.FactuurType);
            selectedHerkomst.AddressHousenumber = herkomst.AddressHousenumber;
            selectedHerkomst.AddressStreet = herkomst.AddressStreet;
            selectedHerkomst.AddressCity = herkomst.AddressCity;
            selectedHerkomst.AddressZipCode = herkomst.AddressZipCode;
            selectedHerkomst.AddressHousenumberAddition = herkomst.AddressHousenumberAddition;
            selectedHerkomst.PostbusAddress = herkomst.PostbusAddress;
            selectedHerkomst.PostbusName = herkomst.PostbusName;
            selectedHerkomst.IsOverrideFactuurAdress = herkomst.IsOverrideFactuurAdress;
            selectedHerkomst.Telefoon = herkomst.Telefoon;

            newHerkomst = false;
            IsEditHerkomstPopupOpen = true;
        }
        public void ExecuteDisableHerkomstCommand(object obj)
        {
            MessageBoxResult disableQuestion = MessageBox.Show("Wil je deze herkomst deactiveren?", "Herkomst deactiveren", MessageBoxButton.YesNo);
            if (disableQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid verzekeraarId))
                {
                    try
                    {
                        commandRepository.DisableVerzekeraar(verzekeraarId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error disableling hekomst: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    HerkomstGridData();
                }
                else
                {
                    MessageBox.Show("Invalid employee ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void HerkomstGridData()
        {
            Herkomsten.Clear();
            foreach (var herkomst in miscellaneousRepository.GetHerkomst())
            {
                if (herkomst.IsHerkomst == true)
                {
                    Herkomsten.Add(new VerzekeraarsModel
                    {
                        Id = herkomst.Id,
                        Name = herkomst.Name,
                        Afkorting = herkomst.Afkorting,
                        HasLidnummer = herkomst.HasLidnummer,
                        IsDeleted = herkomst.IsDeleted,
                        BtnBrush = herkomst.BtnBrush,
                        AddressStreet = herkomst.AddressStreet,
                        AddressHousenumber = herkomst.AddressHousenumber,
                        AddressHousenumberAddition = herkomst.AddressHousenumberAddition,
                        AddressCity = herkomst.AddressCity,
                        AddressZipCode = herkomst.AddressZipCode,
                        FactuurType = System.Net.WebUtility.HtmlDecode(herkomst.FactuurType)

                    });
                }
            }
        }
        public void ExecuteSaveHerkomstCommand(object obj)
        {
            CloseEditHerkomstPopupCommand.Execute(null);
            selectedHerkomst.Pakket = false;

            if (!newHerkomst)
            {
                try
                {
                    updateRepository.VerzekeringUpdate(selectedHerkomst);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating verzekering: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                try
                {
                    createRepository.VerzekeringCreate(selectedHerkomst);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting verzekering: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            HerkomstGridData();
        }
        public void ExecuteCreateNewHerkomst(object obj)
        {
            newHerkomst = true;

            selectedHerkomst.Id = Guid.NewGuid();
            selectedHerkomst.Afkorting = Guid.NewGuid().ToString();
            selectedHerkomst.Name = string.Empty;
            selectedHerkomst.HasLidnummer = false;
            selectedHerkomst.IsHerkomst = true;
            selectedHerkomst.CorrespondentieType = string.Empty;
            selectedHerkomst.IsHerkomst = true;
            selectedHerkomst.IsVerzekeraar = false;
            selectedHerkomst.Pakket = false;
            selectedHerkomst.PostbusAddress = string.Empty;
            selectedHerkomst.PostbusName = string.Empty;
            selectedHerkomst.AddressCity = string.Empty;
            selectedHerkomst.AddressHousenumber = string.Empty;
            selectedHerkomst.AddressHousenumberAddition = string.Empty;
            selectedHerkomst.AddressZipCode = string.Empty;
            selectedHerkomst.AddressStreet = string.Empty;
            selectedHerkomst.IsOverrideFactuurAdress = false;
            selectedHerkomst.Telefoon = string.Empty;

            IsEditHerkomstPopupOpen = true;
        }
    }
}
