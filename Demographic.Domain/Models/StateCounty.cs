using Newtonsoft.Json;

namespace Demographic.Domain.Models
{
    public class StateCounty
    {
        [JsonProperty("POPULATION")]
        public int? Population { get; set; }
        [JsonProperty("STATE_NAME")]
        public string StateName { get; set; } = string.Empty;
    }
}
