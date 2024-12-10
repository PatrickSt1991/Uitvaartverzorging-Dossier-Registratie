﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Dossier_Registratie.Models
{
    public interface IMiscellaneousAndDocumentOperations
    {
        Task<ObservableCollection<NotificatieOverzichtModel>> NotificationDeceasedAfterYearPassedAsync();
        (Guid herkomstId, string herkomstName, bool herkomstLogo) GetHerkomstByUitvaartId(Guid uitvaartId);
        bool UitvaarnummerExists(string uitvaartnummer);
        bool UitvaartPersoonsgegevensExists(Guid UitvaartId);
        bool UitvaartOverlijdenInfoExists(Guid UitvaartId);
        bool UitvaartExtraInfoExists(Guid UitvaartId);
        bool UitvaartOpdrachtgeverPersoonsgegevensExists(Guid UitvaartId);
        bool UitvaartVerzekeringExists(Guid UitvaartId);
        bool UitvaartOpbarenExists(Guid UitvaartId);
        bool UitvaarInfoExists(Guid UitvaartId);
        bool UitvaarInfoMiscExists(Guid UitvaartId);
        bool UitvaarAsbestemmingExists(Guid UitvaartId);
        bool UitvaarFactuurExists(Guid UitvaartId);
        bool UitvaarKlanttevredenheidExists(Guid UitvaartId);
        bool UitvaarKWerkbonExists(Guid UitvaartId);
        bool UitvaarKUrnSieradenExists(Guid UitvaartId);
        bool UitvaarBloemenExists(Guid UitvaartId);
        bool UitvaarSteenhouwerijExists(Guid UitvaartId);
        Task<FactuurInfoCrematie> GetFactuurInfo(Guid HerkomstId);
        (byte[] DocumentData, string DocumentType) GetLogoBlob(string AppType);
        IEnumerable<SuggestionModel> GetSuggestions();
        SuggestionModel GetSuggestionBeheer(Guid suggestionId);
        ObservableCollection<SuggestionModel> GetSuggestionsBeheer();
        ActiveAccountModel GetActiveUsers();
        ObservableCollection<VerzekeraarsModel> GetVerzekeraars();
        ObservableCollection<OverledeneRouwbrieven> GetAdvertenties();
        ObservableCollection<VerzekeraarsModel> GetHerkomst();
        VerzekeraarsModel GetVerzekeraarsById(Guid verzekeraarId);
        VerzekeraarsModel GetHerkomstById(Guid herkomstId);
        ObservableCollection<UitvaartLeiderModel> GetUitvaartleiders();
        ObservableCollection<WerknemersModel> GetWerknemers();
        ObservableCollection<AgendaModel> GetAgenda();
        ObservableCollection<UitvaartOverzichtModel> GetUitvaartOverzicht();
        WerknemersModel GetWerknemer(Guid werknemerId);
        ObservableCollection<KistenModel> GetKisten();
        ObservableCollection<ConfigurationAsbestemmingModel> GetAsbestemmingen();
        KistenModel GetKist(Guid kistId);
        ConfigurationAsbestemmingModel GetAsbestemming(Guid asbestemmingId);
        Guid GetUitvaartGuid(string Uitvaartnummer);
        ObservableCollection<KistenLengte> GetKistenLengte();
        ObservableCollection<VerzorgendPersoneel> GetVerzorgers();
        ObservableCollection<WerkbonPersoneel> GetWerkbonPersoneel();
        ObservableCollection<LeveranciersModel> GetLeveranciers();
        ObservableCollection<OverledeneRouwbrieven> GetRouwbrieven();
        ObservableCollection<FactuurModel> GetAllKostenbegrotingen();
        Task<KostenbegrotingInfoModel> GetKostenbegrotingPersonaliaAsync(Guid uitvaartIdGuid);
        OverledeneSteenhouwerijModel GetSteenUitbetaling(Guid UitvaartIdGuid);
        OverledeneUrnSieradenModel GetUrnSieradenUitbetaling(Guid UitvaartIdGuid);
        OverledeneBloemenModel GetBloemenUitbetaling(Guid UitvaartIdGuid);
        IEnumerable<OverledeneBijlagesModel> GetAktesVanCessieByUitvaatId(string uitvaartId);
        ObservableCollection<GeneratedKostenbegrotingModel> GetPriceComponentsId(Guid verzekeraarId, bool pakketVerzekering);
        ObservableCollection<GeneratedKostenbegrotingModel> GetPriceComponents(string verzekeringMaatschapij, bool pakketVerzekering);
        ObservableCollection<KostenbegrotingModel> GetAllPriceComponentsBeheer();
        KostenbegrotingModel GetSelectedPriceComponentsBeheer(Guid componentId);
        ObservableCollection<RapportagesKisten> GetRapportagesKisten(string startNummer, string endNummer);
        ObservableCollection<RapportagesVerzekering> GetRapportagesVerzekering(string startNummer, string endNummer);
        ObservableCollection<RapportagesVerzekering> GetRapportagesVerzekeringWoonplaats(string startNummer, string endNummer);
        ObservableCollection<RapportagesUitvaartleider> GetRapportagesUitvaartleider(string startNummer, string endNummer);
        ObservableCollection<Volgautos> GetVolgautos(string startNummer, string endNummer);
        ObservableCollection<PeriodeLijst> GetPeriode(string startNummer, string endNummer);
        Klanttevredenheid GetScore(Guid uitvaartId);
        Filter GetFilter();
        string GetLeverancier(Guid leverancierId);
        LeveranciersModel GetLeverancierBeheer(Guid leverancierId);
        OverledeneRouwbrieven GetRouwbriefBeheer(Guid rouwbrievenId);
        OverledeneBijlagesModel GetFinishedDossier(Guid UitvaartId);
        OverledeneBijlagesModel GetVerlofDossier(Guid UitvaartId);
        ObservableCollection<WindowsAccount> GetWerknemerPermissions(Guid werknemerId);
        ObservableCollection<PermissionsModel> GetPermissions();
        ObservableCollection<RapportageKlantWerknemerScores> GetAllEmployeeScore();
        ObservableCollection<RapportageKlantWerknemerScores> GetEmployeeScore(Guid employeeId);
        Task<OverledeneBijlagesModel> GetDocumentInformationAsync(Guid UitvaartId, string DocumentName);
        Task<List<OverledeneBijlagesModel>> GetDocumentsInformationAsync(Guid UitvaartId, string DocumentName);
        Task<KoffieKamerDocument> GetKoffieKamerInfoAsync(Guid UitvaartId);
        Task<DocumentDocument> GetDocumentDocumentInfoAsync(Guid UitvaartId);
        Task<DienstDocument> GetDienstInfoAsync(Guid UitvaartId);
        Task<ChecklistDocument> GetDocumentChecklistInfoAsync(Guid UitvaartId);
        Task<OverdrachtDocument> GetDocumentOverdrachtInfoAsync(Guid UitvaartId);
        Task<BloemenDocument> GetDocumentBloemenInfoAsync(Guid UitvaartId);
        Task<BezittingenDocument> GetDocumentBezittingInfoAsync(Guid UitvaartId);
        Task<CrematieDocument> GetDocumentCrematieInfoAsync(Guid UitvaartId);
        Task<BegrafenisDocument> GetDocumentBegrafenisInfoAsync(Guid UitvaartId);
        Task<TerugmeldingDocument> GetDocumentTerugmeldingInfoAsync(Guid UitvaartId);
        Task<TevredenheidDocument> GetDocumentTevredenheidsInfoAsync(Guid UitvaartId);
        Task<AangifteDocument> GetDocumentAangifteInfoAsync(Guid UitvaartId);
        string GetUitvaartType(Guid UitvaartId);
        int CheckLocationExistance(SuggestionModel newSuggestion);
    }

}
