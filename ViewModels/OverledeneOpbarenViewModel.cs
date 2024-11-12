using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Dossier_Registratie.MainWindow;
using static Dossier_Registratie.ViewModels.OverledeneUitvaartViewModel;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneOpbarenViewModel : ViewModelBase
    {
        public bool initialLoadDone;
        private bool _correctAccessOrNotCompleted = true;
        public List<VerzorgingClass> VerzorgingenElement { get; set; }
        private ObservableCollection<KistenModel> _uitvaartKisten;
        private ObservableCollection<KistenLengte> _uitvaartKistenLengte;
        private ObservableCollection<VerzorgendPersoneel> _verzorgendPersoneel;
        private OverledeneUitvaartleiderModel _uitvaartLeiderModel;
        private VerzorgingClass _verzorging;
        private OverledeneOpbarenModel _overledeneOpbarenModel;
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private ModelCompare modelCompare;
        private OverledeneOpbarenModel _originalOverledeneOpbarenModel;
        private KistenModel _selectedKist;
        public bool CorrectAccessOrNotCompleted
        {
            get { return _correctAccessOrNotCompleted; }
            set
            {
                _correctAccessOrNotCompleted = value;
                OnPropertyChanged(nameof(CorrectAccessOrNotCompleted));
            }
        }
        public KistenModel SelectedKist
        {
            get { return _selectedKist; }
            set
            {
                _selectedKist = value;
                OnPropertyChanged(nameof(SelectedKist));
                UpdateDescription();
            }
        }
        public ObservableCollection<VerzorgendPersoneel> VerzorgersPersonen { get; }
        public ObservableCollection<VerzorgingClass> Verzorgingen { get; }
        public ObservableCollection<KistenModel> UitvaartKisten
        {
            get { return _uitvaartKisten; }
            set
            {
                _uitvaartKisten = value;
                OnPropertyChanged(nameof(UitvaartKisten));
            }
        }
        public ObservableCollection<KistenLengte> UitvaartKistenLengte
        {
            get { return _uitvaartKistenLengte; }
            set
            {
                _uitvaartKistenLengte = value;
                OnPropertyChanged(nameof(UitvaartKistenLengte));
            }
        }
        public ObservableCollection<VerzorgendPersoneel> VerzorgendPersoneel
        {
            get { return _verzorgendPersoneel; }
            set
            {
                _verzorgendPersoneel = value;
                OnPropertyChanged(nameof(VerzorgendPersoneel));
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

        public VerzorgingClass Verzorging
        {
            get { return _verzorging; }
            set { _verzorging = value; OnPropertyChanged(nameof(Verzorging)); }
        }
        public OverledeneOpbarenModel OverledeneOpbarenModel
        {
            get
            {
                return _overledeneOpbarenModel;
            }
            set
            {
                _overledeneOpbarenModel = value;
                OnPropertyChanged(nameof(OverledeneOpbarenModel));
            }
        }
        public ICommand SaveCommand { get; }
        public ICommand PreviousCommand { get; }
        public void UpdateDescription()
        {
            if (SelectedKist != null)
            {
                OverledeneOpbarenModel.OpbaringKistOmschrijving = SelectedKist.KistOmschrijving;
                OverledeneOpbarenModel.OpbaringKistId = SelectedKist.Id;
                OnPropertyChanged(nameof(OverledeneOpbarenModel));
            }
        }

        private OverledeneOpbarenViewModel()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            Verzorgingen = new ObservableCollection<VerzorgingClass>();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            OverledeneOpbarenModel = new OverledeneOpbarenModel();
            UitvaartKisten = new ObservableCollection<KistenModel>();
            UitvaartKistenLengte = new ObservableCollection<KistenLengte>();
            VerzorgersPersonen = new ObservableCollection<VerzorgendPersoneel>();
            Verzorging = new VerzorgingClass();

            foreach (var kel in miscellaneousRepository.GetKisten())
            {
                if (kel.IsDeleted == false)
                    UitvaartKisten.Add(new KistenModel { Id = kel.Id, KistTypeNummer = kel.KistTypeNummer, KistOmschrijving = kel.KistOmschrijving });
            }

            foreach (var lel in miscellaneousRepository.GetKistenLengte())
            {
                if (lel.IsDeleted == false)
                    UitvaartKistenLengte.Add(new KistenLengte { Id = lel.Id, KistLengte = lel.KistLengte });
            }

            foreach (var el in miscellaneousRepository.GetVerzorgers())
            {
                if (!el.IsDeleted && el.IsOpbaren)
                    VerzorgersPersonen.Add(new VerzorgendPersoneel { Id = el.Id, VerzorgendPersoon = el.VerzorgendPersoon });
            }

            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
        }
        public void ReloadDynamicElements()
        {
            foreach (var kel in miscellaneousRepository.GetKisten())
            {
                if (!UitvaartKisten.Any(u => u.Id == kel.Id) && kel.IsDeleted == false)
                {
                    UitvaartKisten.Add(new KistenModel { Id = kel.Id, KistTypeNummer = kel.KistTypeNummer, KistOmschrijving = kel.KistOmschrijving });
                }
            }

            foreach (var lel in miscellaneousRepository.GetKistenLengte())
            {
                if (!UitvaartKistenLengte.Any(u => u.Id == lel.Id) && lel.IsDeleted == false)
                {
                    UitvaartKistenLengte.Add(new KistenLengte { Id = lel.Id, KistLengte = lel.KistLengte });
                }
            }

            foreach (var el in miscellaneousRepository.GetVerzorgers())
            {
                if (!VerzorgersPersonen.Any(u => u.Id == el.Id) && el.IsDeleted == false && el.IsOpbaren)
                    VerzorgersPersonen.Add(new VerzorgendPersoneel { Id = el.Id, VerzorgendPersoon = el.VerzorgendPersoon });
            }
        }
        public void CreateNewDossier()
        {
            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            Verzorgingen.Clear();
            OverledeneOpbarenModel = new OverledeneOpbarenModel();
            Verzorging = new VerzorgingClass();

            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;
        }
        public static OverledeneOpbarenViewModel OpbarenInstance { get; } = new();
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            Verzorgingen.Clear();
            OverledeneOpbarenModel = new OverledeneOpbarenModel();
            Verzorging = new VerzorgingClass();

            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;

            var opbarenResult = searchRepository.GetOverlijdenOpbarenInfoByUitvaartId(uitvaartNummer);
            if (opbarenResult != null)
            {
                OverledeneOpbarenModel.OpbaringId = opbarenResult.OpbaringId;
                OverledeneOpbarenModel.UitvaartId = opbarenResult.UitvaartId;
                OverledeneOpbarenModel.OpbaringLocatie = opbarenResult.OpbaringLocatie;
                OverledeneOpbarenModel.OpbaringKistId = opbarenResult.OpbaringKistId;
                OverledeneOpbarenModel.OpbaringKistLengte = opbarenResult.OpbaringKistLengte;
                OverledeneOpbarenModel.OpbaringVerzorging = opbarenResult.OpbaringVerzorging;
                OverledeneOpbarenModel.OpbaringVerzorgingJson = opbarenResult.OpbaringVerzorgingJson;
                OverledeneOpbarenModel.OpbaringKoeling = opbarenResult.OpbaringKoeling;
                OverledeneOpbarenModel.OpbaringKledingMee = opbarenResult.OpbaringKledingMee;
                OverledeneOpbarenModel.OpbaringKledingRetour = opbarenResult.OpbaringKledingRetour;
                OverledeneOpbarenModel.OpbaringSieraden = opbarenResult.OpbaringSieraden;
                OverledeneOpbarenModel.OpbaringSieradenOmschrijving = opbarenResult.OpbaringSieradenOmschrijving;
                OverledeneOpbarenModel.OpbaringSieradenRetour = opbarenResult.OpbaringSieradenRetour;
                OverledeneOpbarenModel.OpbaringExtraInfo = opbarenResult.OpbaringExtraInfo;

                _originalOverledeneOpbarenModel = new OverledeneOpbarenModel
                {
                    OpbaringId = OverledeneOpbarenModel.OpbaringId,
                    UitvaartId = OverledeneOpbarenModel.UitvaartId,
                    OpbaringLocatie = OverledeneOpbarenModel.OpbaringLocatie,
                    OpbaringKistId = OverledeneOpbarenModel.OpbaringKistId,
                    OpbaringKistLengte = OverledeneOpbarenModel.OpbaringKistLengte,
                    OpbaringVerzorging = OverledeneOpbarenModel.OpbaringVerzorging,
                    OpbaringVerzorgingJson = OverledeneOpbarenModel.OpbaringVerzorgingJson,
                    OpbaringKoeling = OverledeneOpbarenModel.OpbaringKoeling,
                    OpbaringKledingMee = OverledeneOpbarenModel.OpbaringKledingMee,
                    OpbaringKledingRetour = OverledeneOpbarenModel.OpbaringKledingRetour,
                    OpbaringSieraden = OverledeneOpbarenModel.OpbaringSieraden,
                    OpbaringSieradenOmschrijving = OverledeneOpbarenModel.OpbaringSieradenOmschrijving,
                    OpbaringSieradenRetour = OverledeneOpbarenModel.OpbaringSieradenRetour,
                    OpbaringExtraInfo = OverledeneOpbarenModel.OpbaringExtraInfo
                };

                InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
                InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;

                int verzogingCount = 0;
                if (Verzorging.VerzorgersData.Count < 3)
                {
                    Verzorgingen.Add(Verzorging);
                    foreach (var verzorgingJson in JsonConvert.DeserializeObject<List<VerzorgingData>>(OverledeneOpbarenModel.OpbaringVerzorgingJson))
                    {
                        Debug.WriteLine(verzorgingJson.WerknemerStartTijd);
                        Debug.WriteLine(verzorgingJson.WerknemerEindTijd);
                        Verzorging.VerzorgersData.Add(new VerzorgingData { WerknemerId = verzorgingJson.WerknemerId, WerknemerStartTijd = verzorgingJson.WerknemerStartTijd, WerknemerEindTijd = verzorgingJson.WerknemerEindTijd });
                        verzogingCount++;
                    }

                    while (Verzorging.VerzorgersData.Count < 3)
                    {
                        Verzorging.VerzorgersData.Add(new VerzorgingData());
                    }
                }
            }
            else
            {
                if (Verzorging.VerzorgersData.Count < 3)
                {
                    Verzorgingen.Add(Verzorging);
                    while (Verzorging.VerzorgersData.Count < 3)
                    {
                        Verzorging.VerzorgersData.Add(new VerzorgingData());
                    }
                }

            }
        }
        private bool CanExecuteSaveCommand(object obj)
        {
            return true;
        }
        private void ExecuteSaveCommand(object obj)
        {
            List<object> verzorgersDataList = new();
            foreach (var verzorging in Verzorgingen)
            {
                foreach (var verzorgingData in verzorging.VerzorgersData)
                {
                    if (verzorgingData.WerknemerId != Guid.Empty)
                    {
                        if (string.IsNullOrWhiteSpace(verzorgingData.WerknemerStartTijd))
                            verzorgingData.WerknemerStartTijd = "00:00";

                        if (string.IsNullOrWhiteSpace(verzorgingData.WerknemerEindTijd))
                            verzorgingData.WerknemerEindTijd = "00:00";

                        var verzorgingDataEntry = new
                        {
                            verzorgingData.WerknemerId,
                            verzorgingData.WerknemerStartTijd,
                            verzorgingData.WerknemerEindTijd
                        };

                        verzorgersDataList.Add(verzorgingDataEntry);
                    }
                }
            }

            OverledeneOpbarenModel.UitvaartId = Globals.UitvaartCodeGuid;
            OverledeneOpbarenModel.OpbaringVerzorgingJson = JsonConvert.SerializeObject(verzorgersDataList);

            if (!OverledeneOpbarenModel.HasData())
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            bool OpbarenInfoExists = miscellaneousRepository.UitvaartOpbarenExists(OverledeneOpbarenModel.UitvaartId);

            if (OverledeneOpbarenModel.OpbaringId == Guid.Empty && !OpbarenInfoExists)
            {
                OverledeneOpbarenModel.OpbaringId = Guid.NewGuid();
                OverledeneOpbarenModel.UitvaartId = Globals.UitvaartCodeGuid;

                try
                {
                    createRepository.AddOpbaren(OverledeneOpbarenModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error insert opbaren: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (OpbarenInfoExists)
            {
                bool opbarenInfoChanged = modelCompare.AreValuesEqual(_originalOverledeneOpbarenModel, OverledeneOpbarenModel);

                if (opbarenInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditOpbaren(OverledeneOpbarenModel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating opbaren: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalOverledeneOpbarenModel = new OverledeneOpbarenModel
                    {
                        OpbaringId = OverledeneOpbarenModel.OpbaringId,
                        UitvaartId = OverledeneOpbarenModel.UitvaartId,
                        OpbaringLocatie = OverledeneOpbarenModel.OpbaringLocatie,
                        OpbaringKistId = OverledeneOpbarenModel.OpbaringKistId,
                        OpbaringKistLengte = OverledeneOpbarenModel.OpbaringKistLengte,
                        OpbaringVerzorging = OverledeneOpbarenModel.OpbaringVerzorging,
                        OpbaringVerzorgingJson = OverledeneOpbarenModel.OpbaringVerzorgingJson,
                        OpbaringKoeling = OverledeneOpbarenModel.OpbaringKoeling,
                        OpbaringKledingMee = OverledeneOpbarenModel.OpbaringKledingMee,
                        OpbaringKledingRetour = OverledeneOpbarenModel.OpbaringKledingRetour,
                        OpbaringSieraden = OverledeneOpbarenModel.OpbaringSieraden,
                        OpbaringSieradenOmschrijving = OverledeneOpbarenModel.OpbaringSieradenOmschrijving,
                        OpbaringSieradenRetour = OverledeneOpbarenModel.OpbaringSieradenRetour
                    };
                }
            }
            if (obj != null && obj.ToString() == "VolgendeButton")
            {
                UitvaartInstance.RequestedDossierInformationBasedOnUitvaartId(Globals.UitvaartCode);
                IntAggregator.Transmit(5);
            }
        }
        private void ExecutePreviousCommand(object obj)
        {
            bool opbarenInfoChanged = modelCompare.AreValuesEqual(_originalOverledeneOpbarenModel, OverledeneOpbarenModel);

            if (OverledeneOpbarenModel.HasData())
            {
                if (!opbarenInfoChanged)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Onopgeslagen informatie", "Je hebt onopgeslagen informatie!", "Als je nu teruggaat dan verlies je de niet opgelsagen informatie.", "Begrepen", "Blijven");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IntAggregator.Transmit(3);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        return;
                    }
                }
                else
                {
                    IntAggregator.Transmit(3);
                }
            }
            else
            {
                IntAggregator.Transmit(3);
            }
        }
    }
    public class VerzorgingClass : ViewModelBase
    {
        public ObservableCollection<VerzorgingData> VerzorgersData { get; }
        public ObservableCollection<VerzorgendPersoneel> VerzorgersPersonen { get; }
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        public VerzorgingClass()
        {
            VerzorgersData = new ObservableCollection<VerzorgingData>();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            VerzorgersPersonen = new ObservableCollection<VerzorgendPersoneel>();

            foreach (var el in miscellaneousRepository.GetVerzorgers())
            {
                if (!el.IsDeleted && el.IsOpbaren)
                    VerzorgersPersonen.Add(new VerzorgendPersoneel { Id = el.Id, VerzorgendPersoon = el.VerzorgendPersoon });
            }
        }

    }
    public class VerzorgingElement
    {
        public List<VerzorgingData> VerzorgersData { get; set; }
    }

}