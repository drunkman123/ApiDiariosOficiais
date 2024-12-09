namespace ApiDiariosOficiais.Models.Requests.RioDeJaneiro
{
    public record ApiRioDeJaneiroRequestInicial
    {
        public string FinalDate { get; init; }
        public string InitialDate { get; init; }
        public string SearchText { get; init; }
        public int Page { get; set; }
    }
}
