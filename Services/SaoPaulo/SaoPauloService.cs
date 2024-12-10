using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.SaoPaulo;
using ApiDiariosOficiais.Models.Responses.SaoPaulo;
using System.Text;
using System.Text.Json;

namespace ApiDiariosOficiais.Services.Alagoas
{
    public class SaoPauloService : ISaoPauloService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public SaoPauloService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                var saoPauloRequestInicial = requestInicial.ToApiSaoPauloRequestInicialDomain();

                ApiSaoPauloResponseInicial json = await GetDataAsync(saoPauloRequestInicial);

                if (json.items.Count > 0)
                {
                    result.Pages = json.totalPages;
                    foreach (var items in json.items)
                    {
                        Resultado item = new();
                        item.Title = items.title;
                        item.Text = "..." + items.excerpt + "...";
                        item.Link = $"https://doe.sp.gov.br/{items.slug}";
                        item.Date = items.date;
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

        private async Task<ApiSaoPauloResponseInicial> GetDataAsync(ApiSaoPauloRequestInicial requestInicial)
        {
            ApiSaoPauloResponseInicial responseObject = new();
            var httpClient = _httpClientFactory.CreateClient("ApiSaoPaulo");
            try
            {
                var response = await httpClient.GetAsync($"advanced-search/publications?periodStartingDate=personalized&PageNumber={requestInicial.PageNumber}&Terms%5B0%5D={requestInicial.Terms}&FromDate={requestInicial.FromDate}&ToDate={requestInicial.ToDate}&PageSize=10&SortField=Date");
                response.EnsureSuccessStatusCode();

                // Send the POST request
                var responseString = await response.Content.ReadAsStringAsync();
                responseObject = JsonSerializer.Deserialize<ApiSaoPauloResponseInicial>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                throw;
            }
            return responseObject;

        }

    }
}
