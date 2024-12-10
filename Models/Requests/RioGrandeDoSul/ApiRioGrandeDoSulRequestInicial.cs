namespace ApiDiariosOficiais.Models.Requests.RioGrandeDoSul
{
    public record ApiRioGrandeDoSulRequestInicial
    {
        public string FinalDate { get; init; }
        public string InitialDate { get; init; }
        public string SearchText { get; init; }
        public int Page { get; set; }
    }
}
