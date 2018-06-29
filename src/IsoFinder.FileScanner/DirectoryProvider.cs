using System.IO;

namespace IsoFinder.FileScanner
{
    public class DirectoryProvider : IDirectoryProvider
    {
        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}