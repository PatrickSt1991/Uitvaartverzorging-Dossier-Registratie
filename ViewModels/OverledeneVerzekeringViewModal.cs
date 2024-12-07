using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Dossier_Registratie.ViewModels.OverledeneOpbarenViewModel;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneVerzekeringViewModal : ViewModelBase
    {
        public ObservableCollection<Insurance> Insurances { get; }

        private string? _verzekeringProperties;
        private string? _uitvaartLeider;
        private string? _uitvaartNummer;
        private bool _correctAccessOrNotCompleted = true;

        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private OverledeneVerzekeringModel? _verzekeringModel;
        private ObservableCollection<VerzekeraarsModel> _verzekeraars;
        private OverledeneUitvaartleiderModel _uitvaartLeiderModel;
        private VerzekeraarsModel? _verzekeraar;
        private ModelCompare modelCompare;

        private Polis? _polistList;
        public static bool IsPopulated = false;
        public bool CorrectAccessOrNotCompleted
        {
            get { return _correctAccessOrNotCompleted; }
            set
            {
                _correctAccessOrNotCompleted = value;
                OnPropertyChanged(nameof(CorrectAccessOrNotCompleted));
            }
        }

        public VerzekeraarsModel Verzekeraar
        {
            get { return _verzekeraar; }
            set { _verzekeraar = value; OnPropertyChanged(nameof(Verzekeraar)); }
        }
        public OverledeneVerzekeringModel VerzekeringModel
        {
            get { return _verzekeringModel; }
            set { _verzekeringModel = value; OnPropertyChanged(nameof(VerzekeringModel)); }
        }
        private OverledeneVerzekeringModel _originalVerzkeringModel;
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
        public ObservableCollection<VerzekeraarsModel> Verzekeraars
        {
            get { return _verzekeraars; }
            set
            {
                _verzekeraars = value;
                OnPropertyChanged(nameof(Verzekeraars));
            }
        }
        public Polis PolisList
        {
            get { return _polistList; }
            set { _polistList = value; OnPropertyChanged(nameof(PolisList)); }
        }
        public string VerzekeringProperties
        {
            get => _verzekeringProperties;
            set
            {
                _verzekeringProperties = value;
                OnPropertyChanged(nameof(VerzekeringProperties));
            }
        }
        public string UitvaartLeider
        {
            get => _uitvaartLeider;
            set
            {
                _uitvaartLeider = value;
                OnPropertyChanged(nameof(UitvaartLeider));
            }
        }
        public string UitvaartNummer
        {
            get => _uitvaartNummer;
            set
            {
                _uitvaartNummer = value;
                OnPropertyChanged(nameof(UitvaartNummer));
            }
        }
        public ICommand SaveCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand NewPolisCommand { get; }
        public OverledeneVerzekeringViewModal()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            modelCompare = new ModelCompare();
            Insurances = new ObservableCollection<Insurance>();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            VerzekeringModel = new OverledeneVerzekeringModel();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            Verzekeraar = new VerzekeraarsModel();
            Verzekeraars = new ObservableCollection<VerzekeraarsModel>();

            foreach (var el in miscellaneousRepository.GetVerzekeraars())
            {
                if (el.IsDeleted == false && el.IsVerzekeraar == true)
                    Verzekeraars.Add(new VerzekeraarsModel { Id = el.Id, Name = el.Name });
            }


            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
        }

        public void ReloadDynamicElements()
        {
            foreach (var el in miscellaneousRepository.GetVerzekeraars())
            {
                if (!Verzekeraars.Any(u => u.Id == el.Id) && el.IsDeleted == false && el.IsVerzekeraar == true)
                    Verzekeraars.Add(new VerzekeraarsModel { Id = el.Id, Name = el.Name });
            }
        }
        public void CreateNewDossier()
        {
            modelCompare = new ModelCompare();
            Insurances.Clear();
            VerzekeringModel = new OverledeneVerzekeringModel();

            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;
        }
        public static OverledeneVerzekeringViewModal VerzekeringInstance { get; } = new();
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            Insurances.Clear(); // Clear the Insurances collection
            VerzekeringModel = new OverledeneVerzekeringModel();

            var UitvaarLeiderResult = searchRepository.GetUitvaarleiderByUitvaartId(uitvaartNummer);
            if (UitvaarLeiderResult != null)
            {
                InfoUitvaartleider.Uitvaartnummer = UitvaarLeiderResult.Uitvaartnummer;
                InfoUitvaartleider.PersoneelNaam = UitvaarLeiderResult.PersoneelNaam;

                Globals.UitvaartCode = UitvaarLeiderResult.Uitvaartnummer;
                Globals.UitvaartCodeGuid = UitvaarLeiderResult.UitvaartId;
                Globals.UitvaarLeider = UitvaarLeiderResult.PersoneelNaam;
            }

            var verzekeringResult = searchRepository.GetOverlijdenVerzekeringByUitvaartId(uitvaartNummer);

            if (verzekeringResult != null && verzekeringResult.VerzekeringProperties.Length > 2)
            {
                // Populate insurance from data
                VerzekeringModel.UitvaartId = verzekeringResult.UitvaartId;
                VerzekeringModel.Id = verzekeringResult.Id;
                VerzekeringModel.VerzekeringProperties = verzekeringResult.VerzekeringProperties;

                _originalVerzkeringModel = new OverledeneVerzekeringModel
                {
                    Id = VerzekeringModel.Id,
                    UitvaartId = VerzekeringModel.UitvaartId,
                    VerzekeringProperties = VerzekeringModel.VerzekeringProperties
                };

                foreach (var verzekering in JsonConvert.DeserializeObject<List<PolisVerzekering>>(verzekeringResult.VerzekeringProperties))
                {
                    var insurance = new Insurance();
                    Insurances.Add(insurance);
                    insurance.Name = verzekering.VerzekeringName;

                    insurance.NameList.Add(new PolisVerzekering { VerzekeringName = verzekering.VerzekeringName });

                    foreach (var polisInfo in verzekering.PolisInfoList)
                    {
                        insurance.PolisList.Add(new Polis { PolisBedrag = polisInfo.PolisBedrag, PolisNr = polisInfo.PolisNr });
                    }
                }

                UitvaartLeider = Globals.UitvaarLeider;
                UitvaartNummer = Globals.UitvaartCode;
            }
            else
            {
                AddEmptyInsurances();
            }
        }
        private void AddEmptyInsurances()
        {
            for (int i = 0; i < 5; i++)
            {
                var insurance = new Insurance();
                Insurances.Add(insurance);

                for (int j = 0; j < 5; j++)
                    insurance.PolisList.Add(new Polis());
            }
            IsPopulated = true;
        }
        private void ExecutePreviousCommand(object obj)
        {
            bool verzekeringInfoChanged = modelCompare.AreValuesEqual(_originalVerzkeringModel, VerzekeringModel);

            if (VerzekeringModel.HasData())
            {
                if (!verzekeringInfoChanged)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Onopgeslagen informatie", "Je hebt onopgeslagen informatie!", "Als je nu teruggaat dan verlies je de niet opgelsagen informatie.", "Begrepen", "Blijven");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IntAggregator.Transmit(2);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        return;
                    }
                }
                else
                {
                    IntAggregator.Transmit(2);
                }
            }
            else
            {
                IntAggregator.Transmit(2);
            }
        }
        private bool CanExecuteSaveCommand(object obj)
        {
            return true;
        }
        private void ExecuteSaveCommand(object obj)
        {
            VerzekeringModel.VerzekeringProperties = string.Empty;

            List<object> insuranceData = new List<object>();
            foreach (var insurance in Insurances)
            {
                var polisInfoList = insurance.PolisList.Select(polis => new
                {
                    polis.PolisNr,
                    polis.PolisBedrag
                }).ToList();

                var insuranceEntry = new
                {
                    VerzekeringName = insurance.Name,
                    PolisInfoList = polisInfoList
                };

                insuranceData.Add(insuranceEntry);
            }

            VerzekeringModel.UitvaartId = Globals.UitvaartCodeGuid;
            VerzekeringModel.VerzekeringProperties = JsonConvert.SerializeObject(insuranceData);

            bool VerzekeringInfoExists = miscellaneousRepository.UitvaartVerzekeringExists(VerzekeringModel.UitvaartId);

            if (VerzekeringModel.Id == Guid.Empty && !VerzekeringInfoExists)
            {
                VerzekeringModel.Id = Guid.NewGuid();
                try
                {
                    createRepository.AddVerzekering(VerzekeringModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error insert verzekering: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (VerzekeringInfoExists)
            {
                bool verzekeringInfoChanged = modelCompare.AreValuesEqual(_originalVerzkeringModel, VerzekeringModel);

                if (verzekeringInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditVerzekering(VerzekeringModel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating verzekering: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalVerzkeringModel = new OverledeneVerzekeringModel
                    {
                        Id = VerzekeringModel.Id,
                        UitvaartId = VerzekeringModel.UitvaartId,
                        VerzekeringProperties = VerzekeringModel.VerzekeringProperties
                    };
                }
            }
            if (obj != null && obj.ToString() == "VolgendeButton")
            {
                OpbarenInstance.RequestedDossierInformationBasedOnUitvaartId(InfoUitvaartleider.Uitvaartnummer);
                IntAggregator.Transmit(4);
            }
        }
    }
    public class Insurance : ViewModelBase
    {
        public ObservableCollection<Polis> PolisList { get; }
        public ObservableCollection<PolisVerzekering> NameList { get; }
        public ObservableCollection<VerzekeraarsModel> Verzekeraars { get; }
        public ICommand AddNewEntryCommand { get; }
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }
        public Insurance()
        {
            NameList = new ObservableCollection<PolisVerzekering>();
            PolisList = new ObservableCollection<Polis>();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            Verzekeraars = new ObservableCollection<VerzekeraarsModel>();

            foreach (var el in miscellaneousRepository.GetVerzekeraars())
            {
                if (el.IsDeleted == false && el.IsVerzekeraar == true)
                    Verzekeraars.Add(new VerzekeraarsModel { Id = el.Id, Name = el.Name });
            }

            AddNewEntryCommand = new ViewModelCommand(ExecuteAddNewEntry);
        }
        private void ExecuteAddNewEntry(object obj)
        {
            PolisList.Add(new Polis());
        }
    }
}