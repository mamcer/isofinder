namespace IsoFinder.Model
{
    public class FolderSearchResult : SearchResult
    {
        public int IsoId { get; set; }

        public int ParentFolderId { get; set; }
    }
}