using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsPresence
{
    public class TeamsPresenceConfig
    {
        public string HomeAssistantUrl { get; set; }
        public string HomeAssistantToken { get; set; }
        public string AppDataRoamingPath { get; set; }

        public string StatusEntity { get; set; }
        public string ActivityEntity { get; set; }

        public Dictionary<TeamsStatus, string> FriendlyStatusNames { get; set; }
        public Dictionary<TeamsActivity, string> FriendlyActivityNames { get; set; }

        public Dictionary<TeamsActivity, string> ActivityIcons { get; set; }
    }
}
