using Dossier_Registratie.ViewModels;
using System;
using System.Diagnostics;

namespace Dossier_Registratie.Models
{
    public class FinancieelFilterModel : ViewModelBase
    {
        private Guid _uitvaartId;
        private string _uitvaartNummer;
        private string _overledeneAchternaam;
        private DateTime? _overledeneGeboortedatum;
        private string _startNummer;
        private string _eindNummer;

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
        public string OverledeneAchternaam
        {
            get { return _overledeneAchternaam; }
            set { _overledeneAchternaam = value; OnPropertyChanged(nameof(OverledeneAchternaam)); }
        }
        public DateTime? OverledeneGeboortedatum
        {
            get { return _overledeneGeboortedatum; }
            set { _overledeneGeboortedatum = value; OnPropertyChanged(nameof(OverledeneGeboortedatum)); }
        }
        public string StartNummer
        {
            get { return _startNummer; }
            set { _startNummer = value; OnPropertyChanged(nameof(StartNummer)); }
        }
        public string EindNummer
        {
            get { return _eindNummer; }
            set { _eindNummer = value; OnPropertyChanged(nameof(EindNummer)); }
        }
    }
    public class OverledeneSteenhouwerijModel : ViewModelBase
    {
        private Guid _steenhouwerijId;
        private Guid _uitvaartId;
        private string? _uitvaartNummer;
        private string _uitvaartLeider;
        private Guid _steenhouwerLeverancier;
        private string? _steenhouwerOpdracht;
        private string? _steenhouwerBedrag;
        private string? _steenhouwerProvisie;
        private string? _steenhouwerProvisieTotaal;
        private DateTime? _steenhouwerUitbetaing;
        private string? _steenhouwerText;
        private string? _steenhouwerLeverancierName;
        private string? _steenhouwerWerknemer;
        private bool? _steenhouwerPaid;

        public Guid SteenhouwerijId
        {
            get { return _steenhouwerijId; }
            set { _steenhouwerijId = value; OnPropertyChanged(nameof(SteenhouwerijId)); }
        }
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
        public string UitvaartLeider
        {
            get { return _uitvaartLeider; }
            set { _uitvaartLeider = value; OnPropertyChanged(nameof(UitvaartLeider)); }
        }
        public Guid SteenhouwerLeverancier
        {
            get { return _steenhouwerLeverancier; }
            set { _steenhouwerLeverancier = value; OnPropertyChanged(nameof(SteenhouwerLeverancier)); }
        }
        public string SteenhouwerOpdracht
        {
            get { return _steenhouwerOpdracht; }
            set { _steenhouwerOpdracht = value; OnPropertyChanged(nameof(SteenhouwerOpdracht)); }
        }
        public string SteenhouwerBedrag
        {
            get { return _steenhouwerBedrag; }
            set { _steenhouwerBedrag = value; OnPropertyChanged(nameof(SteenhouwerBedrag)); }
        }
        public string SteenhouwerProvisie
        {
            get { return _steenhouwerProvisie; }
            set { _steenhouwerProvisie = value; OnPropertyChanged(nameof(SteenhouwerProvisie)); }
        }
        public string SteenhouwerProvisieTotaal
        {
            get { return _steenhouwerProvisieTotaal; }
            set { _steenhouwerProvisieTotaal = value; OnPropertyChanged(nameof(SteenhouwerProvisieTotaal)); }
        }
        public DateTime? SteenhouwerUitbetaing
        {
            get { return _steenhouwerUitbetaing; }
            set { _steenhouwerUitbetaing = value; OnPropertyChanged(nameof(SteenhouwerUitbetaing)); }
        }
        public string SteenhouwerText
        {
            get { return _steenhouwerText; }
            set { _steenhouwerText = value; OnPropertyChanged(nameof(SteenhouwerText)); }
        }
        public string SteenhouwerLeverancierName
        {
            get { return _steenhouwerLeverancierName; }
            set { _steenhouwerLeverancierName = value; OnPropertyChanged(nameof(SteenhouwerLeverancierName)); }
        }
        public string SteenhouwerWerknemer
        {
            get { return _steenhouwerWerknemer; }
            set { _steenhouwerWerknemer = value; OnPropertyChanged(nameof(SteenhouwerWerknemer)); }
        }
        public bool? SteenhouwerPaid
        {
            get { return _steenhouwerPaid; }
            set { _steenhouwerPaid = value; OnPropertyChanged(nameof(SteenhouwerPaid)); }
        }
        public bool HasData()
        {
            return SteenhouwerLeverancier != Guid.Empty &&
                    !string.IsNullOrEmpty(SteenhouwerText);
        }
    }
    public class OverledeneUrnSieradenModel : ViewModelBase
    {
        private Guid _urnId;
        private Guid _uitvaartId;
        private string? _uitvaartNummer;
        private string _uitvaartLeider;
        private Guid _urnLeverancier;
        private string? _urnOpdracht;
        private string? _urnBedrag;
        private string? _urnProvisie;
        private DateTime? _urnUitbetaing;
        private string? _urnText;
        private string? _urnLeverancierName;
        private string? _urnWerknemer;
        private bool? _urnPaid;

