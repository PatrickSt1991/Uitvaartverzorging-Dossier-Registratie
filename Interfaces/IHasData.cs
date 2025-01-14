using System;

namespace Dossier_Registratie.Interfaces
{
    public interface IHasData
    {
        bool HasData();
        Guid DocumentId { get; set; }
        Guid UitvaartId { get; set; }
        string Dossiernummer { get; set; }
        string DestinationFile { get; set; }
        string DocumentName { get; set; }
        string FileType { get; set; }

    }
}
