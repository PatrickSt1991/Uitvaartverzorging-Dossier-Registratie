using Dossier_Registratie.Models;
using Dossier_Registratie.Repositories;
using Dossier_Registratie.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            DisabledNotification = new ViewModelCommand(DisableNotification);
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            updateRepository = new UpdateOperations();
            DeceasedYearAgoCheck();
        }
        private async Task DeceasedYearAgoCheck()
        {
            try
            {
                YearPassedNotification = await miscellaneousRepository.NotificationDeceasedAfterYearPassedAsync();
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
        }
        public void DisableNotification(object obj)
        {
            try
            {
                if (Guid.TryParse(obj?.ToString(), out Guid uitvaartId))
                    updateRepository.UpdateNotification(uitvaartId);
            }
            catch (Exception ex)
            {
                ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
            }
        }
    }
}
