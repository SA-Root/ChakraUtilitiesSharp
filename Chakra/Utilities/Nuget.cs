namespace Chakra.Utilities;

internal class NugetCleaner : IComparer<string>
{
    public static NugetCleaner Instacne { get; } = new();



    public static int CleanupNugetFolder(string path)
    {
        var di = new DirectoryInfo(path);
        int cleaned = 0;
        Parallel.ForEach(di.GetDirectories(), dir =>
        {
            try
            {
                var subDir = dir.GetDirectories().OrderBy(x => x.Name, Instacne).ToArray();//versions
                if (subDir.Length <= 1)
                    return;
                for (int i = 0; i < subDir.Length - 1; i++)
                {
                    subDir[i].Delete(true);
                }
                Console.WriteLine($"Cleaned: {dir.Name}");
                Interlocked.Increment(ref cleaned);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR in {dir.Name}: {e.Message}");
            }
        });
        return cleaned;
    }

    public int Compare(string? x, string? y)
    {
        if (x is null || y is null)
            return -1;
        return Version.Parse(x).CompareTo(Version.Parse(y));
    }
}