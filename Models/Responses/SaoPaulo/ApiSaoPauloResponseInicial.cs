namespace ApiDiariosOficiais.Models.Responses.SaoPaulo
{
    public record ApiSaoPauloResponseInicial
    {
        public List<Item> items { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public int totalItems { get; set; }
        public int pageSize { get; set; }
        public bool hasPreviousPage { get; set; }
        public bool hasNextPage { get; set; }
        public record Item
        {
            public bool isLegacy { get; set; }
            public string id { get; set; }
            public string publicationTypeId { get; set; }
            public string secondLevelSectionId { get; set; }
            public string thirdLevelSectionId { get; set; }
            public DateTime date { get; set; }
            public string title { get; set; }
            public string slug { get; set; }
            public string excerpt { get; set; }
            public string hierarchy { get; set; }
            public int totalTermsFound { get; set; }
            public List<TermsFound> termsFound { get; set; }
        }

        public record TermsFound
        {
            public string term { get; set; }
            public int matchesFound { get; set; }
        }


    }
}