        public Guid UrnId
        {
            get { return _urnId; }
            set { _urnId = value; OnPropertyChanged(nameof(UrnId)); }
        }
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
        public string UitvaartLeider
        {
            get { return _uitvaartLeider; }
            set { _uitvaartLeider = value; OnPropertyChanged(nameof(UitvaartLeider)); }
        }
        public Guid UrnLeverancier
        {
            get { return _urnLeverancier; }
            set { _urnLeverancier = value; OnPropertyChanged(nameof(UrnLeverancier)); }
        }
        public string UrnOpdracht
        {
            get { return _urnOpdracht; }
            set { _urnOpdracht = value; OnPropertyChanged(nameof(UrnOpdracht)); }
        }
        public string UrnBedrag
        {
            get { return _urnBedrag; }
            set { _urnBedrag = value; OnPropertyChanged(nameof(UrnBedrag)); }
        }
        public string UrnProvisie
        {
            get { return _urnProvisie; }
            set { _urnProvisie = value; OnPropertyChanged(nameof(UrnProvisie)); }
        }
        public DateTime? UrnUitbetaing
        {
            get { return _urnUitbetaing; }
            set { _urnUitbetaing = value; OnPropertyChanged(nameof(UrnUitbetaing)); }
        }
        public string UrnText
        {
            get { return _urnText; }
            set { _urnText = value; OnPropertyChanged(nameof(UrnText)); }
        }
        public string UrnLeverancierName
        {
            get { return _urnLeverancierName; }
            set { _urnLeverancierName = value; OnPropertyChanged(nameof(UrnLeverancierName)); }
        }
        public string UrnWerknemer
        {
            get { return _urnWerknemer; }
            set { _urnWerknemer = value; OnPropertyChanged(nameof(UrnWerknemer)); }
        }
        public bool? UrnPaid
        {
            get { return _urnPaid; }
            set { _urnPaid = value; OnPropertyChanged(nameof(UrnPaid)); }
        }
        public bool HasData()
        {
            return UrnLeverancier != Guid.Empty &&
                    !string.IsNullOrEmpty(UrnText);
        }
    }
    public class OverledeneBloemenModel : ViewModelBase
    {
        private Guid _bloemenId;
        private Guid _uitvaartId;
        private Guid _bloemenLeverancier;
        private string? _uitvaartNummer;
        private string? _uitvaartLeider;
        private string? _bloemenText;
        private bool? _bloemenLint = false;
        private bool? _bloemenKaart = false;
        private string? _bloemenBedrag;
        private string? _bloemenProvisie;
        private DateTime? _bloemenUitbetaling;
        private string? _bloemenLeverancierName;
        private string? _bloemenWerknemer;
        private bool? _bloemenPaid;
        private string? _bloemenDocument;
        private bool _bloemenDocumentUpdated;
        private string? _bloemenLintJson;
        private string? _bloemenBezorgAdres;
        private DateTime? _bloemenBezorgDatum;
        public Guid BloemenId
        {
            get { return _bloemenId; }
            set { _bloemenId = value; OnPropertyChanged(nameof(BloemenId)); }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid BloemenLeverancier
        {
            get { return _bloemenLeverancier; }
            set { _bloemenLeverancier = value; OnPropertyChanged(nameof(BloemenLeverancier)); }
        }
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public string UitvaartLeider
        {
            get { return _uitvaartLeider; }
            set { _uitvaartLeider = value; OnPropertyChanged(nameof(UitvaartLeider)); }
        }
        public string BloemenText
        {
            get { return _bloemenText; }
            set { _bloemenText = value; OnPropertyChanged(nameof(BloemenText)); }
        }
        public bool? BloemenLint
        {
            get { return _bloemenLint; }
            set { _bloemenLint = value; OnPropertyChanged(nameof(BloemenLint)); }
        }
        public bool? BloemenKaart
        {
            get { return _bloemenKaart; }
            set { _bloemenKaart = value; OnPropertyChanged(nameof(BloemenKaart)); }
        }
        public string BloemenBedrag
        {
            get { return _bloemenBedrag; }
            set { _bloemenBedrag = value; OnPropertyChanged(nameof(BloemenBedrag)); }
        }
        public string BloemenProvisie
        {
            get { return _bloemenProvisie; }
            set { _bloemenProvisie = value; OnPropertyChanged(nameof(BloemenProvisie)); }
        }
        public DateTime? BloemenUitbetaling
        {
            get { return _bloemenUitbetaling; }
            set { _bloemenUitbetaling = value; OnPropertyChanged(nameof(BloemenUitbetaling)); }
        }
        public string BloemenLeverancierName
        {
            get { return _bloemenLeverancierName; }
            set { _bloemenLeverancierName = value; OnPropertyChanged(nameof(BloemenLeverancierName)); }
        }
        public string BloemenWerknemer
        {
            get { return _bloemenWerknemer; }
            set { _bloemenWerknemer = value; OnPropertyChanged(nameof(BloemenWerknemer)); }
        }
        public bool? BloemenPaid
        {
            get { return _bloemenPaid; }
            set { _bloemenPaid = value; OnPropertyChanged(nameof(BloemenPaid)); }
        }
        public string? BloemenDocument
        {
            get { return _bloemenDocument; }
            set { _bloemenDocument = value; OnPropertyChanged(nameof(BloemenDocument)); }
        }
        public bool BloemenDocumentUpdated
        {
            get { return _bloemenDocumentUpdated; }
            set { _bloemenDocumentUpdated = value; OnPropertyChanged(nameof(BloemenDocumentUpdated)); }
        }
        public string? BloemenLintJson
        {
            get { return _bloemenLintJson; }
            set { _bloemenLintJson = value; OnPropertyChanged(nameof(BloemenLintJson)); }
        }
        public string? BloemenBezorgAdres
        {
            get { return _bloemenBezorgAdres; }
            set { _bloemenBezorgAdres = value; OnPropertyChanged(nameof(BloemenBezorgAdres)); }
        }
        public DateTime? BloemenBezorgDate
        {
            get { return _bloemenBezorgDatum; }
            set { _bloemenBezorgDatum = value; OnPropertyChanged(nameof(BloemenBezorgDate)); }
        }
        public bool HasData()
        {
            return BloemenLeverancier != Guid.Empty &&
                    !string.IsNullOrEmpty(BloemenText);
        }
    }
    public class OverledeneWerkbonUitvaart : ViewModelBase
    {
        private Guid _id;
        private Guid _uitvaartId;
        private string _werkbonJson;
        private string _uitvaartNummer;

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
        public string WerkbonJson
        {
            get { return _werkbonJson; }
            set { _werkbonJson = value; OnPropertyChanged(nameof(WerkbonJson)); }
        }
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public bool HasData()
        {
            return UitvaartId != Guid.Empty &&
                    !string.IsNullOrEmpty(WerkbonJson);
        }
    }
    public class WerkbonPersoneel : ViewModelBase
    {
        private Guid _id;
        private string _personeel;
        private bool _isDeleted;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string WerkbonPersoon
        {
            get { return _personeel; }
            set { _personeel = value; OnPropertyChanged(nameof(WerkbonPersoon)); }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged(nameof(IsDeleted)); }
        }

    }
    public class WerkbonnenData : ViewModelBase
    {
        private string _uitvaartNummer;
        private Guid _werknemerId;
        private string? _overig;
        private bool _rouwauto;
        private bool _rouwdienaar;
        private bool _laatsteVerzorging;
        private bool _volgauto;
        private bool _overbrengen;
        private bool _condoleance;
        private string _werknemerName;

        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public Guid WerknemerId
        {
            get { return _werknemerId; }
            set { _werknemerId = value; OnPropertyChanged(nameof(WerknemerId)); }
        }
        public string Overig
        {
            get { return _overig; }
            set { _overig = value; OnPropertyChanged(nameof(Overig)); }
        }
        public bool RouwAuto
        {
            get { return _rouwauto; }
            set { _rouwauto = value; OnPropertyChanged(nameof(RouwAuto)); }
        }
        public bool RouwDienaar
        {
            get { return _rouwdienaar; }
            set { _rouwdienaar = value; OnPropertyChanged(nameof(RouwDienaar)); }
        }
        public bool LaatsteVerzorging
        {
            get { return _laatsteVerzorging; }
            set { _laatsteVerzorging = value; OnPropertyChanged(nameof(LaatsteVerzorging)); }
        }
        public bool VolgAuto
        {
            get { return _volgauto; }
            set { _volgauto = value; OnPropertyChanged(nameof(VolgAuto)); }
        }
        public bool Overbrengen
        {
            get { return _overbrengen; }
            set { _overbrengen = value; OnPropertyChanged(nameof(Overbrengen)); }
        }
        public bool Condoleance
        {
            get { return _condoleance; }
            set { _condoleance = value; OnPropertyChanged(nameof(Condoleance)); }
        }
        public string WerknemerName
        {
            get { return _werknemerName; }
            set { _werknemerName = value; OnPropertyChanged(nameof(WerknemerName)); }
        }
    }
}
