using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationRouwbrievenViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private readonly ISearchOperations searchRepository;
        private readonly ICreateOperations createRepository;
        private readonly IUpdateOperations updateRepository;
        private readonly IDeleteAndActivateDisableOperations commandRepository;

        public ICommand ActivateRouwbrievenCommand { get; }
        public ICommand EditRouwbrievenCommand { get; }
        public ICommand DisableRouwbrievenCommand { get; }
        public ICommand CloseEditRouwbrievenPopupCommand { get; }
        public ICommand SaveRouwbrievenCommand { get; }
        public ICommand CreateNewRouwbrievenCommand { get; }
        public ICommand RefreshRouwbrievenGridCommand { get; set; }

        private bool isEditRouwbrievenPopupOpen;
        private bool newRouwbrief;

        private OverledeneRouwbrieven selectedRouwbrief;
        private ObservableCollection<OverledeneRouwbrieven> _rouwbrieven;
        public OverledeneRouwbrieven SelectedRouwbrief
        {
            get { return selectedRouwbrief; }
            set
            {
                if (selectedRouwbrief != value)
                {
                    selectedRouwbrief = value;
                    OnPropertyChanged(nameof(SelectedRouwbrief));
                }
            }
        }
        public ObservableCollection<OverledeneRouwbrieven> Rouwbrieven
        {
            get { return _rouwbrieven; }
            set
            {
                if (_rouwbrieven != value)
                {
                    _rouwbrieven = value;
                    OnPropertyChanged(nameof(Rouwbrieven));
                }
            }
        }
        public bool IsEditRouwbrievenPopupOpen
        {
            get { return isEditRouwbrievenPopupOpen; }
            set
            {
                if (isEditRouwbrievenPopupOpen != value)
                {
                    isEditRouwbrievenPopupOpen = value;
                    OnPropertyChanged(nameof(IsEditRouwbrievenPopupOpen));
                }
            }
        }

        public ConfigurationRouwbrievenViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            searchRepository = new SearchOperations();
            createRepository = new CreateOperations();
            updateRepository = new UpdateOperations();
            commandRepository = new DeleteAndActivateDisableOperations();

            SelectedRouwbrief = new OverledeneRouwbrieven();
            Rouwbrieven = new ObservableCollection<OverledeneRouwbrieven>();

            ActivateRouwbrievenCommand = new ViewModelCommand(ExecuteActivateRouwbrievenCommand);
            EditRouwbrievenCommand = new ViewModelCommand(ExecuteEditRouwbrievenCommand);
            DisableRouwbrievenCommand = new ViewModelCommand(ExecuteDisableRouwbrievenCommand);
            CloseEditRouwbrievenPopupCommand = new RelayCommand(() => IsEditRouwbrievenPopupOpen = false);
            RefreshRouwbrievenGridCommand = new RelayCommand(RouwbrievenGridData);
            SaveRouwbrievenCommand = new ViewModelCommand(ExecuteSaveRouwbriefCommand);
            CreateNewRouwbrievenCommand = new ViewModelCommand(ExecuteCreateNewRouwbrief);
            RouwbrievenGridData();
        }
        public void ExecuteActivateRouwbrievenCommand(object obj)
        {
            MessageBoxResult ativateRouwbrief = MessageBox.Show("Wil je deze rouwbrief heractiveren?", "Rouwbrief heractiveren", MessageBoxButton.YesNo);
            if (ativateRouwbrief == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid rouwbriefId))
                {
                    try
                    {
                        commandRepository.ActivateRouwbrief(rouwbriefId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error activating rouwbrief: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    RouwbrievenGridData();
                }
            }
        }
        public void ExecuteEditRouwbrievenCommand(object obj)
        {
            var rouwbrief = miscellaneousRepository.GetRouwbriefBeheer((Guid)obj);

            selectedRouwbrief.RouwbrievenId = rouwbrief.RouwbrievenId;
            selectedRouwbrief.RouwbrievenName = rouwbrief.RouwbrievenName;

            newRouwbrief = false;
            IsEditRouwbrievenPopupOpen = true;
        }
        public void ExecuteDisableRouwbrievenCommand(object obj)
        {
            MessageBoxResult disableRouwbrief = MessageBox.Show("Wil je deze rouwbrief deactiveren?", "Rouwbrief deactiveren", MessageBoxButton.YesNo);
            if (disableRouwbrief == MessageBoxResult.Yes)
            {
                if (Guid.TryParse(obj?.ToString(), out Guid rouwbrievenId))
                {
                    try
                    {
                        commandRepository.DisableRouwbrief(rouwbrievenId);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error disabeling rouwbrief: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                        return;
                    }

                    RouwbrievenGridData();
                }
                else
                {
                    MessageBox.Show("Invalid rouwbrief ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public void RouwbrievenGridData()
        {
            Rouwbrieven.Clear();
            foreach (var rouwbrief in miscellaneousRepository.GetRouwbrieven())
            {
                Rouwbrieven.Add(new OverledeneRouwbrieven
                {
                    RouwbrievenId = rouwbrief.RouwbrievenId,
                    RouwbrievenName = rouwbrief.RouwbrievenName,
                    IsDeleted = rouwbrief.IsDeleted,
                    BtnBrush = rouwbrief.BtnBrush
                });
            }
        }
        public void ExecuteSaveRouwbriefCommand(object obj)
        {
            if (!newRouwbrief)
            {
                try
                {
                    updateRepository.UpdateRouwbrief(SelectedRouwbrief);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating rouwbrief: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            else
            {
                SelectedRouwbrief.RouwbrievenId = Guid.NewGuid();

                try
                {
                    createRepository.CreateRouwbrief(SelectedRouwbrief);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting rouwbrief: {ex.Message}", "Insert Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return;
                }
            }
            CloseEditRouwbrievenPopupCommand.Execute(null);
            RouwbrievenGridData();
        }
        public void ExecuteCreateNewRouwbrief(object obj)
        {
            newRouwbrief = true;

            selectedRouwbrief.RouwbrievenId = Guid.NewGuid();
            selectedRouwbrief.RouwbrievenName = string.Empty;
            selectedRouwbrief.IsDeleted = false;

            IsEditRouwbrievenPopupOpen = true;
        }
    }
}
