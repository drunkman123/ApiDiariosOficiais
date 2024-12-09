namespace ApiDiariosOficiais.Models.Requests.SaoPaulo
{
    public record ApiSaoPauloRequestInicial
    {
        public int PageNumber { get; set; }
        public string Terms { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
