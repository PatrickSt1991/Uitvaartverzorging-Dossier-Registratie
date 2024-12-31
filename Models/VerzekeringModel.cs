using Dossier_Registratie.ViewModels;
using System;
using System.Collections.Generic;

namespace Dossier_Registratie.Models
{
    public class OverledeneVerzekeringModel : ViewModelBase
    {
        private Guid _id;
        private Guid _uitvaartId;
        private string? _verzekeringProperties;

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
        public string VerzekeringProperties
        {
            get { return _verzekeringProperties; }
            set { _verzekeringProperties = value; OnPropertyChanged(nameof(VerzekeringProperties)); }
        }
        public bool HasData()
        {
            return Id == Guid.Empty ||
                   UitvaartId == Guid.Empty ||
                   !string.IsNullOrEmpty(VerzekeringProperties);
        }
    }
    public class PolisVerzekering : ViewModelBase
    {
        private string? _verzekeringName;
        private List<Polis>? _polisInfoList;
        public string VerzekeringName
        {
            get { return _verzekeringName; }
            set { _verzekeringName = value; OnPropertyChanged(nameof(VerzekeringName)); }
        }
        public List<Polis> PolisInfoList
        {
            get { return _polisInfoList; }
            set { _polisInfoList = value; OnPropertyChanged(nameof(PolisInfoList)); }
        }
    }
    public class Polis : ViewModelBase
    {
        private string? _polisNr;
        private string? _polisBedrag;

        public string PolisNr
        {
            get { return _polisNr; }
            set { _polisNr = value; OnPropertyChanged(nameof(PolisNr)); }
        }
        public string PolisBedrag
        {
            get { return _polisBedrag; }
            set { _polisBedrag = value; OnPropertyChanged(nameof(PolisBedrag)); }
        }
    }
    public class VerzekeraarsModel : ViewModelBase
    {
        private Guid _id;
        private string? _name;
        private string? _afkorting;
        private bool? _herkomst;
        private bool? _verzekeraar;
        private bool? _lidnummer;
        private bool? _pakket;
        private bool _isDeleted;
        private string _btnBrush;
        private string? _postbusAddress = null;
        private string? _postbusName = null;
        private string? _addressStreet = null;
        private string? _addressHousenumber = null;
        private string? _addressHousenumberAddition = null;
        private string? _addressZipCode = null;
        private string? _addressCity = null;
        private string? _factuurType = null;
        private string _correspondentieType = null;
        private bool _isSelected;
        private bool _isOverrideFactuurAdress;
        private string? _telefoon;
        private bool? _customLogo;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }
        public string Afkorting
        {
            get { return _afkorting; }
            set { _afkorting = value; OnPropertyChanged(nameof(Afkorting)); }
        }
        public bool? IsHerkomst
        {
            get { return _herkomst; }
            set { _herkomst = value; OnPropertyChanged(nameof(IsHerkomst)); }
        }
        public bool? IsVerzekeraar
        {
            get { return _verzekeraar; }
            set { _verzekeraar = value; OnPropertyChanged(nameof(IsVerzekeraar)); }
        }
        public bool? HasLidnummer
        {
            get { return _lidnummer; }
            set { _lidnummer = value; OnPropertyChanged(nameof(HasLidnummer)); }
        }
        public bool? Pakket
        {
            get { return _pakket; }
            set { _pakket = value; OnPropertyChanged(nameof(Pakket)); }
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
        public string PostbusAddress
        {
            get { return _postbusAddress; }
            set
            {
                _postbusAddress = value;
                OnPropertyChanged(nameof(PostbusAddress));
            }
        }
        public string PostbusName
        {
            get { return _postbusName; }
            set
            {
                _postbusName = value;
                OnPropertyChanged(nameof(PostbusName));
            }
        }
        public string AddressStreet
        {
            get { return _addressStreet; }
            set
            {
                _addressStreet = value;
                OnPropertyChanged(nameof(AddressStreet));
            }
        }
        public string AddressHousenumber
        {
            get { return _addressHousenumber; }
            set
            {
                _addressHousenumber = value;
                OnPropertyChanged(nameof(AddressHousenumber));
            }
        }
        public string AddressHousenumberAddition
        {
            get { return _addressHousenumberAddition; }
            set
            {
                _addressHousenumberAddition = value;
                OnPropertyChanged(nameof(AddressHousenumberAddition));
            }
        }
        public string AddressZipCode
        {
            get { return _addressZipCode; }
            set
            {
                _addressZipCode = value;
                OnPropertyChanged(nameof(AddressZipCode));
            }
        }
        public string AddressCity
        {
            get { return _addressCity; }
            set
            {
                _addressCity = value;
                OnPropertyChanged(nameof(AddressCity));
            }
        }
        public string FactuurType
        {
            get { return _factuurType; }
            set
            {
                _factuurType = value;
                OnPropertyChanged(nameof(FactuurType));
            }
        }
        public string CorrespondentieType
        {
            get { return _correspondentieType; }
            set
            {
                _correspondentieType = value;
                OnPropertyChanged(nameof(CorrespondentieType));
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public bool IsOverrideFactuurAdress
        {
            get { return _isOverrideFactuurAdress; }
            set
            {
                _isOverrideFactuurAdress = value;
                OnPropertyChanged(nameof(IsOverrideFactuurAdress));
            }
        }
        public string Telefoon
        {
            get { return _telefoon; }
            set
            {
                _telefoon = value;
                OnPropertyChanged(nameof(Telefoon));
            }
        }
        public bool? CustomLogo
        {
            get { return _customLogo; }
            set { _customLogo = value; OnPropertyChanged(nameof(CustomLogo)); }
        }
    }
}
