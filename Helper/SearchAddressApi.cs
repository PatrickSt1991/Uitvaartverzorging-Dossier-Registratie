using Dossier_Registratie.Models;
using Dossier_Registratie.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Dossier_Registratie.Helper
{
    public class SearchAddressApi : ViewModelBase
    {
        public static async Task<SearchAddressApiModel> GetAddressAsync(string postalCode, string houseNumber, string houseNumberAddition)
        {
            string combinedHouseNumber = string.Empty;
            if (string.IsNullOrEmpty(houseNumberAddition))
            {
                combinedHouseNumber = houseNumber.Trim().ToUpper();
            }
            else
            {
                combinedHouseNumber = houseNumber.Trim() + houseNumberAddition.Trim().ToUpper();
            }

            string apiUrl = $"https://api.pdok.nl/bzk/locatieserver/search/v3_1/free?q=postcode:{postalCode} AND huisnummer:{combinedHouseNumber}";
            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseBody);

                if (jsonResponse["response"]["docs"] is JArray docsArray && docsArray.HasValues)
                {
                    var adressResult = docsArray.First;
                    if (adressResult != null && adressResult.Type == JTokenType.Object)
                    {
                        var addressObject = (JObject)adressResult;
                        string streetResult = $"{addressObject["straatnaam"]} {combinedHouseNumber}, {addressObject["postcode"]} {addressObject["woonplaatsnaam"]}";

                        foreach (var doc in docsArray)
                        {
                            var docObject = doc as JObject;
                            if (docObject != null)
                            {
                                if (docObject["weergavenaam"]?.ToString() == streetResult)
                                {
                                    houseNumberAddition = docObject["huisletter"]?.ToString() ?? houseNumberAddition;

                                    var address = new SearchAddressApiModel
                                    {
                                        PostalCode = docObject["postcode"]?.ToString(),
                                        HouseNumber = docObject["huisnummer"]?.ToString(),
                                        HouseNumberAddition = houseNumberAddition,
                                        Street = docObject["straatnaam"]?.ToString(),
                                        City = docObject["woonplaatsnaam"]?.ToString(),
                                        County = docObject["gemeentenaam"]?.ToString()
                                    };

                                    return address;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show($"PDOK is offline {response.StatusCode}", "Publieke Dienstverlening Op de Kaart", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return null;
        }
        public static async Task<SearchAddressApiModel> GetLocationAsync(string queryLocation)
        {
            string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(queryLocation)}&format=json";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Add a User-Agent header to the request
                    client.DefaultRequestHeaders.Add("User-Agent", "DossierRegistratie/1.0");

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JArray results = JArray.Parse(responseBody);

                    // Get the first result or null if none exist
                    var place = results.FirstOrDefault();

                    if (place != null)
                    {
                        string[] parts = place["display_name"]?.ToString()
                                              .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(p => p.Trim()).ToArray();


                        // Create an instance of SearchAddressApiModel with safe access to array elements
                        var address = new SearchAddressApiModel
                        {
                            Location = place["name"]?.ToString(),
                            PostalCode = parts.Length > 1 ? parts.ElementAtOrDefault(parts.Length - 2) : null,
                            County = parts.Length > 4 ? parts.ElementAtOrDefault(parts.Length - 5) : null,
                            City = parts.Length > 6 ? parts.ElementAtOrDefault(parts.Length - 6) : null,
                            Street = parts.Length > 7 ? parts.ElementAtOrDefault(parts.Length - 7) : null,
                            HouseNumber = parts.Length > 8 ? parts.ElementAtOrDefault(parts.Length - 8) : null,
                        };

                        return address; // Return the address object
                    }
                    else
                    {
                        return null; // Return null if no results
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(httpEx);
                    return null; // Return null in case of an exception
                }
                catch (JsonException jsonEx)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(jsonEx);
                    return null; // Return null if JSON parsing fails
                }
                catch (Exception ex)
                {
                    ConfigurationGithubViewModel.GitHubInstance.SendStacktraceToGithubRepo(ex);
                    return null; // Return null for any other unexpected errors
                }
            }
        }


    }
}
