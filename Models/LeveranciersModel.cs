using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class LeverancierContactModel : ViewModelBase
    {
        private string? _leverancierAdres;
        private string? _leverancierHuisnummer;
        private string? _leverancierHuisnummerToevoeging;
        private string? _leverancierPostcode;
        private string? _leverancierPlaats;

        public string LeverancierAdres
        {
            get { return _leverancierAdres; }
            set
            {
                _leverancierAdres = value;
                OnPropertyChanged(nameof(LeverancierAdres));
            }
        }
        public string LeverancierHuisnummer
        {
            get { return _leverancierHuisnummer; }
            set
            {
                _leverancierHuisnummer = value;
                OnPropertyChanged(nameof(LeverancierHuisnummer));
            }
        }
        public string LeverancierHuisnummerToevoeging
        {
            get { return _leverancierHuisnummerToevoeging; }
            set
            {
                _leverancierHuisnummerToevoeging = value;
                OnPropertyChanged(nameof(LeverancierHuisnummerToevoeging));
            }
        }
        public string LeverancierPostcode
        {
            get { return _leverancierPostcode; }
            set
            {
                _leverancierPostcode = value;
                OnPropertyChanged(nameof(_leverancierPostcode));
            }
        }
        public string LeverancierPlaats
        {
            get { return _leverancierPlaats; }
            set
            {
                _leverancierPlaats = value;
                OnPropertyChanged(nameof(LeverancierPlaats));
            }
        }
    }
    public class LeveranciersModel : ViewModelBase
    {
        private Guid _leverancierId;
        private string? _leverancierName;
        private string? _leverancierBeschrijving;
        private string? _leverancierAdres;
        private string? _leverancierContactGegevens;
        private bool _steenhouwer;
        private bool _bloemist;
        private bool _kisten;
        private bool _urnSieraden;
        private bool _isDeleted;
        private string _btnBrush;

        public Guid LeverancierId
        {
            get { return _leverancierId; }
            set { _leverancierId = value; OnPropertyChanged(nameof(_leverancierId)); }
        }
        public string LeverancierName
        {
            get { return _leverancierName; }
            set { _leverancierName = value; OnPropertyChanged(nameof(LeverancierName)); }
        }
        public string LeverancierBeschrijving
        {
            get { return _leverancierBeschrijving; }
            set { _leverancierBeschrijving = value; OnPropertyChanged(nameof(LeverancierBeschrijving)); }
        }
        public string LeverancierContactGegevens
        {
            get { return _leverancierContactGegevens; }
            set
            {
                _leverancierContactGegevens = value;
                OnPropertyChanged(nameof(LeverancierContactGegevens));
            }
        }
        public bool Steenhouwer
        {
            get { return _steenhouwer; }
            set { _steenhouwer = value; OnPropertyChanged(nameof(Steenhouwer)); }
        }
        public bool Bloemist
        {
            get { return _bloemist; }
            set { _bloemist = value; OnPropertyChanged(nameof(Bloemist)); }
        }
        public bool Kisten
        {
            get { return _kisten; }
            set { _kisten = value; OnPropertyChanged(nameof(Kisten)); }
        }
        public bool UrnSieraden
        {
            get { return _urnSieraden; }
            set { _urnSieraden = value; OnPropertyChanged(nameof(UrnSieraden)); }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged(nameof(IsDeleted)); }
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
    }
}
