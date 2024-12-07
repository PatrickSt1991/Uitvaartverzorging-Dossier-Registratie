using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Dossier_Registratie.ViewModels.OverledeneVerzekeringViewModal;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneExtraInfoViewModal : ViewModelBase
    {
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private OverledeneUitvaartleiderModel _extraInfoUitvaartLeiderModel;
        private OverledeneExtraInfoModel? _overledeneExtraInfoModel;
        private OpdrachtgeverPersoonsGegevensModel? _opdrachtgeverPersoonsGegevensModel;
        private OpdrachtgeverPersoonsGegevensModel? _extraOpdrachtgeverPersoonsGegevensModel;
        private OpdrachtgeverPersoonsGegevensModel _originalOpdrachtPersoonsGegevens;
        private OpdrachtgeverPersoonsGegevensModel _originalExtraOpdrachtPersoonsGegevens;
        private OverledeneExtraInfoModel _orginalOverledeneExtraInfo;
        private ModelCompare modelCompare;
        private ComboBoxItem _selectedMaritalStatus;
        private bool isCreateExtraContactPopupOpen;
        private bool extraContact;
        private bool _correctAccessOrNotCompleted = true;

        public ComboBoxItem SelectedMaritalStatus
        {
            get { return _selectedMaritalStatus; }
            set
            {
                if (_selectedMaritalStatus != value)
                {
                    _selectedMaritalStatus = value;
                    OnPropertyChanged(nameof(SelectedMaritalStatus));
                }
            }
        }
        public OverledeneUitvaartleiderModel ExtraInfoUitvaartleider
        {
            get
            {
                return _extraInfoUitvaartLeiderModel;
            }
            set
            {
                _extraInfoUitvaartLeiderModel = value;
                OnPropertyChanged(nameof(ExtraInfoUitvaartleider));
            }
        }
        public OverledeneExtraInfoModel OverledeneExtraInfo
        {
            get
            {
                return _overledeneExtraInfoModel;
            }

            set
            {
                _overledeneExtraInfoModel = value;
                OnPropertyChanged(nameof(OverledeneExtraInfo));
            }
        }
        public bool IsCreateExtraContactPopupOpen
        {
            get { return isCreateExtraContactPopupOpen; }
            set
            {
                if (isCreateExtraContactPopupOpen != value)
                {
                    isCreateExtraContactPopupOpen = value;
                    OnPropertyChanged(nameof(IsCreateExtraContactPopupOpen));
                }
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

        public OpdrachtgeverPersoonsGegevensModel OpdrachtgeverPersoonsgegevens
        {
            get
            {
                return _opdrachtgeverPersoonsGegevensModel;
            }

            set
            {
                _opdrachtgeverPersoonsGegevensModel = value;
                OnPropertyChanged(nameof(OpdrachtgeverPersoonsgegevens));
            }
        }
        public OpdrachtgeverPersoonsGegevensModel ExtraOpdrachtgeverPersoonsgegevens
        {
            get
            {
                return _extraOpdrachtgeverPersoonsGegevensModel;
            }

            set
            {
                _extraOpdrachtgeverPersoonsGegevensModel = value;
                OnPropertyChanged(nameof(ExtraOpdrachtgeverPersoonsgegevens));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand CloseCreateExtraContactPopupCommand { get; }
        public ICommand OpenCreateExtraContactPopupCommand { get; }
        public ICommand SaveExtraContactPersoonCommand { get; }
        private OverledeneExtraInfoViewModal()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            CloseCreateExtraContactPopupCommand = new RelayCommand(() => IsCreateExtraContactPopupOpen = false);
            OpenCreateExtraContactPopupCommand = new RelayCommand(() => IsCreateExtraContactPopupOpen = true);
            modelCompare = new ModelCompare();
            ExtraInfoUitvaartleider = new OverledeneUitvaartleiderModel();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            OverledeneExtraInfo = new OverledeneExtraInfoModel();
            OpdrachtgeverPersoonsgegevens = new OpdrachtgeverPersoonsGegevensModel();
            ExtraOpdrachtgeverPersoonsgegevens = new OpdrachtgeverPersoonsGegevensModel();

            SaveExtraContactPersoonCommand = new ViewModelCommand(ExecuteSaveExtraContactPersoonCommand);
            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
        }
        public async void FetchExtraInfoAddressInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(OpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode) && (!string.IsNullOrEmpty(OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer)))
                {
                    var eIaddress = await SearchAddressApi.GetAddressAsync(OpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode, OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer, OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging);
                    if (eIaddress != null)
                    {
                        OpdrachtgeverPersoonsgegevens.OpdrachtgeverStraat = eIaddress.Street;
                        OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer = eIaddress.HouseNumber;
                        OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging = eIaddress.HouseNumberAddition;
                        OpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode = eIaddress.PostalCode;
                        OpdrachtgeverPersoonsgegevens.OpdrachtgeverWoonplaats = eIaddress.City;
                        OpdrachtgeverPersoonsgegevens.OpdrachtgeverGemeente = eIaddress.County;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Extra Info persoonsgegevens address lookup: {ex.Message}", "Lookup Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }
        }
        public async void FetchSecondExtraInfoAddressInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode) && (!string.IsNullOrEmpty(ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer)))
                {
                    var eEIaddress = await SearchAddressApi.GetAddressAsync(ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode, ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer, ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging);
                    if (eEIaddress != null)
                    {
                        ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverStraat = eEIaddress.Street;
                        ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer = eEIaddress.HouseNumber;
                        ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging = eEIaddress.HouseNumberAddition;
                        ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode = eEIaddress.PostalCode;
                        ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverWoonplaats = eEIaddress.City;
                        ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverGemeente = eEIaddress.County;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Second Extra persoonsgegevens address lookup: {ex.Message}", "Lookup Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }
        }
        public void CreateNewDossier()
        {
            modelCompare = new ModelCompare();
            OverledeneExtraInfo = new OverledeneExtraInfoModel();
            OpdrachtgeverPersoonsgegevens = new OpdrachtgeverPersoonsGegevensModel();
            ExtraOpdrachtgeverPersoonsgegevens = new OpdrachtgeverPersoonsGegevensModel();

            ExtraInfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            ExtraInfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;
        }
        public static OverledeneExtraInfoViewModal ExtraInfoInstance { get; } = new();
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            modelCompare = new ModelCompare();
            OverledeneExtraInfo = new OverledeneExtraInfoModel();
            OpdrachtgeverPersoonsgegevens = new OpdrachtgeverPersoonsGegevensModel();
            ExtraOpdrachtgeverPersoonsgegevens = new OpdrachtgeverPersoonsGegevensModel();

            var UitvaarLeiderResult = searchRepository.GetUitvaarleiderByUitvaartId(uitvaartNummer);
            if (UitvaarLeiderResult != null)
            {
                ExtraInfoUitvaartleider.Uitvaartnummer = UitvaarLeiderResult.Uitvaartnummer;
                ExtraInfoUitvaartleider.PersoneelNaam = UitvaarLeiderResult.PersoneelNaam;

                Globals.UitvaartCode = UitvaarLeiderResult.Uitvaartnummer;
                Globals.UitvaartCodeGuid = UitvaarLeiderResult.UitvaartId;
                Globals.UitvaarLeider = UitvaarLeiderResult.PersoneelNaam;
            }

            var OpdrachtgeverPersoonsGegevensResult = searchRepository.GetOpdrachtgeverByUitvaartId(uitvaartNummer);
            if (OpdrachtgeverPersoonsGegevensResult != null)
            {
                OpdrachtgeverPersoonsgegevens.Id = OpdrachtgeverPersoonsGegevensResult.Id;
                OpdrachtgeverPersoonsgegevens.UitvaartId = OpdrachtgeverPersoonsGegevensResult.UitvaartId;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverBSN = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverBSN;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverAanhef = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverAanhef;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverAchternaam = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverAchternaam;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverTussenvoegsel = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverTussenvoegsel;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverVoornaamen = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverVoornaamen;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverGeboortedatum = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverGeboortedatum;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverLeeftijd = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverLeeftijd;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverStraat = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverStraat;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverHuisnummer;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverHuisnummerToevoeging;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverPostcode;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverWoonplaats = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverWoonplaats;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverGemeente = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverGemeente;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverTelefoon = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverTelefoon;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverRelatieTotOverledene = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverRelatieTotOverledene;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverExtraInformatie = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverExtraInformatie;
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverEmail = OpdrachtgeverPersoonsGegevensResult.OpdrachtgeverEmail;

                _originalOpdrachtPersoonsGegevens = new OpdrachtgeverPersoonsGegevensModel
                {
                    Id = OpdrachtgeverPersoonsgegevens.Id,
                    UitvaartId = OpdrachtgeverPersoonsgegevens.UitvaartId,
                    OpdrachtgeverBSN = OpdrachtgeverPersoonsgegevens.OpdrachtgeverBSN,
                    OpdrachtgeverAanhef = OpdrachtgeverPersoonsgegevens.OpdrachtgeverAanhef,
                    OpdrachtgeverAchternaam = OpdrachtgeverPersoonsgegevens.OpdrachtgeverAchternaam,
                    OpdrachtgeverTussenvoegsel = OpdrachtgeverPersoonsgegevens.OpdrachtgeverTussenvoegsel,
                    OpdrachtgeverVoornaamen = OpdrachtgeverPersoonsgegevens.OpdrachtgeverVoornaamen,
                    OpdrachtgeverGeboortedatum = OpdrachtgeverPersoonsgegevens.OpdrachtgeverGeboortedatum,
                    OpdrachtgeverLeeftijd = OpdrachtgeverPersoonsgegevens.OpdrachtgeverLeeftijd,
                    OpdrachtgeverStraat = OpdrachtgeverPersoonsgegevens.OpdrachtgeverStraat,
                    OpdrachtgeverHuisnummer = OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer,
                    OpdrachtgeverHuisnummerToevoeging = OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging,
                    OpdrachtgeverPostcode = OpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode,
                    OpdrachtgeverWoonplaats = OpdrachtgeverPersoonsgegevens.OpdrachtgeverWoonplaats,
                    OpdrachtgeverGemeente = OpdrachtgeverPersoonsgegevens.OpdrachtgeverGemeente,
                    OpdrachtgeverTelefoon = OpdrachtgeverPersoonsgegevens.OpdrachtgeverTelefoon,
                    OpdrachtgeverRelatieTotOverledene = OpdrachtgeverPersoonsgegevens.OpdrachtgeverRelatieTotOverledene,
                    OpdrachtgeverExtraInformatie = OpdrachtgeverPersoonsgegevens.OpdrachtgeverExtraInformatie,
                    OpdrachtgeverEmail = OpdrachtgeverPersoonsgegevens.OpdrachtgeverEmail
                };
            }

            var OpdrachtgeverExtraPersoonsGegevensResult = searchRepository.GetExtraOpdrachtgeverByUitvaartId(uitvaartNummer);
            if (OpdrachtgeverExtraPersoonsGegevensResult != null)
            {
                extraContact = true;
                ExtraOpdrachtgeverPersoonsgegevens.Id = OpdrachtgeverExtraPersoonsGegevensResult.Id;
                ExtraOpdrachtgeverPersoonsgegevens.UitvaartId = OpdrachtgeverExtraPersoonsGegevensResult.UitvaartId;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverBSN = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverBSN;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverAanhef = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverAanhef;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverAchternaam = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverAchternaam;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverTussenvoegsel = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverTussenvoegsel;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverVoornaamen = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverVoornaamen;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverGeboortedatum = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverGeboortedatum;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverEmail = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverEmail;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverLeeftijd = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverLeeftijd;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverStraat = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverStraat;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverHuisnummer;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverHuisnummerToevoeging;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverPostcode;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverWoonplaats = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverWoonplaats;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverGemeente = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverGemeente;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverTelefoon = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverTelefoon;
                ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverRelatieTotOverledene = OpdrachtgeverExtraPersoonsGegevensResult.OpdrachtgeverRelatieTotOverledene;

                _originalExtraOpdrachtPersoonsGegevens = new OpdrachtgeverPersoonsGegevensModel
                {
                    Id = ExtraOpdrachtgeverPersoonsgegevens.Id,
                    UitvaartId = ExtraOpdrachtgeverPersoonsgegevens.UitvaartId,
                    OpdrachtgeverBSN = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverBSN,
                    OpdrachtgeverAanhef = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverAanhef,
                    OpdrachtgeverAchternaam = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverAchternaam,
                    OpdrachtgeverTussenvoegsel = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverTussenvoegsel,
                    OpdrachtgeverVoornaamen = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverVoornaamen,
                    OpdrachtgeverGeboortedatum = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverGeboortedatum,
                    OpdrachtgeverLeeftijd = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverLeeftijd,
                    OpdrachtgeverStraat = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverStraat,
                    OpdrachtgeverHuisnummer = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer,
                    OpdrachtgeverHuisnummerToevoeging = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging,
                    OpdrachtgeverPostcode = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode,
                    OpdrachtgeverWoonplaats = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverWoonplaats,
                    OpdrachtgeverGemeente = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverGemeente,
                    OpdrachtgeverTelefoon = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverTelefoon,
                    OpdrachtgeverRelatieTotOverledene = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverRelatieTotOverledene,
                    OpdrachtgeverEmail = ExtraOpdrachtgeverPersoonsgegevens.OpdrachtgeverEmail
                };
            }

            var OverledeneExtraInfoResult = searchRepository.GetExtraInfoByUitvaartId(uitvaartNummer);
            if (OverledeneExtraInfoResult != null)
            {
                OverledeneExtraInfo.Id = OverledeneExtraInfoResult.Id;
                OverledeneExtraInfo.UitvaartId = OverledeneExtraInfoResult.UitvaartId;
                OverledeneExtraInfo.OverledeneBurgelijkestaat = OverledeneExtraInfoResult.OverledeneBurgelijkestaat;
                OverledeneExtraInfo.OverledeneLevensovertuiging = OverledeneExtraInfoResult.OverledeneLevensovertuiging;
                OverledeneExtraInfo.OverledeneAantalKinderen = OverledeneExtraInfoResult.OverledeneAantalKinderen;
                OverledeneExtraInfo.OverledeneKinderenMinderjarig = OverledeneExtraInfoResult.OverledeneKinderenMinderjarig;
                OverledeneExtraInfo.OverledeneKinderenMinderjarigOverleden = OverledeneExtraInfoResult.OverledeneKinderenMinderjarigOverleden;
                OverledeneExtraInfo.OverledeneEersteOuder = OverledeneExtraInfoResult.OverledeneEersteOuder;
                OverledeneExtraInfo.OverledeneEersteOuderOverleden = OverledeneExtraInfoResult.OverledeneEersteOuderOverleden;
                OverledeneExtraInfo.OverledeneTweedeOuder = OverledeneExtraInfoResult.OverledeneTweedeOuder;
                OverledeneExtraInfo.OverledeneTweedeOuderOverleden = OverledeneExtraInfoResult.OverledeneTweedeOuderOverleden;
                OverledeneExtraInfo.OverledeneExecuteur = OverledeneExtraInfoResult.OverledeneExecuteur;
                OverledeneExtraInfo.OverledeneExecuteurTelefoon = OverledeneExtraInfoResult.OverledeneExecuteurTelefoon;
                OverledeneExtraInfo.OverledeneTrouwboekje = OverledeneExtraInfoResult.OverledeneTrouwboekje;
                OverledeneExtraInfo.OverledeneNotaris = OverledeneExtraInfoResult.OverledeneNotaris;
                OverledeneExtraInfo.OverledeneNotarisTelefoon = OverledeneExtraInfoResult.OverledeneNotarisTelefoon;
                OverledeneExtraInfo.OverledeneTestament = OverledeneExtraInfoResult.OverledeneTestament;
                OverledeneExtraInfo.OverledeneTrouwDatumTijd = OverledeneExtraInfoResult.OverledeneTrouwDatumTijd;
                OverledeneExtraInfo.OverledeneGeregistreerdDatum = OverledeneExtraInfoResult.OverledeneGeregistreerdDatum;
                OverledeneExtraInfo.OverledeneGeregistreerdTijd = OverledeneExtraInfoResult.OverledeneGeregistreerdTijd;
                OverledeneExtraInfo.OverledeneGescheidenVan = OverledeneExtraInfoResult.OverledeneGescheidenVan;
                OverledeneExtraInfo.OverledeneWedenaarVan = OverledeneExtraInfoResult.OverledeneWedenaarVan;
                OverledeneExtraInfo.NaamWederhelft = OverledeneExtraInfoResult.NaamWederhelft;
                OverledeneExtraInfo.VoornaamWederhelft = OverledeneExtraInfoResult.VoornaamWederhelft;

                _orginalOverledeneExtraInfo = new OverledeneExtraInfoModel
                {
                    Id = OverledeneExtraInfo.Id,
                    OverledeneBurgelijkestaat = OverledeneExtraInfo.OverledeneBurgelijkestaat,
                    OverledeneLevensovertuiging = OverledeneExtraInfo.OverledeneLevensovertuiging,
                    OverledeneAantalKinderen = OverledeneExtraInfo.OverledeneAantalKinderen,
                    OverledeneKinderenMinderjarig = OverledeneExtraInfo.OverledeneKinderenMinderjarig,
                    OverledeneKinderenMinderjarigOverleden = OverledeneExtraInfoResult.OverledeneKinderenMinderjarigOverleden,
                    OverledeneEersteOuder = OverledeneExtraInfo.OverledeneEersteOuder,
                    OverledeneEersteOuderOverleden = OverledeneExtraInfoResult.OverledeneEersteOuderOverleden,
                    OverledeneTweedeOuder = OverledeneExtraInfo.OverledeneTweedeOuder,
                    OverledeneTweedeOuderOverleden = OverledeneExtraInfoResult.OverledeneTweedeOuderOverleden,
                    OverledeneExecuteur = OverledeneExtraInfo.OverledeneExecuteur,
                    OverledeneExecuteurTelefoon = OverledeneExtraInfo.OverledeneExecuteur,
                    OverledeneTrouwboekje = OverledeneExtraInfo.OverledeneTrouwboekje,
                    OverledeneNotaris = OverledeneExtraInfo.OverledeneNotaris,
                    OverledeneNotarisTelefoon = OverledeneExtraInfo.OverledeneNotarisTelefoon,
                    OverledeneTestament = OverledeneExtraInfo.OverledeneTestament,
                    OverledeneTrouwDatumTijd = OverledeneExtraInfo.OverledeneTrouwDatumTijd,
                    OverledeneGeregistreerdDatum = OverledeneExtraInfo.OverledeneGeregistreerdDatum,
                    OverledeneGeregistreerdTijd = OverledeneExtraInfo.OverledeneGeregistreerdTijd,
                    OverledeneGescheidenVan = OverledeneExtraInfo.OverledeneGescheidenVan,
                    OverledeneWedenaarVan = OverledeneExtraInfo.OverledeneWedenaarVan,
                    NaamWederhelft = OverledeneExtraInfo.NaamWederhelft,
                    VoornaamWederhelft = OverledeneExtraInfo.VoornaamWederhelft
                };
            }
        }
        private bool CanExecuteSaveCommand(object obj)
        {
            return true;
        }
        private void ExecuteSaveCommand(object obj)
        {
            Guid ExtraInfoId = Guid.NewGuid();
            Guid PersoonGegevensId = Guid.NewGuid();
            OverledeneExtraInfo.UitvaartId = Globals.UitvaartCodeGuid;
            OpdrachtgeverPersoonsgegevens.UitvaartId = Globals.UitvaartCodeGuid;
            ExtraOpdrachtgeverPersoonsgegevens.UitvaartId = Globals.UitvaartCodeGuid;

            bool ExtraInfoExists = miscellaneousRepository.UitvaartExtraInfoExists(OverledeneExtraInfo.UitvaartId);
            bool OpdrachtgeverPersoonsgegevenExists = miscellaneousRepository.UitvaartOpdrachtgeverPersoonsgegevensExists(OpdrachtgeverPersoonsgegevens.UitvaartId);

            if (string.IsNullOrEmpty(OpdrachtgeverPersoonsgegevens.OpdrachtgeverRelatieTotOverledene))
                OpdrachtgeverPersoonsgegevens.OpdrachtgeverRelatieTotOverledene = "-";

            if (!OverledeneExtraInfo.HasData())
            {
                new ToastWindow("Niet alle verplichte extra info velden zijn ingevuld!").Show();
                return;
            }
            else if (!Globals.Voorregeling && !OpdrachtgeverPersoonsgegevens.HasData())
            {
                new ToastWindow("Niet alle verplichte persoonsgegevens opdrachtgever velden zijn ingevuld! (Geen voorregeling)").Show();
                return;
            }

            if (OverledeneExtraInfo.Id == Guid.Empty && !ExtraInfoExists)
            {
                OverledeneExtraInfo.Id = ExtraInfoId;
                try
                {
                    createRepository.AddOverlijdenExtraInfo(OverledeneExtraInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting extra info: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (ExtraInfoExists)
            {
                bool OverlijdenExtraInfoChanged = modelCompare.AreValuesEqual(_orginalOverledeneExtraInfo, OverledeneExtraInfo);

                if (OverlijdenExtraInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditOverlijdenExtraInfo(OverledeneExtraInfo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating Extra info: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _orginalOverledeneExtraInfo = new OverledeneExtraInfoModel
                    {
                        Id = OverledeneExtraInfo.Id,
                        UitvaartId = OverledeneExtraInfo.UitvaartId,
                        OverledeneBurgelijkestaat = OverledeneExtraInfo.OverledeneBurgelijkestaat,
                        OverledeneLevensovertuiging = OverledeneExtraInfo.OverledeneLevensovertuiging,
                        OverledeneAantalKinderen = OverledeneExtraInfo.OverledeneAantalKinderen,
                        OverledeneKinderenMinderjarig = OverledeneExtraInfo.OverledeneKinderenMinderjarig,
                        OverledeneKinderenMinderjarigOverleden = OverledeneExtraInfo.OverledeneKinderenMinderjarigOverleden,
                        OverledeneEersteOuder = OverledeneExtraInfo.OverledeneEersteOuder,
                        OverledeneEersteOuderOverleden = OverledeneExtraInfo.OverledeneEersteOuderOverleden,
                        OverledeneTweedeOuder = OverledeneExtraInfo.OverledeneTweedeOuder,
                        OverledeneTweedeOuderOverleden = OverledeneExtraInfo.OverledeneTweedeOuderOverleden,
                        OverledeneExecuteur = OverledeneExtraInfo.OverledeneExecuteur,
                        OverledeneExecuteurTelefoon = OverledeneExtraInfo.OverledeneExecuteur,
                        OverledeneTrouwboekje = OverledeneExtraInfo.OverledeneTrouwboekje,
                        OverledeneNotaris = OverledeneExtraInfo.OverledeneNotaris,
                        OverledeneNotarisTelefoon = OverledeneExtraInfo.OverledeneNotarisTelefoon,
                        OverledeneTestament = OverledeneExtraInfo.OverledeneTestament,
                        OverledeneTrouwDatumTijd = OverledeneExtraInfo.OverledeneTrouwDatumTijd,
                        OverledeneGeregistreerdDatumTijd = OverledeneExtraInfo.OverledeneGeregistreerdDatumTijd,
                        OverledeneGescheidenVan = OverledeneExtraInfo.OverledeneGescheidenVan,
                        OverledeneWedenaarVan = OverledeneExtraInfo.OverledeneWedenaarVan,
                        VoornaamWederhelft = OverledeneExtraInfo.VoornaamWederhelft,
                        NaamWederhelft = OverledeneExtraInfo.NaamWederhelft
                    };
                }
            }

            if (OpdrachtgeverPersoonsgegevens.Id == Guid.Empty && !OpdrachtgeverPersoonsgegevenExists)
            {
                OpdrachtgeverPersoonsgegevens.Id = PersoonGegevensId;
                try
                {
                    createRepository.AddOpdrachtgeverPersoonsGegevens(OpdrachtgeverPersoonsgegevens);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting opdrachtgever: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (OpdrachtgeverPersoonsgegevenExists)
            {
                bool OpdrachtgeverGegevensInfoChanged = modelCompare.AreValuesEqual(_originalOpdrachtPersoonsGegevens, OpdrachtgeverPersoonsgegevens);

                if (OpdrachtgeverGegevensInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditOpdrachtgeverPersoonsGegevens(OpdrachtgeverPersoonsgegevens);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating opdrachtgever: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalOpdrachtPersoonsGegevens = new OpdrachtgeverPersoonsGegevensModel
                    {
                        Id = OpdrachtgeverPersoonsgegevens.Id,
                        UitvaartId = OpdrachtgeverPersoonsgegevens.UitvaartId,
                        OpdrachtgeverBSN = OpdrachtgeverPersoonsgegevens.OpdrachtgeverBSN,
                        OpdrachtgeverAanhef = OpdrachtgeverPersoonsgegevens.OpdrachtgeverAanhef,
                        OpdrachtgeverAchternaam = OpdrachtgeverPersoonsgegevens.OpdrachtgeverAchternaam,
                        OpdrachtgeverTussenvoegsel = OpdrachtgeverPersoonsgegevens.OpdrachtgeverTussenvoegsel,
                        OpdrachtgeverVoornaamen = OpdrachtgeverPersoonsgegevens.OpdrachtgeverVoornaamen,
                        OpdrachtgeverGeboortedatum = OpdrachtgeverPersoonsgegevens.OpdrachtgeverGeboortedatum,
                        OpdrachtgeverGeboorteplaats = OpdrachtgeverPersoonsgegevens.OpdrachtgeverGeboorteplaats,
                        OpdrachtgeverLeeftijd = OpdrachtgeverPersoonsgegevens.OpdrachtgeverLeeftijd,
                        OpdrachtgeverStraat = OpdrachtgeverPersoonsgegevens.OpdrachtgeverStraat,
                        OpdrachtgeverHuisnummer = OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer,
                        OpdrachtgeverHuisnummerToevoeging = OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging,
                        OpdrachtgeverPostcode = OpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode,
                        OpdrachtgeverWoonplaats = OpdrachtgeverPersoonsgegevens.OpdrachtgeverWoonplaats,
                        OpdrachtgeverGemeente = OpdrachtgeverPersoonsgegevens.OpdrachtgeverGemeente,
                        OpdrachtgeverTelefoon = OpdrachtgeverPersoonsgegevens.OpdrachtgeverTelefoon,
                        OpdrachtgeverRelatieTotOverledene = OpdrachtgeverPersoonsgegevens.OpdrachtgeverRelatieTotOverledene,
                        OpdrachtgeverExtraInformatie = OpdrachtgeverPersoonsgegevens.OpdrachtgeverExtraInformatie
                    };
                }
            }

            if (obj != null && obj.ToString() == "VolgendeButton")
            {
                VerzekeringInstance.RequestedDossierInformationBasedOnUitvaartId(ExtraInfoUitvaartleider.Uitvaartnummer);
                IntAggregator.Transmit(3);
            }
        }
        private void ExecutePreviousCommand(object obj)
        {
            bool OverlijdenExtraInfoChanged = modelCompare.AreValuesEqual(_orginalOverledeneExtraInfo, OverledeneExtraInfo);
            bool OpdrachtgeverGegevensInfoChanged = modelCompare.AreValuesEqual(_originalOpdrachtPersoonsGegevens, OpdrachtgeverPersoonsgegevens);

            if (OverledeneExtraInfo.HasData() || OpdrachtgeverPersoonsgegevens.HasData())
            {
                if (!OverlijdenExtraInfoChanged || !OpdrachtgeverGegevensInfoChanged)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Onopgeslagen informatie", "Je hebt onopgeslagen informatie!", "Als je nu teruggaat dan verlies je de niet opgelsagen informatie.", "Begrepen", "Blijven");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IntAggregator.Transmit(1);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        return;
                    }
                }
                else
                {
                    IntAggregator.Transmit(1);
                }
            }
            else
            {
                IntAggregator.Transmit(1);
            }
        }
        public void ExecuteSaveExtraContactPersoonCommand(object obj)
        {
            CloseCreateExtraContactPopupCommand.Execute(null);

            if (ExtraOpdrachtgeverPersoonsgegevens.Id == Guid.Empty)
            {
                ExtraOpdrachtgeverPersoonsgegevens.Id = Guid.NewGuid();
                ExtraOpdrachtgeverPersoonsgegevens.UitvaartId = Globals.UitvaartCodeGuid;
                try
                {
                    createRepository.AddOpdrachtgeverExtraPersoonsGegevens(ExtraOpdrachtgeverPersoonsgegevens);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting extra contactpersoon: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                bool ExtraOpdrachtgeverInfoChanged = modelCompare.AreValuesEqual(_originalExtraOpdrachtPersoonsGegevens, ExtraOpdrachtgeverPersoonsgegevens);

                if (ExtraOpdrachtgeverInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditOpdrachtgeverExtraPersoonsGegevens(ExtraOpdrachtgeverPersoonsgegevens);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating extra contactpersoon: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalExtraOpdrachtPersoonsGegevens = new OpdrachtgeverPersoonsGegevensModel
                    {
                        Id = OpdrachtgeverPersoonsgegevens.Id,
                        UitvaartId = OpdrachtgeverPersoonsgegevens.UitvaartId,
                        OpdrachtgeverBSN = OpdrachtgeverPersoonsgegevens.OpdrachtgeverBSN,
                        OpdrachtgeverAanhef = OpdrachtgeverPersoonsgegevens.OpdrachtgeverAanhef,
                        OpdrachtgeverAchternaam = OpdrachtgeverPersoonsgegevens.OpdrachtgeverAchternaam,
                        OpdrachtgeverTussenvoegsel = OpdrachtgeverPersoonsgegevens.OpdrachtgeverTussenvoegsel,
                        OpdrachtgeverVoornaamen = OpdrachtgeverPersoonsgegevens.OpdrachtgeverVoornaamen,
                        OpdrachtgeverGeboortedatum = OpdrachtgeverPersoonsgegevens.OpdrachtgeverGeboortedatum,
                        OpdrachtgeverGeboorteplaats = OpdrachtgeverPersoonsgegevens.OpdrachtgeverGeboorteplaats,
                        OpdrachtgeverLeeftijd = OpdrachtgeverPersoonsgegevens.OpdrachtgeverLeeftijd,
                        OpdrachtgeverStraat = OpdrachtgeverPersoonsgegevens.OpdrachtgeverStraat,
                        OpdrachtgeverHuisnummer = OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummer,
                        OpdrachtgeverHuisnummerToevoeging = OpdrachtgeverPersoonsgegevens.OpdrachtgeverHuisnummerToevoeging,
                        OpdrachtgeverPostcode = OpdrachtgeverPersoonsgegevens.OpdrachtgeverPostcode,
                        OpdrachtgeverWoonplaats = OpdrachtgeverPersoonsgegevens.OpdrachtgeverWoonplaats,
                        OpdrachtgeverGemeente = OpdrachtgeverPersoonsgegevens.OpdrachtgeverGemeente,
                        OpdrachtgeverTelefoon = OpdrachtgeverPersoonsgegevens.OpdrachtgeverTelefoon,
                        OpdrachtgeverRelatieTotOverledene = OpdrachtgeverPersoonsgegevens.OpdrachtgeverRelatieTotOverledene
                    };
                }
            }
        }
    }
}
