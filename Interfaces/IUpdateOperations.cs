﻿using Dossier_Registratie.Models;
using System;
using System.Threading.Tasks;

namespace Dossier_Registratie.Interfaces
{
    public interface IUpdateOperations
    {
        Task UpdateNotification(Guid uitvaartId);
        Task UpdateBlobLogo(string imageName, string imageType, byte[] imageData, string appType);
        void SetDocumentInconsistent(Guid DocumentId);
        void UpdateLeverancier(LeveranciersModel leverancier);
        void UpdateRouwbrief(OverledeneRouwbrieven rouwbrief);
        void EditRechten(Guid employeeId, Guid rechtenId);
        void EditUitvaartleider(OverledeneUitvaartleiderModel uitvaartleiderModel);
        void EditMiscUitvaart(OverledeneMiscModel uitvaartMisc);
        void EditPersoonsGegevens(OverledenePersoonsGegevensModel persoonsGegevensModel);
        void EditOverlijdenInfo(OverledeneOverlijdenInfoModel overlijdenInfoModel);
        void EditOverlijdenExtraInfo(OverledeneExtraInfoModel overledeneExtraInfoModel);
        void EditOpdrachtgeverPersoonsGegevens(OpdrachtgeverPersoonsGegevensModel opdrachtgeverPersoonsGegevensModel);
        void EditOpdrachtgeverExtraPersoonsGegevens(OpdrachtgeverPersoonsGegevensModel opdrachtgeverPersoonsGegevensModel);
        void EditVerzekering(OverledeneVerzekeringModel overledeneVerzekeringModel);
        void EditOpbaren(OverledeneOpbarenModel overledeneOpbarenModel);
        void EditUitvaart(OverledeneUitvaartModel overledeneUitvaartModel);
        void EditAsbestemming(OverledeneAsbestemmingModel asbestemmingModel);
        string EditBijlages(OverledeneBijlagesModel bijlagesModel);
        void EditSteenhouwerij(OverledeneSteenhouwerijModel overledeneSteenhouwerijModel);
        void EditBloemen(OverledeneBloemenModel overledeneBloemenModel);
        void EditWerkbonnen(OverledeneWerkbonUitvaart overledeneWerkbonUitvaart);
        void EditUrnSieraden(OverledeneUrnSieradenModel overledeneUrnSieradenModel);
        void EditFactuur(FactuurModel overledeneFactuurModel);
        void EditKlanttevredenheid(Klanttevredenheid klanttevredenheid);
        void EmployeeUpdate(WerknemersModel werknemerUpdate);
        void KistUpdate(KistenModel kistUpdate);
        void AsbestemmingUpdate(ConfigurationAsbestemmingModel asbestemmingUpdate);
        void VerzekeringUpdate(VerzekeraarsModel verzekeringUpdate);
        void UpdatePriceComponent(KostenbegrotingModel priceComponent);
        void UpdateSteenhouwerijBetaling(OverledeneSteenhouwerijModel steenInfo);
        void UpdateBloemenBetaling(OverledeneBloemenModel bloemInfo);
        void UpdateUrnSieradenBetaling(OverledeneUrnSieradenModel urnSieradenInfo);
        void UpdateSuggestion(SuggestionModel suggestionCreate);
        Task UpdateDocumentInfoAsync(OverledeneBijlagesModel documentInfo);
        Task UpdateKostenbegrotingAsync(string kostenbegrotingUrl, string kostenbegrotingData, DateTime creationDate, Guid uitvaartId, Guid verzekeraarId, decimal korting);
    }
}
