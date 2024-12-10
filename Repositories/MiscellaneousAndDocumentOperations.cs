using Dossier_Registratie.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Dossier_Registratie.Repositories
{
    public class MiscellaneousAndDocumentOperations : RepositoryBase, IMiscellaneousAndDocumentOperations
    {
        public async Task<ObservableCollection<NotificatieOverzichtModel>> NotificationDeceasedAfterYearPassedAsync()
        {
            ObservableCollection<NotificatieOverzichtModel> notificatie = new();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = @"SELECT oul.UitvaartId, Uitvaartnummer, 
                                        CASE 
                                            WHEN overledeneTussenvoegsel IS NULL OR overledeneTussenvoegsel = '' 
                                            THEN CONCAT(overledeneAanhef, ' ', overledeneAchternaam) 
                                            ELSE CONCAT(overledeneAanhef, ' ', overledeneTussenvoegsel, ' ', overledeneAchternaam) 
                                        END AS VolledigeAchternaam, 
                                        ooi.overledenDatumTijd, oo.opdrachtgeverTelefoon, cu.WindowsUsername, okt.Cijfer,
	                                    CASE
		                                    WHEN opdrachtgeverTussenvoegsel IS NULL OR opdrachtgeverTussenvoegsel = ''
		                                    THEN CONCAT(opdrachtgeverAanhef, ' ', opdrachtgeverAchternaam)
		                                    ELSE CONCAT(opdrachtgeverAanhef, ' ', opdrachtgeverTussenvoegsel, ' ', opdrachtgeverAchternaam) 
	                                    END AS Opdrachtgever
                                    FROM OverledeneKlantTevredenheid okt 
                                    INNER JOIN OverledeneUitvaartleider oul ON okt.UitvaartId = oul.UitvaartId 
                                    INNER JOIN ConfigurationPersoneel cp ON oul.PersoneelId = cp.Id 
                                    INNER JOIN ConfigurationUsers cu ON cp.Id = cu.PersoneelId 
                                    INNER JOIN OverledenePersoonsGegevens opg ON oul.UitvaartId = opg.UitvaartId 
                                    INNER JOIN OverledeneOverlijdenInfo ooi ON opg.uitvaartId = ooi.UitvaartId 
                                    INNER JOIN OverledeneOpdrachtgever oo ON opg.uitvaartId = oo.uitvaartId
                                    WHERE okt.NotificatieOverleden = 1 
                                        AND overledenDatumTijd <= DATEADD(YEAR, -1, GETDATE())";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        notificatie.Add(new NotificatieOverzichtModel
                        {
                            UitvaartId = (Guid)reader[0],
                            UitvaartNr = reader[1].ToString(),
                            OverledeneNaam = reader[2].ToString(),
                            OverledenDatumTijd = (DateTime)reader[3],
                            OpdrachtTelefoon = reader[4].ToString(),
                            WindowsAccount = reader[5].ToString(),
                            Cijfer = reader[6].ToString(),
                            Opdrachtgever = reader[7].ToString()
                        });
                    }
                }
            }

            return notificatie;
        }
        public (Guid herkomstId, string herkomstName, bool herkomstLogo) GetHerkomstByUitvaartId(Guid uitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT overledenHerkomst, verzekeraarNaam, CustomLogo " +
                    "FROM OverledeneOverlijdenInfo " +
                    "INNER JOIN ConfigurationVerzekeraar on overledenHerkomst = ConfigurationVerzekeraar.Id " +
                    "WHERE UitvaartId = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", uitvaartId);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var herkomstId = reader.IsDBNull(0) ? Guid.Empty : reader.GetGuid(0);
                    var herkomstNamee = reader.IsDBNull(1) ? "None" : reader.GetString(1);
                    var herkomstCustomLogo = reader.IsDBNull(2) ? false : reader.GetBoolean(2);
                    return (herkomstId, herkomstNamee, herkomstCustomLogo);
                }
                return (Guid.Empty, "None", false);
            }
        }
        public bool UitvaarnummerExists(string uitvaartnummer)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneUitvaartleider] WHERE [Uitvaartnummer] = @uitvaartnummer";
                command.Parameters.AddWithValue("@uitvaartnummer", uitvaartnummer);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaartPersoonsgegevensExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledenePersoonsGegevens] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaartOverlijdenInfoExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneOverlijdenInfo] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaartExtraInfoExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneExtraInfo] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaartOpdrachtgeverPersoonsgegevensExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneOpdrachtgever] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaartVerzekeringExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneVerzerkeringInfo] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaartOpbarenExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneOpbaring] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarInfoExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneUitvaartInfo] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarInfoMiscExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneUitvaartInfoMisc] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarAsbestemmingExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneAsbestemming] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarFactuurExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneFacturen] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarKlanttevredenheidExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneKlantTevredenheid] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarKWerkbonExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneWerkbon] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarKUrnSieradenExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneUrnSieraden] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarBloemenExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneBloemen] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        public bool UitvaarSteenhouwerijExists(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [OverledeneSteenhouwer] WHERE [uitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartId);

                return (int)command.ExecuteScalar() > 0;
            }
        }
        //OverledeneSteenhouwer
        public async Task<FactuurInfoCrematie> GetFactuurInfo(Guid HerkomstId)
        {
            FactuurInfoCrematie info = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT verzekeraarNaam,OverrideFactuurAdress," +
                                        "CASE " +
                                        "WHEN correspondentieType = 'Postbus' " +
                                        "THEN CONCAT(postbusNaam, ', ', postbusAddress) " +
                                        "WHEN correspondentieType = 'Adres' " +
                                        "THEN CONCAT(addressStreet, ' ', addressHousenumber, " +
                                        "ISNULL(addressHousenumberAddition, '')) " +
                                        "ELSE NULL " +
                                        "END AS CorrespondenceAddress, addressZipcode, addressCity, verzekeraarTelefoon " +
                                        "FROM [ConfigurationVerzekeraar] " +
                                        "WHERE Id = @HerkomstId";
                command.Parameters.AddWithValue("@HerkomstId", HerkomstId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        info = new FactuurInfoCrematie()
                        {
                            FactuurAdresNaam = reader["verzekeraarNaam"].ToString(),
                            FactuurAdresRelatie = "Lid",
                            FactuurAdresStraat = reader["CorrespondenceAddress"].ToString(),
                            FactuurAdresPostcode = reader["addressZipcode"].ToString(),
                            FactuurAdresPlaats = reader["addressCity"].ToString(),
                            FactuurAdresGeslacht = "-",
                            FactuurAdresTelefoon = reader["verzekeraarTelefoon"].ToString(),
                        };
                    }
                }
            }
            return info;
        }
        public (byte[] DocumentData, string DocumentType) GetLogoBlob(string AppType)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT DocumentData, DocumentType FROM ConfigurationBlob WHERE AppType = @AppType";
                command.Parameters.AddWithValue("@AppType", AppType);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var documentData = reader["DocumentData"] as byte[] ?? Array.Empty<byte>();
                        var documentType = reader["DocumentType"] as string ?? string.Empty;
                        return (documentData, documentType);
                    }
                }
            }
            return (Array.Empty<byte>(), string.Empty);
        }
        public IEnumerable<SuggestionModel> GetSuggestions()
        {
            var suggestions = new List<SuggestionModel>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],[ShortName],[LongName],[Street],[Housenumber],[Zipcode],[City],[County],[IsDeleted] " +
                                        "FROM [ConfigurationOverledenLocaties] ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if ((bool)reader["IsDeleted"] == false)
                        {
                            suggestions.Add(new SuggestionModel
                            {
                                Id = (Guid)reader["Id"],
                                ShortName = reader["ShortName"].ToString(),
                                LongName = reader["LongName"].ToString(),
                                Street = reader["Street"].ToString(),
                                HouseNumber = reader["HouseNumber"].ToString(),
                                ZipCode = reader["ZipCode"].ToString(),
                                City = reader["City"].ToString(),
                                County = reader["County"].ToString(),
                                IsDeleted = reader["IsDeleted"] as bool?
                            });
                        }
                    }
                }
            }
            return suggestions;
        }
        public SuggestionModel GetSuggestionBeheer(Guid suggestionId)
        {
            SuggestionModel suggestion = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],[ShortName],[LongName],[Street],[Housenumber],[Zipcode],[City],[County],[IsDeleted] " +
                                        "FROM [ConfigurationOverledenLocaties] WHERE [Id] = @suggestionId";
                command.Parameters.AddWithValue("@suggestionId", suggestionId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        suggestion = new SuggestionModel()
                        {
                            Id = (Guid)reader["Id"],
                            ShortName = reader["ShortName"].ToString(),
                            LongName = reader["LongName"].ToString(),
                            Street = reader["Street"].ToString(),
                            HouseNumber = reader["Housenumber"].ToString(),
                            ZipCode = reader["Zipcode"].ToString(),
                            City = reader["City"].ToString(),
                            County = reader["County"].ToString(),
                            IsDeleted = (bool)reader["isDeleted"]
                        };
                    }
                }
            }
            return suggestion;
        }
        public ObservableCollection<SuggestionModel> GetSuggestionsBeheer()
        {
            ObservableCollection<SuggestionModel> suggesties = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],[ShortName],[LongName],[Street],[Housenumber],[Zipcode],[City],[County],[IsDeleted] " +
                                        "FROM [ConfigurationOverledenLocaties] ";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = ((bool)reader[8]) ? "Red" : "Green";

                        suggesties.Add(new SuggestionModel
                        {

                            Id = (Guid)reader[0],
                            ShortName = reader[1].ToString(),
                            LongName = reader[2].ToString(),
                            Street = reader[3].ToString(),
                            HouseNumber = reader[4].ToString(),
                            ZipCode = reader[5].ToString(),
                            City = reader[6].ToString(),
                            County = reader[7].ToString(),
                            IsDeleted = (bool)reader[8],
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return suggesties;
        }
        public ActiveAccountModel GetActiveUsers()
        {
            ActiveAccountModel activeUsers = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Username, MachineName, LoginTime, LogoutTime ORDER BY LoginTime DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activeUsers = new ActiveAccountModel()
                        {
                            UserName = reader["Username"].ToString(),
                            MachineName = reader["MachineName"].ToString(),
                            LoginTime = (DateTime)reader["LoginTime"],
                            LogoutTime = (DateTime)reader["LogoutTime"]
                        };
                    }
                }
            };
            return activeUsers;
        }
        public ObservableCollection<OverledeneRouwbrieven> GetAdvertenties()
        {
            ObservableCollection<OverledeneRouwbrieven> rouwbrieven = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT rouwbrievenId, rouwbrievenName, isDeleted FROM [ConfigurationRouwbrieven]";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = ((bool)reader[2]) ? "Red" : "Green";

                        rouwbrieven.Add(new OverledeneRouwbrieven
                        {
                            RouwbrievenId = (Guid)reader[0],
                            RouwbrievenName = (string)reader[1],
                            IsDeleted = (bool)reader[2],
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return rouwbrieven;
        }
        public ObservableCollection<VerzekeraarsModel> GetVerzekeraars()
        {
            ObservableCollection<VerzekeraarsModel> Verzekeraars = new();
            using (var connecion = GetConnection())
            using (var command = new SqlCommand())
            {
                connecion.Open();
                command.Connection = connecion;
                command.CommandText = "SELECT [Id],[verzekeraarNaam],[isHerkomst],[isVerzekeraar],[hasLidnummer],[isDeleted], " +
                    "[addressStreet],[addressHousenumber],[addressHousenumberAddition],[addressZipcode],[addressCity],[factuurType], " +
                    "postbusAddress, postbusNaam, isPakket, customLogo " +
                    "FROM ConfigurationVerzekeraar " +
                    "ORDER BY isDeleted ASC, verzekeraarNaam";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = (reader["isDeleted"] == DBNull.Value || !(bool)reader["isDeleted"]) ? "Green" : "Red";

                        Verzekeraars.Add(new VerzekeraarsModel
                        {
                            Id = (Guid)reader["Id"],
                            Name = reader["verzekeraarNaam"].ToString(),
                            IsHerkomst = (bool)reader["isHerkomst"],
                            IsVerzekeraar = (bool)reader["isVerzekeraar"],
                            HasLidnummer = (bool)reader["hasLidnummer"],
                            IsDeleted = (bool)reader["isDeleted"],
                            AddressStreet = reader["addressStreet"].ToString(),
                            AddressHousenumber = reader["addressHousenumber"].ToString(),
                            AddressHousenumberAddition = reader["addressHousenumberAddition"].ToString(),
                            AddressZipCode = reader["addressZipcode"].ToString(),
                            AddressCity = reader["addressCity"].ToString(),
                            FactuurType = reader["factuurType"].ToString(),
                            PostbusAddress = reader["postbusAddress"].ToString(),
                            PostbusName = reader["postbusNaam"].ToString(),
                            Pakket = reader.IsDBNull("isPakket") ? false : reader.GetBoolean("isPakket"),
                            CustomLogo = reader.IsDBNull("customLogo") ? false : reader.GetBoolean("customLogo"),
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return Verzekeraars;
        }
        public ObservableCollection<VerzekeraarsModel> GetHerkomst()
        {
            ObservableCollection<VerzekeraarsModel> Herkomst = new();
            using (var connecion = GetConnection())
            using (var command = new SqlCommand())
            {
                connecion.Open();
                command.Connection = connecion;
                command.CommandText = "SELECT [Id],[verzekeraarNaam],[isHerkomst],[isVerzekeraar],[hasLidnummer],[isDeleted], " +
                    "[addressStreet],[addressHousenumber],[addressHousenumberAddition],[addressZipcode],[addressCity],[factuurType], " +
                    "postbusAddress, postbusNaam " +
                    "FROM ConfigurationVerzekeraar";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = (reader["isDeleted"] == DBNull.Value || !(bool)reader["isDeleted"]) ? "Green" : "Red";

                        Herkomst.Add(new VerzekeraarsModel
                        {
                            Id = (Guid)reader["Id"],
                            Name = reader["verzekeraarNaam"].ToString(),
                            IsHerkomst = (bool)reader["isHerkomst"],
                            IsVerzekeraar = (bool)reader["isVerzekeraar"],
                            HasLidnummer = (bool)reader["hasLidnummer"],
                            IsDeleted = (bool)reader["isDeleted"],
                            AddressStreet = reader["addressStreet"].ToString(),
                            AddressHousenumber = reader["addressHousenumber"].ToString(),
                            AddressHousenumberAddition = reader["addressHousenumberAddition"].ToString(),
                            AddressZipCode = reader["addressZipcode"].ToString(),
                            AddressCity = reader["addressCity"].ToString(),
                            FactuurType = System.Net.WebUtility.HtmlDecode(reader["factuurType"].ToString()),
                            PostbusAddress = reader["postbusAddress"].ToString(),
                            PostbusName = reader["postbusNaam"].ToString(),
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return Herkomst;
        }
        public VerzekeraarsModel GetVerzekeraarsById(Guid verzekeringId)
        {
            VerzekeraarsModel Verzekeraar = new();
            using (var connecion = GetConnection())
            using (var command = new SqlCommand())
            {
                connecion.Open();
                command.Connection = connecion;
                command.CommandText = "SELECT [Id],[verzekeraarNaam],[isHerkomst],[isVerzekeraar],[hasLidnummer],[isDeleted]," +
                                        "[addressStreet],[addressHousenumber],[addressHousenumberAddition],[addressZipcode],[addressCity],[factuurType]," +
                                        "postbusAddress, postbusNaam, [correspondentieType],[OverrideFactuurAdress],[verzekeraarTelefoon], isPakket, CustomLogo " +
                                        "FROM ConfigurationVerzekeraar WHERE id = @verzekeraarId";
                command.Parameters.AddWithValue("@verzekeraarId", verzekeringId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Verzekeraar = new VerzekeraarsModel()
                        {
                            Id = (Guid)reader["Id"],
                            Name = reader["verzekeraarNaam"].ToString(),
                            IsHerkomst = (bool)reader["isHerkomst"],
                            IsVerzekeraar = (bool)reader["isVerzekeraar"],
                            HasLidnummer = (bool)reader["hasLidnummer"],
                            IsDeleted = (bool)reader["isDeleted"],
                            AddressStreet = reader["addressStreet"].ToString(),
                            AddressHousenumber = reader["addressHousenumber"].ToString(),
                            AddressHousenumberAddition = reader["addressHousenumberAddition"].ToString(),
                            AddressZipCode = reader["addressZipcode"].ToString(),
                            AddressCity = reader["addressCity"].ToString(),
                            FactuurType = System.Net.WebUtility.HtmlDecode(reader["factuurType"].ToString()),
                            PostbusAddress = reader["postbusAddress"].ToString(),
                            PostbusName = reader["postbusNaam"].ToString(),
                            CorrespondentieType = reader["correspondentieType"].ToString(),
                            IsOverrideFactuurAdress = (bool)reader["OverrideFactuurAdress"],
                            Telefoon = reader["verzekeraarTelefoon"].ToString(),
                            Pakket = reader["isPakket"] is DBNull ? false : (bool)reader["isPakket"],
                            CustomLogo = reader["CustomLogo"] is DBNull ? false : (bool)reader["CustomLogo"]
                        };
                    }
                }
            }
            return Verzekeraar;
        }
        public VerzekeraarsModel GetHerkomstById(Guid herkomstId)
        {
            VerzekeraarsModel Herkomst = new();
            using (var connecion = GetConnection())
            using (var command = new SqlCommand())
            {
                connecion.Open();
                command.Connection = connecion;
                command.CommandText = "SELECT [Id],[verzekeraarNaam],[isHerkomst],[isVerzekeraar],[hasLidnummer],[isDeleted]," +
                                        "[addressStreet],[addressHousenumber],[addressHousenumberAddition],[addressZipcode],[addressCity],[factuurType],postbusAddress, postbusNaam, [correspondentieType],[OverrideFactuurAdress],[verzekeraarTelefoon] " +
                                        "FROM ConfigurationVerzekeraar WHERE id = @herkomstId";
                command.Parameters.AddWithValue("@herkomstId", herkomstId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Herkomst = new VerzekeraarsModel()
                        {
                            Id = (Guid)reader["Id"],
                            Name = reader["verzekeraarNaam"].ToString(),
                            IsHerkomst = (bool)reader["isHerkomst"],
                            IsVerzekeraar = (bool)reader["isVerzekeraar"],
                            HasLidnummer = (bool)reader["hasLidnummer"],
                            IsDeleted = (bool)reader["isDeleted"],
                            AddressStreet = reader["addressStreet"].ToString(),
                            AddressHousenumber = reader["addressHousenumber"].ToString(),
                            AddressHousenumberAddition = reader["addressHousenumberAddition"].ToString(),
                            AddressZipCode = reader["addressZipcode"].ToString(),
                            AddressCity = reader["addressCity"].ToString(),
                            FactuurType = System.Net.WebUtility.HtmlDecode(reader["factuurType"].ToString()),
                            PostbusAddress = reader["postbusAddress"].ToString(),
                            PostbusName = reader["postbusNaam"].ToString(),
                            CorrespondentieType = reader["correspondentieType"].ToString(),
                            IsOverrideFactuurAdress = (bool)reader["OverrideFactuurAdress"],
                            Telefoon = reader["verzekeraarTelefoon"].ToString()
                        };
                    }
                }
            }
            return Herkomst;
        }
        public ObservableCollection<UitvaartLeiderModel> GetUitvaartleiders()
        {
            ObservableCollection<UitvaartLeiderModel> Uitvaartleiders = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, CONCAT(Initialen,' ',Achternaam) as PersoneelName FROM [ConfigurationPersoneel] WHERE [isDeleted] = 0 AND [isUitvaartverzorger] = 1";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Uitvaartleiders.Add(new UitvaartLeiderModel { Id = (Guid)reader[0], Uitvaartleider = reader[1].ToString() });
                    }
                }
            }
            return Uitvaartleiders;
        }
        public ObservableCollection<WerknemersModel> GetWerknemers()
        {
            ObservableCollection<WerknemersModel> Werknemers = new ObservableCollection<WerknemersModel>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],[Initialen],[Voornaam],[Roepnaam],[Tussenvoegsel],[Achternaam],[Geboorteplaats],[Geboortedatum]," +
                                        "[Email],[isDeleted],[isUitvaartverzorger],[isDrager],[isChauffeur],[Mobiel] " +
                                        "FROM [ConfigurationPersoneel] " +
                                        "ORDER BY isDeleted, Achternaam ASC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string voornaam = reader[2].ToString();
                        string tussenvoegsel = reader[4].ToString();
                        string achternaam = reader[5].ToString();

                        string fullName;
                        if (!string.IsNullOrEmpty(tussenvoegsel))
                        {
                            fullName = $"{voornaam} {tussenvoegsel} {achternaam}";
                        }
                        else
                        {
                            fullName = $"{voornaam} {achternaam}";
                        }

                        string styleColor;
                        styleColor = ((bool)reader[9]) ? "Red" : "Green";

                        Werknemers.Add(new WerknemersModel
                        {
                            Id = (Guid)reader[0],
                            Initialen = reader[1].ToString(),
                            Voornaam = reader[2].ToString(),
                            Roepnaam = reader[3].ToString(),
                            Tussenvoegsel = reader[4].ToString(),
                            Achternaam = reader[5].ToString(),
                            VolledigeNaam = fullName,
                            Geboorteplaats = reader[6].ToString(),
                            Geboortedatum = (DateTime)reader[7],
                            Email = reader[8].ToString(),
                            IsDeleted = (bool)reader[9],
                            IsUitvaartverzorger = (bool)reader[10],
                            IsDrager = (bool)reader[11],
                            IsChauffeur = (bool)reader[12],
                            Mobiel = reader[13].ToString(),
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return Werknemers;
        }
        public ObservableCollection<AgendaModel> GetAgenda()
        {
            ObservableCollection<AgendaModel> agendaItems = new ObservableCollection<AgendaModel>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT OUL.UitvaartId AS UitvaartId,OUL.Uitvaartnummer AS UitvaartNummer, CASE WHEN overledeneTussenvoegsel IS NOT NULL THEN CONCAT(overledeneTussenvoegsel, ' ', overledeneAchternaam) ELSE overledeneAchternaam END AS Achternaam, " +
                                        "overledeneVoornamen AS Voornamen, uitvaartInfoDatumTijdUitvaart, uitvaartInfoDienstDatumTijd, CONCAT(CP.Initialen, ' ', CP.Achternaam) AS Uitvaartleider " +
                                        "FROM [OverledenePersoonsGegevens] OPG " +
                                        "INNER JOIN[OverledeneUitvaartInfo] OUI ON OPG.UitvaartId = OUI.uitvaartId " +
                                        "INNER JOIN[OverledeneUitvaartleider] OUL ON OPG.UitvaartId = OUL.UitvaartId " +
                                        "INNER JOIN[ConfigurationPersoneel] CP ON OUL.PersoneelId = CP.Id " +
                                        "WHERE uitvaartInfoDatumTijdUitvaart IS NOT NULL AND uitvaartInfoDatumTijdUitvaart >= GETDATE() AND uitvaartInfoDatumTijdUitvaart < DATEADD(DAY, 8, GETDATE()) " +
                                        "ORDER BY uitvaartInfoDatumTijdUitvaart, uitvaartInfoDienstDatumTijd";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        agendaItems.Add(new AgendaModel
                        {
                            UitvaartId = (Guid)reader["UitvaartId"],
                            UitvaartNr = reader["UitvaartNummer"].ToString(),
                            Achternaam = reader["Achternaam"].ToString(),
                            Voornamen = reader["Voornamen"].ToString(),
                            DatumTijdUitvaart = ((DateTime)reader["uitvaartInfoDatumTijdUitvaart"]).Date,
                            TijdstipDienst = ((DateTime)reader["uitvaartInfoDienstDatumTijd"]).TimeOfDay,
                            Uitvaartleider = reader["Uitvaartleider"].ToString()
                        });
                    }
                }
            }
            return agendaItems;
        }
        public ObservableCollection<UitvaartOverzichtModel> GetUitvaartOverzicht()
        {
            ObservableCollection<UitvaartOverzichtModel> overzichtItems = new ObservableCollection<UitvaartOverzichtModel>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT OUL.UitvaartId AS UitvaartId,OUL.Uitvaartnummer AS UitvaartNummer, CASE WHEN overledeneTussenvoegsel IS NOT NULL THEN CONCAT(overledeneTussenvoegsel, ' ', overledeneAchternaam) ELSE overledeneAchternaam END AS Achternaam, " +
                                        "overledeneVoornamen AS Voornamen, overledenDatumTijd AS DatumTijdOverleden, CONCAT(CP.Initialen, ' ', CP.Achternaam) AS Uitvaartleider, overledeneVoorregeling " +
                                        "FROM [OverledenePersoonsGegevens] OPG " +
                                        "LEFT JOIN[OverledeneOverlijdenInfo] OOI ON OPG.UitvaartId = OOI.uitvaartId " +
                                        "INNER JOIN[OverledeneUitvaartleider] OUL ON OPG.UitvaartId = OUL.UitvaartId " +
                                        "INNER JOIN[ConfigurationPersoneel] CP ON OUL.PersoneelId = CP.Id " +
                                        "ORDER BY CASE WHEN ISNUMERIC(Uitvaartnummer) = 1 THEN CAST(Uitvaartnummer AS INT) " +
                                        "ELSE CAST(SUBSTRING(Uitvaartnummer, PATINDEX('%[0-9]%', Uitvaartnummer), LEN(Uitvaartnummer)) AS INT) END DESC, " +
                                        "Uitvaartleider";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool? voorregeling = reader["overledeneVoorregeling"] as bool?;
                        overzichtItems.Add(new UitvaartOverzichtModel
                        {
                            UitvaartId = (Guid)reader["UitvaartId"],
                            UitvaartNr = reader["UitvaartNummer"].ToString(),
                            AchternaamOverledene = reader["Achternaam"].ToString(),
                            VoornaamOverledene = reader["Voornamen"].ToString(),
                            DatumOverlijden = reader["DatumTijdOverleden"] == DBNull.Value ? (DateTime?)null : ((DateTime)reader["DatumTijdOverleden"]).Date,
                            UitvaartLeider = reader["Uitvaartleider"].ToString(),
                            Voorregeling = voorregeling.HasValue ? voorregeling.Value : false // If null, assign a default value (false in this case)
                        });
                    }
                }
            }
            return overzichtItems;
        }
        public WerknemersModel GetWerknemer(Guid werknemerId)
        {
            WerknemersModel Werknemer = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],[Initialen],[Voornaam],[Roepnaam],[Tussenvoegsel],[Achternaam],[Geboorteplaats],[Geboortedatum],[Email],[isDeleted],[isUitvaartverzorger],[isDrager],[isChauffeur],[isOpbaren],[Mobiel] " +
                                        "FROM [ConfigurationPersoneel] WHERE Id = @werknemerId";
                command.Parameters.AddWithValue("@werknemerId", werknemerId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Werknemer = new WerknemersModel()
                        {
                            Id = (Guid)reader[0],
                            Initialen = reader[1].ToString(),
                            Voornaam = reader[2].ToString(),
                            Roepnaam = reader[3].ToString(),
                            Tussenvoegsel = reader[4].ToString(),
                            Achternaam = reader[5].ToString(),
                            Geboorteplaats = reader[6].ToString(),
                            Geboortedatum = (DateTime)reader[7],
                            Email = reader[8].ToString(),
                            IsDeleted = (bool)reader[9],
                            IsUitvaartverzorger = (bool)reader[10],
                            IsDrager = (bool)reader[11],
                            IsChauffeur = (bool)reader[12],
                            IsOpbaren = reader[13] != DBNull.Value ? (bool)reader[13] : false,
                            Mobiel = reader[14].ToString()
                        };
                    }
                }
            }
            return Werknemer;
        }
        public ObservableCollection<KistenModel> GetKisten()
        {
            ObservableCollection<KistenModel> Kisten = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, kistTypeNummer, kistOmschrijving, isDeleted FROM [ConfigurationKisten] ORDER BY isDeleted ASC, kistTypeNummer";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = (reader[3] == DBNull.Value || !(bool)reader[3]) ? "Green" : "Red";

                        Kisten.Add(new KistenModel
                        {
                            Id = (Guid)reader[0],
                            KistTypeNummer = reader[1].ToString(),
                            KistOmschrijving = reader[2].ToString(),
                            IsDeleted = (bool)reader[3],
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return Kisten;
        }
        public ObservableCollection<ConfigurationAsbestemmingModel> GetAsbestemmingen()
        {
            ObservableCollection<ConfigurationAsbestemmingModel> Asbestemmingen = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT asbestemmingId, asbestemmingOmschrijving, isDeleted FROM [ConfigurationAsbestemming] ORDER BY isDeleted ASC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = (reader[2] == DBNull.Value || !(bool)reader[2]) ? "Green" : "Red";

                        Asbestemmingen.Add(new ConfigurationAsbestemmingModel
                        {
                            AsbestemmingId = (Guid)reader[0],
                            AsbestemmingOmschrijving = reader[1].ToString(),
                            IsDeleted = (bool)reader[2],
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return Asbestemmingen;
        }
        public KistenModel GetKist(Guid kistId)
        {
            KistenModel Kist = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, kistTypeNummer, kistOmschrijving, isDeleted FROM [ConfigurationKisten] WHERE Id = @kistId";
                command.Parameters.AddWithValue("@kistId", kistId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Kist = new KistenModel()
                        {
                            Id = (Guid)reader[0],
                            KistTypeNummer = reader[1].ToString(),
                            KistOmschrijving = reader[2].ToString(),
                            IsDeleted = (bool)reader[3],
                        };
                    }
                }
            }
            return Kist;
        }
        public ConfigurationAsbestemmingModel GetAsbestemming(Guid asbestemmingId)
        {
            ConfigurationAsbestemmingModel Asbestemming = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [asbestemmingId], [asbestemmingOmschrijving], [isDeleted] FROM [ConfigurationAsbestemming] WHERE asbestemmingId =  @asbestemmingId";
                command.Parameters.AddWithValue("@asbestemmingId", asbestemmingId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Asbestemming = new ConfigurationAsbestemmingModel()
                        {
                            AsbestemmingId = (Guid)reader[0],
                            AsbestemmingOmschrijving = reader[1].ToString(),
                            IsDeleted = (bool)reader[2],
                        };
                    }
                }
            }
            return Asbestemming;
        }
        public Guid GetUitvaartGuid(string Uitvaartnummer)
        {
            Guid uitvaartId = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [UitvaartId] FROM [OverledeneUitvaartleider] WHERE Uitvaartnummer = @uitvaartnr";
                command.Parameters.AddWithValue("@uitvaartnr", Uitvaartnummer);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        uitvaartId = (Guid)reader[0];
                    }
                }
            }
            return uitvaartId;
        }
        public ObservableCollection<KistenLengte> GetKistenLengte()
        {
            ObservableCollection<KistenLengte> kistenLengtes = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, kistLengte, isDeleted FROM [ConfigurationKistenLengte]";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kistenLengtes.Add(new KistenLengte
                        {
                            Id = (Guid)reader[0],
                            KistLengte = reader[1].ToString(),
                            IsDeleted = (bool)reader[2],
                        });
                    }
                }
            }
            return kistenLengtes;
        }
        public ObservableCollection<VerzorgendPersoneel> GetVerzorgers()
        {
            ObservableCollection<VerzorgendPersoneel> verzorgers = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, CONCAT(Initialen,' ',Achternaam) as PersoneelName, isDeleted, isOpbaren FROM [ConfigurationPersoneel]";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        verzorgers.Add(new VerzorgendPersoneel
                        {
                            Id = (Guid)reader[0],
                            VerzorgendPersoon = reader[1].ToString(),
                            IsDeleted = (bool)reader[2],
                            IsOpbaren = reader[3] != DBNull.Value ? (bool)reader[3] : false
                        });
                    }
                }
            }
            return verzorgers;
        }
        public ObservableCollection<WerkbonPersoneel> GetWerkbonPersoneel()
        {
            ObservableCollection<WerkbonPersoneel> personeel = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, CONCAT(Initialen,' ',Achternaam) as PersoneelName, isDeleted FROM [ConfigurationPersoneel]";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        personeel.Add(new WerkbonPersoneel
                        {
                            Id = (Guid)reader[0],
                            WerkbonPersoon = reader[1].ToString(),
                            IsDeleted = (bool)reader[2]
                        });
                }
            }
            return personeel;
        }
        public ObservableCollection<LeveranciersModel> GetLeveranciers()
        {
            ObservableCollection<LeveranciersModel> leveranciers = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [leverancierId],[leverancierName],[leverancierBeschrijving],[steenhouwer]," +
                                        "[bloemist],[kisten],[urnsieraden],[isDeleted] " +
                                        "FROM [ConfigurationLeveranciers]";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = ((bool)reader[7]) ? "Red" : "Green";

                        leveranciers.Add(new LeveranciersModel
                        {

                            LeverancierId = (Guid)reader[0],
                            LeverancierName = reader[1].ToString(),
                            LeverancierBeschrijving = reader[2].ToString(),
                            Steenhouwer = (bool)reader[3],
                            Bloemist = (bool)reader[4],
                            Kisten = (bool)reader[5],
                            UrnSieraden = (bool)reader[6],
                            IsDeleted = (bool)reader[7],
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return leveranciers;
        }
        public ObservableCollection<OverledeneRouwbrieven> GetRouwbrieven()
        {
            ObservableCollection<OverledeneRouwbrieven> rouwbrieven = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [rouwbrievenId],[rouwbrievenName],[isDeleted] FROM [ConfigurationRouwbrieven]";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = ((bool)reader[2]) ? "Red" : "Green";

                        rouwbrieven.Add(new OverledeneRouwbrieven
                        {
                            RouwbrievenId = (Guid)reader[0],
                            RouwbrievenName = reader[1].ToString(),
                            IsDeleted = (bool)reader[2],
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return rouwbrieven;
        }
        public ObservableCollection<FactuurModel> GetAllKostenbegrotingen()
        {
            var kostenbegrotingGegevens = new ObservableCollection<FactuurModel>();

            using (var connection = GetConnection())
            using (var command = new SqlCommand("SELECT [Id], OU.UitvaartId AS OUUitvaartId, Uitvaartnummer, [kostenbegrotingUrl], [kostenbegrotingJson], [kostenbegrotingCreationDate], [kostenbegrotingCreated], [factuurCreationDate], [factuurUrl], [factuurCreated] " +
                                                "FROM [OverledeneFacturen] AS OFA " +
                                                "INNER JOIN OverledeneUitvaartleider OU ON OFA.uitvaartId = OU.UitvaartId " +
                                                "WHERE kostenbegrotingUrl IS NOT NULL AND kostenbegrotingUrl != ''", connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string factuurUrlValue = reader["factuurUrl"]?.ToString();
                        string opdrachtUrl = string.Empty;
                        string verenigingUrl = string.Empty;

                        if (!string.IsNullOrWhiteSpace(factuurUrlValue))
                        {
                            try
                            {
                                var jsonFactuur = JsonConvert.DeserializeObject<ExpandoObject>(factuurUrlValue) as IDictionary<string, object>;

                                // Check and retrieve URLs if they exist
                                if (jsonFactuur != null)
                                {
                                    opdrachtUrl = jsonFactuur.TryGetValue("opdrachtgeverFactuurUrl", out var opdrachtgeverFactuurUrl)
                                        ? opdrachtgeverFactuurUrl?.ToString() ?? string.Empty
                                        : string.Empty;

                                    verenigingUrl = jsonFactuur.TryGetValue("verenigingFactuurUrl", out var verenigingFactuurUrl)
                                        ? verenigingFactuurUrl?.ToString() ?? string.Empty
                                        : string.Empty;
                                }
                            }
                            catch (JsonException ex)
                            {
                                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                            }
                        }

                        kostenbegrotingGegevens.Add(new FactuurModel
                        {
                            Id = reader["Id"] != DBNull.Value ? (Guid)reader["Id"] : Guid.Empty,
                            UitvaartId = reader["OUUitvaartId"] != DBNull.Value ? (Guid)reader["OUUitvaartId"] : Guid.Empty,
                            UitvaartNummer = reader["Uitvaartnummer"]?.ToString() ?? string.Empty,
                            KostenbegrotingUrl = reader["kostenbegrotingUrl"]?.ToString() ?? string.Empty,
                            KostenbegrotingJson = reader["kostenbegrotingJson"]?.ToString() ?? string.Empty,
                            KostenbegrotingCreationDate = reader["kostenbegrotingCreationDate"] != DBNull.Value ? (DateTime)reader["kostenbegrotingCreationDate"] : DateTime.MinValue,
                            KostenbegrotingCreated = reader["kostenbegrotingCreated"] != DBNull.Value && (bool)reader["kostenbegrotingCreated"],
                            FactuurCreationDate = reader["factuurCreationDate"] != DBNull.Value ? (DateTime)reader["factuurCreationDate"] : DateTime.MinValue,
                            FactuurVerenigingUrl = verenigingUrl,
                            FactuurOpdrachtgeverUrl = opdrachtUrl,
                            FactuurCreated = reader["factuurCreated"] != DBNull.Value && (bool)reader["factuurCreated"],
                        });
                    }
                }
            }

            return kostenbegrotingGegevens;
        }
        public async Task<KostenbegrotingInfoModel> GetKostenbegrotingPersonaliaAsync(Guid uitvaartIdGuid)
        {
            KostenbegrotingInfoModel personalia = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync();
                command.Connection = connection;
                command.CommandText = "SELECT OUL.Uitvaartnummer AS UitvaartNummer, " +
                                      "overledeneAanhef, overledeneVoornamen," +
                                      "(CASE WHEN (overledeneTussenvoegsel IS NOT NULL) THEN CONCAT(overledeneTussenvoegsel, ' ',overledeneAchternaam) " +
                                      "ELSE overledeneAchternaam END) AS OverledeneAchternaam,  " +
                                      "(CASE WHEN (opdrachtgeverTussenvoegsel IS NOT NULL) THEN CONCAT(opdrachtgeverVoornaamen, ' ', opdrachtgeverTussenvoegsel, ' ', opdrachtgeverAchternaam) " +
                                      "ELSE CONCAT(opdrachtgeverVoornaamen, ' ',opdrachtgeverAchternaam) END) AS OpdrachtgeverNaam, " +
                                      "(CASE WHEN (opdrachtgeverHuisnummerToevoeging IS NOT NULL) THEN CONCAT(opdrachtgeverStraat, ' ', opdrachtgeverHuisnummer, ' ', opdrachtgeverHuisnummerToevoeging) " +
                                      "ELSE CONCAT(opdrachtgeverStraat, ' ', opdrachtgeverHuisnummer) END) AS OpdrachtgeverStraat, " +
                                      "opdrachtgeverPostcode, opdrachtgeverWoonplaats, OOI.overledenDatumTijd " +
                                      "FROM [OverledenePersoonsGegevens] OPG " +
                                      "INNER JOIN [OverledeneOpdrachtgever] OO ON OPG.UitvaartId = OO.uitvaartId " +
                                      "INNER JOIN [OverledeneUitvaartleider] OUL ON OPG.UitvaartId = OUL.UitvaartId " +
                                      "INNER JOIN [OverledeneOverlijdenInfo] OOI ON OPG.uitvaartId = OOI.UitvaartId " +
                                      "WHERE OPG.uitvaartId = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", uitvaartIdGuid);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalia = new KostenbegrotingInfoModel()
                        {
                            UitvaartNummer = reader[0].ToString(),
                            OverledeneAanhef = reader[1].ToString(),
                            OverledeneVoornaam = reader[2].ToString(),
                            OverledeneAchternaam = reader[3].ToString(),
                            OverledeneNaam = string.Empty,
                            OpdrachtgeverNaam = reader[4].ToString(),
                            OpdrachtgeverStraat = reader[5].ToString(),
                            OpdrachtgeverPostcode = reader[6].ToString(),
                            OpdrachtgeverWoonplaats = reader[7].ToString(),
                            OverledenDatum = reader[8] != DBNull.Value ? (DateTime)reader[8] : default(DateTime),
                        };
                    }
                }
            }
            return personalia;
        }
        public OverledeneBloemenModel GetBloemenUitbetaling(Guid UitvaartIdGuid)
        {
            OverledeneBloemenModel bloem = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT ob.[Id],ul.Uitvaartnummer,[bloemenBedrag],[bloemenProvisie],[bloemenUitbetaling],[leverancierName], " +
                    "CONCAT(Voornaam, ' ', Tussenvoegsel, ' ', Achternaam) " +
                    "FROM [OverledeneBloemen] ob " +
                    "INNER JOIN OverledeneUitvaartleider ou ON ob.uitvaartId = ou.UitvaartId " +
                    "INNER JOIN ConfigurationPersoneel cp ON cp.Id = ou.PersoneelId " +
                    "INNER JOIN ConfigurationLeveranciers cl on ob.bloemenLeverancier = cl.leverancierId " +
                    "INNER JOIN OverledeneUitvaartleider ul ON ul.UitvaartId = ob.uitvaartId " +
                    "WHERE ob.uitvaartId = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartIdGuid);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bloem = new OverledeneBloemenModel()
                        {
                            BloemenId = reader[0] != DBNull.Value ? (Guid)reader[0] : Guid.Empty,
                            UitvaartNummer = reader[1].ToString(),
                            BloemenBedrag = reader[2].ToString(),
                            BloemenProvisie = reader[3].ToString(),
                            BloemenUitbetaling = reader[4] != DBNull.Value ? (DateTime)reader[4] : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 25),
                            BloemenLeverancierName = reader[5].ToString(),
                            UitvaartLeider = reader[6].ToString()
                        };

                    }
                }
            }
            return bloem;
        }
        public OverledeneSteenhouwerijModel GetSteenUitbetaling(Guid UitvaartIdGuid)
        {
            OverledeneSteenhouwerijModel steen = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT os.[Id],Uitvaartnummer,[steenhouwerBedrag],[steenhouwerProvisie],[steenhouwerUitbetaing]" +
                                        ",leverancierName, CONCAT(Voornaam, ' ', Tussenvoegsel, ' ', Achternaam), [steenhouwerProvisieTotaal] " +
                                        "FROM [OverledeneSteenhouwer] os " +
                                        "INNER JOIN OverledeneUitvaartleider ou ON os.uitvaartId = ou.UitvaartId " +
                                        "INNER JOIN ConfigurationPersoneel cp ON cp.Id = ou.PersoneelId " +
                                        "INNER JOIN ConfigurationLeveranciers cl on os.steenhouwerLeverancier = cl.leverancierId " +
                                        "WHERE os.uitvaartId = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartIdGuid);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        steen = new OverledeneSteenhouwerijModel()
                        {
                            SteenhouwerijId = reader[0] != DBNull.Value ? (Guid)reader[0] : Guid.Empty,
                            UitvaartNummer = reader[1].ToString(),
                            SteenhouwerBedrag = reader[2].ToString(),
                            SteenhouwerProvisie = reader[3].ToString(),
                            SteenhouwerUitbetaing = reader[4] != DBNull.Value ? (DateTime)reader[4] : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 25),
                            SteenhouwerLeverancierName = reader[5].ToString(),
                            UitvaartLeider = reader[6].ToString(),
                            SteenhouwerProvisieTotaal = reader[7].ToString()
                        };
                    }
                }
            }
            return steen;
        }
        public OverledeneUrnSieradenModel GetUrnSieradenUitbetaling(Guid UitvaartIdGuid)
        {
            OverledeneUrnSieradenModel urnSieraden = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT ous.[Id],Uitvaartnummer,[urnBedrag],[urnProvisie],[urnUitbetaling]" +
                                      ",leverancierName, CONCAT(Voornaam, ' ', Tussenvoegsel, ' ', Achternaam) " +
                                      "FROM [overledeneUrnSieraden] ous " +
                                      "INNER JOIN OverledeneUitvaartleider ou ON ous.uitvaartId = ou.UitvaartId " +
                                      "INNER JOIN ConfigurationPersoneel cp ON cp.Id = ou.PersoneelId " +
                                      "INNER JOIN ConfigurationLeveranciers cl on ous.urnLeverancier = cl.leverancierId " +
                                      "WHERE ous.uitvaartId = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", UitvaartIdGuid);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        urnSieraden = new OverledeneUrnSieradenModel()
                        {
                            UrnId = reader[0] != DBNull.Value ? (Guid)reader[0] : Guid.Empty,
                            UitvaartNummer = reader.IsDBNull(1) ? string.Empty : reader[1].ToString(),
                            UrnBedrag = reader.IsDBNull(2) ? string.Empty : reader[2].ToString(),
                            UrnProvisie = reader.IsDBNull(3) ? string.Empty : reader[3].ToString(),
                            UrnUitbetaing = reader[4] != DBNull.Value ? (DateTime)reader[4] : DateTime.Today,
                            UrnLeverancierName = reader.IsDBNull(5) ? string.Empty : reader[5].ToString(),
                            UitvaartLeider = reader.IsDBNull(6) ? string.Empty : reader[6].ToString()
                        };
                    }
                }

            }
            return urnSieraden;
        }
        public IEnumerable<OverledeneBijlagesModel> GetAktesVanCessieByUitvaatId(string uitvaartId)
        {
            var AkteList = new List<OverledeneBijlagesModel>();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [BijlageId], OB.[UitvaartId], [DocumentName], [DocumentType], [DocumentURL], [DocumentHash], [DocumentInconsistent] " +
                                      "FROM [OverledeneBijlages] OB " +
                                      "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OB.UitvaartId " +
                                      "WHERE Uitvaartnummer = @uitvaartNummer AND [DocumentName] LIKE 'AkteVanCessie%'";

                command.Parameters.AddWithValue("@uitvaartNummer", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var AkteGegevens = new OverledeneBijlagesModel()
                        {
                            BijlageId = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            DocumentName = reader[2].ToString(),
                            DocumentType = reader[3].ToString(),
                            DocumentUrl = reader[4].ToString(),
                            DocumentHash = reader[5].ToString(),
                            DocumentInconsistent = (bool)reader[6],
                        };

                        AkteList.Add(AkteGegevens);
                    }
                }
            }
            return AkteList;
        }
        public ObservableCollection<GeneratedKostenbegrotingModel> GetPriceComponentsId(Guid verzekeraarId, bool pakketVerzekering)
        {
            ObservableCollection<GeneratedKostenbegrotingModel> PriceComponents = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT FC1.[ComponentId], FC1.[Omschrijving], FC1.[Bedrag], FC1.[VerzekerdAantal], FC1.[Verzekering], " +
                    "FC1.[IsDeleted], FC1.[SpecificCrematie], FC1.[SpecificBegrafenis], FC1.[SpecificPakket], FC1.VerzekeringJson " +
                    "FROM [ConfigurationFactuurComponent] FC1 " +
                    "LEFT JOIN [ConfigurationFactuurComponent] FC2 ON FC1.Id <> FC2.Id AND FC1.Omschrijving = FC2.Omschrijving " +
                    "WHERE (EXISTS (SELECT 1 " +
                    "FROM OPENJSON(FC1.VerzekeringJson) WITH (Id UNIQUEIDENTIFIER '$.Id') AS JsonIds " +
                    "WHERE JsonIds.Id = @SelectedVerzekeraarId) OR FC1.VerzekeringJson = '') " +
                    "AND FC2.Id IS NULL " +
                    "ORDER BY FC1.SortOrder, FC1.IsDeleted ASC";
                command.Parameters.AddWithValue("@SelectedVerzekeraarId", SqlDbType.UniqueIdentifier).Value = verzekeraarId;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool specificPakket = !reader.IsDBNull(8) && (bool)reader[8];

                        if (!(bool)reader[5])
                        {
                            PriceComponents.Add(new GeneratedKostenbegrotingModel
                            {
                                Id = (Guid)reader[0],
                                Omschrijving = reader[1].ToString(),
                                Bedrag = (decimal)reader[2],
                                OrgBedrag = (decimal)reader[2],
                                Aantal = reader[3].ToString(),
                                OrgAantal = reader[3].ToString(),
                                Verzekerd = reader[4].ToString(),
                                IsDeleted = (bool)reader[5],
                                SpecificCrematie = reader.IsDBNull(6) ? false : (bool)reader[6],
                                SpecificBegrafenis = reader.IsDBNull(7) ? false : (bool)reader[7],
                                SpecificPakket = specificPakket
                            });
                        }
                    }

                }
            }
            return PriceComponents;
        }
        public ObservableCollection<GeneratedKostenbegrotingModel> GetPriceComponents(string verzekeringMaatschapij, bool pakketVerzekering)
        {
            ObservableCollection<GeneratedKostenbegrotingModel> PriceComponents = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT FC1.[ComponentId], FC1.[Omschrijving], FC1.[Bedrag], FC1.[VerzekerdAantal], FC1.[Verzekering], FC1.[IsDeleted], FC1.[SpecificCrematie], FC1.[SpecificBegrafenis], FC1.[SpecificPakket] " +
                                        "FROM [ConfigurationFactuurComponent] FC1 " +
                                        "LEFT JOIN [ConfigurationFactuurComponent] FC2 " +
                                        "ON FC1.Id <> FC2.Id AND FC1.Omschrijving = FC2.Omschrijving " +
                                        "WHERE (FC1.Verzekering LIKE '%' + @Verzekering + '%' OR FC1.Verzekering = '') AND FC2.Id IS NULL " +
                                        "ORDER BY FC1.SortOrder, FC1.isDeleted ASC";
                command.Parameters.AddWithValue("@Verzekering", SqlDbType.VarChar).Value = verzekeringMaatschapij;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool specificPakket = !reader.IsDBNull(8) && (bool)reader[8];

                        if (!(bool)reader[5])
                        {
                            PriceComponents.Add(new GeneratedKostenbegrotingModel
                            {
                                Id = (Guid)reader[0],
                                Omschrijving = reader[1].ToString(),
                                Bedrag = (decimal)reader[2],
                                OrgBedrag = (decimal)reader[2],
                                Aantal = reader[3].ToString(),
                                OrgAantal = reader[3].ToString(),
                                Verzekerd = reader[4].ToString(),
                                IsDeleted = (bool)reader[5],
                                SpecificCrematie = reader.IsDBNull(6) ? false : (bool)reader[6],
                                SpecificBegrafenis = reader.IsDBNull(7) ? false : (bool)reader[7],
                                SpecificPakket = specificPakket
                            });
                        }
                    }

                }
            }
            return PriceComponents;
        }
        public ObservableCollection<KostenbegrotingModel> GetAllPriceComponentsBeheer()
        {
            ObservableCollection<KostenbegrotingModel> PriceComponents = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [ComponentId],[Omschrijving],[Bedrag],[VerzekerdAantal],[Verzekering],[IsDeleted],[SortOrder],[DefaultPM],[VerzekeringJson] " +
                                        "FROM [ConfigurationFactuurComponent] ORDER BY SortOrder ASC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string styleColor;
                        styleColor = ((bool)reader[5]) ? "Red" : "Green";

                        PriceComponents.Add(new KostenbegrotingModel
                        {
                            ComponentId = (Guid)reader[0],
                            ComponentOmschrijving = reader[1].ToString(),
                            ComponentBedrag = (decimal)reader[2],
                            ComponentAantal = reader[3].ToString(),
                            ComponentVerzekering = reader[4].ToString(),
                            IsDeleted = (bool)reader[5],
                            SortOrder = (int)reader[6],
                            DefaultPM = (bool)reader[7],
                            ComponentVerzekeringJson = reader[8].ToString(),
                            BtnBrush = styleColor
                        });
                    }
                }
            }
            return PriceComponents;
        }
        public KostenbegrotingModel GetSelectedPriceComponentsBeheer(Guid componentId)
        {
            KostenbegrotingModel PriceComponents = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [ComponentId], [Omschrijving], [Bedrag], [VerzekerdAantal], [Verzekering], [IsDeleted], SortOrder, [factuurBedrag], [DefaultPM], [VerzekeringJson] " +
                                        "FROM [ConfigurationFactuurComponent]" +
                                        "WHERE ComponentId = @componentId";
                command.Parameters.AddWithValue("@componentId", componentId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read() && !(bool)reader[5])
                    {
                        PriceComponents = new KostenbegrotingModel()
                        {
                            ComponentId = (Guid)reader[0],
                            ComponentOmschrijving = reader[1].ToString(),
                            ComponentBedrag = reader.IsDBNull(2) ? decimal.Zero : (decimal)reader[2],
                            ComponentAantal = reader[3].ToString(),
                            ComponentVerzekering = reader[4].ToString(),
                            ComponentVerzekeringJson = reader[9].ToString(),
                            SortOrder = reader.IsDBNull(6) ? 0 : (int)reader[6],
                            ComponentFactuurBedrag = reader.IsDBNull(7) ? decimal.Zero : (decimal)reader[7],
                            DefaultPM = (bool)reader[8]
                        };
                    }
                }
            }
            return PriceComponents;
        }
        public ObservableCollection<RapportagesKisten> GetRapportagesKisten(string startNummer, string endNummer)
        {
            ObservableCollection<RapportagesKisten> kistenRapport = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT kistTypeNummer, kistOmschrijving, COUNT(kistTypeNummer) as KistCount " +
                                        "FROM [OverledeneOpbaring] " +
                                        "INNER JOIN[ConfigurationKisten] ON [opbaringKistId] = [ConfigurationKisten].[Id] " +
                                        "INNER JOIN[OverledeneUitvaartleider] ON [OverledeneOpbaring].uitvaartId = [OverledeneUitvaartleider].UitvaartId " +
                                        "WHERE Uitvaartnummer BETWEEN @StartNummer AND @EndNummer " +
                                        "GROUP BY kistTypeNummer, kistOmschrijving " +
                                        "ORDER BY kistCount DESC";
                command.Parameters.AddWithValue("@StartNummer", SqlDbType.Int).Value = int.Parse(startNummer);
                command.Parameters.AddWithValue("@EndNummer", SqlDbType.Int).Value = int.Parse(endNummer);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int kistCount = reader["KistCount"] == DBNull.Value ? 0 : (int)reader["KistCount"];

                        kistenRapport.Add(new RapportagesKisten
                        {
                            KistCount = kistCount,
                            KistOmschrijving = reader["kistOmschrijving"].ToString(),
                            KistTypeNummer = reader["kistTypeNummer"].ToString(),
                        });
                    }
                }
            }
            return kistenRapport;
        }
        public ObservableCollection<RapportagesVerzekering> GetRapportagesVerzekering(string startNummer, string endNummer)
        {
            ObservableCollection<RapportagesVerzekering> verzekeringRapport = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT CV.verzekeraarNaam as HerkomstName, COUNT(overledenHerkomst) AS HerkomstCount " +
                                        "FROM OverledeneOverlijdenInfo " +
                                        "INNER JOIN ConfigurationVerzekeraar AS CV ON overledenHerkomst = CV.Id " +
                                        "INNER JOIN OverledeneUitvaartleider AS OU ON OU.UitvaartId = OverledeneOverlijdenInfo.UitvaartId " +
                                        "WHERE Uitvaartnummer BETWEEN @StartNummer AND @EndNummer " +
                                        "GROUP BY CV.verzekeraarNaam " +
                                        "ORDER BY HerkomstCount DESC";
                command.Parameters.AddWithValue("@StartNummer", SqlDbType.Int).Value = int.Parse(startNummer);
                command.Parameters.AddWithValue("@EndNummer", SqlDbType.Int).Value = int.Parse(endNummer);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int verzekeringCount = reader["HerkomstCount"] == DBNull.Value ? 0 : (int)reader["HerkomstCount"];

                        verzekeringRapport.Add(new RapportagesVerzekering
                        {
                            VerzekeringHerkomst = reader["HerkomstName"].ToString(),
                            VerzekeringHerkomstCount = verzekeringCount,
                        });
                    }
                }
            }
            return verzekeringRapport;
        }
        public ObservableCollection<RapportagesVerzekering> GetRapportagesVerzekeringWoonplaats(string startNummer, string endNummer)
        {
            ObservableCollection<RapportagesVerzekering> verzekeringRapportWoonplaats = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT OPG.overledeneWoonplaats as Woonplaats, CV.verzekeraarNaam as HerkomstName, COUNT(overledenHerkomst) AS HerkomstCount " +
                                        "FROM OverledeneOverlijdenInfo OOI " +
                                        "LEFT JOIN OverledenePersoonsGegevens AS OPG ON OOI.UitvaartId = OPG.uitvaartId " +
                                        "INNER JOIN ConfigurationVerzekeraar AS CV ON overledenHerkomst = CV.Id " +
                                        "INNER JOIN OverledeneUitvaartleider AS OU ON OU.UitvaartId = OOI.UitvaartId " +
                                        "WHERE Uitvaartnummer BETWEEN @StartNummer AND @EndNummer " +
                                        "GROUP BY CV.verzekeraarNaam, overledeneWoonplaats";
                command.Parameters.AddWithValue("@StartNummer", SqlDbType.Int).Value = int.Parse(startNummer);
                command.Parameters.AddWithValue("@EndNummer", SqlDbType.Int).Value = int.Parse(endNummer);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int verzekeringCount = reader["HerkomstCount"] == DBNull.Value ? 0 : (int)reader["HerkomstCount"];

                        verzekeringRapportWoonplaats.Add(new RapportagesVerzekering
                        {
                            VerzekeringWoonplaats = reader["Woonplaats"].ToString(),
                            VerzekeringHerkomst = reader["HerkomstName"].ToString(),
                            VerzekeringHerkomstCount = verzekeringCount,
                        });
                    }
                }
            }
            return verzekeringRapportWoonplaats;
        }
        public ObservableCollection<RapportagesUitvaartleider> GetRapportagesUitvaartleider(string startNummer, string endNummer)
        {
            ObservableCollection<RapportagesUitvaartleider> uitvaartleiderRapport = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT CONCAT(CP.Initialen, ' ', CP.Achternaam) AS UitvaartLeider, COUNT(OU.Uitvaartnummer) AS Uitvaartnummer " +
                                        "FROM OverledeneUitvaartleider OU " +
                                        "INNER JOIN ConfigurationPersoneel CP ON OU.PersoneelId = CP.Id " +
                                        "WHERE Uitvaartnummer BETWEEN @StartNummer AND @EndNummer " +
                                        "GROUP BY CONCAT(CP.Initialen, ' ', CP.Achternaam) " +
                                        "ORDER BY Uitvaartnummer DESC";
                command.Parameters.AddWithValue("@StartNummer", SqlDbType.Int).Value = int.Parse(startNummer);
                command.Parameters.AddWithValue("@EndNummer", SqlDbType.Int).Value = int.Parse(endNummer);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int uitvaartleiderCount = reader["Uitvaartnummer"] == DBNull.Value ? 0 : (int)reader["Uitvaartnummer"];

                        uitvaartleiderRapport.Add(new RapportagesUitvaartleider
                        {
                            Uitvaartleider = reader["UitvaartLeider"].ToString(),
                            Uitvaartnummer = uitvaartleiderCount,
                        });
                    }
                }
            }
            return uitvaartleiderRapport;
        }

        public ObservableCollection<Volgautos> GetVolgautos(string startNummer, string endNummer)
        {
            ObservableCollection<Volgautos> autos = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT CV.verzekeraarNaam as HerkomstName, SUM(CONVERT(INT, COALESCE(uitvaartInfoDienstVolgauto, 0))) AS AantalAutos " +
                                        "FROM OverledeneOverlijdenInfo OOI " +
                                        "LEFT JOIN OverledeneUitvaartInfo AS OPG ON OOI.UitvaartId = OPG.uitvaartId " +
                                        "INNER JOIN ConfigurationVerzekeraar AS CV ON overledenHerkomst = CV.Id " +
                                        "INNER JOIN OverledeneUitvaartleider AS OU ON OU.UitvaartId = OOI.UitvaartId " +
                                        "WHERE Uitvaartnummer BETWEEN @StartNummer AND @EndNummer " +
                                        "GROUP BY CV.verzekeraarNaam;";
                command.Parameters.AddWithValue("@StartNummer", SqlDbType.Int).Value = int.Parse(startNummer);
                command.Parameters.AddWithValue("@EndNummer", SqlDbType.Int).Value = int.Parse(endNummer);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        autos.Add(new Volgautos
                        {
                            VerzekeringNaam = reader["HerkomstName"].ToString(),
                            AantalVolgautos = (int)reader["AantalAutos"],
                        });
                    }
                }
            }
            return autos;
        }
        public ObservableCollection<PeriodeLijst> GetPeriode(string startNummer, string endNummer)
        {
            ObservableCollection<PeriodeLijst> periode = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Uitvaartnummer, CONCAT([overledeneAchternaam],' ',[overledeneTussenvoegsel]) AS Naam, overledeneVoornamen as Voornaam, overledenDatumTijd, CV.verzekeraarNaam as Verzekeraar, uitvaartInfoType, factuurUrl " +
                                        "FROM[OverledeneUitvaartleider] OU " +
                                        "INNER JOIN[OverledenePersoonsGegevens] OPG ON OU.UitvaartId = OPG.uitvaartId " +
                                        "INNER JOIN[OverledeneOverlijdenInfo] OOI ON OPG.uitvaartId = OOI.UitvaartId " +
                                        "INNER JOIN[ConfigurationVerzekeraar] CV ON overledenHerkomst = CV.Id " +
                                        "INNER JOIN[OverledeneUitvaartInfo] OUI ON OPG.uitvaartId = OUI.uitvaartId " +
                                        "LEFT JOIN[OverledeneFacturen] OFA ON OPG.uitvaartId = OFA.uitvaartId " +
                                        "WHERE Uitvaartnummer BETWEEN @StartNummer AND @EndNummer " +
                                        "ORDER BY Uitvaartnummer ASC";
                command.Parameters.AddWithValue("@StartNummer", SqlDbType.Int).Value = int.Parse(startNummer);
                command.Parameters.AddWithValue("@EndNummer", SqlDbType.Int).Value = int.Parse(endNummer);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        periode.Add(new PeriodeLijst
                        {
                            UitvaartNummer = reader["Uitvaartnummer"].ToString(),
                            UitvaartNaam = reader["Naam"].ToString(),
                            Voornamen = reader["Voornaam"].ToString(),
                            DatumOverlijden = reader["overledenDatumTijd"].ToString(),
                            Verzekering = reader["Verzekeraar"].ToString(),
                            UitvaartType = reader["uitvaartInfoType"].ToString(),
                            Factuur = reader["factuurUrl"].ToString(),
                        });
                    }
                }
            }
            return periode;
        }
        public Klanttevredenheid GetScore(Guid uitvaartId)
        {
            Klanttevredenheid score = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],[UitvaartId],[Cijfer],NotificatieOverleden FROM [OverledeneKlantTevredenheid] WHERE [UitvaartId] = @uitvaartId";
                command.Parameters.AddWithValue("@uitvaartId", uitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        score = new Klanttevredenheid()
                        {
                            Id = (Guid)reader[0],
                            UitvaartId = (Guid)reader[1],
                            CijferScore = (int)reader[2],
                            IsNotificationEnabled = (bool)reader[3]
                        };
                    }
                }
            }
            return score;
        }
        public Filter GetFilter()
        {
            Filter filter = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT MIN(CAST('0' AS INT)) AS LowestUitvaartnummer, MAX(CAST(Uitvaartnummer+1 AS INT)) AS HighestUitvaartnummer FROM [OverledeneUitvaartleider] WHERE ISNUMERIC(Uitvaartnummer) = 1";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        filter = new Filter()
                        {
                            LowestNumber = (int)reader[0],
                            HighestNumber = (int)reader[1],
                        };
                    }
                }
            }
            return filter;
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
        public string GetLeverancier(Guid leverancierId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT leverancierName FROM [ConfigurationLeveranciers] WHERE [leverancierId] = @LeverancierId";
                command.Parameters.AddWithValue("@LeverancierId", leverancierId);
                var leverancierName = command.ExecuteScalar();

                if (leverancierName == null)
                {
                    return "SearchingLeverancierFailed";
                }
                else
                {
                    return leverancierName.ToString();
                }
            }
        }
        public LeveranciersModel GetLeverancierBeheer(Guid leverancierId)
        {
            LeveranciersModel leverancier = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [leverancierId],[leverancierName],[leverancierBeschrijving],[steenhouwer],[bloemist],[kisten],[urnsieraden],[isDeleted] " +
                                        "FROM [ConfigurationLeveranciers] WHERE [leverancierId] = @LeverancierId";
                command.Parameters.AddWithValue("@LeverancierId", leverancierId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        leverancier = new LeveranciersModel()
                        {
                            LeverancierId = (Guid)reader["leverancierId"],
                            LeverancierName = reader["leverancierName"].ToString(),
                            LeverancierBeschrijving = reader["leverancierBeschrijving"].ToString(),
                            Steenhouwer = (bool)reader["steenhouwer"],
                            Bloemist = (bool)reader["bloemist"],
                            Kisten = (bool)reader["kisten"],
                            UrnSieraden = (bool)reader["urnsieraden"],
                            IsDeleted = (bool)reader["isDeleted"]
                        };
                    }
                }
            }
            return leverancier;
        }
        public OverledeneRouwbrieven GetRouwbriefBeheer(Guid rouwbrievenId)
        {
            OverledeneRouwbrieven rouwbrief = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [rouwbrievenId],[rouwbrievenName],[isDeleted] FROM [ConfigurationRouwbrieven] WHERE [rouwbrievenId] = @rouwbrievenId";
                command.Parameters.AddWithValue("@rouwbrievenId", rouwbrievenId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rouwbrief = new OverledeneRouwbrieven()
                        {
                            RouwbrievenId = (Guid)reader["rouwbrievenId"],
                            RouwbrievenName = reader["rouwbrievenName"].ToString(),
                            IsDeleted = (bool)reader["isDeleted"]
                        };
                    }
                }
            }
            return rouwbrief;
        }
        public OverledeneBijlagesModel GetFinishedDossier(Guid UitvaartId)
        {
            OverledeneBijlagesModel dossier = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [BijlageId], [UitvaartId], [DocumentName], [DocumentType], [DocumentURL], [DocumentHash], [DocumentInconsistent], [isDeleted] " +
                                        "FROM [OverledeneBijlages] " +
                                        "WHERE UitvaartId = @UitvaartId " +
                                        "AND DocumentName = 'Dossier'";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dossier = new OverledeneBijlagesModel()
                        {
                            BijlageId = (Guid)reader["BijlageId"],
                            UitvaartId = (Guid)reader["UitvaartId"],
                            DocumentType = reader["DocumentType"].ToString(),
                            DocumentUrl = reader["DocumentUrl"].ToString(),
                            DocumentHash = reader["DocumentHash"].ToString(),
                            DocumentInconsistent = (bool)reader["DocumentInconsistent"],
                            IsDeleted = (bool)reader["isDeleted"]
                        };
                    }
                }
            }
            return dossier;
        }
        public OverledeneBijlagesModel GetVerlofDossier(Guid UitvaartId)
        {
            OverledeneBijlagesModel dossier = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [BijlageId], [UitvaartId], [DocumentName], [DocumentType], [DocumentURL], [DocumentHash], [DocumentInconsistent], [isDeleted] " +
                                        "FROM [OverledeneBijlages] " +
                                        "WHERE UitvaartId = @UitvaartId " +
                                        "AND DocumentName = 'Verlof'";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dossier = new OverledeneBijlagesModel()
                        {
                            BijlageId = (Guid)reader["BijlageId"],
                            UitvaartId = (Guid)reader["UitvaartId"],
                            DocumentType = reader["DocumentType"].ToString(),
                            DocumentUrl = reader["DocumentUrl"].ToString(),
                            DocumentHash = reader["DocumentHash"].ToString(),
                            DocumentInconsistent = (bool)reader["DocumentInconsistent"],
                            IsDeleted = (bool)reader["isDeleted"]
                        };
                    }
                }
            }
            return dossier;
        }
        public ObservableCollection<WindowsAccount> GetWerknemerPermissions(Guid werknemerId)
        {
            ObservableCollection<WindowsAccount> userAccount = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT CP.Id, CP.PermissionName FROM [ConfigurationPersoneelPermission] CPP " +
                                        "INNER JOIN [ConfigurationPermissions] CP ON CPP.PermissionId = CP.Id " +
                                        "WHERE CPP.PersoneelId = @werknemerId";
                command.Parameters.AddWithValue("@werknemerId", werknemerId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userAccount.Add(new WindowsAccount
                        {
                            PermissionId = (Guid)reader[0],
                            PermissionName = reader[1].ToString()
                        });
                    }
                }
            }
            return userAccount;
        }
        public ObservableCollection<PermissionsModel> GetPermissions()
        {
            ObservableCollection<PermissionsModel> permissions = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [Id],[PermissionName],[IsEnabled] FROM[ConfigurationPermissions]";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permissions.Add(new PermissionsModel
                        {
                            Id = (Guid)reader[0],
                            PermissionName = reader[1].ToString(),
                            IsEnabled = (bool)reader[2]
                        });
                    }
                }
            }
            return permissions;
        }
        public ObservableCollection<RapportageKlantWerknemerScores> GetAllEmployeeScore()
        {
            ObservableCollection<RapportageKlantWerknemerScores> score = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT CP.Id AS EId, CONCAT(Voornaam, ' ', Achternaam) AS EmployeeName, CAST(AVG(CAST(Cijfer AS DECIMAL(10, 1))) AS DECIMAL(10, 1)) AS AverageCijfer, COUNT(*) AS TotalUitvaarten " +
                                        "FROM[OverledeneUitvaartleider] OU " +
                                        "INNER JOIN[ConfigurationPersoneel] CP ON CP.Id = OU.PersoneelId " +
                                        "INNER JOIN[OverledeneKlantTevredenheid] OKT ON OKT.UitvaartId = OU.UitvaartId " +
                                        "WHERE dossierCompleted = 1 " +
                                        "GROUP BY CP.Id, CONCAT(Voornaam, ' ', Achternaam)";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        score.Add(new RapportageKlantWerknemerScores
                        {
                            EmployeeId = (Guid)reader["EId"],
                            EmployeeName = reader["EmployeeName"].ToString(),
                            GemiddeldCijfer = (decimal)reader["AverageCijfer"],
                            TotalUitvaarten = (int)reader["TotalUitvaarten"],
                        });
                    }
                }
            }
            return score;
        }
        public ObservableCollection<RapportageKlantWerknemerScores> GetEmployeeScore(Guid employeeId)
        {
            ObservableCollection<RapportageKlantWerknemerScores> score = new();
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT CAST(Uitvaartnummer AS INT) AS Uitvaartnr, CONCAT(OPG.overledeneAanhef, ' ', OPG.overledeneVoornamen, ' ', OPG.overledeneTussenvoegsel, ' ', OPG.overledeneAchternaam) AS UitvaartVan, CP.Id as EId, CONCAT(Voornaam, ' ', Achternaam) AS EmployeeName, Cijfer FROM [OverledeneUitvaartleider] OU " +
                                        "INNER JOIN[ConfigurationPersoneel] CP ON CP.Id = OU.PersoneelId " +
                                        "INNER JOIN[OverledeneKlantTevredenheid] OKT ON OKT.UitvaartId = OU.UitvaartId " +
                                        "INNER JOIN[OverledenePersoonsGegevens] OPG ON OPG.UitvaartId = OU.UitvaartId " +
                                        "WHERE CP.id = @employeeid " +
                                        "AND dossierCompleted = 1 " +
                                        "ORDER BY Uitvaartnr ASC";
                command.Parameters.AddWithValue("@employeeid", employeeId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        score.Add(new RapportageKlantWerknemerScores
                        {
                            UitvaartNr = reader["Uitvaartnr"].ToString(),
                            EmployeeId = (Guid)reader["EId"],
                            EmployeeName = reader["EmployeeName"].ToString(),
                            UitvaartVan = reader["UitvaartVan"].ToString(),
                            Cijfer = (int)reader["Cijfer"]
                        });
                    }
                }
            }
            return score;
        }
        public async Task<OverledeneBijlagesModel> GetDocumentInformationAsync(Guid UitvaartId, string DocumentName)
        {
            var dossierInfo = new OverledeneBijlagesModel();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync().ConfigureAwait(false);

                command.Connection = connection;
                command.CommandText = "SELECT DocumentName, DocumentHash, BijlageId, DocumentUrl, DocumentHash, DocumentInconsistent, IsDeleted " +
                                      "FROM OverledeneBijlages " +
                                      "WHERE UitvaartId = @UitvaartId AND DocumentName = @DocumentName AND IsDeleted = 0";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);
                command.Parameters.AddWithValue("@DocumentName", DocumentName);

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        dossierInfo = new OverledeneBijlagesModel
                        {
                            BijlageId = (Guid)reader["BijlageId"],
                            UitvaartId = UitvaartId,
                            DocumentUrl = reader["DocumentUrl"].ToString(),
                            DocumentHash = reader["DocumentHash"].ToString(),
                            DocumentInconsistent = (bool)reader["DocumentInconsistent"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        };
                    }
                }
            }
            return dossierInfo;
        }
        public async Task<List<OverledeneBijlagesModel>> GetDocumentsInformationAsync(Guid UitvaartId, string DocumentName)
        {
            var dossierInfoList = new List<OverledeneBijlagesModel>();

            // Ensure GetConnection is an async method or provide a connection string directly
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                command.Connection = connection;
                command.CommandText = "SELECT DocumentName, DocumentHash, BijlageId, DocumentUrl, DocumentHash, DocumentInconsistent, IsDeleted " +
                                      "FROM OverledeneBijlages " +
                                      "WHERE UitvaartId = @UitvaartId AND DocumentName = @DocumentName";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);
                command.Parameters.AddWithValue("@DocumentName", DocumentName);

                // Use ExecuteReaderAsync for async reading
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var dossierInfo = new OverledeneBijlagesModel()
                        {
                            BijlageId = (Guid)reader["BijlageId"],
                            UitvaartId = UitvaartId,
                            DocumentUrl = reader["DocumentUrl"].ToString(),
                            DocumentHash = reader["DocumentHash"].ToString(),
                            DocumentInconsistent = (bool)reader["DocumentInconsistent"],
                            IsDeleted = (bool)reader["IsDeleted"]
                        };
                        dossierInfoList.Add(dossierInfo);
                    }
                }
            }

            return dossierInfoList;
        }
        public async Task<DienstDocument> GetDienstInfoAsync(Guid UitvaartId)
        {
            DienstDocument dienstInfo = new DienstDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandText = "SELECT uitvaartInfoDienstLocatie, uitvaartInfoDatumTijdUitvaart, " +
                                            "(CASE WHEN OPG.overledeneTussenvoegsel IS NULL THEN CONCAT(OPG.overledeneAanhef, ' ', OPG.overledeneVoornamen, ' ', OPG.overledeneAchternaam) ELSE " +
                                            "CONCAT(OPG.overledeneAanhef, ' ', OPG.overledeneVoornamen, ' ', OPG.overledeneTussenvoegsel, ' ', OPG.overledeneAchternaam) END) as NaamUitvaart, uitvaartInfoUitvaartLocatie, uitvaartInfoDienstDatumTijd, " +
                                            "uitvaartInfoDienstMuziek, uitvaartInfoDienstAfscheid, uitvaartInfoDienstKist, " +
                                            "(CASE WHEN OOG.opdrachtgeverTussenvoegsel IS NULL THEN CONCAT(OOG.opdrachtgeverAanhef, ' ', OOG.opdrachtgeverVoornaamen, ' ', OOG.opdrachtgeverAchternaam) ELSE CONCAT(OOG.opdrachtgeverAanhef, ' ', OOG.opdrachtgeverVoornaamen, ' ', OOG.opdrachtgeverTussenvoegsel, ' ', OOG.opdrachtgeverAchternaam) END) as Opdrachtgever, " +
                                            "(CASE WHEN OOG.opdrachtgeverHuisnummerToevoeging IS NULL THEN CONCAT(OOG.opdrachtgeverStraat, ' ', OOG.opdrachtgeverHuisnummer) ELSE CONCAT(OOG.opdrachtgeverStraat, ' ', OOG.opdrachtgeverHuisnummer, ' ', OOG.opdrachtgeverHuisnummerToevoeging) END) as OpdrachtgeverAdres, " +
                                            "opdrachtgeverTelefoon " +
                                            "FROM [OverledeneUitvaartInfo] OUI " +
                                            "INNER JOIN [OverledenePersoonsGegevens] OPG ON OUI.uitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN [OverledeneOpdrachtgever] OOG ON OUI.uitvaartId = OOG.uitvaartId " +
                                            "WHERE OPG.uitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            dienstInfo = new DienstDocument()
                            {
                                LocatieDienst = reader["uitvaartInfoDienstLocatie"].ToString(),
                                DatumUitvaart = reader["uitvaartInfoDatumTijdUitvaart"] as DateTime? ?? default,
                                NaamUitvaart = reader["NaamUitvaart"].ToString(),
                                AanvraagDienstTe = reader["uitvaartInfoUitvaartLocatie"].ToString(),
                                Aanvang = reader["uitvaartInfoDienstDatumTijd"] as DateTime? ?? default,
                                MuziekAfgespeeld = reader["uitvaartInfoDienstMuziek"].ToString(),
                                AfscheidVoorDienst = reader["uitvaartInfoDienstAfscheid"].ToString(),
                                KistDalen = reader["uitvaartInfoDienstKist"].ToString(),
                                OpdrachtgeverNaam = reader["Opdrachtgever"].ToString(),
                                OpdrachtgeverTelefoon = reader["OpdrachtgeverTelefoon"].ToString(),
                                OpdrachtgeverAdres = reader["OpdrachtgeverAdres"].ToString(),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving service information.", ex);
            }

            return dienstInfo;
        }
        public async Task<KoffieKamerDocument> GetKoffieKamerInfoAsync(Guid UitvaartId)
        {
            KoffieKamerDocument koffieInfo = new KoffieKamerDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync(); // Open the connection asynchronously
                    command.Connection = connection;
                    command.CommandText = "SELECT OUI.uitvaartInfoDatumTijdUitvaart, CONCAT(OPG.overledeneAanhef, ' ', OPG.overledeneAchternaam) AS Naam, uitvaartInfoDienstLocatie, " +
                                            "uitvaartInfoDienstDatumTijd, uitvaartInfoDienstDatumTijd, " +
                                            "(CASE WHEN OOG.opdrachtgeverTussenvoegsel IS NULL THEN CONCAT(OOG.opdrachtgeverAanhef, ' ', OOG.opdrachtgeverVoornaamen, ' ', OOG.opdrachtgeverAchternaam) ELSE CONCAT(OOG.opdrachtgeverAanhef, ' ', OOG.opdrachtgeverVoornaamen, ' ', OOG.opdrachtgeverTussenvoegsel, ' ', OOG.opdrachtgeverAchternaam) END) as Opdrachtgever, " +
                                            "(CASE WHEN OOG.opdrachtgeverHuisnummerToevoeging IS NULL THEN CONCAT(OOG.opdrachtgeverStraat, ' ', OOG.opdrachtgeverHuisnummer, ', ', OOG.opdrachtgeverPostcode, ', ', OOG.opdrachtgeverWoonplaats) ELSE CONCAT(OOG.opdrachtgeverStraat, ' ', OOG.opdrachtgeverHuisnummer, TRIM(OOG.opdrachtgeverHuisnummerToevoeging),', ', OOG.opdrachtgeverPostcode, ', ', OOG.opdrachtgeverWoonplaats) END) as Adres, " +
                                            "OOG.opdrachtgeverTelefoon " +
                                            "FROM OverledenePersoonsGegevens OPG " +
                                            "INNER JOIN OverledeneUitvaartInfo OUI ON OPG.uitvaartId = OUI.uitvaartId " +
                                            "INNER JOIN OverledeneOpdrachtgever OOG ON OOG.uitvaartId = OPG.uitvaartId " +
                                            "WHERE OPG.uitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync()) // ExecuteReaderAsync for asynchronous read
                    {
                        while (await reader.ReadAsync()) // ReadAsync for asynchronous read
                        {
                            koffieInfo = new KoffieKamerDocument()
                            {
                                DatumUitvaart = reader["uitvaartInfoDatumTijdUitvaart"] as DateTime? ?? default,
                                Naam = reader["Naam"].ToString(),
                                DienstLocatie = reader["uitvaartInfoDienstLocatie"].ToString(),
                                DienstDatum = reader["uitvaartInfoDienstDatumTijd"] as DateTime? ?? default,
                                DienstTijd = reader["uitvaartInfoDienstDatumTijd"] as DateTime? ?? default,
                                Opdrachtgever = reader["Opdrachtgever"].ToString(),
                                OpdrachtgeverAdres = reader["Adres"].ToString(),
                                OpdrachtgeverTelefoon = reader["opdrachtgeverTelefoon"].ToString(),
                                UitvaartId = UitvaartId
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving koffie kamer information.", ex);
            }

            return koffieInfo;
        }
        public async Task<DocumentDocument> GetDocumentDocumentInfoAsync(Guid UitvaartId)
        {
            DocumentDocument docInfo = new DocumentDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandText = "SELECT OUI.uitvaartInfoType, OUI.uitvaartInfoDatumTijdUitvaart, OUI.uitvaartInfoUitvaartLocatie, CONCAT(Initialen,' ',Achternaam) as UitvaartLeider, " +
                                            "CP.Email, OPG.overledeneAchternaam, OPG.overledeneVoornamen, OPG.overledeneGeboortedatum, OPG.overledeneGeboorteplaats, OPG.overledeneWoonplaats, " +
                                            "OOI.overledenDatumTijd, OOI.overledenGemeente, OOI.overledenPlaats " +
                                            "FROM OverledenePersoonsGegevens OPG " +
                                            "INNER JOIN OverledeneUitvaartleider OUL ON OUL.UitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN ConfigurationPersoneel CP ON OUL.PersoneelId = CP.Id " +
                                            "JOIN OverledeneOverlijdenInfo OOI ON OOI.UitvaartId = OPG.uitvaartId " +
                                            "LEFT JOIN OverledeneUitvaartInfo OUI ON OUI.uitvaartId = OPG.uitvaartId " +
                                            "WHERE OPG.uitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            docInfo = new DocumentDocument()
                            {
                                DocumentType = reader["uitvaartInfoType"].ToString(),
                                DatumUitvaart = reader["uitvaartInfoDatumTijdUitvaart"] as DateTime? ?? default,
                                LocatieUitvaart = reader["uitvaartInfoUitvaartLocatie"].ToString(),
                                UitvaartVerzorger = reader["UitvaartLeider"].ToString(),
                                UitvaartVerzorgerEmail = reader["Email"].ToString(),
                                GeslachtsnaamOverledene = reader["overledeneAchternaam"].ToString(),
                                VoornaamOverledene = reader["overledeneVoornamen"].ToString(),
                                GeboortedatumOverledene = reader["overledeneGeboortedatum"] as DateTime? ?? default,
                                GeboorteplaatsOverledene = reader["overledeneGeboorteplaats"].ToString(),
                                WoonplaatsOverledene = reader["overledeneWoonplaats"].ToString(),
                                DatumOverlijden = reader["overledenDatumTijd"] as DateTime? ?? default,
                                GemeenteOverlijden = reader["overledenGemeente"].ToString(),
                                PlaatsOverlijden = reader["overledenPlaats"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving document information.", ex);
            }

            return docInfo;
        }
        public async Task<ChecklistDocument> GetDocumentChecklistInfoAsync(Guid UitvaartId)
        {
            ChecklistDocument checklistInfo = new ChecklistDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync();
                    command.Connection = connection;
                    command.CommandText = "SELECT OUI.[uitvaartInfoType], OUI.uitvaartInfoDatumTijdUitvaart, OPG.overledeneAchternaam, " +
                                            "CASE WHEN OPG.overledeneTussenvoegsel IS NOT NULL AND LEN(OPG.overledeneTussenvoegsel) > 0 THEN CONCAT(OPG.overledeneAanhef,' ',OPG.overledeneVoornamen,' ',OPG.overledeneTussenvoegsel,' ',OPG.overledeneAchternaam) ELSE " +
                                            "CONCAT(OPG.overledeneAanhef,' ',OPG.overledeneVoornamen,' ',OPG.overledeneAchternaam) END AS volledigeNaam, " +
                                            "OOI.overledenDatumTijd, CV.verzekeraarNaam, OO.opbaringVerzorgingJson " +
                                            "FROM OverledeneUitvaartInfo OUI " +
                                            "LEFT JOIN OverledenePersoonsGegevens OPG ON OUI.uitvaartId = OPG.uitvaartId " +
                                            "LEFT JOIN OverledeneOverlijdenInfo OOI ON OUI.UitvaartId = OOI.uitvaartId " +
                                            "INNER JOIN ConfigurationVerzekeraar CV ON OOI.overledenHerkomst = CV.Id " +
                                            "LEFT JOIN OverledeneOpbaring OO ON OUI.uitvaartId = OO.uitvaartId " +
                                            "WHERE OUI.uitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            checklistInfo = new ChecklistDocument()
                            {
                                UitvaartType = reader["uitvaartInfoType"] != DBNull.Value ? reader["uitvaartInfoType"].ToString() : string.Empty,
                                DatumUitvaart = reader["uitvaartInfoDatumTijdUitvaart"] != DBNull.Value ? reader["uitvaartInfoDatumTijdUitvaart"].ToString() : string.Empty,
                                Achternaam = reader["overledeneAchternaam"] != DBNull.Value ? reader["overledeneAchternaam"].ToString() : string.Empty,
                                OverledenDatum = reader["overledenDatumTijd"] != DBNull.Value ? reader["overledenDatumTijd"].ToString() : string.Empty,
                                Herkomst = reader["verzekeraarNaam"] != DBNull.Value ? reader["verzekeraarNaam"].ToString() : string.Empty,
                                VolledigeNaam = reader["volledigeNaam"] != DBNull.Value ? reader["volledigeNaam"].ToString() : string.Empty,
                                OpbarenInfo = reader["opbaringVerzorgingJson"] != DBNull.Value ? reader["opbaringVerzorgingJson"].ToString() : string.Empty

                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                // For simplicity, this example just rethrows the exception
                throw new InvalidOperationException("An error occurred while retrieving checklist information.", ex);
            }

            return checklistInfo;
        }
        public async Task<OverdrachtDocument> GetDocumentOverdrachtInfoAsync(Guid UitvaartId)
        {
            var overdrachtInfo = new OverdrachtDocument();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                command.Connection = connection;
                command.CommandText = "SELECT uitvaartInfoType, overledeneAanhef, overledeneVoornamen, " +
                                      "CASE WHEN overledeneTussenvoegsel IS NOT NULL THEN CONCAT(overledeneTussenvoegsel, ' ', overledeneAchternaam) ELSE overledeneAchternaam END AS Achternaam, " +
                                      "CASE WHEN CP.Tussenvoegsel IS NOT NULL THEN CONCAT(CP.Voornaam, ' ', CP.Tussenvoegsel, ' ', CP.Achternaam) ELSE CONCAT(CP.Voornaam, ' ', CP.Achternaam) END AS PersoneelName " +
                                      "FROM OverledeneUitvaartInfo OUI " +
                                      "INNER JOIN OverledenePersoonsGegevens OPG ON OUI.uitvaartId = OPG.uitvaartId " +
                                      "INNER JOIN OverledeneUitvaartleider OU ON OU.UitvaartId = OUI.uitvaartId " +
                                      "INNER JOIN ConfigurationPersoneel CP ON OU.PersoneelId = CP.Id " +
                                      "WHERE OUI.uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        overdrachtInfo = new OverdrachtDocument
                        {
                            OverdrachtType = reader["uitvaartInfoType"].ToString(),
                            OverledeAanhef = reader["overledeneAanhef"].ToString(),
                            OverledeneVoornaam = reader["overledeneVoornamen"].ToString(),
                            OverledeneAchternaam = reader["Achternaam"].ToString(),
                            OverdragendePartij = reader["PersoneelName"].ToString()
                        };
                    }
                }
            }

            return overdrachtInfo;
        }
        public async Task<BloemenDocument> GetDocumentBloemenInfoAsync(Guid UitvaartId)
        {
            BloemenDocument bloemenInfo = new BloemenDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync(); // Open the connection asynchronously
                    command.Connection = connection;
                    command.CommandText = "SELECT leverancierName, " +
                                          "CASE " +
                                          "WHEN cp.Tussenvoegsel IS NULL OR cp.Tussenvoegsel = '' " +
                                          "THEN CONCAT(cp.Voornaam, ' ', cp.Achternaam) " +
                                          "ELSE CONCAT(cp.Voornaam, ' ', cp.Tussenvoegsel, ' ', cp.Achternaam) " +
                                          "END AS Uitvaartleider, Email, " +
                                          "CASE " +
                                          "WHEN opg.overledeneTussenvoegsel IS NULL OR opg.overledeneTussenvoegsel = '' " +
                                          "THEN CONCAT(opg.overledeneAanhef, ' ', opg.overledeneVoornamen, ' ', opg.overledeneAchternaam) " +
                                          "ELSE CONCAT(opg.overledeneAanhef, ' ', opg.overledeneVoornamen, ' ', opg.overledeneTussenvoegsel, ' ', opg.overledeneAchternaam) " +
                                          "END AS Overledene, bloemenText, bloemenLint, bloemenKaart, oo.opdrachtgeverTelefoon," +
                                          "bloemenLintJson, bloemenBezorgingDatum, bloemenBezorgingAdres " +
                                          "FROM [OverledeneBloemen] ob " +
                                          "INNER JOIN [ConfigurationLeveranciers] cl ON ob.bloemenLeverancier = cl.leverancierId " +
                                          "INNER JOIN [OverledeneUitvaartleider] ou ON ob.uitvaartId = ou.UitvaartId " +
                                          "INNER JOIN [ConfigurationPersoneel] cp ON ou.PersoneelId = cp.Id " +
                                          "INNER JOIN [OverledenePersoonsGegevens] opg ON ob.uitvaartId = opg.uitvaartId " +
                                          "INNER JOIN [OverledeneOpdrachtgever] oo ON ob.uitvaartId = oo.uitvaartId " +
                                          "WHERE ob.uitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync()) // ExecuteReaderAsync for asynchronous read
                    {
                        while (await reader.ReadAsync()) // ReadAsync for asynchronous read
                        {
                            bloemenInfo = new BloemenDocument()
                            {
                                LeverancierNaam = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                                Uitvaartleider = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                EmailUitvaartleider = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                NaamOverledene = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Bloemstuk = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                Lint = !reader.IsDBNull(5) && reader.GetBoolean(5),
                                Kaart = !reader.IsDBNull(6) && reader.GetBoolean(6),
                                Telefoonnummer = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                LintJson = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                DatumBezorgen = reader.IsDBNull(9) ? DateTime.Today : reader.GetDateTime(9),
                                Bezorgadres = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving bloemen information.", ex);
            }

            return bloemenInfo;
        }
        public async Task<BezittingenDocument> GetDocumentBezittingInfoAsync(Guid UitvaartId)
        {
            BezittingenDocument bezittingInfo = new BezittingenDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync(); // Open the connection asynchronously
                    command.Connection = connection;
                    command.CommandText = "SELECT OUL.Uitvaartnummer, OPG.overledeneAchternaam, OPG.overledeneVoornamen, OPG.overledeneGeboortedatum, " +
                                            "OOI.overledenDatumTijd, OO.opbaringLocatie, OOI.overledenPlaats, OOG.opdrachtgeverRelatieTotOverledene, " +
                                            "OO.opbaringSieradenOmschrijving, OO.opbaringSieradenRetour," +
                                            "(CASE WHEN OOG.opdrachtgeverTussenvoegsel IS NULL THEN CONCAT(OOG.opdrachtgeverAchternaam, ', ') ELSE CONCAT(OOG.opdrachtgeverTussenvoegsel, ' ', OOG.opdrachtgeverAchternaam, ', ') END) as Opdrachtgever, " +
                                            "opdrachtgeverVoornaamen," +
                                            "(CASE WHEN opdrachtgeverHuisnummerToevoeging IS NULL THEN CONCAT(opdrachtgeverStraat, ' ', opdrachtgeverHuisnummer) ELSE CONCAT(opdrachtgeverStraat, ' ', TRIM(opdrachtgeverHuisnummer), ' ', TRIM(opdrachtgeverHuisnummerToevoeging)) END) as AdresOpdrachtgever " +
                                            "FROM OverledenePersoonsGegevens OPG " +
                                            "INNER JOIN OverledeneUitvaartleider OUL ON OUL.UitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN OverledeneOverlijdenInfo OOI ON OPG.uitvaartId = OOI.UitvaartId " +
                                            "INNER JOIN OverledeneOpdrachtgever OOG ON OOG.uitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN OverledeneOpbaring OO ON OPG.uitvaartId = OO.uitvaartId " +
                                            "WHERE OPG.uitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync()) // ExecuteReaderAsync for asynchronous read
                    {
                        while (await reader.ReadAsync()) // ReadAsync for asynchronous read
                        {
                            string voorletters = string.Empty;
                            if (!string.IsNullOrEmpty(reader["opdrachtgeverVoornaamen"].ToString()))
                            {
                                string[] words = reader["opdrachtgeverVoornaamen"].ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                                voorletters = string.Join(" ", words.Select(word => char.ToUpper(word[0])));
                            }

                            bezittingInfo = new BezittingenDocument()
                            {
                                DossierNummer = reader["Uitvaartnummer"].ToString(),
                                OverledeneNaam = reader["overledeneAchternaam"].ToString(),
                                OverledeneVoornaam = reader["overledeneVoornamen"].ToString(),
                                OverledeneGeborenOp = reader["overledeneGeboortedatum"] as DateTime? ?? default,
                                OverledeneOverledenOp = reader["overledenDatumTijd"] as DateTime? ?? default,
                                OverledeneLocatieOpbaring = reader["opbaringLocatie"].ToString(),
                                OverledenePlaatsOverlijden = reader["overledenPlaats"].ToString(),
                                OverledeneRelatie = reader["opdrachtgeverRelatieTotOverledene"].ToString(),
                                OverledeneBezittingen = reader["opbaringSieradenOmschrijving"].ToString(),
                                OverledeneRetour = reader["opbaringSieradenRetour"].ToString(),
                                OpdrachtgeverAdres = reader["AdresOpdrachtgever"].ToString(),
                                OpdrachtgeverNaamVoorletters = reader["Opdrachtgever"].ToString() + voorletters
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving bezittingen information.", ex);
            }

            return bezittingInfo;
        }
        public async Task<CrematieDocument> GetDocumentCrematieInfoAsync(Guid UitvaartId)
        {
            CrematieDocument crematieInfo = new CrematieDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync(); // Open the connection asynchronously
                    command.Connection = connection;
                    command.CommandText = "SELECT uitvaartInfoDienstLocatie, uitvaartInfoType, " +
                                            "(CASE WHEN OO.opdrachtgeverTussenvoegsel IS NULL THEN OO.opdrachtgeverAchternaam ELSE CONCAT(OO.opdrachtgeverTussenvoegsel, ' ', OO.opdrachtgeverAchternaam) END) as Opdrachtgever, " +
                                            "OO.opdrachtgeverVoornaamen, CONCAT(OO.opdrachtgeverStraat, ' ', OO.opdrachtgeverHuisnummer, OO.opdrachtgeverHuisnummerToevoeging) as opdrachtgeverAdres, " +
                                            "OUI.uitvaartInfoDatumTijdUitvaart, OUL.Uitvaartnummer, CONCAT(Achternaam, ' ', CP.Initialen) as UitvaartLeider, " +
                                            "OPG.overledeneAchternaam, OPG.overledeneVoornamen, OEI.overledeneBurgelijkestaat, " +
                                            "(CASE WHEN OPG.overledeneHuisnummerToevoeging IS NULL THEN CONCAT(OPG.overledeneAdres, ' ', OPG.overledeneHuisnummer) ELSE CONCAT(OPG.overledeneAdres, ' ', OPG.overledeneHuisnummer, ' ', OPG.overledeneHuisnummerToevoeging) END) as Straat, " +
                                            "OPG.overledenePostcode, OEI.overledeneLevensovertuiging, OPG.overledeneLeeftijd, OPG.overledeneWoonplaats, " +
                                            "OPG.overledeneGeboortedatum, OPG.overledeneGeboorteplaats, " +
                                            "OOI.overledenDatumTijd, OOI.overledenPlaats, OA.asbestemming, uitvaartInfoDienstConsumpties, " +
                                            "OOI.overledenHerkomst, OUIM.AulaNaam, OUIM.AantalPersonen, OO.opdrachtgeverGeboortedatum, OO.opdrachtgeverPostcode, " +
                                            "OO.opdrachtgeverRelatieTotOverledene, OO.opdrachtgeverTelefoon, OO.opdrachtgeverWoonplaats, OO.opdrachtgeverEmail, OUI.uitvaartInfoDienstDatumTijd " +
                                            "FROM OverledeneOpdrachtgever OO " +
                                            "INNER JOIN OverledenePersoonsGegevens OPG ON OO.uitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN OverledeneExtraInfo OEI ON OEI.uitvaartId = OO.uitvaartId " +
                                            "INNER JOIN OverledeneUitvaartleider OUL ON OUL.UitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN ConfigurationPersoneel CP ON OUL.PersoneelId = CP.Id " +
                                            "JOIN OverledeneOverlijdenInfo OOI ON OOI.UitvaartId = OPG.uitvaartId " +
                                            "LEFT JOIN OverledeneUitvaartInfo OUI ON OUI.uitvaartId = OPG.uitvaartId " +
                                            "LEFT JOIN OverledeneAsbestemming OA ON OO.uitvaartId = OA.uitvaartId " +
                                            "LEFT JOIN OverledeneUitvaartInfoMisc OUIM ON OO.uitvaartId = OUIM.uitvaartId " +
                                            "WHERE OPG.uitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync()) // ExecuteReaderAsync for asynchronous read
                    {
                        while (await reader.ReadAsync()) // ReadAsync for asynchronous read
                        {
                            crematieInfo = new CrematieDocument()
                            {
                                Aanvangstrijd = (DateTime)reader["uitvaartInfoDatumTijdUitvaart"],
                                StartAula = (DateTime)reader["uitvaartInfoDienstDatumTijd"],
                                StartKoffie = (DateTime)reader["uitvaartInfoDienstDatumTijd"],
                                CrematieLocatie = reader["uitvaartInfoDienstLocatie"].ToString(),
                                CrematieVoor = reader["uitvaartInfoType"].ToString(),
                                OpdrachtgeverNaam = reader["Opdrachtgever"].ToString(),
                                OpdrachtgeverGebDatum = (DateTime)reader["opdrachtgeverGeboortedatum"],
                                OpdrachtgeverVoornamen = reader["opdrachtgeverVoornaamen"].ToString(),
                                OpdrachtgeverStraat = reader["opdrachtgeverAdres"].ToString(),
                                OpdrachtgeverPostcode = reader["opdrachtgeverPostcode"].ToString(),
                                OpdrachtgeverRelatie = reader["opdrachtgeverRelatieTotOverledene"].ToString(),
                                OpdrachtgeverTelefoon = reader["opdrachtgeverTelefoon"].ToString(),
                                OpdrachtgeverPlaats = reader["opdrachtgeverWoonplaats"].ToString(),
                                OpdrachtgeverEmail = reader["opdrachtgeverEmail"].ToString(),
                                CrematieDatum = (DateTime)reader["uitvaartInfoDatumTijdUitvaart"],
                                CrematieDossiernummer = reader["Uitvaartnummer"].ToString(),
                                Uitvaartverzorger = reader["UitvaartLeider"].ToString(),
                                OverledeneNaam = reader["overledeneAchternaam"].ToString(),
                                OverledeneVoornaam = reader["overledeneVoornamen"].ToString(),
                                OverledeneBurgStaat = reader["overledeneBurgelijkestaat"].ToString(),
                                OverledeneStraat = reader["Straat"].ToString(),
                                OverledeneGebDatum = (DateTime)reader["overledeneGeboortedatum"],
                                OverledeneGebPlaats = reader["overledeneGeboorteplaats"].ToString(),
                                OverledeneDatum = (DateTime)reader["overledenDatumTijd"],
                                OverledenePlaats = reader["overledenPlaats"].ToString(),
                                OverledenePostcode = reader["overledenePostcode"].ToString(),
                                OverledeneLevensovertuiging = reader["overledeneLevensovertuiging"].ToString(),
                                OverledeneLeeftijd = reader["overledeneLeeftijd"].ToString(),
                                OverledeneWoonplaats = reader["overledeneWoonplaats"].ToString(),
                                Asbestemming = reader["asbestemming"].ToString(),
                                Consumpties = reader["uitvaartInfoDienstConsumpties"].ToString(),
                                Herkomst = reader["overledenHerkomst"].ToString(),
                                AulaNaam = reader["AulaNaam"].ToString(),
                                AulaPersonen = (int)reader["AantalPersonen"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving crematie information.", ex);
            }

            return crematieInfo;
        }
        public async Task<BegrafenisDocument> GetDocumentBegrafenisInfoAsync(Guid UitvaartId)
        {
            BegrafenisDocument begrafenisInfo = new BegrafenisDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync(); // Open the connection asynchronously
                    command.Connection = connection;
                    command.CommandText = "SELECT (CASE WHEN OO.opdrachtgeverTussenvoegsel IS NULL THEN CONCAT(OO.opdrachtgeverAanhef, ' ', OO.opdrachtgeverVoornaamen, ' ', OO.opdrachtgeverAchternaam) ELSE CONCAT(OO.opdrachtgeverAanhef, ' ', OO.opdrachtgeverVoornaamen, ' ', OO.opdrachtgeverTussenvoegsel, ' ', OO.opdrachtgeverAchternaam) END) as Opdrachtgever, " +
                                            "CONCAT(OO.opdrachtgeverStraat, ' ', OO.opdrachtgeverHuisnummer, OO.opdrachtgeverHuisnummerToevoeging, ', ', OO.opdrachtgeverPostcode, ', ', OO.opdrachtgeverWoonplaats) as opdrachtgeverAdres, " +
                                            "OUI.uitvaartInfoDatumTijdUitvaart, OUI.uitvaartInfoDatumTijdUitvaart, OA.typeGraf, " +
                                            "CONCAT(Initialen, ' ', Achternaam) as UitvaartLeider, CP.Email as UitvaartLeiderEmail, " +
                                            "OPG.overledeneAchternaam, OPG.overledeneVoornamen, " +
                                            "OPG.overledeneGeboortedatum, OPG.overledeneGeboorteplaats, " +
                                            "OOI.overledenDatumTijd, OOI.overledenPlaats, OPG.overledeneBSN, " +
                                            "uitvaartInfoDienstKist, OUIM.AulaNaam, OUIM.AantalPersonen, CP.Mobiel, " +
                                            "OUIM.BegraafplaatsLocatie, OUIM.BegraafplaatsGrafNr " +
                                            "FROM OverledeneOpdrachtgever OO " +
                                            "INNER JOIN OverledenePersoonsGegevens OPG ON OO.uitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN OverledeneUitvaartleider OUL ON OUL.UitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN ConfigurationPersoneel CP ON OUL.PersoneelId = CP.Id " +
                                            "JOIN OverledeneOverlijdenInfo OOI ON OOI.UitvaartId = OPG.uitvaartId " +
                                            "LEFT JOIN OverledeneUitvaartInfo OUI ON OUI.uitvaartId = OPG.uitvaartId " +
                                            "LEFT JOIN OverledeneAsbestemming OA ON OO.uitvaartId = OA.uitvaartId " +
                                            "LEFT JOIN OverledeneUitvaartInfoMisc OUIM ON OO.uitvaartId = OUIM.UitvaartId " +
                                            "WHERE OPG.uitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync()) // ExecuteReaderAsync for asynchronous read
                    {
                        while (await reader.ReadAsync()) // ReadAsync for asynchronous read
                        {
                            begrafenisInfo = new BegrafenisDocument()
                            {
                                NaamOpdrachtgever = reader["Opdrachtgever"].ToString(),
                                AdresOpdrachtgever = reader["opdrachtgeverAdres"].ToString(),
                                DatumUitvaart = (DateTime)reader["uitvaartInfoDatumTijdUitvaart"],
                                SoortGraf = reader["typeGraf"].ToString(),
                                UitvaartLeider = reader["UitvaartLeider"].ToString(),
                                UitvaartLeiderEmail = reader["UitvaartLeiderEmail"].ToString(),
                                NaamOverledene = reader["overledeneAchternaam"].ToString(),
                                VoornamenOverledene = reader["overledeneVoornamen"].ToString(),
                                DatumGeboorte = ((DateTime)reader["overledeneGeboortedatum"]).Date,
                                PlaatsGeboorte = reader["overledeneGeboorteplaats"].ToString(),
                                DatumOverlijden = ((DateTime)reader["overledenDatumTijd"]).Date,
                                PlaatsOverlijden = reader["overledenPlaats"].ToString(),
                                BsnOverledene = reader["overledeneBSN"].ToString(),
                                KistType = reader["uitvaartInfoDienstKist"].ToString(),
                                AulaNaam = reader["AulaNaam"].ToString(),
                                AantalPersonen = (int)reader["AantalPersonen"],
                                UitvaartLeiderMobiel = reader["Mobiel"].ToString(),
                                Begraafplaats = reader["BegraafplaatsLocatie"].ToString(),
                                NrGraf = reader["BegraafplaatsGrafNr"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving begrafenis information.", ex);
            }

            return begrafenisInfo;
        }
        public async Task<TerugmeldingDocument> GetDocumentTerugmeldingInfoAsync(Guid UitvaartId)
        {
            var terugmeldingInfo = new TerugmeldingDocument();

            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                command.Connection = connection;
                command.CommandText = "SELECT OUL.Uitvaartnummer, CONCAT(Achternaam,' ',CP.Initialen) as UitvaartLeider, CP.Email, OVI.verzekeringProperties, OPG.overledeneAanhef, OPG.overledeneAchternaam, OPG.overledeneVoornamen, " +
                                      "(CASE WHEN OPG.overledeneHuisnummerToevoeging IS NULL THEN CONCAT(OPG.overledeneAdres, ' ', OPG.overledeneHuisnummer) ELSE CONCAT(OPG.overledeneAdres, ' ', TRIM(OPG.overledeneHuisnummer), ' ', TRIM(OPG.overledeneHuisnummerToevoeging)) END) as Adres, " +
                                      "OPG.overledenePostcode, OPG.overledeneWoonplaats, OPG.overledeneGeboorteplaats, OOI.overledenDatumTijd as OverledenOp, OOI.overledenAdres, OUI.uitvaartInfoDatumTijdUitvaart, " +
                                      "OUI.uitvaartInfoDatumTijdUitvaart, OUI.uitvaartInfoType, OUI.uitvaartInfoUitvaartLocatie, OO.opdrachtgeverAchternaam, " +
                                      "(CASE WHEN OO.opdrachtgeverHuisnummerToevoeging IS NULL THEN CONCAT(OO.opdrachtgeverStraat, ' ', OO.opdrachtgeverHuisnummer) ELSE CONCAT(OO.opdrachtgeverStraat, ' ', TRIM(OO.opdrachtgeverHuisnummer), ' ', TRIM(OO.opdrachtgeverHuisnummerToevoeging)) END) as AdresOpdrachtgever, " +
                                      "OO.opdrachtgeverPostcode, OO.opdrachtgeverWoonplaats, OO.opdrachtgeverRelatieTotOverledene, OO.opdrachtgeverTelefoon " +
                                      "FROM OverledeneOpdrachtgever OO " +
                                      "INNER JOIN OverledenePersoonsGegevens OPG ON OO.uitvaartId = OPG.uitvaartId " +
                                      "INNER JOIN OverledeneVerzerkeringInfo OVI ON OPG.uitvaartId = OVI.uitvaartId " +
                                      "INNER JOIN OverledeneUitvaartleider OUL ON OUL.UitvaartId = OPG.uitvaartId " +
                                      "INNER JOIN ConfigurationPersoneel CP ON OUL.PersoneelId = CP.Id " +
                                      "JOIN OverledeneOverlijdenInfo OOI ON OOI.UitvaartId = OPG.uitvaartId " +
                                      "LEFT JOIN OverledeneUitvaartInfo OUI ON OUI.uitvaartId = OPG.uitvaartId " +
                                      "WHERE OPG.uitvaartId = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                // Use ExecuteReaderAsync for async reading
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        terugmeldingInfo = new TerugmeldingDocument()
                        {
                            Dossiernummer = reader["Uitvaartnummer"].ToString(),
                            Uitvaartverzorger = reader["UitvaartLeider"].ToString(),
                            UitvaartverzorgerEmail = reader["Email"].ToString(),
                            Polisnummer = reader["verzekeringProperties"].ToString(),
                            OverledeneAanhef = reader["overledeneAanhef"].ToString(),
                            OverledeneNaam = reader["overledeneAchternaam"].ToString(),
                            OverledeneVoornamen = reader["overledeneVoornamen"].ToString(),
                            OverledeneAdres = reader["Adres"].ToString(),
                            OverledenePostcode = reader["overledenePostcode"].ToString(),
                            OverledeneWoonplaats = reader["overledeneWoonplaats"].ToString(),
                            OverledeneGeborenTe = reader["overledeneGeboorteplaats"].ToString(),
                            OverledeneOverledenOp = (DateTime)reader["OverledenOp"],
                            OverledeneOverledenTe = reader["overledenAdres"].ToString(),
                            OverledeneUitvaartDatum = (DateTime)reader["uitvaartInfoDatumTijdUitvaart"],
                            OverledeneUitvaartTijd = (DateTime)reader["uitvaartInfoDatumTijdUitvaart"],
                            OverledeneType = reader["uitvaartInfoType"].ToString(),
                            OverledeneUitvaartTe = reader["uitvaartInfoUitvaartLocatie"].ToString(),
                            OpdrachtgeverNaam = reader["opdrachtgeverAchternaam"].ToString(),
                            OpdrachtgeverAdres = reader["AdresOpdrachtgever"].ToString(),
                            OpdrachtgeverPostcode = reader["opdrachtgeverPostcode"].ToString(),
                            OpdrachtgeverPlaats = reader["opdrachtgeverWoonplaats"].ToString(),
                            OpdrachtgeverRelatie = reader["opdrachtgeverRelatieTotOverledene"].ToString(),
                            OpdrachtgeverTelefoon = reader["opdrachtgeverTelefoon"].ToString()
                        };
                    }
                }
            }

            return terugmeldingInfo;
        }
        public async Task<TevredenheidDocument> GetDocumentTevredenheidsInfoAsync(Guid UitvaartId)
        {
            TevredenheidDocument tevredenheidsInfo = new TevredenheidDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync(); // Open the connection asynchronously
                    command.Connection = connection;
                    command.CommandText = "SELECT Uitvaartnummer, (CASE WHEN CP.Tussenvoegsel IS NULL THEN CONCAT(CP.Initialen, ' ', CP.Achternaam) ELSE CONCAT(CP.Initialen, ' ', CP.Tussenvoegsel, ' ', CP.Achternaam) END) as Uitvaartleider, (CASE WHEN OO.opdrachtgeverTussenvoegsel IS NULL THEN CONCAT(OO.opdrachtgeverAanhef, ' ', OO.opdrachtgeverVoornaamen, ' ', OO.opdrachtgeverAchternaam) ELSE CONCAT(OO.opdrachtgeverAanhef, ' ', OO.opdrachtgeverVoornaamen, ' ', OO.opdrachtgeverTussenvoegsel, ' ', OO.opdrachtgeverAchternaam) END) as Opdrachtgever, " +
                                        "OO.opdrachtgeverWoonplaats, OO.opdrachtgeverTelefoon, " +
                                        "CONCAT(OO.opdrachtgeverStraat, ' ', OO.opdrachtgeverHuisnummer, OO.opdrachtgeverHuisnummerToevoeging) as opdrachtgeverAdres " +
                                        "FROM [OverledeneUitvaartleider] OUL " +
                                        "INNER JOIN ConfigurationPersoneel CP ON OUL.PersoneelId = CP.Id " +
                                        "INNER JOIN OverledeneOpdrachtgever OO ON OUL.UitvaartId = OO.uitvaartId " +
                                        "WHERE OUL.UitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync()) // ExecuteReaderAsync for asynchronous read
                    {
                        while (await reader.ReadAsync()) // ReadAsync for asynchronous read
                        {
                            tevredenheidsInfo = new TevredenheidDocument()
                            {
                                Dossiernummer = reader["Uitvaartnummer"].ToString(),
                                Uitvaartverzorger = reader["Uitvaartleider"].ToString(),
                                IngevuldDoorNaam = reader["Opdrachtgever"].ToString(),
                                IngevuldDoorAdres = reader["opdrachtgeverAdres"].ToString(),
                                IngevuldDoorTelefoon = reader["opdrachtgeverTelefoon"].ToString(),
                                IngevuldDoorWoonplaats = reader["opdrachtgeverWoonplaats"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving tevredenheids information.", ex);
            }

            return tevredenheidsInfo;
        }
        public async Task<AangifteDocument> GetDocumentAangifteInfoAsync(Guid UitvaartId)
        {
            AangifteDocument aangifteInfo = new AangifteDocument();

            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand())
                {
                    await connection.OpenAsync(); // Open the connection asynchronously
                    command.Connection = connection;
                    command.CommandText = "SELECT OPG.overledeneAanhef, OPG.overledeneAchternaam, OPG.overledeneVoornamen, OPG.overledeneGeboorteplaats, OPG.overledeneGeboortedatum, " +
                                            "(CASE WHEN (OPG.overledeneHuisnummerToevoeging IS NOT NULL) THEN CONCAT(OPG.overledeneAdres, ' ', trim(OPG.overledeneHuisnummer), ' ', OPG.overledeneHuisnummerToevoeging) " +
                                            "ELSE CONCAT(OPG.overledeneAdres, ' ', trim(OPG.overledeneHuisnummer)) END) AS OverledenAdres," +
                                            "OPG.overledenePostcode, OPG.overledeneWoonplaats, OPG.overledeneBSN, OOI.overledenDatumTijd," +
                                            "(CASE WHEN (OOI.overledenHuisnummerToevoeging IS NOT NULL) THEN CONCAT(OOI.overledenAdres, ' ', OOI.overledenHuisnummer, ' ', OOI.overledenHuisnummerToevoeging) " +
                                            "ELSE CONCAT(OOI.overledenAdres, ' ', OOI.overledenHuisnummer) END) AS OverledenAdres, OEI.overledeneEersteOuder, OEI.overledeneTweedeOuder, OEI.overledeneWedenaarVan, OEI.overledeneAantalKinderen," +
                                            "OEI.overledeneKinderenMinderjarig, OEI.overledeneAantalKinderenOverleden, " +
                                            "(CASE WHEN CP.Tussenvoegsel IS NULL THEN CONCAT(CP.Voornaam, ' ', CP.Achternaam) " +
                                            "ELSE CONCAT(CP.Voornaam, ' ', CP.Tussenvoegsel, ' ', CP.Achternaam) END) as Aangever," +
                                            "CONCAT(CP.Geboorteplaats,', ', CP.Geboortedatum) AS AangeverPlaats," +
                                            "(CASE WHEN OO.opdrachtgeverTussenvoegsel IS NULL THEN CONCAT(OO.opdrachtgeverAanhef, ' ', OO.opdrachtgeverVoornaamen, ' ', OO.opdrachtgeverAchternaam) " +
                                            "ELSE CONCAT(OO.opdrachtgeverAanhef, ' ', OO.opdrachtgeverVoornaamen, ' ', OO.opdrachtgeverTussenvoegsel, ' ', OO.opdrachtgeverAchternaam) END) as Opdrachtgever," +
                                            "CONCAT(OO.opdrachtgeverStraat, ' ', OO.opdrachtgeverHuisnummer, OO.opdrachtgeverHuisnummerToevoeging) as opdrachtgeverAdres, OO.opdrachtgeverPostcode, OO.opdrachtgeverWoonplaats," +
                                            "OUI.uitvaartInfoDatumTijdUitvaart, OUI.uitvaartInfoUitvaartLocatie, OUI.uitvaartInfoType, OOI.overledenSchouwarts, OUIM.UBS, " +
                                            "OEI.naamWederhelft, OEI.voornaamWederhelft, OEI.overledeneGescheidenVan, OEI.overledeneBurgelijkestaat " +
                                            "FROM [OverledeneUitvaartleider] OUL " +
                                            "INNER JOIN OverledenePersoonsGegevens OPG ON OUL.UitvaartId = OPG.uitvaartId " +
                                            "INNER JOIN OverledeneOverlijdenInfo OOI ON OUL.UitvaartId = OOI.UitvaartId " +
                                            "INNER JOIN OverledeneExtraInfo OEI ON OUL.UitvaartId = OEI.uitvaartId " +
                                            "INNER JOIN OverledeneUitvaartInfo OUI ON OUL.UitvaartId = OUI.uitvaartId " +
                                            "INNER JOIN ConfigurationPersoneel CP ON OUL.PersoneelId = CP.Id " +
                                            "INNER JOIN OverledeneOpdrachtgever OO ON OUL.UitvaartId = OO.uitvaartId " +
                                            "LEFT JOIN OverledeneUitvaartInfoMisc OUIM ON OUI.uitvaartId = OUIM.UitvaartId " +
                                            "WHERE OUL.UitvaartId = @UitvaartId";
                    command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                    using (var reader = await command.ExecuteReaderAsync()) // ExecuteReaderAsync for asynchronous read
                    {
                        while (await reader.ReadAsync()) // ReadAsync for asynchronous read
                        {
                            aangifteInfo = new AangifteDocument()
                            {
                                OverledeneAanhef = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                                OverledeneAchternaam = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                OverledeneVoornaam = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                OverledeneGeboorteplaats = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                OverledeneGeboortedatum = reader.IsDBNull(4) || reader.GetDateTime(4).Date == DateTime.MinValue.Date ? DateTime.MinValue : reader.GetDateTime(4),
                                OverledeneAdres = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                OverledenePostcode = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                OverledeneWoonplaats = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                OverledeneBSN = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                                DatumOverlijden = reader.IsDBNull(9) || reader.GetDateTime(9).Date == DateTime.MinValue.Date ? DateTime.MinValue : reader.GetDateTime(9),
                                TijdOverlijden = reader.IsDBNull(9) || reader.GetDateTime(9).Date == DateTime.MinValue.Date ? DateTime.MinValue : reader.GetDateTime(9),
                                AdresOverlijden = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                EersteOuder = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                TweedeOuder = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                                WeduwenaarVan = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                                AantalKinderen = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                                AantalKinderenMinderjarig = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                                AantalKinderenWaarvanOverleden = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                                AangeverNaam = reader.IsDBNull(17) ? string.Empty : reader.GetString(17),
                                AangeverPlaats = reader.IsDBNull(18) ? string.Empty : reader.GetString(18),
                                ErfgenaamVolledigeNaam = reader.IsDBNull(19) ? string.Empty : reader.GetString(19),
                                ErfgenaamStraat = reader.IsDBNull(20) ? string.Empty : reader.GetString(20),
                                ErfgenaamPostcode = reader.IsDBNull(21) ? string.Empty : reader.GetString(21),
                                ErfgenaamWoonplaats = reader.IsDBNull(22) ? string.Empty : reader.GetString(22),
                                DatumUitvaart = reader.IsDBNull(23) || reader.GetDateTime(23).Date == DateTime.MinValue.Date ? DateTime.MinValue : reader.GetDateTime(23),
                                TijdUitvaart = reader.IsDBNull(23) || reader.GetDateTime(23).Date == DateTime.MinValue.Date ? DateTime.MinValue : reader.GetDateTime(23),
                                LocatieUitvaart = reader.IsDBNull(24) ? string.Empty : reader.GetString(24),
                                TypeUitvaart = reader.IsDBNull(25) ? string.Empty : reader.GetString(25),
                                Schouwarts = reader.IsDBNull(26) ? string.Empty : reader.GetString(26),
                                UBS = reader.IsDBNull(27) ? string.Empty : reader.GetString(27),
                                NaamWederhelft = reader.IsDBNull(28) ? string.Empty : reader.GetString(28),
                                VoornamenWederhelft = reader.IsDBNull(29) ? string.Empty : reader.GetString(29),
                                GehuwdGeweestMet = reader.IsDBNull(30) ? string.Empty : reader.GetString(30),
                                Burgelijkestaat = reader.IsDBNull(31) ? string.Empty : reader.GetString(31)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, or return a default value)
                throw new InvalidOperationException("An error occurred while retrieving aangifte information.", ex);
            }

            return aangifteInfo;
        }
        public string GetUitvaartType(Guid UitvaartId)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT [uitvaartInfoType] FROM [OverledeneUitvaartInfo] WHERE [UitvaartId] = @UitvaartId";
                command.Parameters.AddWithValue("@UitvaartId", UitvaartId);

                var uitvaartType = command.ExecuteScalar();
                if (uitvaartType == null)
                {
                    return "Failed getting UitvaartType";
                }
                else
                {
                    return uitvaartType.ToString();
                }
            }
        }
        public int CheckLocationExistance(SuggestionModel newSuggestion)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) FROM ConfigurationOverledenLocaties WHERE Street = @street AND Housenumber = @housenumber AND Zipcode = @zipcode AND City = @city AND County = @county";
                command.Parameters.AddWithValue("@street", newSuggestion.Street);
                command.Parameters.AddWithValue("@housenumber", newSuggestion.HouseNumber);
                command.Parameters.AddWithValue("@zipcode", newSuggestion.ZipCode);
                command.Parameters.AddWithValue("@city", newSuggestion.City);
                command.Parameters.AddWithValue("@county", newSuggestion.County);

                int locationCount = (int)(command.ExecuteScalar() ?? 0); // Handle potential null

                return locationCount;
            }
        }
    }
}
