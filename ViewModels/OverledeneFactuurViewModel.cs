using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Interfaces;
using Dossier_Registratie.Views;
using Newtonsoft.Json;
using NHunspell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Excel = Microsoft.Office.Interop.Excel;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneFactuurViewModel : ViewModelBase
    {
        string kostenbegrotingUrl = string.Empty;
        SqlConnection conn = new(DataProvider.ConnectionString);

        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private ModelCompare modelCompare;
        private FactuurModel _originalFactuurModel;
        private OverledeneUitvaartleiderModel _uitvaartLeiderModel;
        private KostenbegrotingModel _overledeneKostenbegrotingModel;
        private KostenbegrotingInfoModel _kostenbegrotingInfoModel;
        private GeneratingDocumentView _generatingDocumentView;
        private FactuurModel _overledeneFactuurModel;
        private ObservableCollection<VerzekeraarsModel> _verzekeraars;
        private ObservableCollection<SettingsModel> _settingsModel;
        private VerzekeraarsModel _selectedVerzekeraar;
        private bool _isPopupVisible;
        private bool _createEnabled;
        private decimal _subtotaal;
        private decimal _orgsubtotaal;
        private decimal _calculatedSubtotal;
        private decimal _discountAmount;
        private decimal _total;
        private bool _isExcelButtonEnabled = true;
        private bool _correctAccessOrNotCompleted = true;
        private Visibility _isExcelFileButtonVisable = Visibility.Collapsed;

        public bool CorrectAccessOrNotCompleted
        {
            get { return _correctAccessOrNotCompleted; }
            set
            {
                _correctAccessOrNotCompleted = value;
                OnPropertyChanged(nameof(CorrectAccessOrNotCompleted));
            }
        }
        public bool IsExcelButtonEnabled
        {
            get { return _isExcelButtonEnabled; }
            set
            {
                if (_isExcelButtonEnabled != value)
                {
                    _isExcelButtonEnabled = value;
                    OnPropertyChanged(nameof(IsExcelButtonEnabled));
                }
            }
        }
        public Visibility IsExcelFileButtonVisable
        {
            get { return _isExcelFileButtonVisable; }
            set
            {
                if (_isExcelFileButtonVisable != value)
                {
                    _isExcelFileButtonVisable = value;
                    OnPropertyChanged(nameof(IsExcelFileButtonVisable));
                }
            }
        }
        public decimal Subtotaal
        {
            get { return _subtotaal; }
            set
            {
                if (_subtotaal != value)
                {
                    _subtotaal = value;
                    OnPropertyChanged(nameof(Subtotaal));
                    UpdateTotal();
                }
            }
        }
        public decimal OrgSubtotaal
        {
            get { return _orgsubtotaal; }
            set
            {
                if (_orgsubtotaal != value)
                {
                    _orgsubtotaal = value;
                    OnPropertyChanged(nameof(OrgSubtotaal));
                    UpdateTotal();
                }
            }
        }
        public decimal CalculatedSubtotal
        {
            get { return _calculatedSubtotal; }
            set
            {
                if (_calculatedSubtotal != value)
                {
                    _calculatedSubtotal = value;
                    OnPropertyChanged(nameof(CalculatedSubtotal));
                    UpdateTotal();
                }
            }
        }
        public decimal DiscountAmount
        {
            get { return _discountAmount; }
            set
            {
                if (_discountAmount != value)
                {
                    _discountAmount = value;
                    OnPropertyChanged(nameof(DiscountAmount));
                    UpdateTotal();
                }
            }
        }
        public decimal Total
        {
            get { return _total; }
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged(nameof(Total));
                }
            }
        }
        private ObservableCollection<GeneratedKostenbegrotingModel> _priceComponents;
        public ObservableCollection<GeneratedKostenbegrotingModel> PriceComponents
        {
            get
            {
                if (_priceComponents == null)
                {
                    _priceComponents = new ObservableCollection<GeneratedKostenbegrotingModel>();
                    _priceComponents.CollectionChanged += PriceComponents_CollectionChanged;
                }
                return _priceComponents;
            }
            set
            {
                if (_priceComponents != null)
                    _priceComponents.CollectionChanged -= PriceComponents_CollectionChanged;

                _priceComponents = value;

                if (_priceComponents != null)
                    _priceComponents.CollectionChanged += PriceComponents_CollectionChanged;

                OnPropertyChanged(nameof(PriceComponents));
                UpdateSubtotaal();
            }
        }
        private ObservableCollection<PolisVerzekering> _polisComponents;
        public ObservableCollection<PolisVerzekering> PolisComponents
        {
            get { return _polisComponents; }
            set
            {
                if (_polisComponents != null)
                {
                    _polisComponents = value; OnPropertyChanged(nameof(PolisComponents));
                }
            }
        }
        public VerzekeraarsModel SelectedVerzekeraar
        {
            get { return _selectedVerzekeraar; }
            set
            {
                if (_selectedVerzekeraar != value)
                {
                    _selectedVerzekeraar = value;
                    OnPropertyChanged(nameof(SelectedVerzekeraar));
                }

                IsExcelButtonEnabled = _selectedVerzekeraar != null && !string.IsNullOrEmpty(_selectedVerzekeraar.Name);
                OnPropertyChanged(nameof(IsExcelButtonEnabled));
            }
        }

        public OverledeneUitvaartleiderModel InfoUitvaartleider
        {
            get
            {
                return _uitvaartLeiderModel;
            }
            set
            {
                _uitvaartLeiderModel = value;
                OnPropertyChanged(nameof(InfoUitvaartleider));
            }
        }
        public KostenbegrotingModel OverledeneKostenbegrotingModel
        {
            get
            {
                return _overledeneKostenbegrotingModel;
            }
            set
            {
                _overledeneKostenbegrotingModel = value;
                OnPropertyChanged(nameof(OverledeneKostenbegrotingModel));
            }
        }
        public KostenbegrotingInfoModel KostenbegrotingInfoModel
        {
            get
            {
                return _kostenbegrotingInfoModel;
            }
            set
            {
                _kostenbegrotingInfoModel = value;
                OnPropertyChanged(nameof(KostenbegrotingInfoModel));
            }
        }
        public FactuurModel OverledeneFactuurModel
        {
            get
            {
                return _overledeneFactuurModel;
            }
            set
            {
                _overledeneFactuurModel = value;
                OnPropertyChanged(nameof(OverledeneFactuurModel));
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
        public ObservableCollection<SettingsModel> SettingsModels
        {
            get { return _settingsModel; }
            set
            {
                _settingsModel = value;
                OnPropertyChanged(nameof(SettingsModel));
            }
        }
        public bool IsEnabled
        {
            get { return _createEnabled; }
            set
            {
                if (_createEnabled != value)
                {
                    _createEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }
        public bool IsPopupVisible
        {
            get { return _isPopupVisible; }
            set
            {
                if (_isPopupVisible != value)
                {
                    _isPopupVisible = value;
                    OnPropertyChanged(nameof(IsPopupVisible));
                }
            }
        }
        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand OpenPopupCommand { get; }
        public ICommand GenererenKostenbegrotingCommand { get; }
        public ICommand CreateKostenbegrotingFileCommand { get; }
        public ICommand KeyDownCommand { get; }
        public ICommand OpenKostenbegrotingCommand { get; }
        private void PriceComponents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSubtotaal();
            UpdateTotal();
        }
        public void UpdateSubtotaal()
        {
            Subtotaal = PriceComponents.Sum(item => item.Bedrag ?? 0);
            OrgSubtotaal = PriceComponents.Sum(item => item.OrgBedrag ?? 0);
        }
        private void UpdateTotal()
        {
            CalculatedSubtotal = PriceComponents
                .Where(item => item.Bedrag.HasValue && item.OrgBedrag.HasValue && item.Bedrag.Value < item.OrgBedrag.Value)
                .Sum(item => item.OrgBedrag.Value - item.Bedrag.Value);

            Total = Subtotaal - CalculatedSubtotal - DiscountAmount;
            //Total = CalculatedSubtotal - DiscountAmount;
        }
        private void ExecuteKeyDownCommand(object parameter)
        {
            var e = (KeyEventArgs)parameter;

            if (e.Key == Key.Enter)
            {
                UpdateSubtotaal();

                var dataGrid = e.Source as DataGrid;
                var selectedCells = dataGrid?.SelectedCells;
                if (selectedCells != null && selectedCells.Count > 0)
                {
                    var lastSelectedCell = selectedCells[selectedCells.Count - 1];
                    var item = lastSelectedCell.Item as GeneratedKostenbegrotingModel;

                    if (item != null)
                    {
                        var indexOfItem = PriceComponents.IndexOf(item);
                        var isLastRow = indexOfItem == PriceComponents.Count - 1;

                        if (!isLastRow)
                        {
                            e.Handled = true;

                        }
                    }
                }
            }
        }
        public void ClearPriceComponents()
        {
            if (PriceComponents != null && _priceComponents != null)
            {
                PriceComponents.Clear();
                _priceComponents.Clear();
            }
        }
        public OverledeneFactuurViewModel()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            SelectedVerzekeraar = new VerzekeraarsModel();
            OverledeneKostenbegrotingModel = new KostenbegrotingModel();
            OverledeneFactuurModel = new FactuurModel();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            _generatingDocumentView = new GeneratingDocumentView();

            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, param => CanExecuteSaveCommand(true));
            CloseCommand = new ViewModelCommand(ExecuteCloseCommand, param => CanExecuteSaveCommand(false));
            ClosePopupCommand = new ViewModelCommand(ExecuteClosePopupCommand);
            OpenPopupCommand = new ViewModelCommand(ExecuteOpenPopupCommand);
            KeyDownCommand = new ViewModelCommand(ExecuteKeyDownCommand);
            GenererenKostenbegrotingCommand = new ViewModelCommand(ExecuteGenererenKostenbegrotingCommand);
            CreateKostenbegrotingFileCommand = new ViewModelCommand(ExecuteCreateKostenbegrotingFileCommand);
            OpenKostenbegrotingCommand = new ViewModelCommand(ExecuteOpenKostenbegrotingCommand);
            Verzekeraars = new ObservableCollection<VerzekeraarsModel>();
            SettingsModels = new ObservableCollection<SettingsModel>();

            foreach (var el in miscellaneousRepository.GetVerzekeraars())
            {
                if (el.IsDeleted == false && el.IsHerkomst == true && el.Name != "Default")
                    Verzekeraars.Add(new VerzekeraarsModel { Id = el.Id, Name = el.Name, Afkorting = el.Afkorting, CustomLogo = el.CustomLogo });
            }
        }
        public void ReloadDynamicElements()
        {
            foreach (var el in miscellaneousRepository.GetVerzekeraars())
            {
                if (!Verzekeraars.Any(u => u.Id == el.Id) && el.IsDeleted == false && el.IsHerkomst == true && el.Name != "Default")
                    Verzekeraars.Add(new VerzekeraarsModel { Id = el.Id, Name = el.Name, Afkorting = el.Afkorting, CustomLogo = el.CustomLogo });
            }
        }
        public static OverledeneFactuurViewModel KostenbegrotingInstance { get; } = new();
        public void CreateNewDossier()
        {
            modelCompare = new ModelCompare();
            OverledeneKostenbegrotingModel = new KostenbegrotingModel();
            IsPopupVisible = true;
            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;
            IsExcelButtonEnabled = false;

            var selectedHerkomst = miscellaneousRepository.GetHerkomstByUitvaartId(Globals.UitvaartCodeGuid);
            if (selectedHerkomst.herkomstId != Guid.Empty)
            {
                SelectedVerzekeraar.Id = selectedHerkomst.herkomstId;
                SelectedVerzekeraar.Name = selectedHerkomst.herkomstName;
                SelectedVerzekeraar.CustomLogo = selectedHerkomst.herkomstLogo;
            }
        }
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            modelCompare = new ModelCompare();
            OverledeneKostenbegrotingModel = new KostenbegrotingModel();

            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;

            var factuurResult = searchRepository.GetOverlijdenKostenbegrotingByUitvaartId(uitvaartNummer);

            if (factuurResult != null)
            {
                IsExcelButtonEnabled = true;

                if ((factuurResult.KostenbegrotingUrl != null) && (!string.IsNullOrEmpty(factuurResult.KostenbegrotingUrl)))
                    IsExcelFileButtonVisable = Visibility.Visible;

                OverledeneFactuurModel.Id = factuurResult.Id;
                OverledeneFactuurModel.UitvaartId = factuurResult.UitvaartId;
                OverledeneFactuurModel.KostenbegrotingUrl = factuurResult.KostenbegrotingUrl;
                OverledeneFactuurModel.KostenbegrotingJson = factuurResult.KostenbegrotingJson;
                OverledeneFactuurModel.KostenbegrotingCreationDate = factuurResult.KostenbegrotingCreationDate;
                OverledeneFactuurModel.KostenbegrotingCreated = factuurResult.KostenbegrotingCreated;
                OverledeneFactuurModel.KostenbegrotingVerzekeraar = factuurResult.KostenbegrotingVerzekeraar;
                OverledeneFactuurModel.PolisJson = factuurResult.PolisJson;
                OverledeneFactuurModel.Korting = factuurResult.Korting;
                DiscountAmount = factuurResult.Korting;

                var previouslySelected = miscellaneousRepository.GetVerzekeraarsById(OverledeneFactuurModel.KostenbegrotingVerzekeraar);

                SelectedVerzekeraar.Id = previouslySelected.Id;
                SelectedVerzekeraar.Name = previouslySelected.Name;
                SelectedVerzekeraar.Afkorting = previouslySelected.Afkorting;
                SelectedVerzekeraar.CustomLogo = previouslySelected.CustomLogo;

                _originalFactuurModel = new FactuurModel
                {
                    Id = OverledeneFactuurModel.Id,
                    UitvaartId = OverledeneFactuurModel.UitvaartId,
                    KostenbegrotingUrl = OverledeneFactuurModel.KostenbegrotingUrl,
                    KostenbegrotingJson = OverledeneFactuurModel.KostenbegrotingJson,
                    KostenbegrotingCreationDate = OverledeneFactuurModel.KostenbegrotingCreationDate,
                    KostenbegrotingCreated = OverledeneFactuurModel.KostenbegrotingCreated,
                    KostenbegrotingVerzekeraar = OverledeneFactuurModel.KostenbegrotingVerzekeraar,
                    PolisJson = OverledeneFactuurModel.PolisJson,
                    Korting = OverledeneFactuurModel.Korting
                };

                var priceComponents = JsonConvert.DeserializeObject<List<GeneratedKostenbegrotingModel>>(OverledeneFactuurModel.KostenbegrotingJson);
                if (priceComponents != null)
                {
                    PriceComponents = new ObservableCollection<GeneratedKostenbegrotingModel>(
                        priceComponents
                            .Select(pc => new GeneratedKostenbegrotingModel
                            {
                                Omschrijving = pc.Omschrijving,
                                Aantal = pc.Aantal,
                                OrgAantal = pc.OrgAantal,
                                PmAmount = pc.PmAmount,
                                PrintTrue = pc.PrintTrue,
                                Verzekerd = !string.IsNullOrEmpty(pc.Aantal) && pc.Aantal != "0" ? "X" : "",
                                Bedrag = pc.Bedrag,
                                FactuurBedrag = pc.FactuurBedrag,
                                OrgBedrag = pc.OrgBedrag,
                                Id = pc.Id
                            })
                            .OrderByDescending(pc => pc.SortOrder)
                    //.ThenBy(pc => Math.Abs((decimal)pc.Bedrag))
                    );
                }
                else
                {
                    PriceComponents = new ObservableCollection<GeneratedKostenbegrotingModel>();
                }
                var polisList = JsonConvert.DeserializeObject<List<PolisVerzekering>>(OverledeneFactuurModel.PolisJson);
                if (polisList != null)
                {
                    foreach (var verzekering in polisList)
                    {
                        if (verzekering.VerzekeringName != null && verzekering.PolisInfoList != null)
                        {
                            foreach (var polis in verzekering.PolisInfoList)
                            {
                                if (!string.IsNullOrEmpty(polis.PolisNr) && !string.IsNullOrEmpty(polis.PolisBedrag))
                                {
                                    if (decimal.TryParse(polis.PolisBedrag, out decimal parsedBedrag))
                                    {
                                        decimal negativeBedrag = -Math.Abs(parsedBedrag);

                                        PriceComponents.Add(new GeneratedKostenbegrotingModel
                                        {
                                            Omschrijving = $"Af: {verzekering.VerzekeringName}, PolisNr: {polis.PolisNr}",
                                            Bedrag = negativeBedrag,
                                            FactuurBedrag = negativeBedrag,
                                            OrgBedrag = negativeBedrag,
                                            Id = Guid.NewGuid(),
                                            SortOrder = 9541
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var selectedHerkomst = miscellaneousRepository.GetHerkomstByUitvaartId(Globals.UitvaartCodeGuid);
                if (selectedHerkomst.herkomstId != Guid.Empty)
                {
                    var matchingVerzekeraar = Verzekeraars.FirstOrDefault(v => v.Id == selectedHerkomst.herkomstId);
                    if (matchingVerzekeraar != null)
                    {
                        SelectedVerzekeraar = matchingVerzekeraar;
                        ExecuteGenererenKostenbegrotingCommand(matchingVerzekeraar);
                    }
                    else
                    {
                        IsPopupVisible = true;
                        IsExcelButtonEnabled = false;
                    }
                }
                else
                {
                    IsPopupVisible = true;
                    IsExcelButtonEnabled = false;
                }
            }
        }
        private async void ExecuteCreateKostenbegrotingFileCommand(object parameter)
        {
            if (PriceComponents == null)
                return;

            IsExcelButtonEnabled = false;
            string destinationFile = await searchRepository.GetOverlijdenKostenbegrotingAsync(Globals.UitvaartCodeGuid);

            if (!string.IsNullOrEmpty(destinationFile) && File.Exists(destinationFile))
            {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Kostenbegroting gevonden", "De kostenbegroting (Excel) bestaat al.", $"Nieuw maken en de oude verwijderen?", "Ja", "Nee");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        try
                        {
                            File.Delete(destinationFile);
                        }
                        catch (IOException ex)
                        {
                            var closeExcel = MessageBox.Show("Kon het bestaande bestand niet verwijderen omdat hij geopend is, wil je (alle) Excel vensters sluiten? .", "Verwijderen mislukt", MessageBoxButton.YesNo, MessageBoxImage.Error);
                            if (closeExcel == MessageBoxResult.Yes)
                            {
                                CloseExcelApplication();
                                IsExcelButtonEnabled = true;
                                MessageBox.Show("Je kunt het nu nog eens proberen.", "Probeer het nog eens", MessageBoxButton.OK);
                                return;
                            }
                            else
                            {
                                MessageBox.Show("Sluit zelf Excel af en probeer het opnieuw (ook de verborgen processen", "Sluit Excel", MessageBoxButton.OK, MessageBoxImage.Warning);
                                IsExcelButtonEnabled = false;
                                return;
                            }
                        }
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        var stopInfo = new ProcessStartInfo
                        {
                            FileName = destinationFile,
                            UseShellExecute = true
                        };


                        Process.Start(stopInfo);
                        IsExcelButtonEnabled = true;
                        return;
                    }
            }

            _generatingDocumentView.Show();
            
            string kostenbegrotingUrl = await CreateKostenbegrotingFileAsync(Globals.UitvaartCode);

            OverledeneFactuurModel.UitvaartId = Globals.UitvaartCodeGuid;
            OverledeneFactuurModel.KostenbegrotingUrl = kostenbegrotingUrl;
            OverledeneFactuurModel.KostenbegrotingJson = JsonConvert.SerializeObject(PriceComponents);

            var priceComponentsOnly = SerializePriceComponentsToJson();

            await Task.Run(() => FillKostenbegrotingFile(Globals.UitvaartCodeGuid, kostenbegrotingUrl, OverledeneFactuurModel.KostenbegrotingJson, priceComponentsOnly));

            _generatingDocumentView.Hide();
            IsExcelButtonEnabled = true;
            var startInfo = new ProcessStartInfo
            {
                FileName = kostenbegrotingUrl,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
        static void CloseExcelApplication()
        {
            foreach (Process proc in Process.GetProcessesByName("EXCEL"))
            {
                try
                {
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error closing Excel: " + ex.Message);
                }
            }
        }
        public void PerformSpellCheckAndCorrection()
        {
            string affPath = "nl_NL.aff";
            string dicPath = "nl_NL.dic";

            var correctedList = PriceComponents.AsParallel()
                                               .Select(item =>
                                               {
                                                   using (Hunspell hunspell = new Hunspell(affPath, dicPath))
                                                   {
                                                       item.Omschrijving = CorrectSpelling(item.Omschrijving, hunspell);
                                                   }
                                                   return item;
                                               })
                                               .ToList();

            PriceComponents = new ObservableCollection<GeneratedKostenbegrotingModel>(correctedList);
        }
        private static string CorrectSpelling(string word, Hunspell hunspell)
        {
            if (!hunspell.Spell(word))
            {
                List<string> suggestions = hunspell.Suggest(word);

                if (suggestions.Any())
                    return suggestions.First();
            }
            return word;
        }
        public string SerializePriceComponentsToJson()
        {
            if (PriceComponents != null && PriceComponents.Any())
            {
                var filteredComponents = PriceComponents
                    .Where(pc => pc.SortOrder != 9541)
                    .ToList();

                if (filteredComponents.Any())
                {
                    string jsonData = JsonConvert.SerializeObject(filteredComponents);
                    return jsonData;
                }
            }

            return string.Empty;
        }
        private void ExecuteGenererenKostenbegrotingCommand(object parameter)
        {
            if (SelectedVerzekeraar.Id == Guid.Empty)
            {
                new ToastWindow("Er is geen herkomst gekozen!").Show();
                return;
            }
            string verzekeringMaatschapij = SelectedVerzekeraar?.Name;
            bool pakketVerzekering = SelectedVerzekeraar?.Pakket ?? false;

            if (!string.IsNullOrEmpty(verzekeringMaatschapij))
            {
                var uitvaartType = miscellaneousRepository.GetUitvaartType(Globals.UitvaartCodeGuid);
                var priceComponents = miscellaneousRepository.GetPriceComponentsId(SelectedVerzekeraar.Id, pakketVerzekering);

                var factuurResult = searchRepository.GetPolisInfoByUitvaartId(Globals.UitvaartCode);

                if (priceComponents.Count() == 0)
                {
                    IsPopupVisible = false;

                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{SelectedVerzekeraar.Name}", $"{SelectedVerzekeraar.Name} bevat geen prijs componenten", $"Wil je de standaard kostenbegroting gebruiken?", "Ja", "Nee");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IsEnabled = true;
                        priceComponents = miscellaneousRepository.GetPriceComponents("Default", false);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        MessageBox.Show($"Je kunt geen lege kostenbegroting maken, als je geen standaard kostenbegroting wilt " +
                                            $"zorg dan eerst dat de prijs componenten worden toegewezen of worden aangemaakt voor deze verzekeraar;\r\n\r\n" +
                                            $" - {SelectedVerzekeraar.Name}.", "Ongeldige kostenbegroting", MessageBoxButton.OK, MessageBoxImage.Hand);
                        IntAggregator.Transmit(7);
                    }
                }

                PriceComponents = new ObservableCollection<GeneratedKostenbegrotingModel>(
                    priceComponents.Select(pc => new GeneratedKostenbegrotingModel
                    {
                        Omschrijving = pc.Omschrijving,
                        Aantal = pc.Aantal,
                        OrgAantal = pc.Aantal,
                        PmAmount = pc.PmAmount,
                        PrintTrue = pc.PrintTrue,
                        Verzekerd = !string.IsNullOrEmpty(pc.Aantal) && pc.Aantal != "0" ? "X" : "",
                        Bedrag = pc.Bedrag,
                        OrgBedrag = pc.Bedrag,
                        FactuurBedrag = pc.FactuurBedrag,
                        Id = pc.Id,
                        SpecificBegrafenis = pc.SpecificBegrafenis,
                        SpecificCrematie = pc.SpecificCrematie
                    })
                );

                List<PolisVerzekering> polisList = null;

                try
                {
                    if (!string.IsNullOrEmpty(factuurResult.PolisJson))
                        polisList = JsonConvert.DeserializeObject<List<PolisVerzekering>>(factuurResult.PolisJson);
                }
                catch (JsonException ex)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                }

                // Proceed only if the list is not null
                if (polisList != null)
                {
                    foreach (var verzekering in polisList)
                    {
                        if (verzekering?.VerzekeringName != null && verzekering.PolisInfoList != null)
                        {
                            foreach (var polis in verzekering.PolisInfoList)
                            {
                                if (!string.IsNullOrEmpty(polis?.PolisNr) && !string.IsNullOrEmpty(polis?.PolisBedrag))
                                {
                                    if (decimal.TryParse(polis.PolisBedrag, out decimal parsedBedrag))
                                    {
                                        decimal negativeBedrag = -Math.Abs(parsedBedrag);

                                        PriceComponents.Add(new GeneratedKostenbegrotingModel
                                        {
                                            Omschrijving = $"Af: {verzekering.VerzekeringName}, PolisNr: {polis.PolisNr}",
                                            Bedrag = negativeBedrag,
                                            FactuurBedrag = negativeBedrag,
                                            OrgBedrag = negativeBedrag,
                                            Id = Guid.NewGuid(),
                                            SortOrder = 9541
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                IsPopupVisible = false;
                IsEnabled = true;
                IsExcelButtonEnabled = true;
            }
        }
        public void AddNewItemToCollection()
        {
            var newItem = new GeneratedKostenbegrotingModel
            {
                Omschrijving = PriceComponents.LastOrDefault()?.Omschrijving,
                Aantal = PriceComponents.LastOrDefault()?.Aantal,
                OrgAantal = PriceComponents.LastOrDefault()?.OrgAantal,
                Verzekerd = PriceComponents.LastOrDefault()?.Verzekerd,
                Bedrag = PriceComponents.LastOrDefault()?.Bedrag,
                PrintTrue = PriceComponents.LastOrDefault().PrintTrue,
                FactuurBedrag = PriceComponents.LastOrDefault()?.FactuurBedrag,
                OrgBedrag = PriceComponents.LastOrDefault()?.OrgBedrag,
                Id = Guid.NewGuid() // Generate a unique identifier
            };

            PriceComponents.Add(newItem);
            UpdateSubtotaal();
        }
        public void ExecuteClosePopupCommand(object obj)
        {
            if (obj is Popup verzekeringPopup)
                verzekeringPopup.IsOpen = false;
            IntAggregator.Transmit(7);
        }
        public void ExecuteOpenPopupCommand(object obj)
        {
            IsPopupVisible = true;
        }
        public void ExecuteOpenKostenbegrotingCommand(object obj)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = OverledeneFactuurModel.KostenbegrotingUrl,
                UseShellExecute = true
            });
        }
        private async Task<string> CreateKostenbegrotingFileAsync(string uitvaartId)
        {
            string destinationFile = string.Empty;

            string sourceLoc = DataProvider.DocumentenOpslag;
            string templateLoc = DataProvider.TemplateFolder;

            sourceLoc = EnsureTrailingSlash(sourceLoc);
            templateLoc = EnsureTrailingSlash(templateLoc);

            string fileToCopy = Path.Combine(templateLoc, "Kostenbegroting.xls");
            string destinationLoc = Path.Combine(sourceLoc, uitvaartId);
            destinationLoc = EnsureTrailingSlash(destinationLoc);

            destinationFile = Path.Combine(destinationLoc, $"Kostenbegroting_{Globals.UitvaartCode}.xls");

            if (File.Exists(fileToCopy))
            {
                if (!Directory.Exists(destinationLoc))
                {
                    Directory.CreateDirectory(destinationLoc);
                }
                if (!File.Exists(destinationFile))
                {
                    await Task.Run(() => File.Copy(fileToCopy, destinationFile, true));
                }
            }
            IsExcelFileButtonVisable = Visibility.Visible;
            return destinationFile;
        }
        private string EnsureTrailingSlash(string path)
        {
            if (!path.EndsWith(@"\"))
                path += @"\";

            return path;
        }
        public string LoadImageFromDatabase()
        {
            try
            {
                var (documentData, documentType) = miscellaneousRepository.GetLogoBlob(SelectedVerzekeraar.Name);

                if (documentData == null || documentData.Length == 0)
                {
                    throw new InvalidOperationException("No image data found for the specified Verzekeraar.");
                }

                BitmapImage bitmap;
                using (var stream = new MemoryStream(documentData))
                {
                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }

                string tempDirectory = Path.GetTempPath(); // Use the system temp directory for safety
                string tempFileName = Path.GetRandomFileName() + "." + documentType; // Use documentType from the database
                string tempPath = Path.Combine(tempDirectory, tempFileName);

                try
                {
                    using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                    {
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        encoder.Save(fileStream);
                    }
                }
                catch (IOException ex)
                {
                    throw new InvalidOperationException("Failed to save the image due to I/O error. Check file access and path validity.", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("An unexpected error occurred while saving the image.", ex);
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while processing the image.", ex);
            }
        }
        private async Task FillKostenbegrotingFile(Guid UitvaartCodeGuid, string kostenbegrotingUrl, string kostenbegrotingJson, string priceComponentsOnly)
        {
            CultureInfo.CurrentCulture = new CultureInfo("nl-NL");
            CultureInfo.CurrentUICulture = new CultureInfo("nl-NL");

            string kbImage = string.Empty;
            double totalAmount = 0.0;
            double orgAmount = 0.0;

            int excelRow = 8;
            var kostenbegrotingInfoResult = await miscellaneousRepository.GetKostenbegrotingPersonaliaAsync(UitvaartCodeGuid);

            var excelApp = new Excel.Application { Visible = false };
            var workbook = excelApp.Workbooks.Open(kostenbegrotingUrl);
            var worksheet = (Excel.Worksheet)workbook.ActiveSheet;

            if (SelectedVerzekeraar.CustomLogo == true)
            {
                kbImage = LoadImageFromDatabase();
                if (!string.IsNullOrEmpty(kbImage))
                {
                    var pictures = worksheet.Shapes;
                    Excel.Range cell = (Excel.Range)worksheet.Cells[2, 2];
                    var picture = pictures.AddPicture(kbImage,
                                                        Microsoft.Office.Core.MsoTriState.msoFalse, // LinkToFile
                                                        Microsoft.Office.Core.MsoTriState.msoCTrue, // SaveWithDocument
                                                        Convert.ToSingle(cell.Left),
                                                        Convert.ToSingle(cell.Top),
                                                        -1, // Width, -1 to keep original width
                                                        -1); // Height, -1 to keep original height
                    picture.Placement = Excel.XlPlacement.xlMoveAndSize;
                }
            }

            string voorletters = string.Empty;
            if (!string.IsNullOrEmpty(kostenbegrotingInfoResult.OpdrachtgeverVoornaam))
            {
                string[] words = kostenbegrotingInfoResult.OpdrachtgeverVoornaam.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                voorletters = string.Join(" ", words.Select(word => char.ToUpper(word[0])));
            }


            kostenbegrotingInfoResult.OverledeneNaam = $"{kostenbegrotingInfoResult.OverledeneAanhef} {kostenbegrotingInfoResult.OverledeneVoornaam} {kostenbegrotingInfoResult.OverledeneAchternaam}";

            worksheet.Cells[7, 5] = kostenbegrotingInfoResult.OverledeneNaam;

            ((Excel.Range)worksheet.Cells[7, 8]).Value2 = kostenbegrotingInfoResult.OverledenDatum != default ? kostenbegrotingInfoResult.OverledenDatum : (object)string.Empty;
            ((Excel.Range)worksheet.Cells[7, 8]).NumberFormat = "dd-mm-yyyy";


            var priceComponents = JsonConvert.DeserializeObject<List<GeneratedKostenbegrotingModel>>(kostenbegrotingJson);

            priceComponents = priceComponents
                .OrderByDescending(pc => pc.Verzekerd == "X")
                .ToList();

            var mergeRanges = new List<Excel.Range>();

            foreach (var priceComponent in priceComponents)
            {
                if (priceComponent.PrintTrue || priceComponent.PmAmount ||
                    (priceComponent.Bedrag.HasValue && priceComponent.Bedrag.Value != 0)
                    || priceComponent.Verzekerd == "X" || priceComponent.Aantal != "0")
                {
                    var aantalCell = (Excel.Range)worksheet.Cells[excelRow, 2];

                    aantalCell.Value = (int.TryParse(priceComponent.Aantal, out int aantal) && aantal > 1)
                        ? $"Aantal: {priceComponent.Aantal}  {priceComponent.Omschrijving}"
                        : priceComponent.Omschrijving;

                    ((Excel.Range)worksheet.Rows[excelRow]).RowHeight =
                        aantalCell.Value?.ToString().Length > 200 ? 36 :
                        aantalCell.Value?.ToString().Length > 100 ? 25 : 15;

                    aantalCell.WrapText = aantalCell.Value?.ToString().Length > 100;

                    aantalCell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

                    var verzekerdOrPmCell = (Excel.Range)worksheet.Cells[excelRow, 7];
                    verzekerdOrPmCell.Value = priceComponent.PmAmount
                        ? "pm"
                        : (string.IsNullOrEmpty(priceComponent.Verzekerd)
                        ? ""
                        : priceComponent.Verzekerd);

                    worksheet.Cells[excelRow, 8] = priceComponent.Bedrag;
                    ((Excel.Range)worksheet.Cells[excelRow, 8]).NumberFormatLocal = "_-€ * #.##0,00_-;_-€ * #.##0,00-;_-€ * \"-\"??_-;_-@_-";
                    ((Excel.Range)worksheet.Cells[excelRow, 8]).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;


                    var mergeRange = worksheet.Range[worksheet.Cells[excelRow, 2], worksheet.Cells[excelRow, 6]];
                    mergeRanges.Add(mergeRange);


                    priceComponent.Bedrag ??= 0m;
                    priceComponent.OrgBedrag ??= priceComponent.Bedrag;

                    totalAmount += (double)priceComponent.Bedrag;
                    orgAmount += (double)priceComponent.OrgBedrag;

                    excelRow++;
                    ((Excel.Range)worksheet.Rows[excelRow]).Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                }
            }


            foreach (var range in mergeRanges)
            {
                range.Merge();
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            }

            decimal negativeDiscount = DiscountAmount < 0 ? DiscountAmount : -DiscountAmount;
            worksheet.Cells[excelRow + 1, 8] = negativeDiscount;
            ((Excel.Range)worksheet.Cells[excelRow + 1, 8]).NumberFormatLocal = "_-€ * #.##0,00_-;_-€ * #.##0,00-;_-€ * \"-\"??_-;_-@_-";

            decimal calculatedSubtotal = CalculatedSubtotal < 0 ? CalculatedSubtotal : -CalculatedSubtotal;
            worksheet.Cells[excelRow + 2, 8] = calculatedSubtotal;

            ((Excel.Range)worksheet.Cells[excelRow + 2, 8]).NumberFormatLocal = "_-€ * #.##0,00_-;_-€ * #.##0,00-;_-€ * \"-\"??_-;_-@_-";

            excelRow++;
            if (CalculatedSubtotal > 0) 
                totalAmount = (double)((decimal)totalAmount - CalculatedSubtotal - DiscountAmount);

            worksheet.Cells[excelRow + 2, 8] = totalAmount;
            ((Excel.Range)worksheet.Cells[excelRow + 2, 8]).Formula = $"=SUM(H8:H{excelRow})";
            ((Excel.Range)worksheet.Cells[excelRow + 2, 8]).NumberFormatLocal = "_-€ * #.##0,00_-;_-€ * #.##0,00-;_-€ * \"-\"??_-;_-@_-";
            
            worksheet.Cells[excelRow + 6, 4] = $"{kostenbegrotingInfoResult.OpdrachtgeverAanhef} {voorletters} {kostenbegrotingInfoResult.OpdrachtgeverAchternaam}";
            worksheet.Cells[excelRow + 7, 4] = kostenbegrotingInfoResult.OpdrachtgeverStraat;
            worksheet.Cells[excelRow + 8, 4] = kostenbegrotingInfoResult.OpdrachtgeverPostcode;
            worksheet.Cells[excelRow + 9, 4] = kostenbegrotingInfoResult.OpdrachtgeverWoonplaats;
            worksheet.Cells[excelRow + 13, 2] = $"Dossier: {Globals.UitvaartCode}";

            worksheet.PageSetup.PrintArea = $"A1:I{excelRow + 18},A{excelRow + 19}:I{excelRow + 78}"; //82

            workbook.Close(true);
            excelApp.Quit();

            if (!string.IsNullOrEmpty(kbImage))
                File.Delete(kbImage);

            int returnedKostenbegrotingCount = await searchRepository.SearchKostenbegrotingExistanceAsync(UitvaartCodeGuid);

            if (returnedKostenbegrotingCount > 0)
            {
                await updateRepository.UpdateKostenbegrotingAsync(kostenbegrotingUrl, priceComponentsOnly, DateTime.Now, UitvaartCodeGuid, SelectedVerzekeraar.Id, DiscountAmount);
            }
            else
            {
                Guid documentId = Guid.NewGuid();
                await createRepository.InsertKostenbegrotingAsync(kostenbegrotingUrl, priceComponentsOnly, DateTime.Now, UitvaartCodeGuid, documentId, SelectedVerzekeraar.Id, DiscountAmount);
            }
        }
        public bool CanExecuteSaveCommand(object obj)
        {
            return true;
        }
        public async void ExecuteSaveCommand(object obj)
        {
            if (obj != null && obj.ToString() == "Opslaan")
            {
                OverledeneFactuurModel.UitvaartId = Globals.UitvaartCodeGuid;
                OverledeneFactuurModel.KostenbegrotingUrl = kostenbegrotingUrl;
                OverledeneFactuurModel.KostenbegrotingJson = SerializePriceComponentsToJson();
                OverledeneFactuurModel.KostenbegrotingVerzekeraar = SelectedVerzekeraar.Id;
                OverledeneFactuurModel.Korting = DiscountAmount;

                bool FactuurInfoExists = miscellaneousRepository.UitvaarFactuurExists(OverledeneFactuurModel.UitvaartId);

                if (OverledeneFactuurModel.Id == Guid.Empty && !FactuurInfoExists)
                {
                    OverledeneFactuurModel.Id = Guid.NewGuid();
                    try
                    {
                        Debug.WriteLine("add factuur");
                        createRepository.AddFactuur(OverledeneFactuurModel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error Inserting Factuur: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else if (FactuurInfoExists)
                {
                    bool factuurInfoChanged = modelCompare.AreValuesEqual(_originalFactuurModel, OverledeneFactuurModel);

                    if (!factuurInfoChanged)
                    {
                        try
                        {
                            Debug.WriteLine("edit factuur");
                            updateRepository.EditFactuur(OverledeneFactuurModel);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating factuur: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                            return;
                        }

                        _originalFactuurModel = new FactuurModel
                        {
                            Id = OverledeneFactuurModel.Id,
                            UitvaartId = OverledeneFactuurModel.UitvaartId,
                            KostenbegrotingUrl = OverledeneFactuurModel.KostenbegrotingUrl,
                            KostenbegrotingJson = OverledeneFactuurModel.KostenbegrotingJson,
                            KostenbegrotingCreationDate = OverledeneFactuurModel.KostenbegrotingCreationDate,
                            KostenbegrotingCreated = OverledeneFactuurModel.KostenbegrotingCreated,
                            KostenbegrotingVerzekeraar = OverledeneFactuurModel.KostenbegrotingVerzekeraar,
                            Korting = OverledeneFactuurModel.Korting
                        };
                    }
                }
                IntAggregator.Transmit(7);
            }
        }
        private void ExecuteCloseCommand(object obj)
        {
            if (PriceComponents != null && PriceComponents.Any())
            {
                CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Kostenbegroting sluiten", "Let op!", $"Je staat op het punt om de kosten begroting te sluiten zonder op te slaan! ", "Ja", "Blijven");
                if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                {
                    ClearPriceComponents();
                    IntAggregator.Transmit(7);
                }
                else
                {
                    return;
                }

            }
            else
            {
                ClearPriceComponents();
                IntAggregator.Transmit(7);
            }
        }
    }
}
