using System.Linq;
using System.Web.Mvc;
using IsoFinder.Application;
using IsoFinder.Data;

namespace IsoFinder.Web.Controllers
{
    public class RequestController : Controller
    {
        public ActionResult Index()
        {
            var userInfo = Session["UserInfo"] as Models.UserInfo;

            return View(userInfo);
        }

        public ActionResult Download()
        {
            var userInfo = Session["UserInfo"] as Models.UserInfo;
            var isoRequestService = new IsoRequestService(new UnitOfWork());
            isoRequestService.Insert(userInfo.Id, userInfo.CartFileItems.Select(m => m.Id).ToList(), userInfo.CartFolderItems.Select(m => m.Id).ToList());

            return RedirectToAction("Cart");
        }
    }
}