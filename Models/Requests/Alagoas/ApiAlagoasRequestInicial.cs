namespace ApiDiariosOficiais.Models.Requests.Alagoas
{
    public record ApiAlagoasRequestInicial
    {
        public string edition_number { get; set; } = string.Empty;
        public string keywords { get; set; }
        public string order { get; set; } = "novo";
        public List<string> range { get; set; }
        public string searchType { get; set; } = "frase_exata";
        public int Page { get; set; }
    }
}
