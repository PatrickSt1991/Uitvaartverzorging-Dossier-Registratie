using Dossier_Registratie.ViewModels;
using System;
using System.ComponentModel;

namespace Dossier_Registratie.Models
{
    public class ExtraInfoUitvaartleiderModel : ViewModelBase
    {
        private Guid _uitvaartId;
        private Guid _personeelId;
        private string? _personeelNaam;
        private string? _uitvaartNummer;
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
    }
    public class OverledeneExtraInfoModel : ViewModelBase
    {
        private Guid _id;
        private Guid _uitvaartId;
        private string? _overledeneBurgelijkestaat;
        private string? _overledeneGescheidenVan;
        private string? _overledeneWedenaarVan;
        private string? _overledeneAantalKinderen;
        private string? _overledeneKinderenMinderjarig;
        private string? _overledeneKinderenMinderjarigOverleden;
        private string? _overledeneEersteOuder;
        private string? _overledeneEersteOuderOverleden;
        private string? _overledeneTweedeOuder;
        private string? _overledeneTweedeOuderOverleden;
        private string? _overledeneLevensovertuiging;
        private string? _overledeneExecuteur;
        private string? _overledeneExecuteurTelefoon;
        private string? _overledeneTestament;
        private string? _overledeneTrouwboekje;
        private DateTime? _overledeneTrouwDatumTijd;
        private DateTime? _overledeneGeregistreerdDatumTijd;
        private string? _overledeneNotaris;
        private string? _overledeneNotarisTelefoon;
        private string? _naamWederhelft;
        private string? _voornaamWederhelft;

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
        public string OverledeneBurgelijkestaat
        {
            get { return _overledeneBurgelijkestaat; }
            set { _overledeneBurgelijkestaat = value; OnPropertyChanged(nameof(OverledeneBurgelijkestaat)); }
        }
        public string OverledeneGescheidenVan
        {
            get { return _overledeneGescheidenVan; }
            set { _overledeneGescheidenVan = value; OnPropertyChanged(nameof(OverledeneGescheidenVan)); }
        }
        public string OverledeneWedenaarVan
        {
            get { return _overledeneWedenaarVan; }
            set { _overledeneWedenaarVan = value; OnPropertyChanged(nameof(OverledeneWedenaarVan)); }
        }
        public string OverledeneTrouwboekje
        {
            get { return _overledeneTrouwboekje; }
            set { _overledeneTrouwboekje = value; OnPropertyChanged(nameof(OverledeneTrouwboekje)); }
        }
        public string OverledeneAantalKinderen
        {
            get { return _overledeneAantalKinderen; }
            set { _overledeneAantalKinderen = value; OnPropertyChanged(nameof(OverledeneAantalKinderen)); }
        }
        public string OverledeneKinderenMinderjarig
        {
            get { return _overledeneKinderenMinderjarig; }
            set { _overledeneKinderenMinderjarig = value; OnPropertyChanged(nameof(OverledeneKinderenMinderjarig)); }
        }
        public string OverledeneKinderenMinderjarigOverleden
        {
            get { return _overledeneKinderenMinderjarigOverleden; }
            set { _overledeneKinderenMinderjarigOverleden = value; OnPropertyChanged(nameof(OverledeneKinderenMinderjarigOverleden)); }
        }
        public string OverledeneEersteOuder
        {
            get { return _overledeneEersteOuder; }
            set { _overledeneEersteOuder = value; OnPropertyChanged(nameof(OverledeneEersteOuder)); }
        }
        public string OverledeneEersteOuderOverleden
        {
            get { return _overledeneEersteOuderOverleden; }
            set { _overledeneEersteOuderOverleden = value; OnPropertyChanged(nameof(OverledeneEersteOuderOverleden)); }
        }
        public string OverledeneTweedeOuderOverleden
        {
            get { return _overledeneTweedeOuderOverleden; }
            set { _overledeneTweedeOuderOverleden = value; OnPropertyChanged(nameof(OverledeneTweedeOuderOverleden)); }
        }
        public string OverledeneTweedeOuder
        {
            get { return _overledeneTweedeOuder; }
            set { _overledeneTweedeOuder = value; OnPropertyChanged(nameof(OverledeneTweedeOuder)); }
        }
        public string OverledeneLevensovertuiging
        {
            get { return _overledeneLevensovertuiging; }
            set { _overledeneLevensovertuiging = value; OnPropertyChanged(nameof(OverledeneLevensovertuiging)); }
        }
        public string OverledeneExecuteur
        {
            get { return _overledeneExecuteur; }
            set { _overledeneExecuteur = value; OnPropertyChanged(nameof(OverledeneExecuteur)); }
        }
        public string OverledeneExecuteurTelefoon
        {
            get { return _overledeneExecuteurTelefoon; }
            set { _overledeneExecuteurTelefoon = value; OnPropertyChanged(nameof(OverledeneExecuteurTelefoon)); }
        }
        public string OverledeneTestament
        {
            get { return _overledeneTestament; }
            set { _overledeneTestament = value; OnPropertyChanged(nameof(OverledeneTestament)); }
        }
        public DateTime? OverledeneTrouwDatumTijd
        {
            get => _overledeneTrouwDatumTijd;
            set
            {
                if (_overledeneTrouwDatumTijd != value)
                {
                    _overledeneTrouwDatumTijd = value == DateTime.MinValue ? (DateTime?)null : value;
                    OnPropertyChanged(nameof(OverledeneTrouwDatumTijd));
                    OnPropertyChanged(nameof(OverledeneTrouwDatum));
                    OnPropertyChanged(nameof(OverledeneTrouwTijd));
                }
            }
        }
        public DateTime? OverledeneTrouwDatum
        {
            get => _overledeneTrouwDatumTijd?.Date == DateTime.MinValue.Date ? (DateTime?)null : _overledeneTrouwDatumTijd?.Date;
            set
            {
                if (_overledeneTrouwDatumTijd?.Date != value)
                {
                    if (value == DateTime.MinValue || !value.HasValue)
                    {
                        _overledeneTrouwDatumTijd = null;
                    }
                    else
                    {
                        var timePart = _overledeneTrouwDatumTijd?.TimeOfDay ?? TimeSpan.Zero;
                        _overledeneTrouwDatumTijd = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, timePart.Hours, timePart.Minutes, timePart.Seconds);
                    }

                    OnPropertyChanged(nameof(OverledeneTrouwDatum));
                    OnPropertyChanged(nameof(OverledeneTrouwTijd));
                }
            }
        }
        public TimeSpan? OverledeneTrouwTijd
        {
            get => _overledeneTrouwDatumTijd?.TimeOfDay == TimeSpan.Zero ? (TimeSpan?)null : _overledeneTrouwDatumTijd?.TimeOfDay;
            set
            {
                if (_overledeneTrouwDatumTijd.HasValue)
                {
                    if (value == TimeSpan.Zero || !value.HasValue)
                    {
                        _overledeneTrouwDatumTijd = null;
                    }
                    else
                    {
                        var datePart = _overledeneTrouwDatumTijd.Value.Date;
                        _overledeneTrouwDatumTijd = new DateTime(datePart.Year, datePart.Month, datePart.Day, value.Value.Hours, value.Value.Minutes, value.Value.Seconds);
                    }
                }
                else if (value.HasValue)
                {
                    _overledeneTrouwDatumTijd = DateTime.Today.Add(value.Value);
                }
                else
                {
                    _overledeneTrouwDatumTijd = DateTime.Today;
                }

                OnPropertyChanged(nameof(OverledeneTrouwTijd));
                OnPropertyChanged(nameof(OverledeneTrouwDatum));
            }
        }
        public DateTime? OverledeneGeregistreerdDatumTijd
        {
            get { return _overledeneGeregistreerdDatumTijd; }
            set
            {
                if (_overledeneGeregistreerdDatumTijd != value)
                {
                    _overledeneGeregistreerdDatumTijd = value;
                    OnPropertyChanged(nameof(OverledeneGeregistreerdDatumTijd));
                    OnPropertyChanged(nameof(OverledeneGeregistreerdDatum));
                    OnPropertyChanged(nameof(OverledeneGeregistreerdTijd));
                }
            }
        }
        public DateTime? OverledeneGeregistreerdDatum
        {
            get => OverledeneGeregistreerdDatumTijd?.Date;
            set
            {
                if (OverledeneGeregistreerdDatumTijd?.Date != value)
                {
                    if (value == DateTime.MinValue)
                    {
                        OverledeneGeregistreerdDatumTijd = null;
                    }
                    else
                    {

                        OverledeneGeregistreerdDatumTijd = value.HasValue
                            ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day,
                                           OverledeneGeregistreerdDatumTijd?.Hour ?? 0, OverledeneGeregistreerdDatumTijd?.Minute ?? 0, OverledeneGeregistreerdDatumTijd?.Second ?? 0)
                            : (DateTime?)null;
                    }
                    OnPropertyChanged(nameof(OverledeneGeregistreerdDatum));
                    OnPropertyChanged(nameof(OverledeneGeregistreerdTijd));
                }
            }
        }
        public TimeSpan? OverledeneGeregistreerdTijd
        {
            get => OverledeneGeregistreerdDatumTijd?.TimeOfDay;
            set
            {
                if (OverledeneGeregistreerdDatumTijd.HasValue && value.HasValue)
                {
                    if (value == TimeSpan.Zero)
                    {
                        OverledeneGeregistreerdDatumTijd = null;
                    }
                    else
                    {
                        OverledeneGeregistreerdDatumTijd = new DateTime(OverledeneGeregistreerdDatumTijd.Value.Year, OverledeneGeregistreerdDatumTijd.Value.Month, OverledeneGeregistreerdDatumTijd.Value.Day,
                                                          value.Value.Hours, value.Value.Minutes, value.Value.Seconds);
                    }
                }
                else if (!value.HasValue)
                {
                    OverledeneGeregistreerdDatumTijd = OverledeneGeregistreerdDatumTijd?.Date;
                }
                OnPropertyChanged(nameof(OverledeneGeregistreerdTijd));
                OnPropertyChanged(nameof(OverledeneGeregistreerdDatum));
            }
        }
        public string OverledeneNotaris
        {
            get { return _overledeneNotaris; }
            set { _overledeneNotaris = value; OnPropertyChanged(nameof(OverledeneNotaris)); }
        }
        public string OverledeneNotarisTelefoon
        {
            get { return _overledeneNotarisTelefoon; }
            set { _overledeneNotarisTelefoon = value; OnPropertyChanged(nameof(OverledeneNotarisTelefoon)); }
        }
        public string NaamWederhelft
        {
            get { return _naamWederhelft; }
            set { _naamWederhelft = value; OnPropertyChanged(nameof(NaamWederhelft)); }
        }
        public string VoornaamWederhelft
        {
            get { return _voornaamWederhelft; }
            set { _voornaamWederhelft = value; OnPropertyChanged(nameof(VoornaamWederhelft)); }
        }
        public bool HasData()
        {
            return true;
        }

    }
    public class OpdrachtgeverPersoonsGegevensModel : ViewModelBase, IDataErrorInfo
    {
        private Guid _id;
        private Guid _uitvaartId;
        private string? _opdrachtgeverAanhef;
        private string? _opdrachtgeverAchternaam;
        private string? _opdrachtgeverVoornaamen;
        private string? _opdrachtgeverTussenvoegsel;
        private DateTime? _opdrachtgeverGeboortedatum;
        private string? _opdrachtgeverGeboorteplaats;
        private string? _opdrachtgeverLeeftijd;
        private string? _opdrachtgeverStraat;
        private string? _opdrachtgeverHuisnummer;
        private string? _opdrachtgeverHuisnummerToevoeging;
        private string? _opdrachtgeverPostcode;
        private string? _opdrachtgeverWoonplaats;
        private string? _opdrachtgeverGemeente;
        private string? _opdrachtgeverTelefoon;
        private string? _opdrachtgeverBSN;
        private string? _opdrachtgeverRelatieTotOverledene;
        private string? _opdrachtgeverExtraInformatie;
        private string? _opdrachtgeverEmail;

        public bool HasData()
        {
            return !string.IsNullOrEmpty(OpdrachtgeverAanhef) &&
                   !string.IsNullOrEmpty(OpdrachtgeverAchternaam) &&
                   !string.IsNullOrEmpty(OpdrachtgeverVoornaamen) &&
                   !string.IsNullOrEmpty(OpdrachtgeverStraat) &&
                   !string.IsNullOrEmpty(OpdrachtgeverHuisnummer) &&
                   !string.IsNullOrEmpty(OpdrachtgeverPostcode) &&
                   !string.IsNullOrEmpty(OpdrachtgeverWoonplaats) &&
                   !string.IsNullOrEmpty(OpdrachtgeverTelefoon) &&
                   !string.IsNullOrEmpty(OpdrachtgeverRelatieTotOverledene);
        }

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
        public string OpdrachtgeverAanhef
        {
            get { return _opdrachtgeverAanhef; }
            set { _opdrachtgeverAanhef = value; OnPropertyChanged(nameof(OpdrachtgeverAanhef)); }
        }
        public string OpdrachtgeverAchternaam
        {
            get { return _opdrachtgeverAchternaam; }
            set { _opdrachtgeverAchternaam = value; OnPropertyChanged(nameof(OpdrachtgeverAchternaam)); }
        }
        public string OpdrachtgeverVoornaamen
        {
            get { return _opdrachtgeverVoornaamen; }
            set { _opdrachtgeverVoornaamen = value; OnPropertyChanged(nameof(OpdrachtgeverVoornaamen)); }
        }
        public string OpdrachtgeverTussenvoegsel
        {
            get { return _opdrachtgeverTussenvoegsel; }
            set { _opdrachtgeverTussenvoegsel = value; OnPropertyChanged(nameof(OpdrachtgeverTussenvoegsel)); }
        }
        public DateTime? OpdrachtgeverGeboortedatum
        {
            get { return _opdrachtgeverGeboortedatum; }
            set { _opdrachtgeverGeboortedatum = value; OnPropertyChanged(nameof(OpdrachtgeverGeboortedatum)); }
        }
        public string OpdrachtgeverGeboorteplaats
        {
            get { return _opdrachtgeverGeboorteplaats; }
            set { _opdrachtgeverGeboorteplaats = value; OnPropertyChanged(nameof(OpdrachtgeverGeboorteplaats)); }
        }
        public string OpdrachtgeverLeeftijd
        {
            get { return _opdrachtgeverLeeftijd; }
            set { _opdrachtgeverLeeftijd = value; OnPropertyChanged(nameof(OpdrachtgeverLeeftijd)); }
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
        public string OpdrachtgeverGemeente
        {
            get { return _opdrachtgeverGemeente; }
            set { _opdrachtgeverGemeente = value; OnPropertyChanged(nameof(OpdrachtgeverGemeente)); }
        }
        public string OpdrachtgeverTelefoon
        {
            get { return _opdrachtgeverTelefoon; }
            set { _opdrachtgeverTelefoon = value; OnPropertyChanged(nameof(OpdrachtgeverTelefoon)); }
        }
        public string OpdrachtgeverBSN
        {
            get { return _opdrachtgeverBSN; }
            set { _opdrachtgeverBSN = value; OnPropertyChanged(nameof(OpdrachtgeverBSN)); }
        }
        public string OpdrachtgeverRelatieTotOverledene
        {
            get { return _opdrachtgeverRelatieTotOverledene; }
            set { _opdrachtgeverRelatieTotOverledene = value; OnPropertyChanged(nameof(OpdrachtgeverRelatieTotOverledene)); }
        }
        public string OpdrachtgeverExtraInformatie
        {
            get { return _opdrachtgeverExtraInformatie; }
            set { _opdrachtgeverExtraInformatie = value; OnPropertyChanged(nameof(OpdrachtgeverExtraInformatie)); }
        }
        public string OpdrachtgeverEmail
        {
            get { return _opdrachtgeverEmail; }
            set { _opdrachtgeverEmail = value; OnPropertyChanged(nameof(OpdrachtgeverEmail)); }
        }
        public string Error => string.Empty;
        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (string.IsNullOrEmpty(OpdrachtgeverAanhef) ||
                    (string.IsNullOrEmpty(OpdrachtgeverAchternaam)) ||
                    (string.IsNullOrEmpty(OpdrachtgeverVoornaamen)) ||
                    (string.IsNullOrEmpty(OpdrachtgeverStraat)) ||
                    (string.IsNullOrEmpty(OpdrachtgeverHuisnummer)) ||
                    (string.IsNullOrEmpty(OpdrachtgeverPostcode)) ||
                    (string.IsNullOrEmpty(OpdrachtgeverWoonplaats)) ||
                    (string.IsNullOrEmpty(OpdrachtgeverTelefoon)) ||
                    (string.IsNullOrEmpty(OpdrachtgeverRelatieTotOverledene)))
                {
                    result = "Validatie velden niet correct.";
                }
                return result;
            }
        }
    }
}
