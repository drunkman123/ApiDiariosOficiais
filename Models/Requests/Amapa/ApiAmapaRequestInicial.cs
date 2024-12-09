namespace ApiDiariosOficiais.Models.Requests.Amapa
{
    public record ApiAmapaRequestInicial
    {
        public string FinalDate { get; init; }
        public string InitialDate { get; init; }
        public string SearchText { get; init; }
        public int Page { get; set; }
    }
}
