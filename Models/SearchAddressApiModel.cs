using Dossier_Registratie.ViewModels;

namespace Dossier_Registratie.Models
{
    public class SearchAddressApiModel : ViewModelBase
    {
        private string? _location;
        private string? _postalcode;
        private string? _housenumber;
        private string? _housenumberAddition;
        private string? _street;
        private string? _city;
        private string? _county;

        public string? Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged(nameof(Location)); }
        }
        public string? PostalCode
        {
            get { return _postalcode; }
            set { _postalcode = value; OnPropertyChanged(nameof(PostalCode)); }
        }
        public string? HouseNumber
        {
            get { return _housenumber; }
            set { _housenumber = value; OnPropertyChanged(nameof(HouseNumber)); }
        }
        public string? HouseNumberAddition
        {
            get { return _housenumberAddition; }
            set { _housenumberAddition = value; OnPropertyChanged(nameof(HouseNumberAddition)); }
        }
        public string? Street
        {
            get { return _street; }
            set { _street = value; OnPropertyChanged(nameof(Street)); }
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
    }
}
