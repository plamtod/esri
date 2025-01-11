using Newtonsoft.Json;

namespace Demographic.Domain.Models
{
    public class GeoResponse
    {
        [JsonProperty("exceededTransferLimit")]
        public bool ExceededTransferLimit { get; set; }

        [JsonProperty("features")]
        public List<StateFeature> StateCounties { get; set; } = [];
    }
}
