using System;

namespace Dossier_Registratie.Interfaces
{
    public interface IGenericDocument
    {
        Guid UitvaartId { get; }
        Guid DocumentId { get; }
        bool Updated { get; }
        string DestinationFile { get; }
        string DocumentName { get; }
        string FileType { get; }
    }
}
