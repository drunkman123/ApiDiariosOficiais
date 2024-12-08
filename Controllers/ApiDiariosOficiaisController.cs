using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Factory;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ApiDiariosOficiais.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ApiDiariosOficiaisController : ControllerBase
    {
        private readonly GenericServiceFactory _serviceFactory;

        //private readonly ILogRepository _logRepository;

        public ApiDiariosOficiaisController(GenericServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpPost]
        public async Task<RetrieveDataResponse> RetrieveData([FromBody] RetrieveDataDTO request)
        {
            var results = new Dictionary<string, object>();
            // Define region task mappings
            var regionTasks = new Dictionary<string, Func<Task>>

            //var regionTasks = new ConcurrentDictionary<string, Func<Task>>
    {
        { "Acre", async () => results["Acre"] = await GetRegionResponseAsync<IAcreService>(request) },
        { "Alagoas", async () => results["Alagoas"] = await GetRegionResponseAsync<IAlagoasService>(request) },
        { "São Paulo", async () => results["São Paulo"] = await GetRegionResponseAsync<ISaoPauloService>(request) },
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
                ? (DiarioResponse)acreResponse
                : new DiarioResponse { Success = true, Error = "Api não requisitada.", Resultados = new List<Resultado>() },

                Alagoas = results.TryGetValue("Alagoas", out var alagoasResponse)
                ? (DiarioResponse)alagoasResponse
                : new DiarioResponse { Success = true, Error = "Api não requisitada.", Resultados = new List<Resultado>() }
            };
        }
        private bool ShouldExecuteTask(RetrieveDataDTO request, string regionKey)
        {
            return typeof(RetrieveDataDTO).GetProperty($"Get{regionKey}")?.GetValue(request) as bool? ?? false;
        }

        private async Task<DiarioResponse> GetRegionResponseAsync<TService>(RetrieveDataDTO request)
            where TService : IRegionService
        {
            try
            {
                var regionService = _serviceFactory.Create<TService>();
                var regionResult = await regionService.GetResponseAsync(request);
                return regionResult;
            }
            catch (Exception ex)
            {
                return new DiarioResponse { Success = false, Error = ex.Message };
            }
        }

    }
}
