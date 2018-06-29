using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IsoFinder.Data;
using IsoFinder.Model;

namespace IsoFinder.Api.Controllers
{
    public class RequestController : ApiController
    {
        public readonly string _isoFinderFilesUrl = ConfigurationManager.AppSettings["IsoFinderFilesUrl"];

        [HttpGet]
        [Route("v1/requests")]
        public List<IsoFinderRequestStatus> GetLatestRequests(string status, int count)
        {
            try
            {
                var entities = new IsoFinderEntities();
                List<IsoRequest> requests;
                if (status.ToLower() == "any")
                {
                    requests = entities.IsoRequests.OrderByDescending(r => r.Created).Take(count).ToList();
                }
                else
                {
                    requests = entities.IsoRequests.Where(r => r.IsoRequestStatu.Name.ToLower() == status).OrderByDescending(r => r.Created).Take(count).ToList();
                }

                return requests.Select(request => new IsoFinderRequestStatus
                    {
                        Created = request.Created,
                        Completed = request.Completed,
                        Status = request.IsoRequestStatu.Name,
                        DownloadLink = _isoFinderFilesUrl + request.FileName
                    }).ToList();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPost]
        [Route("v1/requests/file")]
        public HttpResponseMessage Create(IsoFinderFileRequest fileRequest)
        {
            try
            {
                var entities = new IsoFinderEntities();
                var user = entities.Users.Find(fileRequest.UserId);
                var requestStatus = entities.IsoRequestStatus.Find((int)IsoRequestStatus.Pending);

                var isoFiles = new List<IsoRequestFile>();
                foreach (int fileId in fileRequest.FileIds)
                {
                    var file = entities.IsoFiles.Find(fileId);
                    isoFiles.Add(new IsoRequestFile { IsoFile = file });
                }

                var isoFinderRequest = new IsoRequest
                {
                    User = user,
                    IsoRequestFiles = isoFiles,
                    Created = DateTime.Today,
                    IsoRequestStatu = requestStatus,
                    Query = fileRequest.Query
                };

                entities.IsoRequests.Add(isoFinderRequest);
                entities.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        [HttpPost]
        [Route("v1/requests/folder")]
        public HttpResponseMessage Create(IsoFinderFolderRequest fileRequest)
        {
            try
            {
                var entities = new IsoFinderEntities();
                var user = entities.Users.Find(fileRequest.UserId);
                var requestStatus = entities.IsoRequestStatus.Find((int)IsoRequestStatus.Pending);

                var isoFolders = new List<IsoRequestFolder>();
                foreach (int folderId in fileRequest.FolderIds)
                {
                    var folder = entities.IsoFolders.Find(folderId);
                    isoFolders.Add(new IsoRequestFolder { IsoFolder = folder});
                }

                var isoFinderRequest = new IsoRequest
                {
                    User = user,
                    IsoRequestFolders = isoFolders,
                    Created = DateTime.Today,
                    IsoRequestStatu = requestStatus,
                    Query = fileRequest.Query
                };

                entities.IsoRequests.Add(isoFinderRequest);
                entities.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

    }
}