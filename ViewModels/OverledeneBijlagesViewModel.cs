using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using Dossier_Registratie.Interfaces;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using static Dossier_Registratie.ViewModels.OverledeneFactuurViewModel;
using static Dossier_Registratie.ViewModels.OverledeneSteenhouwerijViewModel;
using Range = Microsoft.Office.Interop.Word.Range;
using Task = System.Threading.Tasks.Task;
using Exception = System.Exception;

namespace Dossier_Registratie.ViewModels
{
    [SupportedOSPlatform("windows")]
    public class AllChosenVerzekering
    {
        public string? VerzekeringName { get; set; }
        public ObservableCollection<PolisVerzekering>? PolisInfoList { get; set; }
    }
    public class OverledeneBijlagesViewModel : ViewModelBase
    {
        private readonly IGeneratingDocumentWindow generatingView;
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations deleteRepository;
        private readonly DocumentGenerator documentGenerator = new();

        private OverledeneUitvaartleiderModel _uitvaartLeiderModel;
        private OverledeneBijlagesModel _bijlageModel;
        private OverledeneBijlagesModel _verlofDossier;
        private ObservableCollection<PolisVerzekering> _verzekeringen;
        private PolisVerzekering _selectedVerzekering;
        private ObservableCollection<PolisVerzekering> _verzekeringenWithAll;
        private ObservableCollection<OverledeneBijlagesModel> _originalBijlageList;
        private OverledeneBijlagesModel _originelBijlageModel;
        private BijlageTagModel _tagModel;
        private BijlageButtonModel _buttonModel;
        private KoffieKamerDocument _koffieKamerModel;
        private BijlageContentModel _buttonContentModel;
        private DocumentDocument _documentModel;
        private DienstDocument _dienstAanvraagModel;
        private ChecklistDocument _checklistModel;
        private OverdrachtDocument _overdrachtModel;
        private BezittingenDocument _bezittingenModel;
        private CrematieDocument _crematieModel;
        private FactuurInfoCrematie _factuurCrematieModel;
        private BegrafenisDocument _begrafenisModel;
        private TerugmeldingDocument _terugmeldingModel;
        private TevredenheidDocument _tevredenheidModel;
        private AangifteDocument _aangifteModel;

        public bool initialLoadDone;
        private bool _correctAccessOrNotCompleted = true;
        private bool _isBegrafenis = false;
        private bool _isCrematie = false;
        private string _documentOption;
        private string _fileExists;
        private string _fileGenerate;
        private string _verlofTagContent;
        string destinationFile = string.Empty;

