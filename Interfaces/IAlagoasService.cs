
using ApiDiariosOficiais.Models;
using ApiDiariosOficiais.Models.Requests.Alagoas;

namespace ApiDiariosOficiais.Interfaces
{
    public interface IAlagoasService
    {
        Task<ApiAlagoasResponse> GetAlagoasResponseAsync(ApiAlagoasRequestInicial requestInicial);
    }
}
