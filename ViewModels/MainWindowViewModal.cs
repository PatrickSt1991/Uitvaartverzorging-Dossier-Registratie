using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using static Dossier_Registratie.ViewModels.OverledeneAsbestemmingViewModel;
using static Dossier_Registratie.ViewModels.OverledeneBijlagesViewModel;
using static Dossier_Registratie.ViewModels.OverledeneExtraInfoViewModal;
using static Dossier_Registratie.ViewModels.OverledeneFactuurViewModel;
using static Dossier_Registratie.ViewModels.OverledeneOpbarenViewModel;
using static Dossier_Registratie.ViewModels.OverledeneSteenhouwerijViewModel;
using static Dossier_Registratie.ViewModels.OverledeneUitvaartViewModel;
using static Dossier_Registratie.ViewModels.OverledeneVerzekeringViewModal;
using static Dossier_Registratie.ViewModels.OverledeneViewModel;

namespace Dossier_Registratie.ViewModels
{
    public class MainWindowViewModal : ViewModelBase
    {
        private ImageSource _imageSource;
        private OverledeneNotification notificatieWindow;
        private string _zoekenUitvaartnummer = null;
        private string _ZoekenAchternaam = null;
        private string _ZoekenDoB = null;
        private string _errorMessageUitvaartnummer = "Uitvaartnummer is verplicht";
        private string _errorMessageSurname = "* Verplicht veld";
        private string _currentTime;
        private string _archivePath;
        private string _Title;
        private string _selectedComboBoxItem;
        private string _applicationUnavailable;
        private string _versionLabel = "Dossier Registratie - Versie: 3";
        private string _copyrightText = "© " + DateTime.Now.ToString("yyyy") + " - Patrick Stel - All Rights Reserved - " +
                                "Licensed under GNU Affero General Public License v3.0 ";

        private int _selectedIndex;

