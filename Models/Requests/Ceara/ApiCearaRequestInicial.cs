namespace ApiDiariosOficiais.Models.Requests.Ceara
{
    public record ApiCearaRequestInicial
    {
        public string Page { get; set; } = "pesquisaTextual";
        public string Action { get; set; } = "PesquisarTextual";
        public int Cmd { get; set; } = 11;
        public int Flag { get; set; } = 1;
        public string DataIni { get; set; }
        public string DataFim { get; set; }
        public string NumDiario { get; set; } = string.Empty;
        public string NumCaderno { get; set; } = string.Empty;
        public string NumPagina { get; set; } = string.Empty;
        public string RadioGroup1 { get; set; } = "radio3";
        public string PesqEx { get; set; }
        public string Consultar { get; set; } = string.Empty;
    }
}