        public bool CorrectAccessOrNotCompleted
        {
            get { return _correctAccessOrNotCompleted; }
            set
            {
                _correctAccessOrNotCompleted = value;
                OnPropertyChanged(nameof(CorrectAccessOrNotCompleted));
            }
        }
        public bool IsBegrafenis
        {
            get { return _isBegrafenis; }
            set
            {
                _isBegrafenis = value;
                OnPropertyChanged(nameof(IsBegrafenis));
            }
        }
        public bool IsCrematie
        {
            get { return _isCrematie; }
            set
            {
                _isCrematie = value;
                OnPropertyChanged(nameof(IsCrematie));
            }
        }
        public string DocumentOption
        {
            get { return _documentOption; }
            set { _documentOption = value; OnPropertyChanged(nameof(DocumentOption)); }
        }
        public string FileExists
        {
            get { return _fileExists; }
            set { _fileExists = value; OnPropertyChanged(nameof(FileExists)); }
        }
        public string FileGenerate
        {
            get { return _fileGenerate; }
            set { _fileGenerate = value; OnPropertyChanged(nameof(FileGenerate)); }
        }
        public string VerlofTagContent
        {
            get { return _verlofTagContent; }
            set
            {
                if (_verlofTagContent != value)
                {
                    _verlofTagContent = value;
                    OnPropertyChanged(nameof(VerlofTagContent));
                }
            }
        }
        public ObservableCollection<PolisVerzekering> VerzekeringenWithAll
        {
            get { return _verzekeringenWithAll; }
            set { _verzekeringenWithAll = value; OnPropertyChanged(nameof(VerzekeringenWithAll)); }
        }
        public ObservableCollection<PolisVerzekering> Verzekeringen
        {
            get { return _verzekeringen; }
            set { _verzekeringen = value; OnPropertyChanged(nameof(Verzekeringen)); }
        }
        public PolisVerzekering SelectedVerzekering
        {
            get { return _selectedVerzekering; }
            set
            {
                if (_selectedVerzekering != value)
                {
                    _selectedVerzekering = value;
                    OnPropertyChanged(nameof(SelectedVerzekering));
                }
            }
        }
        public OverledeneUitvaartleiderModel InfoUitvaartleider
        {
            get { return _uitvaartLeiderModel; }
            set { _uitvaartLeiderModel = value; OnPropertyChanged(nameof(InfoUitvaartleider)); }
        }
        public OverledeneBijlagesModel BijlageModel
        {
            get { return _bijlageModel; }
            set { _bijlageModel = value; OnPropertyChanged(nameof(BijlageModel)); }
        }
        public OverledeneBijlagesModel VerlofDossier
        {
            get { return _verlofDossier; }
            set { _verlofDossier = value; OnPropertyChanged(nameof(VerlofDossier)); }
        }
        public BijlageTagModel TagModel
        {
            get { return _tagModel; }
            set { _tagModel = value; OnPropertyChanged(nameof(TagModel)); }
        }
        public BijlageButtonModel ButtonModel
        {
            get { return _buttonModel; }
            set { _buttonModel = value; OnPropertyChanged(nameof(ButtonModel)); }
        }
        public BijlageContentModel ButtonContentModel
        {
            get { return _buttonContentModel; }
            set { _buttonContentModel = value; OnPropertyChanged(nameof(ButtonContentModel)); }
        }
        public KoffieKamerDocument KoffieKamerModel
        {
            get { return _koffieKamerModel; }
            set { _koffieKamerModel = value; OnPropertyChanged(nameof(KoffieKamerModel)); }
        }
        public DocumentDocument DocumentModel
        {
            get { return _documentModel; }
            set { _documentModel = value; OnPropertyChanged(nameof(DocumentModel)); }
        }
        public DienstDocument DienstAanvraagModel
        {
            get { return _dienstAanvraagModel; }
            set { _dienstAanvraagModel = value; OnPropertyChanged(nameof(DienstAanvraagModel)); }
        }
        public ChecklistDocument ChecklistModel
        {
            get { return _checklistModel; }
            set { _checklistModel = value; OnPropertyChanged(nameof(ChecklistModel)); }
        }
        public OverdrachtDocument OverdrachtModel
        {
            get { return _overdrachtModel; }
            set { _overdrachtModel = value; OnPropertyChanged(nameof(OverdrachtModel)); }
        }
        public BezittingenDocument BezittingenModel
        {
            get { return _bezittingenModel; }
            set { _bezittingenModel = value; OnPropertyChanged(nameof(BezittingenModel)); }
        }
        public CrematieDocument CrematieModel
        {
            get { return _crematieModel; }
            set { _crematieModel = value; OnPropertyChanged(nameof(CrematieModel)); }
        }
        public FactuurInfoCrematie FactuurCreatieModel
        {
            get { return _factuurCrematieModel; }
            set { _factuurCrematieModel = value; OnPropertyChanged(nameof(FactuurInfoCrematie)); }
        }
        public BegrafenisDocument BegrafenisModel
        {
            get { return _begrafenisModel; }
            set { _begrafenisModel = value; OnPropertyChanged(nameof(BegrafenisModel)); }
        }
        public TerugmeldingDocument TerugmeldingModel
        {
            get { return _terugmeldingModel; }
            set { _terugmeldingModel = value; OnPropertyChanged(nameof(TerugmeldingModel)); }
        }
        public TevredenheidDocument TevredenheidModel
        {
            get { return _tevredenheidModel; }
            set { _tevredenheidModel = value; OnPropertyChanged(nameof(TevredenheidModel)); }
        }
        public AangifteDocument AangifteModel
        {
            get { return _aangifteModel; }
            set { _aangifteModel = value; OnPropertyChanged(nameof(AangifteModel)); }
        }
        public ObservableCollection<OverledeneBijlagesModel> BijlageList { get; }
        public ICommand SaveCommand { get; }
        public ICommand OpenPopupCommand { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand OpenKostenbegrotingCommand { get; }
        public ICommand CreateDocumentAkteVanCessieCommand { get; }
        public ICommand CreateDocumentKoffieCommand { get; }
        public ICommand DocumentDocumentCommand { get; }
        public ICommand CreateDocumentDocumentCommand { get; }
        public ICommand CreateDocumentDienstCommand { get; }
        public ICommand CreateDocumentChecklistCommand { get; }
        public ICommand CreateDocumentOverdrachtCommand { get; set; }
        public ICommand CreateDocumentOpdrachtBegrafenisCommand { get; set; }
        public ICommand CreateDocumentOpdrachtCrematieCommand { get; set; }
        public ICommand CreateDocumentTerugmeldingCommand { get; set; }
        public ICommand CreateDocumentTevredenheidCommand { get; set; }
        public ICommand CreateDocumentBezittingenCommand { get; set; }
        public ICommand CreateDocumentAangifteCommand { get; set; }
        public ICommand VerlofUploadenCommand { get; }
        public event EventHandler DataLoaded;
        protected virtual void OnDataLoaded()
        {
            DataLoaded?.Invoke(this, EventArgs.Empty);
        }
        private OverledeneBijlagesViewModel()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            generatingView = new GeneratingDocumentView();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            deleteRepository = new DeleteAndActivateDisableOperations();

            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            BijlageModel = new OverledeneBijlagesModel();
            VerlofDossier = new OverledeneBijlagesModel();
            BijlageList = new ObservableCollection<OverledeneBijlagesModel>();
            _originalBijlageList = new ObservableCollection<OverledeneBijlagesModel>();
            TagModel = new BijlageTagModel();
            ButtonModel = new BijlageButtonModel();
            ButtonContentModel = new BijlageContentModel();
            KoffieKamerModel = new KoffieKamerDocument();
            DocumentModel = new DocumentDocument();
            DienstAanvraagModel = new DienstDocument();
            ChecklistModel = new ChecklistDocument();
            OverdrachtModel = new OverdrachtDocument();
            BezittingenModel = new BezittingenDocument();
            CrematieModel = new CrematieDocument();
            FactuurCreatieModel = new FactuurInfoCrematie();
            BegrafenisModel = new BegrafenisDocument();
            TerugmeldingModel = new TerugmeldingDocument();
            TevredenheidModel = new TevredenheidDocument();
            AangifteModel = new AangifteDocument();
            VerlofTagContent = "Verlof uploaden";

            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
            OpenPopupCommand = new ViewModelCommand(ExecuteOpenPopupCommand);
            ClosePopupCommand = new ViewModelCommand(ExecuteClosePopupCommand);
            OpenKostenbegrotingCommand = new ViewModelCommand(ExecuteKostenbegrotingCommand);
            VerlofUploadenCommand = new ViewModelCommand(ExecuteVerlofUploadenCommand);
            CreateDocumentAkteVanCessieCommand = new ViewModelCommand(async (parameter) => await CreateDocumentAkteVanCessie(parameter));
            CreateDocumentKoffieCommand = new ViewModelCommand(async (parameter) => await CreateDocumentKoffie(parameter));
            CreateDocumentDocumentCommand = new ViewModelCommand(async (parameter) => await CreateDocumentDocument(parameter));
            CreateDocumentDienstCommand = new ViewModelCommand(async (parameter) => await CreateDocumentDienst(parameter));
            CreateDocumentChecklistCommand = new ViewModelCommand(async (parameter) => await CreateDocumentChecklist(parameter));
            CreateDocumentOverdrachtCommand = new ViewModelCommand(async (parameter) => await CreateDocumentOverdracht(parameter));
            CreateDocumentOpdrachtBegrafenisCommand = new ViewModelCommand(async (parameter) => await CreateDocumentBegrafenis(parameter));
            CreateDocumentOpdrachtCrematieCommand = new ViewModelCommand(async (parameter) => await CreateDocumentCrematie(parameter));
            CreateDocumentTerugmeldingCommand = new ViewModelCommand(async (parameter) => await CreateDocumentTerugmelding(parameter));
            CreateDocumentTevredenheidCommand = new ViewModelCommand(async (parameter) => await CreateDocumentTevredenheid(parameter));
            CreateDocumentBezittingenCommand = new ViewModelCommand(async (parameter) => await CreateDocumentBezittingen(parameter));
            CreateDocumentAangifteCommand = new ViewModelCommand(async (parameter) => await CreateDocumentAangifte(parameter));
        }
        public void CreateNewDossier()
        {
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            BijlageModel = new OverledeneBijlagesModel();
            TagModel = new BijlageTagModel();
            ButtonModel = new BijlageButtonModel();
            ButtonContentModel = new BijlageContentModel();
            KoffieKamerModel = new KoffieKamerDocument();
            DocumentModel = new DocumentDocument();
            DienstAanvraagModel = new DienstDocument();
            ChecklistModel = new ChecklistDocument();
            OverdrachtModel = new OverdrachtDocument();
            BezittingenModel = new BezittingenDocument();
            CrematieModel = new CrematieDocument();
            BegrafenisModel = new BegrafenisDocument();
            TerugmeldingModel = new TerugmeldingDocument();
            TevredenheidModel = new TevredenheidDocument();
            AangifteModel = new AangifteDocument();

            BijlageList.Clear();
            _originalBijlageList.Clear();

            IsBegrafenis = Globals.UitvaartType == "Begrafenis";
            IsCrematie = Globals.UitvaartType == "Crematie";
        }
        public static OverledeneBijlagesViewModel BijlagesInstance { get; } = new();
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            BijlageModel = new OverledeneBijlagesModel();
            TagModel = new BijlageTagModel();
            ButtonModel = new BijlageButtonModel();
            ButtonContentModel = new BijlageContentModel();
            KoffieKamerModel = new KoffieKamerDocument();
            DocumentModel = new DocumentDocument();
            DienstAanvraagModel = new DienstDocument();
            ChecklistModel = new ChecklistDocument();
            OverdrachtModel = new OverdrachtDocument();
            BezittingenModel = new BezittingenDocument();
            CrematieModel = new CrematieDocument();
            BegrafenisModel = new BegrafenisDocument();
            TerugmeldingModel = new TerugmeldingDocument();
            TevredenheidModel = new TevredenheidDocument();
            AangifteModel = new AangifteDocument();

