using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using Dossier_Registratie.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static Dossier_Registratie.ViewModels.OverledeneAsbestemmingViewModel;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneUitvaartViewModel : ViewModelBase
    {
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;

        private OverledeneUitvaartleiderModel _uitvaartLeiderModel;
        private OverledeneMiscModel _overledeneMisc;
        private OverledeneUitvaartModel _overledeneUitvaartModel;
        private ObservableCollection<OverledeneRouwbrieven> _rouwbrievenData;
        private ModelCompare modelCompare;
        private OverledeneUitvaartModel _originalOverledeneUitvaartModel;
        private OverledeneMiscModel _originalOverledeneMisc;
        private bool _correctAccessOrNotCompleted = true;
        public bool initialLoadDone;
        private Visibility isBegrafenisVisable = Visibility.Collapsed;
        public bool CorrectAccessOrNotCompleted
        {
            get { return _correctAccessOrNotCompleted; }
            set
            {
                _correctAccessOrNotCompleted = value;
                OnPropertyChanged(nameof(CorrectAccessOrNotCompleted));
            }
        }
        public Visibility IsBegrafenisVisable
        {
            get { return isBegrafenisVisable; }
            set
            {
                isBegrafenisVisable = value;
                OnPropertyChanged(nameof(IsBegrafenisVisable));
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
        public OverledeneMiscModel OverledeneMisc
        {
            get { return _overledeneMisc; }
            set { _overledeneMisc = value; OnPropertyChanged(nameof(OverledeneMisc)); }
        }
        public OverledeneUitvaartModel OverledeneUitvaartModel
        {
            get
            {
                return _overledeneUitvaartModel;
            }
            set
            {
                _overledeneUitvaartModel = value;
                OnPropertyChanged(nameof(OverledeneUitvaartModel));
            }
        }
        public ObservableCollection<OverledeneRouwbrieven> RouwbrievenData
        {
            get { return _rouwbrievenData; }
            set
            {
                _rouwbrievenData = value;
                OnPropertyChanged(nameof(RouwbrievenData));
            }
        }
        public ICommand SaveCommand { get; }
        public ICommand PreviousCommand { get; }
        private OverledeneUitvaartViewModel()
        {
            if (Globals.DossierCompleted || Globals.PermissionLevelName == "Gebruiker")
                CorrectAccessOrNotCompleted = false;

            modelCompare = new ModelCompare();
            OverledeneUitvaartModel = new OverledeneUitvaartModel();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            OverledeneMisc = new OverledeneMiscModel();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            RouwbrievenData = new ObservableCollection<OverledeneRouwbrieven>();

            RouwbrievenData.Clear();

            foreach (var el in miscellaneousRepository.GetAdvertenties())
            {
                if (el.IsDeleted == false)
                    RouwbrievenData.Add(new OverledeneRouwbrieven { RouwbrievenId = el.RouwbrievenId, RouwbrievenName = el.RouwbrievenName });
            }

            SaveCommand = new ViewModelCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            PreviousCommand = new ViewModelCommand(ExecutePreviousCommand);
        }
        public void RefresDynamicElements()
        {
            foreach (var el in miscellaneousRepository.GetAdvertenties())
            {
                if (!RouwbrievenData.Any(u => u.RouwbrievenId == el.RouwbrievenId))
                    RouwbrievenData.Add(new OverledeneRouwbrieven { RouwbrievenId = el.RouwbrievenId, RouwbrievenName = el.RouwbrievenName });
            }
        }
        public void CreateNewDossier()
        {
            modelCompare = new ModelCompare();
            OverledeneUitvaartModel = new OverledeneUitvaartModel();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            OverledeneMisc = new OverledeneMiscModel();
        }
        public static OverledeneUitvaartViewModel UitvaartInstance { get; } = new();
        public void RequestedDossierInformationBasedOnUitvaartId(string uitvaartNummer)
        {
            modelCompare = new ModelCompare();
            OverledeneUitvaartModel = new OverledeneUitvaartModel();
            InfoUitvaartleider = new OverledeneUitvaartleiderModel();
            OverledeneMisc = new OverledeneMiscModel();

            InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
            InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;

            var uitvaartResult = searchRepository.GetOverlijdenUitvaartInfoByUitvaartId(uitvaartNummer);
            if (uitvaartResult != null)
            {
                OverledeneUitvaartModel.Id = uitvaartResult.Id;
                OverledeneUitvaartModel.UitvaartId = uitvaartResult.UitvaartId;
                OverledeneUitvaartModel.DatumTijdCondoleance = uitvaartResult.DatumTijdCondoleance;
                OverledeneUitvaartModel.ConsumptiesCondoleance = uitvaartResult.ConsumptiesCondoleance;
                OverledeneUitvaartModel.TypeDienst = uitvaartResult.TypeDienst;
                OverledeneUitvaartModel.VolgAutoDienst = uitvaartResult.VolgAutoDienst.Trim();
                OverledeneUitvaartModel.DatumTijdUitvaart = uitvaartResult.DatumTijdUitvaart;
                OverledeneUitvaartModel.LocatieUitvaart = uitvaartResult.LocatieUitvaart;
                OverledeneUitvaartModel.DatumTijdDienst = uitvaartResult.DatumTijdDienst;
                OverledeneUitvaartModel.LocatieDienst = uitvaartResult.LocatieDienst;
                OverledeneUitvaartModel.AfscheidDienst = uitvaartResult.AfscheidDienst;
                OverledeneUitvaartModel.MuziekDienst = uitvaartResult.MuziekDienst;
                OverledeneUitvaartModel.BeslotenDienst = uitvaartResult.BeslotenDienst;
                OverledeneUitvaartModel.ConsumptiesDienst = uitvaartResult.ConsumptiesDienst;
                OverledeneUitvaartModel.KistDienst = uitvaartResult.KistDienst;
                OverledeneUitvaartModel.Spreker = uitvaartResult.Spreker;
                OverledeneUitvaartModel.PowerPoint = OverledeneUitvaartModel.PowerPoint;
                OverledeneUitvaartModel.CondoleanceYesNo = uitvaartResult.CondoleanceYesNo;
                OverledeneUitvaartModel.AantalTijdsBlokken = uitvaartResult.AantalTijdsBlokken;
                OverledeneUitvaartModel.TijdBlokken = uitvaartResult.TijdBlokken;

                if (OverledeneUitvaartModel.TypeDienst == "Begrafenis")
                    IsBegrafenisVisable = Visibility.Visible;

                _originalOverledeneUitvaartModel = new OverledeneUitvaartModel
                {
                    Id = OverledeneUitvaartModel.Id,
                    UitvaartId = OverledeneUitvaartModel.UitvaartId,
                    DatumTijdCondoleance = OverledeneUitvaartModel.DatumTijdCondoleance,
                    ConsumptiesCondoleance = OverledeneUitvaartModel.ConsumptiesCondoleance,
                    TypeDienst = OverledeneUitvaartModel.TypeDienst,
                    VolgAutoDienst = OverledeneUitvaartModel.VolgAutoDienst,
                    DatumTijdUitvaart = OverledeneUitvaartModel.DatumTijdUitvaart,
                    LocatieUitvaart = OverledeneUitvaartModel.LocatieUitvaart,
                    DatumTijdDienst = OverledeneUitvaartModel.DatumTijdDienst,
                    LocatieDienst = OverledeneUitvaartModel.LocatieDienst,
                    AfscheidDienst = OverledeneUitvaartModel.AfscheidDienst,
                    MuziekDienst = OverledeneUitvaartModel.MuziekDienst,
                    BeslotenDienst = OverledeneUitvaartModel.BeslotenDienst,
                    ConsumptiesDienst = OverledeneUitvaartModel.ConsumptiesDienst,
                    KistDienst = OverledeneUitvaartModel.KistDienst,
                    Spreker = OverledeneUitvaartModel.Spreker,
                    CondoleanceYesNo = OverledeneUitvaartModel.CondoleanceYesNo,
                    PowerPoint = OverledeneUitvaartModel.PowerPoint,
                    TijdBlokken = OverledeneUitvaartModel.TijdBlokken,
                    AantalTijdsBlokken = OverledeneUitvaartModel.AantalTijdsBlokken
                };

                InfoUitvaartleider.Uitvaartnummer = Globals.UitvaartCode;
                InfoUitvaartleider.PersoneelNaam = Globals.UitvaarLeider;
            }

            var miscResult = searchRepository.GetOverledeneMiscByUitvaartId(Globals.UitvaartCodeGuid);
            if (miscResult != null)
            {
                OverledeneMisc.Id = miscResult.Id;
                OverledeneMisc.UitvaartId = miscResult.UitvaartId;
                OverledeneMisc.RouwbrievenId = miscResult.RouwbrievenId;
                OverledeneMisc.Advertenties = miscResult.Advertenties;
                OverledeneMisc.AantalRouwbrieven = miscResult.AantalRouwbrieven;
                OverledeneMisc.AantalUitnodigingen = miscResult.AantalUitnodigingen;
                OverledeneMisc.AantalKennisgeving = miscResult.AantalKennisgeving;
                OverledeneMisc.UBS = miscResult.UBS;
                OverledeneMisc.AulaNaam = miscResult.AulaNaam;
                OverledeneMisc.AulaPersonen = miscResult.AulaPersonen;
                OverledeneMisc.GrafNummer = miscResult.GrafNummer;
                OverledeneMisc.Begraafplaats = miscResult.Begraafplaats;

                _originalOverledeneMisc = new OverledeneMiscModel
                {
                    Id = OverledeneMisc.Id,
                    UitvaartId = OverledeneMisc.UitvaartId,
                    RouwbrievenId = OverledeneMisc.RouwbrievenId,
                    Advertenties = OverledeneMisc.Advertenties,
                    AantalRouwbrieven = OverledeneMisc.AantalRouwbrieven,
                    AantalUitnodigingen = OverledeneMisc.AantalUitnodigingen,
                    AantalKennisgeving = OverledeneMisc.AantalKennisgeving,
                    UBS = OverledeneMisc.UBS,
                    AulaNaam = OverledeneMisc.AulaNaam,
                    AulaPersonen = OverledeneMisc.AulaPersonen,
                    GrafNummer = OverledeneMisc.GrafNummer,
                    Begraafplaats = OverledeneMisc.Begraafplaats
                };
            }
        }
        public bool CanExecuteSaveCommand(object obj)
        {
            return true;
        }
        public void ExecuteSaveCommand(object obj)
        {
            Globals.UitvaartType = OverledeneUitvaartModel.TypeDienst;
            if (!OverledeneUitvaartModel.HasData() || !OverledeneMisc.HasData())
            {
                new ToastWindow("Niet alle verplichte velden zijn ingevuld!").Show();
                return;
            }

            bool UitvaartInfoExists = miscellaneousRepository.UitvaarInfoExists(OverledeneUitvaartModel.UitvaartId);
            bool UitvaartInfoMiscExists = miscellaneousRepository.UitvaarInfoMiscExists(OverledeneUitvaartModel.UitvaartId);

            if (OverledeneUitvaartModel.Id == Guid.Empty && !UitvaartInfoExists)
            {
                OverledeneUitvaartModel.Id = Guid.NewGuid();
                OverledeneUitvaartModel.UitvaartId = Globals.UitvaartCodeGuid;

                try
                {
                    createRepository.AddUitvaart(OverledeneUitvaartModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error Inserting Uitvaartinfo: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (UitvaartInfoExists)
            {
                bool UitvaartInfoChanged = modelCompare.AreValuesEqual(_originalOverledeneUitvaartModel, OverledeneUitvaartModel);

                if (UitvaartInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditUitvaart(OverledeneUitvaartModel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating uitvaartinfo: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalOverledeneUitvaartModel = new OverledeneUitvaartModel
                    {
                        Id = OverledeneUitvaartModel.Id,
                        UitvaartId = OverledeneUitvaartModel.UitvaartId,
                        DatumTijdCondoleance = OverledeneUitvaartModel.DatumTijdCondoleance,
                        ConsumptiesCondoleance = OverledeneUitvaartModel.ConsumptiesCondoleance,
                        TypeDienst = OverledeneUitvaartModel.TypeDienst,
                        VolgAutoDienst = OverledeneUitvaartModel.VolgAutoDienst,
                        DatumTijdUitvaart = OverledeneUitvaartModel.DatumTijdUitvaart,
                        LocatieUitvaart = OverledeneUitvaartModel.LocatieUitvaart,
                        DatumTijdDienst = OverledeneUitvaartModel.DatumTijdDienst,
                        LocatieDienst = OverledeneUitvaartModel.LocatieDienst,
                        AfscheidDienst = OverledeneUitvaartModel.AfscheidDienst,
                        MuziekDienst = OverledeneUitvaartModel.MuziekDienst,
                        BeslotenDienst = OverledeneUitvaartModel.BeslotenDienst,
                        ConsumptiesDienst = OverledeneUitvaartModel.ConsumptiesDienst,
                        KistDienst = OverledeneUitvaartModel.KistDienst,
                        Spreker = OverledeneUitvaartModel.Spreker,
                        CondoleanceYesNo = OverledeneUitvaartModel.CondoleanceYesNo,
                        PowerPoint = OverledeneUitvaartModel.PowerPoint,
                        TijdBlokken = OverledeneUitvaartModel.TijdBlokken,
                        AantalTijdsBlokken = OverledeneUitvaartModel.AantalTijdsBlokken
                    };
                }
            }

            if (OverledeneMisc.Id == Guid.Empty && !UitvaartInfoMiscExists)
            {
                OverledeneMisc.Id = Guid.NewGuid();
                OverledeneMisc.UitvaartId = Globals.UitvaartCodeGuid;

                try
                {
                    createRepository.AddMiscUitvaart(OverledeneMisc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting UitvaartInfoMisc: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else if (UitvaartInfoMiscExists)
            {
                bool UitvaartMiscInfoChanged = modelCompare.AreValuesEqual(_originalOverledeneMisc, OverledeneMisc);

                if (UitvaartMiscInfoChanged == false)
                {
                    try
                    {
                        updateRepository.EditMiscUitvaart(OverledeneMisc);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating uitvaartMiscInfo: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    _originalOverledeneMisc = new OverledeneMiscModel
                    {
                        Id = OverledeneMisc.Id,
                        RouwbrievenId = OverledeneMisc.RouwbrievenId,
                        Advertenties = OverledeneMisc.Advertenties,
                        AantalRouwbrieven = OverledeneMisc.AantalRouwbrieven,
                        AantalUitnodigingen = OverledeneMisc.AantalUitnodigingen,
                        AantalKennisgeving = OverledeneMisc.AantalKennisgeving,
                        UBS = OverledeneMisc.UBS
                    };
                }
            }

            if (obj != null && obj.ToString() == "VolgendeButton")
            {
                AsbestemmingInstance.RequestedDossierInformationBasedOnUitvaartId(Globals.UitvaartCode);
                IntAggregator.Transmit(6);
            }
        }
        private void ExecutePreviousCommand(object obj)
        {
            bool UitvaartInfoChanged = modelCompare.AreValuesEqual(_originalOverledeneUitvaartModel, OverledeneUitvaartModel);

            if (OverledeneUitvaartModel.HasData())
            {
                if (!UitvaartInfoChanged)
                {
                    CustomMessageBox.CustomMessageBoxResult result = CustomMessageBox.Show("Onopgeslagen informatie", "Je hebt onopgeslagen informatie!", "Als je nu teruggaat dan verlies je de niet opgelsagen informatie.", "Begrepen", "Blijven");
                    if (result == CustomMessageBox.CustomMessageBoxResult.Continue)
                    {
                        IntAggregator.Transmit(4);
                    }
                    else if (result == CustomMessageBox.CustomMessageBoxResult.Stop)
                    {
                        return;
                    }
                }
                else
                {
                    IntAggregator.Transmit(4);
                }
            }
            else
            {
                IntAggregator.Transmit(4);
            }
        }
    }
}