        private bool _isSearchVisible = false;
        private bool _isUitvaartnumberVisible = false;
        private bool _isSearchResultVisible = false;
        private bool isCreateUserPopupOpen;
        private bool _uitvaartNummerEnabled = false;
        private bool _isUnderMaintenance;
        private bool _maintenanceDisabled = true;
        private bool _searchOldDatabaseSurname = false;
        private bool _searchOldDatabaseNummer = false;
        private bool _searchArchiveFolder = false;
        private bool _archiveSearchResult = false;
        private bool _notificationYearVisable = false;
        private Visibility _beheerButtonVisable = Visibility.Hidden;

        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private ObservableCollection<OverledeneSearchSurname> _searchUitvaartSurname;
        private List<OverledeneSearchSurname> combinedResults;
        private WerknemersModel newUser;
        private ObservableCollection<NotificatieOverzichtModel> _yearPassedNotification;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }
        public WerknemersModel NewUser
        {
            get { return newUser; }
            set
            {
                if (newUser != value)
                {
                    newUser = value;
                    OnPropertyChanged(nameof(NewUser));
                }
            }
        }
        public ObservableCollection<OverledeneSearchSurname> SearchUitvaartSurname
        {
            get => _searchUitvaartSurname;
            set
            {
                _searchUitvaartSurname = value;
                OnPropertyChanged(nameof(SearchUitvaartSurname));
            }
        }
        public ObservableCollection<NotificatieOverzichtModel> YearPassedNotification
        {
            get => _yearPassedNotification;
            set
            {
                _yearPassedNotification = value;
                OnPropertyChanged(nameof(YearPassedNotification));
            }
        }
        public string ZoekenUitvaartnummer
        {
            get => _zoekenUitvaartnummer;
            set
            {
                _zoekenUitvaartnummer = value;
                OnPropertyChanged(nameof(ZoekenUitvaartnummer));
            }
        }
        public string ZoekenAchternaam
        {
            get => _ZoekenAchternaam;
            set
            {
                _ZoekenAchternaam = value;
                OnPropertyChanged(nameof(ZoekenAchternaam));
            }
        }
        public string ZoekenDoB
        {
            get => _ZoekenDoB;
            set
            {
                _ZoekenDoB = value;
                OnPropertyChanged(nameof(ZoekenDoB));
            }
        }
        public string ErrorMessageUitvaartnummer
        {
            get
            {
                return _errorMessageUitvaartnummer;
            }
            set
            {
                _errorMessageUitvaartnummer = value;
                OnPropertyChanged(nameof(ErrorMessageUitvaartnummer));
            }
        }
        public string ErrorMessageSurname
        {
            get
            {
                return _errorMessageSurname;
            }
            set
            {
                _errorMessageSurname = value;
                OnPropertyChanged(nameof(ErrorMessageSurname));
            }
        }
        public string CurrentTime
        {
            get { return _currentTime; }
            set { _currentTime = value; OnPropertyChanged(nameof(CurrentTime)); }
        }
        public string VersionLabel
        {
            get { return _versionLabel; }
            set { _versionLabel = value; OnPropertyChanged(nameof(VersionLabel)); }
        }
        public string ApplicationUnavailable
        {
            get { return _applicationUnavailable; }
            set
            {
                _applicationUnavailable = value;
                OnPropertyChanged(nameof(ApplicationUnavailable));
            }
        }
        public string CopyrightText
        {
            get { return _copyrightText; }
            set { _copyrightText = value; OnPropertyChanged(nameof(CopyrightText)); }
        }
        public bool UitvaartNummerEnabled
        {
            get { return _uitvaartNummerEnabled; }
            set
            {
                _uitvaartNummerEnabled = value;
                OnPropertyChanged(nameof(UitvaartNummerEnabled));
            }
        }
        public string SelectedComboBoxItem
        {
            get { return _selectedComboBoxItem; }
            set
            {
                if (_selectedComboBoxItem != value)
                {
                    _selectedComboBoxItem = value;
                    OnPropertyChanged(nameof(SelectedComboBoxItem));
                }
            }
        }
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        public bool IsSearchVisible
        {
            get { return _isSearchVisible; }
            set { _isSearchVisible = value; OnPropertyChanged(nameof(IsSearchVisible)); }
        }
        public bool IsSearchResultVisible
        {
            get
            {
                return _isSearchResultVisible;
            }
            set
            {
                _isSearchResultVisible = value;
                OnPropertyChanged(nameof(IsSearchResultVisible));
            }
        }
        public bool IsUitvaartnumberVisible
        {
            get
            {
                return _isUitvaartnumberVisible;
            }
            set
            {
                _isUitvaartnumberVisible = value;
                OnPropertyChanged(nameof(IsUitvaartnumberVisible));
            }
        }
        public bool IsCreateUserPopupOpen
        {
            get { return isCreateUserPopupOpen; }
            set
            {
                if (isCreateUserPopupOpen != value)
                {
                    isCreateUserPopupOpen = value;
                    OnPropertyChanged(nameof(IsCreateUserPopupOpen));
                }
            }
        }
        public bool IsUnderMaintenance
        {
            get => _isUnderMaintenance;
            set
            {
                _isUnderMaintenance = value;
                OnPropertyChanged(nameof(IsUnderMaintenance));
            }
        }
        public bool MaintenanceDisabled
        {
            get { return _maintenanceDisabled; }
            set
            {
                _maintenanceDisabled = value;
                OnPropertyChanged(nameof(MaintenanceDisabled));
            }
        }
        public bool SearchOldDatabaseSurname
        {
            get { return _searchOldDatabaseSurname; }
            set { _searchOldDatabaseSurname = value; OnPropertyChanged(nameof(SearchOldDatabaseSurname)); }
        }
        public bool SearchOldDatabaseNummer
        {
            get { return _searchOldDatabaseNummer; }
            set { _searchOldDatabaseNummer = value; OnPropertyChanged(nameof(SearchOldDatabaseNummer)); }
        }
        public bool SearchArchiveFolder
        {
            get { return _searchArchiveFolder; }
            set { _searchArchiveFolder = value; OnPropertyChanged(nameof(SearchArchiveFolder)); }
        }
        public bool ArchiveSearchResult
        {
            get { return _archiveSearchResult; }
            set { _archiveSearchResult = value; OnPropertyChanged(nameof(ArchiveSearchResult)); }
        }
        public bool NotificationYearVisable
        {
            get => _notificationYearVisable;
            set
            {
                _notificationYearVisable = value; OnPropertyChanged(nameof(NotificationYearVisable));
            }
        }
        public Visibility BeheerButtonVisable
        {
            get { return _beheerButtonVisable; }
            set { _beheerButtonVisable = value; OnPropertyChanged(nameof(BeheerButtonVisable)); }
        }
        public string ArchivePath
        {
            get { return _archivePath; }
            set { _archivePath = value; OnPropertyChanged(nameof(ArchivePath)); }
        }
        public string Title
        {
            get { return _Title; }
            set { _Title = value; OnPropertyChanged(nameof(Title)); }
        }
        private void OnDataReceived(int indexNumber)
        {
            if (indexNumber == 666)
            {
                CheckTabControl();
                SelectedIndex = 1;
            }
            else
            {
                if (indexNumber == 0)
                {
                    Instance.CreateNewDossier();
                    VerzekeringInstance.CreateNewDossier();
                    UitvaartInstance.CreateNewDossier();
                    SteenhouwerijInstance.CreateNewDossier();
                    OpbarenInstance.CreateNewDossier();
                    KostenbegrotingInstance.CreateNewDossier();
                    ExtraInfoInstance.CreateNewDossier();
                    BijlagesInstance.CreateNewDossier();
                    AsbestemmingInstance.CreateNewDossier();
                }

                SelectedIndex = indexNumber;
            }
        }
        private void ResetCombobox()
        {
            SelectedComboBoxItem = null;
        }
        public ICommand NieuwDossierAanmakenCommand { get; }
        public ICommand SearchUitvaartnummerCommand { get; }
        public ICommand SearchAchternaamCommand { get; }
        public ICommand OpenAchternaamCommand { get; }
        public ICommand CreateNewUserCommand { get; }
        public ICommand ClearAllModelsCommand { get; }

