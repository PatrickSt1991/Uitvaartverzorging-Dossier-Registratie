using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;
using static Dossier_Registratie.ViewModels.OverledeneFactuurViewModel;
using static Dossier_Registratie.ViewModels.OverledeneSteenhouwerijViewModel;
using Range = Microsoft.Office.Interop.Word.Range;
using Task = System.Threading.Tasks.Task;

namespace Dossier_Registratie.ViewModels
{
    [SupportedOSPlatform("windows")]
    public class AllChosenVerzekering
    {
        public string VerzekeringName { get; set; }
        public ObservableCollection<PolisVerzekering> PolisInfoList { get; set; }
    }
    public class OverledeneBijlagesViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations deleteRepository;

        readonly DocumentGenerator documentGenerator = new DocumentGenerator();
        readonly SqlConnection conn = new(DataProvider.ConnectionString);

        private GeneratingDocumentView _generatingDocumentView;
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
        private bool _isBusy;
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
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
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
        public ICommand GenererenAkteVanCessieCommand { get; }
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
            _generatingDocumentView = new GeneratingDocumentView();
            VerlofTagContent = "Verlof uploaden";

            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
            OpenPopupCommand = new ViewModelCommand(ExecuteOpenPopupCommand);
            ClosePopupCommand = new ViewModelCommand(ExecuteClosePopupCommand);
            OpenKostenbegrotingCommand = new ViewModelCommand(ExecuteKostenbegrotingCommand);
            VerlofUploadenCommand = new ViewModelCommand(ExecuteVerlofUploadenCommand);
            GenererenAkteVanCessieCommand = new ViewModelCommand(async (parameter) => await GenererenCreateAkteVanCessie(parameter));
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
        public async Task<string> CreateDirectory(string UitvaartId, string DocumentType)
        {
            var opslagFolder = DataProvider.DocumentenOpslag;
            var templateFolder = DataProvider.TemplateFolder;

            Debug.WriteLine(opslagFolder);
            Debug.WriteLine(templateFolder);

            bool sourceLocEndSlash = opslagFolder.EndsWith(@"\");

            if (!sourceLocEndSlash)
                opslagFolder += "\\";

            string fileToCopy = templateFolder + "\\" + DocumentType;
            string destinationLoc = opslagFolder + UitvaartId;

            destinationFile = destinationLoc + "\\" + DocumentType;
            FileInfo sourceFile = new(fileToCopy);

            if (sourceFile.Exists)
            {
                if (!Directory.Exists(destinationLoc))
                    Directory.CreateDirectory(destinationLoc);

                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);

                sourceFile.CopyTo(destinationFile);
            }
            return destinationFile;
        }
        public async Task<string> CreateDirectoryPolis(string uitvaartId, string documentType, string polisNr)
        {
            var opslagFolder = DataProvider.DocumentenOpslag;
            var templateFolder = DataProvider.TemplateFolder;

            if (!opslagFolder.EndsWith(@"\"))
            {
                opslagFolder += "\\";
            }

            string fileToCopy = Path.Combine(templateFolder, documentType);
            string destinationLoc = Path.Combine(opslagFolder, uitvaartId);
            string destinationFile = Path.Combine(destinationLoc, documentType);

            // Create the new file name with the polis number
            string strippedFileName = Path.GetFileNameWithoutExtension(destinationFile);
            string newPolisFileName = $"{strippedFileName}.{polisNr}.docx";
            string destinationFileWithPolisNr = Path.Combine(destinationLoc, newPolisFileName);

            FileInfo sourceFile = new(fileToCopy);

            if (sourceFile.Exists)
            {
                // Ensure the destination directory exists
                if (!Directory.Exists(destinationLoc))
                {
                    Directory.CreateDirectory(destinationLoc);
                }

                // Delete the destination file if it already exists
                if (File.Exists(destinationFileWithPolisNr))
                {
                    File.Delete(destinationFileWithPolisNr);
                }

                sourceFile.CopyTo(destinationFileWithPolisNr);
            }
            else
            {
                throw new FileNotFoundException("The template file does not exist.", fileToCopy);
            }

            if (File.Exists(destinationFile))
                File.Delete(destinationFile);

            return destinationFileWithPolisNr;
        }
        private void ExecuteKostenbegrotingCommand(object obj)
        {
            KostenbegrotingInstance.RequestedDossierInformationBasedOnUitvaartId(Globals.UitvaartCode);
            IntAggregator.Transmit(11);
        }
        static bool IsSingleValue(string input)
        {
            input = input.Trim();

            if (input.Contains(','))
            {
                return false;
            }

            return true;
        }
        private static string EnsureTrailingSlash(string path) => path.EndsWith("\\") ? path : path + "\\";
        private async Task CreateAkteFile(string UitvaartId, string documentName, bool akteStatus, bool sendMail)
        {
            var outlookApp = sendMail ? new Microsoft.Office.Interop.Outlook.Application() : null;
            var mail = sendMail ? (MailItem)outlookApp.CreateItem(OlItemType.olMailItem) : null;

            string sourceLoc = EnsureTrailingSlash(DataProvider.DocumentenOpslag);
            string templateLoc = EnsureTrailingSlash(DataProvider.TemplateFolder);
            string fileToCopy = Path.Combine(templateLoc, "Akte.van.Cessie.docx");
            string destinationLoc = EnsureTrailingSlash(Path.Combine(sourceLoc, UitvaartId));

            FileInfo sourceFile = new(fileToCopy);

            if (!sourceFile.Exists)
                throw new FileNotFoundException($"Template file not found at {fileToCopy}");

            if (IsSingleValue(documentName))
                await HandleSingleDocument(documentName, sourceFile, destinationLoc, akteStatus, mail, sendMail);
            else
                await HandleMultipleDocuments(documentName, sourceFile, destinationLoc, akteStatus, mail, sendMail);

            if (sendMail)
                FinalizeAndDisplayMail(mail, outlookApp);
            return;
        }
        private async Task HandleSingleDocument(string documentName, FileInfo sourceFile, string destinationLoc, bool akteStatus, MailItem mail, bool sendMail)
        {
            string destinationFile = Path.Combine(destinationLoc, $"AkteVanCessie_{SanitizeFileName(documentName)}.docx");
            await CopyAndProcessFile(sourceFile, destinationFile, SelectedVerzekering.VerzekeringName, SelectedVerzekering.PolisInfoList, akteStatus, mail, sendMail, documentName);
        }
        private async Task HandleMultipleDocuments(string documentNames, FileInfo sourceFile, string destinationLoc, bool akteStatus, MailItem mail, bool sendMail)
        {
            foreach (string document in documentNames.Split(','))
            {
                string trimmedDocument = document.Trim();
                string destinationFile = Path.Combine(destinationLoc, $"AkteVanCessie_{SanitizeFileName(trimmedDocument)}.docx");

                var verzekering = VerzekeringenWithAll.FirstOrDefault(v => v.VerzekeringName == trimmedDocument);
                if (verzekering == null || verzekering.VerzekeringName == "Alles")
                    continue;

                await CopyAndProcessFile(sourceFile, destinationFile, verzekering.VerzekeringName, verzekering.PolisInfoList, akteStatus, mail, sendMail, documentNames);
            }
        }
        private static string SanitizeFileName(string fileName) =>
            fileName.Replace(" ", "").Replace("/", "-");
        private void FinalizeAndDisplayMail(MailItem mail, Microsoft.Office.Interop.Outlook.Application outlookApp)
        {
            mail.Display(true);
            if (mail != null) Marshal.ReleaseComObject(mail);
            if (outlookApp != null) Marshal.ReleaseComObject(outlookApp);
        }
        private async Task CopyAndProcessFile(FileInfo sourceFile, string destinationFile, string verzekeringName, List<Polis> polisInfoList, bool akteStatus, MailItem mail, bool sendMail, string documentName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(destinationFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
            }

            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }

            sourceFile.CopyTo(destinationFile);
            await FillAkteFile(verzekeringName, polisInfoList, destinationFile);

            if (sendMail)
            {
                mail.Attachments.Add(destinationFile, OlAttachmentType.olByValue, 1, Path.GetFileName(destinationFile));
                mail.Subject = $"Akte van Cessie - {documentName}";
            }
            else
            {
                DocumentFunctions.OpenFile(destinationFile);
            }
        }
        private async Task FillAkteFile(string verzekeringName, List<Polis> polisInfoList, string destinationFile)
        {
            AkteContent akteContent = await searchRepository.GetAkteContentByUitvaartIdAsync(Globals.UitvaartCodeGuid);

            var bookmarks = GetBookmarks(akteContent, verzekeringName);
            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            Document doc = app.Documents.Open(destinationFile);

            AddPolisInfoToDocument(doc, polisInfoList, bookmarks);

            await AddHeaderImageToDocument(doc);

            doc.Save();
            doc.Close();
        }
        private Dictionary<string, string> GetBookmarks(AkteContent akteContent, string verzekeringName)
        {
            return new Dictionary<string, string>
    {
        { "verzekeringMaatschappij", verzekeringName },
        { "ondergetekendeNaamEnVoorletters", akteContent.OpdrachtgeverNaam },
        { "ondergetekendeAdres", akteContent.OpdrachtgeverAdres },
        { "ondergetekendeRelatieTotVerzekende", akteContent.OpdrachtgeverRelatie },
        { "verzekerdeGeslotenOpHetLevenVan", akteContent.GeslotenOpHetLevenVan },
        { "verzekerdeGeborenOp", akteContent.OverledenGeboorteDatum.ToString("dd-MM-yyyy") },
        { "verzekerdeOverledenOp", akteContent.OverledenOpDatum.ToString("dd-MM-yyyy") },
        { "verzekerdeAdres", akteContent.OverledenOpAdres },
        { "vermeldingNummer", Globals.UitvaartCode },
        { "PolisTable", "" },
        { "OrganisatieNaam", DataProvider.OrganizationName },
        { "OrganisatieIban", DataProvider.OrganizationIban }
    };
        }
        private void AddPolisInfoToDocument(Document doc, List<Polis> polisInfoList, Dictionary<string, string> bookmarks)
        {
            foreach (var bookmark in bookmarks)
            {
                if (!doc.Bookmarks.Exists(bookmark.Key))
                    throw new InvalidOperationException($"The bookmark '{bookmark.Key}' does not exist in the document.");

                Bookmark bm = doc.Bookmarks[bookmark.Key];
                Range range = bm.Range;

                if (bookmark.Key == "PolisTable")
                    AddPolisTableToDocument(doc, polisInfoList, range);
                else
                    range.Text = bookmark.Value;
            }
        }
        private void AddPolisTableToDocument(Document doc, List<Polis> polisInfoList, Range polisTableRange)
        {
            int polisBedragCount = 2; 
            double totalPolisBedrag = 0;

            foreach (var polisInfo in polisInfoList)
            {
                if (!string.IsNullOrEmpty(polisInfo.PolisBedrag) && double.TryParse(polisInfo.PolisBedrag, out double polisBedragValue))
                {
                    polisBedragCount++;
                    totalPolisBedrag += polisBedragValue;
                }
            }

            Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(polisTableRange, polisBedragCount, 2);

            table.Cell(1, 1).Range.Text = "Polisnummer";
            table.Cell(1, 2).Range.Text = "Verzekerd bedrag";
            table.Rows[1].Range.Font.Bold = 1;
            table.Rows[1].Range.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleDashSmallGap;

            int rowIndex = 2;
            foreach (var polisInfo in polisInfoList)
            {
                if (!string.IsNullOrEmpty(polisInfo.PolisBedrag))
                {
                    table.Cell(rowIndex, 1).Range.Text = polisInfo.PolisNr;
                    table.Cell(rowIndex, 2).Range.Text = "€ " + polisInfo.PolisBedrag;
                    rowIndex++;
                }
            }

            table.Rows[polisBedragCount].Range.Font.Bold = 1;
            table.Rows[polisBedragCount].Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
            table.Cell(polisBedragCount, 1).Range.Text = "Totaal";
            table.Cell(polisBedragCount, 1).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            table.Cell(polisBedragCount, 2).Range.Text = "€ " + totalPolisBedrag.ToString("#,0.00", System.Globalization.CultureInfo.CreateSpecificCulture("nl-NL"));
        }
        private async Task AddHeaderImageToDocument(Document doc)
        {
            var (documentData, documentType) = miscellaneousRepository.GetLogoBlob("Document");
            if (documentData != null && documentData.Length > 0)
            {
                string tempImagePath = string.Empty;

                try
                {
                    tempImagePath = Path.Combine(Path.GetTempPath(), $"headerImage.{documentType}");

                    string tempDir = Path.GetDirectoryName(tempImagePath);
                    if (string.IsNullOrEmpty(tempDir) || !Directory.Exists(tempDir))
                    {
                        throw new DirectoryNotFoundException($"Temporary directory not found: {tempDir}");
                    }

                    File.WriteAllBytes(tempImagePath, documentData);

                    foreach (Section section in doc.Sections)
                    {
                        HeaderFooter header = section.Headers[WdHeaderFooterIndex.wdHeaderFooterPrimary];
                        Range headerRange = header.Range;
                        headerRange.InlineShapes.AddPicture(tempImagePath);
                        headerRange.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    }
                }
                catch (System.Exception ex)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    throw;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(tempImagePath) && File.Exists(tempImagePath))
                    {
                        try
                        {
                            File.Delete(tempImagePath);
                        }
                        catch (System.Exception deleteEx)
                        {
                            ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(deleteEx);
                        }
                    }
                }
            }
        }
        private async Task GenererenCreateAkteVanCessie(object parameter)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });

            bool sendMail = false;
            bool akteStatus = false;

            if (parameter != null && parameter.ToString() == "mail")
                sendMail = true;

            if (SelectedVerzekering != null)
            {
                IsBusy = true;
                if (SelectedVerzekering.VerzekeringName == "Alles")
                {
                    foreach (var multipleAktes in miscellaneousRepository.GetAktesVanCessieByUitvaatId(Globals.UitvaartCode))
                    {
                        if (multipleAktes != null && !multipleAktes.IsDeleted && File.Exists(multipleAktes.DocumentUrl))
                        {
                            File.Delete(multipleAktes.DocumentUrl);
                            akteStatus = true;
                        }
                    }

                    var AlleVerzekeringenElementen = VerzekeringenWithAll
                        .Where(item => item.VerzekeringName != "Alles")
                        .Select(item => item.VerzekeringName);

                    string AlleVerzekeringen = string.Join(",", AlleVerzekeringenElementen);

                    await CreateAkteFile(Globals.UitvaartCode, string.Join(",", AlleVerzekeringen), akteStatus, sendMail);
                    IsBusy = false;
                    ExecuteClosePopupCommand("close");
                }
                else
                {
                    foreach (var singleAkte in miscellaneousRepository.GetAktesVanCessieByUitvaatId(Globals.UitvaartCode))
                    {
                        if (singleAkte != null && !singleAkte.IsDeleted && File.Exists(singleAkte.DocumentUrl) && "AkteVanCessie_" + SelectedVerzekering.VerzekeringName == singleAkte.DocumentName)
                        {
                            File.Delete(singleAkte.DocumentUrl);
                            akteStatus = true;
                        }
                    }

                    await CreateAkteFile(Globals.UitvaartCode, SelectedVerzekering.VerzekeringName, akteStatus, sendMail);
                    IsBusy = false;
                    ExecuteClosePopupCommand("close");
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
        }
        public async Task CreateDocumentOverdracht(object obj)
        {
            string documentOption = obj.ToString();
            string destinationFile = string.Empty;
            Guid documentId;
            bool initialCreation = false;

            if (string.IsNullOrWhiteSpace(TagModel.OverdrachtTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Overdracht.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.OverdrachtTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Overdracht");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Overdracht.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.OverdrachtTag))
                    File.Delete(TagModel.OverdrachtTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Overdracht");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Overdracht.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var overdrachtDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "Overdracht").ConfigureAwait(false);
                documentId = overdrachtDocument.BijlageId;
                string savedHash = overdrachtDocument.DocumentHash;

                string documentHash = Checksum.GetMD5Checksum(TagModel.OverdrachtTag);

                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{overdrachtDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.OverdrachtTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.OverdrachtTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.OverdrachtTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.OverdrachtTag))
                        {
                            File.Delete(TagModel.OverdrachtTag);
                            OverdrachtModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Overdracht.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(TagModel.OverdrachtTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.OverdrachtTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.OverdrachtTag);
                            break;
                    }
                    return;
                }
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });

            OverdrachtModel = await miscellaneousRepository.GetDocumentOverdrachtInfoAsync(Globals.UitvaartCodeGuid);
            OverdrachtModel.DocumentId = documentId;
            OverdrachtModel.DestinationFile = destinationFile;
            OverdrachtModel.UitvaartId = Globals.UitvaartCodeGuid;
            OverdrachtModel.UitvaartNummer = Globals.UitvaartCode;

            if (!OverdrachtModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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
            OverledeneBijlagesModel docResults = await documentGenerator.UpdateOverdracht(OverdrachtModel).ConfigureAwait(true);

            if (docResults != null)
            {
                docResults.DocumentInconsistent = false;
                docResults.IsDeleted = false;
                docResults.DocumentType = "Word";
                docResults.DocumentName = "Overdracht";
                docResults.DocumentUrl = OverdrachtModel.DestinationFile;
                docResults.DocumentHash = Checksum.GetMD5Checksum(OverdrachtModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(docResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting overdracht docinfo: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating overdracht docinfo: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }
            TagModel.OverdrachtTag = docResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.OverdrachtTag);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(TagModel.OverdrachtTag);
                    break;
                case "Email":
                    DocumentFunctions.EmailFile(TagModel.OverdrachtTag);
                    break;
            }
            return;
        }
        public async Task CreateDocumentChecklist(object obj)
        {
            string documentOption = obj.ToString();
            string destinationFile = string.Empty;
            bool initialCreation = false;
            Guid documentId = Guid.Empty;

            if (string.IsNullOrWhiteSpace(TagModel.ChecklistTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Checklist.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.ChecklistTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Checklist");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Checklist.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.ChecklistTag))
                    File.Delete(TagModel.ChecklistTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Checklist");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Checklist.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var checklistDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "Checklist").ConfigureAwait(false);
                documentId = checklistDocument.BijlageId;

                string savedHash = checklistDocument.DocumentHash;

                string documentHash = Checksum.GetMD5Checksum(TagModel.ChecklistTag);

                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{checklistDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.ChecklistTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.ChecklistTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.ChecklistTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.ChecklistTag))
                        {
                            File.Delete(TagModel.ChecklistTag);
                            ChecklistModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Checklist.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(TagModel.ChecklistTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.ChecklistTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.ChecklistTag);
                            break;
                    }
                    return;
                }
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
            ChecklistModel = await miscellaneousRepository.GetDocumentChecklistInfoAsync(Globals.UitvaartCodeGuid);
            ChecklistModel.DocumentId = documentId;
            ChecklistModel.DestinationFile = destinationFile;
            ChecklistModel.UitvaartNummer = Globals.UitvaartCode;
            ChecklistModel.UitvartLeider = Globals.UitvaarLeider;

            var werknemers = new List<ChecklistOpbarenDocument>();

            if (ChecklistModel.OpbarenInfo != null)
            {
                werknemers = JsonConvert.DeserializeObject<List<ChecklistOpbarenDocument>>(ChecklistModel.OpbarenInfo);

                foreach (var werknemer in werknemers)
                {
                    var searchEmployeeName = miscellaneousRepository.GetWerknemer(werknemer.WerknemerId);

                    if (searchEmployeeName != null)
                    {
                        if (string.IsNullOrEmpty(searchEmployeeName.Tussenvoegsel))
                        {
                            werknemer.WerknemerName = searchEmployeeName.Initialen + ' ' + searchEmployeeName.Achternaam;
                        }
                        else
                        {
                            werknemer.WerknemerName = searchEmployeeName.Initialen + ' ' + searchEmployeeName.Tussenvoegsel + ' ' + searchEmployeeName.Achternaam;
                        }
                    }
                }
            }

            if (!ChecklistModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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

            OverledeneBijlagesModel docResults = await documentGenerator.UpdateChecklist(ChecklistModel, werknemers);

            if (docResults != null)
            {
                docResults.DocumentInconsistent = false;
                docResults.IsDeleted = false;
                docResults.DocumentType = "Word";
                docResults.DocumentName = "Checklist";
                docResults.DocumentUrl = ChecklistModel.DestinationFile;
                docResults.UitvaartId = Globals.UitvaartCodeGuid;
                docResults.DocumentHash = Checksum.GetMD5Checksum(ChecklistModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(docResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting checklist: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating checklist: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }
            TagModel.ChecklistTag = docResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.ChecklistTag);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(TagModel.ChecklistTag);
                    break;
                case "Email":
                    DocumentFunctions.EmailFile(TagModel.ChecklistTag);
                    break;
            }
            return;
        }
        public async Task CreateDocumentDienst(object obj)
        {
            string documentOption = obj.ToString();
            string destinationFile = string.Empty;
            bool initialCreation = false;
            Guid documentId;

            if (string.IsNullOrWhiteSpace(TagModel.AanvraagDienstTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aanvraag.Dienst.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.AanvraagDienstTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "AanvraagDienst");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aanvraag.Dienst.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.AanvraagDienstTag))
                    File.Delete(TagModel.AanvraagDienstTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "AanvraagDienst");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aanvraag.Dienst.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var dienstDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "AanvraagDienst").ConfigureAwait(false);
                documentId = dienstDocument.BijlageId;
                string savedHash = dienstDocument.DocumentHash;

                string documentHash = Checksum.GetMD5Checksum(TagModel.AanvraagDienstTag);

                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{dienstDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.AanvraagDienstTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.AanvraagDienstTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.AanvraagDienstTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.AanvraagDienstTag))
                        {
                            File.Delete(TagModel.AanvraagDienstTag);
                            DienstAanvraagModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aanvraag.Dienst.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(TagModel.AanvraagDienstTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.AanvraagDienstTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.AanvraagDienstTag);
                            break;
                    }
                    return;
                }
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });

            DienstAanvraagModel = await miscellaneousRepository.GetDienstInfoAsync(Globals.UitvaartCodeGuid);
            DienstAanvraagModel.DocumentId = documentId;
            DienstAanvraagModel.DestinationFile = destinationFile;

            if (!DienstAanvraagModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
                MessageBoxResult dienstaanvraag = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dienstaanvraag == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
                }
            }
            OverledeneBijlagesModel docResults = await documentGenerator.UpdateDienst(DienstAanvraagModel).ConfigureAwait(true);

            if (docResults != null)
            {
                docResults.DocumentInconsistent = false;
                docResults.IsDeleted = false;
                docResults.DocumentType = "Word";
                docResults.UitvaartId = Globals.UitvaartCodeGuid;
                docResults.DocumentName = "AanvraagDienst";
                docResults.DocumentUrl = DienstAanvraagModel.DestinationFile;
                docResults.DocumentHash = Checksum.GetMD5Checksum(DienstAanvraagModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(docResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting dienstaanvraag: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating dienstaanvraag: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }

            TagModel.AanvraagDienstTag = docResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.AanvraagDienstTag);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(TagModel.AanvraagDienstTag);
                    break;
                case "Email":
                    DocumentFunctions.EmailFile(TagModel.AanvraagDienstTag);
                    break;
            }
            return;
        }
        public async Task CreateDocumentDocument(object obj)
        {
            string documentOption = obj.ToString();
            string destinationFile = string.Empty;
            Guid documentId;
            bool initialCreation = false;
            if (string.IsNullOrWhiteSpace(TagModel.DocumentTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Document.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.DocumentTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Document");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Document.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.DocumentTag))
                    File.Delete(TagModel.DocumentTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Document");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Document.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var docDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "Document").ConfigureAwait(false);
                documentId = docDocument.BijlageId;
                string savedHash = docDocument.DocumentHash;

                string documentHash = Checksum.GetMD5Checksum(TagModel.DocumentTag);

                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{docDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.DocumentTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.DocumentTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.DocumentTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.DocumentTag))
                        {
                            File.Delete(TagModel.DocumentTag);
                            DocumentModel.Updated = true;
                            DocumentModel.InitialCreation = false;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Document.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(TagModel.DocumentTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.DocumentTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.DocumentTag);
                            break;
                    }
                    return;
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });

            DocumentModel = await miscellaneousRepository.GetDocumentDocumentInfoAsync(Globals.UitvaartCodeGuid);
            DocumentModel.DocumentId = documentId;
            DocumentModel.DestinationFile = destinationFile;
            DocumentModel.UitvaartId = Globals.UitvaartCodeGuid;
            DocumentModel.UitvaartNummer = Globals.UitvaartCode;

            if (!DocumentModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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
            OverledeneBijlagesModel docResults = await documentGenerator.UpdateDocument(DocumentModel).ConfigureAwait(true);

            if (docResults != null)
            {
                docResults.DocumentInconsistent = false;
                docResults.IsDeleted = false;
                docResults.DocumentType = "Word";
                docResults.UitvaartId = Globals.UitvaartCodeGuid;
                docResults.DocumentName = "Document";
                docResults.DocumentUrl = DocumentModel.DestinationFile;
                docResults.DocumentHash = Checksum.GetMD5Checksum(DocumentModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(docResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting Document: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating Document: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }
            TagModel.DocumentTag = docResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });

            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(destinationFile);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(destinationFile);
                    break;
                case "Email":
                    DocumentFunctions.EmailFile(destinationFile);
                    break;
            }
            return;
        }
        public async Task CreateDocumentKoffie(object obj)
        {
            string documentOption = (string)obj;
            string destinationFile = string.Empty;
            Guid documentId;
            bool initialCreation = false;


            if (string.IsNullOrEmpty(TagModel.KoffiekamerTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aanvraag.Koffiekamer.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.KoffiekamerTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Koffiekamer");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aanvraag.Koffiekamer.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.KoffiekamerTag))
                    File.Delete(TagModel.KoffiekamerTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Koffiekamer");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aanvraag.Koffiekamer.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var koffieDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "Koffiekamer").ConfigureAwait(false);
                documentId = koffieDocument.BijlageId;
                string savedHash = koffieDocument.DocumentHash;

                string documentHash = Checksum.GetMD5Checksum(TagModel.KoffiekamerTag);

                if (savedHash == documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{koffieDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.KoffiekamerTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.KoffiekamerTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.KoffiekamerTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.KoffiekamerTag))
                        {
                            File.Delete(TagModel.KoffiekamerTag);
                            KoffieKamerModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aanvraag.Koffiekamer.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(TagModel.KoffiekamerTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.KoffiekamerTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.KoffiekamerTag);
                            break;
                    }
                    return;
                }

            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
            KoffieKamerModel = await miscellaneousRepository.GetKoffieKamerInfoAsync(Globals.UitvaartCodeGuid);
            KoffieKamerModel.DocumentId = documentId;
            KoffieKamerModel.DestinationFile = destinationFile;

            if (!KoffieKamerModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
                }
            }

            OverledeneBijlagesModel koffieResults = await documentGenerator.UpdateKoffie(KoffieKamerModel).ConfigureAwait(true);

            if (koffieResults != null)
            {
                koffieResults.DocumentInconsistent = false;
                koffieResults.IsDeleted = false;
                koffieResults.DocumentType = "Word";
                koffieResults.UitvaartId = Globals.UitvaartCodeGuid;
                koffieResults.DocumentName = "Koffiekamer";
                koffieResults.DocumentUrl = KoffieKamerModel.DestinationFile;
                koffieResults.DocumentHash = Checksum.GetMD5Checksum(KoffieKamerModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(koffieResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting koffie: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        await updateRepository.UpdateDocumentInfoAsync(koffieResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating koffie: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }

            TagModel.KoffiekamerTag = koffieResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.KoffiekamerTag);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(TagModel.KoffiekamerTag);
                    break;
                case "Email":
                    DocumentFunctions.EmailFile(TagModel.KoffiekamerTag);
                    break;
            }
            return;
        }
        public async Task CreateDocumentBezittingen(object obj)
        {
            string documentOption = (string)obj;
            string destinationFile = string.Empty;
            Guid documentId;
            bool initialCreation = false;

            if (string.IsNullOrEmpty(TagModel.BezittingenTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Bezittingen.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.BezittingenTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Bezittingen");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Bezittingen.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.BezittingenTag))
                    File.Delete(TagModel.BezittingenTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Bezittingen");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Bezittingen.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var bezittingenDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "Bezittingen").ConfigureAwait(false);
                documentId = bezittingenDocument.BijlageId;
                string savedHash = bezittingenDocument.DocumentHash;

                string documentHash = Checksum.GetMD5Checksum(TagModel.BezittingenTag);

                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{bezittingenDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.BezittingenTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.BezittingenTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.BezittingenTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.BezittingenTag))
                        {
                            File.Delete(TagModel.BezittingenTag);
                            BezittingenModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Bezittingen.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(TagModel.BezittingenTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.BezittingenTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.BezittingenTag);
                            break;
                    }
                    return;
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
            BezittingenModel = await miscellaneousRepository.GetDocumentBezittingInfoAsync(Globals.UitvaartCodeGuid);
            BezittingenModel.DocumentId = documentId;
            BezittingenModel.DestinationFile = destinationFile;

            if (!BezittingenModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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

            OverledeneBijlagesModel bezittingResults = await documentGenerator.UpdateBezittingen(BezittingenModel).ConfigureAwait(true);

            if (bezittingResults != null)
            {
                bezittingResults.DocumentInconsistent = false;
                bezittingResults.IsDeleted = false;
                bezittingResults.DocumentType = "Word";
                bezittingResults.UitvaartId = Globals.UitvaartCodeGuid;
                bezittingResults.DocumentName = "Bezittingen";
                bezittingResults.DocumentUrl = BezittingenModel.DestinationFile;
                bezittingResults.DocumentHash = Checksum.GetMD5Checksum(BezittingenModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(bezittingResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error insert bezitting: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        await updateRepository.UpdateDocumentInfoAsync(bezittingResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating bezitting: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }
            TagModel.BezittingenTag = bezittingResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.BezittingenTag);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(TagModel.BezittingenTag);
                    break;
                case "Email":
                    DocumentFunctions.EmailFile(TagModel.BezittingenTag);
                    break;
            }
            return;
        }
        public async Task CreateDocumentCrematie(object obj)
        {
            string documentOption = (string)obj;
            string destinationFile = string.Empty;
            Guid documentId;
            bool initialCreation = false;

            if (string.IsNullOrEmpty(TagModel.OpdrachtCrematieTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Opdracht.Crematie.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.OpdrachtCrematieTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "OpdrachtCrematie");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Opdracht.Crematie.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.OpdrachtCrematieTag))
                    File.Delete(TagModel.OpdrachtCrematieTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "OpdrachtCrematie");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Opdracht.Crematie.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var crematieDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "OpdrachtCrematie").ConfigureAwait(false);
                documentId = crematieDocument.BijlageId;
                string savedHash = crematieDocument.DocumentHash;

                string documentHash = Checksum.GetMD5Checksum(TagModel.OpdrachtCrematieTag);

                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{crematieDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.OpdrachtCrematieTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.OpdrachtCrematieTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.OpdrachtCrematieTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.OpdrachtCrematieTag))
                        {
                            File.Delete(TagModel.OpdrachtCrematieTag);
                            CrematieModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Opdracht.Crematie.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrePrintFile(TagModel.OpdrachtCrematieTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.OpdrachtCrematieTag);
                            break;
                        case "Email":
                            DocumentFunctions.PreEmailFile(TagModel.OpdrachtCrematieTag);
                            break;
                    }
                    return;
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
            CrematieModel = await miscellaneousRepository.GetDocumentCrematieInfoAsync(Globals.UitvaartCodeGuid);
            CrematieModel.DocumentId = documentId;
            CrematieModel.DestinationFile = destinationFile;

            if (!CrematieModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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

            if(CrematieModel.Herkomst != Guid.Empty)
            {
                FactuurCreatieModel = await miscellaneousRepository.GetFactuurInfo(CrematieModel.Herkomst);
            }
            else
            {
                FactuurCreatieModel.FactuurAdresNaam = string.Empty;
                FactuurCreatieModel.FactuurAdresRelatie = string.Empty;
                FactuurCreatieModel.FactuurAdresStraat = string.Empty; 
                FactuurCreatieModel.FactuurAdresPostcode = string.Empty;
                FactuurCreatieModel.FactuurAdresGeslacht = string.Empty;
                FactuurCreatieModel.FactuurAdresTelefoon = string.Empty;
                FactuurCreatieModel.FactuurAdresPlaats = string.Empty;
            }

            OverledeneBijlagesModel crematieResults = await documentGenerator.UpdateCrematie(CrematieModel, FactuurCreatieModel).ConfigureAwait(true);

            if (crematieResults != null)
            {
                crematieResults.DocumentInconsistent = false;
                crematieResults.IsDeleted = false;
                crematieResults.DocumentType = "Word";
                crematieResults.UitvaartId = Globals.UitvaartCodeGuid;
                crematieResults.DocumentUrl = CrematieModel.DestinationFile;
                crematieResults.DocumentName = "OpdrachtCrematie";
                crematieResults.DocumentHash = Checksum.GetMD5Checksum(CrematieModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(crematieResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting crematie: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        await updateRepository.UpdateDocumentInfoAsync(crematieResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating crematie: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }

            TagModel.OpdrachtCrematieTag = crematieResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrePrintFile(TagModel.OpdrachtCrematieTag);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(TagModel.OpdrachtCrematieTag);
                    break;
                case "Email":
                    DocumentFunctions.PreEmailFile(TagModel.OpdrachtCrematieTag);
                    break;
            }
            return;
        }
        public async Task CreateDocumentBegrafenis(object obj)
        {
            string documentOption = (string)obj;
            string destinationFile = string.Empty;
            Guid documentId;
            bool initialCreation = false;

            if (string.IsNullOrEmpty(TagModel.OpdrachtBegrafenisTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Opdracht.Begrafenis.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.OpdrachtBegrafenisTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "OpdrachtBegrafenis");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Opdracht.Begrafenis.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.OpdrachtBegrafenisTag))
                    File.Delete(TagModel.OpdrachtBegrafenisTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "OpdrachtBegrafenis");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Opdracht.Begrafenis.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var begrafenisDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "OpdrachtBegrafenis").ConfigureAwait(false);
                documentId = begrafenisDocument.BijlageId;
                string savedHash = begrafenisDocument.DocumentHash;

                string documentHash = Checksum.GetMD5Checksum(TagModel.OpdrachtBegrafenisTag);

                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{begrafenisDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.OpdrachtBegrafenisTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.OpdrachtBegrafenisTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.OpdrachtBegrafenisTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.OpdrachtBegrafenisTag))
                        {
                            File.Delete(TagModel.OpdrachtBegrafenisTag);
                            BegrafenisModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Opdracht.Begrafenis.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(TagModel.OpdrachtBegrafenisTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.OpdrachtBegrafenisTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.OpdrachtBegrafenisTag);
                            break;
                    }
                    return;
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
            BegrafenisModel = await miscellaneousRepository.GetDocumentBegrafenisInfoAsync(Globals.UitvaartCodeGuid);
            BegrafenisModel.DocumentId = documentId;
            BegrafenisModel.DestinationFile = destinationFile;

            if (!BegrafenisModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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

            OverledeneBijlagesModel begrafenisResults = await documentGenerator.UpdateBegrafenis(BegrafenisModel).ConfigureAwait(true);

            if (begrafenisResults != null)
            {
                begrafenisResults.DocumentInconsistent = false;
                begrafenisResults.IsDeleted = false;
                begrafenisResults.DocumentType = "Word";
                begrafenisResults.UitvaartId = Globals.UitvaartCodeGuid;
                begrafenisResults.DocumentUrl = BegrafenisModel.DestinationFile;
                begrafenisResults.DocumentName = "OpdrachtBegrafenis";
                begrafenisResults.DocumentHash = Checksum.GetMD5Checksum(BegrafenisModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(begrafenisResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting begrafenis: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        await updateRepository.UpdateDocumentInfoAsync(begrafenisResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating begrafenis: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }
            ButtonContentModel.OpdrachtBegrafenisContent = "Bewerken";
            TagModel.OpdrachtBegrafenisTag = begrafenisResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrePrintFile(TagModel.OpdrachtBegrafenisTag);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(TagModel.OpdrachtBegrafenisTag);
                    break;
                case "Email":
                    DocumentFunctions.PreEmailFile(TagModel.OpdrachtBegrafenisTag);
                    break;
            }
            return;
        }
        public async Task CreateDocumentTerugmelding(object obj)
        {
            string documentOption = (string)obj;
            string destinationFile = string.Empty;
            string savedHash = string.Empty;
            string documentHash = string.Empty;
            Guid documentId;

            bool initialCreation = false;
            List<string> documentUrls = new List<string>();
            bool recreationTrigger = false;

            if (string.IsNullOrEmpty(TagModel.TerugmeldingTag))
            {
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var terugmeldingDocuments = await miscellaneousRepository.GetDocumentsInformationAsync(Globals.UitvaartCodeGuid, "Terugmelding").ConfigureAwait(false);

                if (terugmeldingDocuments.Count > 1)
                {
                    foreach (var document in terugmeldingDocuments)
                    {
                        Debug.WriteLine(document.DocumentName);
                        documentId = document.BijlageId;

                        if (!File.Exists(document.DocumentUrl))
                        {
                            recreationTrigger = true;
                            Debug.WriteLine($"File does not exist: {document.DocumentUrl}");
                        }
                        else
                        {
                            documentHash = Checksum.GetMD5Checksum(document.DocumentUrl);
                            savedHash = document.DocumentHash;

                            Debug.WriteLine($"Processing document: {document.DocumentUrl}, Hash: {documentHash}, Saved Hash: {savedHash}");
                        }

                        if (documentOption == "Opnieuw" || savedHash != documentHash || !File.Exists(document.DocumentUrl))
                            recreationTrigger = true;
                        else
                            documentUrls.Add(document.DocumentUrl);
                    }

                    if (!recreationTrigger)
                    {
                        Debug.WriteLine("No recreation required, proceeding with selected document option.");
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrintFiles(documentUrls);
                                break;
                            case "Word":
                            case "Opnieuw":
                                foreach(var doc in documentUrls)
                                    Debug.WriteLine(doc);

                                DocumentFunctions.OpenFiles(documentUrls);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFiles(documentUrls);
                                break;
                        }
                        return;
                    }
                    else
                    {
                        foreach (var document in terugmeldingDocuments)
                        {
                            deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Terugmelding");
                            if (File.Exists(document.DocumentUrl))
                            {
                                File.Delete(document.DocumentUrl);
                                TerugmeldingModel.Updated = true;
                                Debug.WriteLine($"Deleted old document: {document.DocumentUrl}");
                            }
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Terugmelding.docx").ConfigureAwait(true);
                        Debug.WriteLine("New document will be created at: " + destinationFile);
                        documentId = Guid.NewGuid();
                    }
                }
                else
                {
                    var document = terugmeldingDocuments.First();
                    documentId = document.BijlageId;
                    savedHash = document.DocumentHash;
                    documentHash = Checksum.GetMD5Checksum(TagModel.TerugmeldingTag);

                    if (documentOption == "Opnieuw")
                    {
                        if (File.Exists(document.DocumentUrl))
                            File.Delete(document.DocumentUrl);

                        TerugmeldingModel.Updated = true;
                        deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Terugmelding");
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Terugmelding.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                        initialCreation = true;
                    }
                    else if (savedHash == documentHash && File.Exists(document.DocumentUrl))
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrintFile(TagModel.TerugmeldingTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.TerugmeldingTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.TerugmeldingTag);
                                break;
                        }
                        return;
                    }
                    else
                    {
                        if (File.Exists(TagModel.TerugmeldingTag))
                        {
                            File.Delete(TagModel.TerugmeldingTag);
                            TerugmeldingModel.Updated = true;
                            Debug.WriteLine("Deleted existing document: " + TagModel.TerugmeldingTag);
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Terugmelding.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
            }
            
            TerugmeldingModel.DestinationFile = destinationFile;
            TerugmeldingModel = await miscellaneousRepository.GetDocumentTerugmeldingInfoAsync(Globals.UitvaartCodeGuid).ConfigureAwait(false);

            if (!TerugmeldingModel.HasData())
            {
                Debug.WriteLine("No data found for Terugmelding, prompting user.");
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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

            var terugmeldingPolissen = JsonConvert.DeserializeObject<List<PolisVerzekering>>(TerugmeldingModel.Polisnummer);
            List<string> distinctPolissen = new List<string>();

            foreach (var terugmeldingPolis in terugmeldingPolissen)
                foreach (var polis in terugmeldingPolis.PolisInfoList)
                    if (!string.IsNullOrEmpty(polis.PolisNr) && !distinctPolissen.Contains(polis.PolisNr))
                        distinctPolissen.Add(polis.PolisNr);

            Debug.WriteLine($"Found {distinctPolissen.Count} unique policies.");

            if (distinctPolissen.Count == 0)
            {
                MessageBox.Show("Er zijn geen polissen opggeven voor dit uitvaartnummer\r\n" +
                    "Je kunt dus ook geen terugmelding maken.", "Geen polis", MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }

            if (distinctPolissen.Count > 1)
            {
                foreach (var polis in distinctPolissen)
                {
                    TerugmeldingModel.DocumentId = Guid.NewGuid();
                    TerugmeldingModel.Polisnummer = polis;
                    destinationFile = await CreateDirectoryPolis(Globals.UitvaartCode, "Terugmelding.docx", polis).ConfigureAwait(true);
                    TerugmeldingModel.DestinationFile = destinationFile;

                    Debug.WriteLine($"Creating document for policy: {polis}, file destination: {destinationFile}");
                    documentUrls.Add(destinationFile);

                    var terugmeldingResults = await documentGenerator.UpdateTerugmelding(TerugmeldingModel).ConfigureAwait(true);

                    if (terugmeldingResults != null)
                    {
                        terugmeldingResults.DocumentInconsistent = false;
                        terugmeldingResults.IsDeleted = false;
                        terugmeldingResults.DocumentType = "Word";
                        terugmeldingResults.UitvaartId = Globals.UitvaartCodeGuid;
                        terugmeldingResults.DocumentUrl = destinationFile;
                        terugmeldingResults.DocumentName = "Terugmelding";
                        terugmeldingResults.DocumentHash = Checksum.GetMD5Checksum(destinationFile);

                        Debug.WriteLine(terugmeldingResults.DocumentUrl);

                        if (initialCreation)
                        {
                            try
                            {
                                Debug.WriteLine("insert");
                                await createRepository.InsertDocumentInfoAsync(terugmeldingResults).ConfigureAwait(false);
                                Debug.WriteLine("Inserted document info into repository.");
                            }
                            catch (System.Exception ex)
                            {
                                MessageBox.Show($"Error inserting terugmelding: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                Debug.WriteLine("update");
                                await updateRepository.UpdateDocumentInfoAsync(terugmeldingResults).ConfigureAwait(false);
                                Debug.WriteLine("Updated document info in repository.");
                            }
                            catch (System.Exception ex)
                            {
                                MessageBox.Show($"Error updating terugmelding: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                                return;
                            }
                        }
                    }
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
                TagModel.TerugmeldingTag = "Alles";
                switch (documentOption)
                {
                    case "Print":
                        DocumentFunctions.PrintFiles(documentUrls);
                        break;
                    case "Word":
                    case "Opnieuw":
                        DocumentFunctions.OpenFiles(documentUrls);
                        break;
                    case "Email":
                        DocumentFunctions.EmailFiles(documentUrls);
                        break;
                }
                return;
            }
            else if (distinctPolissen.Count == 1)
            {
                destinationFile = await CreateDirectoryPolis(Globals.UitvaartCode, "Terugmelding.docx", distinctPolissen.First()).ConfigureAwait(true);
                TerugmeldingModel.DestinationFile = destinationFile;
                TerugmeldingModel.Polisnummer = distinctPolissen.First();

                var terugmeldingResults = await documentGenerator.UpdateTerugmelding(TerugmeldingModel).ConfigureAwait(true);

                if (terugmeldingResults != null)
                {
                    terugmeldingResults.DocumentInconsistent = false;
                    terugmeldingResults.IsDeleted = false;
                    terugmeldingResults.DocumentType = "Word";
                    terugmeldingResults.UitvaartId = Globals.UitvaartCodeGuid;

                    string dirPath = Path.GetDirectoryName(terugmeldingResults.DocumentUrl);
                    string newPolisFilePath = Path.Combine(dirPath, $"Terugmelding.{TerugmeldingModel.Polisnummer}.docx");
                    File.Move(terugmeldingResults.DocumentUrl, newPolisFilePath);

                    Debug.WriteLine($"File moved to new path: {newPolisFilePath}");

                    terugmeldingResults.DocumentUrl = newPolisFilePath;
                    terugmeldingResults.DocumentName = "Terugmelding";
                    terugmeldingResults.DocumentHash = Checksum.GetMD5Checksum(newPolisFilePath);

                    TagModel.TerugmeldingTag = newPolisFilePath;

                    if (initialCreation)
                    {
                        try
                        {
                            await createRepository.InsertDocumentInfoAsync(terugmeldingResults).ConfigureAwait(false);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show($"Error inserting terugmelding: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            await updateRepository.UpdateDocumentInfoAsync(terugmeldingResults).ConfigureAwait(false);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show($"Error updating terugmelding: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                            return;
                        }
                    }
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
                Debug.WriteLine(documentOption);
                switch (documentOption)
                {
                    case "Print":
                        DocumentFunctions.PrintFile(destinationFile);
                        break;
                    case "Word":
                    case "Opnieuw":
                        DocumentFunctions.OpenFile(destinationFile);
                        break;
                    case "Email":
                        DocumentFunctions.EmailFile(destinationFile);
                        break;
                }
                return;
            }
        }
        public async Task CreateDocumentTevredenheid(object obj)
        {
            string documentOption = (string)obj;
            string destinationFile = string.Empty;
            bool initialCreation = false;
            Guid documentId;

            if (string.IsNullOrEmpty(TagModel.TevredenheidTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Tevredenheidsonderzoek.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.TevredenheidTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Tevredenheid");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Tevredenheidsonderzoek.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.TevredenheidTag))
                    File.Delete(TagModel.TevredenheidTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Tevredenheid");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Tevredenheidsonderzoek.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var tevredenheidDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "Tevredenheid").ConfigureAwait(false);
                documentId = tevredenheidDocument.BijlageId;

                string savedHash = tevredenheidDocument.DocumentHash;
                string documentHash = Checksum.GetMD5Checksum(TagModel.TevredenheidTag);



                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{tevredenheidDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.TevredenheidTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.TevredenheidTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.TevredenheidTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.TevredenheidTag))
                        {
                            File.Delete(TagModel.TevredenheidTag);
                            TevredenheidModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Tevredenheidsonderzoek.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrintFile(TagModel.TevredenheidTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.TevredenheidTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.TevredenheidTag);
                            break;
                    }
                    return;
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });
            TevredenheidModel = await miscellaneousRepository.GetDocumentTevredenheidsInfoAsync(Globals.UitvaartCodeGuid);
            TevredenheidModel.DocumentId = documentId;
            TevredenheidModel.DestinationFile = destinationFile;

            if (!TevredenheidModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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

            OverledeneBijlagesModel tevredenheidResults = await documentGenerator.UpdateTevredenheid(TevredenheidModel).ConfigureAwait(true);

            if (tevredenheidResults != null)
            {
                tevredenheidResults.DocumentInconsistent = false;
                tevredenheidResults.IsDeleted = false;
                tevredenheidResults.DocumentType = "Word";
                tevredenheidResults.UitvaartId = Globals.UitvaartCodeGuid;
                tevredenheidResults.DocumentName = "Tevredenheid";
                tevredenheidResults.DocumentUrl = TevredenheidModel.DestinationFile;
                tevredenheidResults.DocumentHash = Checksum.GetMD5Checksum(TevredenheidModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(tevredenheidResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting tevredenheids docinfo: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        await updateRepository.UpdateDocumentInfoAsync(tevredenheidResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updatign tevredenheids docinfo: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }

            TagModel.TevredenheidTag = tevredenheidResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(destinationFile);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(destinationFile);
                    break;
                case "Email":
                    DocumentFunctions.EmailFile(destinationFile);
                    break;
            }
            return;
        }
        public async Task CreateDocumentAangifte(object obj)
        {
            string documentOption = obj.ToString();
            string destinationFile = string.Empty;
            bool initialCreation = false;
            Guid documentId;

            if (string.IsNullOrEmpty(TagModel.AangifteTag))
            {
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aangifte.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.AangifteTag))
            {
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Aangifte");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aangifte.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (documentOption == "Opnieuw")
            {
                if (File.Exists(TagModel.AangifteTag))
                    File.Delete(TagModel.AangifteTag);

                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Aangifte");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aangifte.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                var aangifteDocument = await miscellaneousRepository.GetDocumentInformationAsync(Globals.UitvaartCodeGuid, "Aangifte").ConfigureAwait(false);
                documentId = aangifteDocument.BijlageId;

                string savedHash = aangifteDocument.DocumentHash;
                string documentHash = Checksum.GetMD5Checksum(TagModel.AangifteTag);


                if (savedHash != documentHash)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show($"{aangifteDocument.DocumentName} - {Globals.UitvaartCode}", "Document inconsistent!", "Het bestaande document is aangepast buiten de applicatie...\r\n Wil je de bestaande openen of een nieuwe aanmaken?", "Openen", "Nieuw");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrePrintFile(TagModel.AangifteTag);
                                break;
                            case "Word":
                            case "Opnieuw":
                                DocumentFunctions.OpenFile(TagModel.AangifteTag);
                                break;
                            case "Email":
                                DocumentFunctions.EmailFile(TagModel.AangifteTag);
                                break;
                        }
                        return;
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        if (File.Exists(TagModel.AangifteTag))
                        {
                            File.Delete(TagModel.AangifteTag);
                            AangifteModel.Updated = true;
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Aangifte.docx").ConfigureAwait(true);
                        documentId = Guid.NewGuid();
                    }
                }
                else if (savedHash == documentHash)
                {
                    switch (documentOption)
                    {
                        case "Print":
                            DocumentFunctions.PrePrintFile(TagModel.AangifteTag);
                            break;
                        case "Word":
                        case "Opnieuw":
                            DocumentFunctions.OpenFile(TagModel.AangifteTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.AangifteTag);
                            break;
                    }
                    return;
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Show(); });

            AangifteModel = await miscellaneousRepository.GetDocumentAangifteInfoAsync(Globals.UitvaartCodeGuid);
            AangifteModel.DocumentId = documentId;
            AangifteModel.DestinationFile = destinationFile;
            AangifteModel.UitvaartNummer = Globals.UitvaartCode;

            if (!AangifteModel.HasData())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
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

            OverledeneBijlagesModel aangifteResults = await documentGenerator.UpdateAangifte(AangifteModel).ConfigureAwait(true);

            if (aangifteResults != null)
            {
                aangifteResults.DocumentInconsistent = false;
                aangifteResults.IsDeleted = false;
                aangifteResults.DocumentType = "Word";
                aangifteResults.UitvaartId = Globals.UitvaartCodeGuid;
                aangifteResults.DocumentName = "Aangifte";
                aangifteResults.DocumentUrl = AangifteModel.DestinationFile;
                aangifteResults.DocumentHash = Checksum.GetMD5Checksum(AangifteModel.DestinationFile);

                if (initialCreation)
                {
                    try
                    {
                        await createRepository.InsertDocumentInfoAsync(aangifteResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error inserting aangifte docinfo: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        await updateRepository.UpdateDocumentInfoAsync(aangifteResults);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Error updating aangifte docinfo: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                }
            }
            TagModel.AangifteTag = aangifteResults.DocumentUrl;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { _generatingDocumentView.Hide(); });
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(destinationFile);
                    break;
                case "Word":
                case "Opnieuw":
                    DocumentFunctions.OpenFile(destinationFile);
                    break;
                case "Email":
                    DocumentFunctions.EmailFile(destinationFile);
                    break;
            }
            return;
        }
    }
    public class JsonDeserializer
    {
        private ISearchOperations searchRepository;
        public ObservableCollection<PolisVerzekering> DeserializeJson(string UitvaartNummer)
        {
            searchRepository = new SearchOperations();
            var verzekeringResult = searchRepository.GetOverlijdenVerzekeringByUitvaartId(UitvaartNummer);
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
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}