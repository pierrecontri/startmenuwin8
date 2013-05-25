using System;

namespace StartMenuWin8
{
    class Constants
    {
        public const string computerManagement = "compmgmt.msc";
        public const string startMenuPath = @"%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs";
        public const string commonStartMenuPath = @"%ALLUSERSPROFILE%\Microsoft\Windows\Start Menu\Programs";
        public const string startMenuPrograms = "Programs";
        public const string deviceManagement = "devmgmt.msc";
        public const string panelConfig = @"%systemroot%\system32\control.exe";
        public const string explorer = "explorer.exe";
        public const string keyRegisterComputer = "::{20d04fe0-3aea-1069-a2d8-08002b30309d}";
        public const string cmdShutdown = "shutdown";
        public const string folderTitle = "'s folders";
    }

    public enum ShutdownParameter
    {
        [StringEnumValue("/s /t 00")]
        Shutdown,
        [StringEnumValue("/s /hybrid /t 00")]
        ShutdownWithFasterStart,
        [StringEnumValue("/r /t 00")]
        Restart,
        [StringEnumValue("/h")]
        Hibernate,
        [StringEnumValue("/h /hybrid")]
        StandBy,
        [StringEnumValue("/l")]
        LogOff
    }
}
