using System;
using System.IO;
using IsoFinder.Core;

namespace IsoFinder.FileScanner
{
    public class IsoFileProvider : IIsoFileProvider
    {
        public IsoFile GetIsoFile(string filePath)
        {
            var info = new FileInfo(filePath);

            var file = new IsoFile
            {
                Name = info.Name,
                Extension = info.Extension.ToLower(),
                Created = info.CreationTime,
                Modified = info.LastWriteTime,
                Size = Convert.ToDecimal(info.Length)
            };

            return file;
        }
    }
}