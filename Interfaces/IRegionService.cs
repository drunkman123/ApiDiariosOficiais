using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models;

namespace ApiDiariosOficiais.Interfaces
{
    public interface IRegionService
    {
        Task<DiarioResponse> GetResponseAsync(RetrieveDataDTO request);
    }
}
