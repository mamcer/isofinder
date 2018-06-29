using IsoFinder.Core;

namespace IsoFinder.FileScanner
{
    public interface IIsoFileProvider
    {
        IsoFile GetIsoFile(string filePath);
    }
}