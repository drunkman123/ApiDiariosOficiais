namespace ApiDiariosOficiais.Models.Requests.MatoGrossoDoSul
{
    public record ApiMatoGrossoDoSulRequestInicial
    {
        public string Numero { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public string Texto { get; set; }
        public string TipoBuscaEnum { get; set; }
        public int Page { get; set; }
    }
}
