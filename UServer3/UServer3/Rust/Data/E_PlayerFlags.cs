using System;

namespace UServer3.Rust.Data
{
    [Flags]
    public enum E_PlayerFlags
    {
        Aiming = 16384,
        ChatMute = 4096,
        Connected = 256,
        DisplaySash = 32768,
        EyesViewmode = 2048,
        HasBuildingPrivilege = 2,
        InBuildingPrivilege = 1,
        IsAdmin = 4,
        IsDeveloper = 128,
        NoSprint = 8192,
        ReceivingSnapshot = 8,
        Sleeping = 16,
        Spectating = 32,
        ThirdPersonViewmode = 1024,
        VoiceMuted = 512,
        Wounded = 64
    }
}