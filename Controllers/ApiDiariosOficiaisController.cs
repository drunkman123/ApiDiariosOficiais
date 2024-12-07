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
            var results = new ConcurrentDictionary<string, object>();

            // Define region task mappings
            var regionTasks = new Dictionary<string, Func<Task>>
    {
        { "Acre", async () => results["AcreResponse"] = await GetAcreResponseAsync(request) },
        { "Alagoas", async () => results["AlagoasResponse"] = await GetAlagoasResponseAsync(request) },
        // Add new regions here
    };

            // Select tasks based on the request
            var tasks = regionTasks
                .Where(rt => ShouldExecuteTask(request, rt.Key))
                .Select(rt => rt.Value.Invoke());

            // Execute all selected tasks
            await Task.WhenAll(tasks);

            // Prepare the final response dynamically
            return new RetrieveDataResponse
            {
                AcreResponse = results.TryGetValue("AcreResponse", out var acreResponse)
                    ? acreResponse as ApiAcreResponse ?? new ApiAcreResponse { Resultados = new List<ResultadoAcre>() }
                    : new ApiAcreResponse { Resultados = new List<ResultadoAcre>() },

                AlagoasResponse = results.TryGetValue("AlagoasResponse", out var alagoasResponse)
                    ? alagoasResponse as ApiAlagoasResponse ?? new ApiAlagoasResponse { Resultados = new List<ResultadoAlagoas>() }
                    : new ApiAlagoasResponse { Resultados = new List<ResultadoAlagoas>() },

                // Handle other regions dynamically if needed
            };
        }

        private bool ShouldExecuteTask(RetrieveDataDTO request, string regionKey)
        {
            return regionKey switch
            {
                "Acre" => request.GetAcre,
                "Alagoas" => request.GetAlagoas,
                // Add conditions for new regions here
                _ => false
            };
        }



        private async Task<ApiAcreResponse> GetAcreResponseAsync(RetrieveDataDTO request)
        {
            var acreRequestInicial = request.ToApiAcreRequestInicialDomain();
            try
            {
                var acreResult = await _apiAcreService.GetAcreResponseAsync(acreRequestInicial);
                return acreResult ?? new ApiAcreResponse { Resultados = new List<ResultadoAcre>() };
            }
            catch (Exception ex)
            {
                // Log the error
                //_logger.LogError(ex, "Error fetching Acre data");
                return new ApiAcreResponse { Resultados = new List<ResultadoAcre>() };
            }

        }

        private async Task<ApiAlagoasResponse> GetAlagoasResponseAsync(RetrieveDataDTO request)
        {

            var alagoasRequestInicial = request.ToApiAlagoasRequestInicialDomain();
            try
            {
                var alagoasResult = await _apiAlagoasService.GetAlagoasResponseAsync(alagoasRequestInicial);
                return alagoasResult ?? new ApiAlagoasResponse { Resultados = new List<ResultadoAlagoas>() };
            }
            catch (Exception ex)
            {
                // Log the error
                //_logger.LogError(ex, "Error fetching Acre data");
                return new ApiAlagoasResponse { Resultados = new List<ResultadoAlagoas>() };
            }
        }

    }
}
