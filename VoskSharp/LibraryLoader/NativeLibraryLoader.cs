using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VoskSharp.LibraryLoader
{
    public static class NativeLibraryLoader
    {
        internal static LoadResult LoadNativeLibrary()
        {
            
            var architecture = RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm64 => "arm64",
                _ => throw new PlatformNotSupportedException($"Unsupported OS platform, architecture: {RuntimeInformation.OSArchitecture}")
            };

            var (platform, dynamicLibraryName) = Environment.OSVersion.Platform switch
            {
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => ("win", "libvosk.dll"),
                _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => ("linux", "libvosk.so"),
                _ => throw new PlatformNotSupportedException($"Unsupported OS platform, architecture: {RuntimeInformation.OSArchitecture}")
            };
            var path = dynamicLibraryName;

            if (!File.Exists(path))
                path = Path.Combine("runtimes",  $"{platform}-{architecture}", "native", dynamicLibraryName);

            ILibraryLoader libraryLoader = platform switch
            {
                "win" => new WindowsLibraryLoader(),
                "linux" => new LinuxLibraryLoader(),
                _ => throw new PlatformNotSupportedException()
            };
            if (libraryLoader is WindowsLibraryLoader loader)
            {
                var dir = new FileInfo(path).DirectoryName;
                loader.OpenLibrary(Path.Combine(dir, "libwinpthread-1.dll"));
                loader.OpenLibrary(Path.Combine(dir, "libgcc_s_seh-1.dll"));
                loader.OpenLibrary(Path.Combine(dir, "libstdc++-6.dll"));
            }

            var result = libraryLoader.OpenLibrary(path);
            return result;
        }
    }
}
