using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.SaoPaulo;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiSaoPaulo
    {
        public static ApiSaoPauloRequestInicial ToApiSaoPauloRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiSaoPauloRequestInicial
            {
                PageNumber = retrieveDataRequest.InitialPage ? 1 : retrieveDataRequest.Page,
                Terms = retrieveDataRequest.TextToSearch,
                FromDate = retrieveDataRequest.InitialDate.ToString("yyyy-MM-dd"),
                ToDate = retrieveDataRequest.EndDate.ToString("yyyy-MM-dd")
            };
        }
    }
}
