using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.SaoPaulo;
using ApiDiariosOficiais.Models.Responses.SaoPaulo;
using HtmlAgilityPack;
using System.Globalization;
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
                await GetImprensaDataAsync(saoPauloRequestInicial, result);

                if (json.items.Count > 0)
                {
                    result.Pages = json.totalPages > result.Pages ? json.totalPages : result.Pages;
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
                result.Resultados = result.Resultados
                                    .OrderByDescending(r => r.Date)
                                    .ToList();
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
        private async Task GetImprensaDataAsync(ApiSaoPauloRequestInicial requestInicial, DiarioResponse result)
        {
            var httpClient = _httpClientFactory.CreateClient("ApiSaoPauloImprensa");
            try
            {
                var response = await httpClient.GetAsync($"DO/BuscaDO2001Resultado_11_3.aspx?filtropalavraschave={requestInicial.Terms}+&f=xhitlist&xhitlist_vpc={requestInicial.PageNumber}&xhitlist_x=Advanced&xhitlist_q=({requestInicial.Terms})&xhitlist_mh=9999&filtrotipopalavraschavesalvar=UP&filtrotodoscadernos=True&xhitlist_hc=%5bXML%5d%5bKwic%2c3%5d&xhitlist_vps=15&xhitlist_xsl=xhitlist.xsl&xhitlist_s=&xhitlist_sel=title%3bField%3adc%3atamanho%3bField%3adc%3adatapubl%3bField%3adc%3acaderno%3bitem-bookmark%3bhit-context");
                response.EnsureSuccessStatusCode();

                // Send the POST request
                var responseString = await response.Content.ReadAsStringAsync();
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(responseString);
                var anchors = document.DocumentNode
                            .SelectNodes("//div[contains(@class, 'resultadoBuscaItem')]//a");
                var pageNumberNode = document.DocumentNode.SelectSingleNode("//span[@id='content_lblDocumentosEncontrados']");
                string numberText = pageNumberNode.InnerText.Trim();


                if (anchors != null)
                {
                    result.Pages = (int)Math.Ceiling(Convert.ToInt32(numberText)/15.0);
                    for (int i = 0; i < anchors.Count; i += 3)
                    {
                        Resultado item = new();
                        item.Title = anchors[i].InnerText.Replace("\r\n", "").Trim();
                        item.Text = anchors[i + 2].InnerText.Replace("\r\n", "").Trim();
                        item.Link = $"https://www.imprensaoficial.com.br/{anchors[i].GetAttributeValue("href", string.Empty)}";
                        if (DateTime.TryParseExact(item.Title.Trim().Split(" - ")[0], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime extractedDate))
                        {
                            // Set the date to a DateTime variable
                            item.Date = extractedDate;
                        }
                        result.Resultados.Add(item);
                    }
                }
                else
                {
                    Console.WriteLine("No anchors found.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
