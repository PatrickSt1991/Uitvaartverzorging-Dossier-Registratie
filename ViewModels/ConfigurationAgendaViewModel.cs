using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Dossier_Registratie.Interfaces;
using Dossier_Registratie.Repositories;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static Dossier_Registratie.ViewModels.OverledeneViewModel;
using System;

namespace Dossier_Registratie.ViewModels
{
    public class ConfigurationAgendaViewModel : ViewModelBase
    {
        private readonly IMiscellaneousAndDocumentOperations miscellaneousRepository;
        private ObservableCollection<AgendaModel> _agenda = [];
        public ICommand OpenDossierViaAgendaCommand { get; }
        public MainWindowViewModal MainWindowViewModel { get; set; }
        public ObservableCollection<AgendaModel> Agenda
        {
            get => _agenda;
            set
            {
                if (_agenda != value)
                {
                    _agenda = value;
                    OnPropertyChanged(nameof(Agenda));
                }
            }
        }

        public ConfigurationAgendaViewModel(IMiscellaneousAndDocumentOperations repository, MainWindowViewModal mainWindowViewModel)
        {
            miscellaneousRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            MainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));

            OpenDossierViaAgendaCommand = new ViewModelCommand(ExecuteOpenDossierViaAgendaCommand);
            GetAllAgendaItems();
        }

        public ConfigurationAgendaViewModel()
            : this(new MiscellaneousAndDocumentOperations(), new MainWindowViewModal()) { }

        public void GetAllAgendaItems()
        {
            Agenda.Clear();

            foreach (var agendaItem in miscellaneousRepository.GetAgenda())
            {
                Agenda.Add(new AgendaModel
                {
                    UitvaartId = agendaItem.UitvaartId,
                    UitvaartNr = agendaItem.UitvaartNr,
                    Achternaam = agendaItem.Achternaam,
                    Voornamen = agendaItem.Voornamen,
                    DatumTijdUitvaart = agendaItem.DatumTijdUitvaart,
                    TijdstipDienst = agendaItem.TijdstipDienst,
                    Uitvaartleider = agendaItem.Uitvaartleider
                });
            }
        }
        public void ExecuteOpenDossierViaAgendaCommand(object obj)
        {
            if (obj is not string objString || string.IsNullOrWhiteSpace(objString))
                return;

            ComboAggregator.Transmit();
            Globals.NewDossierCreation = false;
            Globals.UitvaartCodeGuid = miscellaneousRepository.GetUitvaartGuid(objString);
            Instance.RequestedDossierInformationBasedOnUitvaartId(objString);
            IntAggregator.Transmit(666);
        }
    }

}
