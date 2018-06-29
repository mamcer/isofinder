using System.Configuration;

namespace IsoFinder.Desktop
{
    public static class Constants
    {
        public static string IsoFinderApiUrl
        {
            get
            {
                if (ConfigurationManager.AppSettings["IsoFinderApiUrl"] != null)
                {
                    return ConfigurationManager.AppSettings["IsoFinderApiUrl"];
                }

                return "http://localhost:1906/v1";
            }
        }
        public const string IsoFinderInfoUri = "/isofinderinfo";
        public const string IsoFinderFilesSearchUri = "/files?name={0}&page={1}&pageSize={2}";
        public const string IsoFinderFoldersSearchUri = "/folders?name={0}&page={1}&pageSize={2}";
        public const string IsoFinderSearchAheadUri = "/files?name={0}&count={1}";
        public const string IsoFinderFileRequestUri = "/requests/file";
        public const string IsoFinderFolderRequestUri = "/requests/folder";
        public const string IsoFinderLatestRequestUri = "/requests?status={0}&count={1}";
        public const string IsoFinderVolumeNamesUri = "/isovolumes/names";
        public const int PageSize = 10;
    }
}