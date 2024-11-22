using Dossier_Registratie.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

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
        public async Task InsertKostenbegrotingAsync(string kostenbegrotingUrl, string kostenbegrotingData, DateTime creationDate, Guid uitvaartId, Guid id, Guid verzekeraarId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneFacturen] ([Id],[uitvaartId],[kostenbegrotingUrl],[kostenbegrotingJson],[kostenbegrotingCreationDate],[kostenbegrotingCreated],[kostenbegrotingVerzekeraar]) " +
                                        "VALUES (@id, @uitvaartId, @kostenbegrotingUrl, @kostenbegrotingData, @kostenbegrotingCreationDate, 1, @verzekeraarId)";
                command.Parameters.AddWithValue("@kostenbegrotingUrl", kostenbegrotingUrl);
                command.Parameters.AddWithValue("@kostenbegrotingData", kostenbegrotingData);
                command.Parameters.AddWithValue("@kostenbegrotingCreationDate", creationDate);
                command.Parameters.AddWithValue("@verzekeraarId", verzekeraarId);
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
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneBijlages] ([BijlageId],[UitvaartId],[DocumentName],[DocumentType],[DocumentURL],[DocumentHash],[DocumentInconsistent],[isDeleted]) " +
                                      "VALUES (@bijlageId, @uitvaartId, @documentName, @documentType, @documentUrl, @documentHash, @documentInconsistent, @isDeleted)";
                command.Parameters.AddWithValue("@bijlageId", documentInfo.BijlageId);
                command.Parameters.AddWithValue("@UitvaartId", documentInfo.UitvaartId);
                command.Parameters.AddWithValue("@documentName", documentInfo.DocumentName);
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
                command.Parameters.AddWithValue("@aantalrouwbrieven", uitvaartMisc.AantalRouwbrieven != null ? uitvaartMisc.AantalRouwbrieven : string.Empty);
                command.Parameters.AddWithValue("@aantaluitnodiging", uitvaartMisc.AantalUitnodigingen != null ? uitvaartMisc.AantalUitnodigingen : string.Empty);
                command.Parameters.AddWithValue("@aantalkennisgeving", uitvaartMisc.AantalKennisgeving != null ? uitvaartMisc.AantalKennisgeving : string.Empty);
                command.Parameters.AddWithValue("@advertenties", uitvaartMisc.Advertenties != null ? uitvaartMisc.Advertenties : string.Empty);
                command.Parameters.AddWithValue("@ubs", uitvaartMisc.UBS != null ? uitvaartMisc.UBS : '0');
                command.Parameters.AddWithValue("@aulanaam", uitvaartMisc.AulaNaam != null ? uitvaartMisc.AulaNaam : string.Empty);
                command.Parameters.AddWithValue("@aulapersonen", uitvaartMisc.AulaPersonen != null ? uitvaartMisc.AulaPersonen : 0);
                command.Parameters.AddWithValue("@grafnummer", uitvaartMisc.GrafNummer != null ? uitvaartMisc.GrafNummer : string.Empty);
                command.Parameters.AddWithValue("@begraafplaats", uitvaartMisc.Begraafplaats != null ? uitvaartMisc.Begraafplaats : string.Empty);
                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("InsertUitvaartMiscFailed");
            }
        }
        public void AddPersoonsGegevens(OverledenePersoonsGegevensModel persoonsGegevensModel)
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
                command.Parameters.AddWithValue("@PersoonsGegevensId", persoonsGegevensModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", persoonsGegevensModel.UitvaartId);
                command.Parameters.AddWithValue("@SurNameOverledene", persoonsGegevensModel.OverledeneAchternaam);
                command.Parameters.AddWithValue("@PrepositionOverledene", persoonsGegevensModel.OverledeneTussenvoegsel != null ? persoonsGegevensModel.OverledeneTussenvoegsel : DBNull.Value);
                command.Parameters.AddWithValue("@FirstnameOverledene", persoonsGegevensModel.OverledeneVoornamen);
                command.Parameters.AddWithValue("@SalutationOverledene", persoonsGegevensModel.OverledeneAanhef);
                command.Parameters.AddWithValue("@DateOfBirthOverledene", persoonsGegevensModel.OverledeneGeboortedatum);
                command.Parameters.AddWithValue("@PlaceOfBirthOverledene", persoonsGegevensModel.OverledeneGeboorteplaats);
                command.Parameters.AddWithValue("@CountyOverledene", persoonsGegevensModel.OverledeneGemeente);
                command.Parameters.AddWithValue("@AgeOverledene", persoonsGegevensModel.OverledeneLeeftijd);
                command.Parameters.AddWithValue("@BSNOverledene", persoonsGegevensModel.OverledeneBSN);
                command.Parameters.AddWithValue("@AddressOverledene", persoonsGegevensModel.OverledeneAdres);
                command.Parameters.AddWithValue("@HousenumberOverledene", persoonsGegevensModel.OverledeneHuisnummer);
                command.Parameters.AddWithValue("@HousenumberAdditionOverledene", persoonsGegevensModel.OverledeneHuisnummerToevoeging != null ? persoonsGegevensModel.OverledeneHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@PostalCodeOverledene", persoonsGegevensModel.OverledenePostcode);
                command.Parameters.AddWithValue("@CityOverledene", persoonsGegevensModel.OverledeneWoonplaats);
                command.Parameters.AddWithValue("@VoorregelingOverledene", persoonsGegevensModel.OverledeneVoorregeling ? persoonsGegevensModel.OverledeneVoorregeling : DBNull.Value);
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
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [OverledeneExtraInfo] ([Id], [uitvaartId], [overledeneBurgelijkestaat], " +
                                    "[overledeneGescheidenVan],[overledeneWedenaarVan], [overledeneTrouwboekje], [overledeneAantalKinderen], " +
                                    "[overledeneKinderenMinderjarig], [overledeneAantalKinderenOverleden], [overledeneEersteOuder], [overledeneEersteOuderOverleden], [overledeneTweedeOuder], [overledeneTweedeOuderOverleden],[overledeneLevensovertuiging], " +
                                    "[overledeneExecuteur], [overledeneExecuteurTelefoon], [overledeneNotaris], [overledeneNotarisTelefoon], " +
                                    "[overledeneTestament], [overledeneTrouwDatumTijd], [overledeneGeregistreerdDatumTijd], [naamWederhelft], [voornaamWederhelft]) " +
                                    "VALUES(@ExtraInfoId, @UitvaartCodeGuid, @MaritalStatus_Family, @Divorsed_Family, " +
                                    "@Widow_Family, @MarriageCertificate_Family, @KidsAmount_Family, @KidsMinor_Family, @KinderenOverleden, @ParentOne_Family, " +
                                    "@ParentOne_Overleden, @ParentTwo_Family, @ParentTwo_Overleden, @Religion_Family, @Executeur_Family,@ExecuteurPhone_Family,@Notaris_Family," +
                                    "@NotarisPhone_Family,@Will_Family,@DateTimeMarried_Family,@DateTimePartner_Family, @WederhelftNaam, @WederhelftVoornaam)";
                command.Parameters.AddWithValue("@ExtraInfoId", overledeneExtraInfoModel.Id);
                command.Parameters.AddWithValue("@UitvaartCodeGuid", overledeneExtraInfoModel.UitvaartId);
                command.Parameters.AddWithValue("@MaritalStatus_Family", overledeneExtraInfoModel.OverledeneBurgelijkestaat != null ? overledeneExtraInfoModel.OverledeneBurgelijkestaat : DBNull.Value);
                command.Parameters.AddWithValue("@Divorsed_Family", overledeneExtraInfoModel.OverledeneGescheidenVan != null ? overledeneExtraInfoModel.OverledeneGescheidenVan : DBNull.Value);
                command.Parameters.AddWithValue("@Widow_Family", overledeneExtraInfoModel.OverledeneWedenaarVan != null ? overledeneExtraInfoModel.OverledeneWedenaarVan : DBNull.Value);
                command.Parameters.AddWithValue("@MarriageCertificate_Family", overledeneExtraInfoModel.OverledeneTrouwboekje != null ? overledeneExtraInfoModel.OverledeneTrouwboekje : DBNull.Value);
                command.Parameters.AddWithValue("@KidsAmount_Family", overledeneExtraInfoModel.OverledeneAantalKinderen != null ? overledeneExtraInfoModel.OverledeneAantalKinderen : DBNull.Value);
                command.Parameters.AddWithValue("@KidsMinor_Family", overledeneExtraInfoModel.OverledeneKinderenMinderjarig != null ? overledeneExtraInfoModel.OverledeneKinderenMinderjarig : DBNull.Value);
                command.Parameters.AddWithValue("@KinderenOverleden", overledeneExtraInfoModel.OverledeneKinderenMinderjarigOverleden != null ? overledeneExtraInfoModel.OverledeneKinderenMinderjarigOverleden : DBNull.Value);
                command.Parameters.AddWithValue("@ParentOne_Family", overledeneExtraInfoModel.OverledeneEersteOuder != null ? overledeneExtraInfoModel.OverledeneEersteOuder : DBNull.Value);
                command.Parameters.AddWithValue("@ParentOne_Overleden", overledeneExtraInfoModel.OverledeneEersteOuderOverleden != null ? overledeneExtraInfoModel.OverledeneEersteOuderOverleden : DBNull.Value);
                command.Parameters.AddWithValue("@ParentTwo_Family", overledeneExtraInfoModel.OverledeneTweedeOuder != null ? overledeneExtraInfoModel.OverledeneTweedeOuder : DBNull.Value);
                command.Parameters.AddWithValue("@ParentTwo_Overleden", overledeneExtraInfoModel.OverledeneTweedeOuderOverleden != null ? overledeneExtraInfoModel.OverledeneTweedeOuderOverleden : DBNull.Value);
                command.Parameters.AddWithValue("@Religion_Family", overledeneExtraInfoModel.OverledeneLevensovertuiging != null ? overledeneExtraInfoModel.OverledeneLevensovertuiging : DBNull.Value);
                command.Parameters.AddWithValue("@Executeur_Family", overledeneExtraInfoModel.OverledeneExecuteur != null ? overledeneExtraInfoModel.OverledeneExecuteur : DBNull.Value);
                command.Parameters.AddWithValue("@ExecuteurPhone_Family", overledeneExtraInfoModel.OverledeneExecuteurTelefoon != null ? overledeneExtraInfoModel.OverledeneExecuteurTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@Notaris_Family", overledeneExtraInfoModel.OverledeneNotaris != null ? overledeneExtraInfoModel.OverledeneNotaris : DBNull.Value);
                command.Parameters.AddWithValue("@NotarisPhone_Family", overledeneExtraInfoModel.OverledeneNotarisTelefoon != null ? overledeneExtraInfoModel.OverledeneNotarisTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@Will_Family", overledeneExtraInfoModel.OverledeneTestament != null ? overledeneExtraInfoModel.OverledeneTestament : DBNull.Value);
                command.Parameters.AddWithValue("@WederhelftNaam", overledeneExtraInfoModel.NaamWederhelft != null ? overledeneExtraInfoModel.NaamWederhelft : DBNull.Value);
                command.Parameters.AddWithValue("@WederhelftVoornaam", overledeneExtraInfoModel.VoornaamWederhelft != null ? overledeneExtraInfoModel.VoornaamWederhelft : DBNull.Value);
                command.Parameters.Add(new SqlParameter("@DateTimeMarried_Family", SqlDbType.DateTime)
                {
                    Value = overledeneExtraInfoModel.OverledeneTrouwDatumTijd.HasValue
                                ? (object)overledeneExtraInfoModel.OverledeneTrouwDatumTijd.Value
                                : DBNull.Value
                });
                command.Parameters.Add(new SqlParameter("@DateTimePartner_Family", SqlDbType.DateTime)
                {
                    Value = overledeneExtraInfoModel.OverledeneGeregistreerdDatumTijd.HasValue
                ? (object)overledeneExtraInfoModel.OverledeneGeregistreerdDatumTijd.Value
                : DBNull.Value
                });

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertOverlijdenExtraInfoFailed");
                }
            }
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
                command.CommandText = "INSERT INTO [OverledeneOpdrachtgever] ([Id], [uitvaartId], [opdrachtgeverAanhef], [opdrachtgeverAchternaam]," +
                        " [opdrachtgeverVoornaamen], [opdrachtgeverTussenvoegsel], [opdrachtgeverGeboortedatum], " +
                        "[opdrachtgeverLeeftijd], [opdrachtgeverStraat], [opdrachtgeverHuisnummer], [opdrachtgeverHuisnummerToevoeging], " +
                        "[opdrachtgeverPostcode], [opdrachtgeverWoonplaats],[opdrachtgeverGemeente], [opdrachtgeverTelefoon], [opdrachtgeverBSN], " +
                        "[opdrachtgeverRelatieTotOverledene],[opdrachtgeverExtraInfo],[opdrachtgeverEmail]) VALUES (@OpdrachtId, @UitvaartId, @Salutation_Family, @SurName_Family, " +
                        "@Firstname_Family, @Preposition_Family, @DateOfBirth_Family, @Age_Family, @Address_Family, " +
                        "@Housenumber_Family, @HousenumberAddition_Family, @PostalCode_Family, @City_Family, @County_Family, @PhoneNumber_Family, " +
                        "@BSN_Family, @RelationToDeceased_Family, @extraInformatie, @Email)";
                command.Parameters.AddWithValue("@OpdrachtId", opdrachtgeverPersoonsGegevensModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", opdrachtgeverPersoonsGegevensModel.UitvaartId);
                command.Parameters.AddWithValue("@Salutation_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef : DBNull.Value);
                command.Parameters.AddWithValue("@SurName_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam : DBNull.Value);
                command.Parameters.AddWithValue("@Firstname_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen : DBNull.Value);
                command.Parameters.AddWithValue("@Preposition_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel : DBNull.Value);
                command.Parameters.AddWithValue("@DateOfBirth_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum : DBNull.Value);
                command.Parameters.AddWithValue("@Age_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd : DBNull.Value);
                command.Parameters.AddWithValue("@Address_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat : DBNull.Value);
                command.Parameters.AddWithValue("@Housenumber_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer : DBNull.Value);
                command.Parameters.AddWithValue("@HousenumberAddition_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@PostalCode_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode : DBNull.Value);
                command.Parameters.AddWithValue("@City_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats : DBNull.Value);
                command.Parameters.AddWithValue("@County_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente : DBNull.Value);
                command.Parameters.AddWithValue("@PhoneNumber_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@BSN_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN : DBNull.Value);
                command.Parameters.AddWithValue("@RelationToDeceased_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene : DBNull.Value);
                command.Parameters.AddWithValue("@extraInformatie", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverExtraInformatie != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverExtraInformatie : DBNull.Value);
                command.Parameters.AddWithValue("@Email", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail : DBNull.Value);
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
                command.CommandText = "INSERT INTO [OverledeneExtraOpdrachtgever] ([Id], [uitvaartId], [opdrachtgeverAanhef], [opdrachtgeverAchternaam]," +
                        " [opdrachtgeverVoornaamen], [opdrachtgeverTussenvoegsel], [opdrachtgeverGeboortedatum], " +
                        "[opdrachtgeverLeeftijd], [opdrachtgeverStraat], [opdrachtgeverHuisnummer], [opdrachtgeverHuisnummerToevoeging], " +
                        "[opdrachtgeverPostcode], [opdrachtgeverWoonplaats],[opdrachtgeverGemeente], [opdrachtgeverTelefoon], [opdrachtgeverBSN], " +
                        "[opdrachtgeverRelatieTotOverledene],[opdrachtgeverEmail]) VALUES (@OpdrachtId, @UitvaartId, @Salutation_Family, @SurName_Family, " +
                        "@Firstname_Family, @Preposition_Family, @DateOfBirth_Family, @Age_Family, @Address_Family, " +
                        "@Housenumber_Family, @HousenumberAddition_Family, @PostalCode_Family, @City_Family, @County_Family, @PhoneNumber_Family, " +
                        "@BSN_Family, @RelationToDeceased_Family, @Email)";
                command.Parameters.AddWithValue("@OpdrachtId", opdrachtgeverPersoonsGegevensModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", opdrachtgeverPersoonsGegevensModel.UitvaartId);
                command.Parameters.AddWithValue("@Salutation_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef : DBNull.Value);
                command.Parameters.AddWithValue("@SurName_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam : DBNull.Value);
                command.Parameters.AddWithValue("@Firstname_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen : DBNull.Value);
                command.Parameters.AddWithValue("@Preposition_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel : DBNull.Value);
                command.Parameters.AddWithValue("@DateOfBirth_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum : DBNull.Value);
                command.Parameters.AddWithValue("@Age_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd : DBNull.Value);
                command.Parameters.AddWithValue("@Address_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat : DBNull.Value);
                command.Parameters.AddWithValue("@Housenumber_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer : DBNull.Value);
                command.Parameters.AddWithValue("@HousenumberAddition_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@PostalCode_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode : DBNull.Value);
                command.Parameters.AddWithValue("@City_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats : DBNull.Value);
                command.Parameters.AddWithValue("@County_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente : DBNull.Value);
                command.Parameters.AddWithValue("@PhoneNumber_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@BSN_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN : DBNull.Value);
                command.Parameters.AddWithValue("@RelationToDeceased_Family", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene : DBNull.Value);
                command.Parameters.AddWithValue("@Email", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertOpdrachtgeverExtraPersoonsGegevensFailed");
                }
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
        public void AddOpbaren(OverledeneOpbarenModel overledeneOpbarenModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO OverledeneOpbaring ([opbaringId], [uitvaartId], [opbaringLocatie],[opbaringKistId], [opbaringKistOmschrijving]," +
                                    "[opbaringKistLengte],[opbaringVerzorging],[opbaringVerzorgingJson],[opbaringKoeling],[opbaringKledingMee]," +
                                    "[opbaringKledingRetour],[opbaringSieraden],[opbaringSieradenOmschrijving],[opbaringSieradenRetour], [opbaringBezoek], [opbaringExtraInfo]) " +
                                    "VALUES (@OpbarenId, @UitvaartId, @Locatie, @KistId, @KistOmschrijving, @KistLengte, @Verzorging, @VerzorgingJson, @Koeling, @KledingMee, @KledingRetour," +
                                    " @Sieraden, @SieradenOmschrijving, @SieradenRetour, @Bezoek, @ExtraInfo)";
                command.Parameters.AddWithValue("@OpbarenId", overledeneOpbarenModel.OpbaringId);
                command.Parameters.AddWithValue("@UitvaartId", overledeneOpbarenModel.UitvaartId);
                command.Parameters.AddWithValue("@Locatie", overledeneOpbarenModel.OpbaringLocatie);
                command.Parameters.AddWithValue("@KistId", overledeneOpbarenModel.OpbaringKistId);
                command.Parameters.AddWithValue("@KistOmschrijving", overledeneOpbarenModel.OpbaringKistOmschrijving);
                command.Parameters.AddWithValue("@KistLengte", overledeneOpbarenModel.OpbaringKistLengte);
                command.Parameters.AddWithValue("@Verzorging", overledeneOpbarenModel.OpbaringVerzorging != null ? overledeneOpbarenModel.OpbaringVerzorging : DBNull.Value);
                command.Parameters.AddWithValue("@VerzorgingJson", overledeneOpbarenModel.OpbaringVerzorgingJson != null ? overledeneOpbarenModel.OpbaringVerzorgingJson : DBNull.Value);
                command.Parameters.AddWithValue("@Koeling", overledeneOpbarenModel.OpbaringKoeling != null ? overledeneOpbarenModel.OpbaringKoeling : DBNull.Value);
                command.Parameters.AddWithValue("@KledingMee", overledeneOpbarenModel.OpbaringKledingMee != null ? overledeneOpbarenModel.OpbaringKledingMee : DBNull.Value);
                command.Parameters.AddWithValue("@KledingRetour", overledeneOpbarenModel.OpbaringKledingRetour != null ? overledeneOpbarenModel.OpbaringKledingRetour : DBNull.Value);
                command.Parameters.AddWithValue("@Sieraden", overledeneOpbarenModel.OpbaringSieraden != null ? overledeneOpbarenModel.OpbaringSieraden : DBNull.Value);
                command.Parameters.AddWithValue("@SieradenOmschrijving", overledeneOpbarenModel.OpbaringSieradenOmschrijving != null ? overledeneOpbarenModel.OpbaringSieradenOmschrijving : DBNull.Value);
                command.Parameters.AddWithValue("@SieradenRetour", overledeneOpbarenModel.OpbaringSieradenRetour != null ? overledeneOpbarenModel.OpbaringSieradenRetour : DBNull.Value);
                command.Parameters.AddWithValue("@Bezoek", overledeneOpbarenModel.OpbaringBezoek != null ? overledeneOpbarenModel.OpbaringBezoek : DBNull.Value);
                command.Parameters.AddWithValue("@ExtraInfo", overledeneOpbarenModel.OpbaringExtraInfo != null ? overledeneOpbarenModel.OpbaringExtraInfo : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("InsertOpbarenFailed");
                }
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
                                    "[kostenbegrotingCreated],[kostenbegrotingVerzekeraar]) " +
                                    "VALUES (@FactuurId, @UitvaartId, @kostenbegrotingUrl, @kostenbegrotingJson, @kostenbegrotingCreationDate, @kostenbegrotingCreated,@verzekeraarId)";
                command.Parameters.AddWithValue("@FactuurId", overledeneKostenbegrotingModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", overledeneKostenbegrotingModel.UitvaartId);
                command.Parameters.AddWithValue("@kostenbegrotingUrl", overledeneKostenbegrotingModel.KostenbegrotingUrl);
                command.Parameters.AddWithValue("@kostenbegrotingJson", overledeneKostenbegrotingModel.KostenbegrotingJson);
                command.Parameters.AddWithValue("@kostenbegrotingCreationDate", DateTime.Now);
                command.Parameters.AddWithValue("@kostenbegrotingCreated", 1);
                command.Parameters.AddWithValue("@verzekeraarId", overledeneKostenbegrotingModel.KostenbegrotingVerzekeraar);
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
                command.CommandText = "INSERT INTO OverledeneKlantTevredenheid (Id, UitvaartId, Cijfer) VALUES (@id, @uitvaartId, @cijfer)";
                command.Parameters.AddWithValue("@id", klanttevredenheid.Id);
                command.Parameters.AddWithValue("@UitvaartId", klanttevredenheid.UitvaartId);
                command.Parameters.AddWithValue("@cijfer", klanttevredenheid.CijferScore);
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
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [ConfigurationFactuurComponent] (Id, ComponentId, Omschrijving, Bedrag, VerzekerdAantal, Verzekering,IsDeleted, SortOrder, factuurBedrag, DefaultPM) " +
                                        "VALUES (@id, @compid, @omschrijving, @bedrag, @aantal, @verzekering, 0, @sortOrder, @factuurBedrag, @defaultpm)";
                command.Parameters.AddWithValue("@id", priceComponent.Id);
                command.Parameters.AddWithValue("@compid", priceComponent.ComponentId);
                command.Parameters.AddWithValue("@omschrijving", priceComponent.ComponentOmschrijving);
                command.Parameters.AddWithValue("@aantal", priceComponent.ComponentAantal);
                command.Parameters.AddWithValue("@bedrag", priceComponent.ComponentBedrag);
                command.Parameters.AddWithValue("@factuurBedrag", priceComponent.ComponentFactuurBedrag);
                command.Parameters.AddWithValue("@verzekering", priceComponent.ComponentVerzekering);
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
                command.Parameters.AddWithValue("@Id", asbestemmingCreate.AsbestemmingId);
                command.Parameters.AddWithValue("@omschrijving", asbestemmingCreate.AsbestemmingOmschrijving);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("AsbestemmingCreateFailed");
                }
            }
        }
        public void VerzekeringCreate(VerzekeraarsModel verzekeringCreate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO ConfigurationVerzekeraar (Id, verzekeraarNaam, verzekeraarAfkorting, isHerkomst, isVerzekeraar, hasLidnummer, postbusAddress, postbusNaam, addressStreet, addressHousenumber, addressHousenumberAddition, addressZipcode,  addressCity, factuurType, correspondentieType, isDeleted, isPakket, OverrideFactuurAdress, verzekeraarTelefoon, CustomLogo) " +
                    "VALUES (@Id, @naam, @afkorting, @herkomst, @verzekeraar, @lidnummer, @postbusAdres, @postbusNaam, @street, @housenumber, @addition, @zipcode, @city, @factuurtype, @correspondentieType, 0, @isPakket, @isOverrideAddress, @telefoon, @customLogo)";
                command.Parameters.AddWithValue("@Id", verzekeringCreate.Id);
                command.Parameters.AddWithValue("@naam", verzekeringCreate.Name);
                command.Parameters.AddWithValue("@afkorting", verzekeringCreate.Afkorting);
                command.Parameters.AddWithValue("@herkomst", verzekeringCreate.IsHerkomst);
                command.Parameters.AddWithValue("@verzekeraar", verzekeringCreate.IsVerzekeraar);
                command.Parameters.AddWithValue("@lidnummer", verzekeringCreate.HasLidnummer);
                command.Parameters.AddWithValue("@postbusAdres", string.IsNullOrEmpty(verzekeringCreate.PostbusAddress) ? (object)DBNull.Value : verzekeringCreate.PostbusAddress);
                command.Parameters.AddWithValue("@postbusNaam", string.IsNullOrEmpty(verzekeringCreate.PostbusName) ? (object)DBNull.Value : verzekeringCreate.PostbusName);
                command.Parameters.AddWithValue("@street", string.IsNullOrEmpty(verzekeringCreate.AddressStreet) ? (object)DBNull.Value : verzekeringCreate.AddressStreet);
                command.Parameters.AddWithValue("@housenumber", string.IsNullOrEmpty(verzekeringCreate.AddressHousenumber) ? (object)DBNull.Value : verzekeringCreate.AddressHousenumber);
                command.Parameters.AddWithValue("@addition", string.IsNullOrEmpty(verzekeringCreate.AddressHousenumberAddition) ? (object)DBNull.Value : verzekeringCreate.AddressHousenumberAddition);
                command.Parameters.AddWithValue("@zipcode", string.IsNullOrEmpty(verzekeringCreate.AddressZipCode) ? (object)DBNull.Value : verzekeringCreate.AddressZipCode);
                command.Parameters.AddWithValue("@city", string.IsNullOrEmpty(verzekeringCreate.AddressCity) ? (object)DBNull.Value : verzekeringCreate.AddressCity);
                command.Parameters.AddWithValue("@factuurtype", string.IsNullOrEmpty(verzekeringCreate.FactuurType) ? (object)DBNull.Value : verzekeringCreate.FactuurType);
                command.Parameters.AddWithValue("@correspondentieType", string.IsNullOrEmpty(verzekeringCreate.CorrespondentieType) ? (object)DBNull.Value : verzekeringCreate.CorrespondentieType);
                command.Parameters.AddWithValue("@isPakket", verzekeringCreate.Pakket);
                command.Parameters.AddWithValue("@isOverrideAddress", verzekeringCreate.IsOverrideFactuurAdress);
                command.Parameters.AddWithValue("@telefoon", string.IsNullOrEmpty(verzekeringCreate.Telefoon) ? (object)DBNull.Value : verzekeringCreate.Telefoon);
                command.Parameters.AddWithValue("@customLogo", verzekeringCreate.CustomLogo);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("CreateVerzekeringFailed");
                }
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
