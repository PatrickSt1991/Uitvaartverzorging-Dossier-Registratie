using Dossier_Registratie.ViewModels;
using System;
using System.ComponentModel;

namespace Dossier_Registratie.Models
{
    public class OverledeneSearchSurname : ViewModelBase
    {
        private Guid _uitvaartId;
        private string _uitvaartNummer;
        private string _overledeneAanhef;
        private string _overledeneVoornaam;
        private string _overledeneTussenvoegsel;
        private string _overledeneAchternaam;
        private DateTime _overledeneGeboortedatum;
        private string _personeelNaam;
        private Guid _personeelId;
        private bool _dossierCompleted;
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public string OverledeneAanhef
        {
            get { return _overledeneAanhef; }
            set { _overledeneAanhef = value; OnPropertyChanged(nameof(OverledeneAanhef)); }
        }
        public string OverledeneVoornaam
        {
            get { return _overledeneVoornaam; }
            set { _overledeneVoornaam = value; OnPropertyChanged(nameof(OverledeneVoornaam)); }
        }
        public string OverledeneTussenvoegsel
        {
            get { return _overledeneTussenvoegsel; }
            set { _overledeneTussenvoegsel = value; OnPropertyChanged(nameof(OverledeneTussenvoegsel)); }
        }
        public string OverledeneAchternaam
        {
            get { return _overledeneAchternaam; }
            set { _overledeneAchternaam = value; OnPropertyChanged(nameof(OverledeneAchternaam)); }
        }
        public DateTime OverledeneGeboortedatum
        {
            get { return _overledeneGeboortedatum; }
            set { _overledeneGeboortedatum = value; OnPropertyChanged(nameof(OverledeneGeboortedatum)); }
        }
        public string PersoneelNaam
        {
            get { return _personeelNaam; }
            set { _personeelNaam = value; OnPropertyChanged(nameof(PersoneelNaam)); }
        }
        public Guid PersoneelId
        {
            get { return _personeelId; }
            set { _personeelId = value; OnPropertyChanged(nameof(PersoneelId)); }
        }
        public bool DossierCompleted
        {
            get { return _dossierCompleted; }
            set { _dossierCompleted = value; OnPropertyChanged(nameof(DossierCompleted)); }
        }
    }
    public class OverledeneUitvaartleiderModel : ViewModelBase
    {
        private Guid _uitvaartId;
        private Guid _personeelId;
        private string? _personeelNaam;
        private string? _uitvaartNummer;
        private bool _dossierCompleted;
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid PersoneelId
        {
            get { return _personeelId; }
            set { _personeelId = value; OnPropertyChanged(nameof(PersoneelId)); }
        }
        public string PersoneelNaam
        {
            get { return _personeelNaam; }
            set { _personeelNaam = value; OnPropertyChanged(nameof(PersoneelNaam)); }
        }
        public string Uitvaartnummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(Uitvaartnummer)); }
        }
        public bool DossierCompleted
        {
            get { return _dossierCompleted; }
            set { _dossierCompleted = value; OnPropertyChanged(nameof(DossierCompleted)); }
        }
        public bool HasData()
        {
            return PersoneelId != Guid.Empty &&
                   !string.IsNullOrEmpty(PersoneelNaam) &&
                   !string.IsNullOrEmpty(Uitvaartnummer);
        }
    }
    public class OverledenePersoonsGegevensModel : ViewModelBase
    {
        private Guid _id;
        private Guid _uitvaartId;
        private string? _overledeneAchternaam;
        private string? _overledeneTussenvoegsel;
        private string? _overledeneVoornamen;
        private string? _overledeneAanhef;
        private DateTime? _overledeneGeboortedatum;
        private string? _overledeneGeboorteplaats;
        private string? _overledeneGemeente;
        private string? _overledeneLeeftijd;
        private string? _overledeneBSN;
        private string? _overledeneAdres;
        private string? _overledeneHuisnummer;
        private string? _overledeneHuisnummerToevoeging;
        private string? _overledenePostcode;
        private string? _overledeneWoonplaats;
        private bool _overledeneVoorregeling;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public string OverledeneAchternaam
        {
            get { return _overledeneAchternaam; }
            set
            {
                _overledeneAchternaam = value;
                OnPropertyChanged(nameof(OverledeneAchternaam));
            }
        }
        public string OverledeneTussenvoegsel
        {
            get { return _overledeneTussenvoegsel; }
            set { _overledeneTussenvoegsel = value; OnPropertyChanged(nameof(OverledeneTussenvoegsel)); }
        }
        public string OverledeneVoornamen
        {
            get { return _overledeneVoornamen; }
            set
            {
                _overledeneVoornamen = value;
                OnPropertyChanged(nameof(OverledeneVoornamen));
            }
        }
        public string OverledeneAanhef
        {
            get { return _overledeneAanhef; }
            set { _overledeneAanhef = value; OnPropertyChanged(nameof(OverledeneAanhef)); }
        }
        public DateTime? OverledeneGeboortedatum
        {
            get { return _overledeneGeboortedatum; }
            set
            {
                _overledeneGeboortedatum = value == DateTime.MinValue ? null : value;
                OnPropertyChanged(nameof(OverledeneGeboortedatum));
            }
        }
        public string OverledeneGeboorteplaats
        {
            get { return _overledeneGeboorteplaats; }
            set { _overledeneGeboorteplaats = value; OnPropertyChanged(nameof(OverledeneGeboorteplaats)); }
        }
        public string OverledeneGemeente
        {
            get { return _overledeneGemeente; }
            set { _overledeneGemeente = value; OnPropertyChanged(nameof(OverledeneGemeente)); }
        }
        public string OverledeneLeeftijd
        {
            get { return _overledeneLeeftijd; }
            set { _overledeneLeeftijd = value; OnPropertyChanged(nameof(OverledeneLeeftijd)); }
        }
        public string OverledeneBSN
        {
            get { return _overledeneBSN; }
            set { _overledeneBSN = value; OnPropertyChanged(nameof(OverledeneBSN)); }
        }
        public string OverledeneAdres
        {
            get { return _overledeneAdres; }
            set { _overledeneAdres = value; OnPropertyChanged(nameof(OverledeneAdres)); }
        }
        public string OverledeneHuisnummer
        {
            get { return _overledeneHuisnummer; }
            set { _overledeneHuisnummer = value; OnPropertyChanged(nameof(OverledeneHuisnummer)); }
        }
        public string OverledeneHuisnummerToevoeging
        {
            get { return _overledeneHuisnummerToevoeging; }
            set { _overledeneHuisnummerToevoeging = value; OnPropertyChanged(nameof(OverledeneHuisnummerToevoeging)); }
        }
        public string OverledenePostcode
        {
            get { return _overledenePostcode; }
            set { _overledenePostcode = value; OnPropertyChanged(nameof(OverledenePostcode)); }
        }
        public string OverledeneWoonplaats
        {
            get { return _overledeneWoonplaats; }
            set { _overledeneWoonplaats = value; OnPropertyChanged(nameof(OverledeneWoonplaats)); }
        }
        public bool OverledeneVoorregeling
        {
            get { return _overledeneVoorregeling; }
            set { _overledeneVoorregeling = value; OnPropertyChanged(nameof(OverledeneVoorregeling)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(OverledeneAchternaam) &&
                   !string.IsNullOrEmpty(OverledeneVoornamen) &&
                   !string.IsNullOrEmpty(OverledeneAanhef) &&
                   OverledeneGeboortedatum != DateTime.MinValue &&
                   !string.IsNullOrEmpty(OverledeneGeboorteplaats) &&
                   !string.IsNullOrEmpty(OverledeneGemeente) &&
                   !string.IsNullOrEmpty(OverledeneLeeftijd) &&
                   !string.IsNullOrEmpty(OverledeneBSN) &&
                   !string.IsNullOrEmpty(OverledeneAdres) &&
                   !string.IsNullOrEmpty(OverledeneHuisnummer) &&
                   !string.IsNullOrEmpty(OverledenePostcode) &&
                   !string.IsNullOrEmpty(OverledeneWoonplaats);
        }
    }
    public class OverledeneOverlijdenInfoModel : ViewModelBase
    {
        private Guid _id;
        private Guid _uitvaartId;
        private DateTime? _overledenDatumTijd;
        private string _overledenLocatie;
        private string _overledenAdres;
        private string _overledenePostcode;
        private string _overledenHuisnummer;
        private string _overledenHuisnummerToevoeging;
        private string _overledenPlaats;
        private string _overledenGemeente;
        private string _overledenLijkvinding;
        private Guid _overledenHerkomst;
        private string _overledenLidnummer;
        private string _overledenHuisarts;
        private string _overledenHuisartsTelefoon;
        private string _overledenSchouwarts;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public DateTime? OverledenDatumTijd
        {
            get { return _overledenDatumTijd; }
            set
            {
                if (_overledenDatumTijd != value)
                {
                    _overledenDatumTijd = value;
                    OnPropertyChanged(nameof(OverledenDatumTijd));
                    OnPropertyChanged(nameof(OverledenDatum));
                    OnPropertyChanged(nameof(OverledenTijd));
                }
            }
        }
        public DateTime? OverledenDatum
        {
            get => OverledenDatumTijd?.Date;
            set
            {
                if (OverledenDatumTijd?.Date != value)
                {
                    OverledenDatumTijd = value.HasValue
                        ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day,
                                       OverledenDatumTijd?.Hour ?? 0, OverledenDatumTijd?.Minute ?? 0, OverledenDatumTijd?.Second ?? 0)
                        : (DateTime?)null;
                    OnPropertyChanged(nameof(OverledenDatum));
                    OnPropertyChanged(nameof(OverledenTijd));
                }
            }
        }
        public TimeSpan? OverledenTijd
        {
            get => OverledenDatumTijd?.TimeOfDay;
            set
            {
                if (OverledenDatumTijd.HasValue && value.HasValue)
                {
                    OverledenDatumTijd = new DateTime(OverledenDatumTijd.Value.Year, OverledenDatumTijd.Value.Month, OverledenDatumTijd.Value.Day,
                                                      value.Value.Hours, value.Value.Minutes, value.Value.Seconds);
                }
                else if (!value.HasValue)
                {
                    OverledenDatumTijd = OverledenDatumTijd?.Date;
                }
                OnPropertyChanged(nameof(OverledenTijd));
                OnPropertyChanged(nameof(OverledenDatum));
            }
        }
        public string OverledenLocatie
        {
            get { return _overledenLocatie; }
            set { _overledenLocatie = value; OnPropertyChanged(nameof(OverledenLocatie)); }
        }
        public string OverledenAdres
        {
            get { return _overledenAdres; }
            set { _overledenAdres = value; OnPropertyChanged(nameof(OverledenAdres)); }
        }
        public string OverledenPostcode
        {
            get { return _overledenePostcode; }
            set { _overledenePostcode = value; OnPropertyChanged(nameof(OverledenPostcode)); }
        }
        public string OverledenHuisnummer
        {
            get { return _overledenHuisnummer; }
            set
            {
                _overledenHuisnummer = value;
                OnPropertyChanged(nameof(OverledenHuisnummer));
            }
        }
        public string OverledenHuisnummerToevoeging
        {
            get { return _overledenHuisnummerToevoeging; }
            set { _overledenHuisnummerToevoeging = value; OnPropertyChanged(nameof(OverledenHuisnummerToevoeging)); }
        }
        public string OverledenPlaats
        {
            get { return _overledenPlaats; }
            set { _overledenPlaats = value; OnPropertyChanged(nameof(OverledenPlaats)); }
        }
        public string OverledenGemeente
        {
            get { return _overledenGemeente; }
            set { _overledenGemeente = value; OnPropertyChanged(nameof(OverledenGemeente)); }
        }
        public string OverledenLijkvinding
        {
            get { return _overledenLijkvinding; }
            set { _overledenLijkvinding = value; OnPropertyChanged(nameof(OverledenLijkvinding)); }
        }
        public Guid OverledenHerkomst
        {
            get { return _overledenHerkomst; }
            set { _overledenHerkomst = value; OnPropertyChanged(nameof(OverledenHerkomst)); }
        }
        public string OverledenLidnummer
        {
            get { return _overledenLidnummer; }
            set { _overledenLidnummer = value; OnPropertyChanged(nameof(OverledenLidnummer)); }
        }
        public string OverledenHuisarts
        {
            get { return _overledenHuisarts; }
            set { _overledenHuisarts = value; OnPropertyChanged(nameof(OverledenHuisarts)); }
        }
        public string OverledenHuisartsTelefoon
        {
            get { return _overledenHuisartsTelefoon; }
            set { _overledenHuisartsTelefoon = value; OnPropertyChanged(nameof(OverledenHuisartsTelefoon)); }
        }
        public string OverledenSchouwarts
        {
            get { return _overledenSchouwarts; }
            set { _overledenSchouwarts = value; OnPropertyChanged(nameof(OverledenSchouwarts)); }
        }
        public bool HasData()
        {
            return OverledenDatumTijd != DateTime.MinValue &&
                   !string.IsNullOrEmpty(OverledenAdres) &&
                   !string.IsNullOrEmpty(OverledenHuisnummer) &&
                   !string.IsNullOrEmpty(OverledenPlaats) &&
                   !string.IsNullOrEmpty(OverledenGemeente) &&
                   !string.IsNullOrEmpty(OverledenLijkvinding) &&
                   OverledenHerkomst != Guid.Empty &&
                   !string.IsNullOrEmpty(OverledenLidnummer);
        }
    }
}