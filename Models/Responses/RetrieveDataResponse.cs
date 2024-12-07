namespace ApiDiariosOficiais.Models.Responses
{
    public record RetrieveDataResponse
    {
        public ApiAcreResponse Acre { get; set; }
        public ApiAlagoasResponse Alagoas { get; set; }
    }
}
