using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamsPresence
{
    class Program
    {
        private static HomeAssistantService HomeAssistantService;
        private static TeamsLogService TeamsLogService;
        private static TeamsPresenceConfig Config;

        private static NotifyIcon NotifyIcon;
        private static string ConfigDirectory;

        static void Main(string[] args)
        {
            SetupNotifyIcon();

            var configFile = "config.json";
            var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Teams Presence");
            var configFilePath = Path.Combine(configPath, configFile);

            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);

            ConfigDirectory = configPath;

            if (File.Exists(configFilePath))
            {
                Console.WriteLine("Config file found!");

                try
                {
                    Config = JsonConvert.DeserializeObject<TeamsPresenceConfig>(File.ReadAllText(configFilePath));
                }
                catch
                {
                    NotifyIcon.BalloonTipText = "Config file error. Please fix or recreate.";

                    NotifyIcon.ShowBalloonTip(2000);

                    OpenConfigDirectory();

                    return;
                }
            }
            else
            {
                Console.WriteLine("Config file doesn't exist. Creating...");

                Config = new TeamsPresenceConfig()
                {
                    HomeAssistantUrl = "https://yourha.duckdns.org",
                    HomeAssistantToken = "eyJ0eXAiOiJKV1...",
                    AppDataRoamingPath = "",
                    StatusEntity = "sensor.teams_status",
                    ActivityEntity = "sensor.teams_activity",
                    FriendlyStatusNames = new Dictionary<TeamsStatus, string>()
                    {
                        { TeamsStatus.Available, "Available" },
                        { TeamsStatus.Busy, "Busy" },
                        { TeamsStatus.OnThePhone, "On the phone" },
                        { TeamsStatus.Away, "Away" },
                        { TeamsStatus.BeRightBack, "Be right back" },
                        { TeamsStatus.DoNotDisturb, "Do not disturb" },
                        { TeamsStatus.Presenting, "Presenting" },
                        { TeamsStatus.Focusing, "Focusing" },
                        { TeamsStatus.InAMeeting, "In a meeting" },
                        { TeamsStatus.Offline, "Offline" },
                        { TeamsStatus.Unknown, "Unknown" }
                    },
                    FriendlyActivityNames = new Dictionary<TeamsActivity, string>()
                    {
                        { TeamsActivity.InACall, "In a call" },
                        { TeamsActivity.NotInACall, "Not in a call" },
                        { TeamsActivity.Unknown, "Unknown" }
                    },
                    ActivityIcons = new Dictionary<TeamsActivity, string>()
                    {
                        { TeamsActivity.InACall, "mdi:phone-in-talk-outline" },
                        { TeamsActivity.NotInACall, "mdi:phone-off" },
                        { TeamsActivity.Unknown, "mdi:phone-cancel" }
                    }
                };

                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(Config, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                }));

                NotifyIcon.BalloonTipText = "Generated blank config. Fill out and restart this application.";

                NotifyIcon.ShowBalloonTip(2000);

                OpenConfigDirectory();

                return;
            }

            TeamsLogService = new TeamsLogService();
            HomeAssistantService = new HomeAssistantService(Config.HomeAssistantUrl, Config.HomeAssistantToken);

            TeamsLogService.StatusChanged += Service_StatusChanged;
            TeamsLogService.ActivityChanged += Service_ActivityChanged;

            Thread presenceDetectionThread = new Thread(
                delegate ()
                {
                    TeamsLogService.Start();
                });

            NotifyIcon.BalloonTipText = "Service started. Waiting for Teams updates...";

            NotifyIcon.ShowBalloonTip(2000);

            presenceDetectionThread.Start();

            Application.Run();
        }

        private static void SetupNotifyIcon()
        {
            NotifyIcon = new NotifyIcon()
            {
                Icon = Resources.Icon,
                Visible = true,
                Text = "Teams Presence",
                BalloonTipTitle = "Teams Presence",
                BalloonTipIcon = ToolTipIcon.Info,
                ContextMenu = new ContextMenu()
            };

            var exitMenuItem = new MenuItem()
            {
                Text = "Quit",
                Index = 0,
            };

            exitMenuItem.Click += Program.Quit;

            var openConfigFolderMenuItem = new MenuItem()
            {
                Text = "Open Config Folder",
                Index = 1
            };

            openConfigFolderMenuItem.Click += OpenConfigDirectory;

            NotifyIcon.ContextMenu.MenuItems.AddRange(new MenuItem[] {
                exitMenuItem,
                openConfigFolderMenuItem
            });
        }

        private static void OpenConfigDirectory()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = ConfigDirectory,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }

        private static void Service_StatusChanged(object sender, TeamsStatus status)
        {
            HomeAssistantService.UpdateEntity(Config.StatusEntity, Config.FriendlyStatusNames[status], Config.FriendlyStatusNames[status], "mdi:microsoft-teams");

            Console.WriteLine($"Updated status to {Config.FriendlyStatusNames[status]} ({status})");
        }

        private static void Service_ActivityChanged(object sender, TeamsActivity activity)
        {
            HomeAssistantService.UpdateEntity(Config.ActivityEntity, Config.FriendlyActivityNames[activity], Config.FriendlyActivityNames[activity], Config.ActivityIcons[activity]);

            Console.WriteLine($"Updated activity to {Config.FriendlyActivityNames[activity]} ({activity})");
        }

        private static void OpenConfigDirectory(object sender, EventArgs e)
        {
            OpenConfigDirectory();
        }

        private static void Quit(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
