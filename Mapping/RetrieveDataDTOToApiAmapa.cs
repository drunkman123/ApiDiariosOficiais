using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.Amapa;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiAmapa
    {
        public static ApiAmapaRequestInicial ToApiAmapaRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiAmapaRequestInicial
            {
                Page = (retrieveDataRequest.Page == 0 ? 0 : retrieveDataRequest.Page - 1),
                SearchText = retrieveDataRequest.TextToSearch.Replace(" ","%20"),
                InitialDate = retrieveDataRequest.InitialDate.ToString("yyyy-MM-dd"),
                FinalDate = retrieveDataRequest.EndDate.ToString("yyyy-MM-dd")
            };
        }
    }
}
