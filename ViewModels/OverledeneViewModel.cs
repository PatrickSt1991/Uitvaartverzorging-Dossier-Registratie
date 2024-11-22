using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Dossier_Registratie.MainWindow;
using static Dossier_Registratie.ViewModels.MainWindowViewModal;
using static Dossier_Registratie.ViewModels.OverledeneExtraInfoViewModal;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;

        private ModelCompare modelCompare = new ModelCompare();
        private OverledeneUitvaartleiderModel? _uitvaartLeiderModel;
        private SearchAddressApi searchAddress = new SearchAddressApi();
        private OverledenePersoonsGegevensModel? _persoonGegevensModel;
        private OverledeneOverlijdenInfoModel? _overlijdenInfoModel;
        private ObservableCollection<UitvaartLeiderModel> _uitvaartleidersData;
        private ObservableCollection<VerzekeraarsModel> _herkomst;
        private OverledeneOverlijdenInfoModel _originalOverlijdenInfoModel;
        private OverledeneUitvaartleiderModel _orginalUitvaartLeiderModel;
        private OverledenePersoonsGegevensModel _oginalPersoonGegevensModel;
        private ObservableCollection<SuggestionModel> _suggestions;
        private IEnumerable<SuggestionModel> _allSuggestions;
        private SuggestionModel _selectedSuggestion;
        private SuggestionModel _newSuggestion;
        private bool _isLidnummerVisible;
        private bool _overledeneThuisOverleden = false;
        private bool _correctAccessOrNotCompleted = true;
        private bool _requestSearchInfo = false;
        private bool _deceasedLocationFound = false;
        private bool _isSuggestionListVisible;
        private int _selectedIndex;
        private string _overlijdenLocatieLocal;

        public bool IsLidnummerVisible
        {
            get { return _isLidnummerVisible; }
            set
            {
                _isLidnummerVisible = value;
                OnPropertyChanged(nameof(IsLidnummerVisible));
            }
        }
        public bool OverledeneThuisOverleden
        {
            get { return _overledeneThuisOverleden; }
            set
            {
                _overledeneThuisOverleden = value;
                OnPropertyChanged(nameof(OverledeneThuisOverleden));
                UpdateAddresInfo();
            }
        }
        public bool CorrectAccessOrNotCompleted
        {
            get { return _correctAccessOrNotCompleted; }
            set
            {
                _correctAccessOrNotCompleted = value;
                OnPropertyChanged(nameof(CorrectAccessOrNotCompleted));
            }
        }
        public bool RequestSearchInfo
        {
            get { return _requestSearchInfo; }
            set
            {
                _requestSearchInfo = value;
                OnPropertyChanged(nameof(RequestSearchInfo));
            }
        }
        public bool IsPersoonsGegevensValid { get; private set; }
        public bool IsOverlijdenInfoValid { get; private set; }
        public bool DeceasedLocationFound
        {
            get { return _deceasedLocationFound; }
            set
            {
                _deceasedLocationFound = value;
                OnPropertyChanged(nameof(DeceasedLocationFound));
            }
        }
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                UpdateLidnummerVisibility();
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        public string OverlijdenLocatieLocal
        {
            get { return _overlijdenLocatieLocal; }
            set
            {
                if (_overlijdenLocatieLocal != value)
                {
                    _overlijdenLocatieLocal = value;
                    OnPropertyChanged(nameof(OverlijdenLocatieLocal));
                    OverlijdenInfo.OverledenLocatie = OverlijdenLocatieLocal;

                    if (_overlijdenLocatieLocal == "Thuis")
                    {
                        OverledeneThuisOverleden = true;
                        UpdateAddresInfo();
                    }

                    if (string.IsNullOrEmpty(OverlijdenLocatieLocal))
                        DeceasedLocationFound = false;

                    FilterSuggestions(OverlijdenLocatieLocal);
                }
            }
        }
        public SuggestionModel SelectedSuggestion
        {
            get { return _selectedSuggestion; }
            set
            {
                if (_selectedSuggestion != value)
                {
                    _selectedSuggestion = value;
                    OnPropertyChanged(nameof(SelectedSuggestion));

                    if (_selectedSuggestion != null)
                    {
                        // Set the selected suggestion to the TextBox (bind it to OverledenLocatie)
                        OverlijdenLocatieLocal = _selectedSuggestion.LongName;
                        OverlijdenInfo.OverledenLocatie = _selectedSuggestion.LongName;
                        OverlijdenInfo.OverledenPostcode = _selectedSuggestion.ZipCode;
                        OverlijdenInfo.OverledenHuisnummer = _selectedSuggestion.HouseNumber;
                        OverlijdenInfo.OverledenAdres = _selectedSuggestion.Street;
                        OverlijdenInfo.OverledenPlaats = _selectedSuggestion.City;
                        OverlijdenInfo.OverledenGemeente = _selectedSuggestion.County;

                        // Hide the suggestion list
                        IsSuggestionListVisible = false;
                        DeceasedLocationFound = true;
                        // Clear the selection (optional)
                        _selectedSuggestion = null;
                    }
                }
            }
        }
        public ObservableCollection<SuggestionModel> Suggestions
        {
            get { return _suggestions; }
            set
            {
                _suggestions = value;
                OnPropertyChanged(nameof(Suggestions));
            }
        }
        public ObservableCollection<VerzekeraarsModel> Herkomst
        {
            get { return _herkomst; }
            set { _herkomst = value; OnPropertyChanged(nameof(Herkomst)); }
        }
        public OverledeneUitvaartleiderModel UitvaartLeider
        {
            get
            {
                return _uitvaartLeiderModel;
            }

            set
            {
                _uitvaartLeiderModel = value;
                OnPropertyChanged(nameof(UitvaartLeider));
            }
        }
        public SuggestionModel NewSuggestion
        {
            get { return _newSuggestion; }
            set
            {
                _newSuggestion = value;
                OnPropertyChanged(nameof(NewSuggestion));
            }
        }
        public OverledenePersoonsGegevensModel PersoonsGegevens
        {
            get
            {
                return _persoonGegevensModel;
            }
            set
            {
                _persoonGegevensModel = value;
                OnPropertyChanged(nameof(PersoonsGegevens));
            }
        }
        public OverledeneOverlijdenInfoModel OverlijdenInfo
        {
            get { return _overlijdenInfoModel; }
            set
            {
                _overlijdenInfoModel = value;
                OnPropertyChanged(nameof(OverlijdenInfo));
            }
        }
        public ObservableCollection<UitvaartLeiderModel> UitvaartleidersData
        {
            get { return _uitvaartleidersData; }
            set
            {
                _uitvaartleidersData = value;
                OnPropertyChanged(nameof(UitvaartleidersData));
            }
        }
        public bool IsSuggestionListVisible
        {
            get { return _isSuggestionListVisible; }
            set
            {
                _isSuggestionListVisible = value;
                OnPropertyChanged(nameof(IsSuggestionListVisible));
            }
        }
        public ICommand SaveCommand { get; }
        public ICommand PreviousCommand { get; }
        private OverledeneViewModel()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            modelCompare = new ModelCompare();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            UitvaartLeider = new OverledeneUitvaartleiderModel();
            PersoonsGegevens = new OverledenePersoonsGegevensModel();
            NewSuggestion = new SuggestionModel();
            Suggestions = new ObservableCollection<SuggestionModel>();
            OverlijdenInfo = new OverledeneOverlijdenInfoModel();
            UitvaartleidersData = new ObservableCollection<UitvaartLeiderModel>();
            Herkomst = new ObservableCollection<VerzekeraarsModel>();

            // Load all suggestions from the database
            _allSuggestions = miscellaneousRepository.GetSuggestions();

            FilterSuggestions(string.Empty);

            UitvaartleidersData.Clear();
            Herkomst.Clear();


            foreach (var el in miscellaneousRepository.GetUitvaartleiders())
                UitvaartleidersData.Add(new UitvaartLeiderModel { Id = el.Id, Uitvaartleider = el.Uitvaartleider });


            foreach (var el in miscellaneousRepository.GetVerzekeraars())
            {
                if (el.IsDeleted == false && el.IsHerkomst == true)
                    Herkomst.Add(new VerzekeraarsModel { Id = el.Id, Name = el.Name, HasLidnummer = el.HasLidnummer });
            }

            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
        }
        private void FilterSuggestions(string locationInput)
        {

            if (string.IsNullOrEmpty(locationInput) || DeceasedLocationFound)
            {
                Suggestions.Clear();
                IsSuggestionListVisible = false;
            }
            else
            {
                var filtered = _allSuggestions
                    .Where(s => s.ShortName.Contains(locationInput, StringComparison.OrdinalIgnoreCase) ||
                                s.LongName.Contains(locationInput, StringComparison.OrdinalIgnoreCase) && s.IsDeleted == false)
                    .ToList();

                Suggestions.Clear();

                if ((filtered.Count == 1) && (locationInput.ToLower() == filtered.FirstOrDefault().LongName.ToLower() || locationInput.ToLower() == filtered.FirstOrDefault().ShortName.ToLower()))
                {
                    OverlijdenLocatieLocal = filtered.FirstOrDefault().LongName;
                    OverlijdenInfo.OverledenLocatie = filtered.FirstOrDefault().LongName;
                    OverlijdenInfo.OverledenPostcode = filtered.FirstOrDefault().ZipCode;
                    OverlijdenInfo.OverledenHuisnummer = filtered.FirstOrDefault().HouseNumber;
                    OverlijdenInfo.OverledenAdres = filtered.FirstOrDefault().Street;
                    OverlijdenInfo.OverledenPlaats = filtered.FirstOrDefault().City;
                    OverlijdenInfo.OverledenGemeente = filtered.FirstOrDefault().County;

                    IsSuggestionListVisible = false;
                    DeceasedLocationFound = true;
                }
                else
                {
                    foreach (var suggestion in filtered)
                    {
                        Suggestions.Add(suggestion);
                    }

                    IsSuggestionListVisible = filtered.Any();
                }
            }
        }
        public async void FetchPersoonAddressInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(PersoonsGegevens.OverledenePostcode) && (!string.IsNullOrEmpty(PersoonsGegevens.OverledeneHuisnummer)))
                {
                    var pGaddress = await SearchAddressApi.GetAddressAsync(PersoonsGegevens.OverledenePostcode, PersoonsGegevens.OverledeneHuisnummer, PersoonsGegevens.OverledeneHuisnummerToevoeging);
                    if (pGaddress != null)
                    {
                        PersoonsGegevens.OverledeneAdres = pGaddress.Street;
                        PersoonsGegevens.OverledeneHuisnummer = pGaddress.HouseNumber;
                        PersoonsGegevens.OverledeneHuisnummerToevoeging = pGaddress.HouseNumberAddition;
                        PersoonsGegevens.OverledenePostcode = pGaddress.PostalCode;
                        PersoonsGegevens.OverledeneWoonplaats = pGaddress.City;
                        PersoonsGegevens.OverledeneGemeente = pGaddress.County;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error persoonsgegevens address lookup: {ex.Message}", "Lookup Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }
        }
        public async void FetchOverlijdenAddressInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(OverlijdenInfo.OverledenPostcode) && (!string.IsNullOrEmpty(OverlijdenInfo.OverledenHuisnummer)))
                {
                    if (!OverledeneThuisOverleden)
                    {
                        var oIaddress = await SearchAddressApi.GetAddressAsync(OverlijdenInfo.OverledenPostcode, OverlijdenInfo.OverledenHuisnummer, OverlijdenInfo.OverledenHuisnummerToevoeging);
                        if (oIaddress != null)
                        {
                            OverlijdenInfo.OverledenAdres = oIaddress.Street;
                            OverlijdenInfo.OverledenHuisnummer = oIaddress.HouseNumber;
                            OverlijdenInfo.OverledenHuisnummerToevoeging = oIaddress.HouseNumberAddition;
                            OverlijdenInfo.OverledenPostcode = oIaddress.PostalCode;
                            OverlijdenInfo.OverledenPlaats = oIaddress.City;
                            OverlijdenInfo.OverledenGemeente = oIaddress.County;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error overleden address lookup: {ex.Message}", "Lookup Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }
        }
        public async void FetchOverlijdenLocationInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(OverlijdenInfo.OverledenLocatie) && (!OverledeneThuisOverleden) && (!DeceasedLocationFound))
                {
                    var oLaddress = await SearchAddressApi.GetLocationAsync(OverlijdenInfo.OverledenLocatie);
                    if (oLaddress != null)
                    {
                        OverlijdenLocatieLocal = oLaddress.Location;
                        OverlijdenInfo.OverledenLocatie = oLaddress.Location;
                        OverlijdenInfo.OverledenAdres = oLaddress.Street;
                        OverlijdenInfo.OverledenHuisnummer = oLaddress.HouseNumber;
                        OverlijdenInfo.OverledenHuisnummerToevoeging = null;
                        OverlijdenInfo.OverledenPostcode = oLaddress.PostalCode;
                        OverlijdenInfo.OverledenPlaats = oLaddress.City;
                        OverlijdenInfo.OverledenGemeente = oLaddress.County;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error overleden locatie lookup: {ex.Message}", "Lookup Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }
        }
        public void RefresDynamicElements()
        {
            foreach (var el in miscellaneousRepository.GetUitvaartleiders())
            {
                if (!UitvaartleidersData.Any(u => u.Id == el.Id))
                {
                    UitvaartleidersData.Add(new UitvaartLeiderModel { Id = el.Id, Uitvaartleider = el.Uitvaartleider });
                }
            }

            foreach (var el in miscellaneousRepository.GetVerzekeraars())
            {
                if (!Herkomst.Any(u => u.Id == el.Id) && el.IsDeleted == false)
                {
                    Herkomst.Add(new VerzekeraarsModel { Id = el.Id, Name = el.Name, HasLidnummer = el.HasLidnummer });
                }
            }
        }
        public void UpdateAddresInfo()
        {
            if (OverledeneThuisOverleden)
            {
                if (!string.IsNullOrEmpty(PersoonsGegevens.OverledeneAdres))
                    OverlijdenInfo.OverledenAdres = PersoonsGegevens.OverledeneAdres;

                if (!string.IsNullOrEmpty(PersoonsGegevens.OverledenePostcode))
                    OverlijdenInfo.OverledenPostcode = PersoonsGegevens.OverledenePostcode;

                if (!string.IsNullOrEmpty(PersoonsGegevens.OverledeneHuisnummer))
                    OverlijdenInfo.OverledenHuisnummer = PersoonsGegevens.OverledeneHuisnummer;

                if (!string.IsNullOrEmpty(PersoonsGegevens.OverledeneHuisnummerToevoeging))
                    OverlijdenInfo.OverledenHuisnummerToevoeging = PersoonsGegevens.OverledeneHuisnummerToevoeging;

                if (!string.IsNullOrEmpty(PersoonsGegevens.OverledeneWoonplaats))
                    OverlijdenInfo.OverledenPlaats = PersoonsGegevens.OverledeneWoonplaats;

                if (!string.IsNullOrEmpty(PersoonsGegevens.OverledeneGemeente))
                    OverlijdenInfo.OverledenGemeente = PersoonsGegevens.OverledeneGemeente;

                DeceasedLocationFound = true;
                OverlijdenLocatieLocal = "Thuis";
                OverlijdenInfo.OverledenLocatie = "Thuis";
            }
        }
        public void HomeDeceased()
        {
            if (OverlijdenInfo.OverledenAdres == PersoonsGegevens.OverledeneAdres &&
                OverlijdenInfo.OverledenPostcode == PersoonsGegevens.OverledenePostcode &&
                OverlijdenInfo.OverledenHuisnummer == PersoonsGegevens.OverledeneHuisnummer &&
                OverlijdenInfo.OverledenHuisnummerToevoeging == PersoonsGegevens.OverledeneHuisnummerToevoeging &&
                OverlijdenInfo.OverledenPlaats == PersoonsGegevens.OverledeneWoonplaats &&
                OverlijdenInfo.OverledenGemeente == PersoonsGegevens.OverledeneGemeente)
            {
                OverledeneThuisOverleden = true;
            }
            else
            {
                OverledeneThuisOverleden = false;
            }
        }
        public void CreateNewDossier()
        {
            modelCompare = new ModelCompare();
            UitvaartLeider = new OverledeneUitvaartleiderModel();
            PersoonsGegevens = new OverledenePersoonsGegevensModel();
            OverlijdenInfo = new OverledeneOverlijdenInfoModel();
        }
        public static OverledeneViewModel Instance { get; } = new();
        private void UpdateLidnummerVisibility()
        {
            if (SelectedIndex >= 0 && SelectedIndex < Herkomst.Count)
            {
                IsLidnummerVisible = (bool)Herkomst[SelectedIndex].HasLidnummer;
            }
            else
            {
                IsLidnummerVisible = false;
            }
        }
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            modelCompare = new ModelCompare();
            UitvaartLeider = new OverledeneUitvaartleiderModel();
            PersoonsGegevens = new OverledenePersoonsGegevensModel();
            OverlijdenInfo = new OverledeneOverlijdenInfoModel();

            RequestSearchInfo = true;
            var UitvaarLeiderResult = searchRepository.GetUitvaarleiderByUitvaartId(uitvaartNummer);
            if (UitvaarLeiderResult != null)
            {
                UitvaartLeider.UitvaartId = UitvaarLeiderResult.UitvaartId;
                UitvaartLeider.Uitvaartnummer = UitvaarLeiderResult.Uitvaartnummer;
                UitvaartLeider.PersoneelNaam = UitvaarLeiderResult.PersoneelNaam;
                UitvaartLeider.PersoneelId = UitvaarLeiderResult.PersoneelId;

                _orginalUitvaartLeiderModel = new OverledeneUitvaartleiderModel
                {
                    UitvaartId = UitvaartLeider.UitvaartId,
                    Uitvaartnummer = UitvaartLeider.Uitvaartnummer,
                    PersoneelNaam = UitvaartLeider.PersoneelNaam,
                    PersoneelId = UitvaartLeider.PersoneelId
                };

                Globals.UitvaartCode = UitvaarLeiderResult.Uitvaartnummer;
                Globals.UitvaartCodeGuid = UitvaarLeiderResult.UitvaartId;
            }

            var UitvaartPersoonsGegevens = searchRepository.GetPeroonsGegevensByUitvaartId(uitvaartNummer);
            if (UitvaartPersoonsGegevens != null)
            {
                PersoonsGegevens.Id = UitvaartPersoonsGegevens.Id;
                PersoonsGegevens.UitvaartId = UitvaartPersoonsGegevens.UitvaartId;
                PersoonsGegevens.OverledeneBSN = UitvaartPersoonsGegevens.OverledeneBSN;
                PersoonsGegevens.OverledeneAanhef = UitvaartPersoonsGegevens.OverledeneAanhef;
                PersoonsGegevens.OverledeneAchternaam = UitvaartPersoonsGegevens.OverledeneAchternaam;
                PersoonsGegevens.OverledeneTussenvoegsel = UitvaartPersoonsGegevens.OverledeneTussenvoegsel;
                PersoonsGegevens.OverledeneVoornamen = UitvaartPersoonsGegevens.OverledeneVoornamen;
                PersoonsGegevens.OverledeneGeboortedatum = UitvaartPersoonsGegevens.OverledeneGeboortedatum;
                PersoonsGegevens.OverledeneGeboorteplaats = UitvaartPersoonsGegevens.OverledeneGeboorteplaats;
                PersoonsGegevens.OverledeneLeeftijd = UitvaartPersoonsGegevens.OverledeneLeeftijd;
                PersoonsGegevens.OverledeneAdres = UitvaartPersoonsGegevens.OverledeneAdres;
                PersoonsGegevens.OverledeneHuisnummer = UitvaartPersoonsGegevens.OverledeneHuisnummer;
                PersoonsGegevens.OverledeneHuisnummerToevoeging = UitvaartPersoonsGegevens.OverledeneHuisnummerToevoeging;
                PersoonsGegevens.OverledenePostcode = UitvaartPersoonsGegevens.OverledenePostcode;
                PersoonsGegevens.OverledeneWoonplaats = UitvaartPersoonsGegevens.OverledeneWoonplaats;
                PersoonsGegevens.OverledeneGemeente = UitvaartPersoonsGegevens.OverledeneGemeente;
                PersoonsGegevens.OverledeneVoorregeling = UitvaartPersoonsGegevens.OverledeneVoorregeling;

                _oginalPersoonGegevensModel = new OverledenePersoonsGegevensModel
                {
                    Id = PersoonsGegevens.Id,
                    OverledeneBSN = PersoonsGegevens.OverledeneBSN,
                    OverledeneAanhef = PersoonsGegevens.OverledeneAanhef,
                    OverledeneAchternaam = PersoonsGegevens.OverledeneAchternaam,
                    OverledeneTussenvoegsel = PersoonsGegevens.OverledeneTussenvoegsel,
                    OverledeneVoornamen = PersoonsGegevens.OverledeneVoornamen,
                    OverledeneGeboortedatum = PersoonsGegevens.OverledeneGeboortedatum,
                    OverledeneGeboorteplaats = PersoonsGegevens.OverledeneGeboorteplaats,
                    OverledeneLeeftijd = PersoonsGegevens.OverledeneLeeftijd,
                    OverledeneAdres = PersoonsGegevens.OverledeneAdres,
                    OverledeneHuisnummer = PersoonsGegevens.OverledeneHuisnummer,
                    OverledeneHuisnummerToevoeging = PersoonsGegevens.OverledeneHuisnummerToevoeging,
                    OverledenePostcode = PersoonsGegevens.OverledenePostcode,
                    OverledeneWoonplaats = PersoonsGegevens.OverledeneWoonplaats,
                    OverledeneGemeente = PersoonsGegevens.OverledeneGemeente,
                    OverledeneVoorregeling = PersoonsGegevens.OverledeneVoorregeling
                };
            }

            var UitvaartOverlijdenInfo = searchRepository.GetOverlijdenInfoByUitvaartId(uitvaartNummer);
            if (UitvaartOverlijdenInfo != null)
            {
                OverlijdenInfo.Id = UitvaartOverlijdenInfo.Id;
                OverlijdenInfo.UitvaartId = UitvaartOverlijdenInfo.UitvaartId;
                OverlijdenInfo.OverledenDatumTijd = UitvaartOverlijdenInfo.OverledenDatumTijd;
                OverlijdenInfo.OverledenLocatie = UitvaartOverlijdenInfo.OverledenLocatie;
                OverlijdenLocatieLocal = UitvaartOverlijdenInfo.OverledenLocatie;
                OverlijdenInfo.OverledenAdres = UitvaartOverlijdenInfo.OverledenAdres;
                OverlijdenInfo.OverledenPostcode = UitvaartOverlijdenInfo.OverledenPostcode;
                OverlijdenInfo.OverledenHuisnummer = UitvaartOverlijdenInfo.OverledenHuisnummer;
                OverlijdenInfo.OverledenHuisnummerToevoeging = UitvaartOverlijdenInfo.OverledenHuisnummerToevoeging;
                OverlijdenInfo.OverledenPlaats = UitvaartOverlijdenInfo.OverledenPlaats;
                OverlijdenInfo.OverledenGemeente = UitvaartOverlijdenInfo.OverledenGemeente;
                OverlijdenInfo.OverledenLijkvinding = UitvaartOverlijdenInfo.OverledenLijkvinding;
                OverlijdenInfo.OverledenHerkomst = UitvaartOverlijdenInfo.OverledenHerkomst;
                OverlijdenInfo.OverledenLidnummer = UitvaartOverlijdenInfo.OverledenLidnummer;
                OverlijdenInfo.OverledenHuisarts = UitvaartOverlijdenInfo.OverledenHuisarts;
                OverlijdenInfo.OverledenHuisartsTelefoon = UitvaartOverlijdenInfo.OverledenHuisartsTelefoon;
                OverlijdenInfo.OverledenSchouwarts = UitvaartOverlijdenInfo.OverledenSchouwarts;

                if (!string.IsNullOrEmpty(OverlijdenLocatieLocal))
                {
                    Suggestions.Clear();
                    IsSuggestionListVisible = false;
                }

                _originalOverlijdenInfoModel = new OverledeneOverlijdenInfoModel
                {
                    OverledenDatumTijd = OverlijdenInfo.OverledenDatumTijd,
                    OverledenLocatie = OverlijdenLocatieLocal,
                    OverledenAdres = OverlijdenInfo.OverledenAdres,
                    OverledenPostcode = OverlijdenInfo.OverledenPostcode,
                    OverledenHuisnummer = OverlijdenInfo.OverledenHuisnummer,
                    OverledenHuisnummerToevoeging = OverlijdenInfo.OverledenHuisnummerToevoeging,
                    OverledenPlaats = OverlijdenInfo.OverledenPlaats,
                    OverledenGemeente = OverlijdenInfo.OverledenGemeente,
                    OverledenLijkvinding = OverlijdenInfo.OverledenLijkvinding,
                    OverledenHerkomst = OverlijdenInfo.OverledenHerkomst,
                    OverledenLidnummer = OverlijdenInfo.OverledenLidnummer,
                    OverledenHuisarts = OverlijdenInfo.OverledenHuisarts,
                    OverledenHuisartsTelefoon = OverlijdenInfo.OverledenHuisartsTelefoon,
                    OverledenSchouwarts = OverlijdenInfo.OverledenSchouwarts
                };
            }
            RequestSearchInfo = false;
        }
        private bool CanExecuteSaveCommand(object obj)
        {
            IsPersoonsGegevensValid = !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneAchternaam) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneVoornamen) &&
                                      PersoonsGegevens.OverledeneGeboortedatum != DateTime.MinValue &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneGeboorteplaats) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneAdres) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledenePostcode) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneWoonplaats) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneBSN) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneHuisnummer) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneGemeente) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneLeeftijd) &&
                                      !string.IsNullOrWhiteSpace(PersoonsGegevens.OverledeneAanhef);

            IsOverlijdenInfoValid = OverlijdenInfo.OverledenDatumTijd != DateTime.MinValue &&
                                    !string.IsNullOrWhiteSpace(OverlijdenInfo.OverledenAdres) &&
                                    !string.IsNullOrWhiteSpace(OverlijdenInfo.OverledenHuisnummer) &&
                                    !string.IsNullOrWhiteSpace(OverlijdenInfo.OverledenPlaats) &&
                                    !string.IsNullOrWhiteSpace(OverlijdenInfo.OverledenGemeente) &&
                                    !string.IsNullOrWhiteSpace(OverlijdenInfo.OverledenLijkvinding) &&
                                    OverlijdenInfo.OverledenHerkomst != Guid.Empty;

            if (IsPersoonsGegevensValid && IsOverlijdenInfoValid)
            {
                return true;
            }
            else if (IsPersoonsGegevensValid && IsOverlijdenInfoValid && PersoonsGegevens.OverledeneVoorregeling)
            {
                return true;
            }
            else if (IsPersoonsGegevensValid && !IsOverlijdenInfoValid && PersoonsGegevens.OverledeneVoorregeling)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void ExecuteSaveCommand(object obj)
        {
            Globals.Voorregeling = PersoonsGegevens.OverledeneVoorregeling;
            Guid newGuid = Guid.NewGuid();
            Guid PersoonGegevensId = Guid.NewGuid();
            Guid OverlijdenInfoId = Guid.NewGuid();

            bool UitvaartnummerExists = miscellaneousRepository.UitvaarnummerExists(UitvaartLeider.Uitvaartnummer);
            bool PersoonsgegevensExists = miscellaneousRepository.UitvaartPersoonsgegevensExists(UitvaartLeider.UitvaartId);
            bool OverlijdenInfoExists = miscellaneousRepository.UitvaartOverlijdenInfoExists(UitvaartLeider.UitvaartId);

            if (UitvaartLeider.UitvaartId == Guid.Empty && UitvaartnummerExists)
            {
                new ToastWindow($"Uitvaartnummer {UitvaartLeider.Uitvaartnummer} is al in gebruik!").Show();
                return;
            }

            if (!UitvaartLeider.HasData() || !PersoonsGegevens.HasData())
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            if (!PersoonsGegevens.OverledeneVoorregeling && !IsOverlijdenInfoValid)
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            if (UitvaartLeider.UitvaartId == Guid.Empty && !UitvaartnummerExists)
            {
                UitvaartLeider.UitvaartId = newGuid;

                try
                {
                    createRepository.AddUitvaartleider(UitvaartLeider);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting uitvaartleider: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (UitvaartnummerExists)
            {
                bool uitvaartleiderInfoChanged = modelCompare.AreValuesEqual(_orginalUitvaartLeiderModel, UitvaartLeider);

                Globals.UitvaarLeider = UitvaartLeider.PersoneelNaam;
                Globals.UitvaartCode = UitvaartLeider.Uitvaartnummer;

                if (uitvaartleiderInfoChanged == false)
                {

                    try
                    {
                        updateRepository.EditUitvaartleider(UitvaartLeider);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error updating uitvaartleider: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _orginalUitvaartLeiderModel = new OverledeneUitvaartleiderModel
                    {
                        UitvaartId = UitvaartLeider.UitvaartId,
                        Uitvaartnummer = UitvaartLeider.Uitvaartnummer,
                        PersoneelNaam = UitvaartLeider.PersoneelNaam,
                        PersoneelId = UitvaartLeider.PersoneelId
                    };
                }
            }

            if (PersoonsGegevens.Id == Guid.Empty && !PersoonsgegevensExists)
            {
                PersoonsGegevens.Id = PersoonGegevensId;
                PersoonsGegevens.UitvaartId = UitvaartLeider.UitvaartId;

                try
                {
                    createRepository.AddPersoonsGegevens(PersoonsGegevens);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error inserting persoonsgegevens: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if(!PersoonsgegevensExists)
            {
                bool persoonsgegevensInfoChanged = modelCompare.AreValuesEqual(_oginalPersoonGegevensModel, PersoonsGegevens);

                if (persoonsgegevensInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditPersoonsGegevens(PersoonsGegevens);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error updating Persoonsgegevens: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _oginalPersoonGegevensModel = new OverledenePersoonsGegevensModel
                    {
                        Id = PersoonsGegevens.Id,
                        OverledeneBSN = PersoonsGegevens.OverledeneBSN,
                        OverledeneAanhef = PersoonsGegevens.OverledeneAanhef,
                        OverledeneAchternaam = PersoonsGegevens.OverledeneAchternaam,
                        OverledeneTussenvoegsel = PersoonsGegevens.OverledeneTussenvoegsel,
                        OverledeneVoornamen = PersoonsGegevens.OverledeneVoornamen,
                        OverledeneGeboortedatum = PersoonsGegevens.OverledeneGeboortedatum,
                        OverledeneGeboorteplaats = PersoonsGegevens.OverledeneGeboorteplaats,
                        OverledeneLeeftijd = PersoonsGegevens.OverledeneLeeftijd,
                        OverledeneAdres = PersoonsGegevens.OverledeneAdres,
                        OverledeneHuisnummer = PersoonsGegevens.OverledeneHuisnummer,
                        OverledeneHuisnummerToevoeging = PersoonsGegevens.OverledeneHuisnummerToevoeging,
                        OverledenePostcode = PersoonsGegevens.OverledenePostcode,
                        OverledeneWoonplaats = PersoonsGegevens.OverledeneWoonplaats,
                        OverledeneGemeente = PersoonsGegevens.OverledeneGemeente,
                        OverledeneVoorregeling = PersoonsGegevens.OverledeneVoorregeling
                    };
                }
            }

            if (IsOverlijdenInfoValid)
            {
                try
                {
                    //Add the newly found to the database
                    NewSuggestion.Id = Guid.NewGuid();
                    NewSuggestion.ShortName = string.Empty;
                    NewSuggestion.LongName = OverlijdenLocatieLocal;
                    NewSuggestion.Street = OverlijdenInfo.OverledenAdres;
                    NewSuggestion.HouseNumber = OverlijdenInfo.OverledenHuisnummer;
                    NewSuggestion.ZipCode = OverlijdenInfo.OverledenPostcode;
                    NewSuggestion.City = OverlijdenInfo.OverledenPlaats;
                    NewSuggestion.County = OverlijdenInfo.OverledenGemeente;

                    int locationCheck = miscellaneousRepository.CheckLocationExistance(NewSuggestion);

                    if (locationCheck == 0)
                        createRepository.CreateSuggestion(NewSuggestion);
                }
                catch (Exception ex)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }

                if (OverlijdenInfo.Id == Guid.Empty && !OverlijdenInfoExists)
                {
                    OverlijdenInfo.Id = OverlijdenInfoId;
                    OverlijdenInfo.UitvaartId = UitvaartLeider.UitvaartId;

                    try
                    {
                        createRepository.AddOverlijdenInfo(OverlijdenInfo);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error inserting overlijdeninfo: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else if(OverlijdenInfoExists)
                {
                    bool overlijdenInfoChanged = modelCompare.AreValuesEqual(_originalOverlijdenInfoModel, OverlijdenInfo);

                    if (overlijdenInfoChanged == false)
                    {
                        try
                        {
                            updateRepository.EditOverlijdenInfo(OverlijdenInfo);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Error updating overlijdeninfo: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                            return;
                        }

                        _originalOverlijdenInfoModel = new OverledeneOverlijdenInfoModel
                        {
                            OverledenDatumTijd = OverlijdenInfo.OverledenDatumTijd,
                            OverledenAdres = OverlijdenInfo.OverledenAdres,
                            OverledenPostcode = OverlijdenInfo.OverledenPostcode,
                            OverledenHuisnummer = OverlijdenInfo.OverledenHuisnummer,
                            OverledenHuisnummerToevoeging = OverlijdenInfo.OverledenHuisnummerToevoeging,
                            OverledenPlaats = OverlijdenInfo.OverledenPlaats,
                            OverledenGemeente = OverlijdenInfo.OverledenGemeente,
                            OverledenLijkvinding = OverlijdenInfo.OverledenLijkvinding,
                            OverledenHerkomst = OverlijdenInfo.OverledenHerkomst,
                            OverledenLidnummer = OverlijdenInfo.OverledenLidnummer,
                            OverledenHuisarts = OverlijdenInfo.OverledenHuisarts,
                            OverledenHuisartsTelefoon = OverlijdenInfo.OverledenHuisartsTelefoon,
                            OverledenSchouwarts = OverlijdenInfo.OverledenSchouwarts,
                            OverledenLocatie = OverlijdenInfo.OverledenLocatie
                        };
                    }
                }
            }

            if (obj != null && obj.ToString() == "VolgendeButton")
            {
                ExtraInfoInstance.RequestedDossierInformationBasedOnUitvaartId(UitvaartLeider.Uitvaartnummer);
                IntAggregator.Transmit(2);
            }
        }
        private void ExecutePreviousCommand(object obj)
        {
            bool persoonsgegevensInfoChanged = modelCompare.AreValuesEqual(_oginalPersoonGegevensModel, PersoonsGegevens);
            bool overlijdenInfoChanged = modelCompare.AreValuesEqual(_originalOverlijdenInfoModel, OverlijdenInfo);
            bool uitvaartleiderInfoChanged = modelCompare.AreValuesEqual(_orginalUitvaartLeiderModel, UitvaartLeider);

            if (UitvaartLeider.HasData() || PersoonsGegevens.HasData() || OverlijdenInfo.HasData())
            {
                if (!persoonsgegevensInfoChanged || !overlijdenInfoChanged || !uitvaartleiderInfoChanged)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Onopgeslagen informatie", "Je hebt onopgeslagen informatie!", "Als je nu teruggaat dan verlies je de niet opgelsagen informatie.", "Begrepen", "Blijven");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IntAggregator.Transmit(0);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        return;
                    }
                }
                else
                {
                    IntAggregator.Transmit(0);
                }
            }
            else
            {
                IntAggregator.Transmit(0);
            }
        }
    }
}
