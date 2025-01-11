using Newtonsoft.Json;

namespace Demographic.Domain.Models
{
    public class StateFeature
    {
        [JsonProperty("attributes")]
        public StateCounty? StateCounty { get; set; }
    }
}
