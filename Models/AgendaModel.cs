using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class NotificatieOverzichtModel : ViewModelBase
    {
        private Guid _uitvaartId;
        private string _uitvaartNr;
        private string _overledeneNaam;
        private DateTime _overledenDatumTijd;
        private string _opdrachtTelefoon;
        private string _windowsAccount;
        private string _cijfer;
        private string _opdrachtgever;
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public string UitvaartNr
        {
            get { return _uitvaartNr; }
            set { _uitvaartNr = value; OnPropertyChanged(nameof(UitvaartNr)); }
        }
        public string OverledeneNaam
        {
            get { return _overledeneNaam; }
            set { _overledeneNaam = value; OnPropertyChanged(nameof(OverledeneNaam)); }
        }
        public DateTime OverledenDatumTijd
        {
            get { return _overledenDatumTijd; }
            set { _overledenDatumTijd = value; OnPropertyChanged(nameof(OverledenDatumTijd)); }
        }
        public string OpdrachtTelefoon
        {
            get { return _opdrachtTelefoon; }
            set { _opdrachtTelefoon = value; OnPropertyChanged(nameof(OpdrachtTelefoon)); }
        }
        public string WindowsAccount
        {
            get { return _windowsAccount; }
            set { _windowsAccount = value; OnPropertyChanged(nameof(WindowsAccount)); }
        }
        public string Cijfer
        {
            get { return _cijfer; }
            set { _cijfer = value; OnPropertyChanged(nameof(Cijfer)); }
        }
        public string Opdrachtgever
        {
            get { return _opdrachtgever; }
            set { _opdrachtgever = value; OnPropertyChanged(nameof(Opdrachtgever)); }
        }
    }
    public class UitvaartOverzichtModel : ViewModelBase
    {
        private Guid _uitvaartId;
        private string _uitvaartNr;
        private string _achternaamOverledene;
        private string _voornaamOverledene;
        private DateTime? _datumOverlijden;
        private string _uitvaartleider;
        private bool _voorregeling;

        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public string UitvaartNr
        {
            get { return _uitvaartNr; }
            set { _uitvaartNr = value; OnPropertyChanged(nameof(UitvaartNr)); }
        }
        public string AchternaamOverledene
        {
            get { return _achternaamOverledene; }
            set { _achternaamOverledene = value; OnPropertyChanged(nameof(AchternaamOverledene)); }
        }
        public string VoornaamOverledene
        {
            get { return _voornaamOverledene; }
            set { _voornaamOverledene = value; OnPropertyChanged(nameof(VoornaamOverledene)); }
        }
        public DateTime? DatumOverlijden
        {
            get { return _datumOverlijden; }
            set { _datumOverlijden = value; OnPropertyChanged(nameof(DatumOverlijden)); }
        }
        public string UitvaartLeider
        {
            get { return _uitvaartleider; }
            set { _uitvaartleider = value; OnPropertyChanged(nameof(UitvaartLeider)); }
        }
        public bool Voorregeling
        {
            get { return (bool)_voorregeling; }
            set { _voorregeling = value; OnPropertyChanged(nameof(Voorregeling)); }
        }
    }
    public class AgendaModel : ViewModelBase
    {
        private Guid _uitvaartId;
        private string _uitvaartNr;
        private string _achternaam;
        private string _voornamen;
        private DateTime? _datumTijdUitvaart;
        private TimeSpan? _tijdstipDienst;
        private string _uitvaartleider;
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public string UitvaartNr
        {
            get { return _uitvaartNr; }
            set { _uitvaartNr = value; OnPropertyChanged(nameof(UitvaartNr)); }
        }
        public string Achternaam
        {
            get { return _achternaam; }
            set { _achternaam = value; OnPropertyChanged(nameof(Achternaam)); }
        }
        public string Voornamen
        {
            get { return _voornamen; }
            set { _voornamen = value; OnPropertyChanged(nameof(Voornamen)); }
        }
        public DateTime? DatumTijdUitvaart
        {
            get { return _datumTijdUitvaart; }
            set { _datumTijdUitvaart = value; OnPropertyChanged(nameof(DatumTijdUitvaart)); }
        }
        public TimeSpan? TijdstipDienst
        {
            get { return _tijdstipDienst; }
            set { _tijdstipDienst = value; OnPropertyChanged(nameof(TijdstipDienst)); }
        }
        public string Uitvaartleider
        {
            get { return _uitvaartleider; }
            set { _uitvaartleider = value; OnPropertyChanged(nameof(Uitvaartleider)); }
        }
    }
}