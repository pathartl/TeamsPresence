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
        public string StatusEntityFriendlyName { get; set; }
        public string ActivityEntity { get; set; }
        public string ActivityEntityFriendlyName { get; set; }
        public string CameraStatusEntity { get; set; }
        public string CameraStatusEntityFriendlyName { get; set; }
        public string CameraAppEntity { get; set; }
        public string CameraAppEntityFriendlyName { get; set; }

        public int CameraStatusPollingRate { get; set; }

        public Dictionary<TeamsStatus, string> FriendlyStatusNames { get; set; }
        public Dictionary<TeamsActivity, string> FriendlyActivityNames { get; set; }
        public Dictionary<CameraStatus, string> FriendlyCameraStatusNames { get; set; }

        public Dictionary<TeamsActivity, string> ActivityIcons { get; set; }
        public Dictionary<CameraStatus, string> CameraStatusIcons { get; set; }

        public string CameraAppIcon { get; set; }
    }
}
