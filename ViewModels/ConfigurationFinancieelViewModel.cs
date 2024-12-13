using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationFinancieelViewModel : ViewModelBase
    {
        Task createFactuurTask = null;
        Task createVerenigingFactuurTask = null;

        string kostenbegrotingUrl = string.Empty;
        SqlConnection conn = new(DataProvider.ConnectionString);

        public ICommand ExportBloemenToExcel { get; }
        public ICommand ExportStenenToExcel { get; }
        public ICommand ExportUrnSieradenToExcel { get; }
        public ICommand ExportWerkbonnenToExcel { get; }
        public ICommand OpenFilterFinancieelPopupOpenCommand { get; }
        public ICommand CloseFilterFinancieelPopupOpenCommand { get; }
        public ICommand OpenOpdrachtgeverFactuurCommand { get; }
        public ICommand OpenHerkomstFactuurCommand { get; }
        public ICommand FilterFinancieelCommand { get; }
        public ICommand CreateFactuurCommand { get; }
        public ICommand StartGeneratingCommand { get; }
        public ICommand FinishedGeneratingCommand { get; }
        public ICommand OpenKostenbegrotingCommand { get; }
        public ICommand OpenBloemenUitbetalenCommand { get; }
        public ICommand CloseBloemenUitbetalenCommand { get; }
        public ICommand SaveBloemenUitbetalenCommand { get; }
        public ICommand OpenSteenhouwerUitbetalenCommand { get; }
        public ICommand OpenUrnSieradenUitbetalenCommand { get; }
        public ICommand CloseSteenhouwerUitbetalenCommand { get; }
        public ICommand CloseUrnSieradenUitbetalenCommand { get; }
        public ICommand SaveSteenhouwerUitbetalenCommand { get; }
        public ICommand SaveUrnSieradenUitbetalenCommand { get; }
        public ICommand RefreshFinancieelGridCommand { get; set; }

        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        private bool isFilterFinancieelPopupOpen;
        private bool isGeneratingFilePopupOpen;
        private bool isBloemenUitbetalingPopupOpen;
        private bool isSteenhouwerUitbetalingPopupOpen;
        private bool isUrnSieradenUitbetalingPopupOpen;

        private string _startUitvaartNumber = "1";
        private string _endUitvaartNumber = int.MaxValue.ToString();

        private ObservableCollection<FactuurModel> _kostenbegrotingen;
        private OverledeneBloemenModel selectedBloemUitbetaling;
        private OverledeneSteenhouwerijModel selectedSteenhouwerUitbetaling;
        private OverledeneUrnSieradenModel selectedUrnSieradenUitbetaling;
        private FinancieelFilterModel selectedFinancieelFilter;
        private OverledeneWerkbonUitvaart _werkbonnenEditFinancieel;
        private OverledeneBloemenModel _bloemenEditFinancieel;
        private OverledeneSteenhouwerijModel _steenhouwerEditFinancieel;
        private RapportagesFilter _uitvaartnummerFilter;
        private GenerateFactuur _generateSelectedFactuur;
        private ObservableCollection<WerkbonnenData> _werkbonnenDataSet;
        private ObservableCollection<OverledeneBloemenModel> _bloemenFinancieel;
        private ObservableCollection<OverledeneSteenhouwerijModel> _steenhouwerijFinancieel;
        private ObservableCollection<OverledeneUrnSieradenModel> _urnSieradenFinancieel;
        private ObservableCollection<OverledeneWerkbonUitvaart> _werkbonnenFinancieel;
        private ObservableCollection<WerkbonnenData> _filteredWerkbonnenData;
        private ObservableCollection<OverledeneBloemenModel> _filteredBloemenFinancieel;
        private ObservableCollection<OverledeneSteenhouwerijModel> _filteredSteenhouwerijFinancieel;
        private ObservableCollection<OverledeneUrnSieradenModel> _filteredUrnSieradenFinancieel;

        public OverledeneBloemenModel SelectedBloemUitbetaling
        {
            get { return selectedBloemUitbetaling; }
            set
            {
                if (selectedBloemUitbetaling != value)
                {
                    selectedBloemUitbetaling = value;
                    OnPropertyChanged(nameof(SelectedBloemUitbetaling));
                }
            }
        }
        public OverledeneSteenhouwerijModel SelectedSteenhouwerUitbetaling
        {
            get { return selectedSteenhouwerUitbetaling; }
            set
            {
                if (selectedSteenhouwerUitbetaling != value)
                {
                    selectedSteenhouwerUitbetaling = value;
                    OnPropertyChanged(nameof(SelectedSteenhouwerUitbetaling));
                }
            }
        }
        public OverledeneUrnSieradenModel SelectedUrnSieradenUitbetaling
        {
            get { return selectedUrnSieradenUitbetaling; }
            set
            {
                if (selectedUrnSieradenUitbetaling != value)
                {
                    selectedUrnSieradenUitbetaling = value;
                    OnPropertyChanged(nameof(SelectedUrnSieradenUitbetaling));
                }
            }
        }
        public FinancieelFilterModel SelectedFinancieelFilter
        {
            get { return selectedFinancieelFilter; }
            set
            {
                if (selectedFinancieelFilter != value)
                {
                    selectedFinancieelFilter = value; OnPropertyChanged(nameof(SelectedFinancieelFilter));
                }
            }
        }
        public OverledeneWerkbonUitvaart WerkbonnenEditFinancieel
        {
            get { return _werkbonnenEditFinancieel; }
            set
            {
                if (_werkbonnenEditFinancieel != value)
                {
                    _werkbonnenEditFinancieel = value;
                    OnPropertyChanged(nameof(WerkbonnenEditFinancieel));
                }
            }
        }
        public GenerateFactuur GenerateSelectedFactuur
        {
            get { return _generateSelectedFactuur; }
            set
            {
                if (_generateSelectedFactuur != value)
                {
                    _generateSelectedFactuur = value;
                    OnPropertyChanged(nameof(GenerateSelectedFactuur));
                }
            }
        }
        public OverledeneBloemenModel BloemenEditFinancieel
        {
            get { return _bloemenEditFinancieel; }
            set
            {
                if (_bloemenEditFinancieel != value)
                {
                    _bloemenEditFinancieel = value;
                    OnPropertyChanged(nameof(BloemenEditFinancieel));
                }
            }
        }
        public OverledeneSteenhouwerijModel SteenhouwerEditFinaniceel
        {
            get { return _steenhouwerEditFinancieel; }
            set
            {
                if (_steenhouwerEditFinancieel != value)
                {
                    _steenhouwerEditFinancieel = value;
                    OnPropertyChanged(nameof(SteenhouwerEditFinaniceel));
                }
            }
        }
        public ObservableCollection<OverledeneWerkbonUitvaart> WerkbonnenFinancieel
        {
            get { return _werkbonnenFinancieel; }
            set
            {
                if (_werkbonnenFinancieel != value)
                {
                    _werkbonnenFinancieel = value;
                    OnPropertyChanged(nameof(WerkbonnenFinancieel));
                }
            }
        }
        public ObservableCollection<OverledeneBloemenModel> BloemenFinancieel
        {
            get { return _bloemenFinancieel; }
            set
            {
                if (_bloemenFinancieel != value)
                {
                    _bloemenFinancieel = value;
                    OnPropertyChanged(nameof(BloemenFinancieel));
                }
            }
        }
        public ObservableCollection<OverledeneSteenhouwerijModel> SteenhouwerijFinancieel
        {
            get { return _steenhouwerijFinancieel; }
            set
            {
                if (_steenhouwerijFinancieel != value)
                {
                    _steenhouwerijFinancieel = value;
                    OnPropertyChanged(nameof(SteenhouwerijFinancieel));
                }
            }
        }
        public ObservableCollection<OverledeneUrnSieradenModel> UrnSieradenFinancieel
        {
            get { return _urnSieradenFinancieel; }
            set
            {
                if (_urnSieradenFinancieel != value)
                {
                    _urnSieradenFinancieel = value;
                    OnPropertyChanged(nameof(UrnSieradenFinancieel));
                }
            }
        }
        public ObservableCollection<FactuurModel> Kostenbegrotingen
        {
            get { return _kostenbegrotingen; }
            set
            {
                if (_kostenbegrotingen != value)
                {
                    _kostenbegrotingen = value;
                    OnPropertyChanged(nameof(Kostenbegrotingen));
                }
            }
        }
        public ObservableCollection<WerkbonnenData> WerkbonnenDataSet
        {
            get { return _werkbonnenDataSet; }
            set
            {
                if (_werkbonnenDataSet != value)
                {
                    _werkbonnenDataSet = value;
                    OnPropertyChanged(nameof(WerkbonnenDataSet));
                }
            }
        }
        public ObservableCollection<WerkbonnenData> FilteredWerkbonnenData
        {
            get => _filteredWerkbonnenData;
            set
            {
                if (_filteredWerkbonnenData != value)
                {
                    _filteredWerkbonnenData = value;
                    OnPropertyChanged(nameof(FilteredWerkbonnenData));
                }
            }
        }
        public ObservableCollection<OverledeneBloemenModel> FilteredBloemenFinancieel
        {
            get => _filteredBloemenFinancieel;
            set
            {
                if (_filteredBloemenFinancieel != value)
                {
                    _filteredBloemenFinancieel = value;
                    OnPropertyChanged(nameof(FilteredBloemenFinancieel));
                }
            }
        }
        public ObservableCollection<OverledeneSteenhouwerijModel> FilteredSteenhouwerijFinancieel
        {
            get => _filteredSteenhouwerijFinancieel;
            set
            {
                if (_filteredSteenhouwerijFinancieel != value)
                {
                    _filteredSteenhouwerijFinancieel = value;
                    OnPropertyChanged(nameof(FilteredSteenhouwerijFinancieel));
                }
            }
        }
        public ObservableCollection<OverledeneUrnSieradenModel> FilteredUrnSieradenFinancieel
        {
            get { return _filteredUrnSieradenFinancieel; }
            set
            {
                if (_filteredUrnSieradenFinancieel != value)
                {
                    _filteredUrnSieradenFinancieel = value;
                    OnPropertyChanged(nameof(FilteredUrnSieradenFinancieel));
                }
            }
        }

        public bool IsGeneratingFilePopupOpen
        {
            get { return isGeneratingFilePopupOpen; }
            set
            {
                if (isGeneratingFilePopupOpen != value)
                {
                    isGeneratingFilePopupOpen = value;
                    OnPropertyChanged(nameof(IsGeneratingFilePopupOpen));
                }
            }
        }
        public bool IsBloemenUitbetalingPopupOpen
        {
            get { return isBloemenUitbetalingPopupOpen; }
            set
            {
                if (isBloemenUitbetalingPopupOpen != value)
                {
                    isBloemenUitbetalingPopupOpen = value;
                    OnPropertyChanged(nameof(IsBloemenUitbetalingPopupOpen));
                }
            }
        }
        public bool IsSteenhouwerUitbetalingPopupOpen
        {
            get { return isSteenhouwerUitbetalingPopupOpen; }
            set
            {
                if (isSteenhouwerUitbetalingPopupOpen != value)
                {
                    isSteenhouwerUitbetalingPopupOpen = value;
                    OnPropertyChanged(nameof(IsSteenhouwerUitbetalingPopupOpen));
                }
            }
        }
        public bool IsUrnSieradenUitbetalingPopupOpen
        {
            get { return isUrnSieradenUitbetalingPopupOpen; }
            set
            {
                if (isUrnSieradenUitbetalingPopupOpen != value)
                {
                    isUrnSieradenUitbetalingPopupOpen = value;
                    OnPropertyChanged(nameof(IsUrnSieradenUitbetalingPopupOpen));
                }
            }
        }
        public bool IsFilterFinancieelPopupOpen
        {
            get { return isFilterFinancieelPopupOpen; }
            set
            {
                if (isFilterFinancieelPopupOpen != value)
                {
                    isFilterFinancieelPopupOpen = value;
                    OnPropertyChanged(nameof(IsFilterFinancieelPopupOpen));
                }
            }
        }
        public string StartUitvaartNumber
        {
            get { return _startUitvaartNumber; }
            set
            {
                _startUitvaartNumber = value;
                OnPropertyChanged(nameof(StartUitvaartNumber));
                UpdateFilter();
            }
        }
        public string EndUitvaartNumber
        {
            get { return _endUitvaartNumber; }
            set
            {
                _endUitvaartNumber = value;
                OnPropertyChanged(nameof(EndUitvaartNumber));
                UpdateFilter();
            }
        }
        public DateTime CurrentDate => DateTime.Now;
        public DateTime TargetDate
        {
            get
            {
                return new DateTime(CurrentDate.Year, CurrentDate.Month, 25);
            }
        }
        public ConfigurationFinancieelViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedBloemUitbetaling = new OverledeneBloemenModel();
            SelectedSteenhouwerUitbetaling = new OverledeneSteenhouwerijModel();
            SelectedUrnSieradenUitbetaling = new OverledeneUrnSieradenModel();
            SelectedFinancieelFilter = new FinancieelFilterModel();
            WerkbonnenFinancieel = new();
            GenerateSelectedFactuur = new GenerateFactuur();
            Kostenbegrotingen = new ObservableCollection<FactuurModel>();
            BloemenFinancieel = new ObservableCollection<OverledeneBloemenModel>();
            SteenhouwerijFinancieel = new ObservableCollection<OverledeneSteenhouwerijModel>();
            UrnSieradenFinancieel = new ObservableCollection<OverledeneUrnSieradenModel>();
            WerkbonnenDataSet = new ObservableCollection<WerkbonnenData>();
            FilteredWerkbonnenData = new ObservableCollection<WerkbonnenData>();
            FilteredBloemenFinancieel = new ObservableCollection<OverledeneBloemenModel>();
            FilteredSteenhouwerijFinancieel = new ObservableCollection<OverledeneSteenhouwerijModel>();
            FilteredUrnSieradenFinancieel = new ObservableCollection<OverledeneUrnSieradenModel>();

            FilterFinancieelCommand = new ViewModelCommand(ExecuteFilterFinancieelCommand);
            OpenKostenbegrotingCommand = new ViewModelCommand(ExecuteOpenKostenbegrotingCommand);
            OpenHerkomstFactuurCommand = new ViewModelCommand(ExecuteOpenFactuurCommand);
            OpenOpdrachtgeverFactuurCommand = new ViewModelCommand(ExecuteOpenFactuurCommand);
            OpenBloemenUitbetalenCommand = new ViewModelCommand(ExecuteOpenUitbetalingBloemenCommand);
            OpenSteenhouwerUitbetalenCommand = new ViewModelCommand(ExecuteOpenSteenhouwerUitbetalingCommand);
            OpenUrnSieradenUitbetalenCommand = new ViewModelCommand(ExeucteOpenUrnSieradenUitbetalingCommand);

            ExportBloemenToExcel = new ViewModelCommand(ExecuteExportBloemenToExcel);
            ExportStenenToExcel = new ViewModelCommand(ExecuteExportSteenhouwerijToExcel);
            ExportUrnSieradenToExcel = new ViewModelCommand(ExecuteExportUrnSieradenToExcel);
            ExportWerkbonnenToExcel = new ViewModelCommand(ExecuteExportWerkbonnenToExcel);

            CloseFilterFinancieelPopupOpenCommand = new RelayCommand(() => IsFilterFinancieelPopupOpen = false);
            CloseBloemenUitbetalenCommand = new RelayCommand(() => IsBloemenUitbetalingPopupOpen = false);
            CloseSteenhouwerUitbetalenCommand = new RelayCommand(() => IsSteenhouwerUitbetalingPopupOpen = false);
            CloseUrnSieradenUitbetalenCommand = new RelayCommand(() => IsUrnSieradenUitbetalingPopupOpen = false);
            OpenFilterFinancieelPopupOpenCommand = new RelayCommand(() => IsFilterFinancieelPopupOpen = true);
            StartGeneratingCommand = new RelayCommand(() => IsGeneratingFilePopupOpen = true);
            FinishedGeneratingCommand = new RelayCommand(() => IsGeneratingFilePopupOpen = false);
            RefreshFinancieelGridCommand = new RelayCommand(KostenbegrotingGridData);

            SaveBloemenUitbetalenCommand = new ViewModelCommand(ExecuteSaveBloemenUitbetalenCommand);
            SaveSteenhouwerUitbetalenCommand = new ViewModelCommand(ExecuteSaveSteenhouwerUitbetalenCommand);
            SaveUrnSieradenUitbetalenCommand = new ViewModelCommand(ExecuteSaveUrnSieradenUitbetalenCommand);
            CreateFactuurCommand = new ViewModelCommand(ExecuteCreateFactuurCommand);

            KostenbegrotingGridData();
            BloemenGridData();
            SteenhouwerijGridData();
            WerkbonnenGridData();
            UrnSieradenGridData();
            UpdateFilter();
        }
        public void ExecuteFilterFinancieelCommand(object obj)
        {
            if (string.IsNullOrEmpty(StartUitvaartNumber) && string.IsNullOrEmpty(EndUitvaartNumber))
            {
                MessageBox.Show("Geen start en eind nummer ingegeven als filter.");
            }
            else
            {
                CloseFilterFinancieelPopupOpenCommand.Execute(null);
            }
        }
        public static void ExecuteOpenKostenbegrotingCommand(object obj)
        {
            if (File.Exists(obj.ToString()))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = obj.ToString(),
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show($"Kostenbegroting niet gevonden: \r\n {obj.ToString()}", "Kostenbegroting niet gevonden.", MessageBoxButton.OK, MessageBoxImage.Hand);
            }

        }
        public static void ExecuteOpenFactuurCommand(object obj)
        {
            if (File.Exists(obj.ToString()))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = obj.ToString(),
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show($"Factuur niet gevonden: \r\n {obj.ToString()}", "Factuur niet gevonden.", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }
        public async void ExecuteCreateFactuurCommand(object obj)
        {
            string verenigingFactuurUrl = string.Empty;
            string factuurUrl = string.Empty;
            string factuurSuffix = string.Empty;

            GenerateSelectedFactuur = new GenerateFactuur();

            if ((Guid)obj != Guid.Empty)
            {
                var factuurComps = searchRepository.GetGenerateFactuurDataByUitvaartId((Guid)obj);

                GenerateSelectedFactuur.UitvaartId = factuurComps.UitvaartId;
                GenerateSelectedFactuur.UitvaartNummer = factuurComps.UitvaartNummer;
                GenerateSelectedFactuur.KostenbegrotingJson = factuurComps.KostenbegrotingJson;
                GenerateSelectedFactuur.OpdrachtgeverAanhef = factuurComps.OpdrachtgeverAanhef;
                GenerateSelectedFactuur.OpdrachtgeverVoornamen = factuurComps.OpdrachtgeverVoornamen;
                GenerateSelectedFactuur.OpdrachtgeverTussenvoegsel = factuurComps.OpdrachtgeverTussenvoegsel;
                GenerateSelectedFactuur.OpdrachtgeverAchternaam = factuurComps.OpdrachtgeverAchternaam;
                GenerateSelectedFactuur.OpdrachtgeverStraat = factuurComps.OpdrachtgeverStraat;
                GenerateSelectedFactuur.OpdrachtgeverHuisnummer = factuurComps.OpdrachtgeverHuisnummer;
                GenerateSelectedFactuur.OpdrachtgeverHuisnummerToevoeging = factuurComps.OpdrachtgeverHuisnummerToevoeging;
                GenerateSelectedFactuur.OpdrachtgeverPostcode = factuurComps.OpdrachtgeverPostcode;
                GenerateSelectedFactuur.OpdrachtgeverWoonplaats = factuurComps.OpdrachtgeverWoonplaats;
                GenerateSelectedFactuur.OverledeneAanhef = factuurComps.OverledeneAanhef;
                GenerateSelectedFactuur.OverledeneVoornamen = factuurComps.OverledeneVoornamen;
                GenerateSelectedFactuur.OverledeneTussenvoegsel = factuurComps.OverledeneTussenvoegsel;
                GenerateSelectedFactuur.OverledeneAchternaam = factuurComps.OverledeneAchternaam;
                GenerateSelectedFactuur.OverledeneOpDatum = factuurComps.OverledeneOpDatum;
                GenerateSelectedFactuur.OverledeneOpDatum = factuurComps.OverledeneOpDatum;
                GenerateSelectedFactuur.OverledeneLidnummer = factuurComps.OverledeneLidnummer;
                GenerateSelectedFactuur.OverledeneVerzekeringJson = factuurComps.OverledeneVerzekeringJson;
                GenerateSelectedFactuur.FactuurType = factuurComps.FactuurType;
                GenerateSelectedFactuur.HerkomstCity = factuurComps.HerkomstCity;
                GenerateSelectedFactuur.HerkomstHousenumber = factuurComps.HerkomstHousenumber;
                GenerateSelectedFactuur.HerkomstHousenumberAddition = factuurComps.HerkomstHousenumberAddition;
                GenerateSelectedFactuur.HerkomstPostbus = factuurComps.HerkomstPostbus;
                GenerateSelectedFactuur.HerkomstZipcode = factuurComps.HerkomstZipcode;
                GenerateSelectedFactuur.HerkomstStreet = factuurComps.HerkomstStreet;
                GenerateSelectedFactuur.CorrespondentieType = factuurComps.CorrespondentieType;

                StartGeneratingCommand.Execute(null);

                string splitFactuur;

                switch (GenerateSelectedFactuur.FactuurType)
                {

                    case "Opdrachtgever":
                        factuurUrl = await CreateFactuurFile(factuurSuffix);
                        createFactuurTask = Task.Run(() => FillFactuurFile(GenerateSelectedFactuur.UitvaartId, factuurUrl, GenerateSelectedFactuur.KostenbegrotingJson, GenerateSelectedFactuur.OverledeneVerzekeringJson, string.Empty));
                        break;

                    case "Opdrachtgever & Vereniging":
                        factuurUrl = await CreateFactuurFile(string.Empty);
                        createFactuurTask = Task.Run(() => FillFactuurFile(GenerateSelectedFactuur.UitvaartId, factuurUrl, GenerateSelectedFactuur.KostenbegrotingJson, GenerateSelectedFactuur.OverledeneVerzekeringJson, "Opdrachtgever"));

                        verenigingFactuurUrl = await CreateFactuurFile("A");
                        createVerenigingFactuurTask = Task.Run(() => FillFactuurFile(GenerateSelectedFactuur.UitvaartId, verenigingFactuurUrl, GenerateSelectedFactuur.KostenbegrotingJson, GenerateSelectedFactuur.OverledeneVerzekeringJson, "Vereniging"));
                        break;

                    case "Vereniging":
                        verenigingFactuurUrl = await CreateFactuurFile("A");
                        createVerenigingFactuurTask = Task.Run(() => FillFactuurFile(GenerateSelectedFactuur.UitvaartId, verenigingFactuurUrl, GenerateSelectedFactuur.KostenbegrotingJson, GenerateSelectedFactuur.OverledeneVerzekeringJson, string.Empty));
                        break;
                }

                if (createFactuurTask != null)
                    await createFactuurTask;
                if (createVerenigingFactuurTask != null)
                    await createVerenigingFactuurTask;

                var factuurJsonObject = new JObject();
                if (!string.IsNullOrEmpty(factuurUrl))
                    factuurJsonObject.Add("opdrachtgeverFactuurUrl", factuurUrl);
                if (!string.IsNullOrEmpty(verenigingFactuurUrl))
                    factuurJsonObject.Add("verenigingFactuurUrl", verenigingFactuurUrl);


                using (SqlConnection conn = new(DataProvider.ConnectionString))
                {
                    conn.Open();

                    Guid documentId = Guid.NewGuid();

                    string factuurQuery = "UPDATE [OverledeneFacturen] " +
                                            "SET [factuurUrl] = @FactuurUrl, [factuurCreationDate] = @FactuurCreationDate, [factuurCreated] = '1' " +
                                            "WHERE [UitvaartId] = @UitvaartId";

                    using (SqlCommand command = new(factuurQuery, conn))
                    {
                        command.Parameters.AddWithValue("@DocumentId", documentId);
                        command.Parameters.AddWithValue("@UitvaartId", GenerateSelectedFactuur.UitvaartId);
                        command.Parameters.AddWithValue("@FactuurUrl", factuurJsonObject.ToString());
                        command.Parameters.AddWithValue("@FactuurCreationDate", DateTime.Now);


                        command.ExecuteNonQuery();
                    }
                }

                FinishedGeneratingCommand.Execute(null);
                KostenbegrotingGridData();
            }
        }
        public void ExecuteOpenUitbetalingBloemenCommand(object obj)
        {
            var bloemenId = miscellaneousRepository.GetBloemenUitbetaling((Guid)obj);

            SelectedBloemUitbetaling.BloemenId = bloemenId.BloemenId;
            SelectedBloemUitbetaling.UitvaartId = (Guid)obj;
            SelectedBloemUitbetaling.UitvaartNummer = bloemenId.UitvaartNummer;
            SelectedBloemUitbetaling.UitvaartLeider = bloemenId.UitvaartLeider;
            SelectedBloemUitbetaling.BloemenLeverancierName = bloemenId.BloemenLeverancierName;
            SelectedBloemUitbetaling.BloemenBedrag = bloemenId.BloemenBedrag;
            SelectedBloemUitbetaling.BloemenProvisie = bloemenId.BloemenProvisie;
            SelectedBloemUitbetaling.BloemenUitbetaling = bloemenId.BloemenUitbetaling;

            IsBloemenUitbetalingPopupOpen = true;
        }
        public void ExecuteOpenSteenhouwerUitbetalingCommand(object obj)
        {
            DateTime defaultPaymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 25);

            var steenId = miscellaneousRepository.GetSteenUitbetaling((Guid)obj);
            if (steenId != null)
            {
                SelectedSteenhouwerUitbetaling.SteenhouwerijId = steenId.SteenhouwerijId;
                SelectedSteenhouwerUitbetaling.UitvaartId = (Guid)obj;
                SelectedSteenhouwerUitbetaling.UitvaartNummer = steenId.UitvaartNummer;
                SelectedSteenhouwerUitbetaling.UitvaartLeider = steenId.UitvaartLeider;
                SelectedSteenhouwerUitbetaling.SteenhouwerLeverancierName = steenId.SteenhouwerLeverancierName;
                SelectedSteenhouwerUitbetaling.SteenhouwerBedrag = steenId.SteenhouwerBedrag;
                SelectedSteenhouwerUitbetaling.SteenhouwerProvisie = steenId.SteenhouwerProvisie;
                SelectedSteenhouwerUitbetaling.SteenhouwerProvisieTotaal = steenId.SteenhouwerProvisieTotaal;
                SelectedSteenhouwerUitbetaling.SteenhouwerUitbetaing = steenId.SteenhouwerUitbetaing ?? defaultPaymentDate;

                IsSteenhouwerUitbetalingPopupOpen = true;
            }
        }
        public void ExeucteOpenUrnSieradenUitbetalingCommand(object obj)
        {
            var urnId = miscellaneousRepository.GetUrnSieradenUitbetaling((Guid)obj);
            if (urnId != null)
            {
                SelectedUrnSieradenUitbetaling.UrnId = urnId.UrnId;
                SelectedUrnSieradenUitbetaling.UitvaartId = (Guid)obj;
                SelectedUrnSieradenUitbetaling.UitvaartNummer = urnId.UitvaartNummer;
                SelectedUrnSieradenUitbetaling.UitvaartLeider = urnId.UitvaartLeider;
                SelectedUrnSieradenUitbetaling.UrnLeverancierName = urnId.UrnLeverancierName;
                SelectedUrnSieradenUitbetaling.UrnBedrag = urnId.UrnBedrag;
                SelectedUrnSieradenUitbetaling.UrnProvisie = urnId.UrnProvisie;
                SelectedUrnSieradenUitbetaling.UrnUitbetaing = urnId.UrnUitbetaing;

                IsUrnSieradenUitbetalingPopupOpen = true;
            }
        }
        public void ExecuteExportBloemenToExcel(object obj)
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application();
                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet?)workbook.Worksheets[1];

                int headerIndex = 1;
                foreach (var column in GetBloemenColumns())
                    ((Excel.Range)worksheet.Cells[1, headerIndex++]).Value2 = column.Header;

                int rowIndex = 2;
                foreach (var item in FilteredBloemenFinancieel)
                {
                    if (item.BloemenUitbetaling.HasValue && item.BloemenUitbetaling.Value.Date == TargetDate.Date)
                    {
                        int columnIndex = 1;
                        foreach (var column in GetBloemenColumns())
                            ((Excel.Range)worksheet.Cells[rowIndex, columnIndex++]).Value2 = column.GetValue(item);
                        
                            rowIndex++;
                    }
                }
                excelApp.Visible = true;
                workbook.Activate();
            }catch(Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                worksheet = null;
                workbook = null;
                excelApp = null;
            }
        }
        private static IEnumerable<DataGridColumnBloemen> GetBloemenColumns()
        {
            yield return new DataGridColumnBloemen { Header = "Uitvaart Nummer", PropertyName = "UitvaartNummer" };
            yield return new DataGridColumnBloemen { Header = "Leverancier", PropertyName = "BloemenLeverancierName" };
            yield return new DataGridColumnBloemen { Header = "Werknemer", PropertyName = "BloemenWerknemer" };
            yield return new DataGridColumnBloemen { Header = "Bedrag", PropertyName = "BloemenBedrag" };
            yield return new DataGridColumnBloemen { Header = "Provisie", PropertyName = "BloemenProvisie" };
            yield return new DataGridColumnBloemen { Header = "Uitbetaling", PropertyName = "BloemenUitbetaling" };
        }
        public class DataGridColumnBloemen
        {
            public string Header { get; set; }
            public string PropertyName { get; set; }

            public object GetValue(OverledeneBloemenModel item)
            {
                var property = item.GetType().GetProperty(PropertyName);
                return property?.GetValue(item);
            }
        }
        public void ExecuteExportUrnSieradenToExcel(object obj)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets[1];

            try
            {
                int headerIndex = 1;
                foreach (var column in GetUrnSieradenColumns())
                    ((Excel.Range)worksheet.Cells[1, headerIndex++]).Value2 = column.Header;

                int rowIndex = 2;
                foreach (var item in FilteredUrnSieradenFinancieel)
                {
                    if (item.UrnUitbetaing.HasValue && item.UrnUitbetaing.Value.Date == TargetDate.Date)
                    {
                        int columnIndex = 1;
                        foreach (var column in GetUrnSieradenColumns())
                            ((Excel.Range)worksheet.Cells[rowIndex, columnIndex++]).Value2 = column.GetValue(item);

                        rowIndex++;
                    }
                }
                excelApp.Visible = true;
                workbook.Activate();
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                worksheet = null;
                workbook = null;
                excelApp = null;
            }
        }
        private static IEnumerable<DataGridColumnsUrnSieraden> GetUrnSieradenColumns()
        {
            yield return new DataGridColumnsUrnSieraden { Header = "Uitvaart Nummer", PropertyName = "UitvaartNummer" };
            yield return new DataGridColumnsUrnSieraden { Header = "Leverancier", PropertyName = "UrnLeverancierName" };
            yield return new DataGridColumnsUrnSieraden { Header = "Werknemer", PropertyName = "UrnWerknemer" };
            yield return new DataGridColumnsUrnSieraden { Header = "Bedrag", PropertyName = "UrnBedrag" };
            yield return new DataGridColumnsUrnSieraden { Header = "Provisie", PropertyName = "UrnProvisie" };
            yield return new DataGridColumnsUrnSieraden { Header = "Uitbetaling", PropertyName = "UrnUitbetaing" };
        }
        public class DataGridColumnsUrnSieraden
        {
            public string Header { get; set; }
            public string PropertyName { get; set; }
            public object GetValue(OverledeneUrnSieradenModel item)
            {
                var property = item.GetType().GetProperty(PropertyName);
                return property?.GetValue(item);
            }
        }
        public void ExecuteExportSteenhouwerijToExcel(object obj)
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                // Initialize Excel application
                excelApp = new Excel.Application();
                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet)workbook.Worksheets[1];
                int headerIndex = 1;

                ((Excel.Range)worksheet.Cells[1, 1]).Value2 = "Steenhouwerij";

                // Set the first header
                int headerRowIndex = 2;
                foreach (var column in GetSteenColumns())
                    ((Excel.Range)worksheet.Cells[headerRowIndex, headerIndex++]).Value2 = column.Header;

                int rowIndex = headerRowIndex + 1;

                // Populate the rows for Steen Columns
                foreach (var item in FilteredSteenhouwerijFinancieel)
                {
                    if (item.SteenhouwerUitbetaing.HasValue && item.SteenhouwerUitbetaing.Value.Date == TargetDate.Date)
                    {
                        int columnIndex = 1;
                        foreach (var column in GetSteenColumns())
                            ((Excel.Range)worksheet.Cells[rowIndex, columnIndex++]).Value2 = column.GetValue(item);

                        rowIndex++;
                    }
                }

                rowIndex += 3;

                ((Excel.Range)worksheet.Cells[rowIndex, 1]).Value2 = "Urnen & Sieraden";

                int secondHeaderIndex = 1;
                int secondHeaderRowIndex = rowIndex + 1;
                foreach (var column in GetUrnSieradenColumns())
                    ((Excel.Range)worksheet.Cells[secondHeaderRowIndex, secondHeaderIndex++]).Value2 = column.Header;

                rowIndex = secondHeaderRowIndex + 1;

                // Populate the rows for Urn & Sieraden Columns
                foreach (var item in FilteredUrnSieradenFinancieel)
                {
                    if (item.UrnUitbetaing.HasValue && item.UrnUitbetaing.Value.Date == TargetDate.Date)
                    {
                        int secondColumnIndex = 1;
                        foreach (var column in GetUrnSieradenColumns())
                            ((Excel.Range)worksheet.Cells[rowIndex, secondColumnIndex++]).Value2 = column.GetValue(item);

                        rowIndex++;
                    }
                }


                excelApp.Visible = true;
                workbook.Activate();
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                worksheet = null;
                workbook = null;
                excelApp = null;
            }

        }
        private static IEnumerable<DataGridColumnSteenhouwerij> GetSteenColumns()
        {
            yield return new DataGridColumnSteenhouwerij { Header = "Uitvaart Nummer", PropertyName = "UitvaartNummer" };
            yield return new DataGridColumnSteenhouwerij { Header = "Leverancier", PropertyName = "SteenhouwerLeverancierName" };
            yield return new DataGridColumnSteenhouwerij { Header = "Werknemer", PropertyName = "SteenhouwerWerknemer" };
            yield return new DataGridColumnSteenhouwerij { Header = "Bedrag", PropertyName = "SteenhouwerBedrag" };
            yield return new DataGridColumnSteenhouwerij { Header = "Provisie", PropertyName = "SteenhouwerProvisie" };
            yield return new DataGridColumnSteenhouwerij { Header = "Uitbetaling", PropertyName = "SteenhouwerUitbetaing" };
        }
        public class DataGridColumnSteenhouwerij
        {
            public string Header { get; set; }
            public string PropertyName { get; set; }

            public object GetValue(OverledeneSteenhouwerijModel item)
            {
                var property = item.GetType().GetProperty(PropertyName);
                return property?.GetValue(item);
            }
        }
        public void ExecuteExportWerkbonnenToExcel(object obj)
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                // Initialize Excel application
                excelApp = new Excel.Application();
                workbook = excelApp.Workbooks.Add();
                worksheet = (Excel.Worksheet)workbook.Worksheets[1];
                int headerIndex = 1;
                foreach (var column in GetWerkbonColumns())
                    ((Excel.Range)worksheet.Cells[1, headerIndex++]).Value2 = column.Header;

                int rowIndex = 2;
                foreach (var item in FilteredWerkbonnenData)
                {
                    int columnIndex = 1;
                    foreach (var column in GetWerkbonColumns())
                    {
                        if (column.Header == "Bedrag")
                        {
                            if ((item.RouwAuto || item.VolgAuto) && (item.Condoleance || item.Overbrengen))
                                ((Excel.Range)worksheet.Cells[rowIndex, columnIndex++]).Value2 = "37,50";
                            else if (item.RouwAuto || item.VolgAuto)
                                ((Excel.Range)worksheet.Cells[rowIndex, columnIndex++]).Value2 = "30";
                            else if (item.Condoleance || item.Overbrengen)
                                ((Excel.Range)worksheet.Cells[rowIndex, columnIndex++]).Value2 = "7,50";
                        }
                        else
                        {
                            ((Excel.Range)worksheet.Cells[rowIndex, columnIndex++]).Value2 = column.GetValue(item);
                        }
                    }
                    rowIndex++;
                }
                excelApp.Visible = true;
                workbook.Activate();
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
            finally
            {
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (workbook != null) Marshal.ReleaseComObject(workbook);
                if (excelApp != null) Marshal.ReleaseComObject(excelApp);

                worksheet = null;
                workbook = null;
                excelApp = null;
            }

        }
        private static IEnumerable<DataGridColumnWerkbonnen> GetWerkbonColumns()
        {
            yield return new DataGridColumnWerkbonnen { Header = "Uitvaart Nummer", PropertyName = "UitvaartNummer" };
            yield return new DataGridColumnWerkbonnen { Header = "Werknemer", PropertyName = "WerknemerName" };
            yield return new DataGridColumnWerkbonnen { Header = "Rouwauto", PropertyName = "RouwAuto" };
            yield return new DataGridColumnWerkbonnen { Header = "Laatste verzorging", PropertyName = "LaatsteVerzorging" };
            yield return new DataGridColumnWerkbonnen { Header = "Volgauto", PropertyName = "VolgAuto" };
            yield return new DataGridColumnWerkbonnen { Header = "Overbrengen", PropertyName = "Overbrengen" };
            yield return new DataGridColumnWerkbonnen { Header = "Condoleance", PropertyName = "Condoleance" };
            yield return new DataGridColumnWerkbonnen { Header = "Bedrag", PropertyName = "Bedrag" };
        }
        public class DataGridColumnWerkbonnen
        {
            public string Header { get; set; }
            public string PropertyName { get; set; }

            public object GetValue(WerkbonnenData item)
            {
                var property = item.GetType().GetProperty(PropertyName);
                return property?.GetValue(item);
            }
        }
        public void ExecuteSaveBloemenUitbetalenCommand(object obj)
        {
            CloseBloemenUitbetalenCommand.Execute(null);

            try
            {
                updateRepository.UpdateBloemenBetaling(SelectedBloemUitbetaling);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating bloemen betaling: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }
            finally
            {
                RefreshBloemenFinancieel();
            }
        }
        public void ExecuteSaveSteenhouwerUitbetalenCommand(object obj)
        {
            CloseSteenhouwerUitbetalenCommand.Execute(null);

            try
            {
                updateRepository.UpdateSteenhouwerijBetaling(SelectedSteenhouwerUitbetaling);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating steenhouwerij betaling: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }
            finally
            {
                RefreshSteenhouwerijFinancieel();
            }
        }
        public void ExecuteSaveUrnSieradenUitbetalenCommand(object obj)
        {
            CloseUrnSieradenUitbetalenCommand.Execute(null);

            try
            {
                updateRepository.UpdateUrnSieradenBetaling(SelectedUrnSieradenUitbetaling);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating urnSieraden betaling: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }
            finally
            {
                RefreshUrnSieradenFinancieel();
            }
        }
        public void KostenbegrotingGridData()
        {
            Kostenbegrotingen.Clear();

            foreach (var kostenbegroting in miscellaneousRepository.GetAllKostenbegrotingen())
            {
                if (kostenbegroting.KostenbegrotingCreated == true)
                {
                    Kostenbegrotingen.Add(new FactuurModel
                    {
                        Id = kostenbegroting.Id,
                        UitvaartId = kostenbegroting.UitvaartId,
                        UitvaartNummer = kostenbegroting.UitvaartNummer,
                        KostenbegrotingCreated = kostenbegroting.KostenbegrotingCreated,
                        KostenbegrotingCreationDate = kostenbegroting.KostenbegrotingCreationDate,
                        KostenbegrotingJson = kostenbegroting.KostenbegrotingJson,
                        KostenbegrotingUrl = kostenbegroting.KostenbegrotingUrl,
                        FactuurCreated = kostenbegroting.FactuurCreated,
                        FactuurCreationDate = kostenbegroting.FactuurCreationDate,
                        FactuurOpdrachtgeverUrl = kostenbegroting.FactuurOpdrachtgeverUrl,
                        FactuurVerenigingUrl = kostenbegroting.FactuurVerenigingUrl
                    });
                }
            }
        }
        public void WerkbonnenGridData()
        {
            WerkbonnenDataSet.Clear();

            var nestedWerkbonnenDataLists = searchRepository.GetOverlijdenWerkbonnen();

            foreach (var werkbonnenDataList in nestedWerkbonnenDataLists)
            {
                foreach (var werkbonnenData in werkbonnenDataList)
                {
                    WerkbonnenDataSet.Add(werkbonnenData);
                    FilteredWerkbonnenData.Add(werkbonnenData);
                }
            }
        }
        public void BloemenGridData()
        {
            BloemenFinancieel.Clear();

            foreach (var bloem in searchRepository.GetOverlijdenBloemen())
            {
                BloemenFinancieel.Add(new OverledeneBloemenModel
                {
                    BloemenId = bloem.BloemenId,
                    UitvaartId = bloem.UitvaartId,
                    UitvaartNummer = bloem.UitvaartNummer,
                    BloemenBedrag = bloem.BloemenBedrag,
                    BloemenProvisie = bloem.BloemenProvisie,
                    BloemenUitbetaling = bloem.BloemenUitbetaling,
                    BloemenLeverancierName = bloem.BloemenLeverancierName,
                    BloemenWerknemer = bloem.BloemenWerknemer,
                    BloemenPaid = bloem.BloemenPaid
                });
            }
        }
        public void SteenhouwerijGridData()
        {
            SteenhouwerijFinancieel.Clear();

            foreach (var steen in searchRepository.GetOverlijdenSteenhouwerij())
            {
                SteenhouwerijFinancieel.Add(new OverledeneSteenhouwerijModel
                {
                    SteenhouwerijId = steen.SteenhouwerijId,
                    UitvaartId = steen.UitvaartId,
                    UitvaartNummer = steen.UitvaartNummer,
                    SteenhouwerOpdracht = steen.SteenhouwerOpdracht,
                    SteenhouwerBedrag = steen.SteenhouwerBedrag,
                    SteenhouwerProvisie = steen.SteenhouwerProvisie,
                    SteenhouwerUitbetaing = steen.SteenhouwerUitbetaing,
                    SteenhouwerWerknemer = steen.SteenhouwerWerknemer,
                    SteenhouwerLeverancierName = steen.SteenhouwerLeverancierName,
                    SteenhouwerPaid = steen.SteenhouwerPaid
                });
            }
        }
        public void UrnSieradenGridData()
        {
            UrnSieradenFinancieel.Clear();

            foreach (var urnSieraden in searchRepository.GetOverlijdenUrnSieraden())
            {
                UrnSieradenFinancieel.Add(new OverledeneUrnSieradenModel
                {
                    UrnId = urnSieraden.UrnId,
                    UitvaartId = urnSieraden.UitvaartId,
                    UitvaartNummer = urnSieraden.UitvaartNummer,
                    UrnOpdracht = urnSieraden.UrnOpdracht,
                    UrnBedrag = urnSieraden.UrnBedrag,
                    UrnProvisie = urnSieraden.UrnProvisie,
                    UrnUitbetaing = urnSieraden.UrnUitbetaing,
                    UrnWerknemer = urnSieraden.UrnWerknemer,
                    UrnLeverancierName = urnSieraden.UrnLeverancierName,
                    UrnPaid = urnSieraden.UrnPaid
                });
            }
        }
        private void UpdateFilter()
        {
            FilteredSteenhouwerijFinancieel.Clear();
            FilteredBloemenFinancieel.Clear();
            FilteredWerkbonnenData.Clear();
            FilteredUrnSieradenFinancieel.Clear();

            foreach (var steenItem in SteenhouwerijFinancieel)
            {
                if (RangeUtility.IsInRange(steenItem.UitvaartNummer, StartUitvaartNumber, EndUitvaartNumber))
                    FilteredSteenhouwerijFinancieel.Add(steenItem);
            }

            foreach (var bloemItem in BloemenFinancieel)
            {

                if (RangeUtility.IsInRange(bloemItem.UitvaartNummer, StartUitvaartNumber, EndUitvaartNumber))
                    FilteredBloemenFinancieel.Add(bloemItem);
            }

            foreach (var bonItem in WerkbonnenDataSet)
            {
                if (RangeUtility.IsInRange(bonItem.UitvaartNummer, StartUitvaartNumber, EndUitvaartNumber))
                    FilteredWerkbonnenData.Add(bonItem);
            }

            foreach (var item in UrnSieradenFinancieel)
            {
                if (RangeUtility.IsInRange(item.UitvaartNummer, StartUitvaartNumber, EndUitvaartNumber))
                    FilteredUrnSieradenFinancieel.Add(item);
            }
        }
        private Task<string> CreateFactuurFile(string factuurSuffix)
        {
            return Task.Run(() =>
            {
                string destinationFile = string.Empty;
                string sourceLoc = DataProvider.FactuurOpslag;
                string templateLoc = DataProvider.TemplateFolder;

                bool sourceLocEndSlash = sourceLoc.EndsWith(@"\");

                if (!sourceLocEndSlash)
                    sourceLoc += "\\";

                bool templateLocatEndSlash = templateLoc.EndsWith(@"\");
                if (!templateLocatEndSlash)
                    templateLoc += "\\";

                string fileToCopy = templateLoc + "Factuur.xls";

                string destinationLoc = sourceLoc;

                bool destinationLocEndSlash = destinationLoc.EndsWith(@"\");
                if (!destinationLocEndSlash)
                    destinationLoc += "\\";

                FileInfo sourceFile = new(fileToCopy);

                destinationFile = destinationLoc + "Factuur_" + GenerateSelectedFactuur.UitvaartNummer + factuurSuffix + "_" + GenerateSelectedFactuur.OverledeneAchternaam + ".xls";

                if (sourceFile.Exists)
                {
                    if (!Directory.Exists(destinationLoc))
                    {
                        Directory.CreateDirectory(destinationLoc);
                    }
                    if (!File.Exists(destinationFile))
                    {
                        sourceFile.CopyTo(destinationFile);
                    }
                }

                return destinationFile;

            });
        }
        private Task FillFactuurFile(Guid UitvaartCodeGuid, string factuurUrl, string kostenbegrotingJson, string verzekeringJson, string splitFactuur)
        {
            double subtotalAmount = 0.0;
            double minderingAmount = 0.0;
            double totalAmount = 0.0;
            int excelRow = 22;

            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = false;
            Excel.Workbook workbook = excelApp.Workbooks.Open(factuurUrl);
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.ActiveSheet;

            string[] firstnames = GenerateSelectedFactuur.OpdrachtgeverVoornamen.Split(' ');

            string initials = string.Empty;
            string achternaam = string.Empty;
            string huisnummer = string.Empty;

            foreach (string name in firstnames)
            {
                if (name.Length > 0)
                    initials += name[0];
            }

            if (!string.IsNullOrWhiteSpace(GenerateSelectedFactuur.OpdrachtgeverTussenvoegsel))
            {
                achternaam = GenerateSelectedFactuur.OpdrachtgeverTussenvoegsel + " " + GenerateSelectedFactuur.OpdrachtgeverAchternaam;
            }
            else
            {
                achternaam = GenerateSelectedFactuur.OpdrachtgeverAchternaam;
            }

            if (!string.IsNullOrWhiteSpace(GenerateSelectedFactuur.OpdrachtgeverHuisnummerToevoeging))
            {
                huisnummer = GenerateSelectedFactuur.OpdrachtgeverHuisnummer + " " + GenerateSelectedFactuur.OpdrachtgeverHuisnummerToevoeging;
            }
            else
            {
                huisnummer = GenerateSelectedFactuur.OpdrachtgeverHuisnummer;
            }

            string Opdrachtgever = GenerateSelectedFactuur.OpdrachtgeverAanhef + " " + initials + " " + achternaam;
            string OrganizationStreet = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber}";

            if (!string.IsNullOrEmpty(DataProvider.OrganizationHouseNumberAddition))
                OrganizationStreet = $"{DataProvider.OrganizationStreet} {DataProvider.OrganizationHouseNumber} {DataProvider.OrganizationHouseNumberAddition}";

            worksheet.Cells[9, 2] = OrganizationStreet;
            worksheet.Cells[10, 2] = $"{DataProvider.OrganizationZipcode} {DataProvider.OrganizationCity}";
            worksheet.Cells[11, 2] = $"Tel: {DataProvider.OrganizationPhoneNumber}";

            if (splitFactuur == "Vereniging")
            {
                worksheet.Cells[18, 4] = GenerateSelectedFactuur.UitvaartNummer + "A - Gelieve dit nummer bij betaling vermelden.";
            }
            else
            {
                worksheet.Cells[18, 4] = GenerateSelectedFactuur.UitvaartNummer + " - Gelieve dit nummer bij betaling vermelden.";
            }

            if (GenerateSelectedFactuur.OverledeneVoorregeling)
            {
                worksheet.Cells[20, 2] = "Onderstaande kosten (depotstorting) zijn opgemaakt voor de toekomstige uitvaart van " + GenerateSelectedFactuur.OverledeneAanhef + " " + GenerateSelectedFactuur.OverledeneVoornamen + " " + GenerateSelectedFactuur.OverledeneAchternaam;
                try
                {
                    Excel.Shape shape = worksheet.Shapes.Item("FactuurTypeBox");
                    if (shape != null)
                        shape.TextFrame2.TextRange.Text = "DEPOT Storting";
                }
                catch (Exception ex)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                }
            }
            else
            {
                worksheet.Cells[20, 2] = "Onderstaande kosten zijn gemaakt voor de uitvaart van " + GenerateSelectedFactuur.OverledeneAanhef + " " + GenerateSelectedFactuur.OverledeneVoornamen + " " + GenerateSelectedFactuur.OverledeneAchternaam + " (overleden " + GenerateSelectedFactuur.OverledeneOpDatum.ToString()[..10] + ").";
            }

            Excel.Range overledenCell = (Excel.Range)worksheet.Cells[20, 4];

            int aanhefLength = GenerateSelectedFactuur.OverledeneAanhef.Length;
            int voornamenLength = GenerateSelectedFactuur.OverledeneVoornamen.Length;
            int achternaamLength = GenerateSelectedFactuur.OverledeneAchternaam.Length;
            int opDatumLength = 10;

            overledenCell.Characters[1, aanhefLength].Font.Bold = true;
            overledenCell.Characters[aanhefLength + 2, aanhefLength + 2 + voornamenLength].Font.Bold = true;
            overledenCell.Characters[aanhefLength + 3 + voornamenLength, aanhefLength + 3 + voornamenLength + achternaamLength].Font.Bold = true;
            overledenCell.Characters[aanhefLength + 3 + voornamenLength + achternaamLength + 2, aanhefLength + 3 + voornamenLength + achternaamLength + 2 + opDatumLength].Font.Bold = true;
            overledenCell.EntireColumn.AutoFit();

            if (splitFactuur == string.Empty)
            {
                if (GenerateSelectedFactuur.FactuurType == "Vereniging")
                {
                    switch (GenerateSelectedFactuur.CorrespondentieType)
                    {
                        // Adres = Adres, Huisnummer, Plaats
                        case "Adres":
                            worksheet.Cells[12, 6] = GenerateSelectedFactuur.HerkomstName;
                            worksheet.Cells[13, 6] = GenerateSelectedFactuur.HerkomstStreet + " " + GenerateSelectedFactuur.HerkomstHousenumber + " " + GenerateSelectedFactuur.HerkomstHousenumberAddition;
                            worksheet.Cells[14, 6] = GenerateSelectedFactuur.HerkomstZipcode + " " + GenerateSelectedFactuur.HerkomstCity;
                            break;

                        // Postbus = PostbusNaam, Postbus Adres
                        case "Postbus":
                            worksheet.Cells[12, 6] = GenerateSelectedFactuur.HerkomstPostbusName;
                            worksheet.Cells[13, 6] = GenerateSelectedFactuur.HerkomstPostbus;
                            break;

                        //null = lege waardes
                        default:
                            worksheet.Cells[12, 6] = "Opdrachtgever Naam";
                            worksheet.Cells[13, 6] = "Opdrachtgever Straat, Huisnummer, Toevoeging";
                            worksheet.Cells[14, 6] = "Opdrachtgever Postcode, Plaats";
                            break;
                    }
                }
                else
                {

                    worksheet.Cells[12, 6] = Opdrachtgever;
                    worksheet.Cells[13, 6] = GenerateSelectedFactuur.OpdrachtgeverStraat + " " + huisnummer;
                    worksheet.Cells[14, 6] = GenerateSelectedFactuur.OpdrachtgeverPostcode + " " + GenerateSelectedFactuur.OpdrachtgeverWoonplaats;
                }

                foreach (var priceComponent in JsonConvert.DeserializeObject<List<GeneratedKostenbegrotingModel>>(kostenbegrotingJson))
                {
                    if ((priceComponent.Bedrag.HasValue && priceComponent.Bedrag.Value != 0m) || priceComponent.PrintTrue)
                    {
                        Excel.Range cell = (Excel.Range)worksheet.Cells[excelRow, 2];
                        cell.Value = string.IsNullOrEmpty(priceComponent.Aantal) || priceComponent.Aantal == "0" ? priceComponent.Omschrijving : priceComponent.Aantal + "  " + priceComponent.Omschrijving;

                        if (cell.Value != null && cell.Value.ToString().Length > 98)
                        {
                            ((Excel.Range)worksheet.Rows[excelRow]).RowHeight = 36;
                            cell.WrapText = true;
                        }
                        else
                        {
                            ((Excel.Range)worksheet.Rows[excelRow]).RowHeight = 15;
                            cell.WrapText = false;
                        }
                        cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        Excel.Range currencyCellComp = (Excel.Range)worksheet.Cells[excelRow, 8];
                        // Use a null check for FactuurBedrag and assign default value if needed
                        currencyCellComp.Value = priceComponent.FactuurBedrag.HasValue && priceComponent.FactuurBedrag != decimal.Zero
                            ? priceComponent.FactuurBedrag.Value
                            : priceComponent.Bedrag ?? 0m;  // Use the null-coalescing operator to ensure a valid value

                        currencyCellComp.NumberFormat = "_-€ * #,##0.00_-;_-€ * #,##0.00_-;_-€ * \"-\"??_-;_-@_-";

                        currencyCellComp.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        Excel.Range mergeRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 7]];
                        mergeRange.Merge();
                        mergeRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        excelRow++;
                        ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);


                        subtotalAmount += (double)(priceComponent.Bedrag ?? 0m);  // Safe handling for null
                    }
                }
            }
            else if (splitFactuur == "Opdrachtgever")
            {
                worksheet.Cells[12, 6] = Opdrachtgever;
                worksheet.Cells[13, 6] = GenerateSelectedFactuur.OpdrachtgeverStraat + " " + huisnummer;
                worksheet.Cells[14, 6] = GenerateSelectedFactuur.OpdrachtgeverPostcode + " " + GenerateSelectedFactuur.OpdrachtgeverWoonplaats;

                foreach (var priceComponent in JsonConvert.DeserializeObject<List<GeneratedKostenbegrotingModel>>(kostenbegrotingJson))
                {
                    if (string.IsNullOrEmpty(priceComponent.Verzekerd) && priceComponent.Bedrag != 0m)
                    {
                        Excel.Range cell = (Excel.Range)worksheet.Cells[excelRow, 2];
                        cell.Value = string.IsNullOrEmpty(priceComponent.Aantal) ? priceComponent.Omschrijving : priceComponent.Aantal + "  " + priceComponent.Omschrijving;

                        if (cell.Value != null && cell.Value.ToString().Length > 98)
                        {
                            ((Excel.Range)worksheet.Rows[excelRow]).RowHeight = 36;
                            cell.WrapText = true;
                        }
                        else
                        {
                            ((Excel.Range)worksheet.Rows[excelRow]).RowHeight = 15;
                            cell.WrapText = false;
                        }
                        cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        Excel.Range currencyCellComp = (Excel.Range)worksheet.Cells[excelRow, 8];
                        currencyCellComp.Value = priceComponent.Bedrag;

                        currencyCellComp.NumberFormat = "_-€ * #,##0.00_-;_-€ * #,##0.00_-;_-€ * \"-\"??_-;_-@_-";

                        worksheet.Cells[excelRow, 9] = priceComponent.Verzekerd;

                        Excel.Range mergeRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 7]];
                        mergeRange.Merge();
                        mergeRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        excelRow++;
                        ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);

                        subtotalAmount += (double)priceComponent.Bedrag;
                    }
                }
            }
            else if (splitFactuur == "Vereniging")
            {
                switch (GenerateSelectedFactuur.CorrespondentieType)
                {
                    // 1 = Adres, Huisnummer, Plaats
                    case "Adres":
                        worksheet.Cells[12, 6] = GenerateSelectedFactuur.HerkomstName;
                        worksheet.Cells[13, 6] = GenerateSelectedFactuur.HerkomstStreet + " " + GenerateSelectedFactuur.HerkomstHousenumber + " " + GenerateSelectedFactuur.HerkomstHousenumberAddition;
                        worksheet.Cells[14, 6] = GenerateSelectedFactuur.HerkomstZipcode + " " + GenerateSelectedFactuur.HerkomstCity;
                        break;

                    // 2 = PostbusNaam, Postbus Adres
                    case "Postbus":
                        worksheet.Cells[12, 6] = GenerateSelectedFactuur.HerkomstPostbusName;
                        worksheet.Cells[13, 6] = GenerateSelectedFactuur.HerkomstPostbus;
                        break;

                    //null = lege waardes
                    default:
                        worksheet.Cells[12, 6] = "Opdrachtgever Naam";
                        worksheet.Cells[13, 6] = "Opdrachtgever Straat, Huisnummer, Toevoeging";
                        worksheet.Cells[14, 6] = "Opdrachtgever Postcode, Plaats";
                        break;
                }

                foreach (var priceComponent in JsonConvert.DeserializeObject<List<GeneratedKostenbegrotingModel>>(kostenbegrotingJson))
                {
                    if (!string.IsNullOrEmpty(priceComponent.Verzekerd))
                    {
                        Excel.Range cell = (Excel.Range)worksheet.Cells[excelRow, 2];
                        cell.Value = string.IsNullOrEmpty(priceComponent.Aantal) || priceComponent.Aantal == "0"
                        ? priceComponent.Omschrijving
                        : priceComponent.Aantal + "  " + priceComponent.Omschrijving;

                        if (cell.Value != null && cell.Value.ToString().Length > 98)
                        {
                            ((Excel.Range)worksheet.Rows[excelRow]).RowHeight = 36;
                            cell.WrapText = true;
                        }
                        else
                        {
                            ((Excel.Range)worksheet.Rows[excelRow]).RowHeight = 15;
                            cell.WrapText = false;
                        }
                        cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        Excel.Range currencyCellComp = (Excel.Range)worksheet.Cells[excelRow, 8];
                        currencyCellComp.Value = priceComponent.Bedrag;
                        currencyCellComp.NumberFormat = "_-€ * #,##0.00_-;_-€ * #,##0.00_-;_-€ * \"-\"??_-;_-@_-";
                        currencyCellComp.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;


                        worksheet.Cells[excelRow, 9] = priceComponent.Verzekerd;

                        Excel.Range mergeRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 7]];
                        mergeRange.Merge();
                        mergeRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                        excelRow++;
                        ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);

                        subtotalAmount += (double)priceComponent.Bedrag;
                    }
                }
            }

            excelRow++;
            ((Excel.Range)worksheet.Rows[excelRow]).RowHeight = 15;
            ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);
            Excel.Range mergeTotalRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 7]];
            mergeTotalRange.Merge();
            mergeTotalRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

            mergeTotalRange.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            mergeTotalRange.Borders[Excel.XlBordersIndex.xlEdgeTop].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            mergeTotalRange.Borders[Excel.XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            mergeTotalRange.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;

            mergeTotalRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            mergeTotalRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            mergeTotalRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            mergeTotalRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;

            mergeTotalRange.Value = "Totaal diensten:";

            Excel.Range currencyCellTotal = (Excel.Range)worksheet.Cells[excelRow, 8];
            currencyCellTotal.Value = subtotalAmount;
            currencyCellTotal.NumberFormat = "_-€ * #,##0.00_-;_-€ * #,##0.00_-;_-€ * \"-\"??_-;_-@_-";
            currencyCellTotal.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            currencyCellTotal.Borders[Excel.XlBordersIndex.xlEdgeTop].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            currencyCellTotal.Borders[Excel.XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            currencyCellTotal.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;

            currencyCellTotal.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            currencyCellTotal.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            currencyCellTotal.Borders[Excel.XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            currencyCellTotal.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;


            excelRow++;
            ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);
            Excel.Range mergeMinderingRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 8]];
            mergeMinderingRange.Merge();
            mergeMinderingRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            mergeMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            mergeMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeTop].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            mergeMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            mergeMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThick;

            mergeMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            mergeMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            mergeMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            mergeMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThick;

            mergeMinderingRange.Value = "In mindering";
            ((Excel.Range)worksheet.Rows[excelRow]).Font.Bold = true;

            var polisVerzekeringList = JsonConvert.DeserializeObject<List<PolisVerzekering>>(GenerateSelectedFactuur.OverledeneVerzekeringJson);

            foreach (var verzekering in polisVerzekeringList.Where(v => v.PolisInfoList.Any(p => p.PolisNr != null || p.PolisBedrag != null)))
            {
                foreach (var polis in verzekering.PolisInfoList.Where(p => p.PolisNr != null || p.PolisBedrag != null))
                {
                    excelRow++;
                    ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                    Excel.Range mergeVerzekeringRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 7]];
                    mergeVerzekeringRange.Merge();
                    mergeVerzekeringRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                    mergeVerzekeringRange.Value = verzekering.VerzekeringName + " " + polis.PolisNr;

                    Excel.Range currencyCell = (Excel.Range)worksheet.Cells[excelRow, 8];
                    currencyCell.Value = polis.PolisBedrag;
                    currencyCell.NumberFormat = "_-€ * #,##0.00_-;_-€ * #,##0.00_-;_-€ * \"-\"??_-;_-@_-";
                    ;
                    if (double.TryParse(polis.PolisBedrag, out double doubleValue))
                    {
                        minderingAmount += doubleValue;
                    }
                }
            }

            excelRow++;
            ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);
            Excel.Range mergeTotaalMinderingRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 7]];
            mergeTotaalMinderingRange.Merge();
            mergeTotaalMinderingRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            mergeTotaalMinderingRange.Value = "Totaal in mindering:";

            mergeTotaalMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            mergeTotaalMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeTop].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            mergeTotaalMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            mergeTotaalMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;

            mergeTotaalMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            mergeTotaalMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            mergeTotaalMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            mergeTotaalMinderingRange.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;

            Excel.Range currencyTotaalMinderingCell = (Excel.Range)worksheet.Cells[excelRow, 8];
            currencyTotaalMinderingCell.Value = minderingAmount;
            currencyTotaalMinderingCell.NumberFormat = "_-€ * #,##0.00_-;_-€ * #,##0.00_-;_-€ * \"-\"??_-;_-@_-";
            currencyTotaalMinderingCell.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            currencyTotaalMinderingCell.Borders[Excel.XlBordersIndex.xlEdgeTop].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            currencyTotaalMinderingCell.Borders[Excel.XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            currencyTotaalMinderingCell.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThin;

            currencyTotaalMinderingCell.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            currencyTotaalMinderingCell.Borders[Excel.XlBordersIndex.xlEdgeBottom].ColorIndex = Excel.XlColorIndex.xlColorIndexAutomatic;
            currencyTotaalMinderingCell.Borders[Excel.XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            currencyTotaalMinderingCell.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThin;


            excelRow++;
            ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);

            Excel.Range mergeTotaalRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 7]];
            mergeTotaalRange.Merge();
            mergeTotaalRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            mergeTotaalRange.Value = "Totaal";

            totalAmount = subtotalAmount - minderingAmount;

            Excel.Range currencyTotaalCell = (Excel.Range)worksheet.Cells[excelRow, 8];
            currencyTotaalCell.Value = totalAmount;
            currencyTotaalCell.NumberFormat = "_-€ * #,##0.00_-;_-€ * #,##0.00_-;_-€ * \"-\"??_-;_-@_-";
            ((Excel.Range)worksheet.Rows[excelRow]).Font.Bold = true;

            excelRow++;
            Excel.Range mergeDisclaimerRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 8]];
            mergeDisclaimerRange.Merge();
            mergeDisclaimerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            mergeDisclaimerRange.Value = "Wij danken u voor het door u gestelde vertrouwen.";
            ((Excel.Range)worksheet.Rows[excelRow]).Font.Bold = true;

            excelRow++;
            Excel.Range mergeDisclaimerPart2Range = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow + 4, 8]];
            mergeDisclaimerPart2Range.Merge();
            mergeDisclaimerPart2Range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            mergeDisclaimerPart2Range.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
            mergeDisclaimerPart2Range.Value = $@"Wij verzoeken U bovengenoemd bedrag over te maken op ons bankrekeningnummer t.n.v. {DataProvider.OrganizationName} onder vermelding van het bovenstaande factuurnummer.
                                            Betalingen dienen binnen 14 dagen te zijn voldaan.
                                            Algemene leveringsvoorwaarden gedeponeerd bij de Kamer van Koophandel Noord Nederland onder nummer 02062153";

            excelRow++;

            worksheet.PageSetup.PrintArea = $"A1:H{excelRow + 7}";
            worksheet.HPageBreaks.Add(worksheet.Rows[excelRow + 7]);

            // Set a scaling factor manually (e.g., 100% = no scaling)
            worksheet.PageSetup.Zoom = 100;

            // Optionally clear FitToPages settings
            worksheet.PageSetup.FitToPagesWide = false;
            worksheet.PageSetup.FitToPagesTall = false;




            // AutoFit the column width
            //worksheet.Cells.EntireColumn.AutoFit();

            workbook.Close(true);
            excelApp.Quit();

            Process.Start(new ProcessStartInfo
            {
                FileName = factuurUrl,
                UseShellExecute = true
            });

            return Task.CompletedTask;
        }
        private void RefreshSteenhouwerijFinancieel()
        {
            FilteredSteenhouwerijFinancieel.Clear();

            foreach (var steen in searchRepository.GetOverlijdenSteenhouwerij())
            {
                FilteredSteenhouwerijFinancieel.Add(new OverledeneSteenhouwerijModel
                {
                    SteenhouwerijId = steen.SteenhouwerijId,
                    UitvaartId = steen.UitvaartId,
                    UitvaartNummer = steen.UitvaartNummer,
                    SteenhouwerOpdracht = steen.SteenhouwerOpdracht,
                    SteenhouwerBedrag = steen.SteenhouwerBedrag,
                    SteenhouwerProvisie = steen.SteenhouwerProvisie,
                    SteenhouwerUitbetaing = steen.SteenhouwerUitbetaing,
                    SteenhouwerWerknemer = steen.SteenhouwerWerknemer,
                    SteenhouwerLeverancierName = steen.SteenhouwerLeverancierName,
                    SteenhouwerPaid = steen.SteenhouwerPaid
                });
            }

        }
        private void RefreshBloemenFinancieel()
        {
            FilteredBloemenFinancieel.Clear();

            foreach (var bloem in searchRepository.GetOverlijdenBloemen())
            {
                FilteredBloemenFinancieel.Add(new OverledeneBloemenModel
                {
                    BloemenId = bloem.BloemenId,
                    UitvaartId = bloem.UitvaartId,
                    UitvaartNummer = bloem.UitvaartNummer,
                    BloemenBedrag = bloem.BloemenBedrag,
                    BloemenProvisie = bloem.BloemenProvisie,
                    BloemenUitbetaling = bloem.BloemenUitbetaling,
                    BloemenLeverancierName = bloem.BloemenLeverancierName,
                    BloemenWerknemer = bloem.BloemenWerknemer,
                    BloemenPaid = bloem.BloemenPaid
                });
            }
        }
        private void RefreshUrnSieradenFinancieel()
        {
            FilteredUrnSieradenFinancieel.Clear();

            foreach (var urnSieraden in searchRepository.GetOverlijdenUrnSieraden())
            {
                FilteredUrnSieradenFinancieel.Add(new OverledeneUrnSieradenModel
                {
                    UrnId = urnSieraden.UrnId,
                    UitvaartId = urnSieraden.UitvaartId,
                    UitvaartNummer = urnSieraden.UitvaartNummer,
                    UrnOpdracht = urnSieraden.UrnOpdracht,
                    UrnBedrag = urnSieraden.UrnBedrag,
                    UrnProvisie = urnSieraden.UrnProvisie,
                    UrnUitbetaing = urnSieraden.UrnUitbetaing,
                    UrnWerknemer = urnSieraden.UrnWerknemer,
                    UrnLeverancierName = urnSieraden.UrnLeverancierName,
                    UrnPaid = urnSieraden.UrnPaid
                });
            }
        }
    }
}
