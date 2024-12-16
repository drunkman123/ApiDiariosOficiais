namespace ApiDiariosOficiais.DTO
{
    public record RetrieveDataDTO
    {
        public string TextToSearch { get; init; }
        public DateTime InitialDate { get; init; }
        public DateTime EndDate { get; init; }
        public bool GetAcre { get; init; }
        public bool GetAlagoas { get; init; }
        public bool GetSaoPaulo { get; init; }
        public bool GetRioDeJaneiro { get; init; }
        public bool GetAmapa { get; init; }
        public bool GetRioGrandeDoSul { get; init; }
        public bool GetMinasGerais { get; init; }
        public bool GetMatoGrossoDoSul { get; init; }
        public bool GetCeara { get; init; }
        public bool InitialPage {  get; init; }
        public int Page {  get; init; }
    }
}
