using Dossier_Registratie.Helper;
using Dossier_Registratie.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dossier_Registratie.Repositories
{
    public class SearchOperations : RepositoryBase, ISearchOperations
    {
        public async Task<bool> SearchBlobLogo(string appType)
        {
            using (var connection = GetConnection())
            using (var command = connection.CreateCommand())
            {
                await connection.OpenAsync();

                command.CommandText = @"SELECT 1 FROM ConfigurationBlob WHERE AppType = @AppType";

                command.Parameters.Add(new SqlParameter("@AppType", SqlDbType.NVarChar) { Value = appType });

                var result = await command.ExecuteScalarAsync();
                return result != null;
            }
        }

        public (string PermissionLevelId, string PermissionLevelName, bool IsActive) FetchUserCredentials(string windowsUsername)
        {
            using var connection = new SqlConnection(DataProvider.ConnectionString);
            connection.Open();

            using var command = new SqlCommand(
                "SELECT CPP.PermissionId As PermissionId, CP.PermissionName As PermissionName, CU.IsActive " +
                "FROM ConfigurationPersoneelPermission AS CPP " +
                "INNER JOIN ConfigurationPermissions AS CP ON CPP.PermissionId = CP.Id " +
                "INNER JOIN ConfigurationUsers AS CU ON CU.PersoneelId = CPP.PersoneelId " +
                "WHERE CU.WindowsUsername = @WindowsUsername", connection);

            command.Parameters.AddWithValue("@WindowsUsername", windowsUsername);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var permissionLevelId = reader.IsDBNull(0) ? "NotRegistered" : reader.GetGuid(0).ToString();
                var permissionLevelName = reader.IsDBNull(1) ? "NotRegistered" : reader.GetString(1);
                var isActive = reader.IsDBNull(2) ? false : reader.GetBoolean(2);
                return (permissionLevelId, permissionLevelName, isActive);
            }

            return ("NotRegistered", "NotRegistered", false);
        }

        public async Task<int> SearchKostenbegrotingExistanceAsync(Guid uitvaartGuid)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            await connection.OpenAsync();
            command.Connection = connection;
            command.CommandText = "SELECT COUNT(*) FROM [OverledeneFacturen] WHERE [UitvaartId] = @UitvaartId AND [kostenbegrotingUrl] != ''";
            command.Parameters.AddWithValue("@UitvaartId", uitvaartGuid);

            return (int)await command.ExecuteScalarAsync();
        }
        public WerknemersModel SearchEmployee(WerknemersModel werknemerSearch)
        {
            WerknemersModel werknemer = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],[Achternaam],[Geboorteplaats],[Geboortedatum] " +
                                        "FROM ConfigurationPersoneel " +
                                        "WHERE Achternaam = @Achternaam AND Geboortedatum = @GeboorteDatum AND Geboorteplaats = @Geboorteplaats";
                command.Parameters.AddWithValue("@Achternaam", werknemerSearch.Achternaam);
                command.Parameters.AddWithValue("@GeboorteDatum", werknemerSearch.Geboortedatum);
                command.Parameters.AddWithValue("@Geboorteplaats", werknemerSearch.Geboorteplaats);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        werknemer = new WerknemersModel()
                        {
                            Id = reader.IsDBNull(0) ? Guid.Empty : (Guid)reader[0],
                            Achternaam = reader.IsDBNull(1) ? string.Empty : reader[1].ToString(),
                            Geboorteplaats = reader.IsDBNull(2) ? string.Empty : reader[2].ToString(),
                            Geboortedatum = reader.IsDBNull(3) || ((DateTime)reader[3]).Date == DateTime.MinValue.Date ? DateTime.MinValue : (DateTime)reader[3]
                        };
                    }
                }
            }
            return werknemer;
        }
        public PermissionsModel SelectUserPermission(Guid PersoneelId)
        {
            PermissionsModel permissies = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT CP.PermissionName, PersoneelId, CP.Id " +
                                        "FROM [ConfigurationPersoneelPermission] CPP " +
                                        "INNER JOIN ConfigurationPermissions CP ON CPP.PermissionId = CP.Id " +
                                        "WHERE PersoneelId = @PersoneelId";
                command.Parameters.AddWithValue("@PersoneelId", PersoneelId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        permissies = new PermissionsModel()
                        {
                            PermissionName = reader.IsDBNull(0) ? string.Empty : reader[0].ToString(),
                            EmployeeId = reader.IsDBNull(1) ? Guid.Empty : (Guid)reader[1],
                            Id = reader.IsDBNull(2) ? Guid.Empty : (Guid)reader[2]
                        };
                    }
                }
            }
            return permissies;
        }
        public IEnumerable<OverledeneSearchSurname> GetUitvaarleiderByUitvaartIdSearch(string uitvaartId)
        {
            var overledeneList = new List<OverledeneSearchSurname>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "SELECT OUL.UitvaartId, OUL.Uitvaartnummer, OPG.overledeneAanhef, OPG.overledeneVoornamen, OPG.overledeneTussenvoegsel, OPG.overledeneAchternaam, OPG.overledeneGeboortedatum, CONCAT(Initialen,' ',Achternaam) as PersoneelName, PersoneelId, dossierCompleted " +
                                      "FROM OverledeneUitvaartleider AS OUL " +
                                      "INNER JOIN OverledenePersoonsGegevens AS OPG ON OPG.UitvaartId = OUL.UitvaartId " +
                                      "LEFT JOIN ConfigurationPersoneel AS CP ON CP.Id = OUL.PersoneelId " +
                                      "WHERE Uitvaartnummer=@uitvaartNummer";

                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var searchGegevens = new OverledeneSearchSurname()
                        {
                            UitvaartId = (Guid)reader[0],
                            UitvaartNummer = reader[1].ToString(),
                            OverledeneAanhef = reader[2].ToString(),
                            OverledeneVoornaam = reader[3].ToString(),
                            OverledeneTussenvoegsel = reader[4].ToString(),
                            OverledeneAchternaam = reader[5].ToString(),
                            OverledeneGeboortedatum = (DateTime)reader[6],
                            PersoneelNaam = reader[7].ToString(),
                            PersoneelId = (Guid)reader[8],
                            DossierCompleted = reader[9] != DBNull.Value ? (bool)reader[4] : false,
                        };
                        overledeneList.Add(searchGegevens);
                    }
                }
            }
            return overledeneList;
        }
        public OverledeneUitvaartleiderModel GetUitvaarleiderByUitvaartId(string uitvaartId)
        {
            OverledeneUitvaartleiderModel uitvaartLeiderGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "SELECT UitvaartId, CONCAT(Initialen,' ',Achternaam) as PersoneelName, PersoneelId, Uitvaartnummer, dossierCompleted " +
                                        " FROM OverledeneUitvaartleider" +
                                        " LEFT JOIN ConfigurationPersoneel ON PersoneelId = Id" +
                                        " WHERE Uitvaartnummer=@uitvaartNummer";

                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        uitvaartLeiderGegevens = new OverledeneUitvaartleiderModel()
                        {
                            UitvaartId = (Guid)reader[0],
                            PersoneelNaam = reader[1].ToString(),
                            PersoneelId = (Guid)reader[2],
                            Uitvaartnummer = reader[3].ToString(),
                            DossierCompleted = reader[4] != DBNull.Value ? (bool)reader[4] : false,
                        };
                    }
                }
            }
            return uitvaartLeiderGegevens;
        }
        public OverledenePersoonsGegevensModel GetPeroonsGegevensByUitvaartId(string uitvaartId)
        {
            OverledenePersoonsGegevensModel persoonsGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "SELECT Id, OUL.uitvaartId, overledeneAchternaam, overledeneTussenvoegsel," +
                                        "overledeneVoornamen, overledeneAanhef, overledeneGeboortedatum," +
                                        "overledeneGeboorteplaats, overledeneGemeente, overledeneLeeftijd, overledeneBSN," +
                                        "overledeneAdres, overledeneHuisnummer, overledeneHuisnummerToevoeging, " +
                                        "overledenePostcode,overledeneWoonplaats, overledeneVoorregeling " +
                                        "FROM OverledenePersoonsGegevens OPG " +
                                        "RIGHT JOIN OverledeneUitvaartleider OUL ON OUL.UitvaartId = OPG.UitvaartId " +
                                        "WHERE Uitvaartnummer=@uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        persoonsGegevens = new OverledenePersoonsGegevensModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            OverledeneAchternaam = reader[2].ToString(),
                            OverledeneTussenvoegsel = reader[3].ToString(),
                            OverledeneVoornamen = reader[4].ToString(),
                            OverledeneAanhef = reader[5].ToString(),
                            OverledeneGeboortedatum = (DateTime)reader[6],
                            OverledeneGeboorteplaats = reader[7].ToString(),
                            OverledeneGemeente = reader[8].ToString(),
                            OverledeneLeeftijd = reader[9].ToString(),
                            OverledeneBSN = reader[10].ToString(),
                            OverledeneAdres = reader[11].ToString(),
                            OverledeneHuisnummer = reader[12].ToString(),
                            OverledeneHuisnummerToevoeging = reader[13].ToString(),
                            OverledenePostcode = reader[14].ToString(),
                            OverledeneWoonplaats = reader[15].ToString(),
                            OverledeneVoorregeling = Convert.IsDBNull(reader[16]) ? false : (bool)reader[16],
                        };
                    }
                }
            }
            return persoonsGegevens;
        }
        public OverledeneOverlijdenInfoModel GetOverlijdenInfoByUitvaartId(string uitvaartId)
        {
            OverledeneOverlijdenInfoModel overlijdenInfo = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "SELECT Id, OUL.UitvaartId, overledenDatumTijd, overledenAdres, " +
                                        "overledenHuisnummer, overledenHuisnummerToevoeging, overledenPlaats," +
                                        "overledenGemeente, overledenLijkvinding, overledenHerkomst, overledenLidnummer," +
                                        "overledenHuisarts, overledenHuisartsTelefoon, overledenSchouwarts, overledenPostcode, overledenLocatie " +
                                        "FROM OverledeneOverlijdenInfo OOI " +
                                        "RIGHT JOIN OverledeneUitvaartleider OUL ON OUL.UitvaartId = OOI.UitvaartId " +
                                        "WHERE Uitvaartnummer=@uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        overlijdenInfo = new OverledeneOverlijdenInfoModel()
                        {
                            Id = reader.IsDBNull(0) ? Guid.Empty : (Guid)reader[0],
                            UitvaartId = reader.IsDBNull(1) ? Guid.Empty : (Guid)reader[1],
                            OverledenDatumTijd = reader.IsDBNull(2) || ((DateTime)reader[2]).Date == DateTime.MinValue.Date ? DateTime.MinValue : (DateTime)reader[2],
                            OverledenAdres = reader.IsDBNull(3) ? string.Empty : reader[3].ToString(),
                            OverledenHuisnummer = reader.IsDBNull(4) ? string.Empty : reader[4].ToString(),
                            OverledenHuisnummerToevoeging = reader.IsDBNull(5) ? string.Empty : reader[5].ToString(),
                            OverledenPlaats = reader.IsDBNull(6) ? string.Empty : reader[6].ToString(),
                            OverledenGemeente = reader.IsDBNull(7) ? string.Empty : reader[7].ToString(),
                            OverledenLijkvinding = reader.IsDBNull(8) ? string.Empty : reader[8].ToString(),
                            OverledenHerkomst = reader.IsDBNull(9) ? Guid.Empty : (Guid)reader[9],
                            OverledenLidnummer = reader.IsDBNull(10) ? string.Empty : reader[10].ToString(),
                            OverledenHuisarts = reader.IsDBNull(11) ? string.Empty : reader[11].ToString(),
                            OverledenHuisartsTelefoon = reader.IsDBNull(12) ? string.Empty : reader[12].ToString(),
                            OverledenSchouwarts = reader.IsDBNull(13) ? string.Empty : reader[13].ToString(),
                            OverledenPostcode = reader.IsDBNull(14) ? string.Empty : reader[14].ToString(),
                            OverledenLocatie = reader.IsDBNull(15) ? string.Empty : reader[15].ToString(),
                        };
                    }
                }
            }
            return overlijdenInfo;
        }
        public OverledeneExtraInfoModel GetExtraInfoByUitvaartId(string uitvaartId)
        {
            OverledeneExtraInfoModel extraInfo = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],OEI.[uitvaartId],[overledeneBurgelijkestaat],[overledeneGescheidenVan],[overledeneWedenaarVan]," +
                                        " [overledeneTrouwboekje],[overledeneAantalKinderen],[overledeneAantalKinderenOverleden],[overledeneKinderenMinderjarig]," +
                                        " [overledeneEersteOuder],[overledeneEersteOuderOverleden],[overledeneTweedeOuder],[overledeneTweedeOuderOverleden]," +
                                        " [overledeneLevensovertuiging],[overledeneExecuteur],[overledeneExecuteurTelefoon],[overledeneTestament]," +
                                        " [overledeneTrouwDatumTijd],[overledeneGeregistreerdDatumTijd],[overledeneNotaris],[overledeneNotarisTelefoon]" +
                                        " FROM [OverledeneExtraInfo] OEI" +
                                        " INNER JOIN [OverledeneUitvaartleider] OU ON OU.UitvaartId = OEI.uitvaartId" +
                                        " WHERE Uitvaartnummer=@uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        extraInfo = new OverledeneExtraInfoModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            OverledeneBurgelijkestaat = reader[2].ToString(),
                            OverledeneGescheidenVan = reader[3].ToString(),
                            OverledeneWedenaarVan = reader[4].ToString(),
                            OverledeneTrouwboekje = reader[5].ToString(),
                            OverledeneAantalKinderen = reader[6].ToString(),
                            OverledeneKinderenMinderjarig = reader[7].ToString(),
                            OverledeneKinderenMinderjarigOverleden = reader[8].ToString(),
                            OverledeneEersteOuder = reader[9].ToString(),
                            OverledeneEersteOuderOverleden = reader[10].ToString(),
                            OverledeneTweedeOuder = reader[11].ToString(),
                            OverledeneTweedeOuderOverleden = reader[12].ToString(),
                            OverledeneLevensovertuiging = reader[13].ToString(),
                            OverledeneExecuteur = reader[14].ToString(),
                            OverledeneExecuteurTelefoon = reader[15].ToString(),
                            OverledeneTestament = reader[16].ToString(),
                            OverledeneTrouwDatumTijd = reader[17] != DBNull.Value ? (DateTime)reader[17] : default,
                            OverledeneGeregistreerdDatumTijd = reader[18] != DBNull.Value ? (DateTime)reader[18] : default,
                            OverledeneNotaris = reader[19].ToString(),
                            OverledeneNotarisTelefoon = reader[20].ToString(),
                        };

                    }
                }
            }
            return extraInfo;
        }
        public OpdrachtgeverPersoonsGegevensModel GetOpdrachtgeverByUitvaartId(string uitvaartId)
        {
            OpdrachtgeverPersoonsGegevensModel opdrachtGever = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],OO.[uitvaartId],[opdrachtgeverAanhef],[opdrachtgeverAchternaam],[opdrachtgeverVoornaamen]" +
                                    ",[opdrachtgeverTussenvoegsel],[opdrachtgeverGeboortedatum],[opdrachtgeverLeeftijd]" +
                                    ",[opdrachtgeverStraat],[opdrachtgeverHuisnummer],[opdrachtgeverHuisnummerToevoeging],[opdrachtgeverPostcode]" +
                                    ",[opdrachtgeverWoonplaats],[opdrachtgeverGemeente],[opdrachtgeverTelefoon],[opdrachtgeverBSN]," +
                                    "[opdrachtgeverRelatieTotOverledene], [opdrachtgeverExtraInfo], [opdrachtgeverEmail]" +
                                    " FROM [OverledeneOpdrachtgever] OO" +
                                    " INNER JOIN [OverledeneUitvaartleider] OU ON OU.UitvaartId = OO.uitvaartId" +
                                    " WHERE Uitvaartnummer=@uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        opdrachtGever = new OpdrachtgeverPersoonsGegevensModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            OpdrachtgeverAanhef = reader[2].ToString(),
                            OpdrachtgeverAchternaam = reader[3].ToString(),
                            OpdrachtgeverVoornaamen = reader[4].ToString(),
                            OpdrachtgeverTussenvoegsel = reader[5].ToString(),
                            //OpdrachtgeverGeboortedatum = (DateTime)reader[6],
                            OpdrachtgeverGeboortedatum = reader[6] == DBNull.Value ? (DateTime?)null : (DateTime)reader[6],
                            OpdrachtgeverLeeftijd = reader[7].ToString(),
                            OpdrachtgeverStraat = reader[8].ToString(),
                            OpdrachtgeverHuisnummer = reader[9].ToString(),
                            OpdrachtgeverHuisnummerToevoeging = reader[10].ToString(),
                            OpdrachtgeverPostcode = reader[11].ToString(),
                            OpdrachtgeverWoonplaats = reader[12].ToString(),
                            OpdrachtgeverGemeente = reader[13].ToString(),
                            OpdrachtgeverTelefoon = reader[14].ToString(),
                            OpdrachtgeverBSN = reader[15].ToString(),
                            OpdrachtgeverRelatieTotOverledene = reader[16].ToString(),
                            OpdrachtgeverExtraInformatie = reader[17].ToString(),
                            OpdrachtgeverEmail = reader[18].ToString()
                        };
                    }
                }
            }
            return opdrachtGever;
        }
        public OpdrachtgeverPersoonsGegevensModel GetExtraOpdrachtgeverByUitvaartId(string uitvaartId)
        {
            OpdrachtgeverPersoonsGegevensModel opdrachtGever = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],OO.[uitvaartId],[opdrachtgeverAanhef],[opdrachtgeverAchternaam],[opdrachtgeverVoornaamen]" +
                                    ",[opdrachtgeverTussenvoegsel],[opdrachtgeverGeboortedatum],[opdrachtgeverLeeftijd]" +
                                    ",[opdrachtgeverStraat],[opdrachtgeverHuisnummer],[opdrachtgeverHuisnummerToevoeging],[opdrachtgeverPostcode]" +
                                    ",[opdrachtgeverWoonplaats],[opdrachtgeverGemeente],[opdrachtgeverTelefoon],[opdrachtgeverBSN],[opdrachtgeverRelatieTotOverledene],[opdrachtgeverEmail]" +
                                    " FROM [OverledeneExtraOpdrachtgever] OO" +
                                    " INNER JOIN [OverledeneUitvaartleider] OU ON OU.UitvaartId = OO.uitvaartId" +
                                    " WHERE Uitvaartnummer=@uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        opdrachtGever = new OpdrachtgeverPersoonsGegevensModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            OpdrachtgeverAanhef = reader[2].ToString(),
                            OpdrachtgeverAchternaam = reader[3].ToString(),
                            OpdrachtgeverVoornaamen = reader[4].ToString(),
                            OpdrachtgeverTussenvoegsel = reader[5].ToString(),
                            OpdrachtgeverGeboortedatum = reader.IsDBNull(6) ? (DateTime?)null : (DateTime)reader[6],
                            OpdrachtgeverLeeftijd = reader[7].ToString(),
                            OpdrachtgeverStraat = reader[8].ToString(),
                            OpdrachtgeverHuisnummer = reader[9].ToString(),
                            OpdrachtgeverHuisnummerToevoeging = reader[10].ToString(),
                            OpdrachtgeverPostcode = reader[11].ToString(),
                            OpdrachtgeverWoonplaats = reader[12].ToString(),
                            OpdrachtgeverGemeente = reader[13].ToString(),
                            OpdrachtgeverTelefoon = reader[14].ToString(),
                            OpdrachtgeverBSN = reader[15].ToString(),
                            OpdrachtgeverRelatieTotOverledene = reader[16].ToString(),
                            OpdrachtgeverEmail = reader[17].ToString()
                        };
                    }
                }
            }
            return opdrachtGever;
        }
        public OverledeneVerzekeringModel GetOverlijdenVerzekeringByUitvaartId(string uitvaartId)
        {
            OverledeneVerzekeringModel verzekeringGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, OVI.uitvaartId, verzekeringProperties" +
                                      " FROM OverledeneVerzerkeringInfo OVI" +
                                      " INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OVI.UitvaartId" +
                                      " WHERE Uitvaartnummer = @uitvaartNummer";

                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        verzekeringGegevens = new OverledeneVerzekeringModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            VerzekeringProperties = reader[2].ToString(),
                        };
                    }
                }
            }
            return verzekeringGegevens;
        }
        public OverledeneOpbarenModel GetOverlijdenOpbarenInfoByUitvaartId(string uitvaartId)
        {
            OverledeneOpbarenModel opbarenInfo = null;

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [opbaringId],OO.[uitvaartId],[opbaringLocatie],[opbaringKistId], [opbaringKistOmschrijving],[opbaringKistLengte]," +
                                        "[opbaringVerzorging],[opbaringVerzorgingJson],[opbaringKoeling],[opbaringKledingMee], " +
                                        "[opbaringKledingRetour],[opbaringSieraden],[opbaringSieradenOmschrijving],[opbaringSieradenRetour], [opbaringBezoek], [opbaringExtraInfo] " +
                                        "FROM [OverledeneOpbaring] OO  " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OO.UitvaartId " +
                                        "WHERE Uitvaartnummer=@uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        opbarenInfo = new OverledeneOpbarenModel()
                        {
                            OpbaringId = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            OpbaringLocatie = reader[2].ToString(),
                            OpbaringKistId = (Guid)reader[3],
                            OpbaringKistOmschrijving = reader[4].ToString(),
                            OpbaringKistLengte = (Guid)reader[5],
                            OpbaringVerzorging = reader[6].ToString(),
                            OpbaringVerzorgingJson = reader[7].ToString(),
                            OpbaringKoeling = reader[8].ToString(),
                            OpbaringKledingMee = reader[9].ToString(),
                            OpbaringKledingRetour = reader[10].ToString(),
                            OpbaringSieraden = reader[11].ToString(),
                            OpbaringSieradenOmschrijving = reader[12].ToString(),
                            OpbaringSieradenRetour = reader[13].ToString(),
                            OpbaringBezoek = reader[14].ToString(),
                            OpbaringExtraInfo = reader[15].ToString()
                        };
                    }
                }
            }
            return opbarenInfo;
        }
        public OverledeneUitvaartModel GetOverlijdenUitvaartInfoByUitvaartId(string uitvaartId)
        {
            OverledeneUitvaartModel uitvaartGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],OUI.[uitvaartId],[uitvaartInfoDatumTijdCondoleance]," +
                                        "[uitvaartInfoCondoleanceConsumpties],[uitvaartInfoType]," +
                                        "[uitvaartInfoDatumTijdUitvaart],[uitvaartInfoUitvaartLocatie]," +
                                        "[uitvaartInfoDienstDatumTijd],[uitvaartInfoDienstLocatie]," +
                                        "[uitvaartInfoDienstAfscheid],[uitvaartInfoDienstMuziek],[uitvaartInfoDienstBesloten]," +
                                        "[uitvaartInfoDienstVolgauto],[uitvaartInfoDienstConsumpties] ,[uitvaartInfoDienstKist]," +
                                        "[uitvaartInfoCondoleanceYesNo],[uitvaartInfoSpreker],[uitvaartInfoPowerPoint], [uitvaartTijdBlokken], [uitvaartAantalTijdsBlokken] " +
                                        "FROM [OverledeneUitvaartInfo] OUI " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OUI.UitvaartId " +
                                        "WHERE Uitvaartnummer = @Uitvaartnummer";
                command.Parameters.AddWithValue("@Uitvaartnummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        uitvaartGegevens = new OverledeneUitvaartModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            DatumTijdCondoleance = reader.IsDBNull(2) ? (DateTime?)null : (DateTime)reader[2],
                            ConsumptiesCondoleance = reader[3].ToString(),
                            TypeDienst = reader[4].ToString(),
                            DatumTijdUitvaart = reader.IsDBNull(5) ? (DateTime?)null : (DateTime)reader[5],
                            LocatieUitvaart = reader[6].ToString(),
                            DatumTijdDienst = reader.IsDBNull(7) ? (DateTime?)null : (DateTime)reader[7],
                            LocatieDienst = reader[8].ToString(),
                            AfscheidDienst = reader[9].ToString(),
                            MuziekDienst = reader[10].ToString(),
                            BeslotenDienst = reader[11].ToString(),
                            VolgAutoDienst = reader[12].ToString(),
                            ConsumptiesDienst = reader[13].ToString(),
                            KistDienst = reader[14].ToString(),
                            CondoleanceYesNo = reader[15].ToString(),
                            Spreker = reader[16].ToString(),
                            PowerPoint = reader[17].ToString(),
                            TijdBlokken = reader.IsDBNull(18) ? (int?)null : (int)reader[18],
                            AantalTijdsBlokken = reader.IsDBNull(19) ? (int?)null : (int)reader[19]
                        };
                    }
                }
            }
            return uitvaartGegevens;
        }
        public OverledeneMiscModel GetOverledeneMiscByUitvaartId(Guid uitvaartId)
        {
            OverledeneMiscModel uitvaartMisc = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, UitvaartId, RouwbrievenId, AantalRouwbrieven, Advertenties, UBS, AantalUitnodigingen, " +
                                        "AantalKennisgeving, AulaNaam, AantalPersonen, BegraafplaatsLocatie, BegraafplaatsGrafNr " +
                                        "FROM [OverledeneUitvaartInfoMisc] WHERE UitvaartId = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        uitvaartMisc = new OverledeneMiscModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            RouwbrievenId = (Guid)reader[2],
                            AantalRouwbrieven = reader[3].ToString(),
                            Advertenties = reader[4].ToString(),
                            UBS = reader[5].ToString(),
                            AantalUitnodigingen = reader[6].ToString(),
                            AantalKennisgeving = reader[7].ToString(),
                            AulaNaam = reader[8].ToString(),
                            AulaPersonen = (int)reader[9],
                            Begraafplaats = reader[10].ToString(),
                            GrafNummer = reader[11].ToString()
                        };
                    }
                }
            }
            return uitvaartMisc;
        }
        public OverledeneAsbestemmingModel GetOverlijdenAsbestemmingInfoByUitvaartId(string uitvaartId)
        {
            OverledeneAsbestemmingModel asbestemmingGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [asbestemmingId], OA.[uitvaartId], [asbestemming], [typeGraf], [bestaandGraf], [zandKelderGraf], [grafmonument] " +
                                        "FROM[OverledeneAsbestemming] OA " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OA.UitvaartId " +
                                        "WHERE Uitvaartnummer = @uitvaartNummer";

                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        asbestemmingGegevens = new OverledeneAsbestemmingModel()
                        {
                            AsbestemmingId = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            Asbestemming = reader[2].ToString(),
                            TypeGraf = reader[3].ToString(),
                            BestaandGraf = reader[4].ToString(),
                            ZandKelderGraf = reader[5].ToString(),
                            GrafMonument = reader[6].ToString(),
                        };
                    }
                }
            }
            return asbestemmingGegevens;
        }
        public IEnumerable<OverledeneBijlagesModel> GetOverlijdenBijlagesByUitvaartId(string uitvaartId)
        {
            var bijlagesList = new List<OverledeneBijlagesModel>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [BijlageId], OB.[UitvaartId], [DocumentName], [DocumentType], [DocumentURL], [DocumentHash], [DocumentInconsistent] " +
                                      "FROM [OverledeneBijlages] OB " +
                                      "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OB.UitvaartId " +
                                      "WHERE Uitvaartnummer = @uitvaartNummer AND isDeleted = 0";

                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bijlagesGegevens = new OverledeneBijlagesModel()
                        {
                            BijlageId = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            DocumentName = reader[2].ToString(),
                            DocumentType = reader[3].ToString(),
                            DocumentUrl = reader[4].ToString(),
                            DocumentHash = reader[5].ToString(),
                            DocumentInconsistent = (bool)reader[6],
                        };

                        bijlagesList.Add(bijlagesGegevens);
                    }
                }
            }
            return bijlagesList;
        }
        public IEnumerable<OverledeneBijlagesModel> GetTerugmeldingenByUitvaartId(Guid uitvaartId)
        {
            var bijlagesList = new List<OverledeneBijlagesModel>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [BijlageId], OB.[UitvaartId], [DocumentName], [DocumentType], [DocumentURL], [DocumentHash], [DocumentInconsistent] " +
                                      "FROM [OverledeneBijlages] OB " +
                                      "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OB.UitvaartId " +
                                      "WHERE Uitvaartnummer = @uitvaartNummer AND DocumentName = 'Terugmelding'";

                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bijlagesGegevens = new OverledeneBijlagesModel()
                        {
                            BijlageId = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            DocumentName = reader[2].ToString(),
                            DocumentType = reader[3].ToString(),
                            DocumentUrl = reader[4].ToString(),
                            DocumentHash = reader[5].ToString(),
                            DocumentInconsistent = (bool)reader[6],
                        };

                        bijlagesList.Add(bijlagesGegevens);
                    }
                }
            }
            return bijlagesList;
        }
        public ObservableCollection<OverledeneSteenhouwerijModel> GetOverlijdenSteenhouwerij()
        {
            ObservableCollection<OverledeneSteenhouwerijModel> steenhouwerijGegevens = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT OS.[Id],OS.[uitvaartId],[steenhouwerOpdracht],[steenhouwerBedrag],[steenhouwerProvisie]," +
                                        "[steenhouwerUitbetaing],CONCAT(CP.Initialen, ' ', CP.Achternaam) as Personeel,CL.leverancierName, Uitvaartnummer, [steenhouwerPaid], steenhouwerProvisieTotaal " +
                                        "FROM [OverledeneSteenhouwer] OS " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OS.UitvaartId " +
                                        "INNER JOIN ConfigurationLeveranciers CL ON OS.steenhouwerLeverancier = CL.leverancierId " +
                                        "INNER JOIN ConfigurationPersoneel CP ON OU.PersoneelId = CP.Id";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        steenhouwerijGegevens.Add(new OverledeneSteenhouwerijModel
                        {
                            SteenhouwerijId = (Guid)reader["Id"],
                            UitvaartId = (Guid)reader["uitvaartId"],
                            UitvaartNummer = reader["uitvaartnummer"].ToString(),
                            SteenhouwerOpdracht = reader["steenhouwerOpdracht"].ToString(),
                            SteenhouwerBedrag = reader["steenhouwerBedrag"].ToString(),
                            SteenhouwerProvisie = reader["steenhouwerProvisie"].ToString(),
                            SteenhouwerUitbetaing = reader["steenhouwerUitbetaing"] != DBNull.Value ? (DateTime?)reader["steenhouwerUitbetaing"] : null,
                            SteenhouwerWerknemer = reader["Personeel"].ToString(),
                            SteenhouwerLeverancierName = reader["leverancierName"].ToString(),
                            SteenhouwerPaid = reader["steenhouwerPaid"] != DBNull.Value && Convert.ToBoolean(reader["steenhouwerPaid"]),
                            SteenhouwerProvisieTotaal = reader["steenhouwerProvisieTotaal"].ToString()
                        });
                    }
                }
            }
            return steenhouwerijGegevens;
        }
        public ObservableCollection<OverledeneSteenhouwerijModel> GetOverlijdenSteenhouwerijByEmployee(Guid EmployeeId)
        {
            ObservableCollection<OverledeneSteenhouwerijModel> steenhouwerijGegevens = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [steenhouwerOpdracht],[steenhouwerBedrag]," +
                                        "CONCAT(CP.Initialen, ' ', CP.Achternaam) as Personeel,CL.leverancierName, Uitvaartnummer " +
                                        "FROM [OverledeneSteenhouwer] OS " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OS.UitvaartId " +
                                        "INNER JOIN ConfigurationLeveranciers CL ON OS.steenhouwerLeverancier = CL.leverancierId " +
                                        "INNER JOIN ConfigurationPersoneel CP ON OU.PersoneelId = CP.Id " +
                                        "WHERE OU.PersoneelId = @employeeId";
                command.Parameters.AddWithValue("@employeeId", EmployeeId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        steenhouwerijGegevens.Add(new OverledeneSteenhouwerijModel
                        {
                            UitvaartNummer = reader["uitvaartnummer"].ToString(),
                            SteenhouwerOpdracht = reader["steenhouwerOpdracht"].ToString(),
                            SteenhouwerBedrag = reader["steenhouwerBedrag"].ToString(),
                            SteenhouwerWerknemer = reader["Personeel"].ToString(),
                            SteenhouwerLeverancierName = reader["leverancierName"].ToString()
                        });
                    }
                }
            }
            return steenhouwerijGegevens;
        }
        public OverledeneSteenhouwerijModel GetOverlijdenSteenhouwerijByUitvaartId(string uitvaartId)
        {
            OverledeneSteenhouwerijModel steenhouwerijGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],OS.[uitvaartId],[steenhouwerOpdracht],[steenhouwerBedrag],[steenhouwerProvisie]," +
                                        "[steenhouwerUitbetaing],[steenhouwerText],[steenhouwerLeverancier] " +
                                        "FROM[OverledeneSteenhouwer] OS " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OS.UitvaartId " +
                                        "WHERE Uitvaartnummer = @uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        steenhouwerijGegevens = new OverledeneSteenhouwerijModel()
                        {
                            SteenhouwerijId = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            SteenhouwerOpdracht = reader[2] != DBNull.Value ? reader[2].ToString() : string.Empty,
                            SteenhouwerBedrag = reader[3] != DBNull.Value ? reader[3].ToString() : string.Empty,
                            SteenhouwerProvisie = reader[4] != DBNull.Value ? reader[4].ToString() : string.Empty,
                            SteenhouwerUitbetaing = reader[5] != DBNull.Value ? (DateTime?)reader[5] : null,
                            SteenhouwerText = reader[6] != DBNull.Value ? reader[6].ToString() : string.Empty,
                            SteenhouwerLeverancier = (Guid)reader[7],
                        };
                    }
                }
            }
            return steenhouwerijGegevens;
        }
        public ObservableCollection<OverledeneBloemenModel> GetOverlijdenBloemen()
        {
            ObservableCollection<OverledeneBloemenModel> bloemenGegevens = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT OB.[Id] as BId ,OB.[uitvaartId] as UitvaartId,bloemenBedrag,bloemenProvisie,bloemenUitbetaling,bloemenLeverancier,CL.leverancierName," +
                                        "CASE WHEN CP.Tussenvoegsel IS NULL OR CP.Tussenvoegsel = '' THEN CP.Achternaam " +
                                        "ELSE CONCAT(CP.Tussenvoegsel, ' ', CP.Achternaam) END AS Achternaam, " +
                                        "CASE WHEN CP.Roepnaam IS NULL OR CP.Roepnaam = '' THEN CP.Voornaam ELSE CP.Roepnaam END AS Voornaam, " +
                                        "CONCAT(Voornaam, ' ', Achternaam) as Personeel, Uitvaartnummer, bloemenPaid " +
                                        "FROM[OverledeneBloemen] OB " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OB.UitvaartId " +
                                        "INNER JOIN ConfigurationLeveranciers CL ON OB.bloemenLeverancier = CL.leverancierId " +
                                        "INNER JOIN ConfigurationPersoneel CP ON OU.PersoneelId = CP.Id";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bloemenGegevens.Add(new OverledeneBloemenModel
                        {
                            BloemenId = reader["BId"] != DBNull.Value ? (Guid)reader["BId"] : Guid.Empty,
                            UitvaartId = reader["UitvaartId"] != DBNull.Value ? (Guid)reader["UitvaartId"] : Guid.Empty,
                            UitvaartNummer = reader["uitvaartnummer"] != DBNull.Value ? reader["uitvaartnummer"].ToString() : string.Empty,
                            BloemenBedrag = reader["bloemenBedrag"] != DBNull.Value ? reader["bloemenBedrag"].ToString() : string.Empty,
                            BloemenProvisie = reader["bloemenProvisie"] != DBNull.Value ? reader["bloemenProvisie"].ToString() : string.Empty,
                            BloemenUitbetaling = reader["bloemenUitbetaling"] != DBNull.Value ? (DateTime?)reader["bloemenUitbetaling"] : null,
                            BloemenLeverancier = reader["bloemenLeverancier"] != DBNull.Value ? (Guid)reader["bloemenLeverancier"] : Guid.Empty,
                            BloemenLeverancierName = reader["leverancierName"] != DBNull.Value ? reader["leverancierName"].ToString() : string.Empty,
                            BloemenWerknemer = reader["Personeel"] != DBNull.Value ? reader["Personeel"].ToString() : string.Empty,
                            BloemenPaid = reader["bloemenPaid"] != DBNull.Value && (bool)reader["bloemenPaid"]
                        });
                    }
                }
            }
            return bloemenGegevens;
        }
        public ObservableCollection<OverledeneBloemenModel> GetOverlijdenBloemenByEmployee(Guid EmployeeId)
        {
            ObservableCollection<OverledeneBloemenModel> bloemenGegevens = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [bloemenBedrag],CL.leverancierName,CONCAT(CP.Roepnaam, ' ', CP.Achternaam) as Personeel, Uitvaartnummer " +
                                        "FROM[OverledeneBloemen] OB " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OB.UitvaartId " +
                                        "INNER JOIN ConfigurationLeveranciers CL ON OB.bloemenLeverancier = CL.leverancierId  " +
                                        "INNER JOIN ConfigurationPersoneel CP ON OU.PersoneelId = CP.Id " +
                                        "WHERE OU.PersoneelId = @employeeId";
                command.Parameters.AddWithValue("@employeeId", EmployeeId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bloemenGegevens.Add(new OverledeneBloemenModel
                        {
                            UitvaartNummer = reader["uitvaartnummer"].ToString(),
                            BloemenBedrag = reader["bloemenBedrag"].ToString(),
                            BloemenLeverancierName = reader["leverancierName"].ToString(),
                            BloemenWerknemer = reader["Personeel"].ToString()
                        });
                    }
                }
            }
            return bloemenGegevens;
        }
        public OverledeneBloemenModel GetOverlijdenBloemenByUitvaartId(string uitvaartId)
        {
            OverledeneBloemenModel bloemenGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],OB.uitvaartId,bloemenText,bloemenLint,bloemenKaart,bloemenBedrag,bloemenProvisie,bloemenUitbetaling," +
                                        "bloemenLeverancier, OBI.DocumentURL as DocUrl, bloemenLintJson, bloemenBezorgingDatum, bloemenBezorgingAdres " +
                                        "FROM [OverledeneBloemen] OB " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OB.UitvaartId " +
                                        "LEFT JOIN OverledeneBijlages OBI ON OB.uitvaartId = OBI.UitvaartId AND OBI.DocumentName = 'Bloemen'" +
                                        "WHERE Uitvaartnummer = @uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bloemenGegevens = new OverledeneBloemenModel()
                        {
                            BloemenId = reader["Id"] != DBNull.Value ? (Guid)reader["Id"] : Guid.Empty,
                            UitvaartId = reader["uitvaartId"] != DBNull.Value ? (Guid)reader["uitvaartId"] : Guid.Empty,
                            BloemenText = reader["bloemenText"] != DBNull.Value ? reader["bloemenText"].ToString() : string.Empty,
                            BloemenLint = reader["bloemenLint"] != DBNull.Value ? Convert.ToBoolean(reader["bloemenLint"]) : false,
                            BloemenKaart = reader["bloemenKaart"] != DBNull.Value ? Convert.ToBoolean(reader["bloemenKaart"]) : false,
                            BloemenBedrag = reader["bloemenBedrag"] != DBNull.Value ? reader["bloemenBedrag"].ToString() : string.Empty,
                            BloemenProvisie = reader["bloemenProvisie"] != DBNull.Value ? reader["bloemenProvisie"].ToString() : string.Empty,
                            BloemenUitbetaling = reader["bloemenUitbetaling"] != DBNull.Value ? (DateTime?)reader["bloemenUitbetaling"] : null,
                            BloemenLeverancier = reader["bloemenLeverancier"] != DBNull.Value ? (Guid)reader["bloemenLeverancier"] : Guid.Empty,
                            BloemenDocument = reader["DocUrl"] != DBNull.Value ? reader["DocUrl"].ToString() : string.Empty,
                            BloemenLintJson = reader["bloemenLintJson"] != DBNull.Value ? reader["bloemenLintJson"].ToString() : string.Empty,
                            BloemenBezorgAdres = reader["bloemenBezorgingAdres"] != DBNull.Value ? reader["bloemenBezorgingAdres"].ToString() : string.Empty,
                            BloemenBezorgDate = reader["bloemenBezorgingDatum"] != DBNull.Value ? (DateTime?)reader["bloemenBezorgingDatum"] : null,
                        };
                    }
                }
            }
            return bloemenGegevens;
        }
        public OverledeneWerkbonUitvaart GetOverlijdenWerkbonnenByUitvaartId(string uitvaartId)
        {
            OverledeneWerkbonUitvaart werkbonGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],OW.[uitvaartId],OW.[werkbonJson], Uitvaartnummer " +
                                        "FROM [OverledeneWerkbon] OW " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OW.UitvaartId " +
                                        "WHERE Uitvaartnummer = @uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        werkbonGegevens = new OverledeneWerkbonUitvaart()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            WerkbonJson = reader[2].ToString(),
                            UitvaartNummer = reader[3].ToString(),
                        };
                    }
                }
            }
            return werkbonGegevens;
        }
        public string GetWerkbonWerknemer(Guid werknemerId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT CASE WHEN Tussenvoegsel IS NOT NULL THEN CONCAT(Voornaam, ' ', Tussenvoegsel, ' ', Achternaam) " +
                                        "ELSE CONCAT(Voornaam, ' ', Achternaam) END AS FullName " +
                                        "FROM [ConfigurationPersoneel] " +
                                        "WHERE [Id] = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", werknemerId);
                var fullName = command.ExecuteScalar();

                if (fullName == null)
                {
                    return "SearchingEmployeeFailed";
                }
                else
                {
                    return fullName.ToString();
                }
            }
        }
        public ObservableCollection<ObservableCollection<WerkbonnenData>> GetOverlijdenWerkbonnen()
        {
            ObservableCollection<ObservableCollection<WerkbonnenData>> werkbonGegevens = new();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id], OW.[uitvaartId], OW.[werkbonJson], Uitvaartnummer " +
                                        "FROM[OverledeneWerkbon] OW " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OW.UitvaartId";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var uitvaartNummer = reader["Uitvaartnummer"].ToString();
                        var werkbonnenDataSet = JsonConvert.DeserializeObject<ObservableCollection<WerkbonnenData>>(reader["werkbonJson"].ToString());

                        if (werkbonnenDataSet != null)
                        {
                            foreach (var werkbonData in werkbonnenDataSet)
                            {
                                werkbonData.UitvaartNummer = uitvaartNummer;
                                werkbonData.WerknemerName = GetWerkbonWerknemer(werkbonData.WerknemerId);
                            }

                            werkbonGegevens.Add(werkbonnenDataSet);
                        }
                    }
                }
            }
            return werkbonGegevens;
        }
        public ObservableCollection<OverledeneUrnSieradenModel> GetOverlijdenUrnSieraden()
        {
            ObservableCollection<OverledeneUrnSieradenModel> urnGegevens = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT OUS.[Id],OUS.[uitvaartId],[urnOpdracht],[urnBedrag],[urnProvisie]," +
                                        "[urnUitbetaling],CONCAT(CP.Initialen, ' ', CP.Achternaam) as Personeel,CL.leverancierName, Uitvaartnummer, [urnPaid] " +
                                        "FROM [OverledeneUrnSieraden] OUS " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OUS.UitvaartId " +
                                        "INNER JOIN ConfigurationLeveranciers CL ON OUS.urnLeverancier = CL.leverancierId " +
                                        "INNER JOIN ConfigurationPersoneel CP ON OU.PersoneelId = CP.Id";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        urnGegevens.Add(new OverledeneUrnSieradenModel
                        {
                            UrnId = (Guid)reader["Id"],
                            UitvaartId = (Guid)reader["uitvaartId"],
                            UitvaartNummer = reader["uitvaartnummer"].ToString(),
                            UrnOpdracht = reader["urnOpdracht"] != DBNull.Value ? reader["urnOpdracht"].ToString() : string.Empty,
                            UrnBedrag = reader["urnBedrag"] != DBNull.Value ? reader["urnBedrag"].ToString() : string.Empty,
                            UrnProvisie = reader["urnProvisie"] != DBNull.Value ? reader["urnProvisie"].ToString() : string.Empty,
                            UrnUitbetaing = reader.IsDBNull(reader.GetOrdinal("urnUitbetaling")) ? (DateTime?)null : (DateTime)reader["urnUitbetaling"],
                            UrnWerknemer = reader["Personeel"] != DBNull.Value ? reader["Personeel"].ToString() : string.Empty,
                            UrnLeverancierName = reader["leverancierName"] != DBNull.Value ? reader["leverancierName"].ToString() : string.Empty,
                            UrnPaid = reader["urnPaid"] != DBNull.Value ? (bool)reader["urnPaid"] : false
                        });
                    }
                }
            }
            return urnGegevens;
        }
        public ObservableCollection<OverledeneUrnSieradenModel> GetOverlijdenUrnSieradenByEmployee(Guid EmployeeId)
        {
            ObservableCollection<OverledeneUrnSieradenModel> urnGegevens = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [urnOpdracht],[urnBedrag],[urnText]," +
                                        "CONCAT(CP.Initialen, ' ', CP.Achternaam) as Personeel,CL.leverancierName, Uitvaartnummer " +
                                        "FROM [OverledeneUrnSieraden] OUS " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OUS.UitvaartId " +
                                        "INNER JOIN ConfigurationLeveranciers CL ON OUS.UrnLeverancier = CL.leverancierId " +
                                        "INNER JOIN ConfigurationPersoneel CP ON OU.PersoneelId = CP.Id " +
                                        "WHERE OU.PersoneelId = @employeeId";
                command.Parameters.AddWithValue("@employeeId", EmployeeId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        urnGegevens.Add(new OverledeneUrnSieradenModel
                        {
                            UitvaartNummer = reader["uitvaartnummer"].ToString(),
                            UrnOpdracht = reader["urnOpdracht"].ToString(),
                            UrnBedrag = reader["urnBedrag"].ToString(),
                            UrnWerknemer = reader["Personeel"].ToString(),
                            UrnText = reader["urnText"].ToString(),
                            UrnLeverancierName = reader["leverancierName"].ToString()
                        });
                    }
                }
            }
            return urnGegevens;
        }
        public OverledeneUrnSieradenModel GetOverlijdenUrnSieradenByUitvaartId(string uitvaartId)
        {
            OverledeneUrnSieradenModel urnGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],OUS.[uitvaartId],[UrnOpdracht],[UrnBedrag],[UrnProvisie]," +
                                        "[urnUitbetaling],[UrnText],[UrnLeverancier] " +
                                        "FROM [OverledeneUrnSieraden] OUS " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OUS.UitvaartId " +
                                        "WHERE Uitvaartnummer = @uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        urnGegevens = new OverledeneUrnSieradenModel()
                        {
                            UrnId = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            UrnOpdracht = reader[2].ToString(),
                            UrnBedrag = reader[3].ToString(),
                            UrnProvisie = reader[4].ToString(),
                            UrnText = reader[6].ToString(),
                            UrnLeverancier = (Guid)reader[7],
                        };
                    }
                }
            }
            return urnGegevens;
        }
        public FactuurModel GetPolisInfoByUitvaartId(string uitvaartId)
        {
            FactuurModel kostenbegrotingGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, OVI.uitvaartId, verzekeringProperties FROM OverledeneVerzerkeringInfo OVI " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OVI.uitvaartId = OU.UitvaartId " +
                                        "WHERE OU.Uitvaartnummer = @uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        kostenbegrotingGegevens = new FactuurModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            PolisJson = reader[2].ToString()
                        };
                    }
                }
            }
            return kostenbegrotingGegevens;
        }
        public FactuurModel GetOverlijdenKostenbegrotingByUitvaartId(string uitvaartId)
        {
            FactuurModel kostenbegrotingGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT OFA.Id,OFA.uitvaartId,[kostenbegrotingUrl],[kostenbegrotingJson],[kostenbegrotingCreationDate],[kostenbegrotingCreated],[KostenbegrotingVerzekeraar], [verzekeringProperties] " +
                                        "FROM [OverledeneFacturen] AS OFA " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OFA.uitvaartId = OU.UitvaartId " +
                                        "LEFT JOIN OverledeneVerzerkeringInfo  OVI ON OFA.uitvaartId = OVI.uitvaartId " +
                                        "WHERE OU.Uitvaartnummer = @uitvaartNummer";
                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        kostenbegrotingGegevens = new FactuurModel()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            KostenbegrotingUrl = reader[2].ToString(),
                            KostenbegrotingJson = reader[3].ToString(),
                            KostenbegrotingCreationDate = (DateTime)reader[4],
                            KostenbegrotingCreated = (bool)reader[5],
                            KostenbegrotingVerzekeraar = (Guid)reader[6],
                            PolisJson = reader[7].ToString()
                        };
                    }
                }
            }
            return kostenbegrotingGegevens;
        }
        public async Task<string> GetOverlijdenKostenbegrotingAsync(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = "SELECT [kostenbegrotingUrl] FROM [OverledeneFacturen] WHERE[uitvaartId] = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                return (string)await command.ExecuteScalarAsync();
            }
        }
        public GenerateFactuur GetGenerateFactuurDataByUitvaartId(Guid UitvaartId)
        {
            GenerateFactuur factuurGegevens = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT TOP(1) OPG.uitvaartId,OU.Uitvaartnummer,[kostenbegrotingJson],OO.opdrachtgeverAanhef, OO.opdrachtgeverVoornaamen, OO.opdrachtgeverTussenvoegsel, OO.opdrachtgeverAchternaam, " +
                                        "OO.opdrachtgeverStraat, OO.opdrachtgeverHuisnummer, oo.opdrachtgeverHuisnummerToevoeging, OO.opdrachtgeverPostcode, OO.opdrachtgeverWoonplaats, " +
                                        "OPG.overledeneAanhef, OPG.overledeneVoornamen, OPG.overledeneTussenvoegsel, OPG.overledeneAchternaam, CAST(OI.overledenDatumTijd AS DATE) AS OverledeneOpDatum, OI.overledenLidnummer, OVI.verzekeringProperties, " +
                                        "CV.factuurType, CV.addressStreet, CV.addressHousenumber, CV.addressHousenumberAddition, CV.postbusAddress, CV.postbusNaam, CV.addressZipcode, CV.addressCity, CV.correspondentieType, CV.verzekeraarNaam," +
                                        "overledeneVoorregeling " +
                                        "FROM [OverledeneFacturen] OFA " +
                                        "INNER JOIN OverledeneUitvaartleider OU ON OFA.uitvaartId = OU.UitvaartId " +
                                        "INNER JOIN OverledenePersoonsGegevens OPG ON OFA.uitvaartId = OPG.uitvaartId " +
                                        "LEFT JOIN [OverledeneOpdrachtgever] OO ON OO.uitvaartId = OFA.uitvaartId " +
                                        "INNER JOIN [OverledeneOverlijdenInfo] OI ON OI.uitvaartId = OFA.uitvaartId " +
                                        "INNER JOIN OverledeneVerzerkeringInfo OVI ON OVI.uitvaartId = OFA.uitvaartId " +
                                        "LEFT JOIN [ConfigurationVerzekeraar] CV ON CV.Id = OI.overledenHerkomst " +
                                        "WHERE OPG.uitvaartId = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        factuurGegevens = new GenerateFactuur()
                        {
                            UitvaartId = (Guid)reader["uitvaartId"],
                            UitvaartNummer = reader["Uitvaartnummer"].ToString(),
                            KostenbegrotingJson = reader["kostenbegrotingJson"].ToString(),
                            OpdrachtgeverAanhef = reader["opdrachtgeverAanhef"].ToString(),
                            OpdrachtgeverVoornamen = reader["opdrachtgeverVoornaamen"].ToString(),
                            OpdrachtgeverTussenvoegsel = reader["opdrachtgeverTussenvoegsel"].ToString(),
                            OpdrachtgeverAchternaam = reader["opdrachtgeverAchternaam"].ToString(),
                            OpdrachtgeverStraat = reader["opdrachtgeverStraat"].ToString(),
                            OpdrachtgeverHuisnummer = reader["opdrachtgeverHuisnummer"].ToString(),
                            OpdrachtgeverHuisnummerToevoeging = reader["opdrachtgeverHuisnummerToevoeging"].ToString(),
                            OpdrachtgeverPostcode = reader["opdrachtgeverPostcode"].ToString(),
                            OpdrachtgeverWoonplaats = reader["opdrachtgeverWoonplaats"].ToString(),
                            OverledeneAanhef = reader["overledeneAanhef"].ToString(),
                            OverledeneVoornamen = reader["overledeneVoornamen"].ToString(),
                            OverledeneTussenvoegsel = reader["overledeneTussenvoegsel"].ToString(),
                            OverledeneAchternaam = reader["overledeneAchternaam"].ToString(),
                            OverledeneOpDatum = ((DateTime)reader[16]).Date,
                            OverledeneLidnummer = reader["overledenLidnummer"].ToString(),
                            OverledeneVerzekeringJson = reader["verzekeringProperties"].ToString(),
                            FactuurType = reader["factuurType"].ToString(),
                            HerkomstStreet = reader["addressStreet"].ToString(),
                            HerkomstHousenumber = reader["addressHousenumber"].ToString(),
                            HerkomstHousenumberAddition = reader["addressHousenumberAddition"].ToString(),
                            HerkomstPostbus = reader["postbusAddress"].ToString(),
                            HerkomstPostbusName = reader["postbusNaam"].ToString(),
                            HerkomstZipcode = reader["addressZipcode"].ToString(),
                            HerkomstCity = reader["addressCity"].ToString(),
                            CorrespondentieType = reader["correspondentieType"].ToString(),
                            OverledeneVoorregeling = reader["overledeneVoorregeling"] == DBNull.Value ? false : (bool)reader["overledeneVoorregeling"],
                        };
                    }
                }
            }
            return factuurGegevens;
        }
        public IEnumerable<OverledeneSearchSurname> GetUitvaartleiderBySurnameOverledene(string overledeneAchternaam, DateTime overledeneGeboortedatum)
        {
            var overledeneList = new List<OverledeneSearchSurname>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT OUL.UitvaartId, OUL.Uitvaartnummer, OPG.overledeneAanhef, OPG.overledeneVoornamen, OPG.overledeneTussenvoegsel, OPG.overledeneAchternaam, OPG.overledeneGeboortedatum, CONCAT(Initialen,' ',Achternaam) as PersoneelName, PersoneelId, dossierCompleted " +
                                      "FROM OverledeneUitvaartleider AS OUL " +
                                      "INNER JOIN OverledenePersoonsGegevens AS OPG ON OPG.UitvaartId = OUL.UitvaartId " +
                                      "LEFT JOIN ConfigurationPersoneel AS CP ON CP.Id = OUL.PersoneelId " +
                                      "WHERE OPG.overledeneAchternaam LIKE @overledeneAchternaam " +
                                      "AND (OPG.overledeneGeboortedatum LIKE @overledeneGeboortedatum OR @overledeneGeboortedatum IS NULL)";

                command.Parameters.AddWithValue("@overledeneAchternaam", "%" + overledeneAchternaam + "%");
                command.Parameters.AddWithValue("@overledeneGeboortedatum", overledeneGeboortedatum == default(DateTime) ? (object)DBNull.Value : overledeneGeboortedatum);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var searchGegevens = new OverledeneSearchSurname()
                        {
                            UitvaartId = (Guid)reader[0],
                            UitvaartNummer = reader[1].ToString(),
                            OverledeneAanhef = reader[2].ToString(),
                            OverledeneVoornaam = reader[3].ToString(),
                            OverledeneTussenvoegsel = reader[4].ToString(),
                            OverledeneAchternaam = reader[5].ToString(),
                            OverledeneGeboortedatum = (DateTime)reader[6],
                            PersoneelNaam = reader[7].ToString(),
                            PersoneelId = (Guid)reader[8],
                            DossierCompleted = reader[9] != DBNull.Value ? (bool)reader[4] : false,
                        };

                        overledeneList.Add(searchGegevens);
                    }
                }
            }
            return overledeneList;
        }
    }
}
