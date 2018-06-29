namespace ISOFinder.Domain.MainModule
{
    public class IsoDto
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string VolumeLabel { get; set; }

        public int FileCount { get; set; }

        public decimal Size { get; set; }

        public string SizeString
        {
            get { return FileDto.GetSizeString(Size); }
        }
    }
}