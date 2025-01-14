using System;
using System.Threading.Tasks;
using Dossier_Registratie.Models;

namespace Dossier_Registratie.Interfaces
{
    public interface ICreateOperations
    {
        Task InsertBlobLogo(string imageName, string imageType, byte[] imageData, string appType);
        Task InsertKostenbegrotingAsync(string kostenbegrotingUrl, string kostenbegrotingData, DateTime creationDate, Guid uitvaartId, Guid id, Guid verzekeraarId);
        void EmployeeCreate(WerknemersModel werknemerCreate);
        Task InsertDocumentInfoAsync(OverledeneBijlagesModel documentInfo);
        void InsertDossier(OverledeneBijlagesModel finishDossier);
        void AddUitvaartleider(OverledeneUitvaartleiderModel uitvaarLeiderModel);
        void AddMiscUitvaart(OverledeneMiscModel uitvaartMisc);
        void AddPersoonsGegevens(OverledenePersoonsGegevensModel GetDocumentOverdrachtInfo);
        void AddOverlijdenInfo(OverledeneOverlijdenInfoModel overlijdenInfoModel);
        void AddOverlijdenExtraInfo(OverledeneExtraInfoModel overledeneExtraInfoModel);
        void AddOpdrachtgeverPersoonsGegevens(OpdrachtgeverPersoonsGegevensModel opdrachtgeverPersoonsGegevensModel);
        void AddOpdrachtgeverExtraPersoonsGegevens(OpdrachtgeverPersoonsGegevensModel opdrachtgeverPersoonsGegevensModel);
        void AddVerzekering(OverledeneVerzekeringModel overledeneVerzekeringModel);
        void AddOpbaren(OverledeneOpbarenModel opbarenModel);
        void AddUitvaart(OverledeneUitvaartModel overledeneUitvaartModel);
        void AddAsbestemming(OverledeneAsbestemmingModel asbestemmingModel);
        string AddBijlages(OverledeneBijlagesModel bijlagesModel);
        void AddSteenhouwerij(OverledeneSteenhouwerijModel overledeneSteenhouwerijModel);
        void AddBloemen(OverledeneBloemenModel overledeneBloemenModel);
        void AddWerkbonnen(OverledeneWerkbonUitvaart overledeneWerkbonUitvaart);
        void AddUrnSieraden(OverledeneUrnSieradenModel overledeneUrnSieradenModel);
        void AddFactuur(FactuurModel overledeneKostenbegrotingModel);
        void AddKlanttevredenheid(Klanttevredenheid klanttevredenheid);
        void CreateWindowsUser(Guid Id, Guid PersoneelId, string WindowsUser);
        void CreateUserPermission(Guid PersoneelId, Guid rechtenId);
        void CreatePriceComponent(KostenbegrotingModel priceComponent);
        void CreateLeverancier(LeveranciersModel leveranciers);
        void CreateRouwbrief(OverledeneRouwbrieven rouwbrief);
        void KistCreate(KistenModel kistCreate);
        void AsbestemmingCreate(ConfigurationAsbestemmingModel asbestemmingCreate);
        void VerzekeringCreate(VerzekeraarsModel verzkeringCreate);
        void CreateSuggestion(SuggestionModel suggestionCreate);
    }
}
