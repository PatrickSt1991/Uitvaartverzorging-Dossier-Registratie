using Dossier_Registratie.ViewModels;
using System;
using System.ComponentModel;

namespace Dossier_Registratie.Models
{
    public class OverledeneMiscModel : ViewModelBase
    {
        private Guid _id;
        private Guid _uitvaartId;
        private Guid _rouwbrievenId;
        private string _aantalRouwbrieven;
        private string _aantalUitnodigingen;
        private string _aantalKennisgeving;
        private string _advertenties;
        private string _ubs;
        private string _aulaNaam;
        private int _aulaPersonen;

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
        public Guid RouwbrievenId
        {
            get { return _rouwbrievenId; }
            set { _rouwbrievenId = value; OnPropertyChanged(nameof(RouwbrievenId)); }
        }
        public string AantalRouwbrieven
        {
            get { return _aantalRouwbrieven; }
            set { _aantalRouwbrieven = value; OnPropertyChanged(nameof(AantalRouwbrieven)); }
        }
        public string AantalUitnodigingen
        {
            get { return _aantalKennisgeving; }
            set { _aantalKennisgeving = value; OnPropertyChanged(nameof(AantalUitnodigingen)); }
        }
        public string AantalKennisgeving
        {
            get { return _aantalKennisgeving; }
            set { _aantalKennisgeving = value; OnPropertyChanged(nameof(AantalKennisgeving)); }
        }
        public string Advertenties
        {
            get { return _advertenties; }
            set { _advertenties = value; OnPropertyChanged(nameof(Advertenties)); }
        }
        public string UBS
        {
            get { return _ubs; }
            set { _ubs = value; OnPropertyChanged(nameof(UBS)); }
        }
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

