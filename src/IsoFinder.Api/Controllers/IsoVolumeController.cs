using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IsoFinder.Data;

namespace IsoFinder.Api.Controllers
{
    public class IsoVolumeController : ApiController
    {
        [HttpGet]
        [Route("v1/isovolumes/names")]
        public IEnumerable<string> Get()
        {
            try
            {
                var entities = new IsoFinderEntities();

                return entities.IsoVolumes.Select(v => string.IsNullOrEmpty(v.CustomName) ? v.FileName : v.CustomName).ToList();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}