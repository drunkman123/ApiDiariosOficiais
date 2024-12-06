namespace ApiDiariosOficiais.Models
{
    public record ApiAlagoasResponse
    {
        public IList<ResultadoAlagoas> Resultados {  get; set; }
        public int Pages { get; set; }
    }
    public record ResultadoAlagoas
    {
        public string Link { get; set; }
        public string Text { get; set; }
    }
}
