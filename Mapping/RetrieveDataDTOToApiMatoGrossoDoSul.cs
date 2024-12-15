using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.MatoGrossoDoSul;
using System.Globalization;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiMatoGrossoDoSul
    {
        public static ApiMatoGrossoDoSulRequestInicial ToApiMatoGrossoDoSulRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiMatoGrossoDoSulRequestInicial
            {
                DataFinal = retrieveDataRequest.EndDate.ToString("dd/MM/yyy", CultureInfo.InvariantCulture),
                DataInicial = retrieveDataRequest.InitialDate.ToString("dd/MM/yyy", CultureInfo.InvariantCulture),
                Numero = "",
                Texto = retrieveDataRequest.TextToSearch,
                TipoBuscaEnum = "1",
                Page = retrieveDataRequest.Page == 0 ? 1 : retrieveDataRequest.Page
            };
        }
    }
}
