using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace TeamsPresence
{
    public class HomeAssistantService
    {
        private RestClient Client { get; set; }

        public HomeAssistantService(string url, string token)
        {
            Client = new RestClient(url);

            Client.AddDefaultHeader("Authorization", $"Bearer {token}");
            Client.UseNewtonsoftJson();
        }

        public void UpdateEntity(string entity, string entityFriendlyName, string state, string icon)
        {
            var update = new HomeAssistantEntityStateUpdate()
            {
                State = state,
                Attributes = new HomeAssistantEntityStateUpdateAttributes()
                {
                    EntityFriendlyName = entityFriendlyName,
                    Icon = icon
                }
            };

            var request = new RestRequest($"api/states/{entity}", Method.POST, DataFormat.Json);

            request.AddJsonBody(update);

            Client.Execute(request);
        }
    }
}
