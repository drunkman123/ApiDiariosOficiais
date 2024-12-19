using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Ceara;
using HtmlAgilityPack;
using Polly.Timeout;
using Polly;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ApiDiariosOficiais.Services.Ceara
{
    public class CearaService : ICearaService
    {

        //private readonly IHttpClientFactory _httpClientFactory;

        public CearaService(/*IHttpClientFactory httpClientFactory*/)
        {
            //_httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var cearaRequestInicial = requestInicial.ToApiCearaRequestInicialDomain();

            var responseRequests = new List<string>();

            var cookie = string.Empty;

            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                var policy = Policy
    .Handle<TimeoutRejectedException>() // Handle Timeout exception
    .Or<TaskCanceledException>() // Also handle TaskCanceledException due to timeout
    .RetryAsync(2);
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    cookie = await policy.ExecuteAsync(() => SubmitSearchAsync(cearaRequestInicial, responseRequests, client));
                    if (responseRequests[0].Contains("Registros não encontrados!"))
                    {
                        result.Success = true;
                        return result;
                    }
                    var pages = ExtractPages(responseRequests[0]);
                    for (var i = 1; i < pages; i++)
                    {
                        await SubmitNextPagesSearchAsync(responseRequests, client, cookie);
                    }
                }
                Dictionary<string, DateTime> teste = new Dictionary<string, DateTime>();
                foreach (var item in responseRequests)
                {
                    var document = ParseHtml(item);
                    ExtractDatesAndLinks(document, teste);
                }
                foreach(var item in teste)
                {
                    result.Resultados.Add(new Resultado { Date = item.Value,Link = item.Key });
                    
                }
                result.Resultados = result.Resultados.OrderByDescending(x => x.Date).ToList();


            }
            catch (Exception)
            {
                throw;
            }
            result.Success = true;

            return result;
        }

        private async Task<string> SubmitSearchAsync(ApiCearaRequestInicial requestInicial, List<string> resultados, HttpClient client)
        {
            string cookie = string.Empty;


            try
            {
                string dataIni = requestInicial.DataIni.Replace("/", "%2F");
                string dataFim = requestInicial.DataFim.Replace("/", "%2F");
                string pesqEx = requestInicial.PesqEx.Contains(' ') ? requestInicial.PesqEx : requestInicial.PesqEx + ' ';
                // Send the POST request
                HttpResponseMessage response = await client.GetAsync(($@"http://pesquisa.doe.seplag.ce.gov.br/doepesquisa/sead.to?page=pesquisaTextual&action=PesquisarTextual&cmd=11&flag=1&dataini={dataIni}&datafim={dataFim}&numDiario=&numCaderno=&numPagina=&RadioGroup1=radio3&pesqEx={pesqEx}&consultar="));

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    resultados.Add(await response.Content.ReadAsStringAsync());
                    cookie = response.Headers.GetValues("Set-Cookie").ToList()[0].Replace("; Path=/doepesquisa", "");

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
        private async Task SubmitNextPagesSearchAsync(List<string> resultados, HttpClient client, string cookie)
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                /*client.DefaultRequestHeaders.Add("Connection", "close");*/
                // Send the POST request
                HttpResponseMessage response = await client.GetAsync($@"http://pesquisa.doe.seplag.ce.gov.br/doepesquisa/sead.do?page=pesquisaTextual&cmd=proximo&action=NavegarBasico&flag=1");

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    resultados.Add(await response.Content.ReadAsStringAsync());
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
        }
        private HtmlDocument ParseHtml(string htmlContent)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            return document;
        }

        private void ExtractDatesAndLinks(HtmlDocument document, Dictionary<string, DateTime> teste)
        {
            var nodes = document.DocumentNode.SelectNodes("//td[@bgcolor='#FFFFFF' and not(contains(., 'Último Diário publicado')) and not(.//img[contains(@src, 'images/Duvida.jpg')])]//a");
            /*var nodes = document.DocumentNode.SelectNodes(
    "//td[@bgcolor='#FFFFFF' and not(contains(., 'Último Diário publicado')) and not(.//img[contains(@src, 'images/Duvida.jpg')])]//a");*/

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    // Extract the date text
                    string dateText = node.InnerText.Trim();
                    string link = ExtractUrl(node.OuterHtml.Trim());
                    teste.Add(link, DateTime.ParseExact(dateText, "dd-MM-yyyy", CultureInfo.InvariantCulture));
                }
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
                pages = (int)Math.Ceiling(int.Parse(resultText.Split(' ')[0]) / 8.0);
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
        private string ExtractUrl(string input)
        {
            // Regular expression to extract the href URL
            var match = Regex.Match(input, @"<a\s+href\s*=\s*[""'](https?://.*?)(?=[""'])", RegexOptions.IgnoreCase);

            // Return the URL if a match is found, otherwise return an empty string
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
