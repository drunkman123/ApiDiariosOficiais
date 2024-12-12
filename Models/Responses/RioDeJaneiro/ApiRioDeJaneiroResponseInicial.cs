namespace ApiDiariosOficiais.Models.Responses.RioDeJaneiro
{
    public record Aggregations
    {
        public TipoEdicao TipoEdicao { get; set; }
        public Edicoes Edicoes { get; set; }
        public FileYear FileYear { get; set; }
    }

    public record Bucket
    {
        public int key { get; set; }
        public int doc_count { get; set; }
    }

    public record Edicoes
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }

    public record FileYear
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }

    public record Highlight
    {
        public List<string> conteudo { get; set; }
    }

    public record Hit
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public double _score { get; set; }
        public Source _source { get; set; }
        public Highlight highlight { get; set; }
        public List<object> sort { get; set; }
        public string diario { get; set; }
        public string suplemento { get; set; }
        public int total { get; set; }
        public double max_score { get; set; }
        public List<Hit> hits { get; set; }
    }

    public record ApiRioDeJaneiroResponseInicial
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public Shards _shards { get; set; }
        public Hit hits { get; set; }
        public Aggregations aggregations { get; set; }
        public bool loggedCredit { get; set; }
    }

    public record Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

    public record Source
    {
        public string conteudo { get; set; }
        public string data { get; set; }
        public string paginas { get; set; }
        public string pagina { get; set; }
        public string pdf_id { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string diario_id { get; set; }
        public string tipo_edicao { get; set; }
    }

    public record TipoEdicao
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }



}
