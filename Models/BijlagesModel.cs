using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class BijlageContentModel : ViewModelBase
    {
        private string _tevredenheidContent = "Genereren";
        private string _bezittingenContent = "Genereren";
        private string _aanvraagDienstContent = "Genereren";
        private string _documentContent = "Genereren";
        private string _checklistContent = "Genereren";
        private string _overdrachtContent = "Genereren";
        private string _opdrachtBegrafenisContent = "Genereren";
        private string _opdrachtCrematieContent = "Genereren";
        private string _terugmeldingContent = "Genereren";
        private string _koffiekamerContent = "Genereren";
        private string _aangifteContent = "Genereren";

        public string TevredenheidContent
        {
            get { return _tevredenheidContent; }
            set { _tevredenheidContent = value; OnPropertyChanged(nameof(TevredenheidContent)); }
        }
        public string BezittingenContent
        {
            get { return _bezittingenContent; }
            set { _bezittingenContent = value; OnPropertyChanged(nameof(BezittingenContent)); }
        }
        public string AanvraagDienstContent
        {
            get { return _aanvraagDienstContent; }
            set { _aanvraagDienstContent = value; OnPropertyChanged(nameof(AanvraagDienstContent)); }
        }
        public string DocumentContent
        {
            get { return _documentContent; }
            set { _documentContent = value; OnPropertyChanged(nameof(DocumentContent)); }
        }
        public string ChecklistContent
        {
            get { return _checklistContent; }
            set { _checklistContent = value; OnPropertyChanged(nameof(ChecklistContent)); }
        }
        public string OverdrachtContent
        {
            get { return _overdrachtContent; }
            set { _overdrachtContent = value; OnPropertyChanged(nameof(OverdrachtContent)); }
        }
        public string OpdrachtBegrafenisContent
        {
            get { return _opdrachtBegrafenisContent; }
            set { _opdrachtBegrafenisContent = value; OnPropertyChanged(nameof(OpdrachtBegrafenisContent)); }
        }
        public string OpdrachtCrematieContent
        {
            get { return _opdrachtCrematieContent; }
            set { _opdrachtCrematieContent = value; OnPropertyChanged(nameof(OpdrachtCrematieContent)); }
        }
        public string TerugmeldingContent
        {
            get { return _terugmeldingContent; }
            set { _terugmeldingContent = value; OnPropertyChanged(nameof(TerugmeldingContent)); }
        }
        public string KoffiekamerContent
        {
            get { return _koffiekamerContent; }
            set { _koffiekamerContent = value; OnPropertyChanged(nameof(KoffiekamerContent)); }
        }
        public string AangifteContent
        {
            get { return _aangifteContent; }
            set { _aangifteContent = value; OnPropertyChanged(nameof(AangifteContent)); }
        }
    }
    public class BijlageButtonModel : ViewModelBase
    {
        private bool _tevredenheidButton = true;
        private bool _bezittingenButton = true;
        private bool _aanvraagDienstButton = true;
        private bool _documentButton = true;
        private bool _checklistButton = true;
        private bool _overdrachtButton = true;
        private bool _opdrachtBegrafenisButton = true;
        private bool _opdrachtCrematieButton = true;
        private bool _terugmeldingButton = true;
        private bool _koffiekamerButton = true;
        private bool _aangifteButton = true;

        public bool TevredenheidButton
        {
            get { return _tevredenheidButton; }
            set { _tevredenheidButton = value; OnPropertyChanged(nameof(TevredenheidButton)); }
        }
        public bool BezittingenButton
        {
            get { return _bezittingenButton; }
            set { _bezittingenButton = value; OnPropertyChanged(nameof(BezittingenButton)); }
        }
        public bool AanvraagDienstButton
        {
            get { return _aanvraagDienstButton; }
            set { _aanvraagDienstButton = value; OnPropertyChanged(nameof(AanvraagDienstButton)); }
        }
        public bool DocumentButton
        {
            get { return _documentButton; }
            set { _documentButton = value; OnPropertyChanged(nameof(DocumentButton)); }
        }
        public bool ChecklistButton
        {
            get { return _checklistButton; }
            set { _checklistButton = value; OnPropertyChanged(nameof(ChecklistButton)); }
        }
        public bool OverdrachtButton
        {
            get { return _overdrachtButton; }
            set { _overdrachtButton = value; OnPropertyChanged(nameof(OverdrachtButton)); }
        }
        public bool OpdrachtBegrafenisButton
        {
            get { return _opdrachtBegrafenisButton; }
            set { _opdrachtBegrafenisButton = value; OnPropertyChanged(nameof(OpdrachtBegrafenisButton)); }
        }
        public bool OpdrachtCrematieButton
        {
            get { return _opdrachtCrematieButton; }
            set { _opdrachtCrematieButton = value; OnPropertyChanged(nameof(OpdrachtCrematieButton)); }
        }
        public bool TerugmeldingButton
        {
            get { return _terugmeldingButton; }
            set { _terugmeldingButton = value; OnPropertyChanged(nameof(TerugmeldingButton)); }
        }
        public bool KoffiekamerButton
        {
            get { return _koffiekamerButton; }
            set { _koffiekamerButton = value; OnPropertyChanged(nameof(KoffiekamerButton)); }
        }
        public bool AangifteButton
        {
            get { return _aangifteButton; }
            set { _aangifteButton = value; OnPropertyChanged(nameof(AangifteButton)); }
        }
    }
    public class BijlageTagModel : ViewModelBase
    {
        private string _tevredenheidTag = string.Empty;
        private string _bezittingenTag = string.Empty;
        private string _aanvraagDienstTag = string.Empty;
        private string _documentTag = string.Empty;
        private string _checklistTag = string.Empty;
        private string _overdrachtTag = string.Empty;
        private string _opdrachtBegrafenisTag = string.Empty;
        private string _opdrachtCrematieTag = string.Empty;
        private string _terugmeldingTag = string.Empty;
        private string _koffiekamerTag = string.Empty;
        private string _aangifteTag = string.Empty;

        public string TevredenheidTag
        {
            get { return _tevredenheidTag; }
            set { _tevredenheidTag = value; OnPropertyChanged(nameof(TevredenheidTag)); }
        }
        public string BezittingenTag
        {
            get { return _bezittingenTag; }
            set { _bezittingenTag = value; OnPropertyChanged(nameof(BezittingenTag)); }
        }
        public string AanvraagDienstTag
        {
            get { return _aanvraagDienstTag; }
            set { _aanvraagDienstTag = value; OnPropertyChanged(nameof(AanvraagDienstTag)); }
        }
        public string DocumentTag
        {
            get { return _documentTag; }
            set { _documentTag = value; OnPropertyChanged(nameof(DocumentTag)); }
        }
        public string ChecklistTag
        {
            get { return _checklistTag; }
            set { _checklistTag = value; OnPropertyChanged(nameof(ChecklistTag)); }
        }
        public string OverdrachtTag
        {
            get { return _overdrachtTag; }
            set { _overdrachtTag = value; OnPropertyChanged(nameof(OverdrachtTag)); }
        }
        public string OpdrachtBegrafenisTag
        {
            get { return _opdrachtBegrafenisTag; }
            set { _opdrachtBegrafenisTag = value; OnPropertyChanged(nameof(OpdrachtBegrafenisTag)); }
        }
        public string OpdrachtCrematieTag
        {
            get { return _opdrachtCrematieTag; }
            set { _opdrachtCrematieTag = value; OnPropertyChanged(nameof(OpdrachtCrematieTag)); }
        }
        public string TerugmeldingTag
        {
            get { return _terugmeldingTag; }
            set { _terugmeldingTag = value; OnPropertyChanged(nameof(TerugmeldingTag)); }
        }
        public string KoffiekamerTag
        {
            get { return _koffiekamerTag; }
            set { _koffiekamerTag = value; OnPropertyChanged(nameof(KoffiekamerTag)); }
        }
        public string AangifteTag
        {
            get { return _aangifteTag; }
            set { _aangifteTag = value; OnPropertyChanged(nameof(AangifteTag)); }
        }
    }
    public class OverledeneBijlagesModel : ViewModelBase
    {
        private Guid _bijlageId;
        private Guid _uitvaartId;
        private string? _documentName;
        private string? _documentType;
        private string? _documentUrl;
        private string? _documentHash;
        private bool _documentInconsistent;
        private bool _isDeleted;
        private bool _isModified = false;

        public Guid BijlageId
        {
            get { return _bijlageId; }
            set { _bijlageId = value; OnPropertyChanged(nameof(BijlageId)); }
        }
        public Guid UitvaartId
        {
            get { return _uitvaartId; }
            set { _uitvaartId = value; OnPropertyChanged(nameof(UitvaartId)); }
        }
        public string DocumentName
        {
            get { return _documentName; }
            set { _documentName = value; OnPropertyChanged(nameof(DocumentName)); }
        }
        public string DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; OnPropertyChanged(nameof(DocumentType)); }
        }
        public string DocumentUrl
        {
            get { return _documentUrl; }
            set { _documentUrl = value; OnPropertyChanged(nameof(DocumentUrl)); }
        }
        public string DocumentHash
        {
            get { return _documentHash; }
            set { _documentHash = value; OnPropertyChanged(nameof(DocumentHash)); }
        }
        public bool DocumentInconsistent
        {
            get { return _documentInconsistent; }
            set { _documentInconsistent = value; OnPropertyChanged(nameof(DocumentInconsistent)); }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged(nameof(IsDeleted)); }
        }
        public bool IsModified
        {
            get { return _isModified; }
            set { _isModified = value; OnPropertyChanged(nameof(IsModified)); }
        }
    }
}
