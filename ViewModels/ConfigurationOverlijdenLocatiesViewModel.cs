using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationOverlijdenLocatiesViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivateOverlijdenLocatieCommand { get; }
        public ICommand EditOverlijdenLocatieCommand { get; }
        public ICommand DisableOverlijdenLocatieCommand { get; }
        public ICommand CloseEditOverlijdenLocatiePopupCommand { get; }
        public ICommand SaveOverlijdenLocatieCommand { get; }
        public ICommand CreateNewOverlijdenLocatieCommand { get; }
        public ICommand RefreshOverlijdenLocatieGridCommand { get; set; }

        private bool isEditOverlijdenLocatiePopupOpen;
        private bool newOverlijdenLocatie;

        private SuggestionModel selectedOverlijdenlocatie;
        private ObservableCollection<SuggestionModel> _overlijdenlocatie;

        public SuggestionModel SelectedOverlijdenlocatie
        {
            get { return selectedOverlijdenlocatie; }
            set
            {
                if (selectedOverlijdenlocatie != value)
                {
                    selectedOverlijdenlocatie = value;
                    OnPropertyChanged(nameof(SelectedOverlijdenlocatie));
                }
            }
        }
        public ObservableCollection<SuggestionModel> Overlijdenlocatie
        {
            get { return _overlijdenlocatie; }
            set
            {
                if (_overlijdenlocatie != value)
                {
                    _overlijdenlocatie = value;
                    OnPropertyChanged(nameof(Overlijdenlocatie));
                }
            }
        }
        public bool IsEditOverlijdenLocatiePopupOpen
        {
            get { return isEditOverlijdenLocatiePopupOpen; }
            set
            {
                if (isEditOverlijdenLocatiePopupOpen != value)
                {
                    isEditOverlijdenLocatiePopupOpen = value;
                    OnPropertyChanged(nameof(IsEditOverlijdenLocatiePopupOpen));
                }
            }
        }

        public ConfigurationOverlijdenLocatiesViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedOverlijdenlocatie = new SuggestionModel();
            Overlijdenlocatie = new ObservableCollection<SuggestionModel>();

            ActivateOverlijdenLocatieCommand = new ViewModelCommand(ExecuteActivateOverlijdenLocatieCommand);
            EditOverlijdenLocatieCommand = new ViewModelCommand(ExecuteEditOverlijdenLocatieCommand);
            DisableOverlijdenLocatieCommand = new ViewModelCommand(ExecuteDisableOverlijdenLocatieCommand);
            CloseEditOverlijdenLocatiePopupCommand = new RelayCommand(() => IsEditOverlijdenLocatiePopupOpen = false);
            RefreshOverlijdenLocatieGridCommand = new RelayCommand(OverlijdenLocatieGridData);
            SaveOverlijdenLocatieCommand = new ViewModelCommand(ExecuteSaveOverlijdenLocatieCommand);
            CreateNewOverlijdenLocatieCommand = new ViewModelCommand(ExecuteCreateNewOverlijdenLocatie);
            OverlijdenLocatieGridData();
        }
        public void ExecuteActivateOverlijdenLocatieCommand(object obj)
        {
            MessageBoxResult activateLocatie = MessageBox.Show("Wil je deze suggestie locatie heractiveren?", "Locatie heractiveren", MessageBoxButton.YesNo);
            if (activateLocatie == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid id))
                {
                    try
                    {
                        commandRepository.ActivateSuggestion(id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error activating suggestie locatie: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }
                    OverlijdenLocatieGridData();
                }
            }
        }
        public void ExecuteEditOverlijdenLocatieCommand(object obj)
        {
            var suggestion = miscellaneousRepository.GetSuggestionBeheer((Guid)obj);

            selectedOverlijdenlocatie.Id = suggestion.Id;
            selectedOverlijdenlocatie.ShortName = suggestion.ShortName;
            selectedOverlijdenlocatie.LongName = suggestion.LongName;
            selectedOverlijdenlocatie.Street = suggestion.Street;
            selectedOverlijdenlocatie.HouseNumber = suggestion.HouseNumber;
            selectedOverlijdenlocatie.ZipCode = suggestion.ZipCode;
            selectedOverlijdenlocatie.City = suggestion.City;
            selectedOverlijdenlocatie.County = suggestion.County;

            newOverlijdenLocatie = false;
            IsEditOverlijdenLocatiePopupOpen = true;
        }
        public void ExecuteDisableOverlijdenLocatieCommand(object obj)
        {
            MessageBoxResult disableSuggestie = MessageBox.Show("Wil je deze suggestie locatie deactiveren?", "Suggestie deactiveren", MessageBoxButton.YesNo);
            if (disableSuggestie == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid Id))
                {
                    try
                    {
                        commandRepository.DisableSuggestion(Id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error disabeling suggestie locatie: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    OverlijdenLocatieGridData();
                }
                else
                {
                    MessageBox.Show("Invalid suggestie ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void OverlijdenLocatieGridData()
        {
            Overlijdenlocatie.Clear();
            foreach (var suggestie in miscellaneousRepository.GetSuggestionsBeheer())
            {
                Overlijdenlocatie.Add(new SuggestionModel
                {
                    Id = suggestie.Id,
                    ShortName = suggestie.ShortName,
                    LongName = suggestie.LongName,
                    Street = suggestie.Street,
                    HouseNumber = suggestie.HouseNumber,
                    ZipCode = suggestie.ZipCode,
                    City = suggestie.City,
                    County = suggestie.County,
                    IsDeleted = suggestie.IsDeleted,
                    BtnBrush = suggestie.BtnBrush
                });
            }
        }
        public void ExecuteSaveOverlijdenLocatieCommand(object obj)
        {
            if (!newOverlijdenLocatie)
            {
                try
                {
                    updateRepository.UpdateSuggestion(SelectedOverlijdenlocatie);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating suggestion: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                SelectedOverlijdenlocatie.Id = Guid.NewGuid();

                try
                {
                    createRepository.CreateSuggestion(SelectedOverlijdenlocatie);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting suggestion: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            CloseEditOverlijdenLocatiePopupCommand.Execute(null);
            OverlijdenLocatieGridData();
        }
        public void ExecuteCreateNewOverlijdenLocatie(object obj)
        {
            newOverlijdenLocatie = true;

            selectedOverlijdenlocatie.Id = Guid.NewGuid();
            selectedOverlijdenlocatie.ShortName = string.Empty;
            selectedOverlijdenlocatie.LongName = string.Empty;
            selectedOverlijdenlocatie.Street = string.Empty;
            selectedOverlijdenlocatie.HouseNumber = string.Empty;
            selectedOverlijdenlocatie.ZipCode = string.Empty;
            selectedOverlijdenlocatie.City = string.Empty;
            selectedOverlijdenlocatie.County = string.Empty;
            selectedOverlijdenlocatie.IsDeleted = false;

            IsEditOverlijdenLocatiePopupOpen = true;
        }
    }
}
