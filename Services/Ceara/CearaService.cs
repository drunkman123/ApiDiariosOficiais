using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Ceara;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace ApiDiariosOficiais.Services.Ceara
{
    public class CearaService : ICearaService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public CearaService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var cearaRequestInicial = requestInicial.ToApiCearaRequestInicialDomain();

            var results = new List<string>();

            var cookie = string.Empty;

            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                cookie = await SubmitSearchAsync(cearaRequestInicial, results);
                var pages = ExtractPages(results[0]);

                //await SubmitNextPagesSearchAsync(cearaRequestInicial,results);


                if (results.Count < 2)
                {
                    var document = ParseHtml(results[0]);

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
            catch (Exception)
            {
                throw;
            }
            result.Success = true;

            return result;
        }

        private async Task<string> SubmitSearchAsync(ApiCearaRequestInicial requestInicial, List<string> resultados)
        {
            //var client = new HttpClient();
            client = _httpClientFactory.CreateClient("ApiCeara");

            string cookie = string.Empty;
            try
            {
                string dataIni = requestInicial.DataIni.Replace("/", "%2F");
                string dataFim = requestInicial.DataFim.Replace("/", "%2F");
                string pesqEx = requestInicial.PesqEx.Contains(' ') ? requestInicial.PesqEx : requestInicial.PesqEx + ' ';
                // Send the POST request
                HttpResponseMessage response = await client.GetAsync(($@"doepesquisa/sead.to?page=pesquisaTextual&action=PesquisarTextual&cmd=11&flag=1&dataini={dataIni}&datafim={dataFim}&numDiario=&numCaderno=&numPagina=&RadioGroup1=radio3&pesqEx={pesqEx}&consultar="));

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    resultados.Add(await response.Content.ReadAsStringAsync());
                    //cookie = response.Headers.GetValues("Set-Cookie").ToList()[0].Replace("; Path=/doepesquisa", "");

                }
                else
                {
                    // Handle failure
                    Console.WriteLine($"Error: {response.StatusCode}");
                }

            }
            catch (Exception)
            {
                throw;
            }
            return cookie;
        }
        /*private async Task SubmitNextPagesSearchAsync(ApiCearaRequestInicial requestInicial, List<string> resultados)
        {
            var client = _httpClientFactory.CreateClient("ApiCeara");

            try
            {
                string dataIni = requestInicial.DataIni.Replace("/", "%2F");
                string dataFim = requestInicial.DataFim.Replace("/", "%2F");
                string pesqEx = requestInicial.PesqEx.Contains(' ') ? requestInicial.PesqEx : requestInicial.PesqEx + ' ';

                // Send the POST request
                HttpResponseMessage response = await client.GetAsync($@"doepesquisa/sead.to?page=pesquisaTextual&action=PesquisarTextual&cmd=11&flag=1&dataini={dataIni}&datafim={dataFim}&numDiario=&numCaderno=&numPagina=&RadioGroup1=radio3&pesqEx={pesqEx}&consultar=");

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    resultados.Add(Sanitize(await response.Content.ReadAsStringAsync()));
                    resultados.Add(response.Headers.GetValues("Set-Cookie").ToList()[0].Replace("; Path=/doepesquisa", ""));

                }
                else
                {
                    // Handle failure
                    Console.WriteLine($"Error: {response.StatusCode}");
                }

            }
            catch (Exception)
            {
                throw;
            }
            return responseBody;

        }*/

        private string Sanitize(string response)
        {
            string sanitizedString = Regex.Replace(response, "<.*?>", string.Empty);

            // Step 2: Decode escape sequences (e.g., <\\/em> -> </em>)
            sanitizedString = sanitizedString.Replace("\\/", "/");

            // Step 3: Remove unwanted characters (like \n)
            sanitizedString = sanitizedString.Replace("\n", " ").Replace("\r", " ");

            // Step 4: Normalize spaces (trim and collapse multiple spaces)
            return Regex.Replace(sanitizedString, @"\s+", " ").Trim();
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

        private int ExtractPages(string firstResponse)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(firstResponse);
            var pages = 0;
            // Find the specific <div> containing the text
            var resultDiv = doc.DocumentNode.SelectSingleNode("//div[@align='center' and contains(text(), 'registros')]");

            if (resultDiv != null)
            {
                // Extract the text content
                string resultText = resultDiv.InnerText.Trim();
                pages = int.Parse(resultText);
            }
            else
            {
                Console.WriteLine("Div not found.");
            }
            return pages;
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