        public bool HasData()
        {
            /*
            return !string.IsNullOrEmpty(AantalRouwbrieven) ||
                   !string.IsNullOrEmpty(Advertenties) ||
                   !string.IsNullOrEmpty(UBS);
            */
            return true;
        }
    }
    public class OverledeneUitvaartModel : ViewModelBase, IDataErrorInfo
    {
        private Guid _id;
        private Guid _uitvaartId;
        private DateTime? _datumTijdCondoleance;
        private DateTime? _datumTijdUitvaart;
        private DateTime? _datumTijdDienst;
        private string _consumptiesCondoleance;
        private string _typeDienst;
        private string _locatieUitvaart;
        private string _locatieDienst;
        private string _afscheidDienst;
        private string _muziekDienst;
        private string _beslotenDienst;
        private string _volgautoDienst;
        private string _consumptiesDienst;
        private string _kistDienst;
        private string _condoleanceYesNo;
        private string _spreker;
        private string _powerpoint;
        private int? _aantalTijdsBlokken;
        private int? _tijdBlokken;

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
        public DateTime? DatumTijdCondoleance
        {
            get { return _datumTijdCondoleance; }
            set
            {
                if (_datumTijdCondoleance != value)
                {
                    _datumTijdCondoleance = value;
                    OnPropertyChanged(nameof(DatumTijdCondoleance));
                    OnPropertyChanged(nameof(DatumCondoleance));
                    OnPropertyChanged(nameof(TijdCondoleance));
                }
            }
        }
        public DateTime? DatumCondoleance
        {
            get => DatumTijdCondoleance?.Date;
            set
            {
                if (DatumTijdCondoleance?.Date != value)
                {
                    DatumTijdCondoleance = value.HasValue
                        ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day,
                                       DatumTijdCondoleance?.Hour ?? 0, DatumTijdCondoleance?.Minute ?? 0, DatumTijdCondoleance?.Second ?? 0)
                        : (DateTime?)null;
                    OnPropertyChanged(nameof(DatumCondoleance));
                    OnPropertyChanged(nameof(TijdCondoleance));
                }
            }
        }
        public TimeSpan? TijdCondoleance
        {
            get => DatumTijdCondoleance?.TimeOfDay;
            set
            {
                if (DatumTijdCondoleance.HasValue && value.HasValue)
                {
                    DatumTijdCondoleance = new DateTime(DatumTijdCondoleance.Value.Year, DatumTijdCondoleance.Value.Month, DatumTijdCondoleance.Value.Day,
                                                      value.Value.Hours, value.Value.Minutes, value.Value.Seconds);
                }
                else if (!value.HasValue)
                {
                    DatumTijdCondoleance = DatumTijdCondoleance?.Date;
                }
                OnPropertyChanged(nameof(TijdCondoleance));
                OnPropertyChanged(nameof(DatumCondoleance));
            }
        }
        public string ConsumptiesCondoleance
        {
            get { return _consumptiesCondoleance; }
            set { _consumptiesCondoleance = value; OnPropertyChanged(nameof(ConsumptiesCondoleance)); }
        }
        public string TypeDienst
        {
            get { return _typeDienst; }
            set { _typeDienst = value; OnPropertyChanged(nameof(TypeDienst)); }
        }
        public DateTime? DatumTijdUitvaart
        {
            get { return _datumTijdUitvaart; }
            set
            {
                if (_datumTijdUitvaart != value)
                {
                    _datumTijdUitvaart = value;
                    OnPropertyChanged(nameof(DatumTijdUitvaart));
                    OnPropertyChanged(nameof(DatumUitvaart));
                    OnPropertyChanged(nameof(TijdUitvaart));
                }
            }
        }
        public DateTime? DatumUitvaart
        {
            get => DatumTijdUitvaart?.Date;
            set
            {
                if (DatumTijdUitvaart?.Date != value)
                {
                    DatumTijdUitvaart = value.HasValue
                        ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day,
                                       DatumTijdUitvaart?.Hour ?? 0, DatumTijdUitvaart?.Minute ?? 0, DatumTijdUitvaart?.Second ?? 0)
                        : (DateTime?)null;
                    OnPropertyChanged(nameof(DatumUitvaart));
                    OnPropertyChanged(nameof(TijdUitvaart));
                }
            }
        }
        public TimeSpan? TijdUitvaart
        {
            get => DatumTijdUitvaart?.TimeOfDay;
            set
            {
                if (DatumTijdUitvaart.HasValue && value.HasValue)
                {
                    DatumTijdUitvaart = new DateTime(DatumTijdUitvaart.Value.Year, DatumTijdUitvaart.Value.Month, DatumTijdUitvaart.Value.Day,
                                                      value.Value.Hours, value.Value.Minutes, value.Value.Seconds);
                }
                else if (!value.HasValue)
                {
                    DatumTijdUitvaart = DatumTijdUitvaart?.Date;
                }
                OnPropertyChanged(nameof(TijdUitvaart));
                OnPropertyChanged(nameof(DatumUitvaart));
            }
        }
        public string LocatieUitvaart
        {
            get { return _locatieUitvaart; }
            set { _locatieUitvaart = value; OnPropertyChanged(nameof(LocatieUitvaart)); }
        }
        public DateTime? DatumTijdDienst
        {
            get { return _datumTijdDienst; }
            set
            {
                _datumTijdDienst = value;
                OnPropertyChanged(nameof(DatumTijdDienst));
                OnPropertyChanged(nameof(DatumDienst));
                OnPropertyChanged(nameof(TijdDienst));
            }
        }
        public DateTime? DatumDienst
        {
            get => DatumTijdDienst?.Date;
            set
            {
                if (DatumTijdDienst?.Date != value)
                {
                    DatumTijdDienst = value.HasValue
                        ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day,
                                       DatumTijdDienst?.Hour ?? 0, DatumTijdDienst?.Minute ?? 0, DatumTijdDienst?.Second ?? 0)
                        : (DateTime?)null;
                    OnPropertyChanged(nameof(DatumDienst));
                    OnPropertyChanged(nameof(TijdDienst));
                }
            }
        }
        public TimeSpan? TijdDienst
        {
            get => DatumTijdDienst?.TimeOfDay;
            set
            {
                if (DatumTijdDienst.HasValue && value.HasValue)
                {
                    DatumTijdDienst = new DateTime(DatumTijdDienst.Value.Year, DatumTijdDienst.Value.Month, DatumTijdDienst.Value.Day,
                                                      value.Value.Hours, value.Value.Minutes, value.Value.Seconds);
                }
                else if (!value.HasValue)
                {
                    DatumTijdDienst = DatumTijdDienst?.Date;
                }
                OnPropertyChanged(nameof(TijdDienst));
                OnPropertyChanged(nameof(DatumDienst));
            }
        }
        public string LocatieDienst
        {
            get { return _locatieDienst; }
            set { _locatieDienst = value; OnPropertyChanged(nameof(LocatieDienst)); }
        }
        public string AfscheidDienst
        {
            get { return _afscheidDienst; }
            set { _afscheidDienst = value; OnPropertyChanged(nameof(AfscheidDienst)); }
        }
        public string MuziekDienst
        {
            get { return _muziekDienst; }
            set { _muziekDienst = value; OnPropertyChanged(nameof(MuziekDienst)); }
        }
        public string BeslotenDienst
        {
            get { return _beslotenDienst; }
            set { _beslotenDienst = value; OnPropertyChanged(nameof(BeslotenDienst)); }
        }
        public string VolgAutoDienst
        {
            get { return _volgautoDienst; }
            set { _volgautoDienst = value; OnPropertyChanged(nameof(VolgAutoDienst)); }

        }
        public string ConsumptiesDienst
        {
            get { return _consumptiesDienst; }
            set { _consumptiesDienst = value; OnPropertyChanged(nameof(ConsumptiesDienst)); }
        }
        public string KistDienst
        {
            get { return _kistDienst; }
            set { _kistDienst = value; OnPropertyChanged(nameof(KistDienst)); }
        }
        public string CondoleanceYesNo
        {
            get { return _condoleanceYesNo; }
            set { _condoleanceYesNo = value; OnPropertyChanged(nameof(CondoleanceYesNo)); }
        }
        public string Spreker
        {
            get { return _spreker; }
            set { _spreker = value; OnPropertyChanged(nameof(Spreker)); }
        }
        public string PowerPoint
        {
            get { return _powerpoint; }
            set { _powerpoint = value; OnPropertyChanged(nameof(PowerPoint)); }
        }
        public int? AantalTijdsBlokken
        {
            get { return _aantalTijdsBlokken; }
            set { _aantalTijdsBlokken = value; OnPropertyChanged(nameof(AantalTijdsBlokken)); }
        }
        public int? TijdBlokken
        {
            get { return _tijdBlokken; }
            set { _tijdBlokken = value; OnPropertyChanged(nameof(TijdBlokken)); }
        }

        public string Error => string.Empty;
        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (string.IsNullOrEmpty(TypeDienst) ||
                    (DateTime.MinValue == DatumTijdUitvaart) ||
                    (string.IsNullOrEmpty(LocatieUitvaart)) ||
                    (string.IsNullOrEmpty(LocatieDienst)) ||
                    (DateTime.MinValue == DatumTijdDienst) ||
                    (string.IsNullOrEmpty(BeslotenDienst)))
                {
                    result = "Validatie velden niet correct.";
                }
                return result;
            }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(TypeDienst) &&
                   !string.IsNullOrEmpty(LocatieUitvaart) &&
                   !string.IsNullOrEmpty(LocatieDienst) &&
                   !string.IsNullOrEmpty(AfscheidDienst) &&
                   !string.IsNullOrEmpty(MuziekDienst) &&
                   !string.IsNullOrEmpty(BeslotenDienst) &&
                   !string.IsNullOrEmpty(VolgAutoDienst) &&
                   !string.IsNullOrEmpty(KistDienst);
        }
    }
    public class OverledeneRouwbrieven : ViewModelBase
    {
        private Guid _rouwbrievenId;
        private string _rouwbrievenName;
        private bool _isDeleted;
        private string _btnBrush;

        public Guid RouwbrievenId
        {
            get { return _rouwbrievenId; }
            set { _rouwbrievenId = value; OnPropertyChanged(nameof(RouwbrievenId)); }
        }
        public string RouwbrievenName
        {
            get { return _rouwbrievenName; }
            set { _rouwbrievenName = value; OnPropertyChanged(nameof(RouwbrievenName)); }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged(nameof(IsDeleted)); }
        }
        public string BtnBrush
        {
            get { return _btnBrush; }
            set { _btnBrush = value; OnPropertyChanged(nameof(BtnBrush)); }
        }
    }
}
