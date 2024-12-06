using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Mappings;
using ApiDiariosOficiais.Models.Responses;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<RetrieveDataResponse> RetrieveData([FromBody]RetrieveDataDTO request)
        {
            var result = new RetrieveDataResponse();

            //GetDiarioAcre
            if (request.GetAcre)
            {
                var acreRequestInicial = request.ToApiAcreRequestInicialDomain();
                var acreResult = await _apiAcreService.GetAcreResponseAsync(acreRequestInicial);
                if (acreResult != null) 
                    result.AcreResponse = acreResult;
            }
            //GetDiarioAlagoas
            if (request.GetAlagoas)
            {
                var alagoasRequestInicial = request.ToApiAlagoasRequestInicialDomain();
                var alagoasResult = await _apiAlagoasService.GetAlagoasResponseAsync(alagoasRequestInicial);
                //if (alagoasResult != null)
                    //result.AlagoasResponse = alagoasResult;
            }

            return result;
        }
    }
}
