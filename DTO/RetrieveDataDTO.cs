namespace ApiDiariosOficiais.DTO
{
    public record RetrieveDataDTO
    {
        public string TextToSearch { get; init; }
        public DateTime InitialDate { get; init; }
        public DateTime EndDate { get; init; }
        public bool GetAcre { get; init; }
        public int Page {  get; init; }
    }
}