        public MainWindowViewModal()
        {
            NewUser = new WerknemersModel();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            combinedResults = new List<OverledeneSearchSurname>();

            string MainApplicationName = !string.IsNullOrEmpty(DataProvider.ApplicationName) ? "DigiGraf" : DataProvider.ApplicationName;
            ApplicationUnavailable = MainApplicationName + " is op het moment niet beschikbaar vanwege onderhoud.";

            LoadImageFromDatabase();
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Title = DataProvider.OrganizationName + " - " + DataProvider.SystemTitle;

            if (!string.IsNullOrEmpty(DataProvider.PdfArchiveBaseFolder))
            {
                ArchivePath = DataProvider.PdfArchiveBaseFolder + DateTime.Now.Year.ToString();
                if (!Directory.Exists(ArchivePath))
                    ArchivePath = DataProvider.PdfArchiveBaseFolder + (DateTime.Now.Year - 1);
            }

            if (DataProvider.MaintenanceCheckEnabled)
                CheckMaintenanceWindow();

            DeceasedYearAgoCheck();

            switch (Globals.PermissionLevelId.ToUpper())
            {
                case "A224C94E-2F54-4D43-A976-11E24287A8E0": //Beheerder
                case "D8454762-9245-4B6C-9D29-293B9BC2FFB2": //System
                case "D3BD7AE6-978D-4F1A-972C-B033CFC801E3": //Uitvaartleider - Limited
                case "8DBB3112-153D-4592-ABE2-77C79D61F81A": //Financieel
                    BeheerButtonVisable = Visibility.Visible;
                    break;
            }

            VersionLabel = DataProvider.SystemTitle + " - Versie: " + version;
            
            UpdateTime();
            CheckTabControl();
            ComboAggregator.OnDataTransmitted += ResetCombobox;
            IntAggregator.OnDataTransmitted += OnDataReceived;

            SearchUitvaartnummerCommand = new ViewModelCommand(ExecuteSearchUitvaartnummerCommand, CanExecuteSearchUitvaartnummerCommand);
            SearchAchternaamCommand = new ViewModelCommand(ExecuteSearchAchternaamCommand, CanExecuteSearchAchternaamCommand);
            NieuwDossierAanmakenCommand = new ViewModelCommand(ExecuteCreateNewRegistrationCommand);
            OpenAchternaamCommand = new ViewModelCommand(ExecuteOpenAchternaamCommand);
            CreateNewUserCommand = new ViewModelCommand(ExecuteCreateNewUserCommand);
            ClearAllModelsCommand = new ViewModelCommand(ExecuteClearAllModels, CanExecuteClearAllModels);
        }
        public async Task CheckMaintenanceWindow()
        {
            using (var httpClient = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{DataProvider.MaintenanceUser}:{DataProvider.MaintenancePassword}");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                try
                {
                    var response = await httpClient.GetAsync(DataProvider.MaintenanceUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var xmlContent = await response.Content.ReadAsStringAsync();
                        var xmlDoc = XDocument.Parse(xmlContent);
                        var maintenanceValue = xmlDoc.Root.Element("maintenance")?.Value;
                        IsUnderMaintenance = maintenanceValue != null && maintenanceValue.Equals("true", StringComparison.OrdinalIgnoreCase);

                        if (IsUnderMaintenance == true)
                            MaintenanceDisabled = false;
                    }
                    else
                    {
                        IsUnderMaintenance = false;
                        MaintenanceDisabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching maintenance window: {ex.Message}");
                    IsUnderMaintenance = false;
                    MaintenanceDisabled = true;
                }
            }
        }
        static async Task<List<OverledeneSearchSurname>> SearchAccessDatabaseOnUitvaartNumberAsync(string connectionString, string searchNumber, string db)
        {
            List<OverledeneSearchSurname> records = new List<OverledeneSearchSurname>();

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                await connection.OpenAsync(); // Open the connection asynchronously
                string query = "SELECT Uitvaartnummer, [1 Naam overledene] as achternaam, [1 Tussenvoegsel] as tussenvoegsel, " +
                                "[1 Voornamen] as voornaam, [1 Geboortedatum] as geboortedatum, [1 Aanhef] as aanhef, Uitvaartverzorger FROM data WHERE Uitvaartnummer = ?";

                using (OdbcCommand command = new OdbcCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@param1", searchNumber); // ODBC uses named parameters or placeholders

                    // Use DbDataReader which works with OdbcCommand as well
                    using (DbDataReader reader = await command.ExecuteReaderAsync()) // Execute asynchronously
                    {
                        while (await reader.ReadAsync()) // Read each record asynchronously
                        {
                            OverledeneSearchSurname record = new OverledeneSearchSurname
                            {
                                UitvaartId = Guid.Empty,
                                UitvaartNummer = "archief_" + db + "_" + reader["Uitvaartnummer"]?.ToString() ?? string.Empty,
                                OverledeneAanhef = reader["aanhef"]?.ToString() ?? string.Empty,
                                OverledeneVoornaam = reader["voornaam"]?.ToString() ?? string.Empty,
                                OverledeneTussenvoegsel = reader["tussenvoegsel"]?.ToString() ?? string.Empty,
                                OverledeneAchternaam = reader["achternaam"]?.ToString() ?? string.Empty,
                                OverledeneGeboortedatum = reader["geboortedatum"]?.ToString() ?? string.Empty,
                                PersoneelNaam = reader["Uitvaartverzorger"]?.ToString() ?? string.Empty,
                                PersoneelId = Guid.Empty,
                                DossierCompleted = true
                            };
                            records.Add(record);
                        }
                    }
                }
            }
            return records;
        }
        static async Task<List<OverledeneSearchSurname>> SearchAccessDatabaseOnAchternaamAsync(string connectionString, string achternaam, string db)
        {
            List<OverledeneSearchSurname> records = new List<OverledeneSearchSurname>();

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                await connection.OpenAsync(); // Open the connection asynchronously
                string query = "SELECT Uitvaartnummer, [1 Naam overledene] as achternaam, [1 Tussenvoegsel] as tussenvoegsel, " +
                                "[1 Voornamen] as voornaam, [1 Geboortedatum] as geboortedatum, [1 Aanhef] as aanhef, Uitvaartverzorger " +
                                "FROM data WHERE [1 Naam overledene] LIKE ?";

                using (OdbcCommand command = new OdbcCommand(query, connection))
                {
                    command.Parameters.AddWithValue("?", "%" + achternaam + "%");

                    // Use DbDataReader instead of OdbcDataReader
                    using (DbDataReader reader = await command.ExecuteReaderAsync()) // Execute asynchronously
                    {
                        while (await reader.ReadAsync()) // Read each record asynchronously
                        {
                            OverledeneSearchSurname record = new OverledeneSearchSurname
                            {
                                UitvaartId = Guid.Empty,
                                UitvaartNummer = "archief_" + db + "_" + reader["Uitvaartnummer"]?.ToString() ?? string.Empty,
                                OverledeneAanhef = reader["aanhef"]?.ToString() ?? string.Empty,
                                OverledeneVoornaam = reader["voornaam"]?.ToString() ?? string.Empty,
                                OverledeneTussenvoegsel = reader["tussenvoegsel"]?.ToString() ?? string.Empty,
                                OverledeneAchternaam = reader["achternaam"]?.ToString() ?? string.Empty,
                                OverledeneGeboortedatum = reader["geboortedatum"]?.ToString() ?? string.Empty,
                                PersoneelNaam = reader["Uitvaartverzorger"]?.ToString() ?? string.Empty,
                                PersoneelId = Guid.Empty,
                                DossierCompleted = true
                            };
                            records.Add(record);
                        }
                    }
                }
            }
            return records;
        }
        async Task<bool> OpenArchivePdfAsync(string uitvaartnummer, string achternaam)
        {
            if (string.IsNullOrEmpty(uitvaartnummer) && string.IsNullOrEmpty(achternaam)) return false;

            string searchPattern = string.Empty;
            if (!string.IsNullOrEmpty(uitvaartnummer))
            {
                searchPattern = $"{uitvaartnummer}*.pdf";
            }
            else if (!string.IsNullOrEmpty(achternaam))
            {
                searchPattern = $"*{achternaam}*.pdf";
            }

            return await Task.Run(() =>
            {
                if (Directory.Exists(ArchivePath))
                {
                    var files = Directory.GetFiles(ArchivePath, searchPattern, SearchOption.AllDirectories);
                    if (files.Any())
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = files.First(),
                            UseShellExecute = true
                        });
                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"Geen archief bestand gevonden voor uitvaartnummer: {uitvaartnummer}", "Geen archief bestand gevonden", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show($"Archief folder niet gevonden: {ArchivePath}", "Archief folder niet gevonden", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                return false;
            });
        }
        private bool CanExecuteSearchUitvaartnummerCommand(object obj)
        {
            bool validNummerSearch;
            if (string.IsNullOrWhiteSpace(ZoekenUitvaartnummer))
            {
                validNummerSearch = false;
            }
            else
            {
                validNummerSearch = true;
            }
            return validNummerSearch;
        }
        private bool CanExecuteSearchAchternaamCommand(object obj)
        {
            if (!string.IsNullOrWhiteSpace(ZoekenAchternaam))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task DeceasedYearAgoCheck()
        {
            try
            {
                YearPassedNotification = await miscellaneousRepository.NotificationDeceasedAfterYearPassedAsync();
                if (YearPassedNotification != null && YearPassedNotification.Count > 0)
                {
                    if (notificatieWindow == null || !notificatieWindow.IsVisible)
                    {
                        notificatieWindow = new OverledeneNotification();
                        notificatieWindow.DataContext = this;
                        notificatieWindow.ShowDialog();
                    }
                }
            }
            catch(Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                Debug.WriteLine(ex);
            }
        }
        private async void ExecuteSearchUitvaartnummerCommand(object obj)
        {
            try
            {
                combinedResults.Clear();
                var searchUitvaartNumber = searchRepository.GetUitvaarleiderByUitvaartIdSearch(ZoekenUitvaartnummer);

                combinedResults.AddRange(searchUitvaartNumber);

                if (SearchArchiveFolder)
                    ArchiveSearchResult = await OpenArchivePdfAsync(ZoekenUitvaartnummer, string.Empty);


                if (!searchUitvaartNumber.Any() && SearchOldDatabaseNummer)
                {
                    var access2023Search = await SearchAccessDatabaseOnUitvaartNumberAsync($"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={DataProvider.AccessDatabase2023};Uid=;Pwd=;", ZoekenUitvaartnummer, "2023");
                    var access2024Search = await SearchAccessDatabaseOnUitvaartNumberAsync($"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={DataProvider.AccessDatabase2024};Uid=;Pwd=;", ZoekenUitvaartnummer, "2024");

                    combinedResults.AddRange(access2023Search);
                    combinedResults.AddRange(access2024Search);
                }

                if (!combinedResults.Any() && !ArchiveSearchResult)
                {
                    ErrorMessageUitvaartnummer = "* Geen uitvaart gevonden met nummer: " + ZoekenUitvaartnummer;
                    MessageBox.Show("Uitvaartnummer " + ZoekenUitvaartnummer + " niet gevonden!", "Uitvaartnummer " + ZoekenUitvaartnummer + " niet gevonden ", MessageBoxButton.OK);

                    ZoekenUitvaartnummer = null;
                    IsSearchVisible = false;
                    IsUitvaartnumberVisible = false;
                    return;
                }
                SearchUitvaartSurname = new ObservableCollection<OverledeneSearchSurname>(combinedResults);

                if (combinedResults.Count == 1 && combinedResults.First().UitvaartId != Guid.Empty)
                {
                    var firstSearchResult = combinedResults.First();
                    Globals.UitvaartCode = firstSearchResult.UitvaartNummer;
                    Instance.RequestedDossierInformationBasedOnUitvaartId(firstSearchResult.UitvaartNummer);

                    if (firstSearchResult.PersoneelNaam != null)
                        Globals.UitvaarLeider = firstSearchResult.PersoneelNaam;

                    Globals.UitvaartCodeGuid = firstSearchResult.UitvaartId;
                    Globals.DossierCompleted = firstSearchResult.DossierCompleted;
                    UitvaartNummerEnabled = true;
                    SelectedIndex = 1;
                }
                else if ((combinedResults.Count == 1 && combinedResults.FirstOrDefault().UitvaartId == Guid.Empty) || (combinedResults.Count > 1))
                {
                    IsSearchResultVisible = true;
                }
                IsUitvaartnumberVisible = false;
                ZoekenUitvaartnummer = null;
                IsSearchVisible = false;
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                MessageBox.Show($"Error occurred during search: {ex.Message}");
            }
        }
        private async void ExecuteSearchAchternaamCommand(object obj)
        {
            try
            {
                combinedResults.Clear();
                var searchUitvaartSurname = searchRepository.GetUitvaartleiderBySurnameOverledene(ZoekenAchternaam, ZoekenDoB);

                combinedResults.AddRange(searchUitvaartSurname);

                if (SearchArchiveFolder)
                    ArchiveSearchResult = await OpenArchivePdfAsync(string.Empty, ZoekenAchternaam);

                if (!searchUitvaartSurname.Any() && SearchOldDatabaseSurname)
                {
                    var access2023Search = await SearchAccessDatabaseOnAchternaamAsync($"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={DataProvider.AccessDatabase2023};Uid=;Pwd=;", ZoekenAchternaam, "2023");
                    var access2024Search = await SearchAccessDatabaseOnAchternaamAsync($"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={DataProvider.AccessDatabase2024};Uid=;Pwd=;", ZoekenAchternaam, "2024");

                    combinedResults.AddRange(access2023Search);
                    combinedResults.AddRange(access2024Search);
                }

                if (!combinedResults.Any() && !ArchiveSearchResult)
                {
                    if (ZoekenDoB == null)
                    {
                        MessageBox.Show("Geen uitvaarten gevonden met Achternaam: " + ZoekenAchternaam, "Zoeken op Achternaam mislukt", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Geen uitvaarten gevonden met Achternaam: " + ZoekenAchternaam + " i.c.m. geboortedatum: " + ZoekenDoB, "Zoeken op Achternaam mislukt", MessageBoxButton.OK);
                    }

                    ZoekenAchternaam = null;
                    ZoekenDoB = null;
                    IsSearchVisible = false;
                    return;
                }

                SearchUitvaartSurname = new ObservableCollection<OverledeneSearchSurname>(combinedResults);

                if (combinedResults.Count == 1 && combinedResults.FirstOrDefault().UitvaartId != Guid.Empty)
                {
                    var firstSearchResult = combinedResults.First();
                    Globals.UitvaartCode = firstSearchResult.UitvaartNummer;
                    Instance.RequestedDossierInformationBasedOnUitvaartId(firstSearchResult.UitvaartNummer);

                    if (firstSearchResult.PersoneelNaam != null)
                        Globals.UitvaarLeider = firstSearchResult.PersoneelNaam;

                    Globals.UitvaartCodeGuid = firstSearchResult.UitvaartId;
                    Globals.DossierCompleted = firstSearchResult.DossierCompleted;
                    UitvaartNummerEnabled = true;
                    SelectedIndex = 1;
                }
                else if ((combinedResults.Count == 1 && combinedResults.FirstOrDefault().UitvaartId == Guid.Empty) || (combinedResults.Count > 1))
                {
                    IsSearchResultVisible = true;
                }

                ZoekenAchternaam = null;
                ZoekenDoB = null;
                IsSearchVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred during search: {ex.Message}");
            }
        }
        private void ExecuteCreateNewRegistrationCommand(object obj)
        {
            Instance.CreateNewDossier();
            VerzekeringInstance.CreateNewDossier();
            UitvaartInstance.CreateNewDossier();
            SteenhouwerijInstance.CreateNewDossier();
            OpbarenInstance.CreateNewDossier();
            KostenbegrotingInstance.CreateNewDossier();
            ExtraInfoInstance.CreateNewDossier();
            BijlagesInstance.CreateNewDossier();
            AsbestemmingInstance.CreateNewDossier();

            UitvaartNummerEnabled = false;
            SelectedIndex = 1;
        }
        private void ExecuteOpenAchternaamCommand(object parameter)
        {
            if (parameter.ToString().StartsWith("archief_"))
            {
                string[] uitvaartnummerParts = parameter.ToString().Split('_');
                string uitvaartnummer = uitvaartnummerParts[^1];

                if (parameter.ToString().Contains("_2023_"))
                    AccessFormHelper.OpenAccessFormWithFilter(uitvaartnummer, DataProvider.AccessDatabase2023);

                if (parameter.ToString().Contains("_2024_"))
                    AccessFormHelper.OpenAccessFormWithFilter(uitvaartnummer, DataProvider.AccessDatabase2024);

                return;
            }
            var SearchUitvaartleider = searchRepository.GetUitvaarleiderByUitvaartId(parameter.ToString());

            Instance.RequestedDossierInformationBasedOnUitvaartId(parameter.ToString());

            if (SearchUitvaartleider.Uitvaartnummer != null)
                Globals.UitvaartCode = SearchUitvaartleider.Uitvaartnummer;

            if (SearchUitvaartleider.PersoneelNaam != null)
                Globals.UitvaarLeider = SearchUitvaartleider.PersoneelNaam;

            if (SearchUitvaartleider.UitvaartId != Guid.Empty)
                Globals.UitvaartCodeGuid = SearchUitvaartleider.UitvaartId;

            if (SearchUitvaartleider.DossierCompleted)
            {
                Globals.DossierCompleted = true;
            }
            else
            {
                Globals.DossierCompleted = false;
            }

            IsSearchResultVisible = false;
            UitvaartNummerEnabled = true;
            SelectedIndex = 1;
        }
        private void ExecuteCreateNewUserCommand(object obj)
        {
            Guid Id = Guid.NewGuid();
            Guid PersoneelId = Guid.NewGuid();

            NewUser.Id = PersoneelId;
            NewUser.IsDeleted = false;
            NewUser.IsUitvaartverzorger = false;
            NewUser.IsDrager = false;
            NewUser.IsChauffeur = false;

            var employeeSearch = searchRepository.SearchEmployee(NewUser);
            if (employeeSearch != null)
            {
                try
                {
                    createRepository.CreateWindowsUser(Id, employeeSearch.Id, Environment.UserName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating new user: {ex.Message}", "Creating Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }

                var userPerm = searchRepository.SelectUserPermission(employeeSearch.Id);

                Globals.PermissionLevelId = userPerm.Id.ToString();
                Globals.PermissionLevelName = userPerm.PermissionName;

                IsCreateUserPopupOpen = false;

                var activeAccountFound = MessageBox.Show("Beste " + Environment.UserName + ", de applicatie heeft een bestand account gevonden op basis van;\r\n" +
                                "Achternaam, Geboortedatum en Geboorteplaats\r\n" +
                                "De rechten die bij dat account horen zijn nu actief voor dit account.\r\n" +
                                "Rechten gevonden: " + userPerm.PermissionName + "\r\n" +
                                "De applicatie gaat nu opnieuw opstarten, een moment geduld a.u.b.", "Actief account gevonden!", MessageBoxButton.OK);

                if (activeAccountFound == MessageBoxResult.OK)
                {
                    var executablePath = Process.GetCurrentProcess().MainModule.FileName;
                    Process.Start(executablePath);
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                try
                {
                    createRepository.EmployeeCreate(NewUser);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating employee: {ex.Message}", "New Employee Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }

                try
                {
                    createRepository.CreateWindowsUser(Id, PersoneelId, Environment.UserName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating new user: {ex.Message}", "Creating Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }

                Globals.PermissionLevelId = "D7B812A4-3541-4B6D-A33C-532DCF91B8A7";
                Globals.PermissionLevelName = "Gebruiker";

                try
                {
                    createRepository.CreateUserPermission(PersoneelId, Guid.Parse(Globals.PermissionLevelId));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating user permission: {ex.Message}", "Creating Permissions Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }

                IsCreateUserPopupOpen = false;

                MessageBox.Show("Beste " + Environment.UserName + ", de applicatie heeft een account aangemaakt met kijk rechten.\r\n" +
                                "Het account is nu nog disabled en zal moeten worden geactiveerd door een bevoegd persoon.\r\n" +
                                "Tot die tijd kun je geen dossier aanmaken maar wel bekijken.");
            }
        }
        private async void UpdateTime()
        {
            CurrentTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH: mm:ss");
            await Task.Delay(1000);
            UpdateTime();
        }
        public async void CheckTabControl()
        {
            if (Globals.UitvaartCodeGuid != Guid.Empty)
                UitvaartNummerEnabled = true;
        }
        public void EnableTabControl(Guid uitvaartGuid)
        {
            if (uitvaartGuid != Guid.Empty)
                UitvaartNummerEnabled = true;

            IntAggregator.Transmit(1);

        }
        private bool CanExecuteClearAllModels(object obj)
        {
            return true;
        }
        private void ExecuteClearAllModels(object obj)
        {
            Instance.CreateNewDossier();
            VerzekeringInstance.CreateNewDossier();
            UitvaartInstance.CreateNewDossier();
            SteenhouwerijInstance.CreateNewDossier();
            OpbarenInstance.CreateNewDossier();
            KostenbegrotingInstance.CreateNewDossier();
            ExtraInfoInstance.CreateNewDossier();
            BijlagesInstance.CreateNewDossier();
            AsbestemmingInstance.CreateNewDossier();

            Globals.UitvaarLeider = string.Empty;
            Globals.DossierCompleted = false;
            Globals.UitvaartCode = string.Empty;
            Globals.UitvaartCodeGuid = Guid.Empty;

            ZoekenUitvaartnummer = string.Empty;
            ZoekenAchternaam = string.Empty;
            ZoekenDoB = string.Empty;

            UitvaartNummerEnabled = false;
        }
        public void LoadImageFromDatabase()
        {
            var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Frontend");

            if (documentData != null && documentData.Length > 0)
            {
                using (var stream = new MemoryStream(documentData))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImageSource = bitmap;
                }
            }
        }
    }
}
