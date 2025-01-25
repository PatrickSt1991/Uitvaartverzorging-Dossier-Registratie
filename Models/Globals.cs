using Dossier_Registratie.ViewModels;
using System;

namespace Dossier_Registratie.Models
{
    public class Globals : ViewModelBase
    {
        public static string? UitvaartCode { get; set; }
        public static Guid UitvaartCodeGuid { get; set; }
        public static string? UitvaarLeider { get; set; }
        public static string? PermissionLevelId { get; set; }
        public static string? PermissionLevelName { get; set; }
        public static bool DossierCompleted { get; set; }
        public static bool NewDossierCreation { get; set; }
        public static bool Voorregeling { get; set; }
        public static string? UitvaartType { get; set; }
    }
}
