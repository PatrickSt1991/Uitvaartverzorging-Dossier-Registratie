using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Dossier_Registratie.MainWindow;

namespace Dossier_Registratie.ViewModels
{
    public class GeneralSetupViewModel : ViewModelBase
    {
        private string _organizationName;
        private string _organizationStreet;
        private string _organizationHousenumber;
        private string _organizationHousenumberAddition;
        private string _organizationZipcode;
        private string _organizationCity;
        private string _organizationPhonenumber;
        private string _organizationEmail;
        private string _organizationIban;
        private string _dataSource;
        private string _databaseName;
        private string _userId;
        private string _password;
        private string _connectionString;
        private string _shutdownFile;
        private string _access2023Db;
        private string _access2024Db;
        private string _archiveFolder;
        private string _templateFolder;
        private string _docuSaveFolder;
        private string _billSaveFolder;
        private string _systemTitle;
        private bool _githubEnabled;
        private string _githubKey;
        private string _githubOwner;
        private string _githubRepo;
        private string _githubProduct;
        private bool _maintenanceEnabled;
        private string _maintenanceUrl;
        private string _maintenanceUsername;
        private string _maintenancePassword;
        private string _applicationName;
        private string _configurationTitle;
        private bool _smtpEnabled;
        private string _smtpUsername;
        private string _smtpPassword;
        private string _smtpServer;
        private int _smtpPort;
        private string _smtpReciever;
        private string saveReboot = "Opslaan en opnieuw opstarten";

