using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationAsbestemmingenViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivateAsbestemmingCommand { get; }
        public ICommand EditAsbestemmingCommand { get; }
        public ICommand DisableAsbestemmingCommand { get; }
        public ICommand CloseEditAsbestemmingPopupCommand { get; }
        public ICommand SaveAsbestemmingCommand { get; }
        public ICommand CreateNewAsbestemmingCommand { get; }
        public ICommand RefreshAsbestemmingGridCommand { get; set; }

        private bool isEditAsbestemmingPopupOpen;
        private bool newAsbestemming;
        private bool _showDeleted;

        private ConfigurationAsbestemmingModel selectedAsbestemming;
        private ObservableCollection<ConfigurationAsbestemmingModel> _asbestemming;
        private ICollectionView _filteredAsbestemming;

        public bool IsEditAsbestemmingPopupOpen
        {
            get { return isEditAsbestemmingPopupOpen; }
            set
            {
                if (isEditAsbestemmingPopupOpen != value)
                {
                    isEditAsbestemmingPopupOpen = value;
                    OnPropertyChanged(nameof(IsEditAsbestemmingPopupOpen));
                }
            }
        }
        public ConfigurationAsbestemmingModel SelectedAsbestemming
        {
            get { return selectedAsbestemming; }
            set
            {
                if (selectedAsbestemming != value)
                {
                    selectedAsbestemming = value;
                    OnPropertyChanged(nameof(SelectedAsbestemming));
                }
            }
        }
        public ObservableCollection<ConfigurationAsbestemmingModel> Asbestemming
        {
            get { return _asbestemming; }
            set
            {
                if (_asbestemming != value)
                {
                    _asbestemming = value;
                    OnPropertyChanged(nameof(Asbestemming));
                }
            }
        }
        public ICollectionView FilteredAsbestemming
        {
            get => _filteredAsbestemming;
            set
            {
                _filteredAsbestemming = value;
                OnPropertyChanged(nameof(FilteredAsbestemming));
            }
        }
        public bool ShowDeleted
        {
            get { return _showDeleted; }
            set
            {
                if(_showDeleted != value)
                {
                    _showDeleted = value;
                    OnPropertyChanged(nameof(ShowDeleted));
                    FilteredAsbestemming.Refresh();
                }
            }
        }

        public ConfigurationAsbestemmingenViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedAsbestemming = new ConfigurationAsbestemmingModel();
            Asbestemming = new ObservableCollection<ConfigurationAsbestemmingModel>();
            FilteredAsbestemming = CollectionViewSource.GetDefaultView(Asbestemming);

            FilteredAsbestemming.Filter = FilterAsbestemming;

            ActivateAsbestemmingCommand = new ViewModelCommand(ExecuteActivateAsbestemmingCommand);
            EditAsbestemmingCommand = new ViewModelCommand(ExecuteEditAsbestemmingCommand);
            DisableAsbestemmingCommand = new ViewModelCommand(ExecuteDisableAsbestemmingCommand);
            CloseEditAsbestemmingPopupCommand = new RelayCommand(() => IsEditAsbestemmingPopupOpen = false);
            RefreshAsbestemmingGridCommand = new RelayCommand(AsbestemmingGridData);
            SaveAsbestemmingCommand = new ViewModelCommand(ExecuteSaveAsbestemmingCommand);
            CreateNewAsbestemmingCommand = new ViewModelCommand(ExecuteCreateNewAsbestemming);

            AsbestemmingGridData();
        }
        public void ExecuteActivateAsbestemmingCommand(object obj)
        {
            MessageBoxResult activeQuestion = MessageBox.Show("Wil je deze asbestemming activeren?", "Asbestemming activeren", MessageBoxButton.YesNo);
            if (activeQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid asbestemmingId))
                {
                    try
                    {
                        commandRepository.ActivateAsbestemming(asbestemmingId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error activating asbestemming: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    AsbestemmingGridData();
                }
                else
                {
                    MessageBox.Show("Invalid asbestemming ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ExecuteEditAsbestemmingCommand(object obj)
        {
            var asbestemming = miscellaneousRepository.GetAsbestemming((Guid)obj);

            selectedAsbestemming.AsbestemmingId = asbestemming.AsbestemmingId;
            selectedAsbestemming.AsbestemmingOmschrijving = asbestemming.AsbestemmingOmschrijving;

            newAsbestemming = false;
            IsEditAsbestemmingPopupOpen = true;
        }
        public void ExecuteDisableAsbestemmingCommand(object obj)
        {
            MessageBoxResult disableQuestion = MessageBox.Show("Wil je deze asbestemming deactiveren?", "Asbestemming deactiveren", MessageBoxButton.YesNo);
            if (disableQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid asbestemmingId))
                {
                    try
                    {
                        commandRepository.DisableAsbestemming(asbestemmingId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error disabeling asbestemming: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    AsbestemmingGridData();
                }
                else
                {
                    MessageBox.Show("Invalid asbestemming ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void AsbestemmingGridData()
        {
            Asbestemming.Clear();

            foreach (var asbestemming in miscellaneousRepository.GetAsbestemmingen())
            {
                Asbestemming.Add(new ConfigurationAsbestemmingModel
                {
                    AsbestemmingId = asbestemming.AsbestemmingId,
                    AsbestemmingOmschrijving = asbestemming.AsbestemmingOmschrijving,
                    IsDeleted = asbestemming.IsDeleted,
                    BtnBrush = asbestemming.BtnBrush
                });
            }
        }
        public void ExecuteSaveAsbestemmingCommand(object obj)
        {
            CloseEditAsbestemmingPopupCommand.Execute(null);

            if (!newAsbestemming)
            {
                try
                {
                    updateRepository.AsbestemmingUpdate(SelectedAsbestemming);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating asbestemming: {ex.Message},\r\n" +
                        $"Er is een automatische error melding aangemaakt voor Patrick.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                try
                {
                    createRepository.AsbestemmingCreate(SelectedAsbestemming);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting asbestemming: {ex.Message}", "Kist Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            AsbestemmingGridData();
        }
        public void ExecuteCreateNewAsbestemming(object obj)
        {
            newAsbestemming = true;

            selectedAsbestemming.AsbestemmingId = Guid.NewGuid();
            selectedAsbestemming.AsbestemmingOmschrijving = string.Empty;
            selectedAsbestemming.IsDeleted = false;

            IsEditAsbestemmingPopupOpen = true;
        }
        private bool FilterAsbestemming(object item)
        {
            if (item is ConfigurationAsbestemmingModel asbestemming)
                return ShowDeleted || !asbestemming.IsDeleted;

            return false;
        }
    }
}
