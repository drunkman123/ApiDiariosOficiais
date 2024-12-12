namespace ApiDiariosOficiais.Models.Responses
{
    public record RetrieveDataResponse
    {
        public DiarioResponse Acre { get; set; }
        public DiarioResponse Alagoas { get; set; }
        public DiarioResponse SaoPaulo { get; set; }
        public DiarioResponse RioDeJaneiro { get; set; }
        public DiarioResponse Amapa { get; set; }
        public DiarioResponse RioGrandeDoSul { get; set; }
        public DiarioResponse MinasGerais { get; set; }
    }
}
