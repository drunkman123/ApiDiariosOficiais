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
            var results = new Dictionary<string, object>();

            // Define region task mappings
            var regionTasks = new Dictionary<string, Func<Task>>
            //var regionTasks = new ConcurrentDictionary<string, Func<Task>>
    {
        { "Acre", async () => results["Acre"] = await GetAcreResponseAsync(request) },
        { "Alagoas", async () => results["Alagoas"] = await GetAlagoasResponseAsync(request) },
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
                Acre = results.TryGetValue("Acre", out var acreResponse)
                ? (ApiAcreResponse)acreResponse
                : new ApiAcreResponse { Success = true, Error = "Api não requisitada.", Resultados = new List<ResultadoAcre>() },

                Alagoas = results.TryGetValue("Alagoas", out var alagoasResponse)
                ? (ApiAlagoasResponse)alagoasResponse
                : new ApiAlagoasResponse { Success = true, Error = "Api não requisitada.", Resultados = new List<ResultadoAlagoas>() }
            };
        }
        private bool ShouldExecuteTask(RetrieveDataDTO request, string regionKey)
        {
            return typeof(RetrieveDataDTO).GetProperty($"Get{regionKey}")?.GetValue(request) as bool? ?? false;
        }
        private async Task<ApiAcreResponse> GetAcreResponseAsync(RetrieveDataDTO request)
        {
            try
            {
                var acreRequestInicial = request.ToApiAcreRequestInicialDomain();

                var acreResult = await _apiAcreService.GetAcreResponseAsync(acreRequestInicial);

                return acreResult;
            }
            catch (Exception ex)
            {
                return new ApiAcreResponse { Success = false, Error = ex.Message };
            }
        }

        private async Task<ApiAlagoasResponse> GetAlagoasResponseAsync(RetrieveDataDTO request)
        {

            try
            {
                var alagoasRequestInicial = request.ToApiAlagoasRequestInicialDomain();

                var alagoasResult = await _apiAlagoasService.GetAlagoasResponseAsync(alagoasRequestInicial);

                return alagoasResult;
            }
            catch (Exception ex)
            {
                return new ApiAlagoasResponse { Success = false, Error = ex.Message };
            }
        }

    }
}
