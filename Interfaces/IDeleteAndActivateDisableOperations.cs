using System;

namespace Dossier_Registratie.Interfaces
{
    public interface IDeleteAndActivateDisableOperations
    {
        void CloseDossier(Guid uitvaartId);
        void ReOpenDossier(Guid uitvaartId);
        void ActivateEmployee(Guid employeeId);
        void ActivateKist(Guid kistId);
        void ActivateAsbestemming(Guid asbestemmingId);
        void ActivateVerzekeraar(Guid verzekeraarId);
        void ActivateLeverancier(Guid leverancierId);
        void ActivatePriceComponent(Guid componentId);
        void ActivateSuggestion(Guid suggestionId);
        void ActivateRouwbrief(Guid rouwbriefId);
        void DisableEmployee(Guid employeeId);
        void DisableKist(Guid kistId);
        void DisableAsbestemming(Guid asbestemmingId);
        void DisableVerzekeraar(Guid verzekeraarId);
        void DisableLeverancier(Guid leverancierId);
        void DisablePriceComponent(Guid componentId);
        void DisableRouwbrief(Guid rouwbriefId);
        void SetDocumentInconsistent(Guid DocumentId);
        void SetDocumentDeleted(Guid UitvaartId, string DocumentName);
        void DisableSuggestion(Guid suggestionId);
    }

}
