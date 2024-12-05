using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class Globals : ViewModelBase
    {
        public static string _uitvaartCode;
        public static Guid _uitvaartCodeGuid;
        public static string _uitvaarLeider;
        public static string _permissionLevelId;
        public static string _permissionLevelName;
        public static bool _dossierCompleted;
        public static bool _newDossierCreation;
        public static bool _voorregeling;
        public static string UitvaartCode
        {
            get { return _uitvaartCode; }
            set { _uitvaartCode = value; }
        }
        public static Guid UitvaartCodeGuid
        {
            get { return _uitvaartCodeGuid; }
            set
            {
                _uitvaartCodeGuid = value;
            }
        }
        public static string UitvaarLeider
        {
            get { return _uitvaarLeider; }
            set { _uitvaarLeider = value; }
        }
        public static string PermissionLevelId
        {
            get { return _permissionLevelId; }
            set { _permissionLevelId = value; }
        }
        public static string PermissionLevelName
        {
            get { return _permissionLevelName; }
            set { _permissionLevelName = value; }
        }
        public static bool DossierCompleted
        {
            get { return _dossierCompleted; }
            set { _dossierCompleted = value; }
        }
        public static bool NewDossierCreation
        {
            get { return _newDossierCreation; }
            set { _newDossierCreation = value; }
        }
        public static bool Voorregeling
        {
            get { return _voorregeling; }
            set { _voorregeling = value; }
        }

    }
}
