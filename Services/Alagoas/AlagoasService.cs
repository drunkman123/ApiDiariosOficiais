using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Alagoas;
using ApiDiariosOficiais.Models.Responses.Alagoas;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiDiariosOficiais.Services.Alagoas
{
    public class AlagoasService : IAlagoasService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public AlagoasService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var alagoasRequestInicial = requestInicial.ToApiAlagoasRequestInicialDomain();

            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                ApiAlagoasResponseInicial json = await GetDataAsync(alagoasRequestInicial);

                if (json.result.items.Count > 0)
                {
                    result.Pages = (int)Math.Floor(json.result.total_rows.value / 15.0);//dividir o numero de resultados por 15 pois é o numero de resultados por pagina
                    foreach (var items in json.result.items)
                    {
                        Resultado item = new();
                        item.Text += "...";
                        foreach (var highlights in items.highlight)
                        {
                            var sanitizedHighlights = SanitizeHighlights(highlights);
                            item.Text += sanitizedHighlights + "...";
                        }
                        item.Link = $"https://diario.imprensaoficial.al.gov.br/apinova/api/editions/viewPdf/{items.edition_id}";

                        DateTime date = DateTime.Parse(items.publication_date);

                        item.Date = date;

                        result.Resultados.Add(item);

                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
            result.Success = true;

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
            catch (Exception)
            {
                throw;
            }
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
