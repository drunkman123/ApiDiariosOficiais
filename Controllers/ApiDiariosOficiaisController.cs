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
            var results = new Dictionary<string, DiarioResponse>();
            // Define region task mappings
            var regionTasks = new Dictionary<string, Func<Task>>

            //var regionTasks = new ConcurrentDictionary<string, Func<Task>>
    {
        { "Acre", async () => results["Acre"] = await GetRegionResponseAsync<IAcreService>(request) },
        { "Alagoas", async () => results["Alagoas"] = await GetRegionResponseAsync<IAlagoasService>(request) },
        { "SaoPaulo", async () => results["SaoPaulo"] = await GetRegionResponseAsync<ISaoPauloService>(request) },
        { "RioDeJaneiro", async () => results["RioDeJaneiro"] = await GetRegionResponseAsync<IRioDeJaneiroService>(request) },
        { "Amapa", async () => results["Amapa"] = await GetRegionResponseAsync<IAmapaService>(request) },
        { "RioGrandeDoSul", async () => results["RioGrandeDoSul"] = await GetRegionResponseAsync<IRioGrandeDoSulService>(request) },
        { "MinasGerais", async () => results["MinasGerais"] = await GetRegionResponseAsync<IMinasGeraisService>(request) },
        { "MatoGrossoDoSul", async () => results["MatoGrossoDoSul"] = await GetRegionResponseAsync<IMatoGrossoDoSulService>(request) },
        // Add new regions here
    };

            // Select tasks based on the request
            var tasks = regionTasks
                .Where(rt => ShouldExecuteTask(request, rt.Key))
                .Select(rt => rt.Value.Invoke());

            // Execute all selected tasks
            await Task.WhenAll(tasks);

            // Prepare the final response dynamically
            return CreateDiarioResponse(results, "Acre", "Alagoas", "SaoPaulo","RioDeJaneiro","Amapa","RioGrandeDoSul","MinasGerais","MatoGrossoDoSul");

        }

        private static bool ShouldExecuteTask(RetrieveDataDTO request, string regionKey)
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
        private static RetrieveDataResponse CreateDiarioResponse(Dictionary<string, DiarioResponse> results, params string[] regionKeys)
        {
            var response = new RetrieveDataResponse();

            foreach (var regionKey in regionKeys)
            {
                DiarioResponse regionResponse = results.TryGetValue(regionKey, out var responseValue)
                    ? (DiarioResponse)responseValue
                    : new DiarioResponse { Success = true, Error = "Api não requisitada.", Resultados = new List<Resultado>() };

                switch (regionKey)
                {
                    case "Acre":
                        response.Acre = regionResponse;
                        break;
                    case "Alagoas":
                        response.Alagoas = regionResponse;
                        break;
                    case "SaoPaulo":
                        response.SaoPaulo = regionResponse;
                        break;
                    case "RioDeJaneiro":
                        response.RioDeJaneiro = regionResponse;
                        break; 
                    case "Amapa":
                        response.Amapa = regionResponse;
                        break; 
                    case "RioGrandeDoSul":
                        response.RioGrandeDoSul = regionResponse;
                        break;
                    case "MinasGerais":
                        response.MinasGerais = regionResponse;
                        break;
                    case "MatoGrossoDoSul":
                        response.MatoGrossoDoSul = regionResponse;
                        break;
                    // Add more cases for new regions here
                    default:
                        // Handle any unknown regions, if necessary
                        break;
                }
            }

            return response;
        }
    }
}
