using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationVerzekeraarsViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivateVerzekeringCommand { get; }
        public ICommand EditVerzekeringCommand { get; }
        public ICommand DisableVerzekeringCommand { get; }
        public ICommand CloseEditVerzekeringPopupCommand { get; }
        public ICommand SaveVerzekeringCommand { get; }
        public ICommand CreateNewVerzekeraarCommand { get; }
        public ICommand RefreshVerzekeraarGridCommand { get; set; }
        public ICommand UploadLogo { get; }

        private bool isEditVerzekeringPopupOpen;
        private bool newVerzekering;

        private VerzekeraarsModel selectedVerzekering;
        private ObservableCollection<VerzekeraarsModel> _verzekeraars;

        public VerzekeraarsModel SelectedVerzekering
        {
            get { return selectedVerzekering; }
            set
            {
                if (selectedVerzekering != value)
                {
                    selectedVerzekering = value;
                    OnPropertyChanged(nameof(SelectedVerzekering));
                }
            }
        }
        public ObservableCollection<VerzekeraarsModel> Verzekeraars
        {
            get { return _verzekeraars; }
            set
            {
                if (_verzekeraars != value)
                {
                    _verzekeraars = value;
                    OnPropertyChanged(nameof(Verzekeraars));
                }
            }
        }
        public bool IsEditVerzekeringPopupOpen
        {
            get { return isEditVerzekeringPopupOpen; }
            set
            {
                if (isEditVerzekeringPopupOpen != value)
                {
                    isEditVerzekeringPopupOpen = value;
                    OnPropertyChanged(nameof(IsEditVerzekeringPopupOpen));
                }
            }
        }

        public ConfigurationVerzekeraarsViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedVerzekering = new VerzekeraarsModel();
            Verzekeraars = new ObservableCollection<VerzekeraarsModel>();

            ActivateVerzekeringCommand = new ViewModelCommand(ExecuteActivateVerzekeringCommand);
            EditVerzekeringCommand = new ViewModelCommand(ExecuteEditVerzekeringCommand);
            DisableVerzekeringCommand = new ViewModelCommand(ExecuteDisableVerzekeringCommand);
            RefreshVerzekeraarGridCommand = new RelayCommand(() => VerzekeringGridData());
            SaveVerzekeringCommand = new ViewModelCommand(ExecuteSaveVerzekeringCommand);
            CreateNewVerzekeraarCommand = new ViewModelCommand(ExecuteCreateNewVerzekeraar);
            UploadLogo = new ViewModelCommand(ExecuteUploadLogoCommand);

            CloseEditVerzekeringPopupCommand = new RelayCommand(() => IsEditVerzekeringPopupOpen = false);

            VerzekeringGridData();
        }
        public void VerzekeringGridData()
        {
            Verzekeraars.Clear();
            foreach (var verzekeraar in miscellaneousRepository.GetVerzekeraars())
            {
                Verzekeraars.Add(new VerzekeraarsModel
                {
                    Id = verzekeraar.Id,
                    Name = verzekeraar.Name,
                    Afkorting = verzekeraar.Afkorting,
                    HasLidnummer = verzekeraar.HasLidnummer,
                    IsDeleted = verzekeraar.IsDeleted,
                    BtnBrush = verzekeraar.BtnBrush,
                    AddressStreet = verzekeraar.AddressStreet,
                    AddressHousenumber = verzekeraar.AddressHousenumber,
                    AddressHousenumberAddition = verzekeraar.AddressHousenumberAddition,
                    AddressCity = verzekeraar.AddressCity,
                    AddressZipCode = verzekeraar.AddressZipCode,
                    FactuurType = verzekeraar.FactuurType,
                    Pakket = verzekeraar.Pakket,
                });
            }
        }
        public void ExecuteActivateVerzekeringCommand(object obj)
        {
            MessageBoxResult activeQuestion = MessageBox.Show("Wil je deze activeren?", "Activeren", MessageBoxButton.YesNo);
            if (activeQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid verzekeringId))
                {
                    try
                    {
                        commandRepository.ActivateVerzekeraar(verzekeringId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error activating: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    VerzekeringGridData();
                }
                else
                {
                    MessageBox.Show("Invalid ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ExecuteEditVerzekeringCommand(object obj)
        {
            var verzekering = miscellaneousRepository.GetVerzekeraarsById((Guid)obj);

            selectedVerzekering.Id = verzekering.Id;
            selectedVerzekering.Afkorting = verzekering.Afkorting;
            selectedVerzekering.Name = verzekering.Name;
            selectedVerzekering.HasLidnummer = verzekering.HasLidnummer;
            selectedVerzekering.IsHerkomst = verzekering.IsHerkomst;
            selectedVerzekering.IsVerzekeraar = verzekering.IsVerzekeraar;
            selectedVerzekering.CorrespondentieType = verzekering.CorrespondentieType;
            selectedVerzekering.AddressHousenumber = verzekering.AddressHousenumber;
            selectedVerzekering.AddressCity = verzekering.AddressCity;
            selectedVerzekering.AddressStreet = verzekering.AddressStreet;
            selectedVerzekering.AddressHousenumberAddition = verzekering.AddressHousenumberAddition;
            selectedVerzekering.AddressZipCode = verzekering.AddressZipCode;
            selectedVerzekering.PostbusAddress = verzekering.PostbusAddress;
            selectedVerzekering.PostbusName = verzekering.PostbusName;
            selectedVerzekering.FactuurType = System.Net.WebUtility.HtmlDecode(verzekering.FactuurType);
            selectedVerzekering.Pakket = verzekering.Pakket;
            selectedVerzekering.IsOverrideFactuurAdress = verzekering.IsOverrideFactuurAdress;
            selectedVerzekering.Telefoon = verzekering.Telefoon;

            newVerzekering = false;
            IsEditVerzekeringPopupOpen = true;
        }
        public void ExecuteDisableVerzekeringCommand(object obj)
        {
            MessageBoxResult disableQuestion = MessageBox.Show("Wil je deze deactiveren?", "Deactiveren", MessageBoxButton.YesNo);
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
                        MessageBox.Show($"Error disableling: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    VerzekeringGridData();
                }
                else
                {
                    MessageBox.Show("Invalid ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ExecuteSaveVerzekeringCommand(object obj)
        {
            CloseEditVerzekeringPopupCommand.Execute(null);

            if (!newVerzekering)
            {
                try
                {
                    updateRepository.VerzekeringUpdate(SelectedVerzekering);
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627)
                    {
                        MessageBox.Show("De afkorting moet uniek zijn! \r\n" + selectedVerzekering.Afkorting + " bestaat al.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                try
                {
                    createRepository.VerzekeringCreate(SelectedVerzekering);
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627)
                    {
                        MessageBox.Show("De afkorting moet uniek zijn! \r\n" + selectedVerzekering.Afkorting + " bestaat al.");
                        return;
                    }
                    else
                    {
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(sqlEx);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating: {ex.Message}", "Creating Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            VerzekeringGridData();
        }
        public void ExecuteCreateNewVerzekeraar(object obj)
        {
            newVerzekering = true;

            selectedVerzekering.Id = Guid.NewGuid();
            selectedVerzekering.Afkorting = string.Empty;
            selectedVerzekering.Name = string.Empty;
            selectedVerzekering.HasLidnummer = false;
            selectedVerzekering.IsHerkomst = false;
            SelectedVerzekering.IsVerzekeraar = false;
            selectedVerzekering.Pakket = false;
            selectedVerzekering.PostbusAddress = string.Empty;
            selectedVerzekering.PostbusName = string.Empty;
            selectedVerzekering.AddressCity = string.Empty;
            selectedVerzekering.AddressHousenumber = string.Empty;
            selectedVerzekering.AddressHousenumberAddition = string.Empty;
            selectedVerzekering.AddressZipCode = string.Empty;
            selectedVerzekering.AddressStreet = string.Empty;
            selectedVerzekering.FactuurType = string.Empty;
            selectedVerzekering.CorrespondentieType = string.Empty;
            selectedVerzekering.IsOverrideFactuurAdress = false;
            selectedVerzekering.Telefoon = string.Empty;

            IsEditVerzekeringPopupOpen = true;
        }
        public static void ExecuteUploadLogoCommand(object obj)
        {
            new ToastWindow("Afbeelding is geupload.").Show();
            return;
        }
    }
}
