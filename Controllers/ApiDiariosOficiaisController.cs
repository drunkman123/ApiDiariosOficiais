using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ApiDiariosOficiais.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ApiDiariosOficiaisController : ControllerBase
    {
        private readonly IAcreService _apiAcreService;
        private readonly IAlagoasService _apiAlagoasService;
        //private readonly ILogRepository _logRepository;


        public ApiDiariosOficiaisController(IAcreService apiAcreService, IAlagoasService apiAlagoasService)
        {
            _apiAcreService = apiAcreService;
            _apiAlagoasService = apiAlagoasService;
        }

        [HttpPost]
        public async Task<RetrieveDataResponse> RetrieveData([FromBody] RetrieveDataDTO request)
        {
            var result = new ConcurrentDictionary<string, object>();

            var tasks = new List<Task>();

            // Task for GetDiarioAcre
            if (request.GetAcre)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var acreResponse = await GetAcreResponseAsync(request);
                    result["AcreResponse"] = acreResponse;
                }));
            }

            // Task for GetDiarioAlagoas
            if (request.GetAlagoas)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var alagoasResponse = await GetAlagoasResponseAsync(request);
                    result["AlagoasResponse"] = alagoasResponse;
                }));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            var finalResult = new RetrieveDataResponse();

            // Retrieve responses from the dictionary
            if (result.ContainsKey("AcreResponse"))
                finalResult.AcreResponse = (ApiAcreResponse)result["AcreResponse"];
            else
            {
                finalResult.AcreResponse = new ApiAcreResponse();
                finalResult.AcreResponse.Resultados = new List<ResultadoAcre>();
            }
                
            if (result.ContainsKey("AlagoasResponse"))
                finalResult.AlagoasResponse = (ApiAlagoasResponse)result["AlagoasResponse"];
            else
            {
                finalResult.AlagoasResponse = new ApiAlagoasResponse();
                finalResult.AlagoasResponse.Resultados = new List<ResultadoAlagoas>();
            }

            return finalResult;
        }

        private async Task<ApiAcreResponse> GetAcreResponseAsync(RetrieveDataDTO request)
        {
            var acreResponse = new ApiAcreResponse
            {
                Resultados = new List<ResultadoAcre>()
            };

            var acreRequestInicial = request.ToApiAcreRequestInicialDomain();
            var acreResult = await _apiAcreService.GetAcreResponseAsync(acreRequestInicial);

            if (acreResult != null)
            {
                acreResponse = acreResult;
            }

            return acreResponse;
        }

        private async Task<ApiAlagoasResponse> GetAlagoasResponseAsync(RetrieveDataDTO request)
        {
            var alagoasResponse = new ApiAlagoasResponse
            {
                Resultados = new List<ResultadoAlagoas>()
            };

            var alagoasRequestInicial = request.ToApiAlagoasRequestInicialDomain();
            var alagoasResult = await _apiAlagoasService.GetAlagoasResponseAsync(alagoasRequestInicial);

            if (alagoasResult.Resultados.Count > 0)
            {
                alagoasResponse = alagoasResult;
            }

            return alagoasResponse;
        }

    }
}
