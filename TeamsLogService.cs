using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TeamsPresence
{
    public class TeamsLogService
    {
        public event EventHandler<TeamsStatus> StatusChanged;
        public event EventHandler<TeamsActivity> ActivityChanged;

        private TeamsStatus CurrentStatus;
        private TeamsActivity CurrentActivity;
        private bool Started = false;

        private Stopwatch Stopwatch { get; set; }
        private string LogPath { get; set; }

        public TeamsLogService()
        {
            LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Teams");
            Stopwatch = new Stopwatch();
        }

        public TeamsLogService(string logPath)
        {
            LogPath = logPath;
            Stopwatch = new Stopwatch();
        }

        public void Start()
        {
            Stopwatch.Start();

            var lockMe = new object();
            using (var latch = new ManualResetEvent(true))
            using (var fs = new FileStream(Path.Combine(LogPath, "logs.txt"), FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            using (var fsw = new FileSystemWatcher(LogPath))
            {
                fsw.Changed += (s, e) =>
                {
                    lock (lockMe)
                    {
                        if (e.FullPath != Path.Combine(LogPath, "logs.txt")) return;
                        latch.Set();
                    }
                };

                using (var sr = new StreamReader(fs))
                {
                    while (true)
                    {
                        Thread.Sleep(100);

                        // Throttle based on the stopwatch so we're not sending
                        // tons of updates to Home Assistant.
                        if (Stopwatch.Elapsed.TotalSeconds > 2)
                        {
                            Stopwatch.Stop();

                            if (Started == false)
                            {
                                Started = true;
                                StatusChanged?.Invoke(this, CurrentStatus);
                                ActivityChanged?.Invoke(this, CurrentActivity);
                            }
                        }

                        latch.WaitOne();
                        lock (lockMe)
                        {
                            String line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                LogFileChanged(line, Stopwatch.IsRunning);
                            }
                            latch.Set();
                        }
                    }
                }
            }
        }

        private void LogFileChanged(string line, bool throttled)
        {
            string statusPattern = @"StatusIndicatorStateService: Added (\w+)";
            string activityPattern = @"name: desktop_call_state_change_send, isOngoing: (\w+)";

            RegexOptions options = RegexOptions.Multiline;

            TeamsStatus status = TeamsStatus.Unknown;
            TeamsActivity activity = TeamsActivity.Unknown;

            foreach (Match m in Regex.Matches(line, statusPattern, options))
            {
                if (m.Groups[1].Value != "NewActivity")
                {
                    Enum.TryParse<TeamsStatus>(m.Groups[1].Value, out status);

                    CurrentStatus = status;

                    if (!throttled)
                        StatusChanged?.Invoke(this, status);
                }
            }

            foreach (Match m in Regex.Matches(line, activityPattern, options))
            {
                activity = m.Groups[1].Value == "true" ? TeamsActivity.InACall : TeamsActivity.NotInACall;

                CurrentActivity = activity;

                if (!throttled)
                    ActivityChanged?.Invoke(this, activity);
            }
        }
    }
}
