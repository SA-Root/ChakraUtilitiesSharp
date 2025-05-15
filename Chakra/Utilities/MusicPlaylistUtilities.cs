namespace Chakra.Utilities;

class MusicUtilities
{
    public static void CleanupMusicFolder(List<string> targetPaths,
        string playlistsPath = @"C:\Users\a1240\OneDrive\公开\Playlists")
    {
        var InPlaylist = new List<string>();
        var NotInPlaylist = new List<string>();

        void CheckDirectory(DirectoryInfo ndi)
        {
            foreach (var f in ndi.GetFiles())
            {
                if (!InPlaylist.Contains(f.FullName))
                    NotInPlaylist.Add(f.FullName);
            }
            foreach (var d in ndi.GetDirectories())
            {
                CheckDirectory(d);
            }
        }

        var di = new DirectoryInfo(playlistsPath);
        if (di.Exists)
        {
            foreach (var f in di.GetFiles())
            {
                using var sr = f.OpenText();
                string? song;
                while ((song = sr?.ReadLine()) is not null)
                {
                    if (!song.StartsWith('#'))
                        InPlaylist.Add(song);
                }
            }
            foreach (var path in targetPaths)
            {
                var ndi = new DirectoryInfo(path);
                CheckDirectory(ndi);
            }
        }

        foreach (var song in NotInPlaylist)
        {
            File.Delete(song);
        }

        Console.WriteLine($"{NotInPlaylist.Count} not-in-playlist songs deleted.");
    }
}
