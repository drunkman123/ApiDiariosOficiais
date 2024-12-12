using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.MinasGerais;
using ApiDiariosOficiais.Models.Responses.MinasGerais;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiDiariosOficiais.Services.MinasGerais
{
    public class MinasGeraisService : IMinasGeraisService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public MinasGeraisService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO requestInicial)
        {
            var minasGeraisRequestInicial = requestInicial.ToApiMinasGeraisRequestInicialDomain();

            var result = new DiarioResponse
            {
                Resultados = new List<Resultado>()
            };

            try
            {
                List<ApiMinasGeraisResponseInicial> json = await GetDataAsync(minasGeraisRequestInicial);

                //if (json.hits != null && json.hits.hits.Count > 0)
                //{
                //    result.Pages = (int)Math.Ceiling(json.hits.total / 10.0);//dividir o numero de resultados por 10 pois é o numero de resultados por pagina
                //    foreach (var items in json.hits.hits)
                //    {
                //        Resultado item = new();
                //        item.Text += "...";
                //        foreach (var highlights in items.highlight.conteudo)
                //        {
                //            var sanitizedHighlights = SanitizeHighlights(highlights);
                //            item.Text += sanitizedHighlights + "...";
                //        }
                //        item.Link = $"https://doweb.rio.rj.gov.br/cleanpdf/?file=/apifront/portal/edicoes/pdf_diario/{items._source.diario_id}/{items._source.pagina}&find={MinasGeraisRequestInicial.SearchText}";

                //        DateTime date = DateTime.Parse(items._source.data);

                //        item.Date = date;

                //        result.Resultados.Add(item);

                //    }

                //}
            }
            catch (Exception ex)
            {
                throw;
            }
            result.Success = true;

            return result;
        }

        private async Task<List<ApiMinasGeraisResponseInicial>> GetDataAsync(ApiMinasGeraisRequestInicial requestInicial)
        {
            List<ApiMinasGeraisResponseInicial> responseObject = new();

            var httpClient = _httpClientFactory.CreateClient("ApiMinasGerais");

            var formData = new Dictionary<string, string>
            {
                { "dataf", requestInicial.dataf },
                { "datai", requestInicial.datai },
                { "itens_por_pagina", requestInicial.itens_por_pagina },
                { "pagina", requestInicial.pagina },
                { "texto", requestInicial.texto }
            };
            var content = new FormUrlEncodedContent(formData);

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

            try
            {
                var response = await httpClient.PostAsync($"Home/dashBoard/busca", content);

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                responseObject = JsonSerializer.Deserialize<List<ApiMinasGeraisResponseInicial>>(responseString.Replace(";", ""), new JsonSerializerOptions
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
