using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.Acre;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiAcre
    {
        public static ApiAcreRequestInicial ToApiAcreRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiAcreRequestInicial
            {
                AnoPalavra = retrieveDataRequest.InitialDate.Year.ToString(),
                Palavra = retrieveDataRequest.TextToSearch,
                PaginaIni = retrieveDataRequest.Page.ToString()
            };
        }
    }
}
