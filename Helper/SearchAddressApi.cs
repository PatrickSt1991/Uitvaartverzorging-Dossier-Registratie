using Dossier_Registratie.Models;
using Dossier_Registratie.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Dossier_Registratie.Helper
{
    public class SearchAddressApi : ViewModelBase
    {
        public static async Task<SearchAddressApiModel> GetAddressAsync(string postalCode, string houseNumber, string houseNumberAddition)
        {
            string combinedHouseNumber = CombineHouseNumber(houseNumber, houseNumberAddition);
            string apiUrl = BuildApiUrl(postalCode, combinedHouseNumber);

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                ShowOfflineMessage(response.StatusCode);
                return new SearchAddressApiModel(); 
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse(responseBody);

            return ParseAddressResponse(jsonResponse, combinedHouseNumber, ref houseNumberAddition) ?? new SearchAddressApiModel(); 
        }
        private static string CombineHouseNumber(string houseNumber, string houseNumberAddition)
        {
            return string.IsNullOrEmpty(houseNumberAddition)
                ? houseNumber.Trim().ToUpper()
                : houseNumber.Trim() + houseNumberAddition.Trim().ToUpper();
        }
        private static string BuildApiUrl(string postalCode, string combinedHouseNumber)
        {
            return $"https://api.pdok.nl/bzk/locatieserver/search/v3_1/free?q=postcode:{postalCode} AND huisnummer:{combinedHouseNumber}";
        }
        private static void ShowOfflineMessage(HttpStatusCode statusCode)
        {
            MessageBox.Show($"PDOK is offline {statusCode}", "Publieke Dienstverlening Op de Kaart", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private static SearchAddressApiModel? ParseAddressResponse(JObject jsonResponse, string combinedHouseNumber, ref string houseNumberAddition)
        {
            if (jsonResponse["response"]?["docs"] is not JArray docsArray || !docsArray.HasValues)
                return null;

            var firstDoc = docsArray.FirstOrDefault();

            if (firstDoc == null)
                return null;

            string streetResult = ExtractStreetResult(docsArray.First, combinedHouseNumber);

            foreach (var doc in docsArray.OfType<JObject>())
            {
                if (doc["weergavenaam"]?.ToString() == streetResult)
                {
                    houseNumberAddition = doc["huisletter"]?.ToString() ?? houseNumberAddition;
                    return CreateAddressModel(doc, houseNumberAddition);
                }
            }

            return null;
        }
        private static string ExtractStreetResult(JToken? firstDoc, string combinedHouseNumber)
        {
            if (firstDoc is not JObject addressObject) return string.Empty;

            return $"{addressObject["straatnaam"]} {combinedHouseNumber}, {addressObject["postcode"]} {addressObject["woonplaatsnaam"]}";
        }
        private static SearchAddressApiModel CreateAddressModel(JObject doc, string houseNumberAddition)
        {
            return new SearchAddressApiModel
            {
                PostalCode = doc["postcode"]?.ToString() ?? string.Empty,
                HouseNumber = doc["huisnummer"]?.ToString() ?? string.Empty,
                HouseNumberAddition = houseNumberAddition,
                Street = doc["straatnaam"]?.ToString() ?? string.Empty,
                City = doc["woonplaatsnaam"]?.ToString() ?? string.Empty,
                County = doc["gemeentenaam"]?.ToString() ?? string.Empty
            };
        }
        public static async Task<SearchAddressApiModel> GetLocationAsync(string queryLocation)
        {
            string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(queryLocation)}&format=json";

            using HttpClient client = new();
            try
            {
                client.DefaultRequestHeaders.Add("User-Agent", "DossierRegistratie/1.0");

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                JArray results = JArray.Parse(responseBody);

                var place = results.FirstOrDefault();

                if (place != null)
                {
                    string[]? parts = place["display_name"]?.ToString()
                                          .Split([','], StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => p.Trim()).ToArray();

                    var address = new SearchAddressApiModel
                    {
                        Location = place["name"]?.ToString() ?? string.Empty,
                        PostalCode = parts?.Length > 1 ? parts.ElementAtOrDefault(parts.Length - 2) : null,
                        County = parts?.Length > 4 ? parts.ElementAtOrDefault(parts.Length - 5) : null,
                        City = parts?.Length > 6 ? parts.ElementAtOrDefault(parts.Length - 6) : null,
                        Street = parts?.Length > 7 ? parts.ElementAtOrDefault(parts.Length - 7) : null,
                        HouseNumber = parts?.Length > 8 ? parts.ElementAtOrDefault(parts.Length - 8) : null

                    };

                    return address;
                }
                else
                {
                    return new SearchAddressApiModel();
                }
            }
            catch (HttpRequestException httpEx)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(httpEx);
                return new SearchAddressApiModel();
            }
            catch (JsonException jsonEx)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(jsonEx);
                return new SearchAddressApiModel();
            }
            catch (Exception ex)
            {
                await ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                return new SearchAddressApiModel();
            }
        }
    }
}
