using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.RioGrandeDoSul;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiRioGrandeDoSul
    {
        public static ApiRioGrandeDoSulRequestInicial ToApiRioGrandeDoSulRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiRioGrandeDoSulRequestInicial
            {
                Page = retrieveDataRequest.InitialPage ? 1 : retrieveDataRequest.Page,
                SearchText = retrieveDataRequest.TextToSearch.Replace(" ","%20"),
                InitialDate = retrieveDataRequest.InitialDate.ToString("yyyy-MM-dd"),
                FinalDate = retrieveDataRequest.EndDate.ToString("yyyy-MM-dd")
            };
        }
    }
}
