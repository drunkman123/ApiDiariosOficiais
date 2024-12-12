namespace ApiDiariosOficiais.Models.Requests.MinasGerais
{
    public record ApiMinasGeraisRequestInicial
    {
        public string datai { get; set; }
        public string dataf { get; set; }
        public string texto { get; set; }
        public string pagina { get; set; }
        public string itens_por_pagina { get; set; }
    }
}
