using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.Alagoas;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiAlagoas
    {
        public static ApiAlagoasRequestInicial ToApiAlagoasRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiAlagoasRequestInicial
            {
                Page = retrieveDataRequest.InitialPage ? 1 : retrieveDataRequest.Page,
                keywords = retrieveDataRequest.TextToSearch,
                range = [retrieveDataRequest.InitialDate.ToString("yyyy-MM-dd"), retrieveDataRequest.EndDate.ToString("yyyy-MM-dd")],
            };
        }
    }
}
