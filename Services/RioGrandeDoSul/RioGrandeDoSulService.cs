using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.RioGrandeDoSul;
using ApiDiariosOficiais.Models.Responses.RioGrandeDoSul;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiDiariosOficiais.Services.RioGrandeDoSul
{
    public class RioGrandeDoSulService : IRioGrandeDoSulService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public RioGrandeDoSulService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var rioGrandeDoSulRequestInicial = requestInicial.ToApiRioGrandeDoSulRequestInicialDomain();

            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                ApiRioGrandeDoSulResponseInicial jsonDOE = await GetDataAsync(rioGrandeDoSulRequestInicial, "DOE");
                ApiRioGrandeDoSulResponseInicial jsonDIC = await GetDataAsync(rioGrandeDoSulRequestInicial, "DIC");
                var combinedCollection = jsonDOE.collection
                                        .Concat(jsonDIC.collection)
                                        .OrderByDescending(c => c.procergs.data)
                                        .ToList();
                if (combinedCollection.Count > 0)
                {
                    int jsonDOEPage = (int)Math.Ceiling(jsonDOE.collectionSize / 20.0);
                    int jsonDICPage = (int)Math.Ceiling(jsonDIC.collectionSize / 20.0);
                    result.Pages = jsonDOEPage > jsonDICPage ? jsonDOEPage : jsonDICPage;
                    foreach (var items in combinedCollection)
                    {

                        Resultado item = new();

                        var sanitizedHighlights = SanitizeHighlights(items.procergs.conteudo);

                        item.Text += sanitizedHighlights;

                        item.Title = items.procergs.tipo.ToUpper().Trim();

                        item.Link = $"https://secweb.procergs.com.br/doe/rest/public/materias/geracao/pdf/{items.procergs.id}";

                        DateTime date = DateTime.Parse(items.procergs.data);

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

        private async Task<ApiRioGrandeDoSulResponseInicial> GetDataAsync(ApiRioGrandeDoSulRequestInicial requestInicial, string tipoDiario)
        {
            ApiRioGrandeDoSulResponseInicial responseObject = new();
            var httpClient = _httpClientFactory.CreateClient("ApiRioGrandeDoSul");

            try
            {
                var response = await httpClient.GetAsync($"doe/rest/public/materias/?page={requestInicial.Page}&tipoDiario={tipoDiario}&palavraChave={requestInicial.SearchText}&dataIni={requestInicial.InitialDate}&dataFim={requestInicial.FinalDate}");
                response.EnsureSuccessStatusCode();

                // Send the POST request
                var responseString = await response.Content.ReadAsStringAsync();
                responseObject = JsonSerializer.Deserialize<ApiRioGrandeDoSulResponseInicial>(responseString, new JsonSerializerOptions
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
