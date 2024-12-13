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

                if (json.Count > 0)
                {
                    result.Pages = (int)Math.Ceiling(json.Count / 10.0);//esse resultado deverá ser a quantidade de paginas para o front pagina direto
                    foreach (var items in json)
                    {
                        Resultado item = new();
                        item.Text = "";
                        item.Title = $@"Página {items.Pagina} - Caderno {items.Descricao.Trim()}";
                        DateTime date = DateTime.Parse(items.DataPublicacao);
                        item.Link = $"https://www.jornalminasgerais.mg.gov.br/modulos/www.jornalminasgerais.mg.gov.br//diarioOficial/{date.Year}/{date.Month}/{date.Day}/jornal/{items.Titulo.Trim()}_{items.DataPublicacao.Split(" ")[0]}.pdf";
                        item.Date = date;
                        result.Resultados.Add(item);
                    }
                }
                result.Resultados = result.Resultados.OrderByDescending(x => x.Date).ToList();
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
