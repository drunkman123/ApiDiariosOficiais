namespace ApiDiariosOficiais.Models.Responses.MatoGrossoDoSul
{
    public record ApiMatoGrossoDoSulResponseInicial
    {
        public int dataBD { get; set; }
        public List<DataElastic> dataElastic { get; set; }
        public int from { get; set; }
        public int totalDataElastic { get; set; }
    }
    public class DataElastic
    {
        public Fields Fields { get; set; }
        public Source Source { get; set; }
        public string Index { get; set; }
        public object InnerHits { get; set; }
        public int Score { get; set; }
        public string Type { get; set; }
        public object Version { get; set; }
        public string Id { get; set; }
        public List<object> Sorts { get; set; }
        public Highlights Highlights { get; set; }
        public object Explanation { get; set; }
        public object MatchedQueries { get; set; }
    }

    public class Fields
    {
    }

    public class Highlights
    {
        public Texto texto { get; set; }
    }



    public class Source
    {
        public int DocumentoID { get; set; }
        public object Id { get; set; }
        public object DocumentoPaiID { get; set; }
        public object DocumentoPai { get; set; }
        public int IndiceID { get; set; }
        public string Numero { get; set; }
        public string Descricao { get; set; }
        public string NomeArquivo { get; set; }
        public object CaminhoArquivo { get; set; }
        public string DataInicioPublicacaoArquivo { get; set; }
        public string DataEnvioArquivo { get; set; }
        public string DataCriacaoArquivo { get; set; }
        public bool FoiIndexado { get; set; }
        public object DataExclusao { get; set; }
        public object UsuarioInclusaoId { get; set; }
        public object UsuarioExclusaoId { get; set; }
        public object TamanhoArquivo { get; set; }
        public string Texto { get; set; }
        public int Pagina { get; set; }
        public object Indice { get; set; }
        public object Suplementos { get; set; }
        public object NumeroSuplemento { get; set; }
        public object EdicaoExtra { get; set; }
        public object DocumentoAlteradoID { get; set; }
        public object DocumentoAlterado { get; set; }
    }

    public class Texto
    {
        public string DocumentId { get; set; }
        public string Field { get; set; }
        public List<string> Highlights { get; set; }
    }


}