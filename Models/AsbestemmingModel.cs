using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class ConfigurationAsbestemmingModel : ViewModelBase
    {
        private Guid _asbestemmingId;
        private string _asbestemmingOmschrijving;
        private bool _isDeleted;
        private string _btnBrush;

        public Guid AsbestemmingId
        {
            get { return _asbestemmingId; }
            set { _asbestemmingId = value; OnPropertyChanged(nameof(AsbestemmingId)); }
        }
        public string AsbestemmingOmschrijving
        {
            get { return _asbestemmingOmschrijving; }
            set { _asbestemmingOmschrijving = value; OnPropertyChanged(nameof(AsbestemmingOmschrijving)); }
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
    public class Klanttevredenheid : ViewModelBase
    {
        private Guid _id;
        private Guid _uitvaartId;
        private int _cijferScore;

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value; OnPropertyChanged(nameof(Id));
            }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set
            {
                _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId));
            }
        }
        public int CijferScore
        {
            get { return _cijferScore; }
            set { _cijferScore = value; OnPropertyChanged(nameof(CijferScore)); }
        }
    }
    public class OverledeneAsbestemmingModel : ViewModelBase
    {
        private Guid _asbestemmingId;
        private Guid _uitvaartId;
        private string? _asbestemming;
        private string? _typeGraf;
        private string? _bestaandGraf;
        private string? _zandKelderGraf;
        private string? _grafMonument;

        public bool HasData()
        {
            return !string.IsNullOrEmpty(Asbestemming) ||
                   !string.IsNullOrEmpty(TypeGraf) ||
                   !string.IsNullOrEmpty(BestaandGraf) ||
                   !string.IsNullOrEmpty(ZandKelderGraf) ||
                   !string.IsNullOrEmpty(GrafMonument);
        }

        public Guid AsbestemmingId
        {
            get { return _asbestemmingId; }
            set { _asbestemmingId = value; OnPropertyChanged(nameof(AsbestemmingId)); }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public string Asbestemming
        {
            get { return _asbestemming; }
            set { _asbestemming = value; OnPropertyChanged(nameof(Asbestemming)); }
        }
        public string TypeGraf
        {
            get { return _typeGraf; }
            set { _typeGraf = value; OnPropertyChanged(nameof(TypeGraf)); }
        }
        public string BestaandGraf
        {
            get { return _bestaandGraf; }
            set { _bestaandGraf = value; OnPropertyChanged(nameof(BestaandGraf)); }
        }
        public string ZandKelderGraf
        {
            get { return _zandKelderGraf; }
            set { _zandKelderGraf = value; OnPropertyChanged(nameof(ZandKelderGraf)); }
        }
        public string GrafMonument
        {
            get { return _grafMonument; }
            set { _grafMonument = value; OnPropertyChanged(nameof(GrafMonument)); }
        }
    }
}
