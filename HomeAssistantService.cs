using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace TeamsPresence
{
    public class HomeAssistantService
    {
        private string Token { get; set; }
        private string Url { get; set; }
        private RestClient Client { get; set; }

        public HomeAssistantService(string url, string token)
        {
            Token = token;
            Url = url;
            Client = new RestClient(url);

            Client.AddDefaultHeader("Authorization", $"Bearer {Token}");
            Client.UseNewtonsoftJson();
        }

        public void UpdateEntity(string entity, string state, string stateFriendlyName, string icon)
        {
            var update = new HomeAssistantEntityStateUpdate()
            {
                State = state,
                Attributes = new HomeAssistantEntityStateUpdateAttributes()
                {
                    FriendlyName = stateFriendlyName,
                    Icon = icon
                }
            };

            var request = new RestRequest($"api/states/{entity}", Method.POST, DataFormat.Json);

            request.AddJsonBody(update);

            Client.Execute(request);
        }
    }
}
