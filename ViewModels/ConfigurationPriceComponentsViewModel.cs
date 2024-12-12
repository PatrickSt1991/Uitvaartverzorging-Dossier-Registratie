using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationPriceComponentsViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivatePriceComponentCommand { get; }
        public ICommand EditPriceComponentsCommand { get; }
        public ICommand DisablePriceComponentsCommand { get; }
        public ICommand CloseEditPriceComponentPopupCommand { get; }
        public ICommand OpenPriceComponentPopupOpenCommand { get; }
        public ICommand SavePriceComponentCommand { get; }
        public ICommand CreateNewPriceComponentCommand { get; }
        public ICommand VerzekeringCheckCommand { get; set; }
        public ICommand RefreshPriceComponentsGridCommand { get; set; }
        public ICommand SelectAllCommand { get; }
        public ICommand DeselectAllCommand { get; }

        private bool isEditPriceComponentPopupOpen;
        private bool newPriceComponent;
        private string searchOmschrijving;

        private KostenbegrotingModel _selectedPriceComponent;
        private KostenbegrotingModel _priceComponentFilter;
        private VerzekeraarsModel _selectedPriceComponentVerzekeraar;
        private ObservableCollection<VerzekeraarsModel> _priceComponentsVerzekeraars;
        private ObservableCollection<VerzekeraarsModel> _selectedVerzekeraarsPriceComponents;
        private ObservableCollection<Guid> _selectedVerzekeringen;
        private ObservableCollection<KostenbegrotingModel> _priceComponents;
        private ObservableCollection<KostenbegrotingModel> _allPriceComponents = new ObservableCollection<KostenbegrotingModel>();
        public KostenbegrotingModel SelectedPriceComponent
        {
            get { return _selectedPriceComponent; }
            set
            {
                if (_selectedPriceComponent != value)
                {
                    _selectedPriceComponent = value;
                    OnPropertyChanged(nameof(SelectedPriceComponent));
                }
            }
        }
        public KostenbegrotingModel PriceComponentFilter
        {
            get { return _priceComponentFilter; }
            set
            {
                if (_priceComponentFilter != value)
                {
                    _priceComponentFilter = value;
                    OnPropertyChanged(nameof(PriceComponentFilter));
                }
            }
        }
        public VerzekeraarsModel SelectedPriceComponentVerzekeraar
        {
            get { return _selectedPriceComponentVerzekeraar; }
            set
            {
                if (_selectedPriceComponentVerzekeraar != value)
                {
                    _selectedPriceComponentVerzekeraar = value;
                    OnPropertyChanged(nameof(SelectedPriceComponentVerzekeraar));

                    if (PriceComponentFilter != null && _selectedPriceComponentVerzekeraar != null)
                        PriceComponentFilter.SpecificPakket = _selectedPriceComponentVerzekeraar.Pakket;

                    ExecuteFilterPriceComponentCommand();
                }
            }
        }
        public ObservableCollection<VerzekeraarsModel> PriceComponentsVerzekeraars
        {
            get { return _priceComponentsVerzekeraars; }
            set
            {
                if (_priceComponentsVerzekeraars != value)
                {
                    _priceComponentsVerzekeraars = value;
                    OnPropertyChanged(nameof(PriceComponentsVerzekeraars));
                }
            }
        }
        public ObservableCollection<VerzekeraarsModel> SelectedVerzekeraarsPriceComponents
        {
            get { return _selectedVerzekeraarsPriceComponents; }
            set
            {
                if (_selectedVerzekeraarsPriceComponents != value)
                {
                    _selectedVerzekeraarsPriceComponents = value;
                    OnPropertyChanged(nameof(SelectedVerzekeraarsPriceComponents));
                }
            }
        }
        public ObservableCollection<Guid> SelectedVerzekeringen
        {
            get { return _selectedVerzekeringen; }
            set
            {
                if(_selectedVerzekeringen != value)
                {
                    _selectedVerzekeringen = value;
                    OnPropertyChanged(nameof(SelectedVerzekeringen));
                }
            }
        }
        public ObservableCollection<KostenbegrotingModel> PriceComponents
        {
            get { return _priceComponents; }
            set
            {
                if (_priceComponents != value)
                {
                    _priceComponents = value;
                    OnPropertyChanged(nameof(PriceComponents));
                }
            }
        }
        public bool IsEditPriceComponentPopupOpen
        {
            get { return isEditPriceComponentPopupOpen; }
            set
            {
                if (isEditPriceComponentPopupOpen != value)
                {
                    isEditPriceComponentPopupOpen = value;
                    OnPropertyChanged(nameof(IsEditPriceComponentPopupOpen));
                }
            }
        }
        public string SearchOmschrijving
        {
            get { return searchOmschrijving; }
            set
            {
                if (searchOmschrijving != value)
                {
                    searchOmschrijving = value;
                    OnPropertyChanged(nameof(SearchOmschrijving));
                    FilterPriceComponentsOmschrijving();
                }
            }
        }

        public ConfigurationPriceComponentsViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedPriceComponent = new KostenbegrotingModel();
            PriceComponentFilter = new KostenbegrotingModel();

            PriceComponentsVerzekeraars = new ObservableCollection<VerzekeraarsModel>();
            SelectedVerzekeraarsPriceComponents = new ObservableCollection<VerzekeraarsModel>();
            SelectedVerzekeringen = new ObservableCollection<Guid>();
            PriceComponents = new ObservableCollection<KostenbegrotingModel>();

            ActivatePriceComponentCommand = new ViewModelCommand(ExecuteActivatePriceComponentCommand);
            EditPriceComponentsCommand = new ViewModelCommand(ExecuteEditPriceComponentsCommand);
            DisablePriceComponentsCommand = new ViewModelCommand(ExecuteDisablePriceComponentsCommand);
            OpenPriceComponentPopupOpenCommand = new RelayCommand(() => IsEditPriceComponentPopupOpen = true);
            CloseEditPriceComponentPopupCommand = new RelayCommand(() => IsEditPriceComponentPopupOpen = false);
            VerzekeringCheckCommand = new RelayCommand<VerzekeraarsModel>(OnVerzekeringCheckBoxCheckedChanged);
            RefreshPriceComponentsGridCommand = new RelayCommand(PriceComponentGridData);
            SavePriceComponentCommand = new ViewModelCommand(ExecuteSavePriceComponentCommand);
            CreateNewPriceComponentCommand = new ViewModelCommand(ExecuteCreateNewPriceComponentCommand);

            SelectAllCommand = new RelayCommand(SelectAll);
            DeselectAllCommand = new RelayCommand(DeselectAll);

            PriceComponentGridData();
        }
        private void SelectAll()
        {
            SelectedVerzekeringen.Clear();
            SelectedPriceComponent.ComponentVerzekeringJson = string.Empty;

            foreach (var item in SelectedVerzekeraarsPriceComponents)
            {
                item.IsSelected = true;
                OnVerzekeringCheckBoxCheckedChanged(item);
            }
        }
        private void DeselectAll()
        {
            SelectedVerzekeringen.Clear();
            SelectedPriceComponent.ComponentVerzekeringJson = string.Empty;

            foreach (var item in SelectedVerzekeraarsPriceComponents)
            {
                item.IsSelected = false;
                OnVerzekeringCheckBoxCheckedChanged(item);
            }
        }
        public void ExecuteActivatePriceComponentCommand(object obj)
        {
            MessageBoxResult activateQuestion = MessageBox.Show("Wil je dit prijs component activeren?", "Prijs component activeren", MessageBoxButton.YesNo);
            if (activateQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid priceComponentId))
                {
                    try
                    {
                        commandRepository.ActivatePriceComponent(priceComponentId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error activating pricecomponent: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    PriceComponentGridData();
                }
                else
                {
                    MessageBox.Show("Invalid PriceComponent Id.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ExecuteEditPriceComponentsCommand(object obj)
        {
            var priceComp = miscellaneousRepository.GetSelectedPriceComponentsBeheer((Guid)obj);

            SelectedPriceComponent.ComponentId = priceComp.ComponentId;
            SelectedPriceComponent.ComponentOmschrijving = priceComp.ComponentOmschrijving;
            SelectedPriceComponent.ComponentBedrag = priceComp.ComponentBedrag;
            SelectedPriceComponent.ComponentFactuurBedrag = priceComp.ComponentFactuurBedrag;
            SelectedPriceComponent.DefaultPM = priceComp.DefaultPM;
            SelectedPriceComponent.ComponentAantal = priceComp.ComponentAantal;
            SelectedPriceComponent.ComponentVerzekeringJson = priceComp.ComponentVerzekeringJson;
            SelectedPriceComponent.SortOrder = priceComp.SortOrder;
            
            SelectedVerzekeringen.Clear();

            if (!string.IsNullOrEmpty(priceComp.ComponentVerzekeringJson))
            {
                try
                {
                    var verzekeringIds = JsonConvert.DeserializeObject<List<VerzekeringKbModel>>(priceComp.ComponentVerzekeringJson);

                    foreach (var verzekering in verzekeringIds)
                        SelectedVerzekeringen.Add(verzekering.Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deserializing ComponentVerzekeringJson: {ex.Message}");
                }
            }


            foreach (var verzekeraar in SelectedVerzekeraarsPriceComponents)
                verzekeraar.IsSelected = SelectedVerzekeringen.Contains(verzekeraar.Id);

            newPriceComponent = false;
            IsEditPriceComponentPopupOpen = true;
        }
        public void ExecuteDisablePriceComponentsCommand(object obj)
        {
            MessageBoxResult disableQuestion = MessageBox.Show("Wil je dit prijs component deactiveren?", "Prijs component deactiveren", MessageBoxButton.YesNo);
            if (disableQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid priceComponentId))
                {
                    try
                    {
                        commandRepository.DisablePriceComponent(priceComponentId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error disabeling price component: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    PriceComponentGridData();
                }
                else
                {
                    MessageBox.Show("Invalid PriceComponent Id.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void OnVerzekeringCheckBoxCheckedChanged(VerzekeraarsModel component)
        {
            if (component.IsSelected)
            {
                if (!SelectedVerzekeringen.Contains(component.Id))
                    SelectedVerzekeringen.Add(component.Id);
            }
            else
            {
                if (SelectedVerzekeringen.Contains(component.Id))
                    SelectedVerzekeringen.Remove(component.Id);
            }

            var serializedJson = JsonConvert.SerializeObject(
                SelectedVerzekeringen.Select(id => new { Id = id })
                );

            SelectedPriceComponent.ComponentVerzekeringJson = serializedJson;
        }
        public void PriceComponentGridData()
        {
            string verzekeraarNaam = string.Empty;

            SelectedVerzekeraarsPriceComponents.Clear();
            PriceComponentsVerzekeraars.Clear();
            PriceComponents.Clear();

            foreach (var verzekeraar in miscellaneousRepository.GetVerzekeraars())
            {
                if (verzekeraar.IsHerkomst == true && verzekeraar.IsDeleted == false)
                {

                    if (verzekeraar.Pakket == true) { verzekeraarNaam = verzekeraar.Name + " (Pakket)"; } else { verzekeraarNaam = verzekeraar.Name; }

                    PriceComponentsVerzekeraars.Add(new VerzekeraarsModel
                    {
                        Id = verzekeraar.Id,
                        Name = verzekeraarNaam,
                        Afkorting = verzekeraar.Afkorting,
                        Pakket = verzekeraar.Pakket
                    });

                    SelectedVerzekeraarsPriceComponents.Add(new VerzekeraarsModel
                    {
                        Id = verzekeraar.Id,
                        Name = verzekeraar.Name,
                        Afkorting = verzekeraar.Afkorting
                    });
                }
            }
            PriceComponentsVerzekeraars.Insert(0, new VerzekeraarsModel { Id = Guid.NewGuid(), Name = "Geen Filter", Afkorting = "Alles" });

            foreach (var component in miscellaneousRepository.GetAllPriceComponentsBeheer())
            {
                PriceComponents.Add(new KostenbegrotingModel
                {
                    ComponentId = component.ComponentId,
                    ComponentOmschrijving = component.ComponentOmschrijving,
                    ComponentBedrag = component.ComponentBedrag,
                    ComponentAantal = component.ComponentAantal,
                    ComponentVerzekeringJson = component.ComponentVerzekeringJson,
                    DefaultPM = component.DefaultPM,
                    SortOrder = component.SortOrder,
                    IsDeleted = component.IsDeleted,
                    BtnBrush = component.BtnBrush
                });

                _allPriceComponents.Add(new KostenbegrotingModel
                {
                    ComponentId = component.ComponentId,
                    ComponentOmschrijving = component.ComponentOmschrijving,
                    ComponentBedrag = component.ComponentBedrag,
                    ComponentAantal = component.ComponentAantal,
                    ComponentVerzekeringJson = component.ComponentVerzekeringJson,
                    DefaultPM = component.DefaultPM,
                    SortOrder = component.SortOrder,
                    IsDeleted = component.IsDeleted,
                    BtnBrush = component.BtnBrush
                });
            }
        }
        public void ExecuteSavePriceComponentCommand(object obj)
        {
            if (!newPriceComponent)
            {
                try
                {
                    updateRepository.UpdatePriceComponent(SelectedPriceComponent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating pricecomponent: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                SelectedPriceComponent.ComponentId = Guid.NewGuid();
                SelectedPriceComponent.Id = Guid.NewGuid();

                try
                {
                    createRepository.CreatePriceComponent(SelectedPriceComponent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting price component: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            CloseEditPriceComponentPopupCommand.Execute(null);

            if (SelectedPriceComponentVerzekeraar != null &&
                !string.IsNullOrEmpty(SelectedPriceComponentVerzekeraar.Name) &&
                SelectedPriceComponentVerzekeraar.Name != "Geen Filter")
            {
                ExecuteFilterPriceComponentCommand();
            }
            else if (!string.IsNullOrEmpty(SearchOmschrijving))
            {
                FilterPriceComponentsOmschrijving();
            }
            else
            {
                PriceComponentGridData();
            }

        }
        public void ExecuteCreateNewPriceComponentCommand(object obj)
        {
            newPriceComponent = true;
            SelectedVerzekeringen.Clear();

            foreach (var verzekeraar in SelectedVerzekeraarsPriceComponents)
                verzekeraar.IsSelected = false;

            SelectedPriceComponent.ComponentId = Guid.NewGuid();
            SelectedPriceComponent.ComponentOmschrijving = string.Empty;
            SelectedPriceComponent.ComponentBedrag = decimal.Zero;
            SelectedPriceComponent.ComponentFactuurBedrag = decimal.Zero;
            SelectedPriceComponent.ComponentAantal = string.Empty;
            SelectedPriceComponent.ComponentVerzekeringJson = string.Empty;
            SelectedPriceComponent.DefaultPM = false;

            IsEditPriceComponentPopupOpen = true;
        }
        public void ExecuteFilterPriceComponentCommand()
        {
            var filteredPriceComponents = SelectedPriceComponentVerzekeraar.Afkorting == "Alles"
                ? _priceComponents.ToList()
                : _priceComponents.Where(pc =>
                {
                    if (string.IsNullOrEmpty(pc.ComponentVerzekeringJson))
                        return false;

                    try
                    {
                        var ids = JsonConvert.DeserializeObject<List<Dictionary<string, Guid>>>(pc.ComponentVerzekeringJson).Select(dict => dict["Id"]);

                        return ids.Contains(SelectedPriceComponentVerzekeraar.Id);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deserializing ComponentVerzekeringJson for PriceComponent with ID {pc.ComponentId}: {ex.Message}");
                        return false;
                    }
                }).ToList();

            PriceComponents.Clear();
            foreach (var priceComponent in filteredPriceComponents)
                PriceComponents.Add(priceComponent);

        }
        private void FilterPriceComponentsOmschrijving()
        {
            if (string.IsNullOrEmpty(SearchOmschrijving))
            {
                PriceComponents.Clear();
                foreach (var component in _allPriceComponents)
                    PriceComponents.Add(component);
            }
            else
            {
                var filteredComponents = _allPriceComponents
                    .Where(pc => !string.IsNullOrEmpty(pc.ComponentOmschrijving) &&
                        pc.ComponentOmschrijving.Contains(SearchOmschrijving, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                PriceComponents.Clear();
                foreach (var component in filteredComponents)
                {
                    PriceComponents.Add(component);
                }
            }
        }
    }
}
