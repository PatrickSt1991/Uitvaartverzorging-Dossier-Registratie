using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Interfaces;
using Dossier_Registratie.Repositories;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static Dossier_Registratie.ViewModels.OverledeneViewModel;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationAgendaViewModel : ViewModelBase
    {
        public ICommand OpenDossierViaAgendaCommand { get; }
        public MainWindowViewModal MainWindowViewModel { get; set; }
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private ObservableCollection<AgendaModel> _agenda;
        public ObservableCollection<AgendaModel> Agenda
        {
            get { return _agenda; }
            set
            {
                if (_agenda != value)
                {
                    _agenda = value;
                    OnPropertyChanged(nameof(Agenda));
                }
            }
        }

        public ConfigurationAgendaViewModel()
        {
            miscellaneousRepository = new MiscellaneousAndDocumentOperations();
            Agenda = new ObservableCollection<AgendaModel>();
            OpenDossierViaAgendaCommand = new ViewModelCommand(ExecuteOpenDossierViaAgendaCommand);

            GetAllAgendaItems();
        }
        public void GetAllAgendaItems()
        {
            Agenda.Clear();

            foreach (var agendaItems in miscellaneousRepository.GetAgenda())
            {
                Agenda.Add(new AgendaModel()
                {
                    UitvaartId = agendaItems.UitvaartId,
                    UitvaartNr = agendaItems.UitvaartNr,
                    Achternaam = agendaItems.Achternaam,
                    Voornamen = agendaItems.Voornamen,
                    DatumTijdUitvaart = agendaItems.DatumTijdUitvaart,
                    TijdstipDienst = agendaItems.TijdstipDienst,
                    Uitvaartleider = agendaItems.Uitvaartleider
                });
            }
        }
        public void ExecuteOpenDossierViaAgendaCommand(object obj)
        {
            ComboAggregator.Transmit();
            Globals.NewDossierCreation = false;
            Globals.UitvaartCodeGuid = miscellaneousRepository.GetUitvaartGuid(obj.ToString());
            Instance.RequestedDossierInformationBasedOnUitvaartId(obj.ToString());
            IntAggregator.Transmit(666);
        }
    }
}
