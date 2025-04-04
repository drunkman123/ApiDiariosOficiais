﻿namespace ApiDiariosOficiais.Models
{
    public record DiarioResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public IList<Resultado> Resultados {  get; set; }
        public int Pages { get; set; }
    }
    public record Resultado
    {
        public DateTime Date { get; set; }
        public string? Title { get; set; }
        public string Link { get; set; }
        public string Text { get; set; }
    }
}
