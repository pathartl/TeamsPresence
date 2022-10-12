using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsPresence
{
    public enum TeamsStatus
    {
        Unknown,
        Available,
        Busy,
        OnThePhone,
        Away,
        BeRightBack,
        DoNotDisturb,
        Presenting,
        Focusing,
        InAMeeting,
        Offline
    }

    public enum TeamsActivity
    {
        Unknown,
        NotInACall,
        InACall
    }

    public enum CameraStatus
    {
        Inactive,
        Active
    }
}
