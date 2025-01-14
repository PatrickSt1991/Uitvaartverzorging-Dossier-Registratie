using CommunityToolkit.Mvvm.Input;
using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Dossier_Registratie.ViewModels
{
    public class OverledeneNotificationViewModel : ViewModelBase
    {
        private readonly IUpdateOperations updateRepository;
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;

        private ObservableCollection<NotificatieOverzichtModel> _yearPassedNotification;
        public ObservableCollection<NotificatieOverzichtModel> YearPassedNotification
        {
            get => _yearPassedNotification;
            set
            {
                _yearPassedNotification = value;
                OnPropertyChanged(nameof(YearPassedNotification));
            }
        }
        public ICommand DisabledNotification { get; }
        public OverledeneNotificationViewModel()
        {
            DisabledNotification = new RelayCommand<Guid>(ExecuteDisabledNotification);
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            updateRepository = new UpdateOperations();
            LoadNotifications();
        }
        private async void LoadNotifications()
        {
            try
            {
                var startDate = DateTime.Today.AddYears(-1).AddDays(-7);
                var endDate = DateTime.Today.AddYears(-1).AddDays(7);
                var activeUser = Environment.UserName;

                var notifications = await miscellaneousRepository.NotificationDeceasedAfterYearPassedAsync();
                YearPassedNotification = new ObservableCollection<NotificatieOverzichtModel>(
                    notifications.Where(x => x.OverledenDatumTijd >= startDate && x.OverledenDatumTijd <= endDate && x.WindowsAccount == activeUser)
                );
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
        }
        private async void ExecuteDisabledNotification(Guid uitvaartId)
        {
            try
            {
                await updateRepository.UpdateNotification(uitvaartId);
                LoadNotifications();
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }

        }
    }
}
