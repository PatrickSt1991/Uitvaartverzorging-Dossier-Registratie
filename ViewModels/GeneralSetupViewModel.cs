using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
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
        public ICommand SaveCommand { get; }
        public ICommand UploadLogoCommand { get; }

        public GeneralSetupViewModel()
        {
            LoadSettingsFromDataProvider();
            SaveCommand = new RelayCommand(SaveSettings);
            UploadLogoCommand = new AdminRelayCommand(obj => UploadImage(obj));
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
                    Debug.WriteLine("Error saving image to database: " + ex.Message);
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
        }
        private void UpdateConnectionString()
        {
            ConnectionString = $"Data Source={DataSource};Database={DatabaseName};User Id={UserId};Password={Password};";
        }
        private void SaveSettings()
        {
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
                }

            };

            string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

            try
            {
                File.WriteAllText("AppConnectionSettings.json", jsonString);

                Process.Start(Environment.ProcessPath);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving settings: {ex.Message}");
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
        }
    }
}
