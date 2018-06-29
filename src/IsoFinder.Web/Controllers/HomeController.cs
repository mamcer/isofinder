using System.Linq;
using System.Web.Mvc;
using IsoFinder.Application;
using IsoFinder.Data;
using IsoFinder.Web.Models;
using PagedList;

namespace IsoFinder.Web.Controllers
{
    public class HomeController : Controller
    {
        private int _pageSize = 20;

        public ActionResult Index()
        {
            var isoVolumeService = new IsoVolumeService(new UnitOfWork());
            var isoVolumes = isoVolumeService.GetAll();
            var fileCount = isoVolumes.Sum(m => m.FileCount);
            var isoFinderInfo = new IsoFinderInfo { FileCount = fileCount, IsoVolumeCount = isoVolumes.Count(), PageNumber = 1 };

            return View(isoFinderInfo);
        }

        [HttpPost]
        public ActionResult Index(IsoFinderInfo isoFinderInfo)
        {
            return RedirectToAction("Search", isoFinderInfo);
        }

        public ActionResult Search(IsoFinderInfo isoFinderInfo)
        {
            var searchService = new SearchService(new UnitOfWork());
            var pageNumber = isoFinderInfo.PageNumber == 0 ? 1 : isoFinderInfo.PageNumber;
            var results = searchService.SearchToPagedList(isoFinderInfo.SearchQuery, pageNumber, _pageSize);

            ViewBag.Total = results.TotalItemCount;
            ViewBag.SearchQuery = isoFinderInfo.SearchQuery;
            return View(results);
        }

        public ActionResult IsoVolumes()
        {
            var isoVolumeService = new IsoVolumeService(new UnitOfWork());
            var isoVolumes = isoVolumeService.GetAll().Select(m => new IsoVolumeInfo { Id = m.Id, Name = m.VolumeLabel, RootFolderId = m.RootFolder.Id }).ToList();

            IsoVolumes iv = new IsoVolumes();
            iv.Items = isoVolumes;

            return View(iv);
        }

        public ActionResult IsoFolder(int isoId, int folderId)
        {
            var isoFolderService = new IsoFolderService(new UnitOfWork());
            var isoVolumeService = new IsoVolumeService(new UnitOfWork());
            var isoVolume = isoVolumeService.GetById(isoId);
            var isoFolder = isoFolderService.GetById(folderId);


            var isoFolders = new IsoFolders();
            isoFolders.IsoFolderId = isoFolder.Id;
            isoFolders.IsoFolderName = isoFolder.Name;
            isoFolders.IsoVolumeName = isoVolume.VolumeLabel;
            isoFolders.IsoVolumeId = isoVolume.Id;
            isoFolders.FolderItems = isoFolder.ChildFolders.Select(m => new IsoFolderInfo { Id = m.Id, Name = m.Name });
            isoFolders.FileItems = isoFolder.ChildFiles.Select(m => new IsoFileInfo { Id = m.Id, Name = m.Name });

            return View(isoFolders);
        }

        public ActionResult AddFolderToCart(int folderId, string folderName, int isoId, int rootFolderId)
        {
            var userInfo = Session["UserInfo"] as Models.UserInfo;
            userInfo.CartFolderItems.Add(new IsoFolderInfo { Id = folderId, Name = folderName });

            return RedirectToAction("IsoFolder", new { isoId = isoId, folderId = rootFolderId });
        }

        public ActionResult AddFileToCart(int fileId, string fileName, int isoId, int rootFolderId)
        {
            var userInfo = Session["UserInfo"] as Models.UserInfo;
            userInfo.CartFileItems.Add(new IsoFileInfo { Id = fileId, Name = fileName });

            return RedirectToAction("IsoFolder", new { isoId = isoId, folderId = rootFolderId });
        }
    }
}