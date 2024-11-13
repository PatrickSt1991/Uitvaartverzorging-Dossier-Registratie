using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Dossier_Registratie.MainWindow;
using Task = System.Threading.Tasks.Task;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneSteenhouwerijViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations deleteAndActivateDisableOperations;
        readonly DocumentGenerator documentGenerator = new DocumentGenerator();

        private bool _isLintChecked = false;
        private bool isFinishPopupOpen;
        private bool _correctAccessOrNotCompleted = true;
        private string dossierTagContent;
        private string _lint1 = string.Empty;
        private string _lint2 = string.Empty;
        private string _lint3 = string.Empty;
        private string _lint4 = string.Empty;
        private Guid selectedSteenhouwerEmployeeId;
        private Guid selectedUrnSieradenEmployeeId;
        private Guid selectedBloemEmployeeId;
        private Visibility werkbonPrintButtonVisable = Visibility.Collapsed;
        private Visibility _lintTextVisable = Visibility.Collapsed;

        private OverledeneUitvaartleiderModel _uitvaartLeiderModel;
        private OverledeneSteenhouwerijModel _steenhouwerijModel;
        private OverledeneUrnSieradenModel _urnsieradenModel;
        private OverledeneBloemenModel _bloemenModel;
        private OverledeneWerkbonUitvaart _werkbonModel;
        private ModelCompare modelCompare;
        private OverledeneSteenhouwerijModel _originalSteenhouwerijModel;
        private OverledeneUrnSieradenModel _orginalUrnsieradenModel;
        private OverledeneBloemenModel _originalBloemenModel;
        private OverledeneWerkbonUitvaart _originalWerkbonModel;
        private Klanttevredenheid _klantTevredenheid;
        private OverledeneBijlagesModel _dossier;
        private BloemenDocument _bloemenDocModel;
        private GeneratingDocumentView _generatingDocumentView;

        public bool initialLoadDone;
        public bool CorrectAccessOrNotCompleted
        {
            get { return _correctAccessOrNotCompleted; }
            set
            {
                _correctAccessOrNotCompleted = value;
                OnPropertyChanged(nameof(CorrectAccessOrNotCompleted));
            }
        }
        public OverledeneBijlagesModel Dossier
        {
            get { return _dossier; }
            set { _dossier = value; OnPropertyChanged(nameof(Dossier)); }
        }
        public Klanttevredenheid Klanttevredenheid
        {
            get { return _klantTevredenheid; }
            set { _klantTevredenheid = value; OnPropertyChanged(nameof(Klanttevredenheid)); }
        }
        public OverledeneUitvaartleiderModel InfoUitvaartleider
        {
            get { return _uitvaartLeiderModel; }
            set { _uitvaartLeiderModel = value; OnPropertyChanged(nameof(InfoUitvaartleider)); }
        }
        public OverledeneSteenhouwerijModel SteenhouwerijModel
        {
            get { return _steenhouwerijModel; }
            set { _steenhouwerijModel = value; OnPropertyChanged(nameof(SteenhouwerijModel)); }
        }
        public OverledeneUrnSieradenModel UrnSieradenModel
        {
            get { return _urnsieradenModel; }
            set { _urnsieradenModel = value; OnPropertyChanged(nameof(UrnSieradenModel)); }
        }
        public OverledeneBloemenModel BloemenModel
        {
            get { return _bloemenModel; }
            set 
            { 
                _bloemenModel = value; 
                OnPropertyChanged(nameof(BloemenModel));
            }
        }
        public OverledeneWerkbonUitvaart WerkbonModel
        {
            get
            {
                return _werkbonModel;
            }
            set
            {
                _werkbonModel = value;
                OnPropertyChanged(nameof(WerkbonModel));
            }
        }
        public ObservableCollection<WerkbonPersoneel> WerkbonPersoneel { get; }
        public ObservableCollection<Werkbonnen> WerkbonnenList { get; }
        public ObservableCollection<LeveranciersModel> LeveranciersSteen { get; }
        public ObservableCollection<LeveranciersModel> LeveranciersBloem { get; }
        public ObservableCollection<LeveranciersModel> LeveranciersUrnSieraden { get; }
        public ObservableCollection<WerkbonPersoneel> WerkbonnenPersonen { get; }
        public ObservableCollection<WerknemersModel> WerknemerOverzicht { get; }
        public ObservableCollection<OverledeneSteenhouwerijModel> WerknemerSteenhouwerOverzicht { get; }
        public ObservableCollection<OverledeneUrnSieradenModel> WerknemerUrnSieradenOverzicht { get; }
        public ObservableCollection<OverledeneBloemenModel> WerknemerBloemOverzicht { get; }
        public BloemenDocument BloemenDocModel
        {
            get { return _bloemenDocModel; }
            set { _bloemenDocModel = value; OnPropertyChanged(nameof(BloemenDocModel)); }
        }
        public bool IsFinishPopupOpen
        {
            get { return isFinishPopupOpen; }
            set
            {
                if (isFinishPopupOpen != value)
                {
                    isFinishPopupOpen = value;
                    OnPropertyChanged(nameof(IsFinishPopupOpen));
                }
            }
        }
        public bool IsLintChecked
        {
            get { return _isLintChecked; }
            set
            {
                if(_isLintChecked != value)
                {
                    _isLintChecked = value;
                    OnPropertyChanged(nameof(IsLintChecked));
                    OnPropertyChanged(nameof(LintTextVisable));
                    BloemenModel.BloemenLint = IsLintChecked;
                }
            }
        }
        public Guid SelectedSteenhouwerEmployeeId
        {
            get { return selectedSteenhouwerEmployeeId; }
            set
            {
                if (selectedSteenhouwerEmployeeId != value)
                {
                    selectedSteenhouwerEmployeeId = value;
                    OnPropertyChanged(nameof(SelectedSteenhouwerEmployeeId));
                    RequestEmployeeSteenhouwerData(SelectedSteenhouwerEmployeeId);
                }
            }
        }
        public Guid SelectedUrnSieradenEmployeeId
        {
            get { return selectedUrnSieradenEmployeeId; }
            set
            {
                if (selectedUrnSieradenEmployeeId != value)
                {
                    selectedUrnSieradenEmployeeId = value;
                    OnPropertyChanged(nameof(SelectedUrnSieradenEmployeeId));
                    RequestEmployeeUrnSieradenData(SelectedUrnSieradenEmployeeId);
                }
            }
        }
        public Guid SelectedBloemEmployeeId
        {
            get { return selectedBloemEmployeeId; }
            set
            {
                if (selectedBloemEmployeeId != value)
                {
                    selectedBloemEmployeeId = value;
                    OnPropertyChanged(nameof(SelectedBloemEmployeeId));
                    RequestEmployeeBloemenData(SelectedBloemEmployeeId);
                }
            }
        }
        public Visibility WerkbonPrintButtonVisable
        {
            get { return werkbonPrintButtonVisable; }
            set { werkbonPrintButtonVisable = value; OnPropertyChanged(nameof(WerkbonPrintButtonVisable)); }
        }
        public Visibility LintTextVisable
        {
            get
            {
                return (IsLintChecked) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string Lint1
        {
            get => _lint1;
            set { _lint1 = value; OnPropertyChanged(nameof(Lint1)); }
        }
        public string Lint2
        {
            get => _lint2;
            set { _lint2 = value; OnPropertyChanged(nameof(Lint2)); }
        }
        public string Lint3
        {
            get => _lint3;
            set { _lint3 = value; OnPropertyChanged(nameof(Lint3)); }
        }
        public string Lint4
        {
            get => _lint4;
            set { _lint4 = value; OnPropertyChanged(nameof(Lint4)); }
        }
        public string DossierTagContent
        {
            get { return dossierTagContent; }
            set
            {
                if (dossierTagContent != value)
                {
                    dossierTagContent = value;
                    OnPropertyChanged(nameof(DossierTagContent));
                }
            }
        }
        public ICommand SaveSteenCommand { get; }
        public ICommand SaveBloemCommand { get; }
        public ICommand SaveWerkbonCommand { get; }
        public ICommand SaveUrnSieradenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand FinishCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand OpenFinishPopupOpenCommand { get; }
        public ICommand CloseFinishPopupOpenCommand { get; }
        public ICommand UploadDossierCommand { get; }
        public ICommand CreateDocumentBestelBloemenCommand { get; set; }
        public ICommand PrintWerkbonCommand { get; set; }
        private OverledeneSteenhouwerijViewModel()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            WerkbonnenPersonen = new ObservableCollection<WerkbonPersoneel>();
            SteenhouwerijModel = new OverledeneSteenhouwerijModel();
            UrnSieradenModel = new OverledeneUrnSieradenModel();
            BloemenModel = new OverledeneBloemenModel();
            WerkbonModel = new OverledeneWerkbonUitvaart();
            WerkbonnenList = new ObservableCollection<Werkbonnen>();
            LeveranciersSteen = new ObservableCollection<LeveranciersModel>();
            LeveranciersUrnSieraden = new ObservableCollection<LeveranciersModel>();
            LeveranciersBloem = new ObservableCollection<LeveranciersModel>();
            WerknemerOverzicht = new ObservableCollection<WerknemersModel>();
            WerknemerSteenhouwerOverzicht = new ObservableCollection<OverledeneSteenhouwerijModel>();
            WerknemerUrnSieradenOverzicht = new ObservableCollection<OverledeneUrnSieradenModel>();
            WerknemerBloemOverzicht = new ObservableCollection<OverledeneBloemenModel>();
            Klanttevredenheid = new Klanttevredenheid();
            Dossier = new OverledeneBijlagesModel();
            _generatingDocumentView = new GeneratingDocumentView();
            DossierTagContent = "Dossier Uploaden";

            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            deleteAndActivateDisableOperations = new DeleteAndActivateDisableOperations();

            foreach (var el in miscellaneousRepository.GetWerkbonPersoneel())
            {
                if (!el.IsDeleted)
                    WerkbonnenPersonen.Add(new WerkbonPersoneel { Id = el.Id, WerkbonPersoon = el.WerkbonPersoon });
            }

            foreach (var leverancier in miscellaneousRepository.GetLeveranciers())
            {
                if (!leverancier.IsDeleted)
                {
                    if (leverancier.Steenhouwer)
                    {
                        LeveranciersSteen.Add(new LeveranciersModel
                        {
                            LeverancierId = leverancier.LeverancierId,
                            LeverancierName = leverancier.LeverancierName,
                            LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                        });
                    }
                    if (leverancier.Bloemist)
                    {
                        LeveranciersBloem.Add(new LeveranciersModel
                        {
                            LeverancierId = leverancier.LeverancierId,
                            LeverancierName = leverancier.LeverancierName,
                            LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                        });
                    }
                    if (leverancier.UrnSieraden)
                    {
                        LeveranciersUrnSieraden.Add(new LeveranciersModel
                        {
                            LeverancierId = leverancier.LeverancierId,
                            LeverancierName = leverancier.LeverancierName,
                            LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                        });
                    }
                }
            }
            foreach (var werknemer in miscellaneousRepository.GetWerknemers())
            {
                if (!werknemer.IsDeleted)
                {
                    if (werknemer.IsUitvaartverzorger == true)
                    {
                        WerknemerOverzicht.Add(new WerknemersModel
                        {
                            Id = werknemer.Id,
                            VolledigeNaam = werknemer.VolledigeNaam
                        });
                    }
                }
            }

            SaveBloemCommand = new ViewModelCommand(ExecuteSaveBloemCommand, CanExecuteSaveBloemCommand);
            SaveSteenCommand = new ViewModelCommand(ExecuteSaveSteenCommand, CanExecuteSaveSteenCommand);
            SaveWerkbonCommand = new ViewModelCommand(ExecuteSaveWerkbonCommand, CanExecuteSaveWerkbonCommand);
            SaveUrnSieradenCommand = new ViewModelCommand(ExecuteSaveUrnSieradenCommand, CanExecuteSaveUrnSieradenCommand);
            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            FinishCommand = new ViewModelCommand(ExecuteFinishCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
            UploadDossierCommand = new ViewModelCommand(ExecuteUploadDossierCommand);
            CreateDocumentBestelBloemenCommand = new ViewModelCommand(async (parameter) => await CreateDocumentBestelBloemen(parameter));
            PrintWerkbonCommand = new ViewModelCommand(async (parameter) => await CreateWerkbonnenFileAsync(parameter));

            OpenFinishPopupOpenCommand = new RelayCommand(() => IsFinishPopupOpen = true);
            CloseFinishPopupOpenCommand = new RelayCommand(() => IsFinishPopupOpen = false);
        }
        public void ReloadDynamicElements()
        {
            foreach (var el in miscellaneousRepository.GetWerkbonPersoneel())
            {
                if (!WerkbonnenPersonen.Any(u => u.Id == el.Id) && el.IsDeleted == false)
                {
                    WerkbonnenPersonen.Add(new WerkbonPersoneel { Id = el.Id, WerkbonPersoon = el.WerkbonPersoon });
                }
            }
            foreach (var leverancier in miscellaneousRepository.GetLeveranciers())
            {
                if (!leverancier.IsDeleted)
                {
                    if (!LeveranciersSteen.Any(u => u.LeverancierId == leverancier.LeverancierId && leverancier.Steenhouwer == true))
                    {
                        if (leverancier.Steenhouwer)
                        {
                            MessageBox.Show(leverancier.LeverancierName + " - " + leverancier.Steenhouwer + " - " + leverancier.Bloemist + " - " + leverancier.UrnSieraden);
                            LeveranciersSteen.Add(new LeveranciersModel
                            {
                                LeverancierId = leverancier.LeverancierId,
                                LeverancierName = leverancier.LeverancierName,
                                LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                            });
                        }
                    }
                    if (!LeveranciersBloem.Any(u => u.LeverancierId == leverancier.LeverancierId))
                    {
                        if (leverancier.Bloemist)
                        {
                            LeveranciersBloem.Add(new LeveranciersModel
                            {
                                LeverancierId = leverancier.LeverancierId,
                                LeverancierName = leverancier.LeverancierName,
                                LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                            });
                        }
                    }
                    if (!LeveranciersUrnSieraden.Any(u => u.LeverancierId == leverancier.LeverancierId))
                    {
                        if (leverancier.UrnSieraden)
                        {
                            LeveranciersUrnSieraden.Add(new LeveranciersModel
                            {
                                LeverancierId = leverancier.LeverancierId,
                                LeverancierName = leverancier.LeverancierName,
                                LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                            });
                        }
                    }
                }
            }
            foreach (var werknemer in miscellaneousRepository.GetWerknemers())
            {
                if (!werknemer.IsDeleted)
                {
                    if (werknemer.IsUitvaartverzorger == true)
                    {
                        WerknemerOverzicht.Add(new WerknemersModel
                        {
                            Id = werknemer.Id,
                            VolledigeNaam = werknemer.VolledigeNaam
                        });
                    }
                }
            }

        }
        public void RequestEmployeeUrnSieradenData(Guid SelectedUrnSieradenEmployeeId)
        {
            WerknemerUrnSieradenOverzicht.Clear();
            foreach (var urnEmployee in searchRepository.GetOverlijdenUrnSieradenByEmployee(SelectedUrnSieradenEmployeeId))
            {
                if (urnEmployee != null)
                {
                    WerknemerUrnSieradenOverzicht.Add(new OverledeneUrnSieradenModel
                    {
                        UitvaartNummer = urnEmployee.UitvaartNummer,
                        UrnOpdracht = urnEmployee.UrnOpdracht,
                        UrnText = urnEmployee.UrnText,
                        UrnBedrag = urnEmployee.UrnBedrag,
                        UrnWerknemer = urnEmployee.UrnWerknemer,
                        UrnLeverancierName = urnEmployee.UrnLeverancierName
                    });
                }
            }
        }
        public void RequestEmployeeSteenhouwerData(Guid SelectedSteenhouwerEmployeeId)
        {
            WerknemerSteenhouwerOverzicht.Clear();

            foreach (var steenEmployee in searchRepository.GetOverlijdenSteenhouwerijByEmployee(SelectedSteenhouwerEmployeeId))
            {
                if (steenEmployee != null)
                {
                    WerknemerSteenhouwerOverzicht.Add(new OverledeneSteenhouwerijModel
                    {
                        UitvaartNummer = steenEmployee.UitvaartNummer,
                        SteenhouwerOpdracht = steenEmployee.SteenhouwerOpdracht,
                        SteenhouwerBedrag = steenEmployee.SteenhouwerBedrag,
                        SteenhouwerWerknemer = steenEmployee.SteenhouwerWerknemer,
                        SteenhouwerLeverancierName = steenEmployee.SteenhouwerLeverancierName
                    });
                }
            }
        }
        public void RequestEmployeeBloemenData(Guid SelectedBloemEmployeeId)
        {
            WerknemerBloemOverzicht.Clear();

            foreach (var bloemEmployee in searchRepository.GetOverlijdenBloemenByEmployee(SelectedBloemEmployeeId))
            {
                if (bloemEmployee != null)
                {
                    WerknemerBloemOverzicht.Add(new OverledeneBloemenModel
                    {
                        UitvaartNummer = bloemEmployee.UitvaartNummer,
                        BloemenBedrag = bloemEmployee.BloemenBedrag,
                        BloemenLeverancier = bloemEmployee.BloemenLeverancier,
                        BloemenWerknemer = bloemEmployee.BloemenWerknemer
                    });
                }
            }
        }
        public void CreateNewDossier()
        {
            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            SteenhouwerijModel = new OverledeneSteenhouwerijModel();
            BloemenModel = new OverledeneBloemenModel();
            WerkbonModel = new OverledeneWerkbonUitvaart();
            UrnSieradenModel = new OverledeneUrnSieradenModel();
            Klanttevredenheid = new Klanttevredenheid();
            Dossier = new OverledeneBijlagesModel();
            WerkbonnenList.Clear();
            WerknemerBloemOverzicht.Clear();
            WerknemerSteenhouwerOverzicht.Clear();
            WerknemerUrnSieradenOverzicht.Clear();

            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;
            InfoUitvaartleider.UitvaartId = Globals.UitvaartCodeGuid;
        }
        public static OverledeneSteenhouwerijViewModel SteenhouwerijInstance { get; } = new();
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            SteenhouwerijModel = new OverledeneSteenhouwerijModel();
            BloemenModel = new OverledeneBloemenModel();
            WerkbonModel = new OverledeneWerkbonUitvaart();
            UrnSieradenModel = new OverledeneUrnSieradenModel();
            Klanttevredenheid = new Klanttevredenheid();
            Dossier = new OverledeneBijlagesModel();
            var werkbon = new Werkbonnen();
            WerkbonnenList.Clear();
            WerknemerBloemOverzicht.Clear();
            WerknemerSteenhouwerOverzicht.Clear();
            WerknemerUrnSieradenOverzicht.Clear();
            IsLintChecked = false;
            Lint1 = string.Empty;
            Lint2 = string.Empty;
            Lint3 = string.Empty;
            Lint4 = string.Empty;

            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;

            var klantCijfer = miscellaneousRepository.GetScore(Globals.UitvaartCodeGuid);
            if (klantCijfer != null)
            {
                Klanttevredenheid.Id = klantCijfer.Id;
                Klanttevredenheid.CijferScore = klantCijfer.CijferScore;
                Klanttevredenheid.UitvaartId = klantCijfer.UitvaartId;
            }

            var dossierStatus = miscellaneousRepository.GetFinishedDossier(Globals.UitvaartCodeGuid);
            if (dossierStatus.BijlageId != Guid.Empty)
            {
                Dossier.BijlageId = dossierStatus.BijlageId;
                Dossier.UitvaartId = dossierStatus.UitvaartId;
                Dossier.DocumentUrl = dossierStatus.DocumentUrl;
                DossierTagContent = "Dossier Openen";
            }


            var steenhouwerijResult = searchRepository.GetOverlijdenSteenhouwerijByUitvaartId(uitvaartNummer);
            if (steenhouwerijResult != null)
            {
                SteenhouwerijModel.SteenhouwerijId = steenhouwerijResult.SteenhouwerijId;
                SteenhouwerijModel.UitvaartId = steenhouwerijResult.UitvaartId;
                SteenhouwerijModel.SteenhouwerOpdracht = steenhouwerijResult.SteenhouwerOpdracht;
                SteenhouwerijModel.SteenhouwerBedrag = steenhouwerijResult.SteenhouwerBedrag;
                SteenhouwerijModel.SteenhouwerProvisie = steenhouwerijResult.SteenhouwerProvisie;
                SteenhouwerijModel.SteenhouwerUitbetaing = steenhouwerijResult.SteenhouwerUitbetaing;
                SteenhouwerijModel.SteenhouwerText = steenhouwerijResult.SteenhouwerText;
                SteenhouwerijModel.SteenhouwerLeverancier = steenhouwerijResult.SteenhouwerLeverancier;
                SteenhouwerijModel.SteenhouwerPaid = steenhouwerijResult.SteenhouwerPaid;

                _originalSteenhouwerijModel = new OverledeneSteenhouwerijModel
                {
                    SteenhouwerijId = SteenhouwerijModel.SteenhouwerijId,
                    UitvaartId = SteenhouwerijModel.UitvaartId,
                    SteenhouwerOpdracht = SteenhouwerijModel.SteenhouwerOpdracht,
                    SteenhouwerBedrag = SteenhouwerijModel.SteenhouwerBedrag,
                    SteenhouwerProvisie = SteenhouwerijModel.SteenhouwerProvisie,
                    SteenhouwerUitbetaing = SteenhouwerijModel.SteenhouwerUitbetaing,
                    SteenhouwerText = SteenhouwerijModel.SteenhouwerText,
                    SteenhouwerLeverancier = SteenhouwerijModel.SteenhouwerLeverancier,
                    SteenhouwerPaid = SteenhouwerijModel.SteenhouwerPaid
                };
            }

            var bloemenResult = searchRepository.GetOverlijdenBloemenByUitvaartId(uitvaartNummer);
            if (bloemenResult != null)
            {
                BloemenModel.BloemenId = bloemenResult.BloemenId;
                BloemenModel.UitvaartId = bloemenResult.UitvaartId;
                BloemenModel.BloemenText = bloemenResult.BloemenText;
                BloemenModel.BloemenLint = bloemenResult.BloemenLint;
                IsLintChecked = (bool)bloemenResult.BloemenLint;
                BloemenModel.BloemenKaart = bloemenResult.BloemenKaart;
                BloemenModel.BloemenBedrag = bloemenResult.BloemenBedrag;
                BloemenModel.BloemenProvisie = bloemenResult.BloemenProvisie;
                BloemenModel.BloemenUitbetaling = bloemenResult.BloemenUitbetaling;
                BloemenModel.BloemenLeverancier = bloemenResult.BloemenLeverancier;
                BloemenModel.BloemenPaid = bloemenResult.BloemenPaid;
                BloemenModel.BloemenDocument = bloemenResult.BloemenDocument;
                BloemenModel.BloemenLintJson = bloemenResult.BloemenLintJson;
                BloemenModel.BloemenBezorgAdres = bloemenResult.BloemenBezorgAdres;
                BloemenModel.BloemenBezorgDate = bloemenResult.BloemenBezorgDate;

                _originalBloemenModel = new OverledeneBloemenModel
                {
                    BloemenId = BloemenModel.BloemenId,
                    UitvaartId = BloemenModel.UitvaartId,
                    BloemenText = BloemenModel.BloemenText,
                    BloemenLint = BloemenModel.BloemenLint,
                    BloemenKaart = BloemenModel.BloemenKaart,
                    BloemenBedrag = BloemenModel.BloemenBedrag,
                    BloemenProvisie = BloemenModel.BloemenProvisie,
                    BloemenUitbetaling = BloemenModel.BloemenUitbetaling,
                    BloemenLeverancier = BloemenModel.BloemenLeverancier,
                    BloemenPaid = BloemenModel.BloemenPaid,
                    BloemenDocument = BloemenModel.BloemenDocument,
                    BloemenLintJson = BloemenModel.BloemenLintJson,
                    BloemenBezorgAdres = BloemenModel.BloemenBezorgAdres,
                    BloemenBezorgDate = BloemenModel.BloemenBezorgDate
                };
            }

            if (!string.IsNullOrEmpty(BloemenModel.BloemenLintJson))
            {
                var lintTexts = JsonConvert.DeserializeObject<List<string>>(BloemenModel.BloemenLintJson);
                if (lintTexts != null)
                {
                    Lint1 = lintTexts.ElementAtOrDefault(0) ?? string.Empty;
                    Lint2 = lintTexts.ElementAtOrDefault(1) ?? string.Empty;
                    Lint3 = lintTexts.ElementAtOrDefault(2) ?? string.Empty;
                    Lint4 = lintTexts.ElementAtOrDefault(3) ?? string.Empty;
                }
            }

            var werkbonResult = searchRepository.GetOverlijdenWerkbonnenByUitvaartId(uitvaartNummer);
            if (werkbonResult != null)
            {
                WerkbonModel.Id = werkbonResult.Id;
                WerkbonModel.UitvaartId = werkbonResult.UitvaartId;
                WerkbonModel.UitvaartNummer = werkbonResult.UitvaartNummer;
                WerkbonModel.WerkbonJson = werkbonResult.WerkbonJson;

                _originalWerkbonModel = new OverledeneWerkbonUitvaart
                {
                    Id = WerkbonModel.Id,
                    UitvaartId = WerkbonModel.UitvaartId,
                    UitvaartNummer = WerkbonModel.UitvaartNummer,
                    WerkbonJson = WerkbonModel.WerkbonJson
                };
            }

            var urnSieradenResult = searchRepository.GetOverlijdenUrnSieradenByUitvaartId(uitvaartNummer);
            if (urnSieradenResult != null)
            {
                UrnSieradenModel.UrnId = urnSieradenResult.UrnId;
                UrnSieradenModel.UitvaartId = urnSieradenResult.UitvaartId;
                UrnSieradenModel.UrnOpdracht = urnSieradenResult.UrnOpdracht;
                UrnSieradenModel.UrnBedrag = urnSieradenResult.UrnBedrag;
                UrnSieradenModel.UrnText = urnSieradenResult.UrnText;
                UrnSieradenModel.UrnLeverancier = urnSieradenResult.UrnLeverancier;
                UrnSieradenModel.UrnPaid = urnSieradenResult.UrnPaid;

                _orginalUrnsieradenModel = new OverledeneUrnSieradenModel
                {
                    UrnId = UrnSieradenModel.UrnId,
                    UitvaartId = UrnSieradenModel.UitvaartId,
                    UrnOpdracht = UrnSieradenModel.UrnOpdracht,
                    UrnBedrag = UrnSieradenModel.UrnBedrag,
                    UrnText = UrnSieradenModel.UrnText,
                    UrnLeverancier = UrnSieradenModel.UrnLeverancier,
                    UrnPaid = UrnSieradenModel.UrnPaid
                };
            }

            if (!string.IsNullOrEmpty(WerkbonModel.WerkbonJson))
            {
                WerkbonPrintButtonVisable = Visibility.Visible;
                WerkbonnenList.Add(werkbon);

                foreach (var werkbonElement in JsonConvert.DeserializeObject<List<WerkbonnenData>>(WerkbonModel.WerkbonJson))
                {
                    werkbon.WerkbonData.Add(new WerkbonnenData
                    {
                        UitvaartNummer = werkbonElement.UitvaartNummer,
                        WerknemerId = werkbonElement.WerknemerId,
                        RouwAuto = werkbonElement.RouwAuto,
                        RouwDienaar = werkbonElement.RouwDienaar,
                        LaatsteVerzorging = werkbonElement.LaatsteVerzorging,
                        VolgAuto = werkbonElement.VolgAuto,
                        Overbrengen = werkbonElement.Overbrengen,
                        Condoleance = werkbonElement.Condoleance,
                        Overig = werkbonElement.Overig,
                        WerknemerName = werkbonElement.WerknemerName
                    });
                }
                while (werkbon.WerkbonData.Count < 3)
                    werkbon.WerkbonData.Add(new WerkbonnenData());
            }
            else
            {
                WerkbonnenList.Add(werkbon);
                do
                {
                    werkbon.WerkbonData.Add(new WerkbonnenData());
                } while (werkbon.WerkbonData.Count < 3);
            }
        }
        public bool CanExecuteSaveCommand(object obj)
        {
            return true;
        }
        public void ExecuteSaveCommand(object obj)
        {
            if (CanExecuteSaveSteenCommand(obj))
                ExecuteSaveSteenCommand(obj);

            if (CanExecuteSaveBloemCommand(obj))
                ExecuteSaveBloemCommand(obj);

            if (CanExecuteSaveWerkbonCommand(obj))
                ExecuteSaveWerkbonCommand(obj);

            if (CanExecuteSaveUrnSieradenCommand(obj))
                ExecuteSaveUrnSieradenCommand(obj);

            if (obj != null && (obj.ToString() == "SaveButton" || obj.ToString() == "AfrondenButton"))
            {
                Globals.Voorregeling = false;
                Globals.UitvaartCode = string.Empty;
                Globals.UitvaartCodeGuid = Guid.Empty;
                Globals.UitvaarLeider = string.Empty;

                IntAggregator.Transmit(0);
            }
        }
        public bool CanExecuteSaveSteenCommand(object obj)
        {
            if (SteenhouwerijModel.HasData())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ExecuteSaveSteenCommand(object obj)
        {

            if (!SteenhouwerijModel.HasData())
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            bool SteenExists = miscellaneousRepository.UitvaarSteenhouwerijExists(SteenhouwerijModel.UitvaartId);

            if (SteenhouwerijModel.SteenhouwerijId == Guid.Empty && !SteenExists)
            {
                SteenhouwerijModel.SteenhouwerijId = Guid.NewGuid();
                SteenhouwerijModel.UitvaartId = Globals.UitvaartCodeGuid;

                try
                {
                    createRepository.AddSteenhouwerij(SteenhouwerijModel);
                    new ToastWindow("Steenhouwerij is opgeslagen.").Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Inserting Steenhouwer: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (SteenExists)
            {
                bool steenhouwerjInfoChanged = modelCompare.AreValuesEqual(_originalSteenhouwerijModel, SteenhouwerijModel);

                if (steenhouwerjInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditSteenhouwerij(SteenhouwerijModel);
                        new ToastWindow("Steenhouwerij is geupdate.").Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating Steenhouwer: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalSteenhouwerijModel = new OverledeneSteenhouwerijModel
                    {
                        SteenhouwerijId = SteenhouwerijModel.SteenhouwerijId,
                        UitvaartId = SteenhouwerijModel.UitvaartId,
                        SteenhouwerOpdracht = SteenhouwerijModel.SteenhouwerOpdracht,
                        SteenhouwerBedrag = SteenhouwerijModel.SteenhouwerBedrag,
                        SteenhouwerProvisie = SteenhouwerijModel.SteenhouwerProvisie,
                        SteenhouwerUitbetaing = SteenhouwerijModel.SteenhouwerUitbetaing,
                        SteenhouwerText = SteenhouwerijModel.SteenhouwerText
                    };
                }
            }
        }
        public bool CanExecuteSaveBloemCommand(object obj)
        {
            if (BloemenModel.HasData())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ExecuteSaveBloemCommand(object obj)
        {
            if (!BloemenModel.HasData())
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            var lintValues = new List<string> { Lint1, Lint2, Lint3, Lint4 };
            BloemenModel.BloemenLintJson = JsonConvert.SerializeObject(lintValues);


            bool BloemenExists = miscellaneousRepository.UitvaarBloemenExists(BloemenModel.UitvaartId);

            if (BloemenModel.BloemenId == Guid.Empty && !BloemenExists)
            {
                BloemenModel.BloemenId = Guid.NewGuid();
                BloemenModel.UitvaartId = Globals.UitvaartCodeGuid;

                try
                {
                    createRepository.AddBloemen(BloemenModel);
                    new ToastWindow("Bloemen zijn opgeslagen.").Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error insert bloemen: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (BloemenExists)
            {
                bool bloemenInfoChanged = modelCompare.AreValuesEqual(_originalBloemenModel, BloemenModel);
                if (bloemenInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditBloemen(BloemenModel);
                        new ToastWindow("Bloemen zijn geupdate.").Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating bloemen: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalBloemenModel = new OverledeneBloemenModel
                    {
                        BloemenId = BloemenModel.BloemenId,
                        UitvaartId = BloemenModel.UitvaartId,
                        BloemenText = BloemenModel.BloemenText,
                        BloemenLint = BloemenModel.BloemenLint,
                        BloemenKaart = BloemenModel.BloemenKaart,
                        BloemenBedrag = BloemenModel.BloemenBedrag,
                        BloemenProvisie = BloemenModel.BloemenProvisie,
                        BloemenUitbetaling = BloemenModel.BloemenUitbetaling,
                        BloemenLintJson = BloemenModel.BloemenLintJson
                    };
                }
            }
        }
        public bool CanExecuteSaveWerkbonCommand(object obj)
        {
            if (!string.IsNullOrEmpty(WerkbonModel.WerkbonJson))
                WerkbonPrintButtonVisable = Visibility.Visible;

            List<WerkbonnenData> allWerkbonData = [];

            foreach (var werkbon in WerkbonnenList)
            {
                var filteredData = werkbon.WerkbonData
                    .Where(data => data.WerknemerId != Guid.Empty)
                    .ToList();

                foreach (var data in filteredData)
                {
                    data.UitvaartNummer = Globals.UitvaartCode;

                    var werknemer = WerkbonnenPersonen.FirstOrDefault(p => p.Id == data.WerknemerId);
                    if (werknemer != null)
                        data.WerknemerName = werknemer.WerkbonPersoon;
                }

                allWerkbonData.AddRange(filteredData);
            }
            WerkbonModel.WerkbonJson = JsonConvert.SerializeObject(allWerkbonData, Formatting.Indented);

            return WerkbonModel.HasData();
        }
        public bool CanExecuteSaveUrnSieradenCommand(object obj)
        {
            if (UrnSieradenModel.HasData())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ExecuteSaveUrnSieradenCommand(object obj)
        {
            if (!UrnSieradenModel.HasData())
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            bool UrnSieradenExists = miscellaneousRepository.UitvaarKUrnSieradenExists(UrnSieradenModel.UitvaartId);

            if (UrnSieradenModel.UrnId == Guid.Empty && !UrnSieradenExists)
            {
                UrnSieradenModel.UrnId = Guid.NewGuid();
                UrnSieradenModel.UitvaartId = Globals.UitvaartCodeGuid;

                try
                {
                    createRepository.AddUrnSieraden(UrnSieradenModel);
                    new ToastWindow("Urn & Gedenksieraden zijn opgeslagen.").Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Inserting UrnSieraden: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (UrnSieradenExists)
            {

                bool urnSieradenInfoChanged = modelCompare.AreValuesEqual(_orginalUrnsieradenModel, UrnSieradenModel);
                if (urnSieradenInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditUrnSieraden(UrnSieradenModel);
                        new ToastWindow("Urn & Gedenksieraden zijn geupdate.").Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating UrnSieraden: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                    _orginalUrnsieradenModel = new OverledeneUrnSieradenModel
                    {
                        UrnId = UrnSieradenModel.UrnId,
                        UitvaartId = UrnSieradenModel.UitvaartId,
                        UrnOpdracht = UrnSieradenModel.UrnOpdracht,
                        UrnBedrag = UrnSieradenModel.UrnBedrag,
                        UrnProvisie = UrnSieradenModel.UrnProvisie,
                        UrnUitbetaing = UrnSieradenModel.UrnUitbetaing,
                        UrnText = UrnSieradenModel.UrnText
                    };
                }
            }

        }
        public void ExecuteSaveWerkbonCommand(object obj)
        {
            WerkbonModel.UitvaartId = Globals.UitvaartCodeGuid;

            if (!WerkbonModel.HasData())
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            bool WekbonExists = miscellaneousRepository.UitvaarKWerkbonExists(WerkbonModel.UitvaartId);
            if (WerkbonModel.Id == Guid.Empty && !WekbonExists)
            {
                WerkbonModel.Id = Guid.NewGuid();

                try
                {
                    createRepository.AddWerkbonnen(WerkbonModel);
                    new ToastWindow("Werkbonnen zijn opgeslagen.").Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Inserting Werkbonnen: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (WekbonExists)
            {
                bool werkbonInfoChanged = modelCompare.AreValuesEqual(_originalWerkbonModel, WerkbonModel);
                if (werkbonInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditWerkbonnen(WerkbonModel);
                        new ToastWindow("Werkbonnen zijn geupdate.").Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating Werkbonnen: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalWerkbonModel = new OverledeneWerkbonUitvaart
                    {
                        Id = WerkbonModel.Id,
                        UitvaartId = WerkbonModel.UitvaartId,
                        UitvaartNummer = WerkbonModel.UitvaartNummer,
                        WerkbonJson = WerkbonModel.WerkbonJson
                    };
                }
            }
        }
        public void ExecuteFinishCommand(object obj)
        {
            if (Klanttevredenheid.CijferScore == 0)
            {
                MessageBox.Show("Er is geen klant tevredenheids scrore opgegeven, je kunt het dossier niet afronden zonder.", "Geen Klant Tevredenheids Score", MessageBoxButton.OK);
                return;
            }

            bool KlanttevredenheidExists = miscellaneousRepository.UitvaarFactuurExists(Klanttevredenheid.UitvaartId);

            if (Klanttevredenheid.Id == Guid.Empty && !KlanttevredenheidExists)
            {
                Klanttevredenheid.Id = Guid.NewGuid();
                Klanttevredenheid.UitvaartId = Globals.UitvaartCodeGuid;

                try
                {
                    createRepository.AddKlanttevredenheid(Klanttevredenheid);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Inserting klanttevredenheid: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (KlanttevredenheidExists)
            {
                try
                {
                    updateRepository.EditKlanttevredenheid(Klanttevredenheid);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating klanttevredenheid: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }

            try
            {
                deleteAndActivateDisableOperations.CloseDossier(Globals.UitvaartCodeGuid);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Dossier closing failed: {ex.Message}", "Closing Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }

            try
            {
                ExecuteSaveCommand(obj);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error last save command: {ex.Message}", "Closing Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return;
            }

        }
        private void ExecutePreviousCommand(object obj)
        {
            bool steenhouwerjInfoChanged = modelCompare.AreValuesEqual(_originalSteenhouwerijModel, SteenhouwerijModel);
            bool bloemenInfoChanged = modelCompare.AreValuesEqual(_originalBloemenModel, BloemenModel);
            bool werkbonInfoChanged = modelCompare.AreValuesEqual(_originalWerkbonModel, WerkbonModel);

            if (SteenhouwerijModel.HasData() || BloemenModel.HasData() || WerkbonModel.HasData())
            {
                if (!steenhouwerjInfoChanged || !bloemenInfoChanged || !werkbonInfoChanged)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Onopgeslagen informatie", "Je hebt onopgeslagen informatie!", "Als je nu teruggaat dan verlies je de niet opgelsagen informatie.", "Begrepen", "Blijven");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IntAggregator.Transmit(7);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        return;
                    }
                }
                else
                {
                    IntAggregator.Transmit(7);
                }
            }
            else
            {
                IntAggregator.Transmit(7);
            }
        }
        private void ExecuteUploadDossierCommand(object obj)
        {
            if (!string.IsNullOrEmpty(Dossier.DocumentUrl))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Dossier.DocumentUrl,
                    UseShellExecute = true
                });
            }
            else
            {
                var opslagLocatie = DataProvider.DocumentenOpslag;
                if (opslagLocatie != null)
                {
                    string destinationFolder = Path.Combine(opslagLocatie, Globals.UitvaartCode);

                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "PDF Files|*.pdf",
                        Title = "Selecteer dossier bestand."
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        string selectedFilePath = openFileDialog.FileName;
                        string destinationFilePath = Path.Combine(destinationFolder, Path.GetFileName(selectedFilePath));
                        File.Copy(selectedFilePath, destinationFilePath, true);

                        Dossier.BijlageId = Guid.NewGuid();
                        Dossier.UitvaartId = Globals.UitvaartCodeGuid;
                        Dossier.DocumentName = "Dossier";
                        Dossier.DocumentType = "PDF";
                        Dossier.DocumentUrl = destinationFilePath;
                        Dossier.DocumentHash = Checksum.GetMD5Checksum(selectedFilePath);
                        Dossier.DocumentInconsistent = false;
                        Dossier.IsDeleted = false;

                        try
                        {
                            createRepository.InsertDossier(Dossier);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error Inserting dossier: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                            return;
                        }

                        DossierTagContent = "Dossier Openen";
                        Dossier.DocumentUrl = destinationFilePath;
                    }
                }
            }
        }
        private async Task CreateWerkbonnenFileAsync(object obj)
        {
            SqlConnection conn = new(DataProvider.ConnectionString);
            _generatingDocumentView.Show();

            string sourceLoc = DataProvider.DocumentenOpslag;
            string templateLoc = DataProvider.TemplateFolder;

            static string EnsureTrailingSlash(string path) => path.EndsWith(@"\") ? path : path + @"\";

            sourceLoc = EnsureTrailingSlash(sourceLoc);
            templateLoc = EnsureTrailingSlash(templateLoc);

            string fileToCopy = templateLoc + "Werkbonnen.docx";
            string destinationLoc = EnsureTrailingSlash(sourceLoc + Globals.UitvaartCode);

            FileInfo sourceFile = new(fileToCopy);

            string destinationFile = destinationLoc + "Werkbonnen_" + Globals.UitvaartCode + ".docx";

            if (sourceFile.Exists)
            {
                if (!Directory.Exists(destinationLoc))
                    Directory.CreateDirectory(destinationLoc);

                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);

                sourceFile.CopyTo(destinationFile);
                await FillWerkbonnenFile(destinationFile);
                _generatingDocumentView.Hide();
            }
            _generatingDocumentView.Hide();
            Process wordbonProcess = new Process();
            wordbonProcess.StartInfo.FileName = destinationFile;
            wordbonProcess.StartInfo.UseShellExecute = true;
            wordbonProcess.Start();
            return;
        }
        private async Task FillWerkbonnenFile(string destinationFile)
        {
            SqlConnection conn = new(DataProvider.ConnectionString);
            string UitvaartType = string.Empty;
            DateTime UitvaartDatumTijd = DateTime.MinValue;
            DateTime OverledenDatumTijd = DateTime.MinValue;
            string UitvaartLocatie = string.Empty;
            string DienstLocatie = string.Empty;
            string Overledene = string.Empty;

            conn.Open();
            SqlDataAdapter da = new("  SELECT uitvaartInfoType,uitvaartInfoDatumTijdUitvaart,OOI.overledenDatumTijd,uitvaartInfoUitvaartLocatie,uitvaartInfoDienstLocatie, " +
                                        "CASE WHEN OPG.overledeneTussenvoegsel IS NULL THEN " +
                                        "CONCAT(TRIM(OPG.overledeneAanhef), ' ', TRIM(OPG.overledeneVoornamen), ' ', TRIM(OPG.overledeneAchternaam)) " +
                                        "ELSE " +
                                        "CONCAT(OPG.overledeneAanhef, ' ', TRIM(OPG.overledeneVoornamen), ' ', TRIM(OPG.overledeneTussenvoegsel), ' ', TRIM(OPG.overledeneAchternaam)) " +
                                        "END AS Overledene " +
                                        "FROM OverledeneUitvaartInfo OUI " +
                                        "INNER JOIN OverledeneOverlijdenInfo OOI ON OUI.uitvaartId = OOI.UitvaartId " +
                                        "INNER JOIN OverledenePersoonsGegevens OPG ON OUI.uitvaartId = OPG.uitvaartId " +
                                        "WHERE OUI.UitvaartId = '" + Globals.UitvaartCodeGuid + "'", conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "Werkbonnen");

            if (ds.Tables[0].Rows.Count > 0)
            {
                UitvaartType = ds.Tables[0].Rows[0]["uitvaartInfoType"].ToString();
                UitvaartDatumTijd = (DateTime)ds.Tables[0].Rows[0]["uitvaartInfoDatumTijdUitvaart"];
                OverledenDatumTijd = (DateTime)ds.Tables[0].Rows[0]["overledenDatumTijd"];
                UitvaartLocatie = ds.Tables[0].Rows[0]["uitvaartInfoUitvaartLocatie"].ToString();
                DienstLocatie = ds.Tables[0].Rows[0]["uitvaartInfoDienstLocatie"].ToString();
                Overledene = ds.Tables[0].Rows[0]["Overledene"].ToString();
            }

            conn.Close();

            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            Document doc = app.Documents.Open(destinationFile);

            string UitvaartDT = UitvaartDatumTijd.ToString("dd-MM-yyyy");
            string OverledenDT = OverledenDatumTijd.ToString("dd-MM-yyyy");

            List<WerkbonnenData> werkbonList = JsonConvert.DeserializeObject<List<WerkbonnenData>>(WerkbonModel.WerkbonJson);

            Dictionary<string, string> bookmarks = new Dictionary<string, string>();

            for (int i = 0; i < werkbonList.Count && i < 3; i++)
            {
                string indexSuffix = (i == 0) ? "" : (i + 1).ToString();

                bookmarks[$"uitvaartTe{indexSuffix}"] = UitvaartLocatie;
                bookmarks[$"dienstTe{indexSuffix}"] = DienstLocatie;
                bookmarks[$"datumUitvaart{indexSuffix}"] = UitvaartDT;
                bookmarks[$"datumOverlijden{indexSuffix}"] = OverledenDT;
                bookmarks[$"uitvaartType{indexSuffix}"] = UitvaartType;
                bookmarks[$"naamOverledene{indexSuffix}"] = Overledene;
            }

            for (int i = 0; i < werkbonList.Count && i < 3; i++)
            {
                var werkbon = werkbonList[i];

                // Populate personnel names correctly
                bookmarks[$"naamPersoneel{(i == 0 ? "" : (i + 1).ToString())}"] = werkbon.WerknemerName ?? "Onbekend";

                // Populate other details with correct naming conventions
                bookmarks[$"overigeDiensten{(i == 0 ? "" : (i + 1).ToString())}"] = werkbon.Overig ?? string.Empty;
                bookmarks[$"uitvaartNummer{(i == 0 ? "" : (i + 1).ToString())}"] = werkbon.UitvaartNummer ?? "Onbekend";

                // Handle checkbox values specifically for each werkbon
                bookmarks[$"cbRouwauto{(i == 0 ? "" : (i + 1).ToString())}"] = werkbon.RouwAuto ? "Ja" : "";
                bookmarks[$"cbVolgauto{(i == 0 ? "" : (i + 1).ToString())}"] = werkbon.VolgAuto ? "Ja" : "";
                bookmarks[$"cbVerzorging{(i == 0 ? "" : (i + 1).ToString())}"] = werkbon.LaatsteVerzorging ? "Ja" : "";
                bookmarks[$"cbOverbrengen{(i == 0 ? "" : (i + 1).ToString())}"] = werkbon.Overbrengen ? "Ja" : "";
                bookmarks[$"cbCondoleance{(i == 0 ? "" : (i + 1).ToString())}"] = werkbon.Condoleance ? "Ja" : "";
            }


            foreach (var bookmark in bookmarks)
            {
                if (doc.Bookmarks.Exists(bookmark.Key))
                    doc.Bookmarks[bookmark.Key].Range.Text = bookmark.Value;
            }

            doc.Save();
            doc.Close();
            app.Quit();
        }
        private async Task CreateDocumentBestelBloemen(object obj)
        {
            if (SaveBloemCommand.CanExecute(obj))
                SaveBloemCommand.Execute(obj);

            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
            string destinationFile = string.Empty;
            bool initialCreation = false;
            Guid documentId = Guid.Empty;

            if (string.IsNullOrWhiteSpace(BloemenModel.BloemenDocument))
            {
                destinationFile = await OverledeneBijlagesViewModel.BijlagesInstance.CreateDirectory(Globals.UitvaartCode, "Aanvraag.Bloemen.docx");//.ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var bloemenDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "Bloemen").ConfigureAwait(false);
                documentId = bloemenDocument.BijlageId;

                if (File.Exists(BloemenModel.BloemenDocument))
                {

                    System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
                    MessageBoxResult openExisting = MessageBox.Show("Wil je de bestaande openen?", "Bestaande openen", MessageBoxButton.YesNo);
                    if (openExisting == MessageBoxResult.Yes)
                    {
                        Process wordProcess = new Process();
                        wordProcess.StartInfo.FileName = BloemenModel.BloemenDocument;
                        wordProcess.StartInfo.UseShellExecute = true;
                        wordProcess.Start();
                        return;
                    }
                    else
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
                        File.Delete(BloemenModel.BloemenDocument);
                        BloemenModel.BloemenDocumentUpdated = true;
                    }
                }
                destinationFile = await OverledeneBijlagesViewModel.BijlagesInstance.CreateDirectory(Globals.UitvaartCode, "Aanvraag.Bloemen.docx").ConfigureAwait(true);

            }

            BloemenDocModel = await miscellaneousRepository.GetDocumentBloemenInfoAsync(Globals.UitvaartCodeGuid);
            BloemenDocModel.DocumentId = documentId;
            BloemenDocModel.DestinationFile = destinationFile;
            BloemenDocModel.UitvaartId = Globals.UitvaartCodeGuid;

            if (!BloemenDocModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
                }
            }
            OverledeneBijlagesModel docResults = await documentGenerator.UpdateBloemen(BloemenDocModel).ConfigureAwait(true);

            if (docResults != null)
            {
                docResults.DocumentInconsistent = false;
                docResults.IsDeleted = false;
                docResults.DocumentType = "Word";
                docResults.DocumentName = "Bloemen";
                docResults.DocumentUrl = BloemenDocModel.DestinationFile;
                docResults.DocumentHash = Checksum.GetMD5Checksum(BloemenDocModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(docResults);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error inserting documentinfo: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        await updateRepository.UpdateDocumentInfoAsync(docResults);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating documentinfo: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() =>{ _generatingDocumentView.Hide(); });
            Process.Start(new ProcessStartInfo
            {
                FileName = BloemenDocModel.DestinationFile,
                UseShellExecute = true
            });
            return;
        }
    }
    public class Werkbonnen : ViewModelBase
    {
        public ObservableCollection<WerkbonnenData> WerkbonData { get; }
        public ObservableCollection<WerkbonPersoneel> WerkbonnenPersonen { get; }
        public ObservableCollection<LeveranciersModel> LeveranciersSteen { get; }
        public ObservableCollection<LeveranciersModel> LeveranciersBloem { get; }
        public ObservableCollection<LeveranciersModel> LeveranciersUrnSieraden { get; }
        public ObservableCollection<WerknemersModel> WerknemerOverzicht { get; }

        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;

        public Werkbonnen()
        {
            WerkbonData = new ObservableCollection<WerkbonnenData>();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            WerkbonnenPersonen = new ObservableCollection<WerkbonPersoneel>();
            LeveranciersSteen = new ObservableCollection<LeveranciersModel>();
            LeveranciersBloem = new ObservableCollection<LeveranciersModel>();
            WerknemerOverzicht = new ObservableCollection<WerknemersModel>();
            LeveranciersUrnSieraden = new ObservableCollection<LeveranciersModel>();

            WerkbonnenPersonen.Clear();
            LeveranciersSteen.Clear();
            LeveranciersBloem.Clear();
            WerknemerOverzicht.Clear();
            LeveranciersUrnSieraden.Clear();

            foreach (var el in miscellaneousRepository.GetWerkbonPersoneel())
            {
                if (!el.IsDeleted)
                    WerkbonnenPersonen.Add(new WerkbonPersoneel { Id = el.Id, WerkbonPersoon = el.WerkbonPersoon });
            }

            foreach (var leverancier in miscellaneousRepository.GetLeveranciers())
            {
                if (!leverancier.IsDeleted)
                {
                    if (leverancier.Steenhouwer)
                    {
                        LeveranciersSteen.Add(new LeveranciersModel
                        {
                            LeverancierId = leverancier.LeverancierId,
                            LeverancierName = leverancier.LeverancierName,
                            LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                        });
                    }
                    if (leverancier.Bloemist)
                    {
                        LeveranciersBloem.Add(new LeveranciersModel
                        {
                            LeverancierId = leverancier.LeverancierId,
                            LeverancierName = leverancier.LeverancierName,
                            LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                        });
                    }
                    if (leverancier.UrnSieraden)
                    {
                        LeveranciersUrnSieraden.Add(new LeveranciersModel
                        {
                            LeverancierId = leverancier.LeverancierId,
                            LeverancierName = leverancier.LeverancierName,
                            LeverancierBeschrijving = leverancier.LeverancierBeschrijving
                        });
                    }
                }
            }

            foreach (var werknemer in miscellaneousRepository.GetWerknemers())
            {
                if ((!werknemer.IsDeleted) && werknemer.IsUitvaartverzorger)
                {
                    WerknemerOverzicht.Add(new WerknemersModel
                    {
                        Id = werknemer.Id,
                        VolledigeNaam = werknemer.VolledigeNaam
                    });
                }
            }

        }
    }
}