        public string OrganizationName
        {
            get => _organizationName;
            set
            {
                _organizationName = value;
                OnPropertyChanged(nameof(OrganizationName));
            }
        }
        public string OrganizationStreet
        {
            get => _organizationStreet;
            set
            {
                _organizationStreet = value;
                OnPropertyChanged(nameof(OrganizationStreet));
            }
        }
        public string OrganizationHousenumber
        {
            get => _organizationHousenumber;
            set
            {
                _organizationHousenumber = value;
                OnPropertyChanged(nameof(OrganizationHousenumber));
            }
        }
        public string OrganizationHousenumberAddition
        {
            get => _organizationHousenumberAddition;
            set
            {
                _organizationHousenumberAddition = value;
                OnPropertyChanged(nameof(OrganizationHousenumberAddition));
            }
        }
        public string OrganizationZipcode
        {
            get => _organizationZipcode;
            set
            {
                _organizationZipcode = value;
                OnPropertyChanged(nameof(OrganizationZipcode));
            }
        }
        public string OrganizationCity
        {
            get => _organizationCity;
            set
            {
                _organizationCity = value;
                OnPropertyChanged(nameof(OrganizationCity));
            }
        }
        public string OrganizationPhonenumber
        {
            get => _organizationPhonenumber;
            set
            {
                _organizationPhonenumber = value;
                OnPropertyChanged(nameof(OrganizationPhonenumber));
            }
        }
        public string OrganizationEmail
        {
            get => _organizationEmail;
            set
            {
                _organizationEmail = value;
                OnPropertyChanged(nameof(OrganizationEmail));
            }
        }
        public string OrganizationIban
        {
            get => _organizationIban;
            set
            {
                _organizationIban = value;
                OnPropertyChanged(nameof(OrganizationIban));
            }
        }
        public string DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                OnPropertyChanged(nameof(DataSource));
                UpdateConnectionString();
            }
        }
        public string DatabaseName
        {
            get => _databaseName;
            set
            {
                _databaseName = value;
                OnPropertyChanged(nameof(DatabaseName));
                UpdateConnectionString();
            }
        }
        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
                UpdateConnectionString();
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                UpdateConnectionString();
            }
        }
        public string ConnectionString
        {
            get => _connectionString;
            private set
            {
                _connectionString = value;
                OnPropertyChanged(nameof(ConnectionString));
            }
        }
        public string ShutdownFile
        {
            get => _shutdownFile;
            set
            {
                _shutdownFile = value;
                OnPropertyChanged(nameof(ShutdownFile));
            }
        }
        public string Access2023Db
        {
            get => _access2023Db;
            set
            {
                _access2023Db = value;
                OnPropertyChanged(nameof(Access2023Db));
            }
        }
        public string Access2024Db
        {
            get => _access2024Db;
            set
            {
                _access2024Db = value;
                OnPropertyChanged(nameof(Access2024Db));
            }
        }
        public string ArchiveFolder
        {
            get => _archiveFolder;
            set
            {
                _archiveFolder = value;
                OnPropertyChanged(nameof(ArchiveFolder));
            }
        }
        public string TemplateFolder
        {
            get => _templateFolder;
            set
            {
                _templateFolder = value;
                OnPropertyChanged(nameof(TemplateFolder));
            }
        }
        public string DocuSaveFolder
        {
            get => _docuSaveFolder;
            set
            {
                _docuSaveFolder = value;
                OnPropertyChanged(nameof(DocuSaveFolder));
            }
        }
        public string BillSaveFolder
        {
            get => _billSaveFolder;
            set
            {
                _billSaveFolder = value;
                OnPropertyChanged(nameof(BillSaveFolder));
            }
        }
        public string SystemTitle
        {
            get => _systemTitle;
            set
            {
                _systemTitle = value;
                OnPropertyChanged(nameof(SystemTitle));
            }
        }
        public bool GithubEnabled
        {
            get => _githubEnabled;
            set
            {
                _githubEnabled = value;
                OnPropertyChanged(nameof(GithubEnabled));

                if (!_githubEnabled)
                {
                    SmtpEnabled = false;
                    SmtpUsername = string.Empty;
                    SmtpPassword = string.Empty;
                    SmtpServer = string.Empty;
                    SmtpPort = 0;
                    SmtpReciever = string.Empty;
                }
            }
        }
        public string GithubKey
        {
            get => _githubKey;
            set
            {
                _githubKey = value;
                OnPropertyChanged(nameof(GithubKey));
            }
        }
        public string GithubOwner
        {
            get => _githubOwner;
            set
            {
                _githubOwner = value;
                OnPropertyChanged(nameof(GithubOwner));
            }
        }
        public string GithubRepo
        {
            get => _githubRepo;
            set
            {
                _githubRepo = value;
                OnPropertyChanged(nameof(GithubRepo));
            }
        }
        public string GithubProduct
        {
            get => _githubProduct;
            set
            {
                _githubProduct = value;
                OnPropertyChanged(nameof(GithubProduct));
            }
        }
        public bool MaintenanceEnabled
        {
            get => _maintenanceEnabled;
            set
            {
                _maintenanceEnabled = value;
                OnPropertyChanged(nameof(MaintenanceEnabled));
            }
        }
        public string MaintenanceUrl
        {
            get => _maintenanceUrl;
            set
            {
                _maintenanceUrl = value;
                OnPropertyChanged(nameof(MaintenanceUrl));
            }
        }
        public string MaintenanceUsername
        {
            get => _maintenanceUsername;
            set
            {
                _maintenanceUsername = value;
                OnPropertyChanged(nameof(MaintenanceUsername));
            }
        }
        public string MaintenancePassword
        {
            get => _maintenancePassword;
            set
            {
                _maintenancePassword = value;
                OnPropertyChanged(nameof(MaintenancePassword));
            }
        }

        public string ApplicationName
        {
            get => _applicationName;
            set
            {
                _applicationName = string.IsNullOrEmpty(value) ? "DigiGraf" : value;
                OnPropertyChanged(nameof(ApplicationName));
            }
        }
        public string ConfigurationTitle
        {
            get => _configurationTitle;
            set
            {
                _configurationTitle = value;
                OnPropertyChanged(nameof(ConfigurationTitle));
            }
        }
        public bool SmtpEnabled
        {
            get => _smtpEnabled;
            set
            {
                if (GithubEnabled)
                {
                    _smtpEnabled = value;
                    OnPropertyChanged(nameof(SmtpEnabled));
                }
            }
        }
        public string SmtpUsername
        {
            get => _smtpUsername;
            set
            {
                _smtpUsername = value;
                OnPropertyChanged(nameof(SmtpUsername));
            }
        }
        public string SmtpPassword
        {
            get => _smtpPassword;
            set
            {
                _smtpPassword = value;
                OnPropertyChanged(nameof(SmtpPassword));
            }
        }
        public string SmtpServer
        {
            get => _smtpServer;
            set
            {
                _smtpServer = value;
                OnPropertyChanged(nameof(SmtpServer));
            }
        }
        public int SmtpPort
        {
            get => _smtpPort;
            set
            {
                _smtpPort = value;
                OnPropertyChanged(nameof(SmtpPort));
            }
        }
        public string SmtpReciever
        {
            get => _smtpReciever;
            set
            {
                _smtpReciever = value;
                OnPropertyChanged(nameof(SmtpReciever));
            }
        }
        public string SaveReboot
        {
            get => saveReboot;
            set
            {
                saveReboot = value;
                OnPropertyChanged(nameof(SaveReboot));
            }
        }
        public ICommand SaveCommand { get; }
        public ICommand UploadLogoCommand { get; }

        public GeneralSetupViewModel()
        {
            LoadSettingsFromDataProvider();
            ConfigurationTitle = ApplicationName + " Configuratie";
            SaveCommand = new RelayCommand(SaveSettings);
            UploadLogoCommand = new AdminRelayCommand(obj => UploadImage(obj));

            if (DataProvider.SetupComplete)
                SaveReboot = "Opslaan";
        }
        private static async Task UploadImage(object appType)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select an image",
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var fileExtension = Path.GetExtension(filePath)?.TrimStart('.') ?? "unknown";

                byte[] imageData = File.ReadAllBytes(filePath);

                try
                {
                    var searchRepo = new SearchOperations();
                    bool appBlobCheck = await searchRepo.SearchBlobLogo(appType.ToString());

                    if (appBlobCheck)
                    {
                        var updateRepo = new UpdateOperations();
                        await updateRepo.UpdateBlobLogo(filePath, fileExtension, imageData, appType.ToString());
                    }
                    else
                    {
                        var createRepo = new CreateOperations();
                        await createRepo.InsertBlobLogo(filePath, fileExtension, imageData, appType.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                }
            }
        }
        private void LoadSettingsFromDataProvider()
        {
            var builder = new SqlConnectionStringBuilder(DataProvider.ConnectionString);

            OrganizationName = DataProvider.OrganizationName;
            OrganizationCity = DataProvider.OrganizationCity;
            OrganizationEmail = DataProvider.OrganizationEmail;
            OrganizationHousenumber = DataProvider.OrganizationHouseNumber;
            OrganizationHousenumberAddition = DataProvider.OrganizationHouseNumberAddition;
            OrganizationIban = DataProvider.OrganizationIban;
            OrganizationPhonenumber = DataProvider.OrganizationPhoneNumber;
            OrganizationStreet = DataProvider.OrganizationStreet;
            OrganizationZipcode = DataProvider.OrganizationZipcode;
            SystemTitle = DataProvider.SystemTitle;
            DataSource = builder.DataSource;
            DatabaseName = builder.InitialCatalog;
            UserId = builder.UserID;
            Password = builder.Password;
            ShutdownFile = DataProvider.ShutdownFile;
            Access2023Db = DataProvider.AccessDatabase2023;
            Access2024Db = DataProvider.AccessDatabase2024;
            ArchiveFolder = DataProvider.PdfArchiveBaseFolder;
            TemplateFolder = DataProvider.TemplateFolder;
            DocuSaveFolder = DataProvider.DocumentenOpslag;
            BillSaveFolder = DataProvider.FactuurOpslag;
            GithubEnabled = DataProvider.GithubEnabled;
            GithubKey = DataProvider.GithubKey;
            GithubOwner = DataProvider.GithubOwner;
            GithubRepo = DataProvider.GithubRepo;
            GithubProduct = DataProvider.GithubProduct;
            MaintenanceEnabled = DataProvider.MaintenanceCheckEnabled;
            MaintenanceUrl = DataProvider.MaintenanceUrl;
            MaintenanceUsername = DataProvider.MaintenanceUser;
            MaintenancePassword = DataProvider.MaintenancePassword;
            ApplicationName = DataProvider.ApplicationName;
            SmtpEnabled = DataProvider.SmtpEnabled;
            SmtpPassword = DataProvider.SmtpPassword;
            SmtpUsername = DataProvider.SmtpUsername;
            SmtpPort = DataProvider.SmtpPort;
            SmtpServer = DataProvider.SmtpServer;
            SmtpReciever = DataProvider.SmtpReciever;
        }
        private void UpdateConnectionString()
        {
            ConnectionString = $"Data Source={DataSource};Database={DatabaseName};User Id={UserId};Password={Password};";
        }
        private static bool TestDatabaseConnection(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    new ToastWindow("Database connectie geslaagd").Show();
                    return true;
                }
            }
            catch
            {
                new ToastWindow("Database connectie mislukt, controleer de instellingen").Show();
                return false;
            }
        }
        private static bool SetupInstallUser()
        {
            var newUser = CreateNewUser();
            ICreateOperations createRepository = new CreateOperations();
            Guid personnelId = newUser.Id;

            if (!CreateEmployee(createRepository, newUser))
                return false;

            if (!CreateWindowsAccount(createRepository, personnelId))
                return false;

            Globals.PermissionLevelId = "D8454762-9245-4B6C-9D29-293B9BC2FFB2";
            Globals.PermissionLevelName = "System";

            if (!CreateUserPermission(createRepository, personnelId, Guid.Parse(Globals.PermissionLevelId)))
                return false;

            return true;
        }
        private static WerknemersModel CreateNewUser()
        {
            return new WerknemersModel
            {
                Id = Guid.NewGuid(),
                Initialen = "S",
                Voornaam = "Systeem",
                Roepnaam = "Beheerder",
                Tussenvoegsel = string.Empty,
                Achternaam = "Beheerder",
                VolledigeNaam = "Systeem Beheeder",
                Geboortedatum = DateTime.Today,
                Geboorteplaats = string.Empty,
                Email = DataProvider.OrganizationEmail,
                IsDeleted = false,
                IsUitvaartverzorger = false,
                IsDrager = false,
                IsChauffeur = false,
                IsOpbaren = false,
                PermissionId = Guid.Parse("D8454762-9245-4B6C-9D29-293B9BC2FFB2"),
                PermissionName = "System"
            };
        }
        private static bool CreateEmployee(ICreateOperations createRepository, WerknemersModel user)
        {
            try
            {
                createRepository.EmployeeCreate(user);
                return true;
            }
            catch (Exception ex)
            {
                ShowError("Error creating system account", ex);
                return false;
            }
        }
        private static bool CreateWindowsAccount(ICreateOperations createRepository, Guid personnelId)
        {
            try
            {
                createRepository.CreateWindowsUser(Guid.NewGuid(), personnelId, Environment.UserName);
                return true;
            }
            catch (Exception ex)
            {
                ShowError("Error creating system user", ex);
                return false;
            }
        }
        private static bool CreateUserPermission(ICreateOperations createRepository, Guid personnelId, Guid permissionLevelId)
        {
            try
            {
                createRepository.CreateUserPermission(personnelId, permissionLevelId);
                return true;
            }
            catch (Exception ex)
            {
                ShowError("Error creating system user permission", ex);
                return false;
            }
        }
        private static void ShowError(string message, Exception ex)
        {
            new ToastWindow($"{message}: {ex.Message}").Show();
            MessageBox.Show($"{message}: {ex.Message}", "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void SaveSettings()
        {

            if (!TestDatabaseConnection(ConnectionString))
                return;

            if (!DataProvider.SetupComplete)
                if (!SetupInstallUser())
                    return;

            var config = new
            {
                ConnectionStrings = new
                {
                    DossierRegistratieConnectionString = ConnectionString
                },
                CompanySettings = new
                {
                    DataProvider.OrganizationName,
                    DataProvider.OrganizationStreet,
                    DataProvider.OrganizationHouseNumber,
                    DataProvider.OrganizationHouseNumberAddition,
                    DataProvider.OrganizationZipcode,
                    DataProvider.OrganizationCity,
                    DataProvider.OrganizationPhoneNumber,
                    DataProvider.OrganizationEmail,
                    DataProvider.OrganizationIban
                },
                SystemSettings = new
                {
                    DataProvider.SystemTitle,
                    DataProvider.ApplicationName,
                    DataProvider.GithubKey,
                    DataProvider.GithubOwner,
                    DataProvider.GithubRepo,
                    DataProvider.GithubProduct,
                    DataProvider.GithubEnabled,
                    SetupComplete = true
                },
                SystemConfiguration = new
                {
                    ShutdownFile,
                    Database2024 = Access2024Db,
                    Database2023 = Access2023Db,
                    PdfArchive = ArchiveFolder,
                    TemplateFolder,
                    DocumentenOpslag = DocuSaveFolder,
                    FactuurOpslag = BillSaveFolder
                },
                MaintenanceConfiguration = new
                {
                    MaintenanceCheckEnabled = MaintenanceEnabled,
                    MaintenanceUrl,
                    MaintenanceUsername,
                    MaintenancePassword
                },
                SmtpConfiguration = new
                {
                    DataProvider.SmtpEnabled,
                    DataProvider.SmtpServer,
                    DataProvider.SmtpPort,
                    DataProvider.SmtpUsername,
                    DataProvider.SmtpPassword,
                    DataProvider.SmtpReciever
                }
            };

            string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                File.WriteAllText("AppConnectionSettings.json", jsonString);

                if (!DataProvider.SetupComplete)
                {
                    Process.Start(Environment.ProcessPath);
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
        }
    }
}
