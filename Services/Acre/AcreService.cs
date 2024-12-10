using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Acre;
using HtmlAgilityPack;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

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
                    if (result.Resultados.Count > 0)
                    {

                        ExtractTextFromTd(document, result);
                        ExtractLastPageNumber(document, result);
                        ExtractDates(document, result);
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
            catch (Exception ex)
            {
                throw;
            }
            return responseBody;

        }

        private HtmlDocument ParseHtml(string htmlContent)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            return document;
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

            for (int i = 0; i < result.Resultados.Count; i++)
            {
                result.Resultados[i].Text = tdNodes[i].InnerText.Replace("\n", " ").Trim();
            }

        }

        private void ExtractLastPageNumber(HtmlDocument document, DiarioResponse result)
        {
            var node = document.DocumentNode
                                      .SelectSingleNode("//div[@class='resultados_busca']/p[2]");

            string text = node.InnerText;
            var number = new string(text.Where(char.IsDigit).ToArray());
            if (number.Length > 0)
            {
                result.Pages = (int)Math.Ceiling(Convert.ToInt32(number) / 10.0);
            }

        }

        private void ExtractDates(HtmlDocument document, DiarioResponse result)
        {
            var datePattern = @"\b\d{2}/\d{2}/\d{4}\b";
            var regex = new Regex(datePattern);
            var dates = document.DocumentNode.SelectNodes("//td")
                       .Select(node => node.InnerText.Trim())
                       .Where(text => regex.IsMatch(text)) // Match only valid date formats
                       .Select(text => regex.Match(text).Value).ToList(); // Extract the date

            for (int i = 0; i < result.Resultados.Count; i++)
            {
                // Parse the string into a DateTime object with the specific format
                DateTime date = DateTime.ParseExact(dates[i], "dd/MM/yyyy", null);

                // Format the DateTime object as "yyyy-MM-dd"
                string formattedDate = date.ToString("yyyy-MM-dd");

                result.Resultados[i].Date = Convert.ToDateTime(formattedDate);
            }

        }
    }
}
