using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class TevredenheidDocument : ViewModelBase
    {
        private string _dossiernummer;
        private string _ingevuldDoorAdres;
        private string _ingevuldDoorNaam;
        private string _ingevuldDoorTelefoon;
        private string _ingevuldDoorWoonplaats;
        private string _uitvaartverzorger;
        private bool _updated = false;
        private bool _initialCreation = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;

        public string Dossiernummer
        {
            get { return _dossiernummer; }
            set { _dossiernummer = value; OnPropertyChanged(nameof(Dossiernummer)); }
        }
        public string IngevuldDoorAdres
        {
            get { return _ingevuldDoorAdres; }
            set { _ingevuldDoorAdres = value; OnPropertyChanged(nameof(IngevuldDoorAdres)); }
        }
        public string IngevuldDoorNaam
        {
            get { return _ingevuldDoorNaam; }
            set { _ingevuldDoorNaam = value; OnPropertyChanged(nameof(IngevuldDoorNaam)); }
        }
        public string IngevuldDoorTelefoon
        {
            get { return _ingevuldDoorTelefoon; }
            set { _ingevuldDoorTelefoon = value; OnPropertyChanged(nameof(IngevuldDoorTelefoon)); }
        }
        public string IngevuldDoorWoonplaats
        {
            get { return _ingevuldDoorWoonplaats; }
            set { _ingevuldDoorWoonplaats = value; OnPropertyChanged(nameof(IngevuldDoorWoonplaats)); }
        }
        public string Uitvaartverzorger
        {
            get { return _uitvaartverzorger; }
            set { _uitvaartverzorger = value; OnPropertyChanged(nameof(Uitvaartverzorger)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(Dossiernummer) ||
                    !string.IsNullOrEmpty(IngevuldDoorAdres) ||
                    !string.IsNullOrEmpty(IngevuldDoorNaam) ||
                    !string.IsNullOrEmpty(IngevuldDoorTelefoon) ||
                    !string.IsNullOrEmpty(IngevuldDoorWoonplaats) ||
                    !string.IsNullOrEmpty(Uitvaartverzorger);
        }
    }
    public class TerugmeldingDocument : ViewModelBase
    {
        private string _dossiernummer;
        private string _uitvaartverzorger;
        private string _uitvaartverzorgerEmail;
        private string _polisnummer;
        private string _overledeneAanhef;
        private string _overledeneNaam;
        private string _overledeneVoornamen;
        private string _overledeneAdres;
        private string _overledenePostcode;
        private string _overledeneWoonplaats;
        private string _overledeneGeborenTe;
        private DateTime _overledeneOverledenOp;
        private string _overledeneOverledenTe;
        private DateTime _overledeneUitvaartDatum;
        private DateTime _overledeneUitvaartTijd;
        private string _overledeneType;
        private string _overledeneUitvaartTe;

        private string _opdrachtgeverNaam;
        private string _opdrachtgeverAdres;
        private string _opdrachtgeverPostcode;
        private string _opdrachtgeverPlaats;
        private string _opdrachtgeverRelatie;
        private string _opdrachtgeverTelefoon;

        private bool _updated = false;
        private bool _initialCreation = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;

        public string Dossiernummer
        {
            get { return _dossiernummer; }
            set { _dossiernummer = value; OnPropertyChanged(nameof(Dossiernummer)); }
        }
        public string Uitvaartverzorger
        {
            get { return _uitvaartverzorger; }
            set { _uitvaartverzorger = value; OnPropertyChanged(nameof(Uitvaartverzorger)); }
        }
        public string UitvaartverzorgerEmail
        {
            get { return _uitvaartverzorgerEmail; }
            set { _uitvaartverzorgerEmail = value; OnPropertyChanged(nameof(UitvaartverzorgerEmail)); }
        }
        public string Polisnummer
        {
            get { return _polisnummer; }
            set { _polisnummer = value; OnPropertyChanged(nameof(Polisnummer)); }
        }
        public string OverledeneAanhef
        {
            get { return _overledeneAanhef; }
            set { _overledeneAanhef = value; OnPropertyChanged(nameof(OverledeneAanhef)); }
        }
        public string OverledeneNaam
        {
            get { return _overledeneNaam; }
            set { _overledeneNaam = value; OnPropertyChanged(nameof(OverledeneNaam)); }
        }
        public string OverledeneVoornamen
        {
            get { return _overledeneVoornamen; }
            set { _overledeneVoornamen = value; OnPropertyChanged(nameof(OverledeneVoornamen)); }
        }
        public string OverledeneAdres
        {
            get { return _overledeneAdres; }
            set { _overledeneAdres = value; OnPropertyChanged(nameof(OverledeneAdres)); }
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
        public string OverledeneGeborenTe
        {
            get { return _overledeneGeborenTe; }
            set { _overledeneGeborenTe = value; OnPropertyChanged(nameof(OverledeneGeborenTe)); }
        }
        public DateTime OverledeneOverledenOp
        {
            get { return _overledeneOverledenOp; }
            set { _overledeneOverledenOp = value; OnPropertyChanged(nameof(OverledeneOverledenOp)); }
        }
        public string OverledeneOverledenTe
        {
            get { return _overledeneOverledenTe; }
            set { _overledeneOverledenTe = value; OnPropertyChanged(nameof(OverledeneOverledenTe)); }
        }
        public DateTime OverledeneUitvaartDatum
        {
            get { return _overledeneUitvaartDatum; }
            set { _overledeneUitvaartDatum = value; OnPropertyChanged(nameof(OverledeneUitvaartDatum)); }
        }
        public DateTime OverledeneUitvaartTijd
        {
            get { return _overledeneUitvaartTijd; }
            set { _overledeneUitvaartTijd = value; OnPropertyChanged(nameof(OverledeneUitvaartTijd)); }
        }
        public string OverledeneType
        {
            get { return _overledeneType; }
            set { _overledeneType = value; OnPropertyChanged(nameof(OverledeneType)); }
        }
        public string OverledeneUitvaartTe
        {
            get { return _overledeneUitvaartTe; }
            set { _overledeneUitvaartTe = value; OnPropertyChanged(nameof(OverledeneUitvaartTe)); }
        }
        public string OpdrachtgeverNaam
        {
            get { return _opdrachtgeverNaam; }
            set { _opdrachtgeverNaam = value; OnPropertyChanged(nameof(OpdrachtgeverNaam)); }
        }
        public string OpdrachtgeverAdres
        {
            get { return _opdrachtgeverAdres; }
            set { _opdrachtgeverAdres = value; OnPropertyChanged(nameof(OpdrachtgeverAdres)); }
        }
        public string OpdrachtgeverPostcode
        {
            get { return _opdrachtgeverPostcode; }
            set { _opdrachtgeverPostcode = value; OnPropertyChanged(nameof(OpdrachtgeverPostcode)); }
        }
        public string OpdrachtgeverPlaats
        {
            get { return _opdrachtgeverPlaats; }
            set { _opdrachtgeverPlaats = value; OnPropertyChanged(nameof(OpdrachtgeverPlaats)); }
        }
        public string OpdrachtgeverRelatie
        {
            get { return _opdrachtgeverRelatie; }
            set { _opdrachtgeverRelatie = value; OnPropertyChanged(nameof(OpdrachtgeverRelatie)); }
        }
        public string OpdrachtgeverTelefoon
        {
            get { return _opdrachtgeverTelefoon; }
            set { _opdrachtgeverTelefoon = value; OnPropertyChanged(nameof(OpdrachtgeverTelefoon)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(Dossiernummer) ||
                    !string.IsNullOrEmpty(Uitvaartverzorger) ||
                    !string.IsNullOrEmpty(UitvaartverzorgerEmail) ||
                    !string.IsNullOrEmpty(Polisnummer) ||
                    !string.IsNullOrEmpty(OverledeneAanhef) ||
                    !string.IsNullOrEmpty(OverledeneNaam) ||
                    !string.IsNullOrEmpty(OverledeneVoornamen) ||
                    !string.IsNullOrEmpty(OverledeneAdres) ||
                    !string.IsNullOrEmpty(OverledenePostcode) ||
                    !string.IsNullOrEmpty(OverledeneWoonplaats) ||
                    !string.IsNullOrEmpty(OverledeneGeborenTe) ||
                    OverledeneOverledenOp != DateTime.MinValue ||
                    !string.IsNullOrEmpty(OverledeneOverledenTe) ||
                    OverledeneUitvaartDatum != DateTime.MinValue ||
                    OverledeneUitvaartTijd != DateTime.MinValue ||
                    !string.IsNullOrEmpty(OverledeneType) ||
                    !string.IsNullOrEmpty(OverledeneUitvaartTe) ||
                    !string.IsNullOrEmpty(OpdrachtgeverNaam) ||
                    !string.IsNullOrEmpty(OpdrachtgeverAdres) ||
                    !string.IsNullOrEmpty(OpdrachtgeverPostcode) ||
                    !string.IsNullOrEmpty(OpdrachtgeverPlaats) ||
                    !string.IsNullOrEmpty(OpdrachtgeverRelatie) ||
                    !string.IsNullOrEmpty(OpdrachtgeverTelefoon);
        }
    }
    public class BegrafenisDocument : ViewModelBase
    {
        private string _naamOpdrachtgever;
        private string _adresOpdrachtgever;
        private string _begraafplaats;
        private DateTime _datumUitvaart;
        private DateTime _tijdUitvaart;
        private string _soortGraf;
        private string _nrGraf;
        private string _kistType;
        private string _aulaNaam;
        private int _aantalPersonen;
        private string _naamOverledene;
        private string _voornamenOverledene;
        private DateTime _datumGeboorte;
        private string _plaatsGeboorte;
        private DateTime _datumOverlijden;
        private string _plaatsOverlijden;
        private string _bsnOverledene;
        private string _uitvaartLeider;
        private string _uitvaartLeiderEmail;
        private string _uitvaartLeiderMobiel;
        private bool _updated = false;
        private bool _initialCreation = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;
        public string NaamOpdrachtgever
        {
            get { return _naamOpdrachtgever; }
            set { _naamOpdrachtgever = value; OnPropertyChanged(nameof(NaamOpdrachtgever)); }
        }
        public string AdresOpdrachtgever
        {
            get { return _adresOpdrachtgever; }
            set { _adresOpdrachtgever = value; OnPropertyChanged(nameof(AdresOpdrachtgever)); }
        }
        public string Begraafplaats
        {
            get { return _begraafplaats; }
            set { _begraafplaats = value; OnPropertyChanged(nameof(Begraafplaats)); }
        }
        public DateTime DatumUitvaart
        {
            get { return _datumUitvaart; }
            set { _datumUitvaart = value; OnPropertyChanged(nameof(DatumUitvaart)); }
        }
        public DateTime TijdUitvaart
        {
            get { return _tijdUitvaart; }
            set { _tijdUitvaart = value; OnPropertyChanged(nameof(TijdUitvaart)); }
        }
        public string SoortGraf
        {
            get { return _soortGraf; }
            set { _soortGraf = value; OnPropertyChanged(nameof(SoortGraf)); }
        }
        public string NrGraf
        {
            get { return _nrGraf; }
            set { _nrGraf = value; OnPropertyChanged(nameof(NrGraf)); }
        }
        public string KistType
        {
            get { return _kistType; }
            set { _kistType = value; OnPropertyChanged(nameof(KistType)); }
        }
        public string AulaNaam
        {
            get { return _aulaNaam; }
            set { _aulaNaam = value; OnPropertyChanged(nameof(AulaNaam)); }
        }
        public int AantalPersonen
        {
            get { return _aantalPersonen; }
            set { _aantalPersonen = value; OnPropertyChanged(nameof(AantalPersonen)); }
        }
        public string NaamOverledene
        {
            get { return _naamOverledene; }
            set { _naamOverledene = value; OnPropertyChanged(nameof(NaamOverledene)); }
        }
        public string VoornamenOverledene
        {
            get { return _voornamenOverledene; }
            set { _voornamenOverledene = value; OnPropertyChanged(nameof(VoornamenOverledene)); }
        }
        public DateTime DatumGeboorte
        {
            get { return _datumGeboorte; }
            set { _datumGeboorte = value; OnPropertyChanged(nameof(DatumGeboorte)); }
        }
        public string PlaatsGeboorte
        {
            get { return _plaatsGeboorte; }
            set { _plaatsGeboorte = value; OnPropertyChanged(nameof(PlaatsGeboorte)); }
        }
        public DateTime DatumOverlijden
        {
            get { return _datumOverlijden; }
            set { _datumOverlijden = value; OnPropertyChanged(nameof(DatumOverlijden)); }
        }
        public string PlaatsOverlijden
        {
            get { return _plaatsOverlijden; }
            set { _plaatsOverlijden = value; OnPropertyChanged(nameof(PlaatsOverlijden)); }
        }
        public string BsnOverledene
        {
            get { return _bsnOverledene; }
            set { _bsnOverledene = value; OnPropertyChanged(nameof(BsnOverledene)); }
        }
        public string UitvaartLeider
        {
            get { return _uitvaartLeider; }
            set { _uitvaartLeider = value; OnPropertyChanged(nameof(UitvaartLeider)); }
        }
        public string UitvaartLeiderEmail
        {
            get { return _uitvaartLeiderEmail; }
            set { _uitvaartLeiderEmail = value; OnPropertyChanged(nameof(UitvaartLeiderEmail)); }
        }
        public string UitvaartLeiderMobiel
        {
            get { return _uitvaartLeiderMobiel; }
            set { _uitvaartLeiderMobiel = value; OnPropertyChanged(nameof(UitvaartLeiderMobiel)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(NaamOpdrachtgever) ||
                    !string.IsNullOrEmpty(AdresOpdrachtgever) ||
                    !string.IsNullOrEmpty(Begraafplaats) ||
                    DatumUitvaart != DateTime.MinValue ||
                    TijdUitvaart != DateTime.MinValue ||
                    !string.IsNullOrEmpty(SoortGraf) ||
                    !string.IsNullOrEmpty(NrGraf) ||
                    !string.IsNullOrEmpty(NaamOverledene) ||
                    !string.IsNullOrEmpty(VoornamenOverledene) ||
                    DatumGeboorte != DateTime.MinValue ||
                    !string.IsNullOrEmpty(PlaatsGeboorte) ||
                    DatumOverlijden != DateTime.MinValue ||
                    !string.IsNullOrEmpty(PlaatsOverlijden) ||
                    !string.IsNullOrEmpty(BsnOverledene) ||
                    !string.IsNullOrEmpty(UitvaartLeider) ||
                    !string.IsNullOrEmpty(UitvaartLeiderEmail);
        }
    }
    public class FactuurInfoCrematie : ViewModelBase
    {
        private string _factuurAdresNaam;
        private string _factuurAdresRelatie;
        private string _factuurAdresStraat;
        private string _factuurAdresPostcode;
        private string _factuurAdresGeslacht;
        private string _factuurAdresTelefoon;
        private string _factuurAdresPlaats;
        private bool _factuurAdresOverride;

        public string FactuurAdresNaam
        {
            get { return _factuurAdresNaam; }
            set
            {
                _factuurAdresNaam = value;
                OnPropertyChanged(nameof(FactuurAdresNaam));
            }
        }
        public string FactuurAdresRelatie
        {
            get { return _factuurAdresRelatie; }
            set
            {
                _factuurAdresRelatie = value;
                OnPropertyChanged(nameof(FactuurAdresRelatie));
            }
        }
        public string FactuurAdresStraat
        {
            get { return _factuurAdresStraat; }
            set
            {
                _factuurAdresStraat = value;
                OnPropertyChanged(nameof(FactuurAdresStraat));
            }
        }
        public string FactuurAdresPostcode
        {
            get { return _factuurAdresPostcode; }
            set
            {
                _factuurAdresPostcode = value;
                OnPropertyChanged(nameof(FactuurAdresPostcode));
            }
        }
        public string FactuurAdresGeslacht
        {
            get { return _factuurAdresGeslacht; }
            set
            {
                _factuurAdresGeslacht = value;
                OnPropertyChanged(nameof(FactuurAdresGeslacht));
            }
        }
        public string FactuurAdresTelefoon
        {
            get { return _factuurAdresTelefoon; }
            set
            {
                _factuurAdresTelefoon = value;
                OnPropertyChanged(nameof(FactuurAdresTelefoon));
            }
        }
        public string FactuurAdresPlaats
        {
            get { return _factuurAdresPlaats; }
            set
            {
                _factuurAdresPlaats = value;
                OnPropertyChanged(nameof(FactuurAdresPlaats));
            }
        }
        public bool FactuurAdresOverride
        {
            get { return _factuurAdresOverride; }
            set
            {
                _factuurAdresOverride = value;
                OnPropertyChanged(nameof(FactuurAdresOverride));
            }
        }
    }
    public class CrematieDocument : ViewModelBase
    {
        private string _aulaNaam;
        private int _aulaPersonen;
        private DateTime _aanvangstTijd;
        private DateTime _startAula;
        private DateTime _startKoffie;
        private string _crematieLocatie;
        private string _crematieVoor;
        private DateTime _crematieDatum;
        private string _crematieDossiernummer;

        private string _overledeneNaam;
        private string _overledeneVoornaam;
        private string _overledeneBurgStaat;
        private DateTime _overledeneGebDatum;
        private string _overledeneGebPlaats;
        private string _overledeneStraat;
        private string _overledenePostcode;
        private string _overledeneLevensovertuiging;
        private string _overledeneGeslacht;
        private string _overledeneLeeftijd;
        private string _overledeneWoonplaats;
        private DateTime _overledeneDatum;
        private string _overledenePlaats;
        private string _overledeneRadio;

        private string _opdrachtgeverNaam;
        private string _opdrachtgeverGeslacht;
        private DateTime _opdrachtgeverGebDatum;
        private string _opdrachtgeverStraat;
        private string _opdrachtgeverPostcode;
        private string _opdrachtgeverRelatie;
        private string _opdrachtgeverVoornamen;
        private string _opdrachtgeverTelefoon;
        private string _opdrachtgeverPlaats;
        private string _opdrachtgeverEmail;

        private string _uitvaartverzorger;
        private string _asbestemming;
        private string _consumpties;
        private Guid _herkomst;
        private bool _updated = false;
        private bool _initialCreation = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;

        public string AulaNaam
        {
            get { return _aulaNaam; }
            set { _aulaNaam = value; OnPropertyChanged(nameof(AulaNaam)); }
        }
        public int AulaPersonen
        {
            get { return _aulaPersonen; }
            set { _aulaPersonen = value; OnPropertyChanged(nameof(AulaPersonen)); }
        }
        public DateTime Aanvangstrijd
        {
            get { return _aanvangstTijd; }
            set { _aanvangstTijd = value; OnPropertyChanged(nameof(Aanvangstrijd)); }
        }
        public DateTime StartAula
        {
            get { return _startAula; }
            set { _startAula = value; OnPropertyChanged(nameof(StartAula)); }
        }
        public DateTime StartKoffie
        {
            get { return _startKoffie; }
            set { _startKoffie = value; OnPropertyChanged(nameof(StartKoffie)); }
        }
        public string CrematieLocatie
        {
            get { return _crematieLocatie; }
            set { _crematieLocatie = value; OnPropertyChanged(nameof(CrematieLocatie)); }
        }
        public string CrematieVoor
        {
            get { return _crematieVoor; }
            set { _crematieVoor = value; OnPropertyChanged(nameof(CrematieVoor)); }
        }
        public DateTime CrematieDatum
        {
            get { return _crematieDatum; }
            set { _crematieDatum = value; OnPropertyChanged(nameof(CrematieDatum)); }
        }
        public string CrematieDossiernummer
        {
            get { return _crematieDossiernummer; }
            set { _crematieDossiernummer = value; OnPropertyChanged(nameof(CrematieDossiernummer)); }
        }
        public string OverledeneNaam
        {
            get { return _overledeneNaam; }
            set { _overledeneNaam = value; OnPropertyChanged(nameof(OverledeneNaam)); }
        }
        public string OverledeneVoornaam
        {
            get { return _overledeneVoornaam; }
            set { _overledeneVoornaam = value; OnPropertyChanged(nameof(OverledeneVoornaam)); }
        }
        public string OverledeneBurgStaat
        {
            get { return _overledeneBurgStaat; }
            set { _overledeneBurgStaat = value; OnPropertyChanged(nameof(OverledeneBurgStaat)); }
        }
        public DateTime OverledeneGebDatum
        {
            get { return _overledeneGebDatum; }
            set { _overledeneGebDatum = value; OnPropertyChanged(nameof(OverledeneGebDatum)); }
        }
        public string OverledeneGebPlaats
        {
            get { return _overledeneGebPlaats; }
            set { _overledeneGebPlaats = value; OnPropertyChanged(nameof(OverledeneGebPlaats)); }
        }
        public string OverledeneStraat
        {
            get { return _overledeneStraat; }
            set { _overledeneStraat = value; OnPropertyChanged(nameof(OverledeneStraat)); }
        }
        public string OverledenePostcode
        {
            get { return _overledenePostcode; }
            set { _overledenePostcode = value; OnPropertyChanged(nameof(OverledenePostcode)); }
        }
        public string OverledeneLevensovertuiging
        {
            get { return _overledeneLevensovertuiging; }
            set { _overledeneLevensovertuiging = value; OnPropertyChanged(nameof(OverledeneLevensovertuiging)); }
        }
        public string OverledeneGeslacht
        {
            get { return _overledeneGeslacht; }
            set { _overledeneGeslacht = value; OnPropertyChanged(nameof(OverledeneGeslacht)); }
        }
        public string OverledeneLeeftijd
        {
            get { return _overledeneLeeftijd; }
            set { _overledeneLeeftijd = value; OnPropertyChanged(nameof(OverledeneLeeftijd)); }
        }
        public string OverledeneWoonplaats
        {
            get { return _overledeneWoonplaats; }
            set { _overledeneWoonplaats = value; OnPropertyChanged(nameof(OverledeneWoonplaats)); }
        }
        public DateTime OverledeneDatum
        {
            get { return _overledeneDatum; }
            set { _overledeneDatum = value; OnPropertyChanged(nameof(OverledeneDatum)); }
        }
        public string OverledenePlaats
        {
            get { return _overledenePlaats; }
            set { _overledenePlaats = value; OnPropertyChanged(nameof(OverledenePlaats)); }
        }
        public string OverledeneRadio
        {
            get { return _overledeneRadio; }
            set { _overledeneRadio = value; OnPropertyChanged(nameof(OverledeneRadio)); }
        }
        public string OpdrachtgeverNaam
        {
            get { return _opdrachtgeverNaam; }
            set { _opdrachtgeverNaam = value; OnPropertyChanged(nameof(OpdrachtgeverNaam)); }
        }
        public string OpdrachtgeverGeslacht
        {
            get { return _opdrachtgeverGeslacht; }
            set { _opdrachtgeverGeslacht = value; OnPropertyChanged(nameof(OpdrachtgeverGeslacht)); }
        }
        public DateTime OpdrachtgeverGebDatum
        {
            get { return _opdrachtgeverGebDatum; }
            set { _opdrachtgeverGebDatum = value; OnPropertyChanged(nameof(OpdrachtgeverGebDatum)); }
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
        public string OpdrachtgeverRelatie
        {
            get { return _opdrachtgeverRelatie; }
            set { _opdrachtgeverRelatie = value; OnPropertyChanged(nameof(OpdrachtgeverRelatie)); }
        }
        public string OpdrachtgeverVoornamen
        {
            get { return _opdrachtgeverVoornamen; }
            set { _opdrachtgeverVoornamen = value; OnPropertyChanged(nameof(OpdrachtgeverVoornamen)); }
        }
        public string OpdrachtgeverTelefoon
        {
            get { return _opdrachtgeverTelefoon; }
            set { _opdrachtgeverTelefoon = value; OnPropertyChanged(nameof(OpdrachtgeverTelefoon)); }
        }
        public string OpdrachtgeverPlaats
        {
            get { return _opdrachtgeverPlaats; }
            set { _opdrachtgeverPlaats = value; OnPropertyChanged(nameof(OpdrachtgeverPlaats)); }
        }
        public string OpdrachtgeverEmail
        {
            get { return _opdrachtgeverEmail; }
            set { _opdrachtgeverEmail = value; OnPropertyChanged(nameof(OpdrachtgeverEmail)); }
        }
        public string Uitvaartverzorger
        {
            get { return _uitvaartverzorger; }
            set { _uitvaartverzorger = value; OnPropertyChanged(nameof(Uitvaartverzorger)); }
        }
        public string Asbestemming
        {
            get { return _asbestemming; }
            set { _asbestemming = value; OnPropertyChanged(nameof(Asbestemming)); }
        }
        public string Consumpties
        {
            get { return _consumpties; }
            set { _consumpties = value; OnPropertyChanged(nameof(Consumpties)); }
        }
        public Guid Herkomst
        {
            get { return _herkomst; }
            set { _herkomst = value; OnPropertyChanged(nameof(Herkomst)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(CrematieLocatie) ||
                    !string.IsNullOrEmpty(CrematieVoor) ||
                    CrematieDatum != DateTime.MinValue ||
                    !string.IsNullOrEmpty(CrematieDossiernummer) ||
                    !string.IsNullOrEmpty(OverledeneNaam) ||
                    !string.IsNullOrEmpty(OverledeneVoornaam) ||
                    !string.IsNullOrEmpty(OverledeneBurgStaat) ||
                    OverledeneGebDatum != DateTime.MinValue ||
                    !string.IsNullOrEmpty(OverledeneGebPlaats) ||
                    !string.IsNullOrEmpty(OverledeneStraat) ||
                    !string.IsNullOrEmpty(OverledenePostcode) ||
                    !string.IsNullOrEmpty(OverledeneLevensovertuiging) ||
                    !string.IsNullOrEmpty(OverledeneGeslacht) ||
                    !string.IsNullOrEmpty(OverledeneLeeftijd) ||
                    !string.IsNullOrEmpty(OverledeneWoonplaats) ||
                    OverledeneDatum != DateTime.MinValue ||
                    !string.IsNullOrEmpty(OverledenePlaats) ||
                    !string.IsNullOrEmpty(OverledeneRadio) ||
                    !string.IsNullOrEmpty(OpdrachtgeverNaam) ||
                    !string.IsNullOrEmpty(OpdrachtgeverGeslacht) ||
                    OpdrachtgeverGebDatum != DateTime.MinValue ||
                    !string.IsNullOrEmpty(OpdrachtgeverStraat) ||
                    !string.IsNullOrEmpty(OpdrachtgeverPostcode) ||
                    !string.IsNullOrEmpty(OpdrachtgeverRelatie) ||
                    !string.IsNullOrEmpty(OpdrachtgeverVoornamen) ||
                    !string.IsNullOrEmpty(OpdrachtgeverTelefoon) ||
                    !string.IsNullOrEmpty(OpdrachtgeverPlaats) ||
                    !string.IsNullOrEmpty(OpdrachtgeverEmail) ||
                    !string.IsNullOrEmpty(Uitvaartverzorger) ||
                    !string.IsNullOrEmpty(Asbestemming) ||
                    !string.IsNullOrEmpty(Consumpties);
        }
    }
    public class BezittingenDocument : ViewModelBase
    {
        private string _dossiernummer;
        private string _overledeneNaam;
        private string _overledeneVoornaam;
        private DateTime _overledeneGeborenOp;
        private DateTime _overledeneOverledenOp;
        private string _overledeneLocatieOpbaring;
        private string _overledenePlaatsOverlijden;
        private string _overledeneBezittingen;
        private string _overledeneRetour;
        private string _overledeneRelatie;
        private string _opdrachtgeverNaamVoorletters;
        private string _opdrachtgeverAdres;
        private bool _updated = false;
        private bool _initialCreation;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;

        public string DossierNummer
        {
            get { return _dossiernummer; }
            set { _dossiernummer = value; OnPropertyChanged(nameof(DossierNummer)); }
        }
        public string OverledeneNaam
        {
            get { return _overledeneNaam; }
            set { _overledeneNaam = value; OnPropertyChanged(nameof(OverledeneNaam)); }
        }
        public string OverledeneVoornaam
        {
            get { return _overledeneVoornaam; }
            set { _overledeneVoornaam = value; OnPropertyChanged(nameof(OverledeneVoornaam)); }
        }
        public DateTime OverledeneGeborenOp
        {
            get { return _overledeneGeborenOp; }
            set { _overledeneGeborenOp = value; OnPropertyChanged(nameof(OverledeneGeborenOp)); }
        }
        public DateTime OverledeneOverledenOp
        {
            get { return _overledeneOverledenOp; }
            set
            {
                _overledeneOverledenOp = value; OnPropertyChanged(nameof(OverledeneOverledenOp));
            }
        }
        public string OverledeneLocatieOpbaring
        {
            get { return _overledeneLocatieOpbaring; }
            set
            {
                _overledeneLocatieOpbaring = value; OnPropertyChanged(nameof(OverledeneLocatieOpbaring));
            }
        }
        public string OverledenePlaatsOverlijden
        {
            get { return _overledenePlaatsOverlijden; }
            set
            {
                _overledenePlaatsOverlijden = value; OnPropertyChanged(nameof(OverledenePlaatsOverlijden));
            }
        }
        public string OverledeneBezittingen
        {
            get { return _overledeneBezittingen; }
            set
            {
                _overledeneBezittingen = value; OnPropertyChanged(nameof(OverledeneBezittingen));
            }
        }
        public string OverledeneRetour
        {
            get { return _overledeneRetour; }
            set
            {
                _overledeneRetour = value; OnPropertyChanged(nameof(OverledeneRetour));
            }
        }
        public string OverledeneRelatie
        {
            get { return _overledeneRelatie; }
            set { _overledeneRelatie = value; OnPropertyChanged(nameof(OverledeneRelatie)); }
        }
        public string OpdrachtgeverNaamVoorletters
        {
            get { return _opdrachtgeverNaamVoorletters; }
            set { _opdrachtgeverNaamVoorletters = value; OnPropertyChanged(nameof(OpdrachtgeverNaamVoorletters)); }
        }
        public string OpdrachtgeverAdres
        {
            get { return _opdrachtgeverAdres; }
            set { _opdrachtgeverAdres = value; OnPropertyChanged(nameof(OpdrachtgeverAdres)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(DossierNummer) ||
                    !string.IsNullOrEmpty(OverledeneNaam) ||
                    !string.IsNullOrEmpty(OverledeneVoornaam) ||
                    OverledeneGeborenOp != DateTime.MinValue ||
                    OverledeneOverledenOp != DateTime.MinValue ||
                    !string.IsNullOrEmpty(OverledeneLocatieOpbaring) ||
                    !string.IsNullOrEmpty(OverledenePlaatsOverlijden) ||
                    !string.IsNullOrEmpty(OverledeneBezittingen) ||
                    !string.IsNullOrEmpty(OverledeneRetour) ||
                    !string.IsNullOrEmpty(OverledeneRelatie);
        }
    }
    public class OverdrachtDocument : ViewModelBase
    {
        private string _overdrachtType;
        private string _overledeAanhef;
        private string _overledeneVoornaam;
        private string _overledeneAchternaam;
        private string _uitvaartNummer;
        private string _overdragendePartij;
        private string _destinationFile;
        private bool _updated = false;
        private Guid _uitvaartId;
        private Guid _documentId;

        public string OverdrachtType
        {
            get { return _overdrachtType; }
            set { _overdrachtType = value; OnPropertyChanged(nameof(OverdrachtType)); }
        }
        public string OverledeAanhef
        {
            get { return _overledeAanhef; }
            set { _overledeAanhef = value; OnPropertyChanged(nameof(OverledeAanhef)); }
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
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public string OverdragendePartij
        {
            get { return _overdragendePartij; }
            set { _overdragendePartij = value; OnPropertyChanged(nameof(OverdragendePartij)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(OverdrachtType) ||
                   !string.IsNullOrEmpty(OverledeAanhef) ||
                   !string.IsNullOrEmpty(OverledeneVoornaam) ||
                   !string.IsNullOrEmpty(OverledeneAchternaam) ||
                   !string.IsNullOrEmpty(UitvaartNummer) ||
                   !string.IsNullOrEmpty(OverdragendePartij);
        }
    }
    public class ChecklistOpbarenDocument : ViewModelBase
    {
        private Guid _werknemerId;
        private string _werknemerName;

        public Guid WerknemerId
        {
            get { return _werknemerId; }
            set { _werknemerId = value; OnPropertyChanged(nameof(WerknemerId)); }
        }
        public string WerknemerName
        {
            get { return _werknemerName; }
            set { _werknemerName = value; OnPropertyChanged(nameof(WerknemerName)); }
        }
    }
    public class ChecklistDocument : ViewModelBase
    {
        private string _documentType;
        private string _achternaam;
        private string _datumUitvaart;
        private string _overledenDatum;
        private string _uitvaartNummer;
        private string _herkomst;
        private string _uitvaartType;
        private string _destinationFile;
        private string _uitvaartLeider;
        private string _volledigeNaam;
        private string _opbarenInfo;
        private bool _updated = false;
        private Guid _uitvaartId;
        private Guid _documentId;

        public string DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; OnPropertyChanged(nameof(DocumentType)); }
        }
        public string Achternaam
        {
            get { return _achternaam; }
            set { _achternaam = value; OnPropertyChanged(nameof(Achternaam)); }
        }
        public string DatumUitvaart
        {
            get { return _datumUitvaart; }
            set { _datumUitvaart = value; OnPropertyChanged(nameof(DatumUitvaart)); }
        }
        public string OverledenDatum
        {
            get { return _overledenDatum; }
            set { _overledenDatum = value; OnPropertyChanged(nameof(OverledenDatum)); }
        }
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public string Herkomst
        {
            get { return _herkomst; }
            set { _herkomst = value; OnPropertyChanged(nameof(Herkomst)); }
        }
        public string UitvaartType
        {
            get { return _uitvaartType; }
            set { _uitvaartType = value; OnPropertyChanged(nameof(UitvaartType)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public string UitvartLeider
        {
            get { return _uitvaartLeider; }
            set
            {
                _uitvaartLeider = value; OnPropertyChanged(nameof(UitvartLeider));
            }
        }
        public string VolledigeNaam
        {
            get { return _volledigeNaam; }
            set
            {
                _volledigeNaam = value; OnPropertyChanged(nameof(VolledigeNaam));
            }
        }
        public string OpbarenInfo
        {
            get { return _opbarenInfo; }
            set
            {
                _opbarenInfo = value; OnPropertyChanged(nameof(OpbarenInfo));
            }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(DocumentType) ||
                    !string.IsNullOrEmpty(Achternaam) ||
                    !string.IsNullOrEmpty(DatumUitvaart) ||
                    !string.IsNullOrEmpty(OverledenDatum) ||
                    !string.IsNullOrEmpty(UitvaartNummer) ||
                    !string.IsNullOrEmpty(Herkomst) ||
                    !string.IsNullOrEmpty(UitvaartType) ||
                    !string.IsNullOrEmpty(UitvartLeider);
        }
    }
    public class DienstDocument : ViewModelBase
    {
        private string _aanvraagDienstTe;
        private DateTime _datumUitvaart;
        private string _naamUitvaart;
        private string _locatieDienst;
        private DateTime _datumDienst;
        private string _ontvangstekamer;
        private DateTime _aanvang;
        private string _aantalPersonen;
        private string _muziekAfgespeeld;
        private string _beeldmateriaal;
        private string _opname;
        private string _afscheidVoorDienst;
        private string _baarOntrekken;
        private string _kistDalen;
        private string _familieLaatste;
        private string _orgel;
        private string _koor;
        private string _attributen;
        private string _liturgische;
        private string _opdrachtgeverNaam;
        private string _opdrachtgeverAdres;
        private string _opdrachtgeverPostcode;
        private string _opdrachtgeverPlaats;
        private string _opdrachtgeverTelefoon;
        private string _opmerkingen;
        private bool _updated = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;

        public string AanvraagDienstTe
        {
            get { return _aanvraagDienstTe; }
            set { _aanvraagDienstTe = value; OnPropertyChanged(nameof(AanvraagDienstTe)); }
        }
        public DateTime DatumUitvaart
        {
            get { return _datumUitvaart; }
            set { _datumUitvaart = value; OnPropertyChanged(nameof(DatumUitvaart)); }
        }
        public string NaamUitvaart
        {
            get { return _naamUitvaart; }
            set { _naamUitvaart = value; OnPropertyChanged(nameof(NaamUitvaart)); }
        }
        public string LocatieDienst
        {
            get { return _locatieDienst; }
            set { _locatieDienst = value; OnPropertyChanged(nameof(LocatieDienst)); }
        }
        public DateTime DatumDienst
        {
            get { return _datumDienst; }
            set { _datumDienst = value; OnPropertyChanged(nameof(DatumDienst)); }
        }
        public string OntvangstKamer
        {
            get { return _ontvangstekamer; }
            set { _ontvangstekamer = value; OnPropertyChanged(nameof(OntvangstKamer)); }
        }
        public DateTime Aanvang
        {
            get { return _aanvang; }
            set { _aanvang = value; OnPropertyChanged(nameof(Aanvang)); }
        }
        public string AantalPersonen
        {
            get { return _aantalPersonen; }
            set { _aantalPersonen = value; OnPropertyChanged(nameof(AantalPersonen)); }
        }
        public string MuziekAfgespeeld
        {
            get { return _muziekAfgespeeld; }
            set { _muziekAfgespeeld = value; OnPropertyChanged(nameof(MuziekAfgespeeld)); }
        }
        public string Beeldmateriaal
        {
            get { return _beeldmateriaal; }
            set { _beeldmateriaal = value; OnPropertyChanged(nameof(Beeldmateriaal)); }
        }
        public string Opname
        {
            get { return _opname; }
            set { _opname = value; OnPropertyChanged(nameof(Opname)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public string AfscheidVoorDienst
        {
            get { return _afscheidVoorDienst; }
            set { _afscheidVoorDienst = value; OnPropertyChanged(nameof(AfscheidVoorDienst)); }
        }
        public string BaarOntrekken
        {
            get { return _baarOntrekken; }
            set { _baarOntrekken = value; OnPropertyChanged(nameof(BaarOntrekken)); }
        }
        public string KistDalen
        {
            get { return _kistDalen; }
            set { _kistDalen = value; OnPropertyChanged(nameof(KistDalen)); }
        }
        public string FamilieLaatste
        {
            get { return _familieLaatste; }
            set { _familieLaatste = value; OnPropertyChanged(nameof(FamilieLaatste)); }
        }
        public string Orgel
        {
            get { return _orgel; }
            set { _orgel = value; OnPropertyChanged(nameof(Orgel)); }
        }
        public string Koor
        {
            get { return _koor; }
            set { _koor = value; OnPropertyChanged(nameof(Koor)); }
        }
        public string Attributen
        {
            get { return _attributen; }
            set { _attributen = value; OnPropertyChanged(nameof(Attributen)); }
        }
        public string Liturgische
        {
            get { return _liturgische; }
            set { _liturgische = value; OnPropertyChanged(nameof(Liturgische)); }
        }
        public string OpdrachtgeverNaam
        {
            get { return _opdrachtgeverNaam; }
            set { _opdrachtgeverNaam = value; OnPropertyChanged(nameof(OpdrachtgeverNaam)); }
        }
        public string OpdrachtgeverAdres
        {
            get { return _opdrachtgeverAdres; }
            set { _opdrachtgeverAdres = value; OnPropertyChanged(nameof(OpdrachtgeverAdres)); }
        }
        public string OpdrachtgeverPostcode
        {
            get { return _opdrachtgeverPostcode; }
            set { _opdrachtgeverPostcode = value; OnPropertyChanged(nameof(OpdrachtgeverPostcode)); }
        }
        public string OpdrachtgeverPlaats
        {
            get { return _opdrachtgeverPlaats; }
            set { _opdrachtgeverPlaats = value; OnPropertyChanged(nameof(OpdrachtgeverPlaats)); }
        }
        public string OpdrachtgeverTelefoon
        {
            get { return _opdrachtgeverTelefoon; }
            set { _opdrachtgeverTelefoon = value; OnPropertyChanged(nameof(OpdrachtgeverTelefoon)); }
        }
        public string Opmerkingen
        {
            get { return _opmerkingen; }
            set { _opmerkingen = value; OnPropertyChanged(nameof(Opmerkingen)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(AanvraagDienstTe) ||
                    DatumUitvaart != DateTime.MinValue ||
                    !string.IsNullOrEmpty(NaamUitvaart) ||
                    !string.IsNullOrEmpty(LocatieDienst) ||
                    DatumDienst != DateTime.MinValue ||
                    !string.IsNullOrEmpty(OntvangstKamer) ||
                    Aanvang != DateTime.MinValue ||
                    !string.IsNullOrEmpty(AantalPersonen) ||
                    !string.IsNullOrEmpty(MuziekAfgespeeld) ||
                    !string.IsNullOrEmpty(Beeldmateriaal) ||
                    !string.IsNullOrEmpty(Opname) ||
                    !string.IsNullOrEmpty(AfscheidVoorDienst) ||
                    !string.IsNullOrEmpty(BaarOntrekken) ||
                    !string.IsNullOrEmpty(KistDalen) ||
                    !string.IsNullOrEmpty(FamilieLaatste) ||
                    !string.IsNullOrEmpty(Orgel) ||
                    !string.IsNullOrEmpty(Koor) ||
                    !string.IsNullOrEmpty(Attributen) ||
                    !string.IsNullOrEmpty(Liturgische) ||
                    !string.IsNullOrEmpty(OpdrachtgeverNaam) ||
                    !string.IsNullOrEmpty(OpdrachtgeverAdres) ||
                    !string.IsNullOrEmpty(OpdrachtgeverTelefoon) ||
                    !string.IsNullOrEmpty(Opmerkingen);
        }
    }
    public class DocumentDocument : ViewModelBase
    {
        private string _documentType;
        private string _uitvaartverzorger;
        private string _uitvaartverzorgerEmail;
        private string _geslachtnaamOverledene;
        private string _voornaamOverledene;
        private DateTime _geboortedatumOverledene;
        private string _geboorteplaatsOverledene;
        private string _woonplaatsOverledene;
        private DateTime _datumOverlijden;
        private string _gemeenteOverlijden;
        private string _plaatsOverlijden;
        private string _locatieUitvaart;
        private DateTime _datumUitvaart;
        private string _ondergetekendeUitvaart;
        private bool _updated = false;
        private bool _initialCreation = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;
        private string _uitvaartNummer;
        public string DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; OnPropertyChanged(nameof(DocumentType)); }
        }
        public string UitvaartVerzorger
        {
            get { return _uitvaartverzorger; }
            set { _uitvaartverzorger = value; OnPropertyChanged(nameof(UitvaartVerzorger)); }
        }
        public string UitvaartVerzorgerEmail
        {
            get { return _uitvaartverzorgerEmail; }
            set { _uitvaartverzorgerEmail = value; OnPropertyChanged(nameof(UitvaartVerzorgerEmail)); }
        }
        public string GeslachtsnaamOverledene
        {
            get { return _geslachtnaamOverledene; }
            set { _geslachtnaamOverledene = value; OnPropertyChanged(nameof(GeslachtsnaamOverledene)); }
        }
        public string VoornaamOverledene
        {
            get { return _voornaamOverledene; }
            set { _voornaamOverledene = value; OnPropertyChanged(nameof(VoornaamOverledene)); }
        }
        public DateTime GeboortedatumOverledene
        {
            get { return _geboortedatumOverledene; }
            set { _geboortedatumOverledene = value; OnPropertyChanged(nameof(_geboortedatumOverledene)); }
        }
        public string GeboorteplaatsOverledene
        {
            get { return _geboorteplaatsOverledene; }
            set { _geboorteplaatsOverledene = value; OnPropertyChanged(nameof(GeboorteplaatsOverledene)); }
        }
        public string WoonplaatsOverledene
        {
            get { return _woonplaatsOverledene; }
            set { _woonplaatsOverledene = value; OnPropertyChanged(nameof(WoonplaatsOverledene)); }
        }
        public DateTime DatumOverlijden
        {
            get { return _datumOverlijden; }
            set { _datumOverlijden = value; OnPropertyChanged(nameof(DatumOverlijden)); }
        }
        public string GemeenteOverlijden
        {
            get { return _gemeenteOverlijden; }
            set { _gemeenteOverlijden = value; OnPropertyChanged(nameof(GemeenteOverlijden)); }
        }
        public string PlaatsOverlijden
        {
            get { return _plaatsOverlijden; }
            set { _plaatsOverlijden = value; OnPropertyChanged(nameof(PlaatsOverlijden)); }
        }
        public string LocatieUitvaart
        {
            get { return _locatieUitvaart; }
            set { _locatieUitvaart = value; OnPropertyChanged(nameof(LocatieUitvaart)); }
        }
        public DateTime DatumUitvaart
        {
            get { return _datumUitvaart; }
            set { _datumUitvaart = value; OnPropertyChanged(nameof(DatumUitvaart)); }
        }
        public string OndergetekendeUitvaart
        {
            get { return _ondergetekendeUitvaart; }
            set { _ondergetekendeUitvaart = value; OnPropertyChanged(nameof(OndergetekendeUitvaart)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public string UitvaartNummer
        {
            get { return _uitvaartNummer; }
            set { _uitvaartNummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(DocumentType) ||
                    !string.IsNullOrEmpty(UitvaartVerzorger) ||
                    !string.IsNullOrEmpty(UitvaartVerzorgerEmail) ||
                    !string.IsNullOrEmpty(GeslachtsnaamOverledene) ||
                    !string.IsNullOrEmpty(VoornaamOverledene) ||
                    GeboortedatumOverledene != DateTime.MinValue ||
                    !string.IsNullOrEmpty(GeboorteplaatsOverledene) ||
                    !string.IsNullOrEmpty(WoonplaatsOverledene) ||
                    DatumOverlijden != DateTime.MinValue ||
                    !string.IsNullOrEmpty(GemeenteOverlijden) ||
                    !string.IsNullOrEmpty(PlaatsOverlijden) ||
                    !string.IsNullOrEmpty(LocatieUitvaart) ||
                    DatumUitvaart != DateTime.MinValue ||
                    !string.IsNullOrEmpty(OndergetekendeUitvaart);
        }
    }
    public class KoffieKamerDocument : ViewModelBase
    {
        private DateTime _datumUitvaart;
        private string _naam;
        private string _dienstLocatie;
        private DateTime _dienstDatum;
        private DateTime _dienstTijd;
        private string _opdrachtgever;
        private string _opdrachtgeverAdres;
        private string _opdrachtgeverTelefoon;
        private bool _updated = false;
        private bool _initialCreation = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;

        public DateTime DatumUitvaart
        {
            get { return _datumUitvaart; }
            set { _datumUitvaart = value; OnPropertyChanged(nameof(DatumUitvaart)); }
        }
        public string Naam
        {
            get { return _naam; }
            set { _naam = value; OnPropertyChanged(nameof(Naam)); }
        }
        public string DienstLocatie
        {
            get { return _dienstLocatie; }
            set { _dienstLocatie = value; OnPropertyChanged(nameof(DienstLocatie)); }
        }
        public DateTime DienstDatum
        {
            get { return _dienstDatum; }
            set { _dienstDatum = value; OnPropertyChanged(nameof(DienstDatum)); }
        }
        public DateTime DienstTijd
        {
            get { return _dienstTijd; }
            set { _dienstTijd = value; OnPropertyChanged(nameof(DienstTijd)); }
        }
        public string Opdrachtgever
        {
            get { return _opdrachtgever; }
            set { _opdrachtgever = value; OnPropertyChanged(nameof(Opdrachtgever)); }
        }
        public string OpdrachtgeverAdres
        {
            get { return _opdrachtgeverAdres; }
            set { _opdrachtgeverAdres = value; OnPropertyChanged(nameof(OpdrachtgeverAdres)); }
        }
        public string OpdrachtgeverTelefoon
        {
            get { return _opdrachtgeverTelefoon; }
            set { _opdrachtgeverTelefoon = value; OnPropertyChanged(nameof(OpdrachtgeverTelefoon)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public bool HasData()
        {
            return DatumUitvaart != DateTime.MinValue ||
                    !string.IsNullOrEmpty(Naam) ||
                    !string.IsNullOrEmpty(DienstLocatie) ||
                    DienstDatum != DateTime.MinValue ||
                    DienstTijd != DateTime.MinValue ||
                    !string.IsNullOrEmpty(Opdrachtgever) ||
                    !string.IsNullOrEmpty(OpdrachtgeverAdres) ||
                    !string.IsNullOrEmpty(OpdrachtgeverTelefoon);
        }
    }
    public class BloemenDocument : ViewModelBase
    {
        private string _leverancierNaam;
        private string _uitvaartleider;
        private string _emailUitvaartleider;
        private string _naamOverledene;
        private string _bloemstuk;
        private bool _lint;
        private bool _kaart;
        private string _bezorgadres;
        private string _telefoonnummer;
        private DateTime _datumBezorgen;
        private bool _updated = false;
        private bool _initialCreation = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;
        private string _lintJson;

        public string LeverancierNaam
        {
            get { return _leverancierNaam; }
            set { _leverancierNaam = value; OnPropertyChanged(nameof(LeverancierNaam)); }
        }
        public string Uitvaartleider
        {
            get { return _uitvaartleider; }
            set { _uitvaartleider = value; OnPropertyChanged(nameof(Uitvaartleider)); }
        }
        public string EmailUitvaartleider
        {
            get { return _emailUitvaartleider; }
            set { _emailUitvaartleider = value; OnPropertyChanged(nameof(EmailUitvaartleider)); }
        }
        public string NaamOverledene
        {
            get { return _naamOverledene; }
            set { _naamOverledene = value; OnPropertyChanged(nameof(NaamOverledene)); }
        }
        public string Bloemstuk
        {
            get { return _bloemstuk; }
            set { _bloemstuk = value; OnPropertyChanged(nameof(Bloemstuk)); }
        }
        public bool Lint
        {
            get { return _lint; }
            set { _lint = value; OnPropertyChanged(nameof(Lint)); }
        }
        public bool Kaart
        {
            get { return _kaart; }
            set { _kaart = value; OnPropertyChanged(nameof(Kaart)); }
        }
        public string Bezorgadres
        {
            get { return _bezorgadres; }
            set
            {
                _bezorgadres = value; OnPropertyChanged(nameof(Bezorgadres));
            }
        }
        public string Telefoonnummer
        {
            get { return _telefoonnummer; }
            set
            {
                _telefoonnummer = value; OnPropertyChanged(nameof(Telefoonnummer));
            }
        }
        public DateTime DatumBezorgen
        {
            get { return _datumBezorgen; }
            set
            {
                _datumBezorgen = value; OnPropertyChanged(nameof(DatumBezorgen));
            }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public string LintJson
        {
            get { return _lintJson; }
            set { _lintJson = value; OnPropertyChanged(nameof(LintJson)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(LeverancierNaam) ||
                    !string.IsNullOrEmpty(Uitvaartleider) ||
                    !string.IsNullOrEmpty(EmailUitvaartleider) ||
                    !string.IsNullOrEmpty(NaamOverledene) ||
                    !string.IsNullOrEmpty(Bloemstuk) ||
                    Lint ||
                    Kaart ||
                    !string.IsNullOrEmpty(Bezorgadres) ||
                    !string.IsNullOrEmpty(Telefoonnummer) ||
                    DatumBezorgen != DateTime.MinValue;
        }
    }
    public class AangifteDocument : ViewModelBase
    {
        private string _overledeneAanhef;
        private string _overledeneAchternaam;
        private string _overledeneVoornaam;
        private string _overledeneGeboorteplaats;
        private DateTime _overledeneGeboortedatum;
        private string _overledeneAdres;
        private string _overledenePostcode;
        private string _overledeneWoonplaats;
        private string _overledeneBSN;
        private DateTime _datumOverlijden;
        private DateTime _tijdOverlijden;
        private string _adresOverlijden;
        private string _naamWederhelft;
        private string _voornaamWederhelft;
        private string _eersteOuder;
        private string _tweedeOuder;
        private string _gehuwdGeweestMet;
        private string _weduwenaarVan;
        private string _aantalKinderen;
        private string _aantalKinderenMinderjarig;
        private string _aantalKinderenWaarvanOverleden;
        private string _aangeverNaam;
        private string _aangeverPlaats;
        private string _erfgenaamVolledigeNaam;
        private string _erfgenaamStraat;
        private string _erfgenaamPostcode;
        private string _erfgenaamWoonplaats;
        private DateTime _datumUitvaart;
        private DateTime _tijdUitvaart;
        private string _locatieUitvaart;
        private string _typeUitvaart;
        private string _schouwarts;
        private string _ubs;
        private bool _updated = false;
        private bool _initialCreation = false;
        private string _destinationFile;
        private Guid _uitvaartId;
        private Guid _documentId;
        private string _uitvaartnummer;
        private string _burgelijkestaat;


        public string OverledeneAanhef
        {
            get { return _overledeneAanhef; }
            set { _overledeneAanhef = value; OnPropertyChanged(nameof(OverledeneAanhef)); }
        }
        public string OverledeneAchternaam
        {
            get { return _overledeneAchternaam; }
            set { _overledeneAchternaam = value; OnPropertyChanged(nameof(OverledeneAchternaam)); }
        }
        public string OverledeneVoornaam
        {
            get { return _overledeneVoornaam; }
            set { _overledeneVoornaam = value; OnPropertyChanged(nameof(OverledeneVoornaam)); }
        }
        public string OverledeneGeboorteplaats
        {
            get { return _overledeneGeboorteplaats; }
            set { _overledeneGeboorteplaats = value; OnPropertyChanged(nameof(OverledeneGeboorteplaats)); }
        }
        public DateTime OverledeneGeboortedatum
        {
            get { return _overledeneGeboortedatum; }
            set { _overledeneGeboortedatum = value; OnPropertyChanged(nameof(OverledeneGeboortedatum)); }
        }
        public string OverledeneAdres
        {
            get { return _overledeneAdres; }
            set { _overledeneAdres = value; OnPropertyChanged(nameof(OverledeneAdres)); }
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
        public string OverledeneBSN
        {
            get { return _overledeneBSN; }
            set { _overledeneBSN = value; OnPropertyChanged(nameof(OverledeneBSN)); }
        }
        public DateTime DatumOverlijden
        {
            get { return _datumOverlijden; }
            set { _datumOverlijden = value; OnPropertyChanged(nameof(DatumOverlijden)); }
        }
        public DateTime TijdOverlijden
        {
            get { return _tijdOverlijden; }
            set { _tijdOverlijden = value; OnPropertyChanged(nameof(TijdOverlijden)); }
        }
        public string AdresOverlijden
        {
            get { return _adresOverlijden; }
            set { _adresOverlijden = value; OnPropertyChanged(nameof(AdresOverlijden)); }
        }
        public string NaamWederhelft
        {
            get { return _naamWederhelft; }
            set { _naamWederhelft = value; OnPropertyChanged(nameof(NaamWederhelft)); }
        }
        public string VoornamenWederhelft
        {
            get { return _voornaamWederhelft; }
            set { _voornaamWederhelft = value; OnPropertyChanged(nameof(VoornamenWederhelft)); }
        }
        public string EersteOuder
        {
            get { return _eersteOuder; }
            set { _eersteOuder = value; OnPropertyChanged(nameof(EersteOuder)); }
        }
        public string TweedeOuder
        {
            get { return _tweedeOuder; }
            set { _tweedeOuder = value; OnPropertyChanged(nameof(TweedeOuder)); }
        }
        public string GehuwdGeweestMet
        {
            get { return _gehuwdGeweestMet; }
            set { _gehuwdGeweestMet = value; OnPropertyChanged(nameof(GehuwdGeweestMet)); }
        }
        public string WeduwenaarVan
        {
            get { return _weduwenaarVan; }
            set { _weduwenaarVan = value; OnPropertyChanged(nameof(WeduwenaarVan)); }
        }
        public string AantalKinderen
        {
            get { return _aantalKinderen; }
            set { _aantalKinderen = value; OnPropertyChanged(nameof(AantalKinderen)); }
        }
        public string AantalKinderenMinderjarig
        {
            get { return _aantalKinderenMinderjarig; }
            set { _aantalKinderenMinderjarig = value; OnPropertyChanged(nameof(AantalKinderenMinderjarig)); }
        }
        public string AantalKinderenWaarvanOverleden
        {
            get { return _aantalKinderenWaarvanOverleden; }
            set { _aantalKinderenWaarvanOverleden = value; OnPropertyChanged(nameof(AantalKinderenWaarvanOverleden)); }
        }
        public string AangeverNaam
        {
            get { return _aangeverNaam; }
            set { _aangeverNaam = value; OnPropertyChanged(nameof(AangeverNaam)); }
        }
        public string AangeverPlaats
        {
            get { return _aangeverPlaats; }
            set { _aangeverPlaats = value; OnPropertyChanged(nameof(AangeverPlaats)); }
        }
        public string ErfgenaamVolledigeNaam
        {
            get { return _erfgenaamVolledigeNaam; }
            set { _erfgenaamVolledigeNaam = value; OnPropertyChanged(nameof(ErfgenaamVolledigeNaam)); }
        }
        public string ErfgenaamStraat
        {
            get { return _erfgenaamStraat; }
            set { _erfgenaamStraat = value; OnPropertyChanged(nameof(ErfgenaamStraat)); }
        }
        public string ErfgenaamPostcode
        {
            get { return _erfgenaamPostcode; }
            set { _erfgenaamPostcode = value; OnPropertyChanged(nameof(ErfgenaamPostcode)); }
        }
        public string ErfgenaamWoonplaats
        {
            get { return _erfgenaamWoonplaats; }
            set { _erfgenaamWoonplaats = value; OnPropertyChanged(nameof(ErfgenaamWoonplaats)); }
        }
        public DateTime DatumUitvaart
        {
            get { return _datumUitvaart; }
            set { _datumUitvaart = value; OnPropertyChanged(nameof(DatumUitvaart)); }
        }
        public DateTime TijdUitvaart
        {
            get { return _tijdUitvaart; }
            set { _tijdUitvaart = value; OnPropertyChanged(nameof(TijdUitvaart)); }
        }
        public string LocatieUitvaart
        {
            get { return _locatieUitvaart; }
            set { _locatieUitvaart = value; OnPropertyChanged(nameof(LocatieUitvaart)); }
        }
        public string TypeUitvaart
        {
            get { return _typeUitvaart; }
            set { _typeUitvaart = value; OnPropertyChanged(nameof(TypeUitvaart)); }
        }
        public string Schouwarts
        {
            get { return _schouwarts; }
            set { _schouwarts = value; OnPropertyChanged(nameof(Schouwarts)); }
        }
        public string UBS
        {
            get { return _ubs; }
            set { _ubs = value; OnPropertyChanged(nameof(UBS)); }
        }
        public bool Updated
        {
            get { return _updated; }
            set { _updated = value; OnPropertyChanged(nameof(Updated)); }
        }
        public bool InitialCreation
        {
            get { return _initialCreation; }
            set { _initialCreation = value; OnPropertyChanged(nameof(InitialCreation)); }
        }
        public string DestinationFile
        {
            get { return _destinationFile; }
            set
            {
                _destinationFile = value; OnPropertyChanged(nameof(DestinationFile));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public Guid DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; OnPropertyChanged(nameof(DocumentId)); }
        }
        public string UitvaartNummer
        {
            get { return _uitvaartnummer; }
            set { _uitvaartnummer = value; OnPropertyChanged(nameof(UitvaartNummer)); }
        }
        public string Burgelijkestaat
        {
            get { return _burgelijkestaat; }
            set { _burgelijkestaat = value; OnPropertyChanged(nameof(Burgelijkestaat)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(OverledeneAanhef) ||
                    !string.IsNullOrEmpty(OverledeneAchternaam) ||
                    !string.IsNullOrEmpty(OverledeneVoornaam) ||
                    !string.IsNullOrEmpty(OverledeneGeboorteplaats) ||
                    !string.IsNullOrEmpty(OverledeneAdres) ||
                    !string.IsNullOrEmpty(OverledenePostcode) ||
                    !string.IsNullOrEmpty(OverledeneWoonplaats) ||
                    !string.IsNullOrEmpty(OverledeneBSN);
        }
    }
}
