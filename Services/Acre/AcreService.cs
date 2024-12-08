using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Acre;
using HtmlAgilityPack;
using System.Net.Http.Headers;

namespace ApiDiariosOficiais.Services.Acre
{
    public class AcreService : IAcreService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public AcreService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var acreRequestInicial = requestInicial.ToApiAcreRequestInicialDomain();

            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                var pageContent = await SubmitSearchFormAsync(acreRequestInicial);


                if (!string.IsNullOrEmpty(pageContent))
                {
                    var document = ParseHtml(pageContent);

                    //RemoveCalendarioDiv(document); //se nao remover a extração dos links buga

                    ExtractLinks(document, result);
                    ExtractTextFromTd(document, result);
                    ExtractLastPageNumber(document, result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            result.Success = true;

            return result;
        }

        private async Task<string> SubmitSearchFormAsync(ApiAcreRequestInicial requestInicial)
        {
            var responseBody = string.Empty;
            var client = _httpClientFactory.CreateClient("ApiAcre");
            var formData = new Dictionary<string, string>
                            {
                                { "paginaIni", requestInicial.PaginaIni },  // Set the updated paginaIni value
                                { "palavraTipo", "" },
                                { "ano_palavra", requestInicial.AnoPalavra },
                                { "palavra", requestInicial.Palavra }
                            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var content = new FormUrlEncodedContent(formData);
            try
            {
                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(string.Empty, content);

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Handle failure
                    Console.WriteLine($"Error: {response.StatusCode}");
                }

            }
            catch (Exception ex) { }
            return responseBody;

        }

        private HtmlDocument ParseHtml(string htmlContent)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            return document;
        }

        private void RemoveCalendarioDiv(HtmlDocument document)
        {
            var calendarioDiv = document.DocumentNode.SelectSingleNode("//div[@id='calendario']");
            calendarioDiv?.Remove();
        }

        private void ExtractLinks(HtmlDocument document, DiarioResponse result)
        {
            var links = document.DocumentNode.SelectNodes("//tbody//a");

            if (links != null)
            {
                foreach (var link in links)
                {
                    result.Resultados.Add(new Resultado
                    {
                        Link = link.GetAttributeValue("href", string.Empty)
                    });
                }
            }
            else
            {
                Console.WriteLine("No <a> tags found inside <tbody>.");
            }
        }

        private void ExtractTextFromTd(HtmlDocument document, DiarioResponse result)
        {
            var tdNodes = document.DocumentNode.SelectNodes("//td[@colspan='3']");

            if (tdNodes != null && tdNodes.Count > 0)
            {
                for (int i = 0; i < result.Resultados.Count; i++)
                {
                    result.Resultados[i].Text = tdNodes[i].InnerText.Replace("\n", " ").Trim();
                }
            }
            else
            {
                Console.WriteLine("No <td colspan='3'> elements found.");
            }
        }

        private void ExtractLastPageNumber(HtmlDocument document, DiarioResponse result)
        {
            var lastPageNode = document.DocumentNode.SelectNodes("//span[contains(@onclick, 'vaiParaPaginaBusca')]")?.LastOrDefault();

            if (lastPageNode != null)
            {
                var onclickValue = lastPageNode.GetAttributeValue("onclick", string.Empty);
                var startIndex = onclickValue.IndexOf("(") + 1;
                var endIndex = onclickValue.IndexOf(")");
                var pageValue = onclickValue.Substring(startIndex, endIndex - startIndex);

                result.Pages = Convert.ToInt32(pageValue);
            }
            else
            {
                Console.WriteLine("No 'vaiParaPaginaBusca' found.");
            }
        }
    }
}
