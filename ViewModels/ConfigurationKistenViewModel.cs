using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Interfaces;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationKistenViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivateKistCommand { get; }
        public ICommand EditKistCommand { get; }
        public ICommand DisableKistCommand { get; }
        public ICommand CloseEditKistPopupCommand { get; }
        public ICommand SaveKistCommand { get; }
        public ICommand CreateNewKistCommand { get; }
        public ICommand RefreshKistenGridCommand { get; set; }

        private bool isEditKistPopupOpen;
        private bool newKist;
        private bool _showDeleted;

        private KistenModel selectedKist;
        private ObservableCollection<KistenModel> _kisten;
        private ICollectionView _filteredKisten;

        public KistenModel SelectedKist
        {
            get { return selectedKist; }
            set
            {
                if (selectedKist != value)
                {
                    selectedKist = value;
                    OnPropertyChanged(nameof(SelectedKist));
                }
            }
        }
        public ObservableCollection<KistenModel> Kisten
        {
            get { return _kisten; }
            set
            {
                if (_kisten != value)
                {
                    _kisten = value;
                    OnPropertyChanged(nameof(Kisten));
                }
            }
        }
        public ICollectionView FilteredKisten
        {
            get => _filteredKisten;
            set
            {
                _filteredKisten = value;
                OnPropertyChanged(nameof(FilteredKisten));
            }
        }
        public bool IsEditKistPopupOpen
        {
            get { return isEditKistPopupOpen; }
            set
            {
                if (isEditKistPopupOpen != value)
                {
                    isEditKistPopupOpen = value;
                    OnPropertyChanged(nameof(IsEditKistPopupOpen));
                }
            }
        }
        public bool ShowDeleted
        {
            get { return _showDeleted; }
            set
            {
                if (_showDeleted != value)
                {
                    _showDeleted = value;
                    OnPropertyChanged(nameof(ShowDeleted));
                    FilteredKisten.Refresh();
                }
            }
        }

        public ConfigurationKistenViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedKist = new KistenModel();
            Kisten = new ObservableCollection<KistenModel>();

            FilteredKisten = CollectionViewSource.GetDefaultView(Kisten);
            FilteredKisten.Filter = FilterKisten;

            ActivateKistCommand = new ViewModelCommand(ExecuteActivateKistCommand);
            EditKistCommand = new ViewModelCommand(ExecuteEditKistCommand);
            DisableKistCommand = new ViewModelCommand(ExecuteDisableKistCommand);
            CloseEditKistPopupCommand = new RelayCommand(() => IsEditKistPopupOpen = false);
            RefreshKistenGridCommand = new RelayCommand(KistenGridData);
            SaveKistCommand = new ViewModelCommand(ExecuteSaveKistCommand);
            CreateNewKistCommand = new ViewModelCommand(ExecuteCreateNewKistCommand);

            KistenGridData();
        }
        public void ExecuteActivateKistCommand(object obj)
        {
            MessageBoxResult activeQuestion = MessageBox.Show("Wil je deze uitvaart kist activeren?", "Uitvaart kist activeren", MessageBoxButton.YesNo);
            if (activeQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid kistId))
                {
                    try
                    {
                        commandRepository.ActivateKist(kistId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error activating kist: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    KistenGridData();
                }
                else
                {
                    MessageBox.Show("Invalid kist ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void ExecuteEditKistCommand(object obj)
        {
            var kist = miscellaneousRepository.GetKist((Guid)obj);

            selectedKist.Id = kist.Id;
            selectedKist.KistOmschrijving = kist.KistOmschrijving;
            selectedKist.KistTypeNummer = kist.KistTypeNummer;

            newKist = false;
            IsEditKistPopupOpen = true;
        }
        public void ExecuteDisableKistCommand(object obj)
        {
            MessageBoxResult disableQuestion = MessageBox.Show("Wil je deze uitvaart kist deactiveren?", "Uitvaart kist deactiveren", MessageBoxButton.YesNo);
            if (disableQuestion == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid kistId))
                {
                    try
                    {
                        commandRepository.DisableKist(kistId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error disabeling kist: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    KistenGridData();
                }
                else
                {
                    MessageBox.Show("Invalid asbestemming ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void KistenGridData()
        {
            Kisten.Clear();

            foreach (var kist in miscellaneousRepository.GetKisten())
            {
                Kisten.Add(new KistenModel
                {
                    Id = kist.Id,
                    KistTypeNummer = kist.KistTypeNummer,
                    KistOmschrijving = kist.KistOmschrijving,
                    IsDeleted = kist.IsDeleted,
                    BtnBrush = kist.BtnBrush
                });
            }
        }
        public void ExecuteSaveKistCommand(object obj)
        {
            CloseEditKistPopupCommand.Execute(null);

            if (!newKist)
            {
                try
                {
                    updateRepository.KistUpdate(SelectedKist);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating kist: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                try
                {
                    createRepository.KistCreate(SelectedKist);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting kist: {ex.Message}", "Kist Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            KistenGridData();
        }
        public void ExecuteCreateNewKistCommand(object obj)
        {
            newKist = true;

            selectedKist.Id = Guid.NewGuid();
            selectedKist.KistOmschrijving = string.Empty;
            selectedKist.KistTypeNummer = string.Empty;

            IsEditKistPopupOpen = true;
        }
        private bool FilterKisten(object item)
        {
            if (item is KistenModel kist)
                return ShowDeleted || !kist.IsDeleted;

            return false;
        }
    }
}
