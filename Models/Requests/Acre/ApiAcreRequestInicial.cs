namespace ApiDiariosOficiais.Models.Requests.Acre
{
    public record ApiAcreRequestInicial
    {
        public string PaginaIni { get; init; } = "0";
        public string PalavraTipo { get; init; } = string.Empty;
        public string AnoPalavra { get; init; }
        public string Palavra { get; init; }
    }
}
