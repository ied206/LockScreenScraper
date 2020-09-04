using Joveler.FileMagician;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LockScreenScraper
{
    public class NativeLibrary
    {
        public static string MagicFile { get; private set; }

        public static void Init(string baseDir)
        {
            MagicFile = Path.Combine(baseDir, "magic.mgc");

            string libDir = Path.Combine(baseDir, "runtimes");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                libDir = Path.Combine(libDir, "win-");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                libDir = Path.Combine(libDir, "linux-");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                libDir = Path.Combine(libDir, "osx-");

            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    libDir += "x86";
                    break;
                case Architecture.X64:
                    libDir += "x64";
                    break;
                case Architecture.Arm:
                    libDir += "arm";
                    break;
                case Architecture.Arm64:
                    libDir += "arm64";
                    break;
            }
            libDir = Path.Combine(libDir, "native");

            string libPath = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                libPath = Path.Combine(libDir, "libmagic-1.dll");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                libPath = Path.Combine(libDir, "libmagic.so");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                libPath = Path.Combine(libDir, "libmagic.dylib");

            if (libPath == null)
                throw new PlatformNotSupportedException($"Unable to find native library.");
            if (!File.Exists(libPath))
                throw new PlatformNotSupportedException($"Unable to find native library [{libPath}].");

            Magic.GlobalInit(libPath);
        }
    }
}
