using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class KostenbegrotingInfoModel : ViewModelBase
    {
        private string _uitvaartNummer;
        private string _overledeneNaam;
        private string _overledeneAanhef;
        private string _overledeneVoornaam;
        private string _overledeneAchternaam;
        private string _opdrachtgeverAanhef;
        private string _opdrachtgeverVoornaam;
        private string _opdrachtgeverAchternaam;
        private string _opdrachtgeverStraat;
        private string _opdrachtgeverPostcode;
        private string _opdrachtgeverWoonplaats;
        private DateTime _overledenDatum;
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public string OverledeneNaam
        {
            get { return _overledeneNaam; }
            set { _overledeneNaam = value; OnPropertyChanged(nameof(OverledeneNaam)); }
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
        public string OverledeneAchternaam
        {
            get { return _overledeneAchternaam; }
            set { _overledeneAchternaam = value; OnPropertyChanged(nameof(OverledeneAchternaam)); }
        }
        public string OpdrachtgeverAanhef
        {
            get { return _opdrachtgeverAanhef; }
            set { _opdrachtgeverAanhef = value; OnPropertyChanged(nameof(OpdrachtgeverAanhef)); }
        }
        public string OpdrachtgeverVoornaam
        {
            get { return _opdrachtgeverVoornaam; }
            set { _opdrachtgeverVoornaam = value; OnPropertyChanged(nameof(OpdrachtgeverVoornaam)); }
        }
        public string OpdrachtgeverAchternaam
        {
            get { return _opdrachtgeverAchternaam; }
            set { _opdrachtgeverAchternaam = value; OnPropertyChanged(nameof(OpdrachtgeverAchternaam)); }
        }
        public string OpdrachtgeverStraat
        {
            get { return _opdrachtgeverStraat; }
            set { _opdrachtgeverStraat = value; OnPropertyChanged(nameof(OpdrachtgeverStraat)); }
        }
        public string OpdrachtgeverPostcode
        {
            get { return _opdrachtgeverPostcode; }
            set { _opdrachtgeverPostcode = value; OnPropertyChanged(nameof(OpdrachtgeverPostcode)); }
        }
        public string OpdrachtgeverWoonplaats
        {
            get { return _opdrachtgeverWoonplaats; }
            set { _opdrachtgeverWoonplaats = value; OnPropertyChanged(nameof(OpdrachtgeverWoonplaats)); }
        }
        public DateTime OverledenDatum
        {
            get { return _overledenDatum; }
            set { _overledenDatum = value; OnPropertyChanged(nameof(OverledenDatum)); }
        }
    }
    public class GenerateFactuur : ViewModelBase
    {
        public Guid _uitvaartId;
        private string _uitvaartNummer;
        private string _kostenbegrotingJson;
        private string _opdrachtgeverAanhef;
        private string _opdrachtgeverVoornamen;
        private string _opdrachtgeverTussenvoegsel;
        private string _opdrachtgeverAchternaam;
        private string _opdrachtgeverStraat;
        private string _opdrachtgeverHuisnummer;
        private string _opdrachtgeverHuisnummerToevoeging;
        private string _opdrachtgeverPostcode;
        private string _opdrachtgeverWoonplaats;
        private string _overledeneAanhef;
        private string _overledeneVoornamen;
        private string _overledeneTussenvoegsel;
        private string _overledeneAchternaam;
        private DateTime _overledenOpDatum;
        private string _overledenLidnummer;
        private string _overledenVerzekeringJson;
        private string _factuurType;
        private string _herkomstName;
        private string _herkomstStreet;
        private string _herkomstHousenumber;
        private string _herkomstHousenumberAddition;
        private string _herkomstPostbus;
        private string _herkomstPostbusName;
        private string _herkomstZipcode;
        private string _herkomstCity;
        private string _correspondentieType;
        private bool _overledeneVoorregeling;

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
        public string KostenbegrotingJson
        {
            get { return _kostenbegrotingJson; }
            set { _kostenbegrotingJson = value; OnPropertyChanged(nameof(KostenbegrotingJson)); }
        }
        public string OpdrachtgeverAanhef
        {
            get { return _opdrachtgeverAanhef; }
            set { _opdrachtgeverAanhef = value; OnPropertyChanged(nameof(OpdrachtgeverAanhef)); }
        }
        public string OpdrachtgeverVoornamen
        {
            get { return _opdrachtgeverVoornamen; }
            set { _opdrachtgeverVoornamen = value; OnPropertyChanged(nameof(OpdrachtgeverVoornamen)); }
        }
        public string OpdrachtgeverTussenvoegsel
        {
            get { return _opdrachtgeverTussenvoegsel; }
            set { _opdrachtgeverTussenvoegsel = value; OnPropertyChanged(nameof(OpdrachtgeverTussenvoegsel)); }
        }
        public string OpdrachtgeverAchternaam
        {
            get { return _opdrachtgeverAchternaam; }
            set { _opdrachtgeverAchternaam = value; OnPropertyChanged(nameof(OpdrachtgeverAchternaam)); }
        }
        public string OpdrachtgeverStraat
        {
            get { return _opdrachtgeverStraat; }
            set { _opdrachtgeverStraat = value; OnPropertyChanged(nameof(OpdrachtgeverStraat)); }
        }
        public string OpdrachtgeverHuisnummer
        {
            get { return _opdrachtgeverHuisnummer; }
            set { _opdrachtgeverHuisnummer = value; OnPropertyChanged(nameof(OpdrachtgeverHuisnummer)); }
        }
        public string OpdrachtgeverHuisnummerToevoeging
        {
            get { return _opdrachtgeverHuisnummerToevoeging; }
            set { _opdrachtgeverHuisnummerToevoeging = value; OnPropertyChanged(nameof(OpdrachtgeverHuisnummerToevoeging)); }
        }
        public string OpdrachtgeverPostcode
        {
            get { return _opdrachtgeverPostcode; }
            set { _opdrachtgeverPostcode = value; OnPropertyChanged(nameof(OpdrachtgeverPostcode)); }
        }
        public string OpdrachtgeverWoonplaats
        {
            get { return _opdrachtgeverWoonplaats; }
            set { _opdrachtgeverWoonplaats = value; OnPropertyChanged(nameof(OpdrachtgeverWoonplaats)); }
        }
        public string OverledeneAanhef
        {
            get { return _overledeneAanhef; }
            set { _overledeneAanhef = value; OnPropertyChanged(nameof(OverledeneAanhef)); }
        }
        public string OverledeneVoornamen
        {
            get { return _overledeneVoornamen; }
            set { _overledeneVoornamen = value; OnPropertyChanged(nameof(OverledeneVoornamen)); }
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
        public DateTime OverledeneOpDatum
        {
            get { return _overledenOpDatum.Date; }
            set { _overledenOpDatum = value.Date; OnPropertyChanged(nameof(OverledeneOpDatum)); }
        }
        public string OverledeneLidnummer
        {
            get { return _overledenLidnummer; }
            set { _overledenLidnummer = value; OnPropertyChanged(nameof(_overledenLidnummer)); }
        }
        public string OverledeneVerzekeringJson
        {
            get { return _overledenVerzekeringJson; }
            set { _overledenVerzekeringJson = value; OnPropertyChanged(nameof(OverledeneVerzekeringJson)); }
        }
        public string FactuurType
        {
            get { return _factuurType; }
            set { _factuurType = value; OnPropertyChanged(nameof(FactuurType)); }
        }
        public string HerkomstName
        {
            get { return _herkomstName; }
            set { _herkomstName = value; OnPropertyChanged(nameof(HerkomstName)); }
        }
        public string HerkomstStreet
        {
            get { return _herkomstStreet; }
            set { _herkomstStreet = value; OnPropertyChanged(nameof(HerkomstStreet)); }
        }
        public string HerkomstHousenumber
        {
            get { return _herkomstHousenumber; }
            set { _herkomstHousenumber = value; OnPropertyChanged(nameof(HerkomstHousenumber)); }
        }
        public string HerkomstPostbusName
        {
            get { return _herkomstPostbusName; }
            set { _herkomstPostbusName = value; OnPropertyChanged(nameof(HerkomstPostbusName)); }
        }
        public string HerkomstPostbus
        {
            get { return _herkomstPostbus; }
            set { _herkomstPostbus = value; OnPropertyChanged(nameof(HerkomstPostbus)); }
        }
        public string HerkomstHousenumberAddition
        {
            get { return _herkomstHousenumberAddition; }
            set { _herkomstHousenumberAddition = value; OnPropertyChanged(nameof(HerkomstHousenumberAddition)); }
        }
        public string HerkomstZipcode
        {
            get { return _herkomstZipcode; }
            set { _herkomstZipcode = value; OnPropertyChanged(nameof(HerkomstZipcode)); }
        }
        public string HerkomstCity
        {
            get { return _herkomstCity; }
            set { _herkomstCity = value; OnPropertyChanged(nameof(HerkomstCity)); }
        }
        public string CorrespondentieType
        {
            get { return _correspondentieType; }
            set { _correspondentieType = value; OnPropertyChanged(nameof(CorrespondentieType)); }
        }
        public bool OverledeneVoorregeling
        {
            get { return _overledeneVoorregeling; }
            set { _overledeneVoorregeling = value; OnPropertyChanged(nameof(OverledeneVoorregeling)); }
        }
    }
    public class FactuurModel : ViewModelBase
    {
        private Guid _id;
        private Guid _uitvaartId;
        private string _uitvaartNummer;
        private Guid _kostenbegrotingVerzekeraar;
        private string _kostenbegrotingUrl;
        private string _kostenbegrotingJson;
        private DateTime _kostenbegrotingCreationDate;
        private bool _kostenbegrotingCreated;
        private bool _factuurCreated;
        private DateTime _factuurCreationDate;
        private string _factuurOpdrachtgeverUrl;
        private string _factuurVerenigingUrl;
        private string _polisJson;
        private decimal _korting;

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
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public Guid KostenbegrotingVerzekeraar
        {
            get { return _kostenbegrotingVerzekeraar; }
            set { _kostenbegrotingVerzekeraar = value; OnPropertyChanged(nameof(KostenbegrotingVerzekeraar)); }
        }
        public string KostenbegrotingUrl
        {
            get { return _kostenbegrotingUrl; }
            set { _kostenbegrotingUrl = value; OnPropertyChanged(nameof(KostenbegrotingUrl)); }
        }
        public string KostenbegrotingJson
        {
            get { return _kostenbegrotingJson; }
            set { _kostenbegrotingJson = value; OnPropertyChanged(nameof(KostenbegrotingJson)); }
        }
        public bool KostenbegrotingCreated
        {
            get { return _kostenbegrotingCreated; }
            set { _kostenbegrotingCreated = value; OnPropertyChanged(nameof(KostenbegrotingCreated)); }
        }
        public DateTime KostenbegrotingCreationDate
        {
            get { return _kostenbegrotingCreationDate; }
            set { _kostenbegrotingCreationDate = value; OnPropertyChanged(nameof(KostenbegrotingCreationDate)); }
        }
        public bool FactuurCreated
        {
            get { return _factuurCreated; }
            set { _factuurCreated = value; OnPropertyChanged(nameof(FactuurCreated)); }
        }
        public DateTime FactuurCreationDate
        {
            get { return _factuurCreationDate; }
            set { _factuurCreationDate = value; OnPropertyChanged(nameof(FactuurCreationDate)); }
        }
        public string FactuurOpdrachtgeverUrl
        {
            get { return _factuurOpdrachtgeverUrl; }
            set { _factuurOpdrachtgeverUrl = value; OnPropertyChanged(nameof(FactuurOpdrachtgeverUrl)); }
        }
        public string FactuurVerenigingUrl
        {
            get { return _factuurVerenigingUrl; }
            set { _factuurVerenigingUrl = value; OnPropertyChanged(nameof(FactuurVerenigingUrl)); }
        }
        public string PolisJson
        {
            get { return _polisJson; }
            set { _polisJson = value; OnPropertyChanged(nameof(PolisJson)); }
        }
        public decimal Korting
        {
            get { return _korting; }
            set { _korting = value; OnPropertyChanged(nameof(Korting)); }
        }
    }
    public class VerzekeringKbModel : ViewModelBase
    {
        public Guid Id { get; set; }
    }
    public class KostenbegrotingModel : ViewModelBase
    {
        private Guid _id;
        private Guid _componentId;
        private string _componentOmschrijving;
        private string _componentAantal;
        private string _componentVerzekering;
        private string _componentVerzekeringJson;
        private decimal _componentBedrag;
        private decimal _componentFactuurBedrag;
        private bool _isDeleted;
        private bool? _specificCrematie;
        private bool? _specificBegrafenis;
        private bool? _specificPakket;
        private bool _defaultPM = false;
        private string _btnBrush;
        private int? _sortOrder;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public Guid ComponentId
        {
            get { return _componentId; }
            set { _componentId = value; OnPropertyChanged(nameof(ComponentId)); }
        }
        public string ComponentOmschrijving
        {
            get { return _componentOmschrijving; }
            set { _componentOmschrijving = value; OnPropertyChanged(nameof(ComponentOmschrijving)); }
        }
        public string ComponentAantal
        {
            get { return _componentAantal; }
            set { _componentAantal = value; OnPropertyChanged(nameof(ComponentAantal)); }
        }
        public string ComponentVerzekering
        {
            get { return _componentVerzekering; }
            set { _componentVerzekering = value; OnPropertyChanged(nameof(ComponentVerzekering)); }
        }
        public string ComponentVerzekeringJson
        {
            get { return _componentVerzekeringJson; }
            set { _componentVerzekeringJson = value; OnPropertyChanged(nameof(ComponentVerzekeringJson)); }
        }
        public decimal ComponentBedrag
        {
            get { return _componentBedrag; }
            set { _componentBedrag = value; OnPropertyChanged(nameof(ComponentBedrag)); }
        }
        public decimal ComponentFactuurBedrag
        {
            get { return _componentFactuurBedrag; }
            set { _componentFactuurBedrag = value; OnPropertyChanged(nameof(ComponentFactuurBedrag)); }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged(nameof(IsDeleted)); }
        }
        public bool? SpecificCrematie
        {
            get { return _specificCrematie; }
            set { _specificCrematie = value; OnPropertyChanged(nameof(SpecificCrematie)); }
        }
        public bool? SpecificBegrafenis
        {
            get { return _specificBegrafenis; }
            set { _specificBegrafenis = value; OnPropertyChanged(nameof(SpecificBegrafenis)); }
        }
        public bool? SpecificPakket
        {
            get { return _specificPakket; }
            set { _specificPakket = value; OnPropertyChanged(nameof(SpecificPakket)); }
        }
        public bool DefaultPM
        {
            get { return _defaultPM; }
            set { _defaultPM = value; OnPropertyChanged(nameof(DefaultPM)); }
        }
        public string BtnBrush
        {
            get { return _btnBrush; }
            set
            {
                _btnBrush = value;
                OnPropertyChanged(nameof(BtnBrush));
            }
        }
        public int? SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; OnPropertyChanged(nameof(SortOrder)); }
        }

        public bool HasData()
        {
            return SortOrder.HasValue && !string.IsNullOrEmpty(ComponentOmschrijving);
        }

    }
    public class GeneratedKostenbegrotingModel : ViewModelBase
    {
        private bool _specificCrematie;
        private bool _specificBegrafenis;
        private bool _specificPakket;
        private bool _componentDeleted;
        private string _componentOmschrijving;
        private string _componentAantal;
        private string _componentOrgAantal;
        private string _componentVerzekering;
        private decimal? _componentBedrag;
        private decimal? _componentFactuurBedrag;
        private decimal? _componentOrgBedrag;
        private Guid _componentId;
        private int? _sortOrder;
        private bool _pmAmount = false;
        private bool _printTrue = false;

        public bool SpecificCrematie
        {
            get { return _specificCrematie; }
            set { _specificCrematie = value; OnPropertyChanged(nameof(SpecificCrematie)); }
        }
        public bool SpecificBegrafenis
        {
            get { return _specificBegrafenis; }
            set { _specificBegrafenis = value; OnPropertyChanged(nameof(SpecificBegrafenis)); }
        }
        public bool SpecificPakket
        {
            get { return _specificPakket; }
            set { _specificPakket = value; OnPropertyChanged(nameof(SpecificPakket)); }
        }
        public bool IsDeleted
        {
            get { return _componentDeleted; }
            set { _componentDeleted = value; OnPropertyChanged(nameof(IsDeleted)); }
        }
        public string Omschrijving
        {
            get { return _componentOmschrijving; }
            set { _componentOmschrijving = value; OnPropertyChanged(nameof(Omschrijving)); }
        }
        public string Aantal
        {
            get { return _componentAantal; }
            set { _componentAantal = value; OnPropertyChanged(nameof(Aantal)); }
        }
        public string OrgAantal
        {
            get { return _componentOrgAantal; }
            set { _componentOrgAantal = value; OnPropertyChanged(nameof(OrgAantal)); }
        }
        public string Verzekerd
        {
            get { return _componentVerzekering; }
            set { _componentVerzekering = value; OnPropertyChanged(nameof(Verzekerd)); }
        }
        public decimal? Bedrag
        {
            get { return _componentBedrag; }
            set
            {
                if (_componentBedrag != value)
                {
                    _componentBedrag = value;
                    OnPropertyChanged(nameof(Bedrag));
                }
            }
        }
        public decimal? FactuurBedrag
        {
            get { return _componentFactuurBedrag; }
            set { _componentFactuurBedrag = value; OnPropertyChanged(nameof(FactuurBedrag)); }
        }
        public decimal? OrgBedrag
        {
            get { return _componentOrgBedrag; }
            set
            {
                if (_componentOrgBedrag != value)
                {
                    _componentOrgBedrag = value;
                    OnPropertyChanged(nameof(OrgBedrag));
                }
            }
        }
        public Guid Id
        {
            get { return _componentId; }
            set { _componentId = value; OnPropertyChanged(nameof(Id)); }
        }
        public int? SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; OnPropertyChanged(nameof(SortOrder)); }
        }
        public bool PmAmount
        {
            get { return _pmAmount; }
            set { _pmAmount = value; OnPropertyChanged(nameof(PmAmount)); }
        }
        public bool PrintTrue
        {
            get { return _printTrue; }
            set { _printTrue = value; OnPropertyChanged(nameof(PrintTrue)); }
        }
    }
}
