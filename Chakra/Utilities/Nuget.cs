namespace Chakra.Utilities;

internal class NugetCleaner
{
    public static void CleanupNugetFolder(string path)
    {
        var di = new DirectoryInfo(path);
        Parallel.ForEach(di.GetDirectories(), dir =>
        {
            var subDir = dir.GetDirectories();//versions
            foreach (var i in Enumerable.Range(0, subDir.Length - 1))
            {
                Directory.Delete(subDir[i].FullName, true);
                Console.WriteLine($"Deleted: {subDir[i].FullName}");
            }
        });
    }
}