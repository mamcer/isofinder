using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IsoFinder.Data;
using IsoFinder.Model;

namespace IsoFinder.Api.Controllers
{
    public class FolderController : ApiController
    {
        private int AddPartialResults(IsoFinderEntities entities, Func<IsoFolder, bool> searchExpression, SearchResultInfo<FolderSearchResult> searchResults, int skip, int pageSize)
        {
            var total = entities.IsoFolders.Count(searchExpression);
            var results = entities.IsoFolders.Where(searchExpression).OrderBy(f => f.Name).Skip(skip).Take(pageSize).ToList();

            foreach (var isoFolder in results)
            {
                searchResults.Results.Add(new FolderSearchResult
                {
                    Id = isoFolder.Id,
                    Name = isoFolder.Name,
                    IsoId = isoFolder.IsoId,
                    ParentFolderId = isoFolder.ParentFolderId.Value
                });
            }

            return total;
        }

        [HttpGet]
        [Route("v1/folders")]
        public SearchResultInfo<FolderSearchResult> Search(string name, int page, int pageSize)
        {
            try
            {
                var entities = new IsoFinderEntities();
                var searchResults = new SearchResultInfo<FolderSearchResult>();

                //Starts with
                Func<IsoFolder, bool> searchExpression = f => f.Name.StartsWith(name);
                var startWithTotal = AddPartialResults(entities, searchExpression, searchResults, page * pageSize, pageSize);

                //Contains
                var containsTotal = 0;
                if (startWithTotal < pageSize)
                {
                    searchExpression = f => !f.Name.StartsWith(name) && f.Name.Contains(name);
                    containsTotal = AddPartialResults(entities, searchExpression, searchResults, page * pageSize, pageSize - startWithTotal);
                }

                searchResults.Total = startWithTotal + containsTotal;

                return searchResults;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

    }
}
