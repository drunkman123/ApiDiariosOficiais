namespace ApiDiariosOficiais.Models
{
    public record ApiCearaResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public IList<ResultadoCeara> Resultados {  get; set; }
        public int Pages { get; set; }
    }
    public record ResultadoCeara
    {
        public string Link { get; set; }
        public string Text { get; set; }
    }
}
