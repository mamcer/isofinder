using System.Linq;
using IsoFinder.Core;
using IsoFinder.Data;
using PagedList;

namespace IsoFinder.Application
{
    public class SearchService : ISearchService
    {
        private IUnitOfWork _unitOfWork;

        public SearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IPagedList<IsoFile> SearchToPagedList(string fileName, int page, int pageSize)
        {
            var pagedResult = _unitOfWork.IsoFileRepository.SearchToPagedList(q => q.OrderBy(f => f.Name), page, pageSize, f => f.Name.ToLower().Contains(fileName.ToLower()));
            return pagedResult;
        }
    }
}