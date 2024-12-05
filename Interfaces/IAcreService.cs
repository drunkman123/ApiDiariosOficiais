
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Acre;

namespace ApiDiariosOficiais.Interfaces
{
    public interface IAcreService
    {
        Task<ApiAcreResponse> GetAcreResponseAsync(ApiAcreRequestInicial requestInicial);
    }
}
