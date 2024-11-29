using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        public ICommand FilterPriceComponentsCommand { get; }
        public ICommand VerzekeringCheckCommand { get; set; }
        public ICommand RefreshPriceComponentsGridCommand { get; set; }
        public ICommand SelectAllCommand { get; }
        public ICommand DeselectAllCommand { get; }

        private bool isEditPriceComponentPopupOpen;
        private bool newPriceComponent;
        private bool _keepFilterActive = false;
        private string searchOmschrijving;

        private KostenbegrotingModel _selectedPriceComponent;
        private KostenbegrotingModel _priceComponentFilter;
        private VerzekeraarsModel _selectedPriceComponentVerzekeraar;
        private ObservableCollection<VerzekeraarsModel> _priceComponentsVerzekeraars;
        private ObservableCollection<VerzekeraarsModel> _selectedVerzekeraarsPriceComponents;
        private ObservableCollection<string> _selectedAfkortingen;
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
                    {
                        PriceComponentFilter.ComponentVerzekering = _selectedPriceComponentVerzekeraar.Afkorting;
                        PriceComponentFilter.SpecificPakket = _selectedPriceComponentVerzekeraar.Pakket;
                    }

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
        public ObservableCollection<string> SelectedAfkortingen
        {
            get { return _selectedAfkortingen; }
            set
            {
                if (_selectedAfkortingen != value)
                {
                    if (_selectedAfkortingen != null)
                    {
                        _selectedAfkortingen.CollectionChanged -= SelectedAfkortingen_CollectionChanged;
                    }

                    _selectedAfkortingen = value;

                    if (_selectedAfkortingen != null)
                    {
                        _selectedAfkortingen.CollectionChanged += SelectedAfkortingen_CollectionChanged;
                    }

                    OnPropertyChanged(nameof(SelectedAfkortingen));
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
                    OnPropertyChanged(nameof(_priceComponents));
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
            SelectedAfkortingen = new ObservableCollection<string>();
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
            SelectedAfkortingen.Clear();
            SelectedPriceComponent.ComponentVerzekering = string.Empty;

            foreach (var item in SelectedVerzekeraarsPriceComponents)
            {
                item.IsSelected = true;
                OnVerzekeringCheckBoxCheckedChanged(item); // Update SelectedAfkortingen
            }
        }
        private void DeselectAll()
        {
            SelectedAfkortingen.Clear();
            SelectedPriceComponent.ComponentVerzekering = string.Empty;

            foreach (var item in SelectedVerzekeraarsPriceComponents)
            {
                item.IsSelected = false;
                OnVerzekeringCheckBoxCheckedChanged(item); // Update SelectedAfkortingen
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
            SelectedPriceComponent.ComponentVerzekering = priceComp.ComponentVerzekering;
            SelectedPriceComponent.SortOrder = priceComp.SortOrder;

            SelectedAfkortingen.Clear();
            if (!string.IsNullOrEmpty(priceComp.ComponentVerzekering))
            {
                var afkortingen = priceComp.ComponentVerzekering.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var afkorting in afkortingen)
                    SelectedAfkortingen.Add(afkorting.Trim());
            }

            foreach (var verzekeraar in SelectedVerzekeraarsPriceComponents)
                verzekeraar.IsSelected = SelectedAfkortingen.Contains(verzekeraar.Afkorting);

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
                if (!SelectedAfkortingen.Contains(component.Afkorting))
                {
                    SelectedAfkortingen.CollectionChanged -= SelectedAfkortingen_CollectionChanged;
                    SelectedAfkortingen.Add(component.Afkorting);
                    SelectedAfkortingen.CollectionChanged += SelectedAfkortingen_CollectionChanged;
                }
            }
            else
            {
                if (SelectedAfkortingen.Contains(component.Afkorting))
                {
                    SelectedAfkortingen.CollectionChanged -= SelectedAfkortingen_CollectionChanged;
                    SelectedAfkortingen.Remove(component.Afkorting);
                    SelectedAfkortingen.CollectionChanged += SelectedAfkortingen_CollectionChanged;
                }
            }

            // Manually trigger the collection changed handler
            SelectedAfkortingen_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void PriceComponentGridData()
        {
            string verzekeraarNaam = string.Empty;

            SelectedVerzekeraarsPriceComponents.Clear();
            PriceComponentsVerzekeraars.Clear();
            PriceComponents.Clear();

            foreach (var verzekeraar in miscellaneousRepository.GetVerzekeraars())
            {
                if (verzekeraar.IsVerzekeraar == true && verzekeraar.IsDeleted == false)
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
                    ComponentVerzekering = component.ComponentVerzekering,
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
                    ComponentVerzekering = component.ComponentVerzekering,
                    DefaultPM = component.DefaultPM,
                    SortOrder = component.SortOrder,
                    IsDeleted = component.IsDeleted,
                    BtnBrush = component.BtnBrush
                });
            }
        }
        public void ExecuteSavePriceComponentCommand(object obj)
        {
            string InsuranceAfkorting = string.Empty;
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
                _keepFilterActive = true;
                ExecuteFilterPriceComponentCommand();
            }
            else if (!string.IsNullOrEmpty(SearchOmschrijving))
            {
                _keepFilterActive = true;
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
            SelectedAfkortingen.Clear();

            foreach (var verzekeraar in SelectedVerzekeraarsPriceComponents)
            {
                verzekeraar.IsSelected = false;
            }

            SelectedPriceComponent.ComponentId = Guid.NewGuid();
            SelectedPriceComponent.ComponentOmschrijving = string.Empty;
            SelectedPriceComponent.ComponentBedrag = decimal.Zero;
            SelectedPriceComponent.ComponentFactuurBedrag = decimal.Zero;
            SelectedPriceComponent.ComponentAantal = string.Empty;
            SelectedPriceComponent.ComponentVerzekering = string.Empty;
            SelectedPriceComponent.DefaultPM = false;

            IsEditPriceComponentPopupOpen = true;
        }
        public void ExecuteFilterPriceComponentCommand()
        {
            if (!string.IsNullOrEmpty(PriceComponentFilter.ComponentVerzekering))
            {
                PriceComponents.Clear();

                if (PriceComponentFilter.ComponentVerzekering == "Alles")
                {
                    foreach (var PriceComponent in miscellaneousRepository.GetAllPriceComponentsBeheer())
                    {
                        PriceComponents.Add(new KostenbegrotingModel
                        {
                            ComponentId = PriceComponent.ComponentId,
                            ComponentOmschrijving = PriceComponent.ComponentOmschrijving,
                            ComponentBedrag = PriceComponent.ComponentBedrag,
                            ComponentAantal = PriceComponent.ComponentAantal,
                            ComponentVerzekering = PriceComponent.ComponentVerzekering,
                            DefaultPM = PriceComponent.DefaultPM,
                            IsDeleted = PriceComponent.IsDeleted,
                            BtnBrush = PriceComponent.BtnBrush
                        });
                    }
                }
                else
                {
                    var filteredComponents = miscellaneousRepository
                        .GetFilterdPriceComponentsBeheer(PriceComponentFilter.ComponentVerzekering);

                    foreach (var PriceComponent in filteredComponents)
                    {
                        PriceComponents.Add(new KostenbegrotingModel
                        {
                            ComponentId = PriceComponent.ComponentId,
                            ComponentOmschrijving = PriceComponent.ComponentOmschrijving,
                            ComponentBedrag = PriceComponent.ComponentBedrag,
                            ComponentAantal = PriceComponent.ComponentAantal,
                            ComponentVerzekering = PriceComponent.ComponentVerzekering,
                            DefaultPM = PriceComponent.DefaultPM,
                            IsDeleted = PriceComponent.IsDeleted,
                            BtnBrush = PriceComponent.BtnBrush
                        });
                    }
                }
            }
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
        private void SelectedAfkortingen_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectedPriceComponent.ComponentVerzekering = string.Join(",", SelectedAfkortingen);
        }
    }
}
