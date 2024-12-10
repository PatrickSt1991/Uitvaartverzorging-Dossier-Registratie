using Dossier_Registratie.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dossier_Registratie.Repositories
{
    public class UpdateOperations : RepositoryBase, IUpdateOperations
    {
        public async Task UpdateNotification(Guid uitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = "UPDATE [EeftingDossierRegistratieRik].[dbo].[OverledeneKlantTevredenheid] " +
                                        "SET NotificatieOverleden = 0 " +
                                        "WHERE UitvaartId = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", uitvaartId);

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdateBlobLogo(string imageName, string imageType, byte[] imageData, string appType)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = "UPDATE ConfigurationBlob " +
                                        "SET DocumentName = @DocumentName, " +
                                        "DocumentType = @DocumentType, " +
                                        "DocumentData = @DocumentData, " +
                                        "UploadDate = GETDATE() " +
                                        "WHERE AppType = @AppType";
                command.Parameters.AddWithValue("@DocumentName", imageName);
                command.Parameters.AddWithValue("@DocumentType", imageType);
                command.Parameters.AddWithValue("@DocumentData", imageData);
                command.Parameters.AddWithValue("@AppType", appType);

                await command.ExecuteNonQueryAsync();
            }

        }
        public void SetDocumentInconsistent(Guid DocumentId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneBijlages] " +
                                        "SET DocumentInconsistent = 1 " +
                                        "WHERE BijlageId = @DocumentId";
                command.Parameters.AddWithValue("@DocumentId", DocumentId);

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateDocumentInconsistentFailed");
                }
            }
        }
        public void UpdateLeverancier(LeveranciersModel leverancier)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationLeveranciers] " +
                                        "SET leverancierName = @Name, " +
                                        "leverancierBeschrijving = @Beschrijving, " +
                                        "steenhouwer = @Steen, " +
                                        "bloemist = @Bloem, " +
                                        "kisten = @kisten, " +
                                        "urnsieraden = @urn " +
                                        "WHERE leverancierId = @id";
                command.Parameters.AddWithValue("@id", leverancier.LeverancierId);
                command.Parameters.AddWithValue("@Beschrijving", leverancier.LeverancierBeschrijving);
                command.Parameters.AddWithValue("@Name", leverancier.LeverancierName);
                command.Parameters.AddWithValue("@Steen", leverancier.Steenhouwer);
                command.Parameters.AddWithValue("@Bloem", leverancier.Bloemist);
                command.Parameters.AddWithValue("@kisten", leverancier.Kisten);
                command.Parameters.AddWithValue("@urn", leverancier.UrnSieraden);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateLeverancierFailed");
                }
            }
        }
        public void UpdateRouwbrief(OverledeneRouwbrieven rouwbrief)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationRouwbrieven] " +
                                        "SET rouwbrievenName = @Name " +
                                        "WHERE rouwbrievenId = @id";
                command.Parameters.AddWithValue("@id", rouwbrief.RouwbrievenId);
                command.Parameters.AddWithValue("@Name", rouwbrief.RouwbrievenName);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateRouwbriefFailed");
                }
            }
        }
        public void EditRechten(Guid employeeId, Guid rechtenId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = @"
        SELECT 
            CASE 
                WHEN COUNT(1) = 0 THEN 'NotFound'
                WHEN MAX(PermissionId) = @permissionId THEN 'NoChangeNeeded'
                ELSE 'UpdateNeeded'
            END
        FROM [ConfigurationPersoneelPermission] 
        WHERE PersoneelId = @employeeId";
                command.Parameters.Clear(); // Clear previous parameters
                command.Parameters.AddWithValue("@employeeId", employeeId);
                command.Parameters.AddWithValue("@permissionId", rechtenId);

                string result = (string)command.ExecuteScalar();

                if (result == "NotFound")
                {
                    throw new InvalidOperationException("EmployeeIdNotFound");
                }
                else if (result == "NoChangeNeeded")
                {
                    // No need to update if the permission is already set to the desired value
                    return; // Or handle it as needed
                }
                else if (result == "UpdateNeeded")
                {
                    // Proceed with the update
                    command.CommandText = "UPDATE [ConfigurationPersoneelPermission] SET PermissionId = @permissionId WHERE PersoneelId = @employeeId";
                    command.Parameters.Clear(); // Clear previous parameters
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@permissionId", rechtenId);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException("RechtenUpdatingFailed");
                    }
                }
            }
        }
        public void EditUitvaartleider(OverledeneUitvaartleiderModel uitvaartleiderModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneUitvaartleider] SET [PersoneelId] = @UitvaartLeiderId, [Uitvaartnummer] = @Uitvaartnummer WHERE [UitvaartId] = @UitvaartGuid";
                command.Parameters.AddWithValue("@UitvaartLeiderId", uitvaartleiderModel.PersoneelId);
                command.Parameters.AddWithValue("@Uitvaartnummer", uitvaartleiderModel.Uitvaartnummer);
                command.Parameters.AddWithValue("@UitvaartGuid", uitvaartleiderModel.UitvaartId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateUitvaartLeiderFailed");
                }
            }
        }
        public void EditMiscUitvaart(OverledeneMiscModel uitvaartMisc)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneUitvaartInfoMisc] " +
                                        "SET [AantalRouwbrieven] = @aantalrouwbrieven, " +
                                        "[AantalUitnodigingen] = @aantaluitnodiging, " +
                                        "[AantalKennisgeving] = @aantalkennisgeving, " +
                                        "[Advertenties] = @advertenties, " +
                                        "[UBS] = @ubs, " +
                                        "[AulaNaam] = @aulanaam, " +
                                        "[AantalPersonen] = @aantalpersonen, " +
                                        "[BegraafplaatsLocatie] = @begraafplaats, " +
                                        "[BegraafplaatsGrafNr] = @grafnummer " +
                                        "WHERE [UitvaartId] = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", uitvaartMisc.UitvaartId);
                command.Parameters.AddWithValue("@aantalrouwbrieven", uitvaartMisc.AantalRouwbrieven != null ? uitvaartMisc.AantalRouwbrieven : string.Empty);
                command.Parameters.AddWithValue("@aantaluitnodiging", uitvaartMisc.AantalUitnodigingen != null ? uitvaartMisc.AantalUitnodigingen : string.Empty);
                command.Parameters.AddWithValue("@aantalkennisgeving", uitvaartMisc.AantalKennisgeving != null ? uitvaartMisc.AantalKennisgeving : string.Empty);
                command.Parameters.AddWithValue("@advertenties", uitvaartMisc.Advertenties != null ? uitvaartMisc.Advertenties : string.Empty);
                command.Parameters.AddWithValue("@ubs", uitvaartMisc.UBS != null ? uitvaartMisc.UBS : '0');
                command.Parameters.AddWithValue("@aulanaam", uitvaartMisc.AulaNaam != null ? uitvaartMisc.AulaNaam : string.Empty);
                command.Parameters.AddWithValue("@aantalpersonen", uitvaartMisc.AulaPersonen != null ? uitvaartMisc.AulaPersonen : 0);
                command.Parameters.AddWithValue("@begraafplaats", uitvaartMisc.Begraafplaats != null ? uitvaartMisc.Begraafplaats : string.Empty);
                command.Parameters.AddWithValue("@grafnummer", uitvaartMisc.GrafNummer != null ? uitvaartMisc.GrafNummer : string.Empty);
                if (command.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("UpdateUitvaartMiscFailed");
            }
        }
        public void EditPersoonsGegevens(OverledenePersoonsGegevensModel persoonsGegevensModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledenePersoonsGegevens] SET [overledeneAchternaam] = @Achternaam, [overledeneTussenvoegsel] = @Tussenvoegsel, " +
                                        "[overledeneVoornamen] = @Voornamen, [overledeneAanhef] = @Aanhef, " +
                                        "[overledeneGeboortedatum] = @GeboorteDatum, [overledeneGeboorteplaats] = @GeboortePlaats, " +
                                        "[overledeneGemeente] = @Gemeente, [overledeneLeeftijd] = @Leeftijd, " +
                                        "[overledeneBSN] = @BSN, [overledeneAdres] = @Adres, " +
                                        "[overledeneHuisnummer] = @Huisnummer, [overledeneHuisnummerToevoeging] = @HuisnummerToevoeging, " +
                                        "[overledenePostcode] = @Postcode, [overledeneWoonplaats] = @Woonplaats, " +
                                        "[overledeneVoorregeling] = @Voorregeling WHERE [UitvaartId] = @UitvaartGuid";

                command.Parameters.AddWithValue("@Achternaam", persoonsGegevensModel.OverledeneAchternaam);
                command.Parameters.AddWithValue("@Tussenvoegsel", persoonsGegevensModel.OverledeneTussenvoegsel != null ? persoonsGegevensModel.OverledeneTussenvoegsel : DBNull.Value);
                command.Parameters.AddWithValue("@Voornamen", persoonsGegevensModel.OverledeneVoornamen);
                command.Parameters.AddWithValue("@Aanhef", persoonsGegevensModel.OverledeneAanhef);
                command.Parameters.AddWithValue("@GeboorteDatum", persoonsGegevensModel.OverledeneGeboortedatum);
                command.Parameters.AddWithValue("@GeboortePlaats", persoonsGegevensModel.OverledeneGeboorteplaats);
                command.Parameters.AddWithValue("@Gemeente", persoonsGegevensModel.OverledeneGemeente);
                command.Parameters.AddWithValue("@Leeftijd", persoonsGegevensModel.OverledeneLeeftijd);
                command.Parameters.AddWithValue("@BSN", persoonsGegevensModel.OverledeneBSN);
                command.Parameters.AddWithValue("@Adres", persoonsGegevensModel.OverledeneAdres);
                command.Parameters.AddWithValue("@Huisnummer", persoonsGegevensModel.OverledeneHuisnummer);
                command.Parameters.AddWithValue("@HuisnummerToevoeging", persoonsGegevensModel.OverledeneHuisnummerToevoeging != null ? persoonsGegevensModel.OverledeneHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@Postcode", persoonsGegevensModel.OverledenePostcode);
                command.Parameters.AddWithValue("@Woonplaats", persoonsGegevensModel.OverledeneWoonplaats);
                command.Parameters.AddWithValue("@Voorregeling", persoonsGegevensModel.OverledeneVoorregeling ? persoonsGegevensModel.OverledeneVoorregeling : DBNull.Value);
                command.Parameters.AddWithValue("@UitvaartGuid", persoonsGegevensModel.UitvaartId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdatePersoonsGegevensFailed");
                }
            }
        }
        public void EditOverlijdenInfo(OverledeneOverlijdenInfoModel overlijdenInfoModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneOverlijdenInfo] " +
                                        "SET [overledenDatumTijd] = @DatumOverledenTijd, [overledenAdres] = @Adres, " +
                                        "[overledenHuisnummer] = @Huisnummer,[overledenHuisnummerToevoeging] = @HuisnummerToevoeging, " +
                                        "[overledenPlaats] = @Plaats,[overledenGemeente] = @Gemeente, " +
                                        "[overledenLijkvinding] = @Lijkvinding,[overledenHerkomst] = @Herkomst, " +
                                        "[overledenLidnummer] = @Lidnummer,[overledenHuisarts] = @Huisarts, " +
                                        "[overledenHuisartsTelefoon] = @HuisartsTelefoon,[overledenSchouwarts] = @Schouwarts, " +
                                        "[overledenPostcode] = @Postcode, " +
                                        "[overledenLocatie] = @Locatie " +
                                        "WHERE uitvaartId = @UitvaartGuid";
                command.Parameters.AddWithValue("@UitvaartGuid", overlijdenInfoModel.UitvaartId);
                command.Parameters.AddWithValue("@DatumOverledenTijd", overlijdenInfoModel.OverledenDatumTijd);
                command.Parameters.AddWithValue("@Adres", overlijdenInfoModel.OverledenAdres);
                command.Parameters.AddWithValue("@Huisnummer", overlijdenInfoModel.OverledenHuisnummer);
                command.Parameters.AddWithValue("@HuisnummerToevoeging", overlijdenInfoModel.OverledenHuisnummerToevoeging != null ? overlijdenInfoModel.OverledenHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@Plaats", overlijdenInfoModel.OverledenPlaats);
                command.Parameters.AddWithValue("@Gemeente", overlijdenInfoModel.OverledenGemeente);
                command.Parameters.AddWithValue("@Lijkvinding", overlijdenInfoModel.OverledenLijkvinding);
                command.Parameters.AddWithValue("@Herkomst", overlijdenInfoModel.OverledenHerkomst);
                command.Parameters.AddWithValue("@Lidnummer", overlijdenInfoModel.OverledenLidnummer != null ? overlijdenInfoModel.OverledenLidnummer : DBNull.Value);
                command.Parameters.AddWithValue("@Huisarts", overlijdenInfoModel.OverledenHuisarts != null ? overlijdenInfoModel.OverledenHuisarts : DBNull.Value);
                command.Parameters.AddWithValue("@HuisartsTelefoon", overlijdenInfoModel.OverledenHuisartsTelefoon != null ? overlijdenInfoModel.OverledenHuisartsTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@Schouwarts", overlijdenInfoModel.OverledenSchouwarts != null ? overlijdenInfoModel.OverledenSchouwarts : DBNull.Value);
                command.Parameters.AddWithValue("@Postcode", overlijdenInfoModel.OverledenPostcode != null ? overlijdenInfoModel.OverledenPostcode : DBNull.Value);
                command.Parameters.AddWithValue("@Locatie", overlijdenInfoModel.OverledenLocatie != null ? overlijdenInfoModel.OverledenLocatie : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateOverlijdenInfoFailed");
                }
            }
        }
        public void EditOverlijdenExtraInfo(OverledeneExtraInfoModel overledeneExtraInfoModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneExtraInfo] SET [overledeneBurgelijkestaat] = @BurgelijkeStaat, [overledeneGescheidenVan] = @GescheidenVan, " +
                                        "[overledeneWedenaarVan] = @WeduwenaarVan, [overledeneTrouwboekje] = @TrouwBoekje, [overledeneAantalKinderen] = @AantalKinderen, " +
                                        "[overledeneAantalKinderenOverleden] = @KinderenOverleden, [overledeneKinderenMinderjarig] = @Minderjarig, [overledeneEersteOuder] = @EersteOuder, " +
                                        "[overledeneEersteOuderOverleden] = @EersteOuderOverleden, [overledeneTweedeOuderOverleden] = @TweedeOuderOverleden, [overledeneTweedeOuder] = @TweedeOuder, " +
                                        "[overledeneLevensovertuiging] = @Levensovertuiging, [overledeneExecuteur] = @Executeur, [overledeneExecuteurTelefoon] = @ExecuteurTelefoon, " +
                                        "[overledeneNotaris] = @Notaris, [overledeneNotarisTelefoon] = @NotarisTelefoon, [overledeneTestament] = @Testament, " +
                                        "[overledeneTrouwDatumTijd] = @TrouwDatumTijd, [overledeneGeregistreerdDatumTijd] = @PartnerDatumTijd, " +
                                        "[naamWederhelft] = @naamWederhelft, [voornaamWederhelft] = @voornaamWederhelft " +
                                        "WHERE UitvaartId = @UitvaartGuid";

                command.Parameters.AddWithValue("@BurgelijkeStaat", overledeneExtraInfoModel.OverledeneBurgelijkestaat != null ? overledeneExtraInfoModel.OverledeneBurgelijkestaat : DBNull.Value);
                command.Parameters.AddWithValue("@GescheidenVan", overledeneExtraInfoModel.OverledeneGescheidenVan != null ? overledeneExtraInfoModel.OverledeneGescheidenVan : DBNull.Value);
                command.Parameters.AddWithValue("@WeduwenaarVan", overledeneExtraInfoModel.OverledeneWedenaarVan != null ? overledeneExtraInfoModel.OverledeneWedenaarVan : DBNull.Value);
                command.Parameters.AddWithValue("@TrouwBoekje", overledeneExtraInfoModel.OverledeneTrouwboekje != null ? overledeneExtraInfoModel.OverledeneTrouwboekje : DBNull.Value);
                command.Parameters.AddWithValue("@AantalKinderen", overledeneExtraInfoModel.OverledeneAantalKinderen != null ? overledeneExtraInfoModel.OverledeneAantalKinderen : DBNull.Value);
                command.Parameters.AddWithValue("@KinderenOverleden", overledeneExtraInfoModel.OverledeneKinderenMinderjarigOverleden != null ? overledeneExtraInfoModel.OverledeneKinderenMinderjarigOverleden : DBNull.Value);
                command.Parameters.AddWithValue("@Minderjarig", overledeneExtraInfoModel.OverledeneKinderenMinderjarig != null ? overledeneExtraInfoModel.OverledeneKinderenMinderjarig : DBNull.Value);
                command.Parameters.AddWithValue("@EersteOuder", overledeneExtraInfoModel.OverledeneEersteOuder != null ? overledeneExtraInfoModel.OverledeneEersteOuder : DBNull.Value);
                command.Parameters.AddWithValue("@EersteOuderOverleden", overledeneExtraInfoModel.OverledeneEersteOuderOverleden != null ? overledeneExtraInfoModel.OverledeneEersteOuderOverleden : DBNull.Value);
                command.Parameters.AddWithValue("@TweedeOuder", overledeneExtraInfoModel.OverledeneTweedeOuder != null ? overledeneExtraInfoModel.OverledeneTweedeOuder : DBNull.Value);
                command.Parameters.AddWithValue("@TweedeOuderOverleden", overledeneExtraInfoModel.OverledeneTweedeOuderOverleden != null ? overledeneExtraInfoModel.OverledeneTweedeOuderOverleden : DBNull.Value);
                command.Parameters.AddWithValue("@Levensovertuiging", overledeneExtraInfoModel.OverledeneLevensovertuiging != null ? overledeneExtraInfoModel.OverledeneLevensovertuiging : DBNull.Value);
                command.Parameters.AddWithValue("@Executeur", overledeneExtraInfoModel.OverledeneExecuteur != null ? overledeneExtraInfoModel.OverledeneExecuteur : DBNull.Value);
                command.Parameters.AddWithValue("@ExecuteurTelefoon", overledeneExtraInfoModel.OverledeneExecuteurTelefoon != null ? overledeneExtraInfoModel.OverledeneExecuteurTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@Notaris", overledeneExtraInfoModel.OverledeneNotaris != null ? overledeneExtraInfoModel.OverledeneNotaris : DBNull.Value);
                command.Parameters.AddWithValue("@NotarisTelefoon", overledeneExtraInfoModel.OverledeneNotarisTelefoon != null ? overledeneExtraInfoModel.OverledeneNotarisTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@Testament", overledeneExtraInfoModel.OverledeneTestament != null ? overledeneExtraInfoModel.OverledeneTestament : DBNull.Value);
                command.Parameters.AddWithValue("@voornaamWederhelft", overledeneExtraInfoModel.VoornaamWederhelft != null ? overledeneExtraInfoModel.VoornaamWederhelft : DBNull.Value);
                command.Parameters.AddWithValue("@naamWederhelft", overledeneExtraInfoModel.NaamWederhelft != null ? overledeneExtraInfoModel.NaamWederhelft : DBNull.Value);
                command.Parameters.Add(new SqlParameter("@TrouwDatumTijd", SqlDbType.DateTime)
                {
                    Value = overledeneExtraInfoModel.OverledeneTrouwDatumTijd.HasValue
                                ? (object)overledeneExtraInfoModel.OverledeneTrouwDatumTijd.Value
                                : DBNull.Value
                });

                command.Parameters.Add(new SqlParameter("@PartnerDatumTijd", SqlDbType.DateTime)
                {
                    Value = overledeneExtraInfoModel.OverledeneGeregistreerdDatumTijd.HasValue
                                ? (object)overledeneExtraInfoModel.OverledeneGeregistreerdDatumTijd.Value
                                : DBNull.Value
                });
                command.Parameters.AddWithValue("@UitvaartGuid", overledeneExtraInfoModel.UitvaartId);

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateOverledeneExtraInfoFailed");
                }
            }
        }
        public void EditOpdrachtgeverPersoonsGegevens(OpdrachtgeverPersoonsGegevensModel opdrachtgeverPersoonsGegevensModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneOpdrachtgever] SET[opdrachtgeverAanhef] = @Aanhef, " +
                                        "[opdrachtgeverAchternaam] = @Achternaam, [opdrachtgeverVoornaamen] = @Voornamen, " +
                                        "[opdrachtgeverTussenvoegsel] = @Tussenvoegsel, [opdrachtgeverGeboortedatum]= @GeboorteDatum, " +
                                        "[opdrachtgeverLeeftijd] = @Leeftijd, " +
                                        "[opdrachtgeverStraat] = @Adres, [opdrachtgeverHuisnummer] = @Huisnummer, " +
                                        "[opdrachtgeverHuisnummerToevoeging] = @HuisnummerToevoeging, [opdrachtgeverPostcode] = @Postcode, " +
                                        "[opdrachtgeverWoonplaats] =  @Woonplaats, [opdrachtgeverGemeente] = @Gemeente, " +
                                        "[opdrachtgeverTelefoon] = @Telefoon, [opdrachtgeverBSN] = @BSN, " +
                                        "[opdrachtgeverRelatieTotOverledene] = @RelatieTotOverledene," +
                                        "[opdrachtgeverExtraInfo] = @ExtraInformatie, " +
                                        "[opdrachtgeverEmail] = @Email " +
                                        "WHERE uitvaartId = @UitvaartGuid";

                command.Parameters.AddWithValue("@Aanhef", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef : DBNull.Value);
                command.Parameters.AddWithValue("@Achternaam", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam : DBNull.Value);
                command.Parameters.AddWithValue("@Voornamen", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen : DBNull.Value);
                command.Parameters.AddWithValue("@Tussenvoegsel", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel : DBNull.Value);
                command.Parameters.AddWithValue("@GeboorteDatum", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum : DBNull.Value);
                command.Parameters.AddWithValue("@Leeftijd", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd : DBNull.Value);
                command.Parameters.AddWithValue("@Adres", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat : DBNull.Value);
                command.Parameters.AddWithValue("@Huisnummer", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer : DBNull.Value);
                command.Parameters.AddWithValue("@HuisnummerToevoeging", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@Postcode", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode : DBNull.Value);
                command.Parameters.AddWithValue("@Woonplaats", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats : DBNull.Value);
                command.Parameters.AddWithValue("@Gemeente", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente : DBNull.Value);
                command.Parameters.AddWithValue("@Telefoon", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@BSN", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN : DBNull.Value);
                command.Parameters.AddWithValue("@RelatieTotOverledene", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene : DBNull.Value);
                command.Parameters.AddWithValue("@ExtraInformatie", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverExtraInformatie != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverExtraInformatie : DBNull.Value);
                command.Parameters.AddWithValue("@UitvaartGuid", opdrachtgeverPersoonsGegevensModel.UitvaartId);
                command.Parameters.AddWithValue("@Email", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateOpdrachtgeverPersoonsGegevensFailed");
                }
            }
        }
        public void EditOpdrachtgeverExtraPersoonsGegevens(OpdrachtgeverPersoonsGegevensModel opdrachtgeverPersoonsGegevensModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneExtraOpdrachtgever] SET[opdrachtgeverAanhef] = @Aanhef, " +
                                        "[opdrachtgeverAchternaam] = @Achternaam, [opdrachtgeverVoornaamen] = @Voornamen, " +
                                        "[opdrachtgeverTussenvoegsel] = @Tussenvoegsel, [opdrachtgeverGeboortedatum]= @GeboorteDatum, " +
                                        "[opdrachtgeverLeeftijd] = @Leeftijd, " +
                                        "[opdrachtgeverStraat] = @Adres, [opdrachtgeverHuisnummer] = @Huisnummer, " +
                                        "[opdrachtgeverHuisnummerToevoeging] = @HuisnummerToevoeging, [opdrachtgeverPostcode] = @Postcode, " +
                                        "[opdrachtgeverWoonplaats] =  @Woonplaats, [opdrachtgeverGemeente] = @Gemeente, " +
                                        "[opdrachtgeverTelefoon] = @Telefoon, [opdrachtgeverBSN] = @BSN, " +
                                        "[opdrachtgeverRelatieTotOverledene] = @RelatieTotOverledene," +
                                        "[opdrachtgeverEmail] = @Email " +
                                        "WHERE uitvaartId = @UitvaartGuid";

                command.Parameters.AddWithValue("@Aanhef", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAanhef : DBNull.Value);
                command.Parameters.AddWithValue("@Achternaam", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverAchternaam : DBNull.Value);
                command.Parameters.AddWithValue("@Voornamen", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverVoornaamen : DBNull.Value);
                command.Parameters.AddWithValue("@Tussenvoegsel", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTussenvoegsel : DBNull.Value);
                command.Parameters.AddWithValue("@GeboorteDatum", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGeboortedatum : DBNull.Value);
                command.Parameters.AddWithValue("@Leeftijd", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverLeeftijd : DBNull.Value);
                command.Parameters.AddWithValue("@Adres", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverStraat : DBNull.Value);
                command.Parameters.AddWithValue("@Huisnummer", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummer : DBNull.Value);
                command.Parameters.AddWithValue("@HuisnummerToevoeging", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverHuisnummerToevoeging : DBNull.Value);
                command.Parameters.AddWithValue("@Postcode", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverPostcode : DBNull.Value);
                command.Parameters.AddWithValue("@Woonplaats", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverWoonplaats : DBNull.Value);
                command.Parameters.AddWithValue("@Gemeente", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverGemeente : DBNull.Value);
                command.Parameters.AddWithValue("@Telefoon", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverTelefoon : DBNull.Value);
                command.Parameters.AddWithValue("@BSN", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverBSN : DBNull.Value);
                command.Parameters.AddWithValue("@RelatieTotOverledene", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverRelatieTotOverledene : DBNull.Value);
                command.Parameters.AddWithValue("@UitvaartGuid", opdrachtgeverPersoonsGegevensModel.UitvaartId);
                command.Parameters.AddWithValue("@Email", opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail != null ? opdrachtgeverPersoonsGegevensModel.OpdrachtgeverEmail : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateOpdrachtgeverPersoonsGegevensFailed");
                }
            }
        }
        public void EditVerzekering(OverledeneVerzekeringModel overledeneVerzekeringModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneVerzerkeringInfo] SET [verzekeringProperties] = @verzekeringProperties WHERE uitvaartId = @UitvaartGuid";
                command.Parameters.AddWithValue("@verzekeringProperties", overledeneVerzekeringModel.VerzekeringProperties);
                command.Parameters.AddWithValue("@UitvaartGuid", overledeneVerzekeringModel.UitvaartId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateVerzekeringFailed");
                }
            }
        }
        public void EditOpbaren(OverledeneOpbarenModel overledeneOpbarenModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneOpbaring] " +
                                        "SET [opbaringLocatie] = @Locatie, " +
                                        "[opbaringKistId] = @KistId, " +
                                        "[opbaringKistOmschrijving] = @KistOmschrijving, " +
                                        "[opbaringKistLengte] = @KistLengte, " +
                                        "[opbaringVerzorging] = @Verzorging, " +
                                        "[opbaringVerzorgingJson] = @verzorgingJson," +
                                        "[opbaringKoeling] = @Koeling, " +
                                        "[opbaringKledingMee] = @KledingMee, " +
                                        "[opbaringKledingRetour] = @KledingRetour, " +
                                        "[opbaringSieraden] = @SieradenMee, " +
                                        "[opbaringSieradenOmschrijving] = @SieradenOmschrijving, " +
                                        "[opbaringSieradenRetour] = @SieradenRetour, " +
                                        "[opbaringBezoek] = @Bezoek," +
                                        "[opbaringExtraInfo] = @ExtraInfo " +
                                        "WHERE UitvaartId= @UitvaartGuid";
                command.Parameters.AddWithValue("@Locatie", overledeneOpbarenModel.OpbaringLocatie);
                command.Parameters.AddWithValue("@KistId", overledeneOpbarenModel.OpbaringKistId);
                command.Parameters.AddWithValue("@KistOmschrijving", overledeneOpbarenModel.OpbaringKistOmschrijving);
                command.Parameters.AddWithValue("@KistLengte", overledeneOpbarenModel.OpbaringKistLengte);
                command.Parameters.AddWithValue("@Verzorging", overledeneOpbarenModel.OpbaringVerzorging != null ? overledeneOpbarenModel.OpbaringVerzorging : DBNull.Value);
                command.Parameters.AddWithValue("@verzorgingJson", overledeneOpbarenModel.OpbaringVerzorgingJson != null ? overledeneOpbarenModel.OpbaringVerzorgingJson : DBNull.Value);
                command.Parameters.AddWithValue("@Koeling", overledeneOpbarenModel.OpbaringKoeling != null ? overledeneOpbarenModel.OpbaringKoeling : DBNull.Value);
                command.Parameters.AddWithValue("@KledingMee", overledeneOpbarenModel.OpbaringKledingMee != null ? overledeneOpbarenModel.OpbaringKledingMee : DBNull.Value);
                command.Parameters.AddWithValue("@KledingRetour", overledeneOpbarenModel.OpbaringKledingRetour != null ? overledeneOpbarenModel.OpbaringKledingRetour : DBNull.Value);
                command.Parameters.AddWithValue("@SieradenMee", overledeneOpbarenModel.OpbaringSieraden != null ? overledeneOpbarenModel.OpbaringSieraden : DBNull.Value);
                command.Parameters.AddWithValue("@SieradenOmschrijving", overledeneOpbarenModel.OpbaringSieradenOmschrijving != null ? overledeneOpbarenModel.OpbaringSieradenOmschrijving : DBNull.Value);
                command.Parameters.AddWithValue("@SieradenRetour", overledeneOpbarenModel.OpbaringSieradenRetour != null ? overledeneOpbarenModel.OpbaringSieradenRetour : DBNull.Value);
                command.Parameters.AddWithValue("@Bezoek", overledeneOpbarenModel.OpbaringBezoek != null ? overledeneOpbarenModel.OpbaringBezoek : DBNull.Value);
                command.Parameters.AddWithValue("@ExtraInfo", overledeneOpbarenModel.OpbaringExtraInfo != null ? overledeneOpbarenModel.OpbaringExtraInfo : DBNull.Value);
                command.Parameters.AddWithValue("@UitvaartGuid", overledeneOpbarenModel.UitvaartId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateOpbarenFailed");
                }
            }
        }
        public void EditUitvaart(OverledeneUitvaartModel overledeneUitvaartModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneUitvaartInfo] " +
                                        "SET [uitvaartInfoDatumTijdCondoleance] = @datumTijdCondoleance," +
                                        "[uitvaartInfoCondoleanceConsumpties] = @consumptiesCondoleance, " +
                                        "[uitvaartInfoType] = @Type, " +
                                        "[uitvaartInfoDatumTijdUitvaart] = @datumTijdUitvaart, " +
                                        "[uitvaartInfoUitvaartLocatie] = @locatieUitvaart, " +
                                        "[uitvaartInfoDienstDatumTijd] = @datumTijdDienst, " +
                                        "[uitvaartInfoDienstLocatie] = @locatieDienst, " +
                                        "[uitvaartInfoDienstAfscheid] = @afscheidDienst, " +
                                        "[uitvaartInfoDienstMuziek] = @muziekDienst, " +
                                        "[uitvaartInfoDienstBesloten] = @beslotenDienst, " +
                                        "[uitvaartInfoDienstVolgauto] = @dienstVolgautos, " +
                                        "[uitvaartInfoDienstConsumpties] = @consumptiesDienst, " +
                                        "[uitvaartInfoDienstKist] = @dienstKist," +
                                        "[uitvaartInfoCondoleanceYesNo] = @condoleanceYesNo," +
                                        "[uitvaartInfoSpreker] = @spreker," +
                                        "[uitvaartInfoPowerPoint] = @powerpoint," +
                                        "[uitvaartTijdBlokken] = @tijdblokken," +
                                        "[uitvaartAantalTijdsBlokken] = @aantaltijdblokken " +
                                        "WHERE [Id] = @OpbarenId " +
                                        "AND [uitvaartId] = @UitvaartId";
                command.Parameters.AddWithValue("@condoleanceYesNo", overledeneUitvaartModel.CondoleanceYesNo != null ? overledeneUitvaartModel.CondoleanceYesNo : DBNull.Value);
                command.Parameters.AddWithValue("@spreker", overledeneUitvaartModel.Spreker != null ? overledeneUitvaartModel.Spreker : DBNull.Value);
                command.Parameters.AddWithValue("@powerpoint", overledeneUitvaartModel.PowerPoint != null ? overledeneUitvaartModel.PowerPoint : DBNull.Value);
                command.Parameters.AddWithValue("@datumTijdCondoleance", overledeneUitvaartModel.DatumTijdCondoleance != null ? overledeneUitvaartModel.DatumTijdCondoleance : DBNull.Value);
                command.Parameters.AddWithValue("@consumptiesCondoleance", overledeneUitvaartModel.ConsumptiesCondoleance != null ? overledeneUitvaartModel.ConsumptiesCondoleance : DBNull.Value);
                command.Parameters.AddWithValue("@Type", overledeneUitvaartModel.TypeDienst);
                command.Parameters.AddWithValue("@datumTijdUitvaart", overledeneUitvaartModel.DatumTijdUitvaart != null ? overledeneUitvaartModel.DatumTijdUitvaart : DBNull.Value);
                command.Parameters.AddWithValue("@locatieUitvaart", overledeneUitvaartModel.LocatieUitvaart);
                command.Parameters.AddWithValue("@datumTijdDienst", overledeneUitvaartModel.DatumTijdDienst != null ? overledeneUitvaartModel.DatumTijdDienst : DBNull.Value);
                command.Parameters.AddWithValue("@locatieDienst", overledeneUitvaartModel.LocatieDienst);
                command.Parameters.AddWithValue("@afscheidDienst", overledeneUitvaartModel.AfscheidDienst);
                command.Parameters.AddWithValue("@muziekDienst", overledeneUitvaartModel.MuziekDienst);
                command.Parameters.AddWithValue("@beslotenDienst", overledeneUitvaartModel.BeslotenDienst);
                command.Parameters.AddWithValue("@dienstVolgautos", overledeneUitvaartModel.VolgAutoDienst);
                command.Parameters.AddWithValue("@consumptiesDienst", overledeneUitvaartModel.ConsumptiesDienst != null ? overledeneUitvaartModel.ConsumptiesDienst : DBNull.Value);
                command.Parameters.AddWithValue("@dienstKist", overledeneUitvaartModel.KistDienst);
                command.Parameters.AddWithValue("@tijdblokken", overledeneUitvaartModel.TijdBlokken != null ? overledeneUitvaartModel.TijdBlokken : DBNull.Value);
                command.Parameters.AddWithValue("@aantaltijdblokken", overledeneUitvaartModel.AantalTijdsBlokken != null ? overledeneUitvaartModel.AantalTijdsBlokken : DBNull.Value);
                command.Parameters.AddWithValue("@OpbarenId", overledeneUitvaartModel.Id);
                command.Parameters.AddWithValue("@UitvaartId", overledeneUitvaartModel.UitvaartId);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateUitvaartFailed");
                }
            }
        }
        public void EditAsbestemming(OverledeneAsbestemmingModel asbestemmingModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneAsbestemming] SET [asbestemming] = @Asbestemming, [typeGraf] = @typeGraf, " +
                                        "[bestaandGraf] = @bestaandGraf,[zandKelderGraf] = @zandKelderGraf, [grafmonument] = @grafMonument " +
                                        "WHERE uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@AsbestemmingId", asbestemmingModel.AsbestemmingId);
                command.Parameters.AddWithValue("@UitvaartId", asbestemmingModel.UitvaartId);
                command.Parameters.AddWithValue("@Asbestemming", asbestemmingModel.Asbestemming != null ? asbestemmingModel.Asbestemming : DBNull.Value);
                command.Parameters.AddWithValue("@typeGraf", asbestemmingModel.TypeGraf != null ? asbestemmingModel.TypeGraf : DBNull.Value);
                command.Parameters.AddWithValue("@bestaandGraf", asbestemmingModel.BestaandGraf != null ? asbestemmingModel.BestaandGraf : DBNull.Value);
                command.Parameters.AddWithValue("@zandKelderGraf", asbestemmingModel.ZandKelderGraf != null ? asbestemmingModel.ZandKelderGraf : DBNull.Value);
                command.Parameters.AddWithValue("@grafMonument", asbestemmingModel.GrafMonument != null ? asbestemmingModel.GrafMonument : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateAsbestemmingFailed");
                }
            }
        }
        public string EditBijlages(OverledeneBijlagesModel bijlagesModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneBijlages] SET [DocumentName] = @DocumentName, [DocumentType] = @DocumentType, " +
                                        "[DocumentURL] = @DocumentUrl, [DocumentHash] = @DocumentHash, [DocumentInconsistent] = @DocumentInconsitent " +
                                        "WHERE uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", bijlagesModel.UitvaartId);
                command.Parameters.AddWithValue("@DocumentName", bijlagesModel.DocumentName);
                command.Parameters.AddWithValue("@DocumentType", bijlagesModel.DocumentType);
                command.Parameters.AddWithValue("@DocumentUrl", bijlagesModel.DocumentUrl);
                command.Parameters.AddWithValue("@DocumentHash", bijlagesModel.DocumentHash);
                command.Parameters.AddWithValue("@DocumentInconsitent", bijlagesModel.DocumentInconsistent);
                if (command.ExecuteNonQuery() == 0)
                {
                    return "UpdateBijlagesFailed";
                }
                else
                {
                    return "UpdateBijlagesSucces";
                }
            }
        }
        public void EditSteenhouwerij(OverledeneSteenhouwerijModel overledeneSteenhouwerijModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneSteenhouwer] SET [steenhouwerOpdracht] = @Opdracht, " +
                                        "[steenhouwerBedrag] = @Bedrag, " +
                                        "[steenhouwerProvisie] = @Provisie, " +
                                        "[steenhouwerUitbetaing] = @Uitbetaling, " +
                                        "[steenhouwerText] = @Text, " +
                                        "[steenhouwerLeverancier] = @Leverancier " +
                                        "WHERE uitvaartId = @UitvaartId";
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
                    throw new InvalidOperationException("UpdateSteenhouwerijFailed");
                }
            }
        }
        public void EditBloemen(OverledeneBloemenModel overledeneBloemenModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneBloemen] SET [bloemenText] = @Text," +
                                        "[bloemenLint] = @Lint," +
                                        "[bloemenKaart] = @Kaart," +
                                        "[bloemenBedrag] = @Bedrag," +
                                        "[bloemenProvisie] = @Provisie," +
                                        "[bloemenUitbetaling] = @Uitbetaling, " +
                                        "[bloemenLeverancier] = @Leverancier," +
                                        "[bloemenLintJson] = @bloemenJson," +
                                        "[bloemenBezorgingDatum] = @bezorgdatum," +
                                        "[bloemenBezorgingAdres] = @bezorgadres " +
                                        "WHERE uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@BloemenId", overledeneBloemenModel.BloemenId);
                command.Parameters.AddWithValue("@UitvaartId", overledeneBloemenModel.UitvaartId);
                command.Parameters.AddWithValue("@Text", overledeneBloemenModel.BloemenText != null ? overledeneBloemenModel.BloemenText : DBNull.Value);
                command.Parameters.AddWithValue("@Lint", overledeneBloemenModel.BloemenLint != null ? overledeneBloemenModel.BloemenLint : DBNull.Value);
                command.Parameters.AddWithValue("@Kaart", overledeneBloemenModel.BloemenKaart != null ? overledeneBloemenModel.BloemenKaart : DBNull.Value);
                command.Parameters.AddWithValue("@Bedrag", overledeneBloemenModel.BloemenBedrag != null ? overledeneBloemenModel.BloemenBedrag : DBNull.Value);
                command.Parameters.AddWithValue("@Provisie", overledeneBloemenModel.BloemenProvisie != null ? overledeneBloemenModel.BloemenProvisie : DBNull.Value);
                command.Parameters.AddWithValue("@Uitbetaling", overledeneBloemenModel.BloemenUitbetaling != null ? overledeneBloemenModel.BloemenUitbetaling : DBNull.Value);
                command.Parameters.AddWithValue("@Leverancier", overledeneBloemenModel.BloemenLeverancier);
                command.Parameters.AddWithValue("@bloemenJson", overledeneBloemenModel.BloemenLintJson != null ? overledeneBloemenModel.BloemenLintJson : DBNull.Value);
                command.Parameters.Add(new SqlParameter("@bezorgdatum", SqlDbType.DateTime)
                {
                    Value = overledeneBloemenModel.BloemenBezorgDate.HasValue ? (object)overledeneBloemenModel.BloemenBezorgDate.Value : DBNull.Value
                });
                command.Parameters.AddWithValue("@bezorgadres", overledeneBloemenModel.BloemenBezorgAdres != null ? overledeneBloemenModel.BloemenBezorgAdres : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateBloemenFailed");
                }
            }
        }
        public void EditWerkbonnen(OverledeneWerkbonUitvaart overledeneWerkbonUitvaart)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneWerkbon] SET [werkbonJson] = @WerkbonJson WHERE uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@WerkbonId", overledeneWerkbonUitvaart.Id);
                command.Parameters.AddWithValue("@UitvaartId", overledeneWerkbonUitvaart.UitvaartId);
                command.Parameters.AddWithValue("@WerkbonJson", overledeneWerkbonUitvaart.WerkbonJson != null ? overledeneWerkbonUitvaart.WerkbonJson : DBNull.Value);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateWerkbonnenFailed");
                }
            }
        }
        public void EditUrnSieraden(OverledeneUrnSieradenModel overledeneUrnSieradenModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [overledeneUrnSieraden] SET [urnOpdracht] = @Opdracht," +
                                        "[urnBedrag] = @Bedrag, " +
                                        "[urnProvisie] = @Provisie, " +
                                        "[urnUitbetaling] = @Uitbetaling, " +
                                        "[urnText] = @Text, " +
                                        "[urnLeverancier] = @Leverancier " +
                                        "WHERE uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@SteenhouwerijId", overledeneUrnSieradenModel.UrnId);
                command.Parameters.AddWithValue("@UitvaartId", overledeneUrnSieradenModel.UitvaartId);
                command.Parameters.AddWithValue("@Opdracht", overledeneUrnSieradenModel.UrnOpdracht != null ? overledeneUrnSieradenModel.UrnOpdracht : DBNull.Value);
                command.Parameters.AddWithValue("@Bedrag", overledeneUrnSieradenModel.UrnBedrag != null ? overledeneUrnSieradenModel.UrnBedrag : DBNull.Value);
                command.Parameters.AddWithValue("@Provisie", overledeneUrnSieradenModel.UrnProvisie != null ? overledeneUrnSieradenModel.UrnProvisie : DBNull.Value);
                command.Parameters.AddWithValue("@Uitbetaling", overledeneUrnSieradenModel.UrnUitbetaing != null ? overledeneUrnSieradenModel.UrnUitbetaing : DBNull.Value);
                command.Parameters.AddWithValue("@Text", overledeneUrnSieradenModel.UrnText);
                command.Parameters.AddWithValue("@Leverancier", overledeneUrnSieradenModel.UrnLeverancier);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateUrnSieradenFailed");
                }
            }
        }
        public void EditFactuur(FactuurModel overledeneFactuurModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE OverledeneFacturen SET kostenbegrotingJson = @kostenbegrotingJson, " +
                                        "kostenbegrotingCreationDate = @kostenbegrotingCreationDate, " +
                                        "kostenbegrotingVerzekeraar = @verzekeraarId" +
                                        " WHERE uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", overledeneFactuurModel.UitvaartId);
                command.Parameters.AddWithValue("@kostenbegrotingJson", overledeneFactuurModel.KostenbegrotingJson);
                command.Parameters.AddWithValue("@kostenbegrotingCreationDate", DateTime.Now);
                command.Parameters.AddWithValue("@verzekeraarId", overledeneFactuurModel.KostenbegrotingVerzekeraar);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateKostenbegrotingFailed");
                }
            }
        }
        public void EditKlanttevredenheid(Klanttevredenheid klanttevredenheid)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE OverledeneKlantTevredenheid SET Cijfer = @cijfer, NotificatieOverleden = @notificatie WHERE uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", klanttevredenheid.UitvaartId);
                command.Parameters.AddWithValue("@cijfer", klanttevredenheid.CijferScore);
                command.Parameters.AddWithValue("@notificatie", (klanttevredenheid.IsNotificationEnabled == true) ? true : false);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateKlanttevredenheidFailed");
                }
            }
        }
        public void EmployeeUpdate(WerknemersModel werknemerUpdate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE ConfigurationPersoneel SET Initialen = @initialen, " +
                                        "Voornaam = @voornaam, " +
                                        "Roepnaam = @roepnaam, " +
                                        "Tussenvoegsel = @tussenvoegsel, " +
                                        "Achternaam = @achternaam, " +
                                        "Geboorteplaats = @geboorteplaats, " +
                                        "Geboortedatum = @geboortedatum, " +
                                        "Email = @email, " +
                                        "isDeleted = 0, " +
                                        "isUitvaartverzorger = @uitvaartverzorger, " +
                                        "isDrager = @isdrager, " +
                                        "isChauffeur = @ischauffeur, " +
                                        "isOpbaren = @isopbaren, " +
                                        "Mobiel = @mobiel " +
                                        "WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", werknemerUpdate.Id);
                command.Parameters.AddWithValue("@initialen", string.IsNullOrEmpty(werknemerUpdate.Initialen) ? DBNull.Value : werknemerUpdate.Initialen);
                command.Parameters.AddWithValue("@voornaam", string.IsNullOrEmpty(werknemerUpdate.Voornaam) ? DBNull.Value : werknemerUpdate.Voornaam);
                command.Parameters.AddWithValue("@roepnaam", string.IsNullOrEmpty(werknemerUpdate.Roepnaam) ? DBNull.Value : werknemerUpdate.Roepnaam);
                command.Parameters.AddWithValue("@tussenvoegsel", string.IsNullOrEmpty(werknemerUpdate.Tussenvoegsel) ? DBNull.Value : werknemerUpdate.Tussenvoegsel);
                command.Parameters.AddWithValue("@achternaam", string.IsNullOrEmpty(werknemerUpdate.Achternaam) ? DBNull.Value : werknemerUpdate.Achternaam);
                command.Parameters.AddWithValue("@geboorteplaats", string.IsNullOrEmpty(werknemerUpdate.Geboorteplaats) ? DBNull.Value : werknemerUpdate.Geboorteplaats);
                command.Parameters.AddWithValue("@geboortedatum", werknemerUpdate.Geboortedatum);
                command.Parameters.AddWithValue("@email", string.IsNullOrEmpty(werknemerUpdate.Email) ? DBNull.Value : werknemerUpdate.Email);
                command.Parameters.AddWithValue("@uitvaartverzorger", werknemerUpdate.IsUitvaartverzorger);
                command.Parameters.AddWithValue("@isdrager", werknemerUpdate.IsDrager);
                command.Parameters.AddWithValue("@ischauffeur", werknemerUpdate.IsChauffeur);
                command.Parameters.AddWithValue("@isopbaren", werknemerUpdate.IsOpbaren);
                command.Parameters.AddWithValue("@mobiel", string.IsNullOrEmpty(werknemerUpdate.Mobiel) ? DBNull.Value : werknemerUpdate.Mobiel);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("EmployeeUpdateFailed");
                }
            }
        }
        public void KistUpdate(KistenModel kistUpdate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE ConfigurationKisten SET kistOmschrijving = @omschrijving, " +
                                        "kistTypeNummer = @typenummer WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", kistUpdate.Id);
                command.Parameters.AddWithValue("@omschrijving", kistUpdate.KistOmschrijving);
                command.Parameters.AddWithValue("@typenummer", kistUpdate.KistTypeNummer);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("KistUpdateFailed");
                }
            }
        }
        public void AsbestemmingUpdate(ConfigurationAsbestemmingModel asbestemmingUpdate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE ConfigurationAsbestemming SET asbestemmingOmschrijving = @omschrijving WHERE asbestemmingId = @Id";
                command.Parameters.AddWithValue("@Id", asbestemmingUpdate.AsbestemmingId);
                command.Parameters.AddWithValue("@omschrijving", asbestemmingUpdate.AsbestemmingOmschrijving);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("AsbestemmingUpdateFailed");
                }
            }
        }
        public void VerzekeringUpdate(VerzekeraarsModel verzekeringUpdate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE ConfigurationVerzekeraar " +
                                        "SET verzekeraarNaam = @naam, " +
                                        "isHerkomst = @herkomst, " +
                                        "isVerzekeraar = @verzekeraar, " +
                                        "hasLidnummer = @lidnummer, " +
                                        "postbusAddress = @postbus, " +
                                        "postbusNaam = @postbusNaam, " +
                                        "addressStreet = @street, " +
                                        "addressHousenumber = @housenumber, " +
                                        "addressHousenumberAddition = @addition, " +
                                        "addressZipcode = @zipcode, " +
                                        "addressCity = @city, " +
                                        "factuurType = @factuurtype, " +
                                        "correspondentieType = @correspondentieType," +
                                        "isPakket = @isPakket," +
                                        "OverrideFactuurAdress = @isOverrideAddress," +
                                        "verzekeraarTelefoon = @telefoon," +
                                        "CustomLogo = @customLogo " +
                                        "WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", verzekeringUpdate.Id);
                command.Parameters.AddWithValue("@naam", verzekeringUpdate.Name);
                command.Parameters.AddWithValue("@herkomst", verzekeringUpdate.IsHerkomst);
                command.Parameters.AddWithValue("@verzekeraar", verzekeringUpdate.IsVerzekeraar);
                command.Parameters.AddWithValue("@lidnummer", verzekeringUpdate.HasLidnummer);
                command.Parameters.AddWithValue("@postbus", string.IsNullOrEmpty(verzekeringUpdate.PostbusAddress) ? (object)DBNull.Value : verzekeringUpdate.PostbusAddress);
                command.Parameters.AddWithValue("@postbusNaam", string.IsNullOrEmpty(verzekeringUpdate.PostbusName) ? (object)DBNull.Value : verzekeringUpdate.PostbusName);
                command.Parameters.AddWithValue("@street", string.IsNullOrEmpty(verzekeringUpdate.AddressStreet) ? (object)DBNull.Value : verzekeringUpdate.AddressStreet);
                command.Parameters.AddWithValue("@housenumber", string.IsNullOrEmpty(verzekeringUpdate.AddressHousenumber) ? (object)DBNull.Value : verzekeringUpdate.AddressHousenumber);
                command.Parameters.AddWithValue("@addition", string.IsNullOrEmpty(verzekeringUpdate.AddressHousenumberAddition) ? (object)DBNull.Value : verzekeringUpdate.AddressHousenumberAddition);
                command.Parameters.AddWithValue("@zipcode", string.IsNullOrEmpty(verzekeringUpdate.AddressZipCode) ? (object)DBNull.Value : verzekeringUpdate.AddressZipCode);
                command.Parameters.AddWithValue("@city", string.IsNullOrEmpty(verzekeringUpdate.AddressCity) ? (object)DBNull.Value : verzekeringUpdate.AddressCity);
                command.Parameters.AddWithValue("@factuurtype", string.IsNullOrEmpty(verzekeringUpdate.FactuurType) ? (object)DBNull.Value : verzekeringUpdate.FactuurType);
                command.Parameters.AddWithValue("@correspondentieType", string.IsNullOrEmpty(verzekeringUpdate.CorrespondentieType) ? (object)DBNull.Value : verzekeringUpdate.CorrespondentieType);
                command.Parameters.AddWithValue("@isPakket", verzekeringUpdate.Pakket);
                command.Parameters.AddWithValue("@isOverrideAddress", verzekeringUpdate.IsOverrideFactuurAdress);
                command.Parameters.AddWithValue("@telefoon", string.IsNullOrEmpty(verzekeringUpdate.Telefoon) ? (object)DBNull.Value : verzekeringUpdate.Telefoon);
                command.Parameters.AddWithValue("@customLogo", verzekeringUpdate.CustomLogo);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateVerzekeringFailed");
                }
            }
        }
        public void UpdatePriceComponent(KostenbegrotingModel priceComponent)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationFactuurComponent] " +
                                        "SET Omschrijving = @omschrijving, " +
                                        "Bedrag = @bedrag, " +
                                        "VerzekerdAantal = @aantal, " +
                                        "VerzekeringJson = @verzekeringJson, " +
                                        "SortOrder = @sortOrder, " +
                                        "factuurBedrag = @factuurBedrag, " +
                                        "DefaultPM = @defaultpm " +
                                        "WHERE ComponentId = @compid";
                command.Parameters.AddWithValue("@compid", priceComponent.ComponentId);
                command.Parameters.AddWithValue("@omschrijving", priceComponent.ComponentOmschrijving);
                command.Parameters.AddWithValue("@aantal", priceComponent.ComponentAantal);
                command.Parameters.AddWithValue("@bedrag", priceComponent.ComponentBedrag);
                command.Parameters.AddWithValue("@factuurBedrag", priceComponent.ComponentFactuurBedrag);
                command.Parameters.AddWithValue("@verzekeringJson", priceComponent.ComponentVerzekeringJson);
                command.Parameters.AddWithValue("@sortOrder", priceComponent.SortOrder);
                command.Parameters.AddWithValue("@defaultpm", priceComponent.DefaultPM);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdatePriceComponentFailed");
                }
            }
        }
        public void UpdateSteenhouwerijBetaling(OverledeneSteenhouwerijModel steenInfo)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneSteenhouwer] " +
                                        "SET [steenhouwerPaid] = 1, " +
                                        "[steenhouwerProvisie] = @provisie, " +
                                        "[steenhouwerUitbetaing] = @betalingDatum, " +
                                        "[steenhouwerBedrag] = @bedrag, " +
                                        "[steenhouwerProvisieTotaal] = @provisieTotaal " +
                                        "WHERE [uitvaartId] = @UitvaartId ";
                command.Parameters.AddWithValue("@provisie", steenInfo.SteenhouwerProvisie);
                command.Parameters.AddWithValue("@betalingDatum", steenInfo.SteenhouwerUitbetaing);
                command.Parameters.AddWithValue("@bedrag", steenInfo.SteenhouwerBedrag);
                command.Parameters.AddWithValue("@provisieTotaal", steenInfo.SteenhouwerProvisieTotaal ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UitvaartId", steenInfo.UitvaartId);

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateSteenhouwerijBetalingFailed");
                }
            }
        }
        public void UpdateBloemenBetaling(OverledeneBloemenModel bloemInfo)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneBloemen] " +
                                        "SET [bloemenPaid] = 1, " +
                                        "[bloemenProvisie] = @provisie, " +
                                        "[bloemenUitbetaling] = @betalingDatum, " +
                                        "[bloemenBedrag] = @bedrag " +
                                        "WHERE [uitvaartId] = @UitvaartId ";
                command.Parameters.AddWithValue("@provisie", bloemInfo.BloemenProvisie);
                command.Parameters.AddWithValue("@betalingDatum", bloemInfo.BloemenUitbetaling);
                command.Parameters.AddWithValue("@bedrag", bloemInfo.BloemenBedrag);
                command.Parameters.AddWithValue("@UitvaartId", bloemInfo.UitvaartId);

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateBloemenBetalingFailed");
                }
            }
        }
        public void UpdateSuggestion(SuggestionModel suggestionCreate)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [ConfigurationOverledenLocaties] " +
                                        "SET [ShortName] = @ShortName," +
                                        "[LongName] = @LongName," +
                                        "[Street] = @Street," +
                                        "[Housenumber] = @Housenumber," +
                                        "[Zipcode] = @Zipcode," +
                                        "[City] = @City," +
                                        "[County] = @County " +
                                        "WHERE [Id] = @Id";
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
        public void UpdateUrnSieradenBetaling(OverledeneUrnSieradenModel urnSieradenInfo)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [overledeneUrnSieraden] " +
                                        "SET [urnPaid] = 1, " +
                                        "[urnProvisie] = @provisie, " +
                                        "[urnUitbetaling] = @betalingDatum, " +
                                        "[urnBedrag] = @bedrag " +
                                        "WHERE [uitvaartId] = @UitvaartId";
                command.Parameters.AddWithValue("@provisie", urnSieradenInfo.UrnProvisie);
                command.Parameters.AddWithValue("@betalingDatum", urnSieradenInfo.UrnUitbetaing);
                command.Parameters.AddWithValue("@bedrag", urnSieradenInfo.UrnBedrag);
                command.Parameters.AddWithValue("@UitvaartId", urnSieradenInfo.UitvaartId);

                if (command.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("UpdateUrnSieradenBetalingFailed");
                }
            }
        }
        public async Task UpdateKostenbegrotingAsync(string kostenbegrotingUrl, string kostenbegrotingData, DateTime creationDate, Guid uitvaartId, Guid verzekeraarId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneFacturen]" +
                                    " SET [kostenbegrotingUrl] = @kostenbegrotingUrl," +
                                    " [kostenbegrotingJson] = @kostenbegrotingData," +
                                    " [kostenbegrotingCreationDate] = @kostenbegrotingCreationDate," +
                                    " [kostenbegrotingVerzekeraar] = @verzekeraarId" +
                                    " WHERE [UitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@kostenbegrotingUrl", kostenbegrotingUrl);
                command.Parameters.AddWithValue("@kostenbegrotingData", kostenbegrotingData);
                command.Parameters.AddWithValue("@kostenbegrotingCreationDate", creationDate);
                command.Parameters.AddWithValue("@verzekeraarId", verzekeraarId);
                command.Parameters.AddWithValue("@uitvaartId", uitvaartId);

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdateDocumentInfoAsync(OverledeneBijlagesModel documentInfo)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                command.Connection = connection;
                command.CommandText = "UPDATE [OverledeneBijlages] " +
                                      "SET [DocumentURL] = @documentUrl, " +
                                      "[DocumentHash] = @documentHash " +
                                      "WHERE [UitvaartId] = @UitvaartId " +
                                      "AND [DocumentName] = @documentName";
                command.Parameters.AddWithValue("@documentUrl", documentInfo.DocumentUrl);
                command.Parameters.AddWithValue("@documentHash", documentInfo.DocumentHash);
                command.Parameters.AddWithValue("@UitvaartId", documentInfo.UitvaartId);
                command.Parameters.AddWithValue("@documentName", documentInfo.DocumentName);

                var rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException("UpdateDocumentInfoFailed");
                }
            }
        }
    }
}
