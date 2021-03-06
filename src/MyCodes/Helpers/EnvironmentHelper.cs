﻿using System;
using System.IO;

namespace MyCodes.Helpers
{
    public class EnvironmentHelper
    {
        public string ArchSpecificPath()
        {
            var archSpecificPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Environment.Is64BitProcess ? "x64" : "x86", "");
            return archSpecificPath;
        }

        public bool Is64BitProcess()
        {
            return Environment.Is64BitProcess;
        }
        public bool Is64BitOperatingSystem()
        {
            return Environment.Is64BitOperatingSystem;
        }

        public Version ClrVersion()
        {
            return Environment.Version;
        }
        public OperatingSystem OsVersion()
        {
            return Environment.OSVersion;
        }

        public string CurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        public EnvironmentInfo CreateEnvironmentInfo()
        {
            var info = new EnvironmentInfo();
            info.Is64BitProcess = Is64BitProcess();
            info.Is64BitOperatingSystem = Is64BitOperatingSystem();
            info.ClrVersion = ClrVersion().ToString();
            info.OsVersion = OsVersion().ToString();
            info.CurrentDirectory = CurrentDirectory();
            return info;
        }

        public static EnvironmentHelper Instance = new EnvironmentHelper();
    }

    public class EnvironmentInfo
    {
        public bool Is64BitProcess { get; set; }
        public bool Is64BitOperatingSystem { get; set; }
        public string ClrVersion { get; set; }
        public string OsVersion { get; set; }
        public string CurrentDirectory { get; set; }
    }
}
