using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Alagoas;
using ApiDiariosOficiais.Models.Responses.Alagoas;
using HtmlAgilityPack;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiDiariosOficiais.Services
{
    public class AlagoasService : IAlagoasService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public AlagoasService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiAlagoasResponse> GetAlagoasResponseAsync(ApiAlagoasRequestInicial requestInicial)
        {
            var result = new ApiAlagoasResponse
            {
                Resultados = new List<ResultadoAlagoas>()
            };

            try
            {
                ApiAlagoasResponseInicial json = await GetDataAsync(requestInicial);

                // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

                //if (!string.IsNullOrEmpty(pageContent))
                //{
                //    var document = ParseHtml(pageContent);

                //    //RemoveCalendarioDiv(document); //se nao remover a extração dos links buga

                //    ExtractLinks(document, result);
                //    ExtractTextFromTd(document, result);
                //    ExtractLastPageNumber(document, result);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return result;
        }

        private async Task<ApiAlagoasResponseInicial> GetDataAsync(ApiAlagoasRequestInicial requestInicial)
        {
            ApiAlagoasResponseInicial responseObject = new ApiAlagoasResponseInicial();
            var httpClient = _httpClientFactory.CreateClient("ApiAlagoas");
            var jsonPayload = JsonSerializer.Serialize(requestInicial);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {
                var response = await httpClient.PostAsync("apinova/api/editions/searchES?page=1", content);
                response.EnsureSuccessStatusCode();

                // Send the POST request
                var responseString = await response.Content.ReadAsStringAsync();
                responseObject = JsonSerializer.Deserialize<ApiAlagoasResponseInicial>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex) { }
            return responseObject;

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

        private void ExtractLinks(HtmlDocument document, ApiAcreResponse result)
        {
            var links = document.DocumentNode.SelectNodes("//tbody//a");

            if (links != null)
            {
                foreach (var link in links)
                {
                    result.Resultados.Add(new ResultadoAcre
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

        private void ExtractTextFromTd(HtmlDocument document, ApiAcreResponse result)
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

        private void ExtractLastPageNumber(HtmlDocument document, ApiAcreResponse result)
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
