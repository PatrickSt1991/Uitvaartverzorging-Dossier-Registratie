using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class ActiveAccountModel : ViewModelBase
    {
        private string _userName;
        private string _machineName;
        private DateTime? _loginTime;
        private DateTime? _logoutTime;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(nameof(UserName)); }
        }
        public string MachineName
        {
            get { return _machineName; }
            set { _machineName = value; OnPropertyChanged(nameof(MachineName)); }
        }
        public DateTime? LoginTime
        {
            get { return _loginTime; }
            set { _loginTime = value; OnPropertyChanged(nameof(LoginTime)); }
        }
        public DateTime? LogoutTime
        {
            get { return _logoutTime; }
            set { _logoutTime = value; OnPropertyChanged(nameof(LogoutTime)); }
        }
    }
    public class PermissionsModel : ViewModelBase
    {
        private Guid _id;
        private string _permissionName;
        private bool _isEnabled;
        private Guid _employeeId;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string PermissionName
        {
            get { return _permissionName; }
            set { _permissionName = value; OnPropertyChanged(nameof(PermissionName)); }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); }
        }
        public Guid EmployeeId
        {
            get { return _employeeId; }
            set { _employeeId = value; OnPropertyChanged(nameof(EmployeeId)); }
        }
    }
    public class WindowsAccount : ViewModelBase
    {
        private Guid _personeelId;
        private string _accountName;
        private bool _isActive;
        private string _permissionName;
        private Guid _permissionId;

        public Guid PersoneelId
        {
            get { return _personeelId; }
            set { _personeelId = value; OnPropertyChanged(nameof(PersoneelId)); }
        }
        public string AccountName
        {
            get { return _accountName; }
            set { _accountName = value; OnPropertyChanged(nameof(AccountName)); }
        }
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(nameof(IsActive)); }
        }
        public string PermissionName
        {
            get { return _permissionName; }
            set { _permissionName = value; OnPropertyChanged(nameof(PermissionName)); }
        }
        public Guid PermissionId
        {
            get { return _permissionId; }
            set { _permissionId = value; OnPropertyChanged(nameof(PermissionId)); }
        }
    }
    public class UitvaartLeiderModel : ViewModelBase
    {
        private Guid _id;
        private string? _uitvaartleider;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string Uitvaartleider
        {
            get { return _uitvaartleider; }
            set { _uitvaartleider = value; OnPropertyChanged(nameof(Uitvaartleider)); }
        }
    }
    public class WerknemersModel : ViewModelBase
    {
        private Guid _id;
        private string? _initialen;
        private string? _voornaam;
        private string? _roepnaam;
        private string? _tussenvoegsel;
        private string? _achternaam;
        private string? _volledigeNaam;
        private string? _geboorteplaats;
        private DateTime? _geboortedatum;
        private string? _email;
        private bool _isdeleted;
        private bool _isuitvaartverzorger;
        private bool _isdrager;
        private bool _ischauffeur;
        private bool _isOpbaren;
        private string _btnBrush;
        private string _permissionName;
        private Guid _permissionId;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string Initialen
        {
            get { return _initialen; }
            set { _initialen = value; OnPropertyChanged(nameof(Initialen)); }
        }
        public string Voornaam
        {
            get { return _voornaam; }
            set { _voornaam = value; OnPropertyChanged(nameof(Voornaam)); }
        }
        public string Roepnaam
        {
            get { return _roepnaam; }
            set { _roepnaam = value; OnPropertyChanged(nameof(Roepnaam)); }
        }
        public string Tussenvoegsel
        {
            get { return _tussenvoegsel; }
            set { _tussenvoegsel = value; OnPropertyChanged(nameof(Tussenvoegsel)); }
        }
        public string Achternaam
        {
            get { return _achternaam; }
            set { _achternaam = value; OnPropertyChanged(nameof(Achternaam)); }
        }
        public string VolledigeNaam
        {
            get { return _volledigeNaam; }
            set { _volledigeNaam = value; OnPropertyChanged(nameof(VolledigeNaam)); }
        }
        public string Geboorteplaats
        {
            get { return _geboorteplaats; }
            set { _geboorteplaats = value; OnPropertyChanged(nameof(Geboorteplaats)); }
        }
        public DateTime? Geboortedatum
        {
            get { return _geboortedatum; }
            set { _geboortedatum = value; OnPropertyChanged(nameof(Geboortedatum)); }
        }
        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }
        public bool IsDeleted
        {
            get { return (bool)_isdeleted; }
            set { _isdeleted = value; OnPropertyChanged(nameof(IsDeleted)); }
        }
        public bool IsUitvaartverzorger
        {
            get { return (bool)_isuitvaartverzorger; }
            set { _isuitvaartverzorger = value; OnPropertyChanged(nameof(IsUitvaartverzorger)); }
        }
        public bool IsDrager
        {
            get { return (bool)_isdrager; }
            set { _isdrager = value; OnPropertyChanged(nameof(IsDrager)); }
        }
        public bool IsChauffeur
        {
            get { return (bool)_ischauffeur; }
            set { _ischauffeur = value; OnPropertyChanged(nameof(IsChauffeur)); }
        }
        public bool IsOpbaren
        {
            get { return (bool)_isOpbaren; }
            set { _isOpbaren = value; OnPropertyChanged(nameof(IsOpbaren)); }
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
        public string PermissionName
        {
            get { return _permissionName; }
            set
            {
                if (_permissionName != value)
                {
                    _permissionName = value;
                    OnPropertyChanged(nameof(PermissionName));
                }
            }
        }
        public Guid PermissionId
        {
            get { return _permissionId; }
            set { _permissionId = value; OnPropertyChanged(nameof(PermissionId)); }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(Initialen) &&
                   !string.IsNullOrEmpty(Voornaam) &&
                   !string.IsNullOrEmpty(Roepnaam) &&
                   !string.IsNullOrEmpty(Achternaam) &&
                   !string.IsNullOrEmpty(Geboorteplaats) &&
                   Geboortedatum != DateTime.MinValue &&
                   !string.IsNullOrEmpty(Email) &&
                   PermissionId != Guid.Empty;
        }
    }
}
