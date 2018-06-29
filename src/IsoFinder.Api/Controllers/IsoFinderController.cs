using System.Linq;
using System.Web.Http;
using IsoFinder.Data;
using IsoFinder.Model;

namespace IsoFinder.Api.Controllers
{
    public class IsoFinderController : ApiController
    {
        /// <summary>
        /// Returns general information about the application.
        /// </summary>
        /// <returns>The number of files and volumes in the database.</returns>
        [Route("v1/isofinderinfo")]
        public IsoFinderInfo Get()
        {
            var entities = new IsoFinderEntities();

            var isoFinderInfo = new IsoFinderInfo
            {
                IsoFileCount = entities.IsoFiles.Count(),
                IsoVolumeCount = entities.IsoVolumes.Count()
            };

            return isoFinderInfo;
        }
    }
}