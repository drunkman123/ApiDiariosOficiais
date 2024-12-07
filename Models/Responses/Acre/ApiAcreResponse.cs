namespace ApiDiariosOficiais.Models
{
    public record ApiAcreResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public IList<ResultadoAcre> Resultados {  get; set; }
        public int Pages { get; set; }
    }
    public record ResultadoAcre
    {
        public string Link { get; set; }
        public string Text { get; set; }
    }
}
