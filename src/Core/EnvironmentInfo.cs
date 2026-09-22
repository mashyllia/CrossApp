using System.Runtime.InteropServices;

namespace Core;

public sealed record EnvironmentReport(
    string Application,
    string Student,
    string OsDescription,
    string OsVersion,
    string ProcessArchitecture,
    string DotNetVersion,
    string FrameworkDescription,
    string DetectedRid,
    string ReportedRid,
    string BaseDirectory,
    string Domain);

public static class EnvironmentInfo
{
    public static EnvironmentReport Collect() => new(
        Application: "CrossApp",
        Student: "Пинчук Марія, ФЕІ-33",
        OsDescription: RuntimeInformation.OSDescription,
        OsVersion: Environment.OSVersion.ToString(),
        ProcessArchitecture: RuntimeInformation.ProcessArchitecture.ToString(),
        DotNetVersion: Environment.Version.ToString(),
        FrameworkDescription: RuntimeInformation.FrameworkDescription,
        DetectedRid: DetectRid(),
        ReportedRid: RuntimeInformation.RuntimeIdentifier,
        BaseDirectory: AppContext.BaseDirectory,
        Domain: "Замовлення (клієнт, товар, замовлення, рядок замовлення)"
    );

    private static string DetectRid()
    {
        string os = 
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "unknown";

        string arch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm64 => "arm64",
            Architecture.Arm => "arm",
            _ => "unknown"
        };

        return $"{os}-{arch}";
    }
}