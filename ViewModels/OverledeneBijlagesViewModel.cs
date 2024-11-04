using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Interop.Word;
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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using static Dossier_Registratie.MainWindow;
using static Dossier_Registratie.ViewModels.OverledeneFactuurViewModel;
using static Dossier_Registratie.ViewModels.OverledeneSteenhouwerijViewModel;
using Range = Microsoft.Office.Interop.Word.Range;
using Task = System.Threading.Tasks.Task;

namespace Dossier_Registratie.ViewModels
{
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
        private ObservableCollection<PolisVerzekering> _verzekeringen;
        private PolisVerzekering _selectedVerzekering;
        private ObservableCollection<PolisVerzekering> _verzekeringenWithAll;
        private ModelCompare modelCompare;
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
        private string _documentOption;
        private string _fileExists;
        private string _fileGenerate;
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

            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            BijlageModel = new OverledeneBijlagesModel();
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

            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
            OpenPopupCommand = new ViewModelCommand(ExecuteOpenPopupCommand);
            ClosePopupCommand = new ViewModelCommand(ExecuteClosePopupCommand);
            OpenKostenbegrotingCommand = new ViewModelCommand(ExecuteKostenbegrotingCommand);
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
            modelCompare = new ModelCompare();
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
        }
        public static OverledeneBijlagesViewModel BijlagesInstance { get; } = new();
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {

            modelCompare = new ModelCompare();
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
            }
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
                {
                    Directory.CreateDirectory(destinationLoc);
                }
                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
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

                // Copy the template file to the new location with the new name
                sourceFile.CopyTo(destinationFileWithPolisNr);
            }
            else
            {
                throw new FileNotFoundException("The template file does not exist.", fileToCopy);
            }

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
        private Task CreateAkteFile(string UitvaartId, string documentName, bool akteStatus, bool sendMail)
        {
            Microsoft.Office.Interop.Outlook.Application outlookApp = null;
            MailItem mail = null;

            if (sendMail == true)
            {
                outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                mail = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);
            }

            string sourceLoc = DataProvider.DocumentenOpslag;
            string templateLoc = DataProvider.TemplateFolder;

            bool sourceLocEndSlash = sourceLoc.EndsWith(@"\");

            if (!sourceLocEndSlash)
                sourceLoc += "\\";

            bool templateLocatEndSlash = templateLoc.EndsWith(@"\");
            if (!templateLocatEndSlash)
                templateLoc += "\\";

            string fileToCopy = templateLoc + "Akte.van.Cessie.docx";

            string destinationLoc = sourceLoc + UitvaartId;

            bool destinationLocEndSlash = destinationLoc.EndsWith(@"\");
            if (!destinationLocEndSlash)
                destinationLoc += "\\";

            FileInfo sourceFile = new(fileToCopy);

            bool isSingleAkte = IsSingleValue(documentName);
            if (!isSingleAkte)
            {
                foreach (string document in documentName.Split(','))
                {
                    destinationFile = destinationLoc + "AkteVanCessie_" + document + ".docx";

                    if (sourceFile.Exists)
                    {
                        if (!Directory.Exists(destinationLoc))
                        {
                            Directory.CreateDirectory(destinationLoc);
                        }
                        if (File.Exists(destinationFile))
                        {
                            File.Delete(destinationFile);
                        }
                        sourceFile.CopyTo(destinationFile);

                        foreach (var verzekeringElement in VerzekeringenWithAll)
                        {
                            if (verzekeringElement.VerzekeringName != "Alles" && verzekeringElement.VerzekeringName == document)
                            {
                                FillAkteFile(verzekeringElement.VerzekeringName, verzekeringElement.PolisInfoList, akteStatus);
                            }
                        }
                        if (sendMail == true)
                            mail.Attachments.Add(destinationFile, OlAttachmentType.olByValue, 1, "AkteVanCessie_" + document + ".docx");
                    }
                    if (sendMail == false)
                    {
                        Process.Start(new ProcessStartInfo()
                        {
                            FileName = destinationFile,
                            UseShellExecute = true,
                            Verb = "open"
                        });
                    }
                }
                if (sendMail == true)
                    mail.Subject = "Akte van Cessies - " + documentName;
            }
            else
            {
                destinationFile = destinationLoc + "AkteVanCessie_" + documentName + ".docx";

                if (sourceFile.Exists)
                {
                    if (!Directory.Exists(destinationLoc))
                    {
                        Directory.CreateDirectory(destinationLoc);
                    }
                    if (File.Exists(destinationFile))
                    {
                        File.Delete(destinationFile);
                    }
                    sourceFile.CopyTo(destinationFile);
                    FillAkteFile(SelectedVerzekering.VerzekeringName, SelectedVerzekering.PolisInfoList, akteStatus);

                    if (sendMail == true)
                        mail.Attachments.Add(destinationFile, OlAttachmentType.olByValue, 1, "AkteVanCessie_" + documentName + ".docx");
                }

                if (sendMail == false)
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = destinationFile,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                else
                {
                    if (sendMail == true)
                        mail.Subject = "Akte van Cessie - " + documentName;
                }
            }

            if (sendMail == true)
            {
                mail.Display(true);
                if (mail != null) Marshal.ReleaseComObject(mail);
                if (outlookApp != null) Marshal.ReleaseComObject(outlookApp);
            }
            return Task.CompletedTask;
        }
        private Task FillAkteFile(string verzekeringName, List<Polis> polisInfoList, bool akteStatus)
        {
            string opdrachtgeverNaam = string.Empty;
            string opdrachtgeverAdres = string.Empty;
            string opdrachtgeverRelatie = string.Empty;
            string geslotenOpLevenVan = string.Empty;
            string overledeneAdres = string.Empty;
            DateTime geborenOp = DateTime.MinValue;
            DateTime overledenOp = DateTime.MinValue;

            conn.Open();
            SqlDataAdapter da = new("SELECT (CASE WHEN opdrachtgeverTussenvoegsel IS NULL THEN CONCAT(opdrachtgeverAanhef, ' ', opdrachtgeverAchternaam, ', ', LEFT(ISNULL(opdrachtgeverVoornaamen, ''), 1)) " +
                "ELSE CONCAT(opdrachtgeverAanhef, ' ', opdrachtgeverTussenvoegsel, ' ', opdrachtgeverAchternaam, ', ', LEFT(ISNULL(opdrachtgeverVoornaamen, ''), 1)) END) AS NaamOpdrachtgever, " +
                "(CASE WHEN opdrachtgeverHuisnummerToevoeging IS NULL THEN CONCAT(opdrachtgeverStraat, ' ', opdrachtgeverHuisnummer) ELSE CONCAT(opdrachtgeverStraat, ' ', TRIM(opdrachtgeverHuisnummer), ' ', TRIM(opdrachtgeverHuisnummerToevoeging)) END) as AdresOpdrachtgever, " +
                "opdrachtgeverRelatieTotOverledene, " +
                "(CASE WHEN overledeneTussenvoegsel IS NULL THEN CONCAT(overledeneAanhef, ' ', overledeneAchternaam) " +
                "ELSE CONCAT(overledeneAanhef, ' ', overledeneTussenvoegsel, ' ', overledeneAchternaam) END) AS NaamOverledene, " +
                "overledeneGeboortedatum, overledenPlaats, overledenDatumTijd, " +
                "(CASE WHEN overledenHuisnummerToevoeging IS NULL THEN CONCAT(overledenAdres, ' ', overledenHuisnummer, ', ', overledenPlaats) ELSE CONCAT(overledenAdres, ' ', TRIM(overledenHuisnummer), ' ', TRIM(overledenHuisnummerToevoeging), ', ', overledenPlaats) END) as AdresOpverledene " +
                "FROM OverledeneOpdrachtgever OO " +
                "INNER JOIN OverledenePersoonsGegevens OPG ON OO.uitvaartId = OPG.uitvaartId " +
                "INNER JOIN OverledeneOverlijdenInfo OOI ON OO.uitvaartId = OOI.UitvaartId " +
                "WHERE OO.uitvaartId = '" + Globals.UitvaartCodeGuid + "'", conn);
            DataSet ds = new();
            da.Fill(ds, "AkteInfo");

            if (ds.Tables[0].Rows.Count > 0)
            {
                opdrachtgeverNaam = ds.Tables[0].Rows[0]["NaamOpdrachtgever"].ToString();
                opdrachtgeverAdres = ds.Tables[0].Rows[0]["AdresOpdrachtgever"].ToString();
                opdrachtgeverRelatie = ds.Tables[0].Rows[0]["opdrachtgeverRelatieTotOverledene"].ToString();
                geslotenOpLevenVan = ds.Tables[0].Rows[0]["NaamOverledene"].ToString();
                geborenOp = (DateTime)ds.Tables[0].Rows[0]["overledeneGeboortedatum"];
                overledenOp = (DateTime)ds.Tables[0].Rows[0]["overledenDatumTijd"];
                overledeneAdres = ds.Tables[0].Rows[0]["AdresOpverledene"].ToString();
            }

            conn.Close();

            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            Document doc = app.Documents.Open(destinationFile);

            string verzekeringNaam;
            int polisBedragCount = 2;
            double totalPolisBedrag = 0;


            Dictionary<string, string> bookmarks = new Dictionary<string, string>
            {
                { "verzekeringMaatschappij", verzekeringName },
                { "ondergetekendeNaamEnVoorletters", opdrachtgeverNaam },
                { "ondergetekendeAdres", opdrachtgeverAdres },
                { "ondergetekendeRelatieTotVerzekende", opdrachtgeverRelatie },
                { "verzekerdeGeslotenOpHetLevenVan", geslotenOpLevenVan },
                { "verzekerdeGeborenOp", geborenOp.ToString("dd-MM-yyyy") },
                { "verzekerdeOverledenOp", overledenOp.ToString("dd-MM-yyyy") },
                { "verzekerdeAdres", overledeneAdres },
                { "vermeldingNummer", Globals.UitvaartCode },
                { "PolisTable", "" },
                { "OrganisatieNaam", DataProvider.OrganizationName },
                { "OrganisatieIban", DataProvider.OrganizationIban }
            };

            foreach (var polisInfo in polisInfoList)
            {
                if (!string.IsNullOrEmpty(polisInfo.PolisBedrag))
                {
                    polisBedragCount++;

                    if (double.TryParse(polisInfo.PolisBedrag, out double polisBedragValue))
                        totalPolisBedrag += polisBedragValue;
                }
            }

            foreach (var bookmark in bookmarks)
            {
                Bookmark bm = doc.Bookmarks[bookmark.Key];
                Range range = bm.Range;
                range.Text = bookmark.Value;

                if (bookmark.Key == "PolisTable")
                {
                    Microsoft.Office.Interop.Word.Table table = doc.Tables.Add(range, polisBedragCount, 2);

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
                    table.Cell(polisBedragCount, 2).Range.Text = "€ " + totalPolisBedrag.ToString();
                }
            }

            doc.Save();
            doc.Close();

            string akteQuery;
            if (akteStatus)
            {
                akteQuery = "UPDATE [OverledeneBijlages]" +
                                    " SET [DocumentURL] = '" + destinationFile + "'," +
                                    " [DocumentHash] = 'recreated_on_build'" +
                                    " WHERE [UitvaartId] = '" + Globals.UitvaartCodeGuid + "'" +
                                    " AND [DocumentName] = 'AkteVanCessie_" + verzekeringName + ".docx'";

            }
            else
            {
                Guid documentId = Guid.NewGuid();

                akteQuery = "INSERT INTO [OverledeneBijlages] ([BijlageId],[UitvaartId],[DocumentName],[DocumentType],[DocumentURL],[DocumentHash],[DocumentInconsistent],[isDeleted]) " +
                    "VALUES ('" + documentId + "', '" + Globals.UitvaartCodeGuid + "', 'AkteVanCessie_" + verzekeringName + ".docx', 'Word', '" + destinationFile + "', 'recreated_on_build',0,0)";
            }

            /* No need to import to Database cause it's regenerated every time.
            using (SqlCommand command = new SqlCommand(akteQuery, conn))
            {
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
            */
            return Task.CompletedTask;
        }
        private async Task GenererenCreateAkteVanCessie(object parameter)
        {
            _generatingDocumentView.Show();

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
            _generatingDocumentView.Hide();
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
                            DocumentFunctions.OpenFile(TagModel.OverdrachtTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.OverdrachtTag);
                            break;
                    }
                    return;
                }
            }

            _generatingDocumentView.Show();

            OverdrachtModel = await miscellaneousRepository.GetDocumentOverdrachtInfoAsync(Globals.UitvaartCodeGuid);
            OverdrachtModel.DocumentId = documentId;
            OverdrachtModel.DestinationFile = destinationFile;
            OverdrachtModel.UitvaartId = Globals.UitvaartCodeGuid;
            OverdrachtModel.UitvaartNummer = Globals.UitvaartCode;

            if (!OverdrachtModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.OverdrachtTag);
                    break;
                case "Word":
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
                Debug.WriteLine(TagModel.ChecklistTag);
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Checklist.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else if (!File.Exists(TagModel.ChecklistTag))
            {
                Debug.WriteLine(TagModel.ChecklistTag);
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Checklist");
                destinationFile = await CreateDirectory(Globals.UitvaartCode, "Checklist.docx").ConfigureAwait(true);
                documentId = Guid.NewGuid();
                initialCreation = true;
            }
            else
            {
                Debug.WriteLine(TagModel.ChecklistTag);
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
                            DocumentFunctions.OpenFile(TagModel.ChecklistTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.ChecklistTag);
                            break;
                    }
                    return;
                }
            }

            _generatingDocumentView.Show();
            ChecklistModel = await miscellaneousRepository.GetDocumentChecklistInfoAsync(Globals.UitvaartCodeGuid);
            ChecklistModel.DocumentId = documentId;
            ChecklistModel.DestinationFile = destinationFile;
            ChecklistModel.UitvaartNummer = Globals._uitvaartCode;
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
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.ChecklistTag);
                    break;
                case "Word":
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
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Aanvraag.Dienst");
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
                            DocumentFunctions.OpenFile(TagModel.AanvraagDienstTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.AanvraagDienstTag);
                            break;
                    }
                    return;
                }
            }

            _generatingDocumentView.Show();

            DienstAanvraagModel = await miscellaneousRepository.GetDienstInfoAsync(Globals.UitvaartCodeGuid);
            DienstAanvraagModel.DocumentId = documentId;
            DienstAanvraagModel.DestinationFile = destinationFile;

            if (!DienstAanvraagModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult dienstaanvraag = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dienstaanvraag == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.AanvraagDienstTag);
                    break;
                case "Word":
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
                            DocumentFunctions.OpenFile(TagModel.DocumentTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.DocumentTag);
                            break;
                    }
                    return;
                }
            }
            _generatingDocumentView.Show();

            DocumentModel = await miscellaneousRepository.GetDocumentDocumentInfoAsync(Globals.UitvaartCodeGuid);
            DocumentModel.DocumentId = documentId;
            DocumentModel.DestinationFile = destinationFile;
            DocumentModel.UitvaartId = Globals.UitvaartCodeGuid;


            if (!DocumentModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
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
            _generatingDocumentView.Hide();

            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(destinationFile);
                    break;
                case "Word":
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
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Aanvraag.Koffiekamer");
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
                            DocumentFunctions.OpenFile(TagModel.KoffiekamerTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.KoffiekamerTag);
                            break;
                    }
                    return;
                }

            }

            _generatingDocumentView.Show();
            KoffieKamerModel = await miscellaneousRepository.GetKoffieKamerInfoAsync(Globals.UitvaartCodeGuid);
            KoffieKamerModel.DocumentId = documentId;
            KoffieKamerModel.DestinationFile = destinationFile;

            if (!KoffieKamerModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Hide();
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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.KoffiekamerTag);
                    break;
                case "Word":
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
                            DocumentFunctions.OpenFile(TagModel.BezittingenTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.BezittingenTag);
                            break;
                    }
                    return;
                }
            }
            _generatingDocumentView.Show();
            BezittingenModel = await miscellaneousRepository.GetDocumentBezittingInfoAsync(Globals.UitvaartCodeGuid);
            BezittingenModel.DocumentId = documentId;
            BezittingenModel.DestinationFile = destinationFile;

            if (!BezittingenModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(TagModel.BezittingenTag);
                    break;
                case "Word":
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
                            DocumentFunctions.OpenFile(TagModel.OpdrachtCrematieTag);
                            break;
                        case "Email":
                            DocumentFunctions.PreEmailFile(TagModel.OpdrachtCrematieTag);
                            break;
                    }
                    return;
                }
            }
            _generatingDocumentView.Show();
            CrematieModel = await miscellaneousRepository.GetDocumentCrematieInfoAsync(Globals.UitvaartCodeGuid);
            CrematieModel.DocumentId = documentId;
            CrematieModel.DestinationFile = destinationFile;

            if (!CrematieModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
                }
            }
            Guid HerkomstId = Guid.Parse(CrematieModel.Herkomst);
            FactuurCreatieModel = await miscellaneousRepository.GetFactuurInfo(HerkomstId);

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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrePrintFile(TagModel.OpdrachtCrematieTag);
                    break;
                case "Word":
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
                            DocumentFunctions.OpenFile(TagModel.OpdrachtBegrafenisTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.OpdrachtBegrafenisTag);
                            break;
                    }
                    return;
                }
            }
            _generatingDocumentView.Show();
            BegrafenisModel = await miscellaneousRepository.GetDocumentBegrafenisInfoAsync(Globals.UitvaartCodeGuid);
            BegrafenisModel.DocumentId = documentId;
            BegrafenisModel.DestinationFile = destinationFile;

            if (!BegrafenisModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrePrintFile(TagModel.OpdrachtBegrafenisTag);
                    break;
                case "Word":
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
                        documentId = document.BijlageId;
                        documentHash = Checksum.GetMD5Checksum(document.DocumentUrl);
                        savedHash = document.DocumentHash;

                        if (!File.Exists(document.DocumentUrl))
                        {

                        }

                        if (savedHash == documentHash && File.Exists(document.DocumentUrl))
                        {
                            documentUrls.Add(document.DocumentUrl);
                        }
                        else
                        {
                            recreationTrigger = true;
                        }
                    }

                    if (!recreationTrigger)
                    {
                        // Perform the required action based on documentOption
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrintFiles(documentUrls);
                                break;
                            case "Word":
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
                            if (File.Exists(document.DocumentUrl))
                            {
                                File.Delete(document.DocumentUrl);
                                TerugmeldingModel.Updated = true;
                            }
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Terugmelding.docx").ConfigureAwait(true);
                    }
                }
                else
                {
                    var document = terugmeldingDocuments.First();
                    documentId = document.BijlageId;
                    savedHash = document.DocumentHash;
                    documentHash = Checksum.GetMD5Checksum(TagModel.TerugmeldingTag);

                    if (savedHash == documentHash && File.Exists(document.DocumentUrl))
                    {
                        switch (documentOption)
                        {
                            case "Print":
                                DocumentFunctions.PrintFile(TagModel.TerugmeldingTag);
                                break;
                            case "Word":
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
                        }
                        destinationFile = await CreateDirectory(Globals.UitvaartCode, "Terugmelding.docx").ConfigureAwait(true);
                    }
                }
            }

            TerugmeldingModel = await miscellaneousRepository.GetDocumentTerugmeldingInfoAsync(Globals.UitvaartCodeGuid).ConfigureAwait(false);
            TerugmeldingModel.DestinationFile = destinationFile;

            if (!TerugmeldingModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
                }
            }

            var terugmeldingPolissen = JsonConvert.DeserializeObject<List<PolisVerzekering>>(TerugmeldingModel.Polisnummer);

            List<string> distinctPolissen = new List<string>();

            foreach (var terugmeldingPolis in terugmeldingPolissen)
            {
                foreach (var polis in terugmeldingPolis.PolisInfoList)
                {
                    if (!string.IsNullOrEmpty(polis.PolisNr) && !distinctPolissen.Contains(polis.PolisNr))
                    {
                        distinctPolissen.Add(polis.PolisNr);
                    }
                }
            }

            Debug.WriteLine(distinctPolissen.Count);
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
                }
                _generatingDocumentView.Hide();
                switch (documentOption)
                {
                    case "Print":
                        DocumentFunctions.PrintFiles(documentUrls);
                        break;
                    case "Word":
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

                    terugmeldingResults.DocumentUrl = newPolisFilePath;
                    terugmeldingResults.DocumentName = "Terugmelding";
                    terugmeldingResults.DocumentHash = Checksum.GetMD5Checksum(newPolisFilePath);

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
                _generatingDocumentView.Hide();
                switch (documentOption)
                {
                    case "Print":
                        DocumentFunctions.PrintFile(destinationFile);
                        break;
                    case "Word":
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
                deleteRepository.SetDocumentDeleted(Globals.UitvaartCodeGuid, "Tevredenheidsonderzoek");
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
                            DocumentFunctions.OpenFile(TagModel.TevredenheidTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.TevredenheidTag);
                            break;
                    }
                    return;
                }
            }
            _generatingDocumentView.Show();
            TevredenheidModel = await miscellaneousRepository.GetDocumentTevredenheidsInfoAsync(Globals.UitvaartCodeGuid);
            TevredenheidModel.DocumentId = documentId;
            TevredenheidModel.DestinationFile = destinationFile;

            if (!TevredenheidModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(destinationFile);
                    break;
                case "Word":
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
                            DocumentFunctions.OpenFile(TagModel.AangifteTag);
                            break;
                        case "Email":
                            DocumentFunctions.EmailFile(TagModel.AangifteTag);
                            break;
                    }
                    return;
                }
            }
            _generatingDocumentView.Show();

            AangifteModel = await miscellaneousRepository.GetDocumentAangifteInfoAsync(Globals.UitvaartCodeGuid);
            AangifteModel.DocumentId = documentId;
            AangifteModel.DestinationFile = destinationFile;
            AangifteModel.UitvaartNummer = Globals.UitvaartCode;

            if (!AangifteModel.HasData())
            {
                _generatingDocumentView.Hide();
                MessageBoxResult result = MessageBox.Show("Geen data gevonden voor " + Globals.UitvaartCode + ", leeg formulier maken?", "Geen data gevonden", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    _generatingDocumentView.Show();
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
            _generatingDocumentView.Hide();
            switch (documentOption)
            {
                case "Print":
                    DocumentFunctions.PrintFile(destinationFile);
                    break;
                case "Word":
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
            var verzekeringen = JsonConvert.DeserializeObject<List<PolisVerzekering>>(jsonString);
            var filteredVerzekeringen = verzekeringen.Where(v => !string.IsNullOrEmpty(v.VerzekeringName) && v.VerzekeringName != "null").ToList();

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