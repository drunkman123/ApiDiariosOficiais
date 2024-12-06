namespace ApiDiariosOficiais.Models.Responses.Alagoas
{
    public class Aggregations
    {
        public MaxDate? max_date { get; set; }
        public Year year { get; set; }
        public MinDate? min_date { get; set; }
        public EditionTypeName edition_type_name { get; set; }
        public Suplement suplement { get; set; }
    }

    public class Bucket
    {
        public dynamic key { get; set; }
        public int doc_count { get; set; }
        public string key_as_string { get; set; }
    }

    public class EditionTypeName
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }

    public class Item
    {
        public int edition_id { get; set; }
        public string publication_date { get; set; }
        public string path { get; set; }
        public int page_number { get; set; }
        public int id { get; set; }
        public int edition_number { get; set; }
        public string edition_type_name { get; set; }
        public bool is_suplement { get; set; }
        public List<string> highlight { get; set; }
    }

    public class MaxDate
    {
        public long? value { get; set; }
        public string value_as_string { get; set; }
    }

    public class MinDate
    {
        public long? value { get; set; }
        public string value_as_string { get; set; }
    }

    public class Result
    {
        public List<Item> items { get; set; }
        public Aggregations aggregations { get; set; }
        public TotalRows total_rows { get; set; }
    }

    public class ApiAlagoasResponseInicial
    {
        public string status { get; set; }
        public Result result { get; set; }
    }

    public class Suplement
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }

    public class TotalRows
    {
        public int value { get; set; }
        public string relation { get; set; }
    }

    public class Year
    {
        public int doc_count_error_upper_bound { get; set; }
        public int sum_other_doc_count { get; set; }
        public List<Bucket> buckets { get; set; }
    }

}
