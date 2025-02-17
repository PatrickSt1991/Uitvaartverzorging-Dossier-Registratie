﻿using Dossier_Registratie.Models;
using Dossier_Registratie.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using Dossier_Registratie.Helper;

namespace Dossier_Registratie.Repositories
{
    public class CreateOperations : RepositoryBase, ICreateOperations
    {
        public async Task InsertBlobLogo(string imageName, string imageType, byte[] imageData, string appType)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = "INSERT INTO ConfigurationBlob (DocumentID, DocumentName, DocumentType, DocumentData, UploadDate, AppType) " +
                                      "VALUES (@DocumentID, @DocumentName, @DocumentType, @DocumentData, GETDATE(), @AppType)";
                command.Parameters.AddWithValue("@DocumentID", Guid.NewGuid());
                command.Parameters.AddWithValue("@DocumentName", imageName);
                command.Parameters.AddWithValue("@DocumentType", imageType);
                command.Parameters.AddWithValue("@DocumentData", imageData);
                command.Parameters.AddWithValue("@AppType", appType);

                await command.ExecuteNonQueryAsync();
            }

        }
        public async Task InsertKostenbegrotingAsync(string kostenbegrotingUrl, string kostenbegrotingData, DateTime creationDate, Guid uitvaartId, Guid id, Guid verzekeraarId, decimal korting)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneFacturen] ([Id],[uitvaartId],[kostenbegrotingUrl],[kostenbegrotingJson],[kostenbegrotingCreationDate],[kostenbegrotingCreated],[kostenbegrotingVerzekeraar], [Korting]) " +
                                        "VALUES (@id, @uitvaartId, @kostenbegrotingUrl, @kostenbegrotingData, @kostenbegrotingCreationDate, 1, @verzekeraarId, @korting)";
                command.Parameters.AddWithValue("@kostenbegrotingUrl", kostenbegrotingUrl);
                command.Parameters.AddWithValue("@kostenbegrotingData", kostenbegrotingData);
                command.Parameters.AddWithValue("@kostenbegrotingCreationDate", creationDate);
                command.Parameters.AddWithValue("@verzekeraarId", verzekeraarId);
                command.Parameters.AddWithValue("@korting", korting);
                command.Parameters.AddWithValue("@uitvaartId", uitvaartId);
                command.Parameters.AddWithValue("@id", id);

                await command.ExecuteNonQueryAsync();
            }
        }
        public void EmployeeCreate(WerknemersModel werknemerCreate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO ConfigurationPersoneel ([Id],[Initialen],[Voornaam],[Roepnaam],[Tussenvoegsel]," +
                                        "[Achternaam],[Geboorteplaats],[Geboortedatum],[Email],[isDeleted],[isUitvaartverzorger],[isDrager]," +
                                        "[isChauffeur],[isOpbaren], [Mobiel]) VALUES (@Id, @initialen, @voornaam, @roepnaam, @tussenvoegsel, @achternaam, @geboorteplaats, " +
                                        "@geboortedatum, @email, '0', @uitvaartverzorger, @isdrager, @ischauffeur, @isopbaren, @mobiel)";
                command.Parameters.AddWithValue("@Id", werknemerCreate.Id);
                command.Parameters.AddWithValue("@initialen", string.IsNullOrEmpty(werknemerCreate.Initialen) ? DBNull.Value : werknemerCreate.Initialen);
                command.Parameters.AddWithValue("@voornaam", string.IsNullOrEmpty(werknemerCreate.Voornaam) ? DBNull.Value : werknemerCreate.Voornaam);
                command.Parameters.AddWithValue("@roepnaam", string.IsNullOrEmpty(werknemerCreate.Roepnaam) ? DBNull.Value : werknemerCreate.Roepnaam);
                command.Parameters.AddWithValue("@tussenvoegsel", string.IsNullOrEmpty(werknemerCreate.Tussenvoegsel) ? DBNull.Value : werknemerCreate.Tussenvoegsel);
                command.Parameters.AddWithValue("@achternaam", string.IsNullOrEmpty(werknemerCreate.Achternaam) ? DBNull.Value : werknemerCreate.Achternaam);
                command.Parameters.AddWithValue("@geboorteplaats", string.IsNullOrEmpty(werknemerCreate.Geboorteplaats) ? DBNull.Value : werknemerCreate.Geboorteplaats);
                command.Parameters.AddWithValue("@geboortedatum", werknemerCreate.Geboortedatum);
                command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(werknemerCreate.Email) ? DBNull.Value : werknemerCreate.Email);
                command.Parameters.AddWithValue("@mobiel", string.IsNullOrEmpty(werknemerCreate.Mobiel) ? DBNull.Value : werknemerCreate.Mobiel);
                command.Parameters.AddWithValue("@uitvaartverzorger", werknemerCreate.IsUitvaartverzorger);
                command.Parameters.AddWithValue("@isdrager", werknemerCreate.IsDrager);
                command.Parameters.AddWithValue("@ischauffeur", werknemerCreate.IsChauffeur);
                command.Parameters.AddWithValue("@isopbaren", werknemerCreate.IsOpbaren);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("EmployeeCreateFailed");
                }
            }
        }
        public async Task InsertDocumentInfoAsync(OverledeneBijlagesModel documentInfo)
        {
            string fileNameWithoutExtension = documentInfo.DocumentName;

            if (documentInfo.DocumentName.Contains('_'))
                fileNameWithoutExtension = Path.GetFileNameWithoutExtension(documentInfo.DocumentName).Split('_')[1];

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneBijlages] ([BijlageId],[UitvaartId],[DocumentName],[DocumentType],[DocumentURL],[DocumentHash],[DocumentInconsistent],[isDeleted]) " +
                                      "VALUES (@bijlageId, @uitvaartId, @documentName, @documentType, @documentUrl, @documentHash, @documentInconsistent, @isDeleted)";
                command.Parameters.AddWithValue("@bijlageId", documentInfo.BijlageId);
                command.Parameters.AddWithValue("@UitvaartId", documentInfo.UitvaartId);
                command.Parameters.AddWithValue("@documentName", fileNameWithoutExtension);
                command.Parameters.AddWithValue("@documentType", documentInfo.DocumentType);
                command.Parameters.AddWithValue("@documentUrl", documentInfo.DocumentUrl);
                command.Parameters.AddWithValue("@documentHash", documentInfo.DocumentHash);
                command.Parameters.AddWithValue("@documentInconsistent", documentInfo.DocumentInconsistent);
                command.Parameters.AddWithValue("@isDeleted", documentInfo.IsDeleted);

                var rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException("InsertDocumentInfoFailed");
                }
            }
        }
        public void InsertDossier(OverledeneBijlagesModel finishDossier)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneBijlages] (BijlageId, UitvaartId, DocumentName, DocumentType, DocumentURL, DocumentHash, DocumentInconsistent, isDeleted) " +
                                        "VALUES (@BijlageId, @UivaartId, @DocumentName, @DocumenType, @DocumentURL, @DocumentHash, @DocumentInconsistent, @DocumentIsDeleted)";
                command.Parameters.AddWithValue("@BijlageId", finishDossier.BijlageId);
                command.Parameters.AddWithValue("@UivaartId", finishDossier.UitvaartId);
                command.Parameters.AddWithValue("@DocumentName", finishDossier.DocumentName);
                command.Parameters.AddWithValue("@DocumenType", finishDossier.DocumentType);
                command.Parameters.AddWithValue("@DocumentURL", finishDossier.DocumentUrl);
                command.Parameters.AddWithValue("@DocumentHash", finishDossier.DocumentHash);
                command.Parameters.AddWithValue("@DocumentInconsistent", finishDossier.DocumentInconsistent);
                command.Parameters.AddWithValue("@DocumentIsDeleted", finishDossier.IsDeleted);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertFinishDossierFailed");
                }
            }
        }
        public void AddUitvaartleider(OverledeneUitvaartleiderModel uitvaarLeiderModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneUitvaartleider] ([UitvaartId], [PersoneelId], [Uitvaartnummer]) VALUES (@uitvaartId,@personeelId,@uitvaartNummer)";
                command.Parameters.AddWithValue("@uitvaartId", uitvaarLeiderModel.UitvaartId);
                command.Parameters.AddWithValue("@personeelId", uitvaarLeiderModel.PersoneelId);
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaarLeiderModel.Uitvaartnummer);
                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("InsertUitvaartleiderFailed");
            }
        }
        public void AddMiscUitvaart(OverledeneMiscModel uitvaartMisc)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneUitvaartInfoMisc] ([Id],[UitvaartId],[RouwbrievenId],[AantalRouwbrieven],[AantalUitnodigingen],[AantalKennisgeving],[Advertenties],[UBS], AulaNaam, AantalPersonen, BegraafplaatsGrafNr, BegraafplaatsLocatie) " +
                                        "VALUES (@id, @uitvaartid, @rouwbrievenid, @aantalrouwbrieven, @aantaluitnodiging, @aantalkennisgeving, @advertenties, @ubs, " +
                                        "@aulanaam, @aulapersonen, @grafnummer, @begraafplaats)";
                command.Parameters.AddWithValue("@id", uitvaartMisc.Id);
                command.Parameters.AddWithValue("@uitvaartid", uitvaartMisc.UitvaartId);
                command.Parameters.AddWithValue("@rouwbrievenid", uitvaartMisc.RouwbrievenId);
                command.Parameters.AddWithValue("@aantalrouwbrieven", uitvaartMisc.AantalRouwbrieven ?? string.Empty);
                command.Parameters.AddWithValue("@aantaluitnodiging", uitvaartMisc.AantalUitnodigingen ?? string.Empty);
                command.Parameters.AddWithValue("@aantalkennisgeving", uitvaartMisc.AantalKennisgeving ?? string.Empty);
                command.Parameters.AddWithValue("@advertenties", uitvaartMisc.Advertenties ?? string.Empty);
                command.Parameters.AddWithValue("@ubs", uitvaartMisc.UBS != null ? uitvaartMisc.UBS : '0');
                command.Parameters.AddWithValue("@aulanaam", uitvaartMisc.AulaNaam ?? string.Empty);
                command.Parameters.AddWithValue("@aulapersonen", uitvaartMisc.AulaPersonen);
                command.Parameters.AddWithValue("@grafnummer", uitvaartMisc.GrafNummer ?? string.Empty);
                command.Parameters.AddWithValue("@begraafplaats", uitvaartMisc.Begraafplaats ?? string.Empty);
                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("InsertUitvaartMiscFailed");
            }
        }
        public void AddPersoonsGegevens(OverledenePersoonsGegevensModel GetDocumentOverdrachtInfo)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledenePersoonsGegevens] ([Id], [uitvaartId], [overledeneAchternaam], [overledeneTussenvoegsel], [overledeneVoornamen], [overledeneAanhef], [overledeneGeboortedatum]," +
                                        "[overledeneGeboorteplaats], [overledeneGemeente], [overledeneLeeftijd], [overledeneBSN], [overledeneAdres], [overledeneHuisnummer], [overledeneHuisnummerToevoeging], [overledenePostcode]," +
                                        "[overledeneWoonplaats], [overledeneVoorregeling]) VALUES (@PersoonsGegevensId, @UitvaartId, @SurNameOverledene, @PrepositionOverledene, @FirstnameOverledene, @SalutationOverledene, " +
                                        "@DateOfBirthOverledene, @PlaceOfBirthOverledene,@CountyOverledene, @AgeOverledene, @BSNOverledene, @AddressOverledene, @HousenumberOverledene, @HousenumberAdditionOverledene, " +
                                        "@PostalCodeOverledene, @CityOverledene, @VoorregelingOverledene)";
                command.Parameters.AddWithValue("@PersoonsGegevensId", GetDocumentOverdrachtInfo.Id);
                command.Parameters.AddWithValue("@UitvaartId", GetDocumentOverdrachtInfo.UitvaartId);
                command.Parameters.AddWithValue("@SurNameOverledene", GetDocumentOverdrachtInfo.OverledeneAchternaam);
                command.Parameters.AddWithValue("@PrepositionOverledene", GetDocumentOverdrachtInfo.OverledeneTussenvoegsel != null ? GetDocumentOverdrachtInfo.OverledeneTussenvoegsel : DBNull.Value);
                command.Parameters.AddWithValue("@FirstnameOverledene", GetDocumentOverdrachtInfo.OverledeneVoornamen);
                command.Parameters.AddWithValue("@SalutationOverledene", GetDocumentOverdrachtInfo.OverledeneAanhef);
                command.Parameters.AddWithValue("@DateOfBirthOverledene", GetDocumentOverdrachtInfo.OverledeneGeboortedatum);
                command.Parameters.AddWithValue("@PlaceOfBirthOverledene", GetDocumentOverdrachtInfo.OverledeneGeboorteplaats);
                command.Parameters.AddWithValue("@CountyOverledene", GetDocumentOverdrachtInfo.OverledeneGemeente);
                command.Parameters.AddWithValue("@AgeOverledene", GetDocumentOverdrachtInfo.OverledeneLeeftijd);
                command.Parameters.AddWithValue("@BSNOverledene", GetDocumentOverdrachtInfo.OverledeneBSN);
                command.Parameters.AddWithValue("@AddressOverledene", GetDocumentOverdrachtInfo.OverledeneAdres);
                command.Parameters.AddWithValue("@HousenumberOverledene", GetDocumentOverdrachtInfo.OverledeneHuisnummer);
                command.Parameters.AddWithValue("@HousenumberAdditionOverledene", GetDocumentOverdrachtInfo.OverledeneHuisnummerToevoeging != null ? GetDocumentOverdrachtInfo.OverledeneHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@PostalCodeOverledene", GetDocumentOverdrachtInfo.OverledenePostcode);
                command.Parameters.AddWithValue("@CityOverledene", GetDocumentOverdrachtInfo.OverledeneWoonplaats);
                command.Parameters.AddWithValue("@VoorregelingOverledene", GetDocumentOverdrachtInfo.OverledeneVoorregeling ? GetDocumentOverdrachtInfo.OverledeneVoorregeling : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertPersoonsGegevensFailed");
                }
            }
        }
        public void AddOverlijdenInfo(OverledeneOverlijdenInfoModel overlijdenInfoModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneOverlijdenInfo] ([Id],[UitvaartId],[overledenDatumTijd],[overledenAdres]," +
                                        "[overledenHuisnummer],[overledenHuisnummerToevoeging],[overledenPlaats],[overledenGemeente]," +
                                        "[overledenLijkvinding],[overledenHerkomst],[overledenLidnummer],[overledenHuisarts],[overledenHuisartsTelefoon]," +
                                        "[overledenSchouwarts],[overledenPostcode],[overledenLocatie]) VALUES (@OverlijdenId, @UitvaartId, @DateOverledene," +
                                        "@AddressDeceasedOverledene, @HousenumberDeceasedOverledene, @HousenumberAdditionDeceasedOverlede," +
                                        "@CityDeceasedOverledene, @CountyDeceasedOverledene, @FindingOverledene, @OriginOverledene," +
                                        "@LidNumberOverledene, @HomeDocter, @HomeDocterPhone, @DeceasedDocter, @Postcode, @Locatie)";
                command.Parameters.AddWithValue("@OverlijdenId", overlijdenInfoModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", overlijdenInfoModel.UitvaartId);
                command.Parameters.AddWithValue("@DateOverledene", overlijdenInfoModel.OverledenDatumTijd);
                command.Parameters.AddWithValue("@AddressDeceasedOverledene", overlijdenInfoModel.OverledenAdres);
                command.Parameters.AddWithValue("@HousenumberDeceasedOverledene", overlijdenInfoModel.OverledenHuisnummer);
                command.Parameters.AddWithValue("@HousenumberAdditionDeceasedOverlede", overlijdenInfoModel.OverledenHuisnummerToevoeging != null ? overlijdenInfoModel.OverledenHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@CityDeceasedOverledene", overlijdenInfoModel.OverledenPlaats);
                command.Parameters.AddWithValue("@CountyDeceasedOverledene", overlijdenInfoModel.OverledenGemeente);
                command.Parameters.AddWithValue("@FindingOverledene", overlijdenInfoModel.OverledenLijkvinding);
                command.Parameters.AddWithValue("@OriginOverledene", overlijdenInfoModel.OverledenHerkomst);
                command.Parameters.AddWithValue("@LidNumberOverledene", overlijdenInfoModel.OverledenLidnummer != null ? overlijdenInfoModel.OverledenLidnummer : DBNull.Value);
                command.Parameters.AddWithValue("@HomeDocter", overlijdenInfoModel.OverledenHuisarts != null ? overlijdenInfoModel.OverledenHuisarts : DBNull.Value);
                command.Parameters.AddWithValue("@HomeDocterPhone", overlijdenInfoModel.OverledenHuisartsTelefoon != null ? overlijdenInfoModel.OverledenHuisartsTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@DeceasedDocter", overlijdenInfoModel.OverledenSchouwarts != null ? overlijdenInfoModel.OverledenSchouwarts : DBNull.Value);
                command.Parameters.AddWithValue("@Postcode", overlijdenInfoModel.OverledenPostcode != null ? overlijdenInfoModel.OverledenPostcode : DBNull.Value);
                command.Parameters.AddWithValue("@Locatie", overlijdenInfoModel.OverledenLocatie != null ? overlijdenInfoModel.OverledenLocatie : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertOverlijdenInfoFailed");
                }
            }
        }
        public void AddOverlijdenExtraInfo(OverledeneExtraInfoModel overledeneExtraInfoModel)
        {
            if (overledeneExtraInfoModel == null)
                throw new ArgumentNullException(nameof(overledeneExtraInfoModel));

            using var connection = GetConnection();
            using var command = new SqlCommand
            {
                Connection = connection,
                CommandText = @"
            INSERT INTO [OverledeneExtraInfo] 
            ([Id], [uitvaartId], [overledeneBurgelijkestaat], [overledeneGescheidenVan], [overledeneWedenaarVan], 
             [overledeneTrouwboekje], [overledeneAantalKinderen], [overledeneKinderenMinderjarig], 
             [overledeneAantalKinderenOverleden], [overledeneEersteOuder], [overledeneEersteOuderOverleden], 
             [overledeneTweedeOuder], [overledeneTweedeOuderOverleden], [overledeneLevensovertuiging], 
             [overledeneExecuteur], [overledeneExecuteurTelefoon], [overledeneNotaris], [overledeneNotarisTelefoon], 
             [overledeneTestament], [overledeneTrouwDatumTijd], [overledeneGeregistreerdDatumTijd], 
             [naamWederhelft], [voornaamWederhelft])
            VALUES 
            (@ExtraInfoId, @UitvaartCodeGuid, @MaritalStatus_Family, @Divorsed_Family, @Widow_Family, 
             @MarriageCertificate_Family, @KidsAmount_Family, @KidsMinor_Family, @KinderenOverleden, 
             @ParentOne_Family, @ParentOne_Overleden, @ParentTwo_Family, @ParentTwo_Overleden, 
             @Religion_Family, @Executeur_Family, @ExecuteurPhone_Family, @Notaris_Family, 
             @NotarisPhone_Family, @Will_Family, @DateTimeMarried_Family, @DateTimePartner_Family, 
             @WederhelftNaam, @WederhelftVoornaam)"
            };

            QueryParameters.AddDbNull(command, "@ExtraInfoId", overledeneExtraInfoModel.Id);
            QueryParameters.AddDbNull(command, "@UitvaartCodeGuid", overledeneExtraInfoModel.UitvaartId);
            QueryParameters.AddDbNull(command, "@MaritalStatus_Family", overledeneExtraInfoModel.OverledeneBurgelijkestaat);
            QueryParameters.AddDbNull(command, "@Divorsed_Family", overledeneExtraInfoModel.OverledeneGescheidenVan);
            QueryParameters.AddDbNull(command, "@Widow_Family", overledeneExtraInfoModel.OverledeneWedenaarVan);
            QueryParameters.AddDbNull(command, "@MarriageCertificate_Family", overledeneExtraInfoModel.OverledeneTrouwboekje);
            QueryParameters.AddDbNull(command, "@KidsAmount_Family", overledeneExtraInfoModel.OverledeneAantalKinderen);
            QueryParameters.AddDbNull(command, "@KidsMinor_Family", overledeneExtraInfoModel.OverledeneKinderenMinderjarig);
            QueryParameters.AddDbNull(command, "@KinderenOverleden", overledeneExtraInfoModel.OverledeneKinderenMinderjarigOverleden);
            QueryParameters.AddDbNull(command, "@ParentOne_Family", overledeneExtraInfoModel.OverledeneEersteOuder);
            QueryParameters.AddDbNull(command, "@ParentOne_Overleden", overledeneExtraInfoModel.OverledeneEersteOuderOverleden);
            QueryParameters.AddDbNull(command, "@ParentTwo_Family", overledeneExtraInfoModel.OverledeneTweedeOuder);
            QueryParameters.AddDbNull(command, "@ParentTwo_Overleden", overledeneExtraInfoModel.OverledeneTweedeOuderOverleden);
            QueryParameters.AddDbNull(command, "@Religion_Family", overledeneExtraInfoModel.OverledeneLevensovertuiging);
            QueryParameters.AddDbNull(command, "@Executeur_Family", overledeneExtraInfoModel.OverledeneExecuteur);
            QueryParameters.AddDbNull(command, "@ExecuteurPhone_Family", overledeneExtraInfoModel.OverledeneExecuteurTelefoon);
            QueryParameters.AddDbNull(command, "@Notaris_Family", overledeneExtraInfoModel.OverledeneNotaris);
            QueryParameters.AddDbNull(command, "@NotarisPhone_Family", overledeneExtraInfoModel.OverledeneNotarisTelefoon);
            QueryParameters.AddDbNull(command, "@Will_Family", overledeneExtraInfoModel.OverledeneTestament);
            QueryParameters.AddDbNull(command, "@WederhelftNaam", overledeneExtraInfoModel.NaamWederhelft);
            QueryParameters.AddDbNull(command, "@WederhelftVoornaam", overledeneExtraInfoModel.VoornaamWederhelft);
            QueryParameters.AddDateTime(command, "@DateTimeMarried_Family", overledeneExtraInfoModel.OverledeneTrouwDatumTijd);
            QueryParameters.AddDateTime(command, "@DateTimePartner_Family", overledeneExtraInfoModel.OverledeneGeregistreerdDatumTijd);

            connection.Open();

            if (command.ExecuteNonQuery() == 0)
                throw new InvalidOperationException("InsertOverlijdenExtraInfoFailed");
        }
        public void AddOpdrachtgeverPersoonsGegevens(OpdrachtgeverPersoonsGegevensModel opdrachtgeverPersoonsGegevensModel)
        {
            if (string.IsNullOrEmpty(opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging))
                opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneOpdrachtgever] ([Id], [uitvaartId], [opdrachtgeverAanhef], [opdrachtgeverAchternaam], " +
                    "[opdrachtgeverVoornaamen], [opdrachtgeverTussenvoegsel], [opdrachtgeverGeboortedatum], " +
                    "[opdrachtgeverLeeftijd], [opdrachtgeverStraat], [opdrachtgeverHuisnummer], [opdrachtgeverHuisnummerToevoeging], " +
                    "[opdrachtgeverPostcode], [opdrachtgeverWoonplaats], [opdrachtgeverGemeente], [opdrachtgeverTelefoon], [opdrachtgeverBSN], " +
                    "[opdrachtgeverRelatieTotOverledene], [opdrachtgeverExtraInfo], [opdrachtgeverEmail]) VALUES " +
                    "(@OpdrachtId, @UitvaartId, @Salutation_Family, @SurName_Family, " +
                    "@Firstname_Family, @Preposition_Family, @DateOfBirth_Family, @Age_Family, @Address_Family, " +
                    "@Housenumber_Family, @HousenumberAddition_Family, @PostalCode_Family, @City_Family, @County_Family, " +
                    "@PhoneNumber_Family, @BSN_Family, @RelationToDeceased_Family, @extraInformatie, @Email)";

                QueryParameters.AddDbNull(command, "@OpdrachtId", opdrachtgeverPersoonsGegevensModel.Id);
                QueryParameters.AddDbNull(command, "@UitvaartId", opdrachtgeverPersoonsGegevensModel.UitvaartId);
                QueryParameters.AddDbNull(command, "@Salutation_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef);
                QueryParameters.AddDbNull(command, "@SurName_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam);
                QueryParameters.AddDbNull(command, "@Firstname_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen);
                QueryParameters.AddDbNull(command, "@Preposition_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel);
                QueryParameters.AddDateTime(command, "@DateOfBirth_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum);
                QueryParameters.AddDbNull(command, "@Age_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd);
                QueryParameters.AddDbNull(command, "@Address_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat);
                QueryParameters.AddDbNull(command, "@Housenumber_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer);
                QueryParameters.AddDbNull(command, "@HousenumberAddition_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging);
                QueryParameters.AddDbNull(command, "@PostalCode_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode);
                QueryParameters.AddDbNull(command, "@City_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats);
                QueryParameters.AddDbNull(command, "@County_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente);
                QueryParameters.AddDbNull(command, "@PhoneNumber_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon);
                QueryParameters.AddDbNull(command, "@BSN_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN);
                QueryParameters.AddDbNull(command, "@RelationToDeceased_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene);
                QueryParameters.AddDbNull(command, "@extraInformatie", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverExtraInformatie);
                QueryParameters.AddDbNull(command, "@Email", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail);

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertOpdrachtgeverPersoonsGegevensFailed");
                }
            }
        }
        public void AddOpdrachtgeverExtraPersoonsGegevens(OpdrachtgeverPersoonsGegevensModel opdrachtgeverPersoonsGegevensModel)
        {
            if (string.IsNullOrEmpty(opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging))
                opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging = "";

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneExtraOpdrachtgever] ([Id], [uitvaartId], [opdrachtgeverAanhef], [opdrachtgeverAchternaam], " +
                    "[opdrachtgeverVoornaamen], [opdrachtgeverTussenvoegsel], [opdrachtgeverGeboortedatum], " +
                    "[opdrachtgeverLeeftijd], [opdrachtgeverStraat], [opdrachtgeverHuisnummer], [opdrachtgeverHuisnummerToevoeging], " +
                    "[opdrachtgeverPostcode], [opdrachtgeverWoonplaats], [opdrachtgeverGemeente], [opdrachtgeverTelefoon], [opdrachtgeverBSN], " +
                    "[opdrachtgeverRelatieTotOverledene], [opdrachtgeverEmail]) VALUES " +
                    "(@OpdrachtId, @UitvaartId, @Salutation_Family, @SurName_Family, @Firstname_Family, @Preposition_Family, " +
                    "@DateOfBirth_Family, @Age_Family, @Address_Family, @Housenumber_Family, @HousenumberAddition_Family, " +
                    "@PostalCode_Family, @City_Family, @County_Family, @PhoneNumber_Family, @BSN_Family, @RelationToDeceased_Family, @Email)";

                QueryParameters.AddDbNull(command, "@OpdrachtId", opdrachtgeverPersoonsGegevensModel.Id);
                QueryParameters.AddDbNull(command, "@UitvaartId", opdrachtgeverPersoonsGegevensModel.UitvaartId);
                QueryParameters.AddDbNull(command, "@Salutation_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef);
                QueryParameters.AddDbNull(command, "@SurName_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam);
                QueryParameters.AddDbNull(command, "@Firstname_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen);
                QueryParameters.AddDbNull(command, "@Preposition_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel);
                QueryParameters.AddDateTime(command, "@DateOfBirth_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum);
                QueryParameters.AddDbNull(command, "@Age_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd);
                QueryParameters.AddDbNull(command, "@Address_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat);
                QueryParameters.AddDbNull(command, "@Housenumber_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer);
                QueryParameters.AddDbNull(command, "@HousenumberAddition_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging);
                QueryParameters.AddDbNull(command, "@PostalCode_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode);
                QueryParameters.AddDbNull(command, "@City_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats);
                QueryParameters.AddDbNull(command, "@County_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente);
                QueryParameters.AddDbNull(command, "@PhoneNumber_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon);
                QueryParameters.AddDbNull(command, "@BSN_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN);
                QueryParameters.AddDbNull(command, "@RelationToDeceased_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene);
                QueryParameters.AddDbNull(command, "@Email", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail);

                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("InsertOpdrachtgeverExtraPersoonsGegevensFailed");
            }
        }
        public void AddVerzekering(OverledeneVerzekeringModel overledeneVerzekeringModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneVerzerkeringInfo] (Id, uitvaartId, verzekeringProperties) " +
                        "VALUES (@verzekeringId, @UitvaartId, @verzekeringProperties)";
                command.Parameters.AddWithValue("@verzekeringId", overledeneVerzekeringModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", overledeneVerzekeringModel.UitvaartId);
                command.Parameters.AddWithValue("@verzekeringProperties", overledeneVerzekeringModel.VerzekeringProperties);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertVerzekeringFailed");
                }
            }
        }
        public void AddOpbaren(OverledeneOpbarenModel opbarenModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO OverledeneOpbaring ([opbaringId], [uitvaartId], [opbaringLocatie], [opbaringKistId], [opbaringKistOmschrijving], " +
                    "[opbaringKistLengte], [opbaringVerzorging], [opbaringVerzorgingJson], [opbaringKoeling], [opbaringKledingMee], [opbaringKledingRetour], " +
                    "[opbaringSieraden], [opbaringSieradenOmschrijving], [opbaringSieradenRetour], [opbaringBezoek], [opbaringExtraInfo]) " +
                    "VALUES (@OpbarenId, @UitvaartId, @Locatie, @KistId, @KistOmschrijving, @KistLengte, @Verzorging, @VerzorgingJson, @Koeling, @KledingMee, @KledingRetour, " +
                    "@Sieraden, @SieradenOmschrijving, @SieradenRetour, @Bezoek, @ExtraInfo)";

                QueryParameters.AddDbNull(command, "@OpbarenId", opbarenModel.OpbaringId);
                QueryParameters.AddDbNull(command, "@UitvaartId", opbarenModel.UitvaartId);
                QueryParameters.AddDbNull(command, "@Locatie", opbarenModel.OpbaringLocatie);
                QueryParameters.AddDbNull(command, "@KistId", opbarenModel.OpbaringKistId);
                QueryParameters.AddDbNull(command, "@KistOmschrijving", opbarenModel.OpbaringKistOmschrijving);
                QueryParameters.AddDbNull(command, "@KistLengte", opbarenModel.OpbaringKistLengte);
                QueryParameters.AddDbNull(command, "@Verzorging", opbarenModel.OpbaringVerzorging);
                QueryParameters.AddDbNull(command, "@VerzorgingJson", opbarenModel.OpbaringVerzorgingJson);
                QueryParameters.AddDbNull(command, "@Koeling", opbarenModel.OpbaringKoeling);
                QueryParameters.AddDbNull(command, "@KledingMee", opbarenModel.OpbaringKledingMee);
                QueryParameters.AddDbNull(command, "@KledingRetour", opbarenModel.OpbaringKledingRetour);
                QueryParameters.AddDbNull(command, "@Sieraden", opbarenModel.OpbaringSieraden);
                QueryParameters.AddDbNull(command, "@SieradenOmschrijving", opbarenModel.OpbaringSieradenOmschrijving);
                QueryParameters.AddDbNull(command, "@SieradenRetour", opbarenModel.OpbaringSieradenRetour);
                QueryParameters.AddDbNull(command, "@Bezoek", opbarenModel.OpbaringBezoek);
                QueryParameters.AddDbNull(command, "@ExtraInfo", opbarenModel.OpbaringExtraInfo);

                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("InsertOpbarenFailed");
            }
        }
        public void AddUitvaart(OverledeneUitvaartModel overledeneUitvaartModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneUitvaartInfo] ([Id],[uitvaartId],[uitvaartInfoDatumTijdCondoleance]," +
                                        "[uitvaartInfoCondoleanceConsumpties],[uitvaartInfoType],[uitvaartInfoDatumTijdUitvaart]," +
                                        "[uitvaartInfoUitvaartLocatie],[uitvaartInfoDienstDatumTijd],[uitvaartInfoDienstLocatie]," +
                                        "[uitvaartInfoDienstAfscheid],[uitvaartInfoDienstMuziek],[uitvaartInfoDienstBesloten],[uitvaartInfoDienstVolgauto]," +
                                        "[uitvaartInfoDienstConsumpties],[uitvaartInfoDienstKist],[uitvaartInfoSpreker],[uitvaartInfoPowerPoint]," +
                                        "[uitvaartInfoCondoleanceYesNo],[uitvaartTijdBlokken],[uitvaartAantalTijdsBlokken]) " +
                                        "VALUES (@UitvaartInfoId, @UitvaartId, @DatumTijdCondoleance, @ConsumptiesCondoleance," +
                                        "@Type, @DatumTijdUitvaart, @LocatieUitvaart, @DatumTijdDienst, @DienstLocatie, @AfscheidDienst," +
                                        "@MuziekDienst, @BeslotenDienst, @VolgautoDienst, @ConsumptiesDienst, @KistDienst, @spreker, @powerpoint," +
                                        "@condoleanceYesNo, @TijdBlokken, @AantalTijdsBlokken)";
                command.Parameters.AddWithValue("@condoleanceYesNo", overledeneUitvaartModel.CondoleanceYesNo != null ? overledeneUitvaartModel.CondoleanceYesNo : DBNull.Value);
                command.Parameters.AddWithValue("@spreker", overledeneUitvaartModel.Spreker != null ? overledeneUitvaartModel.Spreker : DBNull.Value);
                command.Parameters.AddWithValue("@powerpoint", overledeneUitvaartModel.PowerPoint != null ? overledeneUitvaartModel.PowerPoint : DBNull.Value);
                command.Parameters.AddWithValue("@UitvaartInfoId", overledeneUitvaartModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", overledeneUitvaartModel.UitvaartId);
                command.Parameters.AddWithValue("@DatumTijdCondoleance", overledeneUitvaartModel.DatumTijdCondoleance != null ? overledeneUitvaartModel.DatumTijdCondoleance : DBNull.Value);
                command.Parameters.AddWithValue("@ConsumptiesCondoleance", overledeneUitvaartModel.ConsumptiesCondoleance != null ? overledeneUitvaartModel.ConsumptiesCondoleance : DBNull.Value);
                command.Parameters.AddWithValue("@Type", overledeneUitvaartModel.TypeDienst);
                command.Parameters.AddWithValue("@DatumTijdUitvaart", overledeneUitvaartModel.DatumTijdUitvaart != null ? overledeneUitvaartModel.DatumTijdUitvaart : DBNull.Value);
                command.Parameters.AddWithValue("@LocatieUitvaart", overledeneUitvaartModel.LocatieUitvaart);
                command.Parameters.AddWithValue("@DienstLocatie", overledeneUitvaartModel.LocatieDienst);
                command.Parameters.AddWithValue("@DatumTijdDienst", overledeneUitvaartModel.DatumTijdDienst != null ? overledeneUitvaartModel.DatumTijdDienst : DBNull.Value);
                command.Parameters.AddWithValue("@AfscheidDienst", overledeneUitvaartModel.AfscheidDienst);
                command.Parameters.AddWithValue("@MuziekDienst", overledeneUitvaartModel.MuziekDienst);
                command.Parameters.AddWithValue("@BeslotenDienst", overledeneUitvaartModel.BeslotenDienst);
                command.Parameters.AddWithValue("@VolgautoDienst", overledeneUitvaartModel.VolgAutoDienst);
                command.Parameters.AddWithValue("@ConsumptiesDienst", overledeneUitvaartModel.ConsumptiesDienst != null ? overledeneUitvaartModel.ConsumptiesDienst : DBNull.Value);
                command.Parameters.AddWithValue("@KistDienst", overledeneUitvaartModel.KistDienst);
                command.Parameters.AddWithValue("@TijdBlokken", overledeneUitvaartModel.TijdBlokken != null ? overledeneUitvaartModel.TijdBlokken : DBNull.Value);
                command.Parameters.AddWithValue("@AantalTijdsBlokken", overledeneUitvaartModel.AantalTijdsBlokken != null ? overledeneUitvaartModel.AantalTijdsBlokken : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertUitvaartFailed");
                }
            }
        }
        public void AddAsbestemming(OverledeneAsbestemmingModel asbestemmingModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneAsbestemming] ([asbestemmingId],[uitvaartId],[asbestemming],[typeGraf],[bestaandGraf],[zandKelderGraf],[grafmonument]) " +
                                        "VALUES (@AsbestemmingId, @UitvaartId, @Asbestemming, @typeGraf, @bestaandGraf, @zandKelderGraf, @grafMonument)";
                command.Parameters.AddWithValue("@AsbestemmingId", asbestemmingModel.AsbestemmingId);
                command.Parameters.AddWithValue("@UitvaartId", asbestemmingModel.UitvaartId);
                command.Parameters.AddWithValue("@Asbestemming", asbestemmingModel.Asbestemming != null ? asbestemmingModel.Asbestemming : DBNull.Value);
                command.Parameters.AddWithValue("@typeGraf", asbestemmingModel.TypeGraf != null ? asbestemmingModel.TypeGraf : DBNull.Value);
                command.Parameters.AddWithValue("@bestaandGraf", asbestemmingModel.BestaandGraf != null ? asbestemmingModel.BestaandGraf : DBNull.Value);
                command.Parameters.AddWithValue("@zandKelderGraf", asbestemmingModel.ZandKelderGraf != null ? asbestemmingModel.ZandKelderGraf : DBNull.Value);
                command.Parameters.AddWithValue("@grafMonument", asbestemmingModel.GrafMonument != null ? asbestemmingModel.GrafMonument : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertAsbestemmingFailed");
                }
            }
        }
        public string AddBijlages(OverledeneBijlagesModel bijlagesModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneBijlages] ([BijlageId],[UitvaartId],[DocumentName],[DocumentType]," +
                                        "[DocumentURL],[DocumentHash],[DocumentInconsistent]) " +
                                        "VALUES (@BijlageId, @UitvaartId, @DocumentName, @DocumentType, @DocumentUrl, @DocumentHash, @DocumentInconsistent)";
                command.Parameters.AddWithValue("@BijlageId", bijlagesModel.BijlageId);
                command.Parameters.AddWithValue("@UitvaartId", bijlagesModel.UitvaartId);
                command.Parameters.AddWithValue("@DocumentName", bijlagesModel.DocumentName);
                command.Parameters.AddWithValue("@DocumentType", bijlagesModel.DocumentType);
                command.Parameters.AddWithValue("@DocumentUrl", bijlagesModel.DocumentUrl);
                command.Parameters.AddWithValue("@DocumentHash", bijlagesModel.DocumentHash);
                command.Parameters.AddWithValue("@DocumentInconsistent", bijlagesModel.DocumentInconsistent);
                if (command.ExecuteNonQuery() == 0)
                {
                    return "InsertBijlagesFailed";
                }
                else
                {
                    return "InsertBijlagesSucces";
                }
            }
        }
        public void AddSteenhouwerij(OverledeneSteenhouwerijModel overledeneSteenhouwerijModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneSteenhouwer] ([Id],[uitvaartId],[steenhouwerOpdracht],[steenhouwerBedrag],[steenhouwerProvisie],[steenhouwerUitbetaing],[steenhouwerText], [steenhouwerLeverancier]) " +
                                        "VALUES (@SteenhouwerijId, @UitvaartId, @Opdracht, @Bedrag, @Provisie, @Uitbetaling, @Text, @Leverancier)";
                command.Parameters.AddWithValue("@SteenhouwerijId", overledeneSteenhouwerijModel.SteenhouwerijId);
                command.Parameters.AddWithValue("@UitvaartId", overledeneSteenhouwerijModel.UitvaartId);
                command.Parameters.AddWithValue("@Opdracht", overledeneSteenhouwerijModel.SteenhouwerOpdracht != null ? overledeneSteenhouwerijModel.SteenhouwerOpdracht : string.Empty);
                command.Parameters.AddWithValue("@Bedrag", overledeneSteenhouwerijModel.SteenhouwerBedrag != null ? overledeneSteenhouwerijModel.SteenhouwerBedrag : DBNull.Value);
                command.Parameters.AddWithValue("@Provisie", overledeneSteenhouwerijModel.SteenhouwerProvisie != null ? overledeneSteenhouwerijModel.SteenhouwerProvisie : DBNull.Value);
                command.Parameters.AddWithValue("@Uitbetaling", overledeneSteenhouwerijModel.SteenhouwerUitbetaing != null ? overledeneSteenhouwerijModel.SteenhouwerUitbetaing : DBNull.Value);
                command.Parameters.AddWithValue("@Text", overledeneSteenhouwerijModel.SteenhouwerText);
                command.Parameters.AddWithValue("@Leverancier", overledeneSteenhouwerijModel.SteenhouwerLeverancier);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertSteenhouwerijFailed");
                }
            }
        }
        public void AddBloemen(OverledeneBloemenModel overledeneBloemenModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneBloemen] ([Id],[uitvaartId],[bloemenText],[bloemenLint],[bloemenKaart],[bloemenBedrag],[bloemenProvisie],[bloemenUitbetaling], [bloemenLeverancier], [bloemenLintJson], [bloemenBezorgingDatum], [bloemenBezorgingAdres]) " +
                                        "VALUES (@BloemenId, @UitvaartId, @Text, @Lint, @Kaart, @Bedrag, @Provisie, @Uitbetaling, @Leverancier, @bloemenjson, @bezorgdatum, @bezorgadres)";
                command.Parameters.AddWithValue("@BloemenId", overledeneBloemenModel.BloemenId);
                command.Parameters.AddWithValue("@UitvaartId", overledeneBloemenModel.UitvaartId);
                command.Parameters.AddWithValue("@Text", overledeneBloemenModel.BloemenText != null ? overledeneBloemenModel.BloemenText : DBNull.Value);
                command.Parameters.AddWithValue("@Lint", overledeneBloemenModel.BloemenLint.HasValue ? (object)overledeneBloemenModel.BloemenLint.Value : DBNull.Value);
                command.Parameters.AddWithValue("@Kaart", overledeneBloemenModel.BloemenKaart.HasValue ? (object)overledeneBloemenModel.BloemenKaart.Value : DBNull.Value);
                command.Parameters.AddWithValue("@Bedrag", overledeneBloemenModel.BloemenBedrag != null ? overledeneBloemenModel.BloemenBedrag : DBNull.Value);
                command.Parameters.AddWithValue("@Provisie", overledeneBloemenModel.BloemenProvisie != null ? overledeneBloemenModel.BloemenProvisie : DBNull.Value);
                command.Parameters.AddWithValue("@Uitbetaling", overledeneBloemenModel.BloemenUitbetaling != null ? overledeneBloemenModel.BloemenUitbetaling : DBNull.Value);
                command.Parameters.AddWithValue("@Leverancier", overledeneBloemenModel.BloemenLeverancier);
                command.Parameters.AddWithValue("@bloemenjson", overledeneBloemenModel.BloemenLintJson != null ? overledeneBloemenModel.BloemenLintJson : DBNull.Value);
                command.Parameters.Add(new SqlParameter("@bezorgdatum", SqlDbType.DateTime)
                {
                    Value = overledeneBloemenModel.BloemenBezorgDate.HasValue ? (object)overledeneBloemenModel.BloemenBezorgDate.Value : DBNull.Value
                });
                command.Parameters.AddWithValue("@bezorgadres", overledeneBloemenModel.BloemenBezorgAdres != null ? overledeneBloemenModel.BloemenBezorgAdres : DBNull.Value);

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertBloemenFailed");
                }
            }
        }
        public void AddWerkbonnen(OverledeneWerkbonUitvaart overledeneWerkbonUitvaart)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneWerkbon] ([Id],[uitvaartId],[werkbonJson]) " +
                                        "VALUES (@WerkbonId, @UitvaartId, @WerkbonJson)";
                command.Parameters.AddWithValue("@WerkbonId", overledeneWerkbonUitvaart.Id);
                command.Parameters.AddWithValue("@UitvaartId", overledeneWerkbonUitvaart.UitvaartId);
                command.Parameters.AddWithValue("@WerkbonJson", overledeneWerkbonUitvaart.WerkbonJson != null ? overledeneWerkbonUitvaart.WerkbonJson : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertWerkbonnenFailed");
                }
            }
        }
        public void AddUrnSieraden(OverledeneUrnSieradenModel overledeneUrnSieradenModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneUrnSieraden] ([Id],[uitvaartId],[urnOpdracht],[urnBedrag],[urnProvisie],[urnUitbetaling],[urnText],[urnLeverancier]) " +
                                        "VALUES (@SteenhouwerijId, @UitvaartId, @Opdracht, @Bedrag, @Provisie, @Uitbetaling, @Text, @Leverancier)";
                command.Parameters.AddWithValue("@SteenhouwerijId", overledeneUrnSieradenModel.UrnId);
                command.Parameters.AddWithValue("@UitvaartId", overledeneUrnSieradenModel.UitvaartId);
                command.Parameters.AddWithValue("@Opdracht", overledeneUrnSieradenModel.UrnOpdracht != null ? overledeneUrnSieradenModel : DBNull.Value);
                command.Parameters.AddWithValue("@Bedrag", overledeneUrnSieradenModel.UrnBedrag != null ? overledeneUrnSieradenModel.UrnBedrag : DBNull.Value);
                command.Parameters.AddWithValue("@Provisie", overledeneUrnSieradenModel.UrnProvisie != null ? overledeneUrnSieradenModel.UrnProvisie : DBNull.Value);
                command.Parameters.AddWithValue("@Uitbetaling", overledeneUrnSieradenModel.UrnUitbetaing != null ? overledeneUrnSieradenModel.UrnUitbetaing : DBNull.Value);
                command.Parameters.AddWithValue("@Text", overledeneUrnSieradenModel.UrnText);
                command.Parameters.AddWithValue("@Leverancier", overledeneUrnSieradenModel.UrnLeverancier);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertUrnSieradenFailed");
                }
            }
        }
        public void AddFactuur(FactuurModel overledeneKostenbegrotingModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO OverledeneFacturen ([Id],[uitvaartId],[kostenbegrotingUrl],[kostenbegrotingJson],[kostenbegrotingCreationDate]," +
                                    "[kostenbegrotingCreated],[kostenbegrotingVerzekeraar], [korting]) " +
                                    "VALUES (@FactuurId, @UitvaartId, @kostenbegrotingUrl, @kostenbegrotingJson, @kostenbegrotingCreationDate, @kostenbegrotingCreated,@verzekeraarId, @korting)";
                command.Parameters.AddWithValue("@FactuurId", overledeneKostenbegrotingModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", overledeneKostenbegrotingModel.UitvaartId);
                command.Parameters.AddWithValue("@kostenbegrotingUrl", overledeneKostenbegrotingModel.KostenbegrotingUrl);
                command.Parameters.AddWithValue("@kostenbegrotingJson", overledeneKostenbegrotingModel.KostenbegrotingJson);
                command.Parameters.AddWithValue("@kostenbegrotingCreationDate", DateTime.Now);
                command.Parameters.AddWithValue("@kostenbegrotingCreated", 1);
                command.Parameters.AddWithValue("@verzekeraarId", overledeneKostenbegrotingModel.KostenbegrotingVerzekeraar);
                command.Parameters.AddWithValue("@korting", overledeneKostenbegrotingModel.Korting == 0 ? 0 : overledeneKostenbegrotingModel.Korting);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertFactuurFailed");
                }
            }
        }
        public void AddKlanttevredenheid(Klanttevredenheid klanttevredenheid)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO OverledeneKlantTevredenheid (Id, UitvaartId, Cijfer, NotificatieOverleden) VALUES (@id, @uitvaartId, @cijfer, @notificatie)";
                command.Parameters.AddWithValue("@id", klanttevredenheid.Id);
                command.Parameters.AddWithValue("@UitvaartId", klanttevredenheid.UitvaartId);
                command.Parameters.AddWithValue("@cijfer", klanttevredenheid.CijferScore);
                command.Parameters.AddWithValue("@notificatie", (klanttevredenheid.IsNotificationEnabled == true) ? true : false);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertKlanttevredenheidFailed");
                }
            }
        }
        public void CreateWindowsUser(Guid Id, Guid PersoneelId, string WindowsUser)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [ConfigurationUsers] ([Id],[PersoneelId],[WindowsUsername],[IsActive]) VALUES (@Id, @PersoneelId, @WindowsUser, @Inactive)";
                command.Parameters.AddWithValue("@Id", Id);
                command.Parameters.AddWithValue("@PersoneelId", PersoneelId);
                command.Parameters.AddWithValue("@WindowsUser", WindowsUser);
                command.Parameters.AddWithValue("@Inactive", 0);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreatingNewUserFailed");
                }
            }
        }
        public void CreateUserPermission(Guid PersoneelId, Guid rechtenId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [ConfigurationPersoneelPermission] ([Id],[PermissionId],[PersoneelId],[PersoneelKey]) VALUES (@Id, @PermissionId, @PersoneelId, @Key)";
                command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                command.Parameters.AddWithValue("@PermissionId", rechtenId);
                command.Parameters.AddWithValue("@PersoneelId", PersoneelId);
                command.Parameters.AddWithValue("@Key", DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreateUserPermissionFailed");
                }
            }
        }
        public void CreatePriceComponent(KostenbegrotingModel priceComponent)
        {
            Debug.WriteLine(priceComponent.ComponentAantal);
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [ConfigurationFactuurComponent] (Id, ComponentId, Omschrijving, Bedrag, VerzekerdAantal, VerzekeringJson,IsDeleted, SortOrder, factuurBedrag, DefaultPM) " +
                                        "VALUES (@id, @compid, @omschrijving, @bedrag, @aantal, @verzekeringJson, 0, @sortOrder, @factuurBedrag, @defaultpm)";
                command.Parameters.AddWithValue("@id", priceComponent.Id);
                command.Parameters.AddWithValue("@compid", priceComponent.ComponentId);
                command.Parameters.AddWithValue("@omschrijving", priceComponent.ComponentOmschrijving);
                command.Parameters.AddWithValue("@aantal", priceComponent.ComponentAantal);
                command.Parameters.AddWithValue("@bedrag", priceComponent.ComponentBedrag);
                command.Parameters.AddWithValue("@factuurBedrag", priceComponent.ComponentFactuurBedrag);
                command.Parameters.AddWithValue("@verzekeringJson", priceComponent.ComponentVerzekeringJson);
                command.Parameters.AddWithValue("@defaultpm", priceComponent.DefaultPM);
                command.Parameters.AddWithValue("sortOrder", priceComponent.SortOrder);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreatePriceComponentFailed");
                }
            }
        }
        public void CreateLeverancier(LeveranciersModel leveranciers)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [ConfigurationLeveranciers] ([leverancierId],[leverancierName],[leverancierBeschrijving]," +
                                        "[steenhouwer],[bloemist],[kisten],[isDeleted],[urnsieraden]) " +
                                        "VALUES (@id, @name, @beschrijving, @steen, @bloem, @kist, 0, @urn)";
                command.Parameters.AddWithValue("@id", leveranciers.LeverancierId);
                command.Parameters.AddWithValue("@name", leveranciers.LeverancierName);
                command.Parameters.AddWithValue("@beschrijving", leveranciers.LeverancierBeschrijving);
                command.Parameters.AddWithValue("@steen", leveranciers.Steenhouwer);
                command.Parameters.AddWithValue("@bloem", leveranciers.Bloemist);
                command.Parameters.AddWithValue("@kist", leveranciers.Kisten);
                command.Parameters.AddWithValue("@urn", leveranciers.UrnSieraden);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreateLeverancierFailed");
                }
            }
        }
        public void CreateRouwbrief(OverledeneRouwbrieven rouwbrief)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [ConfigurationRouwbrieven] ([rouwbrievenId],[rouwbrievenName],[isDeleted]) " +
                                        "VALUES (@id, @name, 0)";
                command.Parameters.AddWithValue("@id", rouwbrief.RouwbrievenId);
                command.Parameters.AddWithValue("@name", rouwbrief.RouwbrievenName);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreateRouwbriefFailed");
                }
            }
        }
        public void KistCreate(KistenModel kistCreate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO ConfigurationKisten (Id, kistOmschrijving, kistTypeNummer, isDeleted) VALUES (@Id, @omschrijving, @typenummer, 0)";
                command.Parameters.AddWithValue("@Id", kistCreate.Id);
                command.Parameters.AddWithValue("@omschrijving", kistCreate.KistOmschrijving);
                command.Parameters.AddWithValue("@typenummer", kistCreate.KistTypeNummer);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("KistCreateFailed");
                }
            }
        }
        public void AsbestemmingCreate(ConfigurationAsbestemmingModel asbestemmingCreate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO ConfigurationAsbestemming (asbestemmingId, asbestemmingOmschrijving, isDeleted) VALUES (@Id, @omschrijving, 0)";
                QueryParameters.AddDbNull(command, "@Id", asbestemmingCreate.AsbestemmingId);
                QueryParameters.AddDbNull(command, "@omschrijving", asbestemmingCreate.AsbestemmingOmschrijving);

                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("AsbestemmingCreateFailed");
            }
        }
        public void VerzekeringCreate(VerzekeraarsModel verzkeringCreate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO ConfigurationVerzekeraar (Id, verzekeraarNaam, isHerkomst, isVerzekeraar, hasLidnummer, postbusAddress, postbusNaam, addressStreet, addressHousenumber, addressHousenumberAddition, addressZipcode, addressCity, factuurType, correspondentieType, isDeleted, isPakket, OverrideFactuurAdress, verzekeraarTelefoon, CustomLogo) " +
                    "VALUES (@Id, @naam, @herkomst, @verzekeraar, @lidnummer, @postbusAdres, @postbusNaam, @street, @housenumber, @addition, @zipcode, @city, @factuurtype, @correspondentieType, 0, @isPakket, @isOverrideAddress, @telefoon, @customLogo)";

                QueryParameters.AddDbNull(command, "@Id", verzkeringCreate.Id);
                QueryParameters.AddDbNull(command, "@naam", verzkeringCreate.Name);
                QueryParameters.AddBoolean(command, "@herkomst", verzkeringCreate.IsHerkomst);
                QueryParameters.AddBoolean(command, "@verzekeraar", verzkeringCreate.IsVerzekeraar);
                QueryParameters.AddBoolean(command, "@lidnummer", verzkeringCreate.HasLidnummer);
                QueryParameters.AddDbNull(command, "@postbusAdres", verzkeringCreate.PostbusAddress);
                QueryParameters.AddDbNull(command, "@postbusNaam", verzkeringCreate.PostbusName);
                QueryParameters.AddDbNull(command, "@street", verzkeringCreate.AddressStreet);
                QueryParameters.AddDbNull(command, "@housenumber", verzkeringCreate.AddressHousenumber);
                QueryParameters.AddDbNull(command, "@addition", verzkeringCreate.AddressHousenumberAddition);
                QueryParameters.AddDbNull(command, "@zipcode", verzkeringCreate.AddressZipCode);
                QueryParameters.AddDbNull(command, "@city", verzkeringCreate.AddressCity);
                QueryParameters.AddDbNull(command, "@factuurtype", verzkeringCreate.FactuurType);
                QueryParameters.AddDbNull(command, "@correspondentieType", verzkeringCreate.CorrespondentieType);
                QueryParameters.AddBoolean(command, "@isPakket", verzkeringCreate.Pakket);
                QueryParameters.AddDbNull(command, "@isOverrideAddress", verzkeringCreate.IsOverrideFactuurAdress);
                QueryParameters.AddDbNull(command, "@telefoon", verzkeringCreate.Telefoon);
                QueryParameters.AddBoolean(command, "@customLogo", verzkeringCreate.CustomLogo);

                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("CreateVerzekeringFailed");
            }
        }
        public void CreateSuggestion(SuggestionModel suggestionCreate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO ConfigurationOverledenLocaties ([Id],[ShortName],[LongName],[Street],[Housenumber],[Zipcode],[City],[County],[IsDeleted]) " +
                                        " VALUES  (@Id, @ShortName, @LongName, @Street, @Housenumber, @Zipcode, @City, @County, 0)";
                command.Parameters.AddWithValue("@Id", suggestionCreate.Id);
                command.Parameters.AddWithValue("@ShortName", suggestionCreate.ShortName != null ? suggestionCreate.ShortName : DBNull.Value);
                command.Parameters.AddWithValue("@LongName", suggestionCreate.LongName != null ? suggestionCreate.LongName : DBNull.Value);
                command.Parameters.AddWithValue("@Street", suggestionCreate.Street != null ? suggestionCreate.Street : DBNull.Value);
                command.Parameters.AddWithValue("@Housenumber", suggestionCreate.HouseNumber != null ? suggestionCreate.HouseNumber : DBNull.Value);
                command.Parameters.AddWithValue("@Zipcode", suggestionCreate.ZipCode != null ? suggestionCreate.ZipCode : DBNull.Value);
                command.Parameters.AddWithValue("@City", suggestionCreate.City != null ? suggestionCreate.City : DBNull.Value);
                command.Parameters.AddWithValue("@County", suggestionCreate.County != null ? suggestionCreate.County : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreateSuggestionFailed");
                }
            }
        }
    }
}
