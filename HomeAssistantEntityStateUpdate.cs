using Newtonsoft.Json;

namespace TeamsPresence
{
    public class HomeAssistantEntityStateUpdate
    {
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "attributes")]
        public HomeAssistantEntityStateUpdateAttributes Attributes { get; set; }
    }
}
