using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Alagoas;
using ApiDiariosOficiais.Models.Responses.Alagoas;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiDiariosOficiais.Services
{
    public class AlagoasService : IAlagoasService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public AlagoasService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiAlagoasResponse> GetAlagoasResponseAsync(ApiAlagoasRequestInicial requestInicial)
        {
            var result = new ApiAlagoasResponse
            {
                Resultados = new List<ResultadoAlagoas>()
            };

            try
            {
                ApiAlagoasResponseInicial json = await GetDataAsync(requestInicial);

                if(json.result.items.Count > 0)
                {
                    result.Pages = (int)Math.Ceiling(json.result.total_rows.value / 15.0);//dividir o numero de resultados por 15 pois é o numero de resultados por pagina
                    foreach (var items in json.result.items)
                    {
                        ResultadoAlagoas item = new();
                        item.Text += "...";
                        foreach(var highlights in items.highlight)
                        {
                            var sanitizedHighlights = SanitizeHighlights(highlights);
                            item.Text += sanitizedHighlights + "...";
                        }
                        item.Link = $"https://diario.imprensaoficial.al.gov.br/apinova/api/editions/viewPdf/{items.edition_id}";
                        result.Resultados.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return result;
        }

        private async Task<ApiAlagoasResponseInicial> GetDataAsync(ApiAlagoasRequestInicial requestInicial)
        {
            ApiAlagoasResponseInicial responseObject = new ApiAlagoasResponseInicial();
            var httpClient = _httpClientFactory.CreateClient("ApiAlagoas");
            var jsonPayload = JsonSerializer.Serialize(requestInicial);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {
                var response = await httpClient.PostAsync($"apinova/api/editions/searchES?page={requestInicial.Page}", content);
                response.EnsureSuccessStatusCode();

                // Send the POST request
                var responseString = await response.Content.ReadAsStringAsync();
                responseObject = JsonSerializer.Deserialize<ApiAlagoasResponseInicial>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex) { }
            return responseObject;

        }        

        private string SanitizeHighlights(string highlights)
        {
            string sanitizedString = Regex.Replace(highlights, "<.*?>", string.Empty);

            // Step 2: Decode escape sequences (e.g., <\\/em> -> </em>)
            sanitizedString = sanitizedString.Replace("\\/", "/");

            // Step 3: Remove unwanted characters (like \n)
            sanitizedString = sanitizedString.Replace("\n", " ").Replace("\r", " ");

            // Step 4: Normalize spaces (trim and collapse multiple spaces)
            return Regex.Replace(sanitizedString, @"\s+", " ").Trim();
        }
    }
}
