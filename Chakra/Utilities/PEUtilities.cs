using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace Chakra.Utilities;

class PEUtilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="folderPath"></param>
    public static void CountArm64xPeFiles(string folderPath)
    {
        var arch = RuntimeInformation.ProcessArchitecture;
        if (arch != Architecture.X64 || arch != Architecture.Arm64)
        {
            Console.WriteLine("Unsupported Platform!");
            return;
        }

        var folderInfo = new DirectoryInfo(folderPath);
        var (X64Binaries, arm64xBinaries) = CountArm64xPeFilesInDirectory(folderInfo);
        Console.WriteLine("============Summary============");
        Console.WriteLine($"x64 Binaries: {X64Binaries}");
        Console.WriteLine($"x64 (ARM64X) Binaries: {arm64xBinaries}");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="folderInfo"></param>
    /// <returns></returns>
    private static (uint X64Binaries, uint arm64xBinaries) CountArm64xPeFilesInDirectory(DirectoryInfo folderInfo)
    {
        uint X64Binaries = 0;
        uint Arm64xBinaries = 0;

        //Enumerate files
        foreach (var f in folderInfo.GetFiles())
        {
            var ext = f.Extension.ToLower();
            if (ext == ".dll" || ext == ".exe")
            {
                uint X64 = 0, A64x = 0;
                (X64, A64x) = CheckPeFileMachineType(f.FullName);
                X64Binaries += X64;
                Arm64xBinaries += A64x;
            }
        }

        //Recursive checking
        foreach (var d in folderInfo.GetDirectories())
        {
            uint X64 = 0, A64x = 0;
            (X64, A64x) = CountArm64xPeFilesInDirectory(d);
            X64Binaries += X64;
            Arm64xBinaries += A64x;
        }

        return (X64Binaries, Arm64xBinaries);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static (uint isAmd64, uint isArm64x) CheckPeFileMachineType(string path)
    {
        uint IsArm64x = 0;
        uint IsAmd64 = 0;
        // Open the Portable Executable (PE) file
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var peReader = new PEReader(fs);
        try
        {
            var coffHeader = peReader.PEHeaders.CoffHeader;
            if (coffHeader.Machine == Machine.Amd64)
            {
                IsAmd64 = 1;
                var secHeaders = peReader.PEHeaders.SectionHeaders;
                //Search for arm64x section
                foreach (var s in secHeaders)
                {
                    if (s.Name == ".a64xrm")
                    {
                        IsArm64x = 1;
                        Console.WriteLine(path);
                        break;
                    }
                }
            }
        }
        //Skip x86 Binaries
        catch (BadImageFormatException)
        {

        }

        return (IsAmd64, IsArm64x);
    }
}