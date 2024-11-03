using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class SettingsModel : ViewModelBase
    {
        private Guid _settingId;
        private string _settingName;
        private string _settingValue;

        public Guid SettingId
        {
            get { return _settingId; }
            set { _settingId = value; OnPropertyChanged(nameof(SettingId)); }
        }
        public string SettingName
        {
            get { return _settingName; }
            set { _settingName = value; OnPropertyChanged(nameof(SettingName)); }
        }
        public string SettingValue
        {
            get { return _settingValue; }
            set { _settingValue = value; OnPropertyChanged(nameof(SettingValue)); }
        }
    }
}
