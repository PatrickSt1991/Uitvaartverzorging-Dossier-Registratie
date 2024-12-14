using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using static Dossier_Registratie.ViewModels.OverledeneViewModel;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationUitvaartOverzichtViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private ObservableCollection<UitvaartOverzichtModel> _uitvaartOverzicht;
        private List<UitvaartOverzichtModel> _allItems;
        private string _selectedVoorregeling;
        private string _selectedVoornaam;
        private string _searchAchternaam;
        private int _selectedYear;
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
        public string SelectedVoorregeling
        {
            get { return _selectedVoorregeling; }
            set
            {
                _selectedVoorregeling = value;
                OnPropertyChanged(nameof(SelectedVoorregeling));
                ApplyFilters();
            }
        }
        public string SelectedVoornaam
        {
            get { return _selectedVoornaam; }
            set
            {
                _selectedVoornaam = value;
                OnPropertyChanged(nameof(SelectedVoornaam));
                ApplyFilters();
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
        public int SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
                ApplyFilters();
            }
        }
        public ObservableCollection<int> AvailableYears { get; set; }
        public ICommand OpenDossierViaOverzichtCommand { get; }
        public ICommand SearchAchternaamCommand { get; }
        public ICommand ClearFilterCommand { get; }

        public ConfigurationUitvaartOverzichtViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            UitvaartOverzicht = new ObservableCollection<UitvaartOverzichtModel>();
            _allItems = new List<UitvaartOverzichtModel>();

            OpenDossierViaOverzichtCommand = new ViewModelCommand(ExecuteOpenDossierViaOverzichtCommand);
            SearchAchternaamCommand = new ViewModelCommand(ExecuteSearchAchternaamCommand);
            ClearFilterCommand = new ViewModelCommand(ExecuteClearFilterCommand);

            LoadAllItems();
            //GetAllUitvaartItems();
        }
        public static ConfigurationUitvaartOverzichtViewModel OverzichtInstance { get; } = new();
        public void LoadAllItems()
        {
            _allItems = miscellaneousRepository.GetUitvaartOverzicht().ToList();
            var currentYear = DateTime.Now.Year;
            AvailableYears = new ObservableCollection<int>(
            _allItems.Where(item => item.DatumOverlijden.HasValue || item.Voorregeling)
                     .Select(item => item.DatumOverlijden.HasValue ? item.DatumOverlijden.Value.Year : DateTime.MinValue.Year)
                     .Distinct()
                     .OrderByDescending(year => year)
            );
            
            SelectedYear = currentYear;
           
            ApplyFilters();
        }
        public void ApplyFilters()
        {
            UitvaartOverzicht.Clear();

            var filteredItems = _allItems.Where(item =>
                ((item.DatumOverlijden?.Year == SelectedYear || item.DatumOverlijden?.Year == SelectedYear - 1) || (item.DatumOverlijden == null))
                && (string.IsNullOrEmpty(SelectedVoorregeling) || item.VoornaamOverledene.StartsWith(SelectedVoorregeling, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrEmpty(SelectedVoornaam) || item.VoornaamOverledene.StartsWith(SelectedVoornaam, StringComparison.OrdinalIgnoreCase))
                &&(string.IsNullOrEmpty(SearchAchternaam) || item.AchternaamOverledene.Contains(SearchAchternaam, StringComparison.OrdinalIgnoreCase))
            ).ToList();


            // Assuming filteredItems is already created
            foreach (var item in filteredItems.OrderByDescending(item => item.DatumOverlijden ?? DateTime.MinValue)) 
                UitvaartOverzicht.Add(item);

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
            ApplyFilters();
        }
        public void ExecuteClearFilterCommand(object obj)
        {
            SelectedYear = DateTime.Now.Year;
            SelectedVoorregeling = string.Empty;
            SelectedVoornaam = string.Empty;
            SearchAchternaam = string.Empty;
            //GetAllUitvaartItems();
            ApplyFilters();
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
