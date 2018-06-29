namespace IsoFinder.Model
{
    public abstract class IsoFinderRequest
    {
        public int UserId { get; set; }

        public string Query { get; set; }
    }
}
