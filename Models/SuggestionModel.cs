using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class SuggestionModel : ViewModelBase
    {
        private Guid _id;
        private string? _shortName;
        private string? _longName;
        private string? _street;
        private string? _houseNumber;
        private string? _zipCode;
        private string? _city;
        private string? _county;
        private bool _isDeleted;
        private string _btnBrush;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string? ShortName
        {
            get { return _shortName; }
            set { _shortName = value; OnPropertyChanged(nameof(ShortName)); }
        }
        public string? LongName
        {
            get { return _longName; }
            set { _longName = value; OnPropertyChanged(nameof(LongName)); }
        }
        public string? Street
        {
            get { return _street; }
            set { _street = value; OnPropertyChanged(nameof(Street)); }
        }
        public string? HouseNumber
        {
            get { return _houseNumber; }
            set { _houseNumber = value; OnPropertyChanged(nameof(HouseNumber)); }
        }
        public string? ZipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; OnPropertyChanged(nameof(ZipCode)); }
        }
        public string? City
        {
            get { return _city; }
            set { _city = value; OnPropertyChanged(nameof(City)); }
        }
        public string? County
        {
            get { return _county; }
            set { _county = value; OnPropertyChanged(nameof(County)); }
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
