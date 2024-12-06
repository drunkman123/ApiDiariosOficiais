namespace ApiDiariosOficiais.Models.Responses
{
    public record RetrieveDataResponse
    {
        public ApiAcreResponse AcreResponse { get; set; }
        public ApiAlagoasResponse AlagoasResponse { get; set; }
    }
}
