namespace ApiDiariosOficiais.Models.Responses
{
    public record RetrieveDataResponse
    {
        public DiarioResponse Acre { get; set; }
        public DiarioResponse Alagoas { get; set; }
        public DiarioResponse SaoPaulo { get; set; }
    }
}
