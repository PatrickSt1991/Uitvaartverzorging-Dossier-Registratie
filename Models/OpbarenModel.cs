using Dossier_Registratie.ViewModels;
using System;
using System.ComponentModel;

namespace Dossier_Registratie.Models
{
    public class OverledeneOpbarenModel : ViewModelBase, IDataErrorInfo
    {
        private Guid _opbaringId;
        private Guid _uitvaartId;
        private string? _opbaringLocatie;
        private Guid _opbaringKistId;
        private string? _opbaringKistOmschrijving;
        private Guid _opbaringKistLengte;
        private string? _opbaringVerzorging;
        private string? _opbaringVerzorgingJson;
        private string? _opbaringKoeling;
        private string? _opbaringKledingMee;
        private string? _opbaringKledingRetour;
        private string? _opbaringSieraden;
        private string? _opbaringSieradenOmschrijving;
        private string? _opbaringSieradenRetour;
        private string? _opbaringBezoek;
        private string? _opbaringExtraInfo;

        public Guid OpbaringId
        {
            get { return _opbaringId; }
            set { _opbaringId = value; OnPropertyChanged(nameof(OpbaringId)); }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public string OpbaringLocatie
        {
            get { return _opbaringLocatie; }
            set { _opbaringLocatie = value; OnPropertyChanged(nameof(OpbaringLocatie)); }
        }
        public Guid OpbaringKistId
        {
            get { return _opbaringKistId; }
            set { _opbaringKistId = value; OnPropertyChanged(nameof(OpbaringKistId)); }
        }
        public string OpbaringKistOmschrijving
        {
            get { return _opbaringKistOmschrijving; }
            set { _opbaringKistOmschrijving = value; OnPropertyChanged(nameof(OpbaringKistOmschrijving)); }
        }
        public Guid OpbaringKistLengte
        {
            get { return _opbaringKistLengte; }
            set { _opbaringKistLengte = value; OnPropertyChanged(nameof(OpbaringKistLengte)); }
        }
        public string OpbaringVerzorging
        {
            get { return _opbaringVerzorging; }
            set { _opbaringVerzorging = value; OnPropertyChanged(nameof(OpbaringVerzorging)); }
        }
        public string OpbaringVerzorgingJson
        {
            get { return _opbaringVerzorgingJson; }
            set { _opbaringVerzorgingJson = value; OnPropertyChanged(nameof(OpbaringVerzorgingJson)); }
        }
        public string OpbaringKoeling
        {
            get { return _opbaringKoeling; }
            set { _opbaringKoeling = value; OnPropertyChanged(nameof(OpbaringKoeling)); }
        }
        public string OpbaringKledingMee
        {
            get { return _opbaringKledingMee; }
            set { _opbaringKledingMee = value; OnPropertyChanged(nameof(OpbaringKledingMee)); }
        }
        public string OpbaringKledingRetour
        {
            get { return _opbaringKledingRetour; }
            set { _opbaringKledingRetour = value; OnPropertyChanged(nameof(OpbaringKledingRetour)); }
        }
        public string OpbaringSieraden
        {
            get { return _opbaringSieraden; }
            set { _opbaringSieraden = value; OnPropertyChanged(nameof(OpbaringSieraden)); }
        }
        public string OpbaringSieradenOmschrijving
        {
            get { return _opbaringSieradenOmschrijving; }
            set { _opbaringSieradenOmschrijving = value; OnPropertyChanged(nameof(OpbaringSieradenOmschrijving)); }
        }
        public string OpbaringSieradenRetour
        {
            get { return _opbaringSieradenRetour; }
            set { _opbaringSieradenRetour = value; OnPropertyChanged(nameof(OpbaringSieradenRetour)); }
        }
        public string OpbaringBezoek
        {
            get { return _opbaringBezoek; }
            set { _opbaringBezoek = value; OnPropertyChanged(nameof(OpbaringBezoek)); }
        }
        public string OpbaringExtraInfo
        {
            get { return _opbaringExtraInfo; }
            set { _opbaringExtraInfo = value; OnPropertyChanged(nameof(OpbaringExtraInfo)); }
        }
        public string Error => string.Empty;
        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (string.IsNullOrEmpty(OpbaringLocatie) ||
                    (OpbaringKistId == Guid.Empty) ||
                    (OpbaringKistLengte == Guid.Empty) ||
                    (string.IsNullOrEmpty(OpbaringVerzorgingJson)) ||
                    (string.IsNullOrEmpty(OpbaringKoeling)) ||
                    (string.IsNullOrEmpty(OpbaringSieraden)) ||
                    (string.IsNullOrEmpty(OpbaringSieradenRetour)))
                {
                    result = "Validatie velden niet correct.";
                }
                return result;
            }
        }
        public bool HasData()
        {
            return !string.IsNullOrEmpty(OpbaringLocatie) &&
                    OpbaringKistId != Guid.Empty &&
                    OpbaringKistLengte != Guid.Empty;
        }
    }
    public class KistenModel : ViewModelBase
    {
        private Guid _id;
        private string? _kistTypeNummer;
        private string? _kistOmschrijving;
        private bool? _isDeleted;
        private string _btnBrush;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string KistTypeNummer
        {
            get { return _kistTypeNummer; }
            set { _kistTypeNummer = value; OnPropertyChanged(nameof(KistTypeNummer)); }
        }
        public string KistOmschrijving
        {
            get { return _kistOmschrijving; }
            set { _kistOmschrijving = value; OnPropertyChanged(nameof(KistOmschrijving)); }
        }
        public bool? IsDeleted
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
    public class KistenLengte : ViewModelBase
    {
        private Guid _id;
        private string? _kistLengte;
        private bool? _isDeleted;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string KistLengte
        {
            get { return _kistLengte; }
            set { _kistLengte = value; OnPropertyChanged(nameof(KistLengte)); }
        }
        public bool? IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                _isDeleted = value; OnPropertyChanged(nameof(IsDeleted));
            }
        }
    }
    public class VerzorgendPersoneel : ViewModelBase
    {
        private Guid _id;
        private string _verzorger;
        private bool _isDeleted;
        private bool _isOpbaren;
        public Guid Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }
        public string VerzorgendPersoon
        {
            get { return _verzorger; }
            set { _verzorger = value; OnPropertyChanged(nameof(VerzorgendPersoon)); }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged(nameof(IsDeleted)); }
        }
        public bool IsOpbaren
        {
            get { return _isOpbaren; }
            set { _isOpbaren = value; OnPropertyChanged(nameof(IsOpbaren)); }
        }

    }
    public class VerzorgingData : ViewModelBase
    {
        private Guid _werknemerId;
        private string _werknemerStartTijd;
        private string _werknemerEindTijd;

        public Guid WerknemerId
        {
            get { return _werknemerId; }
            set { _werknemerId = value; OnPropertyChanged(nameof(WerknemerId)); }
        }
        public string WerknemerStartTijd
        {
            get { return _werknemerStartTijd; }
            set { _werknemerStartTijd = value; OnPropertyChanged(nameof(WerknemerStartTijd)); }
        }
        public string WerknemerEindTijd
        {
            get { return _werknemerEindTijd; }
            set { _werknemerEindTijd = value; OnPropertyChanged(nameof(WerknemerEindTijd)); }
        }
    }
}
