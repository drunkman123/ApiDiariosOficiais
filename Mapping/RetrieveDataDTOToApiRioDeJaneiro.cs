using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.RioDeJaneiro;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiRioDeJaneiro
    {
        public static ApiRioDeJaneiroRequestInicial ToApiRioDeJaneiroRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiRioDeJaneiroRequestInicial
            {
                Page = (retrieveDataRequest.Page == 0 ? 0 : retrieveDataRequest.Page - 1),
                SearchText = retrieveDataRequest.TextToSearch.Replace(" ","%20"),
                InitialDate = retrieveDataRequest.InitialDate.ToString("yyyy-MM-dd"),
                FinalDate = retrieveDataRequest.EndDate.ToString("yyyy-MM-dd")
            };
        }
    }
}
