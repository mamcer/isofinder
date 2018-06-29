using IsoFinder.Core;
using PagedList;

namespace IsoFinder.Application
{
    public interface ISearchService
    {
        IPagedList<IsoFile> SearchToPagedList(string fileName, int page, int pageSize);
    }
}