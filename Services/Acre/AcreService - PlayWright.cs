//using ApiDiariosOficiais.Infrastructure.BrowserManager;
//using ApiDiariosOficiais.Interfaces;
//using ApiDiariosOficiais.Models;
//using ApiDiariosOficiais.Models.Requests.Acre;
//using HtmlAgilityPack;
//using Microsoft.Playwright;

//namespace ApiDiariosOficiais.Services
//{
//    public class AcreService : IAcreService
//    {
//        private const string url = "https://diario.ac.gov.br/";

//        private readonly BrowserManager _browserManager;

//        public AcreService(BrowserManager browserManager)
//        {
//            _browserManager = browserManager;
//        }

//        public async Task<ApiAcreResponse> GetAcreResponseAsync(ApiAcreRequestInicial requestInicial)
//        {
//            var result = new ApiAcreResponse
//            {
//                Resultados = new List<ResultadoAcre>()
//            };

//            try
//            {
//                var pageContent = string.Empty;
//                if(requestInicial.PaginaIni != "0")
//                {
//                    pageContent = await SubmitSearchFormNavigateAsync(requestInicial,requestInicial.PaginaIni);
//                }
//                else
//                {
//                    pageContent = await SubmitSearchFormAsync(requestInicial);
//                }

//                if (!string.IsNullOrEmpty(pageContent))
//                {
//                    var document = ParseHtml(pageContent);

//                    RemoveCalendarioDiv(document); //se nao remover a extração dos links buga

//                    ExtractLinks(document, result);
//                    ExtractTextFromTd(document, result);
//                    ExtractLastPageNumber(document, result);
//                }
//            }
//            catch (Exception)
//            {
//                Console.WriteLine($"An error occurred: {ex.Message}");
//            }

//            return result;
//        }

//        private async Task<string> SubmitSearchFormAsync(ApiAcreRequestInicial requestInicial)
//        {
//            string responseBody;

//            var browser = await _browserManager.GetBrowserAsync();
//            var context = await browser.NewContextAsync();
//            try
//            {
//                var page = await context.NewPageAsync();
//                await page.GotoAsync(url);
//                await page.WaitForSelectorAsync("form[name='buscaPorPalavra']");

//                // Fill form fields
//                await page.EvaluateAsync($"document.querySelector('input[name=\"paginaIni\"]').value = '{requestInicial.PaginaIni}';");
//                await page.FillAsync("input[name='palavra']", requestInicial.Palavra);
//                await page.SelectOptionAsync("select[name='ano_palavra']", new SelectOptionValue { Value = requestInicial.AnoPalavra });

//                // Submit the form
//                await page.ClickAsync("form[name='buscaPorPalavra'] button[type='submit']");
//                await page.WaitForURLAsync(url);

//                responseBody = await page.ContentAsync();
//            }
//            finally
//            {
//                await context.CloseAsync();
//            }

//            return responseBody;
//        }
//        private async Task<string> SubmitSearchFormNavigateAsync(ApiAcreRequestInicial requestInicial, string requestedPage)
//        {
//            string responseBody;

//            var browser = await _browserManager.GetBrowserAsync();
//            var context = await browser.NewContextAsync();
//            try
//            {

//                var page = await context.NewPageAsync();
//                await page.GotoAsync(url);
//                await page.WaitForSelectorAsync("form[name='buscaPorPalavra']");

//                // Fill form fields
//                await page.EvaluateAsync($"document.querySelector('input[name=\"paginaIni\"]').value = '0';");
//                await page.FillAsync("input[name='palavra']", requestInicial.Palavra);
//                await page.SelectOptionAsync("select[name='ano_palavra']", new SelectOptionValue { Value = requestInicial.AnoPalavra });

//                // Submit the form
//                await page.ClickAsync("form[name='buscaPorPalavra'] button[type='submit']");
//                await page.WaitForURLAsync(url);
//                responseBody = await page.ContentAsync();

//                //await page.WaitForSelectorAsync("form[name='buscaPorPalavra']");

//                // Fill form fields
//                await page.EvaluateAsync($"document.querySelector('input[name=\"paginaIni\"]').value = '{requestedPage}';");
//                await page.FillAsync("input[name='palavra']", requestInicial.Palavra);
//                await page.SelectOptionAsync("select[name='ano_palavra']", new SelectOptionValue { Value = requestInicial.AnoPalavra });

//                // Submit the form
//                await page.ClickAsync("form[name='buscaPorPalavra'] button[type='submit']");
//                await page.WaitForURLAsync(url);

//                responseBody = await page.ContentAsync();
//            }
//            finally
//            {
//                await context.CloseAsync();
//            }

//            return responseBody;
//        }

//        private HtmlDocument ParseHtml(string htmlContent)
//        {
//            var document = new HtmlDocument();
//            document.LoadHtml(htmlContent);
//            return document;
//        }

//        private void RemoveCalendarioDiv(HtmlDocument document)
//        {
//            var calendarioDiv = document.DocumentNode.SelectSingleNode("//div[@id='calendario']");
//            calendarioDiv?.Remove();
//        }

//        private void ExtractLinks(HtmlDocument document, ApiAcreResponse result)
//        {
//            var links = document.DocumentNode.SelectNodes("//tbody//a");

//            if (links != null)
//            {
//                foreach (var link in links)
//                {
//                    result.Resultados.Add(new ResultadoAcre
//                    {
//                        Link = link.GetAttributeValue("href", string.Empty)
//                    });
//                }
//            }
//            else
//            {
//                Console.WriteLine("No <a> tags found inside <tbody>.");
//            }
//        }

//        private void ExtractTextFromTd(HtmlDocument document, ApiAcreResponse result)
//        {
//            var tdNodes = document.DocumentNode.SelectNodes("//td[@colspan='3']");

//            if (tdNodes != null && tdNodes.Count > 0)
//            {
//                for (int i = 0; i < result.Resultados.Count; i++)
//                {
//                    result.Resultados[i].Text = tdNodes[i].InnerText.Replace("\n", " ").Trim();
//                }
//            }
//            else
//            {
//                Console.WriteLine("No <td colspan='3'> elements found.");
//            }
//        }

//        private void ExtractLastPageNumber(HtmlDocument document, ApiAcreResponse result)
//        {
//            var lastPageNode = document.DocumentNode.SelectNodes("//span[contains(@onclick, 'vaiParaPaginaBusca')]")?.LastOrDefault();

//            if (lastPageNode != null)
//            {
//                var onclickValue = lastPageNode.GetAttributeValue("onclick", string.Empty);
//                var startIndex = onclickValue.IndexOf("(") + 1;
//                var endIndex = onclickValue.IndexOf(")");
//                var pageValue = onclickValue.Substring(startIndex, endIndex - startIndex);

//                result.Pages = Convert.ToInt32(pageValue);
//            }
//            else
//            {
//                Console.WriteLine("No 'vaiParaPaginaBusca' found.");
//            }
//        }
//    }
//}
