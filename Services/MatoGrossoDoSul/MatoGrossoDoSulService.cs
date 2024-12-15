using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.MatoGrossoDoSul;
using ApiDiariosOficiais.Models.Responses.MatoGrossoDoSul;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiDiariosOficiais.Services.MatoGrossoDoSul
{
    public class MatoGrossoDoSulService : IMatoGrossoDoSulService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public MatoGrossoDoSulService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var matoGrossoDoSulRequestInicial = requestInicial.ToApiMatoGrossoDoSulRequestInicialDomain();

            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                ApiMatoGrossoDoSulResponseInicial json = await GetDataAsync(matoGrossoDoSulRequestInicial);

                if (json.dataElastic.Count > 0)
                {
                    result.Pages = (int)Math.Ceiling(json.totalDataElastic / 10.0);//esse resultado deverá ser a quantidade de paginas para o front pagina direto
                    foreach (var items in json.dataElastic)
                    {
                        Resultado item = new();
                        string sanitizedText = SanitizeHighlights(items.Source.Texto.Trim());
                        item.Text = sanitizedText;
                        item.Title = $@"Página {items.Source.Pagina} - Caderno {items.Source.Descricao.Trim()}";
                        int startIndex = items.Source.DataInicioPublicacaoArquivo.IndexOf('(') + 1;
                        int endIndex = items.Source.DataInicioPublicacaoArquivo.IndexOf(')');

                        // Extract the timestamp substring
                        string timestampString = items.Source.DataInicioPublicacaoArquivo.Substring(startIndex, endIndex - startIndex);
                        long timestamp = long.Parse(timestampString);
                        DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime.Date;
                        item.Date = date;
                        item.Link = $"https://www.spdo.ms.gov.br/diariodoe/Index/PaginaDocumento/{items.Source.DocumentoID}/?Pagina={items.Source.Pagina}";
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

        private async Task<ApiMatoGrossoDoSulResponseInicial> GetDataAsync(ApiMatoGrossoDoSulRequestInicial requestInicial)
        {
            ApiMatoGrossoDoSulResponseInicial responseObject = new();

            var httpClient = _httpClientFactory.CreateClient("ApiMatoGrossoDoSul");

            var formData = new Dictionary<string, string>
            {
                { "Filter.Numero", requestInicial.Numero }, // Empty value
                { "Filter.DataInicial", requestInicial.DataInicial },
                { "Filter.DataFinal", requestInicial.DataFinal },
                { "Filter.Texto", requestInicial.Texto },
                { "Filter.TipoBuscaEnum", requestInicial.TipoBuscaEnum }
            };
            var content = new FormUrlEncodedContent(formData);

            //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

            try
            {
                var response = await httpClient.PostAsync($"DiarioDOE/Index/Index/{requestInicial.Page}", content);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                responseObject = JsonSerializer.Deserialize<ApiMatoGrossoDoSulResponseInicial>(responseString, new JsonSerializerOptions
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
            if (string.IsNullOrEmpty(highlights))
                return string.Empty;

            // Step 1: Remove HTML tags
            string sanitizedString = Regex.Replace(highlights, "<.*?>", string.Empty);

            // Step 2: Decode escaped characters (e.g., <\\/em> -> </em>)
            sanitizedString = sanitizedString.Replace("\\/", "/");

            // Step 3: Remove unwanted characters (e.g., \n and \r)
            sanitizedString = sanitizedString.Replace("\\n", " ").Replace("\\r", " ");

            // Step 4: Normalize spaces by trimming and collapsing multiple spaces into one
            sanitizedString = Regex.Replace(sanitizedString, @"\s+", " ").Trim();

            return sanitizedString;
        }
    }
}
