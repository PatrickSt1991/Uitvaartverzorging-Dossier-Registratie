using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class Filter : ViewModelBase
    {
        private int _lowestNumber;
        private int _highestNumber;

        public int LowestNumber
        {
            get { return _lowestNumber; }
            set { _lowestNumber = value; OnPropertyChanged(nameof(LowestNumber)); }
        }
        public int HighestNumber
        {
            get { return _highestNumber; }
            set { _highestNumber = value; OnPropertyChanged(nameof(HighestNumber)); }
        }
    }
    public class RapportageKlantWerknemerScores : ViewModelBase
    {
        private Guid _employeeId;
        private string _uitvaartNr;
        private string _employeeName;
        private int _cijfer;
        private decimal _gemiddeldCijfer;
        private int _totalUitvaarten;
        private string _uitvaartVan;

        public Guid EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; OnPropertyChanged(nameof(EmployeeId)); }
        }
        public string UitvaartNr
        {
            get { return _uitvaartNr; }
            set { _uitvaartNr = value; OnPropertyChanged(nameof(UitvaartNr)); }
        }
        public string EmployeeName
        {
            get { return _employeeName; }
            set { _employeeName = value; OnPropertyChanged(nameof(EmployeeName)); }
        }
        public int Cijfer
        {
            get { return _cijfer; }
            set { _cijfer = value; OnPropertyChanged(nameof(Cijfer)); }
        }
        public decimal GemiddeldCijfer
        {
            get { return _gemiddeldCijfer; }
            set { _gemiddeldCijfer = value; OnPropertyChanged(nameof(GemiddeldCijfer)); }
        }
        public int TotalUitvaarten
        {
            get { return _totalUitvaarten; }
            set { _totalUitvaarten = value; OnPropertyChanged(nameof(TotalUitvaarten)); }
        }
        public string UitvaartVan
        {
            get { return _uitvaartVan; }
            set { _uitvaartVan = value; OnPropertyChanged(nameof(UitvaartVan)); }
        }
    }
    public class RapportagesFilter : ViewModelBase
    {
        private string _startNummer;
        private string _endNummer;
        public string StartNummer
        {
            get { return _startNummer; }
            set { _startNummer = value; OnPropertyChanged(nameof(StartNummer)); }
        }
        public string EndNummer
        {
            get { return _endNummer; }
            set { _endNummer = value; OnPropertyChanged(nameof(EndNummer)); }
        }
    }
    public class RapportagesKisten : ViewModelBase
    {
        private string _kistTypeNummer;
        private string _kistOmschrijving;
        private int _kistCount;

        public string KistTypeNummer
        {
            get { return _kistTypeNummer; }
            set { _kistTypeNummer = value; OnPropertyChanged(nameof(KistTypeNummer)); }
        }
        public string KistOmschrijving
        {
            get { return _kistOmschrijving; }
            set { _kistOmschrijving = value; OnPropertyChanged(nameof(KistOmschrijving)); }
        }
        public int KistCount
        {
            get { return _kistCount; }
            set { _kistCount = value; OnPropertyChanged(nameof(KistCount)); }
        }
    }
    public class RapportagesVerzekering : ViewModelBase
    {
        private string _verzekeringHerkomst;
        private int _verzekeringHerkomstCount;
        private string _verzekeringWoonplaats;

        public string VerzekeringHerkomst
        {
            get { return _verzekeringHerkomst; }
            set
            {
                _verzekeringHerkomst = value; OnPropertyChanged(nameof(VerzekeringHerkomst));
            }
        }
        public int VerzekeringHerkomstCount
        {
            get { return _verzekeringHerkomstCount; }
            set
            {
                _verzekeringHerkomstCount = value; OnPropertyChanged(nameof(VerzekeringHerkomstCount));
            }
        }
        public string VerzekeringWoonplaats
        {
            get { return _verzekeringWoonplaats; }
            set
            {
                _verzekeringWoonplaats = value; OnPropertyChanged(nameof(VerzekeringWoonplaats));
            }
        }
    }
    public class RapportagesUitvaartleider : ViewModelBase
    {
        private string _uitvaartleider;
        private int _uitvaartnummer;

        public string Uitvaartleider
        {
            get { return _uitvaartleider; }
            set
            {
                _uitvaartleider = value;
                OnPropertyChanged(nameof(Uitvaartleider));
            }
        }
        public int Uitvaartnummer
        {
            get { return _uitvaartnummer; }
            set
            {
                _uitvaartnummer = value;
                OnPropertyChanged(nameof(Uitvaartnummer));
            }
        }
    }
    public class Instellingen : ViewModelBase
    {
        private Guid _id;
        private string _settingName;
        private string _settingValue;
        private string _settingDescription;
        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string SettingName
        {
            get { return _settingName; }
            set
            {
                _settingName = value;
                OnPropertyChanged(nameof(SettingName));
            }
        }
        public string SettingValue
        {
            get { return _settingValue; }
            set
            {
                _settingValue = value;
                OnPropertyChanged(nameof(SettingValue));
            }
        }
        public string SettingDescription
        {
            get { return _settingDescription; }
            set
            {
                _settingDescription = value;
                OnPropertyChanged(nameof(SettingDescription));
            }
        }
    }
    public class Volgautos : ViewModelBase
    {
        private Guid _id;
        private string? _verzekeringNaam;
        private int? _aantalVolgautos;
        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string? VerzekeringNaam
        {
            get { return _verzekeringNaam; }
            set { _verzekeringNaam = value; OnPropertyChanged(nameof(VerzekeringNaam)); }
        }
        public int? AantalVolgautos
        {
            get { return _aantalVolgautos; }
            set { _aantalVolgautos = value; OnPropertyChanged(nameof(AantalVolgautos)); }
        }
    }
    public class TevredenheidCijfer : ViewModelBase
    {
        private Guid _id;
        private int _aantalUitvaarten;
        private string _uitvaartverzorger;
        private int _gemiddeldCijfer;
        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string Uitvaartverzorger
        {
            get { return _uitvaartverzorger; }
            set { _uitvaartverzorger = value; OnPropertyChanged(nameof(Uitvaartverzorger)); }
        }
        public int AantalUitvaarten
        {
            get { return _aantalUitvaarten; }
            set { _aantalUitvaarten = value; OnPropertyChanged(nameof(AantalUitvaarten)); }
        }
        public int GemiddeldCijfer
        {
            get { return _gemiddeldCijfer; }
            set { _gemiddeldCijfer = value; OnPropertyChanged(nameof(GemiddeldCijfer)); }
        }
    }
    public class PeriodeLijst : ViewModelBase
    {
        private Guid _id;
        private string _uitvaartNummer;
        private string _uitvaartNaam;
        private string _voornamen;
        private string _datumOverlijden;
        private string _uitvaartType;
        private string _verzekering;
        private string _factuur;
        private string _opdrachtgeverFactuur;
        private string _herkomstFactuur;

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public string UitvaartNaam
        {
            get { return _uitvaartNaam; }
            set { _uitvaartNaam = value; OnPropertyChanged(nameof(UitvaartNaam)); }
        }
        public string Voornamen
        {
            get { return _voornamen; }
            set { _voornamen = value; OnPropertyChanged(nameof(Voornamen)); }
        }
        public string DatumOverlijden
        {
            get { return _datumOverlijden; }
            set { _datumOverlijden = value; OnPropertyChanged(nameof(DatumOverlijden)); }
        }
        public string UitvaartType
        {
            get { return _uitvaartType; }
            set { _uitvaartType = value; OnPropertyChanged(nameof(UitvaartType)); }
        }
        public string Verzekering
        {
            get { return _verzekering; }
            set { _verzekering = value; OnPropertyChanged(nameof(Verzekering)); }
        }
        public string Factuur
        {
            get { return _factuur; }
            set { _factuur = value; OnPropertyChanged(nameof(Factuur)); }
        }
        public string OpdrachtgeverFactuur
        {
            get { return _opdrachtgeverFactuur; }
            set { _opdrachtgeverFactuur = value; OnPropertyChanged(nameof(OpdrachtgeverFactuur)); }
        }
        public string HerkomstFactuur
        {
            get { return _herkomstFactuur; }
            set { _herkomstFactuur = value; OnPropertyChanged(nameof(HerkomstFactuur)); }
        }
    }
    public class FactuurJson : ViewModelBase
    {
        public string OpdrachtgeverFactuurUrl { get; set; }
        public string VerenigingFactuurUrl { get; set; }
    }

}