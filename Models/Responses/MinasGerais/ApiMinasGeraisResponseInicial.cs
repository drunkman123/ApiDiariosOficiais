namespace ApiDiariosOficiais.Models.Responses.MinasGerais
{ 
    public record ApiMinasGeraisResponseInicial
    {
        public string DataPublicacao { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Pagina { get; set; }
    }
}
