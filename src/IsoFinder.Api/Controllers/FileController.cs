using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IsoFinder.Data;
using IsoFinder.Model;
using System.Linq.Expressions;

namespace IsoFinder.Api.Controllers
{
    public class FileController : ApiController
    {
        private int AddPartialResults(GenericRepository<IsoFile> isoFileRepository, Expression<Func<IsoFile, bool>> searchExpression, SearchResultInfo<FileSearchResult> searchResults, int skip, int pageSize)
        {
            var results = isoFileRepository.SearchPaged(q => q.OrderBy(p => p.Name), skip, pageSize, searchExpression);
            var total = results.TotalCount;

            foreach (var isoFile in results.Items)
            {
                searchResults.Results.Add(new FileSearchResult
                {
                    Id = isoFile.Id,
                    FolderId = isoFile.FolderId,
                    Name = isoFile.Name,
                    Created = isoFile.Created,
                    Extension = isoFile.Extension,
                    Modified = isoFile.Modified,
                    Path = isoFile.Path,
                    Size = isoFile.Size,
                    IsoVolumeLabel = isoFile.IsoFolder.IsoVolume.VolumeLabel
                });
            }

            return total;
        }

        /// <summary>
        /// Search for files
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>A list with information about the files that start with 'name' or contains the 'name'</returns>
        [HttpGet]
        [Route("v1/files")]
        public SearchResultInfo<FileSearchResult> Search(string name, int page, int pageSize)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var searchResults = new SearchResultInfo<FileSearchResult>();

                    //Starts with
                    Expression<Func<IsoFile, bool>> searchExpression = f => f.Name.StartsWith(name);
                    var startWithTotal = AddPartialResults(unitOfWork.IsoFileRepository, searchExpression, searchResults, page * pageSize, pageSize);

                    //Contains
                    var containsTotal = 0;
                    if (startWithTotal < pageSize)
                    {
                        searchExpression = f => !f.Name.StartsWith(name) && f.Name.Contains(name);
                        containsTotal = AddPartialResults(unitOfWork.IsoFileRepository, searchExpression, searchResults, page * pageSize, pageSize - startWithTotal);
                    }

                    searchResults.Total = startWithTotal + containsTotal;

                    return searchResults;
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        private int AddSearchAheadResults(GenericRepository<IsoFile> entities, Expression<Func<IsoFile, bool>> searchExpression, List<string> searchResults, int count)
        {
            var results = entities.SearchPaged(q => q.OrderBy(p => p.Name), 0, count, searchExpression, distinct: new NameComparer());

            foreach (var isoFile in results.Items)
            {
                searchResults.Add(isoFile.Name);
            }

            return results.TotalCount;
        }
        
        [HttpGet]
        [Route("v1/files")]
        public List<string> SearchAhead(string name, int count)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var searchResults = new List<string>();

                    //Starts with
                    Expression<Func<IsoFile, bool>> searchExpression = f => f.Name.StartsWith(name);
                    var startWithTotal = AddSearchAheadResults(unitOfWork.IsoFileRepository, searchExpression, searchResults, count);

                    //Contains
                    if (startWithTotal < count)
                    {
                        searchExpression = f => !f.Name.StartsWith(name) && f.Name.Contains(name);
                        AddSearchAheadResults(unitOfWork.IsoFileRepository, searchExpression, searchResults, count - startWithTotal);
                    }

                    return searchResults;
                }
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }

    public class NameComparer : IEqualityComparer<IsoFile>
    {
        public bool Equals(IsoFile isoFile01, IsoFile isoFile02)
        {
            return isoFile01.Name.Equals(isoFile02.Name);
        }

        public int GetHashCode(IsoFile isoFile)
        {
            return isoFile.Name.GetHashCode();
        }
    }
}