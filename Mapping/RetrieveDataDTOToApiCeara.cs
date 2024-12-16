using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.Ceara;
using System.Globalization;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiCeara
    {
        public static ApiCearaRequestInicial ToApiCearaRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiCearaRequestInicial
            {
                DataFim = retrieveDataRequest.EndDate.ToString("dd/MM/yyy", CultureInfo.InvariantCulture),
                DataIni = retrieveDataRequest.InitialDate.ToString("dd/MM/yyy", CultureInfo.InvariantCulture),
                PesqEx = retrieveDataRequest.TextToSearch
            };
        }
    }
}
