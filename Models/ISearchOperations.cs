using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Dossier_Registratie.Models
{
    public interface ISearchOperations
    {
        Task<AkteContent> GetAkteContentByUitvaartIdAsync(Guid uitvaartId);
        Task<WekbonnenContent> GetWerkbonInfoByUitvaartIdAsync(Guid uitvaartId);
        Task<bool> SearchBlobLogo(string appType);
        (string PermissionLevelId, string PermissionLevelName) FetchUserCredentials(string windowsUsername);
        string GetWerkbonWerknemer(Guid werknemerId);
        Task<int> SearchKostenbegrotingExistanceAsync(Guid uitvaartIdGuid);
        WerknemersModel SearchEmployee(WerknemersModel werknemerSearch);
        PermissionsModel SelectUserPermission(Guid PersoneelId);
        OverledeneMiscModel GetOverledeneMiscByUitvaartId(Guid uitvaartId);
        IEnumerable<OverledeneSearchSurname> GetUitvaarleiderByUitvaartIdSearch(string uitvaartId);
        OverledeneUitvaartleiderModel GetUitvaarleiderByUitvaartId(string uitvaartId);
        OverledenePersoonsGegevensModel GetPeroonsGegevensByUitvaartId(string UitvaartId);
        OverledeneOverlijdenInfoModel GetOverlijdenInfoByUitvaartId(string UitvaartId);
        OverledeneExtraInfoModel GetExtraInfoByUitvaartId(string UitvaartId);
        OpdrachtgeverPersoonsGegevensModel GetOpdrachtgeverByUitvaartId(string UitvaartId);
        OpdrachtgeverPersoonsGegevensModel GetExtraOpdrachtgeverByUitvaartId(string UitvaartId);
        OverledeneVerzekeringModel GetOverlijdenVerzekeringByUitvaartId(string UitvaartId);
        OverledeneOpbarenModel GetOverlijdenOpbarenInfoByUitvaartId(string UitvaartId);
        OverledeneUitvaartModel GetOverlijdenUitvaartInfoByUitvaartId(string UitvaartId);
        OverledeneAsbestemmingModel GetOverlijdenAsbestemmingInfoByUitvaartId(string UitvaartId);
        IEnumerable<OverledeneBijlagesModel> GetOverlijdenBijlagesByUitvaartId(string uitvaartId);
        IEnumerable<OverledeneBijlagesModel> GetTerugmeldingenByUitvaartId(Guid uitvaartId);
        ObservableCollection<OverledeneSteenhouwerijModel> GetOverlijdenSteenhouwerij();
        ObservableCollection<OverledeneSteenhouwerijModel> GetOverlijdenSteenhouwerijByEmployee(Guid EmployeeId);
        OverledeneSteenhouwerijModel GetOverlijdenSteenhouwerijByUitvaartId(string uitvaartId);
        ObservableCollection<OverledeneBloemenModel> GetOverlijdenBloemen();
        ObservableCollection<OverledeneBloemenModel> GetOverlijdenBloemenByEmployee(Guid EmployeeId);
        OverledeneBloemenModel GetOverlijdenBloemenByUitvaartId(string uitvaartId);
        OverledeneWerkbonUitvaart GetOverlijdenWerkbonnenByUitvaartId(string UitvaartId);
        ObservableCollection<ObservableCollection<WerkbonnenData>> GetOverlijdenWerkbonnen();
        ObservableCollection<OverledeneUrnSieradenModel> GetOverlijdenUrnSieraden();
        ObservableCollection<OverledeneUrnSieradenModel> GetOverlijdenUrnSieradenByEmployee(Guid EmployeeId);
        OverledeneUrnSieradenModel GetOverlijdenUrnSieradenByUitvaartId(string uitvaartId);
        FactuurModel GetPolisInfoByUitvaartId(string uitvaartId);
        FactuurModel GetOverlijdenKostenbegrotingByUitvaartId(string UitvaartId);
        Task<string> GetOverlijdenKostenbegrotingAsync(Guid UitvaartId);
        GenerateFactuur GetGenerateFactuurDataByUitvaartId(Guid UitvaartId);
        IEnumerable<OverledeneSearchSurname> GetUitvaartleiderBySurnameOverledene(string overledeneAchternaam, DateTime overledeneGeboortedatum);
    }

}
