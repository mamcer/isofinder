namespace IsoFinder.FileScanner
{
    public interface IDirectoryProvider
    {
        string[] GetFiles(string path, string searchPattern);

        string[] GetDirectories(string path);
    }
}