using ApiDiariosOficiais.DTO;
using ApiDiariosOficiais.Models.Requests.MinasGerais;
using System.Globalization;

namespace ApiDiariosOficiais.Mappings
{
    public static class RetrieveDataDTOToApiMinasGerais
    {
        public static ApiMinasGeraisRequestInicial ToApiMinasGeraisRequestInicialDomain(this RetrieveDataDTO retrieveDataRequest)
        {
            return new ApiMinasGeraisRequestInicial
            {
                dataf = retrieveDataRequest.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                datai = retrieveDataRequest.InitialDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                itens_por_pagina = "10",//no momento é um parametro não usado
                pagina = "1", //no momento é um parametro não usado
                texto = retrieveDataRequest.TextToSearch
            };
        }
    }
}