            BijlageList.Clear();
            _originalBijlageList.Clear();

            if (string.IsNullOrEmpty(Globals.UitvaartType))
                Globals.UitvaartType = miscellaneousRepository.UitvaartType(Globals.UitvaartCodeGuid);

            IsBegrafenis = Globals.UitvaartType == "Begrafenis";
            IsCrematie = Globals.UitvaartType == "Crematie";

            var dossierStatus = miscellaneousRepository.GetVerlofDossier(Globals.UitvaartCodeGuid);
            if (dossierStatus.BijlageId != Guid.Empty)
            {
                VerlofDossier.BijlageId = dossierStatus.BijlageId;
                VerlofDossier.UitvaartId = dossierStatus.UitvaartId;
                VerlofDossier.DocumentUrl = dossierStatus.DocumentUrl;
                VerlofTagContent = "Verlof openen";
            }

            var UitvaarLeiderResult = searchRepository.GetUitvaarleiderByUitvaartId(uitvaartNummer);
            if (UitvaarLeiderResult != null)
            {
                InfoUitvaartleider.Uitvaartnummer = UitvaarLeiderResult.Uitvaartnummer;
                InfoUitvaartleider.PersoneelNaam = UitvaarLeiderResult.PersoneelNaam;

                Globals.UitvaartCode = UitvaarLeiderResult.Uitvaartnummer;
                Globals.UitvaartCodeGuid = UitvaarLeiderResult.UitvaartId;
                Globals.UitvaarLeider = UitvaarLeiderResult.PersoneelNaam;
            }


