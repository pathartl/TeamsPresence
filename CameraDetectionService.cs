using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TeamsPresence
{
    public class CameraStatusChangedEventArgs : EventArgs
    {
        public CameraStatus Status { get; set; }
        public string AppName { get; set; }
    }

    public class CameraDetectionService
    {
        private const string SubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam";
        private const string AppNamePattern = @"_[\d|\w]{13}$";

        public event EventHandler<CameraStatusChangedEventArgs> StatusChanged;

        private int PollingRate;
        private string ActiveAppName = "";

        public CameraDetectionService(int pollingRate)
        {
            PollingRate = pollingRate;
        }

        public void Start()
        {
            var appNameRegex = new Regex(AppNamePattern);

            while (true)
            {
                var activeCameraApp = GetActiveCameraApp();

                if (activeCameraApp != null)
                    activeCameraApp = appNameRegex.Replace(activeCameraApp, "", 1);

                if (activeCameraApp != ActiveAppName)
                {
                    ActiveAppName = activeCameraApp;

                    StatusChanged?.Invoke(this, new CameraStatusChangedEventArgs()
                    {
                        Status = ActiveAppName == "" ? CameraStatus.Inactive : CameraStatus.Active,
                        AppName = ActiveAppName
                    });
                }

                Thread.Sleep(PollingRate);
            }
        }

        private string GetActiveCameraApp()
        {
            var key = Registry.CurrentUser.OpenSubKey(SubKey);

            foreach (var app in key.GetSubKeyNames())
            {
                var lastUsedTimeStop = Registry.CurrentUser.OpenSubKey($@"{SubKey}\{app}")?.GetValue("LastUsedTimeStop");

                if (lastUsedTimeStop != null && (long)lastUsedTimeStop == 0)
                {
                    return app;
                }
            }

            return "";
        }
    }
}
