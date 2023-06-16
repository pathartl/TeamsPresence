using Newtonsoft.Json;

namespace TeamsPresence
{
    public class HomeAssistantEntityStateUpdateAttributes
    {
        [JsonProperty(PropertyName = "friendly_name")]
        public string EntityFriendlyName { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }
    }
}
