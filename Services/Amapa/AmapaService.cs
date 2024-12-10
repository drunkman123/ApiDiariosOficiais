using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Amapa;
using ApiDiariosOficiais.Models.Responses.Amapa;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiDiariosOficiais.Services.Amapa
{
    public class AmapaService : IAmapaService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public AmapaService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var amapaRequestInicial = requestInicial.ToApiAmapaRequestInicialDomain();

            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                ApiAmapaResponseInicial json = await GetDataAsync(amapaRequestInicial);

                if (json.hits != null && json.hits.hits.Count > 0)
                {
                    result.Pages = (int)Math.Ceiling(json.hits.total / 10.0);//dividir o numero de resultados por 10 pois é o numero de resultados por pagina
                    foreach (var items in json.hits.hits)
                    {
                        Resultado item = new();
                        item.Text += "...";
                        foreach (var highlights in items.highlight.conteudo)
                        {
                            var sanitizedHighlights = SanitizeHighlights(highlights);
                            item.Text += sanitizedHighlights + "...";
                        }
                        item.Link = $"https://diofe.portal.ap.gov.br/cleanpdf/?file=/apifront/portal/edicoes/pdf_diario/{items._source.diario_id}/{items._source.pagina}&find={amapaRequestInicial.SearchText}";

                        DateTime date = DateTime.Parse(items._source.data);

                        item.Date = date;

                        result.Resultados.Add(item);

                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            result.Success = true;

            return result;
        }

        private async Task<ApiAmapaResponseInicial> GetDataAsync(ApiAmapaRequestInicial requestInicial)
        {
            ApiAmapaResponseInicial responseObject = new ();
            var httpClient = _httpClientFactory.CreateClient("ApiAmapa");

            try
            {
                var response = await httpClient.GetAsync($"busca/busca/buscar/query/{requestInicial.Page}/di:{requestInicial.InitialDate}/df:{requestInicial.FinalDate}/?1=1&q=%22{requestInicial.SearchText}%22");
                response.EnsureSuccessStatusCode();

                // Send the POST request
                var responseString = await response.Content.ReadAsStringAsync();
                responseObject = JsonSerializer.Deserialize<ApiAmapaResponseInicial>(responseString, new JsonSerializerOptions
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
