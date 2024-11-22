using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationRapportagesViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        public ICommand CloseFilterRapportagePopupOpenCommand { get; }
        public ICommand OpenFilterRapportagePopupOpenCommand { get; }
        public ICommand OpenEmployeeScoreCommand { get; }
        public ICommand CloseEmployeeScoreCommand { get; }
        public ICommand FilterRapportageCommand { get; }
        public ICommand RefreshRapportageGridCommand { get; set; }
        public ICommand OpenOpdrachtgeverFactuurCommand { get; set; }
        public ICommand OpenHerkomstFactuurCommand { get; set; }

        private ObservableCollection<ISeries> _kistenSeries;
        private ObservableCollection<ISeries> _verzekeringSeries;
        private ObservableCollection<ISeries> _uitvaartleiderSeries;
        private ObservableCollection<Volgautos> _volgAutosRapportages;
        private ObservableCollection<RapportagesVerzekering> _verzekeringRapportages;
        private ObservableCollection<RapportagesVerzekering> _verzekeringWoonplaatsRapportages;
        private ObservableCollection<RapportagesUitvaartleider> _uitvaartleiderRapportages;
        private ObservableCollection<RapportagesKisten> _kistenRapportages;
        private ObservableCollection<PeriodeLijst> _periodeLijst;
        private ObservableCollection<WerkbonnenData> _werkbonnenDataSet;
        private ObservableCollection<RapportageKlantWerknemerScores> _allWerknemerScore;
        private ObservableCollection<RapportageKlantWerknemerScores> _werknemerScore;
        private ObservableCollection<WerkbonnenData> _filteredWerkbonnenData;
        private ObservableCollection<OverledeneBloemenModel> _filteredBloemenFinancieel;
        private ObservableCollection<OverledeneSteenhouwerijModel> _filteredSteenhouwerijFinancieel;
        private ObservableCollection<OverledeneUrnSieradenModel> _filteredUrnSieradenFinancieel;
        private ObservableCollection<OverledeneBloemenModel> _bloemenFinancieel;
        private ObservableCollection<OverledeneSteenhouwerijModel> _steenhouwerijFinancieel;
        private ObservableCollection<OverledeneUrnSieradenModel> _urnSieradenFinancieel;
        private ObservableCollection<OverledeneWerkbonUitvaart> _werkbonnenFinancieel;

        private RapportagesFilter _uitvaartnummerFilter;
        private RapportageKlantWerknemerScores _selectedEmployeeScore;

        private LabelVisual _kistenTitle;
        private LabelVisual _verzekeringTitle;
        private LabelVisual _uitvaartleiderTitle;
        private List<Axis> _xAxes;
        private List<Axis> _yAxes;

        private bool _isKistenChartVisible = true;
        private bool _isUitvaartleiderChartVisible = true;
        private bool _isVerzekeringChartVisible = true;
        private bool isFilterRapportagePopupOpen;
        private bool isShowEmployeeScorePopupOpen;

        private string _startUitvaartNumber = "1";
        private string _endUitvaartNumber = int.MaxValue.ToString();
        private string _selectedScoreEmployeeName = string.Empty;

        public RapportagesFilter UitvaartnummerFilter
        {
            get { return _uitvaartnummerFilter; }
            set
            {
                if (_uitvaartnummerFilter != value)
                {
                    _uitvaartnummerFilter = value; OnPropertyChanged(nameof(UitvaartnummerFilter));
                }
            }
        }
        public RapportageKlantWerknemerScores SelectedEmployeeScore
        {
            get { return _selectedEmployeeScore; }
            set
            {
                if (_selectedEmployeeScore != value)
                {
                    _selectedEmployeeScore = value;
                    OnPropertyChanged(nameof(SelectedEmployeeScore));
                }
            }
        }
        public ObservableCollection<Volgautos> VolgAutosRapportage
        {
            get { return _volgAutosRapportages; }
            set
            {
                if (_volgAutosRapportages != value)
                {
                    _volgAutosRapportages = value;
                    OnPropertyChanged(nameof(VolgAutosRapportage));
                }
            }
        }
        public ObservableCollection<RapportagesVerzekering> VerzekeringRapportages
        {
            get { return _verzekeringRapportages; }
            set
            {
                if (_verzekeringRapportages != value)
                {
                    _verzekeringRapportages = value;
                    OnPropertyChanged(nameof(VerzekeringRapportages));
                }
            }
        }
        public ObservableCollection<RapportagesVerzekering> VerzekeringWoonplaatsRapportages
        {
            get { return _verzekeringWoonplaatsRapportages; }
            set
            {
                if (_verzekeringWoonplaatsRapportages != value)
                {
                    _verzekeringWoonplaatsRapportages = value;
                    OnPropertyChanged(nameof(VerzekeringWoonplaatsRapportages));
                }
            }
        }
        public ObservableCollection<RapportagesUitvaartleider> UitvaartleiderRapportages
        {
            get { return _uitvaartleiderRapportages; }
            set
            {
                if (_uitvaartleiderRapportages != value)
                {
                    _uitvaartleiderRapportages = value;
                    OnPropertyChanged(nameof(UitvaartleiderRapportages));
                }
            }
        }
        public ObservableCollection<RapportagesKisten> KistenRapportages
        {
            get { return _kistenRapportages; }
            set
            {
                if (_kistenRapportages != value)
                {
                    _kistenRapportages = value;
                    OnPropertyChanged(nameof(KistenRapportages));
                }
            }
        }
        public ObservableCollection<PeriodeLijst> PeriodeLijst
        {
            get { return _periodeLijst; }
            set
            {
                if (_periodeLijst != value)
                {
                    _periodeLijst = value;
                    OnPropertyChanged(nameof(PeriodeLijst));
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
        public ObservableCollection<ISeries> UitvaartleiderSeries
        {
            get => _uitvaartleiderSeries;
            set
            {
                if (_uitvaartleiderSeries != value)
                {
                    _uitvaartleiderSeries = value;
                    OnPropertyChanged(nameof(UitvaartleiderSeries));
                }
            }
        }
        public ObservableCollection<ISeries> KistenSeries
        {
            get => _kistenSeries;
            set
            {
                if (_kistenSeries != value)
                {
                    _kistenSeries = value;
                    OnPropertyChanged(nameof(KistenSeries));
                }
            }
        }
        public ObservableCollection<ISeries> VerzekeringSeries
        {
            get => _verzekeringSeries;
            set
            {
                if (_verzekeringSeries != value)
                {
                    _verzekeringSeries = value;
                    OnPropertyChanged(nameof(VerzekeringSeries));
                }
            }
        }
        public ObservableCollection<RapportageKlantWerknemerScores> AllWerknemerScore
        {
            get => _allWerknemerScore;
            set
            {
                if (_allWerknemerScore != value)
                {
                    _allWerknemerScore = value;
                    OnPropertyChanged(nameof(AllWerknemerScore));
                }
            }
        }
        public ObservableCollection<RapportageKlantWerknemerScores> WerknemerScore
        {
            get => _werknemerScore;
            set
            {
                if (_werknemerScore != value)
                {
                    _werknemerScore = value;
                    OnPropertyChanged(nameof(WerknemerScore));
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

        public LabelVisual KistenTitle
        {
            get => _kistenTitle;
            set
            {
                if (_kistenTitle != value)
                {
                    _kistenTitle = value;
                    OnPropertyChanged(nameof(KistenTitle));
                }
            }
        }
        public LabelVisual VerzekeringTitle
        {
            get => _verzekeringTitle;
            set
            {
                if (_verzekeringTitle != value)
                {
                    _verzekeringTitle = value;
                    OnPropertyChanged(nameof(VerzekeringTitle));
                }
            }
        }
        public LabelVisual UitvaartleiderTitle
        {
            get => _uitvaartleiderTitle;
            set
            {
                if (_uitvaartleiderTitle != value)
                {
                    _uitvaartleiderTitle = value;
                    OnPropertyChanged(nameof(UitvaartleiderTitle));
                }
            }
        }
        public List<Axis> XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                OnPropertyChanged(nameof(XAxes));
            }
        }
        public List<Axis> YAxes
        {
            get => _yAxes;
            set
            {
                _yAxes = value;
                OnPropertyChanged(nameof(YAxes));
            }
        }

        public bool IsKistenChartVisible
        {
            get { return _isKistenChartVisible; }
            set
            {
                if (_isKistenChartVisible != value)
                {
                    _isKistenChartVisible = value;
                    OnPropertyChanged(nameof(IsKistenChartVisible));
                }
            }
        }
        public bool IsUitvaartleiderChartVisible
        {
            get { return _isUitvaartleiderChartVisible; }
            set
            {
                if (_isUitvaartleiderChartVisible != value)
                {
                    _isUitvaartleiderChartVisible = value;
                    OnPropertyChanged(nameof(IsUitvaartleiderChartVisible));
                }
            }
        }
        public bool IsVerzekeringChartVisible
        {
            get { return _isVerzekeringChartVisible; }
            set
            {
                if (_isVerzekeringChartVisible != value)
                {
                    _isVerzekeringChartVisible = value;
                    OnPropertyChanged(nameof(IsVerzekeringChartVisible));
                }
            }
        }
        public bool IsFilterRapportagePopupOpen
        {
            get { return isFilterRapportagePopupOpen; }
            set
            {
                if (isFilterRapportagePopupOpen != value)
                {
                    isFilterRapportagePopupOpen = value;
                    OnPropertyChanged(nameof(IsFilterRapportagePopupOpen));
                }
            }
        }
        public bool IsShowEmployeeScorePopupOpen
        {
            get { return isShowEmployeeScorePopupOpen; }
            set
            {
                if (isShowEmployeeScorePopupOpen != value)
                {
                    isShowEmployeeScorePopupOpen = value;
                    OnPropertyChanged(nameof(IsShowEmployeeScorePopupOpen));
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
        public string SelectedScoreEmployeeName
        {
            get { return _selectedScoreEmployeeName; }
            set
            {
                if (_selectedScoreEmployeeName != value)
                {
                    _selectedScoreEmployeeName = value;
                    OnPropertyChanged(nameof(SelectedScoreEmployeeName));
                }
            }
        }
        public ConfigurationRapportagesViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();

            UitvaartnummerFilter = new RapportagesFilter();
            SelectedEmployeeScore = new RapportageKlantWerknemerScores();
            VolgAutosRapportage = new ObservableCollection<Volgautos>();
            UitvaartleiderRapportages = new ObservableCollection<RapportagesUitvaartleider>();
            VerzekeringRapportages = new ObservableCollection<RapportagesVerzekering>();
            VerzekeringWoonplaatsRapportages = new ObservableCollection<RapportagesVerzekering>();
            KistenRapportages = new ObservableCollection<RapportagesKisten>();
            PeriodeLijst = new ObservableCollection<PeriodeLijst>();
            AllWerknemerScore = new ObservableCollection<RapportageKlantWerknemerScores>();
            WerknemerScore = new ObservableCollection<RapportageKlantWerknemerScores>();

            UitvaartleiderSeries = new ObservableCollection<ISeries>();
            VerzekeringSeries = new ObservableCollection<ISeries>();
            KistenSeries = new ObservableCollection<ISeries>();
            YAxes = new List<Axis>();
            XAxes = new List<Axis>();

            FilterRapportageCommand = new ViewModelCommand(ExecuteFilterRapportageCommand);
            OpenEmployeeScoreCommand = new ViewModelCommand(ExecuteOpenEmployeeScoreCommand);
            OpenHerkomstFactuurCommand = new ViewModelCommand(ExecuteOpenFactuurCommand);
            OpenOpdrachtgeverFactuurCommand = new ViewModelCommand(ExecuteOpenFactuurCommand);
            OpenFilterRapportagePopupOpenCommand = new RelayCommand(() => IsFilterRapportagePopupOpen = true);
            CloseFilterRapportagePopupOpenCommand = new RelayCommand(() => IsFilterRapportagePopupOpen = false);
            CloseEmployeeScoreCommand = new RelayCommand(() => IsShowEmployeeScorePopupOpen = false);
            RefreshRapportageGridCommand = new RelayCommand(() => RefreshRapportageGrid());

            GetAllWerknemersScores();
            KistenRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            VerzekeringRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            VerzekeringWoonplaatsRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            UitvaartleiderRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            VolgautosRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            PeriodeLijstRapportageData(StartUitvaartNumber, EndUitvaartNumber);
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
        public void ExecuteFilterRapportageCommand(object obj)
        {
            if (string.IsNullOrEmpty(StartUitvaartNumber) || string.IsNullOrEmpty(EndUitvaartNumber))
            {
                MessageBox.Show("Je hebt geen Start en/of eind nummer ingevuld!");
            }
            else
            {
                KistenRapportageData(StartUitvaartNumber, EndUitvaartNumber);
                VerzekeringRapportageData(StartUitvaartNumber, EndUitvaartNumber);
                VerzekeringWoonplaatsRapportageData(StartUitvaartNumber, EndUitvaartNumber);
                UitvaartleiderRapportageData(StartUitvaartNumber, EndUitvaartNumber);
                VolgautosRapportageData(StartUitvaartNumber, EndUitvaartNumber);
                PeriodeLijstRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            }
            CloseFilterRapportagePopupOpenCommand.Execute(null);
        }
        public void ExecuteOpenEmployeeScoreCommand(object obj)
        {
            foreach (var empScore in miscellaneousRepository.GetEmployeeScore((Guid)obj))
            {
                SelectedScoreEmployeeName = empScore.EmployeeName;

                WerknemerScore.Clear();
                WerknemerScore.Add(new RapportageKlantWerknemerScores
                {
                    EmployeeName = empScore.EmployeeName,
                    Cijfer = empScore.Cijfer,
                    UitvaartNr = empScore.UitvaartNr,
                    UitvaartVan = empScore.UitvaartVan,
                });
            }

            IsShowEmployeeScorePopupOpen = true;
        }
        public void RefreshRapportageGrid()
        {
            KistenRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            VerzekeringRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            VerzekeringWoonplaatsRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            UitvaartleiderRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            VolgautosRapportageData(StartUitvaartNumber, EndUitvaartNumber);
            PeriodeLijstRapportageData(StartUitvaartNumber, EndUitvaartNumber);
        }
        public void GetAllWerknemersScores()
        {
            foreach (var employeeScore in miscellaneousRepository.GetAllEmployeeScore())
            {
                AllWerknemerScore.Add(new RapportageKlantWerknemerScores()
                {
                    UitvaartNr = employeeScore.UitvaartNr,
                    EmployeeName = employeeScore.EmployeeName,
                    EmployeeId = employeeScore.EmployeeId,
                    GemiddeldCijfer = employeeScore.GemiddeldCijfer,
                    TotalUitvaarten = employeeScore.TotalUitvaarten,
                });
            }
        }
        public void GetWerknemersScores(Guid werknemerId)
        {
            foreach (var employeeScore in miscellaneousRepository.GetEmployeeScore(werknemerId))
            {
                WerknemerScore.Add(new RapportageKlantWerknemerScores()
                {
                    UitvaartNr = employeeScore.UitvaartNr,
                    EmployeeName = employeeScore.EmployeeName,
                    EmployeeId = employeeScore.EmployeeId,
                    Cijfer = employeeScore.Cijfer
                });
            }
        }
        public void UitvaartleiderRapportageData(string startNummer, string endNummer)
        {
            UitvaartleiderSeries.Clear();
            UitvaartleiderRapportages.Clear();

            UitvaartleiderTitle = new LabelVisual
            {
                Text = "Uitvaartleiders",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
            foreach (var uitvaartleiderResults in miscellaneousRepository.GetRapportagesUitvaartleider(startNummer, endNummer))
            {
                double uitvaartnummerAsDouble = Convert.ToDouble(uitvaartleiderResults.Uitvaartnummer);

                UitvaartleiderSeries.Add(new PieSeries<double>
                {
                    DataLabelsSize = 20,
                    DataLabelsPaint = new SolidColorPaint(SKColors.WhiteSmoke),
                    DataLabelsFormatter = (point) => uitvaartleiderResults.Uitvaartleider + " - " + point.PrimaryValue,
                    Values = new[] { uitvaartnummerAsDouble }, // Convert to double
                    Name = uitvaartleiderResults.Uitvaartleider,
                    Stroke = null
                });

                UitvaartleiderRapportages.Add(new RapportagesUitvaartleider()
                {
                    Uitvaartleider = uitvaartleiderResults.Uitvaartleider,
                    Uitvaartnummer = uitvaartleiderResults.Uitvaartnummer,
                });
            }
        }
        public void VerzekeringRapportageData(string startNummer, string endNummer)
        {
            // Clear existing data
            VerzekeringSeries.Clear();
            XAxes.Clear();
            YAxes.Clear();
            VerzekeringRapportages.Clear();

            // Set chart title
            VerzekeringTitle = new LabelVisual
            {
                Text = "Overleden Herkomsten",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

            // Color generator function for unique colors
            Func<int, SKColor> GenerateColor = (index) =>
            {
                Random rand = new Random(index);
                return new SKColor(
                    (byte)rand.Next(0, 255),
                    (byte)rand.Next(0, 255),
                    (byte)rand.Next(0, 255));
            };

            // Fetch data from the database
            var verzekeringResults = miscellaneousRepository.GetRapportagesVerzekeringWoonplaats(startNummer, endNummer);

            // Group results by VerzekeringHerkomst
            var groupedResults = verzekeringResults
                .GroupBy(v => v.VerzekeringHerkomst)
                .Select(g => new
                {
                    VerzekeringHerkomst = g.Key,
                    HerkomstCount = g.Sum(v => v.VerzekeringHerkomstCount) // Sum the counts for the same insurance type
                }).ToList();

            int colorIndex = 0;

            // Add series for each unique VerzekeringHerkomst
            foreach (var groupedResult in groupedResults)
            {
                var color = GenerateColor(colorIndex++);

                VerzekeringSeries.Add(new ColumnSeries<double>
                {
                    Values = new double[] { groupedResult.HerkomstCount },  // Bar height
                    Fill = new SolidColorPaint(color),
                    Name = $"{groupedResult.VerzekeringHerkomst} - Aantal: {groupedResult.HerkomstCount}", // Customize legend item
                    Stroke = null,
                });

                // Add the results to the observable collection
                VerzekeringRapportages.Add(new RapportagesVerzekering
                {
                    VerzekeringHerkomst = groupedResult.VerzekeringHerkomst,
                    VerzekeringHerkomstCount = groupedResult.HerkomstCount
                });
            }

            // Configure X-Axis (insurer names), but hide the labels
            XAxes.Add(new Axis
            {
                Labels = Array.Empty<string>(),  // Set to empty to hide labels
                LabelsRotation = 0,
                SeparatorsPaint = null,  // No vertical grid lines
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35))
            });

            // Configure Y-Axis (numeric values starting from 0)
            YAxes.Add(new Axis
            {
                MinLimit = 0,  // Y-axis starts at 0
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                MinStep = 1,   // Adjust step size as needed
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35))
            });
        }
        public void VerzekeringWoonplaatsRapportageData(string startNummer, string endNummer)
        {
            VerzekeringWoonplaatsRapportages.Clear();

            foreach (var verzekeringWoonplaatsResults in miscellaneousRepository.GetRapportagesVerzekeringWoonplaats(startNummer, endNummer))
            {
                VerzekeringWoonplaatsRapportages.Add(new RapportagesVerzekering()
                {
                    VerzekeringHerkomst = verzekeringWoonplaatsResults.VerzekeringHerkomst,
                    VerzekeringHerkomstCount = verzekeringWoonplaatsResults.VerzekeringHerkomstCount,
                    VerzekeringWoonplaats = verzekeringWoonplaatsResults.VerzekeringWoonplaats
                });
            }
        }
        public void VolgautosRapportageData(string startNummer, string endNummer)
        {
            VolgAutosRapportage.Clear();
            foreach (var volgauto in miscellaneousRepository.GetVolgautos(startNummer, endNummer))
            {
                VolgAutosRapportage.Add(new Volgautos()
                {
                    VerzekeringNaam = volgauto.VerzekeringNaam,
                    AantalVolgautos = volgauto.AantalVolgautos
                });
            }
        }
        public void PeriodeLijstRapportageData(string startNummer, string endNummer)
        {
            PeriodeLijst.Clear();
            foreach (var periode in miscellaneousRepository.GetPeriode(startNummer, endNummer))
            {
                string herkomstFactuur = string.Empty;
                string opdrachtgeverFactuur = string.Empty;

                if (!string.IsNullOrEmpty(periode.Factuur))
                {
                    try
                    {
                        var factuurData = JsonConvert.DeserializeObject<FactuurJson>(periode.Factuur);
                        if (factuurData != null)
                        {
                            opdrachtgeverFactuur = factuurData.OpdrachtgeverFactuurUrl ?? string.Empty;
                            herkomstFactuur = factuurData.VerenigingFactuurUrl ?? string.Empty;
                        }
                    }
                    catch (JsonException ex)
                    {
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    }
                }

                PeriodeLijst.Add(new PeriodeLijst()
                {
                    UitvaartNummer = periode.UitvaartNummer,
                    UitvaartNaam = periode.UitvaartNaam,
                    Voornamen = periode.Voornamen,
                    DatumOverlijden = periode.DatumOverlijden,
                    UitvaartType = periode.UitvaartType,
                    Verzekering = periode.Verzekering,
                    HerkomstFactuur = herkomstFactuur,
                    OpdrachtgeverFactuur = opdrachtgeverFactuur
                });
            }
        }
        public void KistenRapportageData(string startNummer, string endNummer)
        {
            KistenSeries.Clear();
            KistenRapportages.Clear();

            KistenTitle = new LabelVisual
            {
                Text = "Uitvaartkisten",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

            foreach (var kistenResults in miscellaneousRepository.GetRapportagesKisten(startNummer, endNummer))
            {
                KistenSeries.Add(new PieSeries<double>
                {
                    Values = new double[] { kistenResults.KistCount },
                    DataLabelsSize = 20,
                    DataLabelsPaint = new SolidColorPaint(SKColors.WhiteSmoke),
                    DataLabelsFormatter = (point) => $"{kistenResults.KistTypeNummer} - {point.PrimaryValue}",
                    Name = kistenResults.KistTypeNummer + " - " + kistenResults.KistOmschrijving,
                    Stroke = null
                });

                KistenRapportages.Add(new RapportagesKisten()
                {
                    KistTypeNummer = kistenResults.KistTypeNummer,
                    KistCount = kistenResults.KistCount,
                    KistOmschrijving = kistenResults.KistOmschrijving
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
    }
}