            foreach (var bijlage in searchRepository.GetOverlijdenBijlagesByUitvaartId(uitvaartNummer))
            {
                if (!bijlage.IsDeleted)
                {
                    BijlageList.Add(BijlageModel = new OverledeneBijlagesModel
                    {
                        BijlageId = bijlage.BijlageId,
                        UitvaartId = bijlage.UitvaartId,
                        DocumentName = bijlage.DocumentName,
                        DocumentType = bijlage.DocumentType,
                        DocumentUrl = bijlage.DocumentUrl,
                        DocumentHash = bijlage.DocumentHash,
                        DocumentInconsistent = bijlage.DocumentInconsistent
                    });

                    _originalBijlageList.Add(_originelBijlageModel = new OverledeneBijlagesModel
                    {
                        BijlageId = BijlageModel.BijlageId,
                        UitvaartId = BijlageModel.UitvaartId,
                        DocumentName = BijlageModel.DocumentName,
                        DocumentType = BijlageModel.DocumentType,
                        DocumentUrl = BijlageModel.DocumentUrl,
                        DocumentHash = BijlageModel.DocumentHash,
                        DocumentInconsistent = BijlageModel.DocumentInconsistent
                    });
                }
            }
            var jsonDeserializer = new JsonDeserializer();

            Verzekeringen = new ObservableCollection<PolisVerzekering>(jsonDeserializer.DeserializeJson(uitvaartNummer));

            if (Verzekeringen.Count > 1)
            {
                VerzekeringenWithAll = new ObservableCollection<PolisVerzekering>(new[] { new PolisVerzekering { VerzekeringName = "Alles" } }.Concat(Verzekeringen));
            }
            else
            {
                VerzekeringenWithAll = new ObservableCollection<PolisVerzekering>(Verzekeringen);
            }
            SelectedVerzekering = VerzekeringenWithAll.FirstOrDefault();

            initialLoadDone = true;
            OnDataLoaded();
        }
        public bool CanExecuteSaveCommand(object obj)
        {
            return true;
        }
        public void ExecuteSaveCommand(object obj)
        {
            if (obj != null && obj.ToString() == "VolgendeButton")
            {
                SteenhouwerijInstance.RequestedDossierInformationBasedOnUitvaartId(Globals.UitvaartCode);
                IntAggregator.Transmit(8);
            }
        }
        public void ExecuteOpenPopupCommand(object obj)
        {
            var myPopup = obj as Popup;
            if (myPopup != null)
            {
                myPopup.IsOpen = !myPopup.IsOpen;
            }
        }
        public void ExecuteVerlofUploadenCommand(object obj)
        {
            if (!string.IsNullOrEmpty(VerlofDossier.DocumentUrl))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = VerlofDossier.DocumentUrl,
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
                        Title = "Selecteer verlof bestand."
                    };

