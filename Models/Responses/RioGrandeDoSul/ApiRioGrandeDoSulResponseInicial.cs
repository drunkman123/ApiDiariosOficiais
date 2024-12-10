using System.Globalization;

namespace ApiDiariosOficiais.Models.Responses.RioGrandeDoSul
{

    public class Collection
    {
        public string origem { get; set; }
        public object corag { get; set; }
        public Procergs procergs { get; set; }
    }

    public class Procergs
    {
        private string _data;

        public int id { get; set; }
        public string tipo { get; set; }
        public string data
        {
            get => _data; // Return the stored value
            set
            {
                // Parse and reformat the value if it's a valid date
                if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    _data = parsedDate.ToString("yyyy-MM-dd");
                }
                else
                {
                    _data = value; // Keep the value as-is if parsing fails
                }
            }
        }
        public string conteudo { get; set; }
    }

    public class ApiRioGrandeDoSulResponseInicial
    {
        public List<Collection> collection { get; set; }
        public int collectionSize { get; set; }
        public int pageSize { get; set; }
    }
}