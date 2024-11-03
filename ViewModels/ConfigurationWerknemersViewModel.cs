using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using static Dossier_Registratie.MainWindow;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationWerknemersViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivateUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DisableUserCommand { get; }
        public ICommand CloseEditUserPopupCommand { get; }
        public ICommand SaveUserCommand { get; }
        public ICommand CreateNewEmployeeCommand { get; }
        public ICommand RefreshGebruikersGridCommand { get; set; }

        private bool isEditUserPopupOpen;
        private bool newEmployee;

        private WerknemersModel selectedUser;
        private ObservableCollection<WerknemersModel> _werknemers;
        private ObservableCollection<WindowsAccount> _werknemerPermissions;
        private ObservableCollection<PermissionsModel> _permissions;
        public WerknemersModel SelectedUser
        {
            get { return selectedUser; }
            set
            {
                if (selectedUser != value)
                {
                    selectedUser = value;
                    OnPropertyChanged(nameof(SelectedUser));
                }
            }
        }
        public ObservableCollection<WerknemersModel> Werknemers
        {
            get { return _werknemers; }
            set
            {
                if (_werknemers != value)
                {
                    _werknemers = value;
                    OnPropertyChanged(nameof(Werknemers));
                }
            }
        }
        public ObservableCollection<WindowsAccount> WerknemersPermissions
        {
            get { return _werknemerPermissions; }
            set
            {
                if (_werknemerPermissions != value)
                {
                    _werknemerPermissions = value;
                    OnPropertyChanged(nameof(WerknemersPermissions));
                }
            }
        }
        public ObservableCollection<PermissionsModel> Permissions
        {
            get { return _permissions; }
            set
            {
                if (_permissions != value)
                {
                    _permissions = value;
                    OnPropertyChanged(nameof(Permissions));
                }
            }
        }
        public bool IsEditUserPopupOpen
        {
            get { return isEditUserPopupOpen; }
            set
            {
                if (isEditUserPopupOpen != value)
                {
                    isEditUserPopupOpen = value;
                    OnPropertyChanged(nameof(IsEditUserPopupOpen));
                }
            }
        }
        public ConfigurationWerknemersViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedUser = new WerknemersModel();
            Werknemers = new ObservableCollection<WerknemersModel>();
            WerknemersPermissions = new ObservableCollection<WindowsAccount>();
            Permissions = new ObservableCollection<PermissionsModel>();

            ActivateUserCommand = new ViewModelCommand(ExecuteActivateUserCommand);
            EditUserCommand = new ViewModelCommand(ExecuteEditUserCommand);
            DisableUserCommand = new ViewModelCommand(ExecuteDisableUserCommand);
            CloseEditUserPopupCommand = new RelayCommand(() => IsEditUserPopupOpen = false);
            SaveUserCommand = new ViewModelCommand(ExecuteSaveUserCommand);
            CreateNewEmployeeCommand = new ViewModelCommand(ExecuteCreateNewEmployeeCommand);

            GetPermissions();
            WerknemersGridData();
        }
        public void ExecuteActivateUserCommand(object obj)
        {
            MessageBoxResult activeQuestion = MessageBox.Show("Wil je deze werknemer activeren?", "Werknemer activeren", MessageBoxButton.YesNo);
            if (activeQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid employeeId))
                {
                    try
                    {
                        commandRepository.ActivateEmployee(employeeId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error activating employee: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    WerknemersGridData();
                }
                else
                {
                    MessageBox.Show("Invalid employee ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ExecuteEditUserCommand(object obj)
        {
            var werknemer = miscellaneousRepository.GetWerknemer((Guid)obj);

            SelectedUser.Id = werknemer.Id;
            selectedUser.Initialen = werknemer.Initialen;
            selectedUser.Voornaam = werknemer.Voornaam;
            selectedUser.Achternaam = werknemer.Achternaam;
            selectedUser.Tussenvoegsel = werknemer.Tussenvoegsel;
            selectedUser.Roepnaam = werknemer.Roepnaam;
            selectedUser.Geboorteplaats = werknemer.Geboorteplaats;
            selectedUser.Geboortedatum = werknemer.Geboortedatum?.Date;
            selectedUser.Email = werknemer.Email;
            selectedUser.IsUitvaartverzorger = werknemer.IsUitvaartverzorger;
            selectedUser.IsDrager = werknemer.IsDrager;
            selectedUser.IsChauffeur = werknemer.IsChauffeur;
            selectedUser.IsOpbaren = werknemer.IsOpbaren;

            foreach (var userPermission in miscellaneousRepository.GetWerknemerPermissions(werknemer.Id))
            {
                selectedUser.PermissionId = userPermission.PermissionId;
                selectedUser.PermissionName = userPermission.PermissionName;
            }

            newEmployee = false;
            IsEditUserPopupOpen = true;
        }
        public void ExecuteDisableUserCommand(object obj)
        {
            MessageBoxResult disableQuestion = MessageBox.Show("Wil je deze werknemer deactiveren?", "Werknemer deactiveren", MessageBoxButton.YesNo);
            if (disableQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid employeeId))
                {
                    try
                    {
                        commandRepository.DisableEmployee(employeeId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error disablelign employee: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    WerknemersGridData();
                }
                else
                {
                    MessageBox.Show("Invalid employee ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ExecuteSaveUserCommand(object obj)
        {
            Debug.WriteLine(SelectedUser.HasData());
            if (!SelectedUser.HasData())
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            CloseEditUserPopupCommand.Execute(null);

            if (!newEmployee)
            {
                try
                {
                    updateRepository.EmployeeUpdate(SelectedUser);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating employee: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
                try
                {
                    updateRepository.EditRechten(selectedUser.Id, selectedUser.PermissionId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating rechten: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                try
                {
                    createRepository.EmployeeCreate(SelectedUser);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating employee: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }

                try
                {
                    createRepository.CreateUserPermission(selectedUser.Id, selectedUser.PermissionId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating user permissions: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            WerknemersGridData();
        }
        public void ExecuteCreateNewEmployeeCommand(object obj)
        {
            newEmployee = true;

            SelectedUser.Id = Guid.NewGuid();
            selectedUser.Initialen = string.Empty;
            selectedUser.Voornaam = string.Empty;
            selectedUser.Achternaam = string.Empty;
            selectedUser.Tussenvoegsel = string.Empty;
            selectedUser.Roepnaam = string.Empty;
            selectedUser.Geboorteplaats = string.Empty;
            selectedUser.Geboortedatum = DateTime.MinValue;
            selectedUser.Email = string.Empty;
            selectedUser.IsUitvaartverzorger = false;
            selectedUser.IsDrager = false;
            selectedUser.IsChauffeur = false;

            IsEditUserPopupOpen = true;
        }
        public void WerknemersGridData()
        {
            Werknemers.Clear();
            WerknemersPermissions.Clear();

            foreach (var werknemer in miscellaneousRepository.GetWerknemers())
            {
                WerknemersModel werknemersModel = new WerknemersModel
                {
                    Id = werknemer.Id,
                    Initialen = werknemer.Initialen,
                    Voornaam = werknemer.Voornaam,
                    Roepnaam = werknemer.Roepnaam,
                    Tussenvoegsel = werknemer.Tussenvoegsel,
                    Achternaam = werknemer.Achternaam,
                    Geboorteplaats = werknemer.Geboorteplaats,
                    Geboortedatum = werknemer.Geboortedatum?.Date,
                    Email = werknemer.Email,
                    IsDeleted = werknemer.IsDeleted,
                    IsUitvaartverzorger = werknemer.IsUitvaartverzorger,
                    IsDrager = werknemer.IsDrager,
                    IsChauffeur = werknemer.IsChauffeur,
                    BtnBrush = werknemer.BtnBrush,
                    PermissionName = string.Empty
                };

                foreach (var werknemerPermission in miscellaneousRepository.GetWerknemerPermissions(werknemer.Id))
                {
                    if (werknemerPermission.PermissionId != Guid.Empty)
                    {
                        werknemersModel.PermissionName = werknemerPermission.PermissionName;
                    }
                }

                Werknemers.Add(werknemersModel);
            }

        }
        public void GetPermissions()
        {
            var permissionsFromRepo = miscellaneousRepository.GetPermissions();

            foreach (var permission in permissionsFromRepo)
            {
                // Add permission only if it is enabled and doesn't need to be excluded
                if (permission.IsEnabled)
                {
                    bool shouldAddPermission = true;

                    // Exclude specific permissions if the condition is met
                    if (Globals.PermissionLevelName != "System")
                    {
                        if (permission.PermissionName == "System" || permission.PermissionName == "Financieel")
                        {
                            shouldAddPermission = false;
                        }
                    }

                    // Add the permission if it passes the checks
                    if (shouldAddPermission)
                    {
                        Permissions.Add(new PermissionsModel()
                        {
                            Id = permission.Id,
                            PermissionName = permission.PermissionName,
                            IsEnabled = permission.IsEnabled
                        });
                    }
                }
            }
        }
    }
}
