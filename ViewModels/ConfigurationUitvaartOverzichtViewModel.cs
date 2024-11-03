using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static Dossier_Registratie.MainWindow;
using static Dossier_Registratie.ViewModels.OverledeneViewModel;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationUitvaartOverzichtViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private ObservableCollection<UitvaartOverzichtModel> _uitvaartOverzicht;
        private string _selectedVoorregeling;
        private string _selectedVoornaam;
        private string _searchAchternaam;
        public string SelectedVoorregeling
        {
            get { return _selectedVoorregeling; }
            set
            {
                _selectedVoorregeling = value;
                OnPropertyChanged(nameof(SelectedVoorregeling));
                UpdateUitvaartItemsVoorregeling(SelectedVoorregeling);
            }
        }
        public string SelectedVoornaam
        {
            get { return _selectedVoornaam; }
            set
            {
                _selectedVoornaam = value;
                OnPropertyChanged(nameof(SelectedVoornaam));
                UpdateUitvaartItemsVoornaam(SelectedVoornaam);
            }
        }
        public string SearchAchternaam
        {
            get { return _searchAchternaam; }
            set
            {
                _searchAchternaam = value;
                OnPropertyChanged(nameof(SearchAchternaam));
            }
        }
        public ICommand OpenDossierViaOverzichtCommand { get; }
        public ICommand SearchAchternaamCommand { get; }
        public ICommand clearFilterCommand { get; }
        public ObservableCollection<UitvaartOverzichtModel> UitvaartOverzicht
        {
            get { return _uitvaartOverzicht; }
            set
            {
                if (_uitvaartOverzicht != value)
                {
                    _uitvaartOverzicht = value;
                    OnPropertyChanged(nameof(UitvaartOverzicht));
                }
            }
        }

        public ConfigurationUitvaartOverzichtViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            UitvaartOverzicht = new ObservableCollection<UitvaartOverzichtModel>();

            OpenDossierViaOverzichtCommand = new ViewModelCommand(ExecuteOpenDossierViaOverzichtCommand);
            SearchAchternaamCommand = new ViewModelCommand(ExecuteSearchAchternaamCommand);
            clearFilterCommand = new ViewModelCommand(ExecuteClearFilterCommand);
            GetAllUitvaartItems();
        }
        public void GetAllUitvaartItems()
        {
            UitvaartOverzicht.Clear();
            foreach (var overzichtItem in miscellaneousRepository.GetUitvaartOverzicht())
            {
                UitvaartOverzicht.Add(new UitvaartOverzichtModel()
                {
                    UitvaartId = overzichtItem.UitvaartId,
                    UitvaartNr = overzichtItem.UitvaartNr,
                    AchternaamOverledene = overzichtItem.AchternaamOverledene,
                    VoornaamOverledene = overzichtItem.VoornaamOverledene,
                    DatumOverlijden = overzichtItem.DatumOverlijden,
                    UitvaartLeider = overzichtItem.UitvaartLeider,
                    Voorregeling = overzichtItem.Voorregeling
                });
            }
        }
        public void UpdateUitvaartItemsVoorregeling(string letterElement)
        {
            UitvaartOverzicht.Clear();
            foreach (var overzichtItem in miscellaneousRepository.GetUitvaartOverzicht())
            {
                if (overzichtItem.Voorregeling && overzichtItem.VoornaamOverledene.StartsWith(letterElement, StringComparison.OrdinalIgnoreCase))
                {
                    UitvaartOverzicht.Add(new UitvaartOverzichtModel()
                    {
                        UitvaartId = overzichtItem.UitvaartId,
                        UitvaartNr = overzichtItem.UitvaartNr,
                        AchternaamOverledene = overzichtItem.AchternaamOverledene,
                        VoornaamOverledene = overzichtItem.VoornaamOverledene,
                        DatumOverlijden = overzichtItem.DatumOverlijden,
                        UitvaartLeider = overzichtItem.UitvaartLeider,
                        Voorregeling = overzichtItem.Voorregeling
                    });
                }
            }
        }
        public void UpdateUitvaartItemsVoornaam(string letterElement)
        {
            UitvaartOverzicht.Clear();
            foreach (var overzichtItem in miscellaneousRepository.GetUitvaartOverzicht())
            {
                if (overzichtItem.VoornaamOverledene.StartsWith(letterElement, StringComparison.OrdinalIgnoreCase))
                {
                    UitvaartOverzicht.Add(new UitvaartOverzichtModel()
                    {
                        UitvaartId = overzichtItem.UitvaartId,
                        UitvaartNr = overzichtItem.UitvaartNr,
                        AchternaamOverledene = overzichtItem.AchternaamOverledene,
                        VoornaamOverledene = overzichtItem.VoornaamOverledene,
                        DatumOverlijden = overzichtItem.DatumOverlijden,
                        UitvaartLeider = overzichtItem.UitvaartLeider,
                        Voorregeling = overzichtItem.Voorregeling
                    });
                }
            }
        }
        public void ExecuteSearchAchternaamCommand(object obj)
        {
            UitvaartOverzicht.Clear();
            foreach (var overzichtItem in miscellaneousRepository.GetUitvaartOverzicht())
            {
                if (overzichtItem.AchternaamOverledene.Contains(SearchAchternaam, StringComparison.OrdinalIgnoreCase))
                {
                    UitvaartOverzicht.Add(new UitvaartOverzichtModel()
                    {
                        UitvaartId = overzichtItem.UitvaartId,
                        UitvaartNr = overzichtItem.UitvaartNr,
                        AchternaamOverledene = overzichtItem.AchternaamOverledene,
                        VoornaamOverledene = overzichtItem.VoornaamOverledene,
                        DatumOverlijden = overzichtItem.DatumOverlijden,
                        UitvaartLeider = overzichtItem.UitvaartLeider,
                        Voorregeling = overzichtItem.Voorregeling
                    });
                }
            }
        }
        public void ExecuteClearFilterCommand(object obj)
        {
            SelectedVoorregeling = string.Empty;
            SelectedVoornaam = string.Empty;
            SearchAchternaam = string.Empty;
            GetAllUitvaartItems();
        }
        public void ExecuteOpenDossierViaOverzichtCommand(object obj)
        {
            ComboAggregator.Transmit();
            Globals.NewDossierCreation = false;
            Globals.UitvaartCodeGuid = miscellaneousRepository.GetUitvaartGuid(obj.ToString());
            Instance.RequestedDossierInformationBasedOnUitvaartId(obj.ToString());
            IntAggregator.Transmit(666);
        }
    }
}