                    if (openFileDialog.ShowDialog() == true)
                    {
                        string selectedFilePath = openFileDialog.FileName;
                        string destinationFilePath = Path.Combine(destinationFolder, Path.GetFileName(selectedFilePath));
                        File.Copy(selectedFilePath, destinationFilePath, true);

                        VerlofDossier.BijlageId = Guid.NewGuid();
                        VerlofDossier.UitvaartId = Globals.UitvaartCodeGuid;
                        VerlofDossier.DocumentName = "Verlof";
                        VerlofDossier.DocumentType = "PDF";
                        VerlofDossier.DocumentUrl = destinationFilePath;
                        VerlofDossier.DocumentHash = Checksum.GetMD5Checksum(selectedFilePath);
                        VerlofDossier.DocumentInconsistent = false;
                        VerlofDossier.IsDeleted = false;

                        try
                        {
                            createRepository.InsertDossier(VerlofDossier);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show($"Error Inserting verlof dossier: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                            return;
                        }

                        VerlofTagContent = "Verlof Openen";
                        VerlofDossier.DocumentUrl = destinationFilePath;
                    }
                }
            }
        }
        public void ExecuteClosePopupCommand(object obj)
        {
            if (obj is Popup myPopup)
            {
                myPopup.IsOpen = false;
            }
        }
        private void ExecutePreviousCommand(object obj)
        {
            IntAggregator.Transmit(6);
        }       
        private void ExecuteKostenbegrotingCommand(object obj)
        {
            KostenbegrotingInstance.RequestedDossierInformationBasedOnUitvaartId(Globals.UitvaartCode);
            IntAggregator.Transmit(11);
        }
        public async Task CreateDocumentAkteVanCessie(object obj)
        {
            if (SelectedVerzekering == null)
                return;

            AkteDocument akteDocument = await searchRepository.GetAkteContentByUitvaartIdAsync(Globals.UitvaartCodeGuid);
            if (akteDocument == null || string.IsNullOrWhiteSpace(akteDocument.VerzekeringInfo))
            {
                MessageBox.Show("Géén verzekering data gevonden.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var akteVerzekeringen = JsonConvert.DeserializeObject<List<PolisVerzekering>>(akteDocument.VerzekeringInfo);
            if (akteVerzekeringen == null || akteVerzekeringen.Count == 0)
            {
                MessageBox.Show("Géén polis informatie gevonden.","Data Error",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            var documentsInfo = akteVerzekeringen
                .Where(v => !string.IsNullOrEmpty(v.VerzekeringName))
                .Select(v => (
                    fileName: $"AkteVanCessie_{v.VerzekeringName}.docx",
                    documentType: "AktevanCessie",
                    tag: ""
                ))
                .ToList();

            if (documentsInfo.Count == 0)
            {
                MessageBox.Show("Géén valide verzekeringen gevonden voor document generatie.","Géén Documents",MessageBoxButton.OK,MessageBoxImage.Information);
                return;
            }

            await CreateMultipleDocuments<AkteDocument, AkteDocument?>(
                documentOption: obj.ToString(),
                documentsInfo: documentsInfo,
                templateFile: "Akte.van.Cessie.docx",
                fileType: "Word",
                fetchModel: async (guid) => akteDocument,
                generateDocument: async (model, additionalData) =>
                {
                    var bijlageModels = await documentGenerator.UpdateAkte(model);
                    return bijlageModels;
                },
                prepareAdditionalData: () => Task.FromResult<AkteDocument?>(null),
                createRepository: createRepository,
                updateRepository: updateRepository,
                deleteRepository: deleteRepository,
                updateTagModels: (docResults) =>{},
                generatingWindow: generatingView
            );
        }
        public async Task CreateDocumentChecklist(object obj)
        {
            await CreateDocument<ChecklistDocument, ChecklistDocument?>(
                obj.ToString(),
                "Checklist.docx",
                "Checklist",
                "Word",
                TagModel.ChecklistTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDocumentChecklistInfoAsync(guid),
                generateDocument: async (model, additionalData) =>
                {
                    model.UitvaartNummer = Globals.UitvaartCode;
                    model.UitvartLeider = Globals.UitvaarLeider;
                    var werknemers = GetWerknemersFromModel(model.OpbarenInfo);
                    return await documentGenerator.UpdateChecklist(model, werknemers);
                },
                prepareAdditionalData: () => Task.FromResult<ChecklistDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.ChecklistTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentDienst(object obj)
        {
            await CreateDocument<DienstDocument, DienstDocument?>(
                obj.ToString(),
                "Aanvraag.Dienst.docx",
                "AanvraagDienst",
                "Word",
                TagModel.AanvraagDienstTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDienstInfoAsync(guid),
                generateDocument: async (model, additionalData) => await documentGenerator.UpdateDienst(model),
                prepareAdditionalData: () => Task.FromResult<DienstDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.AanvraagDienstTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentDocument(object obj)
        {
            await CreateDocument<DocumentDocument, DocumentDocument?>(
                obj.ToString(),
                "Document.docx",
                "Document",
                "Word",
                TagModel.DocumentTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDocumentDocumentInfoAsync(guid),
                generateDocument: async (model, additionalData) => await documentGenerator.UpdateDocument(model),
                prepareAdditionalData: () => Task.FromResult<DocumentDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.DocumentTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentKoffie(object obj)
        {
            await CreateDocument<KoffieKamerDocument, KoffieKamerDocument?>(
                obj.ToString(),
                "Aanvraag.Koffiekamer.docx",
                "Koffiekamer",
                "Word",
                TagModel.KoffiekamerTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetKoffieKamerInfoAsync(guid),
                generateDocument: async (model, additionalData) => await documentGenerator.UpdateKoffie(model),
                prepareAdditionalData: () => Task.FromResult<KoffieKamerDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.KoffiekamerTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentBezittingen(object obj)
        {
            await CreateDocument<BezittingenDocument, BezittingenDocument?>(
                obj.ToString(),
                "Bezittingen.docx",
                "Bezittingen",
                "Word",
                TagModel.BezittingenTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDocumentBezittingInfoAsync(guid),
                generateDocument: async (model, additionalData) => await documentGenerator.UpdateBezittingen(model),
                prepareAdditionalData: () => Task.FromResult<BezittingenDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.BezittingenTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentCrematie(object obj)
        {
            await CreateDocument<CrematieDocument, FactuurInfoCrematie?>(
                documentOption: obj.ToString(),
                fileName: "Opdracht.Crematie.docx",
                documentType: "OpdrachtCrematie",
                fileType: "Word",
                tag: TagModel.OpdrachtCrematieTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDocumentCrematieInfoAsync(Globals.UitvaartCodeGuid),
                generateDocument: async (model, additionalData) =>
                {
                    var crematieDocument = model;

                    FactuurInfoCrematie? additionalDataResult = null;
                    if (crematieDocument.Herkomst != Guid.Empty)
                        additionalDataResult = await miscellaneousRepository.GetFactuurInfo(crematieDocument.Herkomst);

                    return await documentGenerator.UpdateCrematie(crematieDocument, additionalDataResult);
                },
                prepareAdditionalData: async () => null,
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.OpdrachtCrematieTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentBegrafenis(object obj)
        {
            await CreateDocument<BegrafenisDocument, BegrafenisDocument?>(
                obj.ToString(),
                "Opdracht.Begrafenis.docx",
                "OpdrachtBegrafenis",
                "Word",
                TagModel.OpdrachtBegrafenisTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDocumentBegrafenisInfoAsync(guid),
                generateDocument: async (model, additionalData) => await documentGenerator.UpdateBegrafenis(model),
                prepareAdditionalData: () => Task.FromResult<BegrafenisDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.OpdrachtBegrafenisTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentTerugmelding(object obj)
        {
            var terugmeldingData = await miscellaneousRepository.GetDocumentTerugmeldingInfoAsync(Globals.UitvaartCodeGuid);
            if (terugmeldingData == null || string.IsNullOrWhiteSpace(terugmeldingData.Polisnummer))
            {
                MessageBox.Show("Géén terugmelding data gevonden.","Data Error",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            var verzekeringen = JsonConvert.DeserializeObject<List<PolisVerzekering>>(terugmeldingData.Polisnummer);
            if (verzekeringen == null || verzekeringen.Count == 0)
            {
                MessageBox.Show("Géén verzekering data gevonden in terugmelding.","Data Error",MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            var documentsInfo = verzekeringen
                .Where(v => !string.IsNullOrEmpty(v.VerzekeringName))
                .Select(v => (
                    fileName: $"{v.VerzekeringName}_Terugmelding.docx",
                    documentType: "Terugmelding",
                    tag: TagModel.TerugmeldingTag
                ))
                .ToList();

            if (documentsInfo.Count == 0)
            {
                MessageBox.Show(
                    "No valid verzekeringen found for document generation.",
                    "No Documents",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            await CreateMultipleDocuments<TerugmeldingDocument, TerugmeldingDocument?>(
                documentOption: obj.ToString(),
                documentsInfo: documentsInfo,
                templateFile: "Terugmelding.docx",
                fileType: "Word",
                fetchModel: async (guid) => terugmeldingData,
                generateDocument: async (model, additionalData) =>
                {
                    var bijlageModels = await documentGenerator.UpdateTerugmelding(model);
                    return bijlageModels;
                },
                prepareAdditionalData: () => Task.FromResult<TerugmeldingDocument?>(null),
                createRepository: createRepository,
                updateRepository: updateRepository,
                deleteRepository: deleteRepository,
                updateTagModels: (docResults) =>
                {
                    if (docResults.Any())
                        TagModel.TerugmeldingTag = "Alles";
                },
                generatingWindow: generatingView
            );
        }
        public async Task CreateDocumentTevredenheid(object obj)
        {
            await CreateDocument<TevredenheidDocument, TevredenheidDocument?>(
                obj.ToString(),
                "Tevredenheidsonderzoek.docx",
                "Tevredenheid",
                "Word",
                TagModel.TevredenheidTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDocumentTevredenheidsInfoAsync(guid),
                generateDocument: async (model, additionalData) => await documentGenerator.UpdateTevredenheid(model),
                prepareAdditionalData: () => Task.FromResult<TevredenheidDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.TevredenheidTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentAangifte(object obj)
        {
            await CreateDocument<AangifteDocument, AangifteDocument?>(
                obj.ToString(),
                "Aangifte.docx",
                "Aangifte",
                "Word",
                TagModel.AangifteTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDocumentAangifteInfoAsync(guid),
                generateDocument: async (model, additionalData) => await documentGenerator.UpdateAangifte(model),
                prepareAdditionalData: () => Task.FromResult<AangifteDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.AangifteTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        public async Task CreateDocumentOverdracht(object obj)
        {
            await CreateDocument<OverdrachtDocument, OverdrachtDocument?>(
                obj.ToString(),
                "Overdracht.docx",
                "Overdracht",
                "Word",
                TagModel.OverdrachtTag,
                fetchModel: async (guid) => await miscellaneousRepository.GetDocumentOverdrachtInfoAsync(guid),
                generateDocument: async (model, additionalData) => await documentGenerator.UpdateOverdracht(model),
                prepareAdditionalData: () => Task.FromResult<OverdrachtDocument?>(null),
                createRepository,
                updateRepository,
                deleteRepository,
                updateTagModel: (docResults) => { TagModel.OverdrachtTag = docResults.DocumentUrl; },
                generatingView
            );
        }
        private List<ChecklistOpbarenDocument> GetWerknemersFromModel(string opbarenInfoJson)
        {
            if (string.IsNullOrEmpty(opbarenInfoJson))
                return new List<ChecklistOpbarenDocument>();

            var werknemers = JsonConvert.DeserializeObject<List<ChecklistOpbarenDocument>>(opbarenInfoJson);

            foreach (var werknemer in werknemers)
            {
                var employee = miscellaneousRepository.GetWerknemer(werknemer.WerknemerId);
                if (employee != null)
                {
                    werknemer.WerknemerName = string.IsNullOrEmpty(employee.Tussenvoegsel)
                        ? $"{employee.Initialen} {employee.Achternaam}"
                        : $"{employee.Initialen} {employee.Tussenvoegsel} {employee.Achternaam}";
                }
            }

            return werknemers;
        }

        public static async Task<(string destinationFile, Guid documentId, bool initialCreation)> InitializeDocument(
            string tag,
            string fileName,
            string documentType,
            string documentOption,
            IDeleteAndActivateDisableOperations deleteRepository
        )
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                string destinationUrl = await DocumentFunctions.CreateDirectory(Globals.UitvaartCode, fileName).ConfigureAwait(true);
                return (destinationUrl, Guid.NewGuid(), true);
            }

            if (!File.Exists(tag) || documentOption == "Opnieuw")
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, documentType);
                string destinationUrl = await DocumentFunctions.CreateDirectory(Globals.UitvaartCode, fileName).ConfigureAwait(true);
                return (destinationUrl, Guid.NewGuid(), true);
            }

            return (tag, Guid.Empty, false);
        }
        public static async Task<(string destinationUrl, bool initialCreation)> HandleDocumentConsistency(
            string tag,
            string documentType,
            string fileName,
            IDeleteAndActivateDisableOperations deleteRepository,
            IMiscellaneousAndDocumentOperations miscellaneousRepository,
            string documentOption,
            bool initialCreation
        )
        {
            if (initialCreation)
                return (string.Empty, true);

            if (string.IsNullOrWhiteSpace(tag))
                return (string.Empty, true);

            if (!File.Exists(tag))
                return (string.Empty, true);

            var documentInfo = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, documentType).ConfigureAwait(true);
            if (documentInfo == null)
                return (string.Empty, true);

            string savedHash = documentInfo.DocumentHash;
            string currentHash = Checksum.GetMD5Checksum(tag);

            if (savedHash != currentHash)
            {
                var result = CustomMessageBox.Show(
                    $"{documentInfo.DocumentName} - {Globals.UitvaartCode}",
                    "Document inconsistent!",
                    "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?",
                    "Openen",
                    "Nieuw"
                );

                if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(tag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(tag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(tag);
                            break;
                    }
                    return (tag, false);
                }
                else if ((result == CustomMessageBox.CustomMessageBoxResult.Stop) && File.Exists(tag))
                {
                    File.Delete(tag);
                    deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, documentType);
                    string destinationUrl = await DocumentFunctions.CreateDirectory(Globals.UitvaartCode, fileName).ConfigureAwait(true);
                    return (destinationUrl, true);
                }
            }

            return (tag, false);
        }
        private static bool HandleEmptyModel(string uitvaartCode)
        {
            var result = MessageBox.Show(
                $"Geen data gevonden voor {uitvaartCode}, leeg formulier maken?",
                "Geen data gevonden",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            return result == MessageBoxResult.Yes;
        }
        private static async Task<bool> SaveOrUpdateDocumentInfo(
            OverledeneBijlagesModel docResults,
            bool isNewDocument,
            ICreateOperations createRepository,
            IUpdateOperations updateRepository,
            string documentType
        )
        {
            try
            {
                docResults.UitvaartId = Globals.UitvaartCodeGuid;
                if (isNewDocument)
                    await createRepository.InsertDocumentInfoAsync(docResults);
                else
                    await updateRepository.UpdateDocumentInfoAsync(docResults);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error while {(isNewDocument ? "saving" : "updating")} the document info for {documentType}: {ex.Message}",
                    $"{(isNewDocument ? "Save" : "Update")} Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return false;
            }
        }
        private static void PerformPostGenerationAction(string documentOption, string documentUrl)
        {
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(documentUrl);
                    break;

                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(documentUrl);
                    break;

                case "Email":
                    DocumentFunctions.EmailFile(documentUrl);
                    break;

                default:
                    MessageBox.Show(
                        $"The action '{documentOption}' is not supported.",
                        "Unsupported Action",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    break;
            }
        }

        public async Task CreateDocument<TModel, TAdditionalData>(
            string documentOption,
            string fileName,
            string documentType,
            string fileType,
            string tag,
            Func<Guid, Task<TModel>> fetchModel,
            Func<TModel, TAdditionalData, Task<OverledeneBijlagesModel>> generateDocument,
            Func<Task<TAdditionalData>> prepareAdditionalData,
            ICreateOperations createRepository,
            IUpdateOperations updateRepository,
            IDeleteAndActivateDisableOperations deleteRepository,
            Action<OverledeneBijlagesModel> updateTagModel,
            IGeneratingDocumentWindow generatingWindow
        ) where TModel : class, IHasData
        {
            var (destinationFile, documentId, initialCreation) = await InitializeDocument(
                tag,
                fileName,
                documentType,
                documentOption,
                deleteRepository
            ).ConfigureAwait(true);

            var (destinationUrl, initialCreationCheck) = await HandleDocumentConsistency(tag,
                documentType,
                fileName,
                deleteRepository,
                miscellaneousRepository,
                documentOption,
                initialCreation).ConfigureAwait(true);

            if (documentOption != "Opnieuw" && !initialCreation && !initialCreationCheck)
            {
                PerformPostGenerationAction(documentOption, tag);
            }else
            {
                generatingWindow.Show();

                var model = await fetchModel(Globals.UitvaartCodeGuid);
                model.DocumentId = documentId == Guid.Empty ? model.DocumentId : documentId;
                model.DestinationFile = destinationFile;
                model.DocumentName = documentType;
                model.FileType = fileType;
                model.UitvaartId = Globals.UitvaartCodeGuid;
                model.Dossiernummer = Globals.UitvaartCode;

                if (!model.HasData())
                {
                    generatingWindow.Hide();
                    if (!HandleEmptyModel(Globals.UitvaartCode))
                        return;

                    generatingWindow.Show();
                }

                var additionalData = await prepareAdditionalData();

                var docResults = await generateDocument(model, additionalData).ConfigureAwait(true);
                docResults.DocumentType = model.FileType;
                docResults.DocumentUrl = model.DestinationFile;
                docResults.UitvaartId = model.UitvaartId;
                docResults.Dossiernummer = model.Dossiernummer;
                if (docResults == null)
                {
                    generatingWindow.Hide();
                    return;
                }

                var success = await SaveOrUpdateDocumentInfo(
                    docResults,
                    initialCreation,
                    createRepository,
                    updateRepository,
                    documentType
                );

                if (!success)
                {
                    generatingWindow.Hide();
                    return;
                }

                updateTagModel(docResults);
                generatingWindow.Hide();
                PerformPostGenerationAction(documentOption, docResults.DocumentUrl);
            }
        }
        public static async Task CreateMultipleDocuments<TModel, TAdditionalData>(
            string documentOption,
            List<(string fileName, string documentType, string tag)> documentsInfo,
            string templateFile,
            string fileType,
            Func<Guid, Task<TModel>> fetchModel,
            Func<TModel, TAdditionalData, Task<OverledeneBijlagesModel>> generateDocument,
            Func<Task<TAdditionalData>> prepareAdditionalData,
            ICreateOperations createRepository,
            IUpdateOperations updateRepository,
            IDeleteAndActivateDisableOperations deleteRepository,
            Action<List<OverledeneBijlagesModel>> updateTagModels,
            IGeneratingDocumentWindow generatingWindow
        ) where TModel : class, IHasData
        {
            var results = new List<OverledeneBijlagesModel>();
            string baseTemplateCopy = string.Empty;

            foreach (var (fileName, documentType, tag) in documentsInfo)
            {
                try
                {
                    var (destinationFile, documentId, initialCreation) = await InitializeDocument(
                        tag,
                        templateFile,
                        documentType,
                        documentOption,
                        deleteRepository
                    ).ConfigureAwait(true);

                    var uniqueDestinationFile = Path.Combine(Path.GetDirectoryName(destinationFile), fileName);

                    if (File.Exists(uniqueDestinationFile))
                        File.Delete(uniqueDestinationFile);

                    baseTemplateCopy = destinationFile;
                    if (!File.Exists(uniqueDestinationFile))
                    {
                        try
                        {
                            File.Copy(destinationFile, uniqueDestinationFile, overwrite: false);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                            await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                            continue;
                        }
                    }



                    if (documentOption != "Opnieuw" && !initialCreation)
                    {
                        PerformPostGenerationAction(documentOption, tag);
                        continue;
                    }

                    generatingWindow.Show();

                    var model = await fetchModel(Globals.UitvaartCodeGuid);
                    model.DocumentId = documentId == Guid.Empty ? model.DocumentId : documentId;
                    model.DestinationFile = uniqueDestinationFile;
                    model.DocumentName = fileName;
                    model.FileType = fileType;
                    model.UitvaartId = Globals.UitvaartCodeGuid;
                    model.Dossiernummer = Globals.UitvaartCode;

                    if (!model.HasData())
                    {
                        generatingWindow.Hide();
                        if (!HandleEmptyModel(Globals.UitvaartCode))
                            continue;

                        generatingWindow.Show();
                    }

                    var additionalData = await prepareAdditionalData();

                    var docResults = await generateDocument(model, additionalData).ConfigureAwait(true);
                    if (docResults == null)
                    {
                        generatingWindow.Hide();
                        continue;
                    }

                    string currentHash = Checksum.GetMD5Checksum(model.DestinationFile);

                    docResults.DocumentType = model.FileType;
                    docResults.DocumentUrl = model.DestinationFile;
                    docResults.UitvaartId = model.UitvaartId;
                    docResults.Dossiernummer = model.Dossiernummer;
                    docResults.DocumentHash = currentHash;
                    docResults.BijlageId = Guid.NewGuid();

                    results.Add(docResults);
                    generatingWindow.Hide();
                    PerformPostGenerationAction(documentOption, docResults.DocumentUrl);
                }
                catch (Exception ex)
                {
                    generatingWindow.Hide();
                    Debug.WriteLine(ex);
                    await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                }
            }

            if (File.Exists(baseTemplateCopy))
                File.Delete(baseTemplateCopy);

            updateTagModels(results);
        }
    }
    public class JsonDeserializer
    {
        private ISearchOperations searchRepository;
        public ObservableCollection<PolisVerzekering> DeserializeJson(string UitvaartNummer)
        {
            searchRepository = new SearchOperations();
            var verzekeringResult = searchRepository.GetOverlijdenVerzekeringByUitvaartId(UitvaartNummer);
            if(verzekeringResult?.VerzekeringProperties != null)
            {
                string jsonString = verzekeringResult.VerzekeringProperties;
                Debug.WriteLine(jsonString);
                var verzekeringen = JsonConvert.DeserializeObject<List<PolisVerzekering>>(jsonString);
                var filteredVerzekeringen = verzekeringen
                    .Where(v => !string.IsNullOrEmpty(v.VerzekeringName) && v.VerzekeringName != "null")
                    .GroupBy(v => v.VerzekeringName)
                    .Select(g => g.First())
                    .ToList();

                return new ObservableCollection<PolisVerzekering>(filteredVerzekeringen);
            }

            return new ObservableCollection<PolisVerzekering>();
        }
    }
}