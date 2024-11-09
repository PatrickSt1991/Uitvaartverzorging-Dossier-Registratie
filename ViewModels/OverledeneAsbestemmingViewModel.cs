using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Dossier_Registratie.MainWindow;
using static Dossier_Registratie.ViewModels.OverledeneBijlagesViewModel;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneAsbestemmingViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;

        private OverledeneUitvaartleiderModel _uitvaartLeiderModel;
        private OverledeneAsbestemmingModel _asbestemmingModel;
        private ModelCompare modelCompare;
        private OverledeneAsbestemmingModel _originalAsbestemmingModel;
        private ObservableCollection<ConfigurationAsbestemmingModel> _asbestemmingen;

        public bool initialLoadDone;
        private bool _correctAccessOrNotCompleted = true;
        public bool CorrectAccessOrNotCompleted
        {
            get { return _correctAccessOrNotCompleted; }
            set
            {
                _correctAccessOrNotCompleted = value;
                OnPropertyChanged(nameof(CorrectAccessOrNotCompleted));
            }
        }
        public OverledeneUitvaartleiderModel InfoUitvaartleider
        {
            get { return _uitvaartLeiderModel; }
            set { _uitvaartLeiderModel = value; OnPropertyChanged(nameof(InfoUitvaartleider)); }
        }
        public OverledeneAsbestemmingModel AsbestemmingModel
        {
            get { return _asbestemmingModel; }
            set { _asbestemmingModel = value; OnPropertyChanged(nameof(AsbestemmingModel)); }
        }
        public ObservableCollection<ConfigurationAsbestemmingModel> Asbestemmingen
        {
            get { return _asbestemmingen; }
            set
            {
                _asbestemmingen = value;
                OnPropertyChanged(nameof(Asbestemmingen));
            }
        }
        public ICommand SaveCommand { get; }
        public ICommand PreviousCommand { get; }
        private OverledeneAsbestemmingViewModel()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            AsbestemmingModel = new OverledeneAsbestemmingModel();
            Asbestemmingen = new ObservableCollection<ConfigurationAsbestemmingModel>();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();

            foreach (var el in miscellaneousRepository.GetAsbestemmingen())
            {
                if (!el.IsDeleted)
                {
                    Asbestemmingen.Add(new ConfigurationAsbestemmingModel { AsbestemmingId = el.AsbestemmingId, AsbestemmingOmschrijving = el.AsbestemmingOmschrijving });
                }
            }

            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
        }
        public void ReloadDynamicElements()
        {
            foreach (var el in miscellaneousRepository.GetAsbestemmingen())
            {
                if (!Asbestemmingen.Any(u => u.AsbestemmingId == el.AsbestemmingId) && el.IsDeleted == false)
                {
                    Asbestemmingen.Add(new ConfigurationAsbestemmingModel { AsbestemmingId = el.AsbestemmingId, AsbestemmingOmschrijving = el.AsbestemmingOmschrijving });
                }
            }
        }
        public void CreateNewDossier()
        {
            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            AsbestemmingModel = new OverledeneAsbestemmingModel();
        }
        public static OverledeneAsbestemmingViewModel AsbestemmingInstance { get; } = new();
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            modelCompare = new ModelCompare();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            AsbestemmingModel = new OverledeneAsbestemmingModel();

            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;

            var asbestemmingResult = searchRepository.GetOverlijdenAsbestemmingInfoByUitvaartId(uitvaartNummer);
            if (asbestemmingResult != null)
            {
                InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
                InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;

                AsbestemmingModel.AsbestemmingId = asbestemmingResult.AsbestemmingId;
                AsbestemmingModel.UitvaartId = asbestemmingResult.UitvaartId;
                AsbestemmingModel.Asbestemming = asbestemmingResult.Asbestemming;
                AsbestemmingModel.TypeGraf = asbestemmingResult.TypeGraf;
                AsbestemmingModel.BestaandGraf = asbestemmingResult.BestaandGraf;
                AsbestemmingModel.ZandKelderGraf = asbestemmingResult.ZandKelderGraf;
                AsbestemmingModel.GrafMonument = asbestemmingResult.GrafMonument;

                _originalAsbestemmingModel = new OverledeneAsbestemmingModel
                {
                    AsbestemmingId = AsbestemmingModel.AsbestemmingId,
                    UitvaartId = AsbestemmingModel.UitvaartId,
                    Asbestemming = AsbestemmingModel.Asbestemming,
                    TypeGraf = AsbestemmingModel.TypeGraf,
                    BestaandGraf = AsbestemmingModel.BestaandGraf,
                    ZandKelderGraf = AsbestemmingModel.ZandKelderGraf,
                    GrafMonument = AsbestemmingModel.GrafMonument
                };
            }
        }
        public bool CanExecuteSaveCommand(object obj)
        {
            return true;
        }
        public void ExecuteSaveCommand(object obj)
        {
            AsbestemmingModel.UitvaartId = Globals.UitvaartCodeGuid;

            bool UitvaartAsbestemmingExists = miscellaneousRepository.UitvaarAsbestemmingExists(AsbestemmingModel.UitvaartId);

            if (AsbestemmingModel.AsbestemmingId == Guid.Empty && !UitvaartAsbestemmingExists)
            {
                AsbestemmingModel.AsbestemmingId = Guid.NewGuid();
                try
                {
                    createRepository.AddAsbestemming(AsbestemmingModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error insert asbestemming: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (UitvaartAsbestemmingExists)
            {
                bool asbestemmingInfoChanged = modelCompare.AreValuesEqual(_originalAsbestemmingModel, AsbestemmingModel);

                if (!asbestemmingInfoChanged)
                {
                    try
                    {
                        updateRepository.EditAsbestemming(AsbestemmingModel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating asbestemming: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalAsbestemmingModel = new OverledeneAsbestemmingModel
                    {
                        AsbestemmingId = AsbestemmingModel.AsbestemmingId,
                        UitvaartId = AsbestemmingModel.UitvaartId,
                        Asbestemming = AsbestemmingModel.Asbestemming,
                        TypeGraf = AsbestemmingModel.TypeGraf,
                        BestaandGraf = AsbestemmingModel.BestaandGraf,
                        ZandKelderGraf = AsbestemmingModel.ZandKelderGraf,
                        GrafMonument = AsbestemmingModel.GrafMonument
                    };
                }
            }

            if (obj != null && obj.ToString() == "VolgendeButton")
            {
                BijlagesInstance.RequestedDossierInformationBasedOnUitvaartId(InfoUitvaartleider.Uitvaartnummer);
                IntAggregator.Transmit(7);
            }
        }
        private void ExecutePreviousCommand(object obj)
        {
            bool asbestemmingInfoChanged = modelCompare.AreValuesEqual(_originalAsbestemmingModel, AsbestemmingModel);

            if (AsbestemmingModel.HasData())
            {
                if (!asbestemmingInfoChanged)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Onopgeslagen informatie", "Je hebt onopgeslagen informatie!", "Als je nu teruggaat dan verlies je de niet opgelsagen informatie.", "Begrepen", "Blijven");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IntAggregator.Transmit(5);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        return;
                    }
                }
                else
                {
                    IntAggregator.Transmit(5);
                }
            }
            else
            {
                IntAggregator.Transmit(5);
            }
        }
    }
}
