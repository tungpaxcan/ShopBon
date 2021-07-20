using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopBon.Models;

using PagedList;
using PagedList.Mvc;
namespace ShopBon.Controllers
{
    public class SiteController : Controller
    {
        dbShopBonDataContext db = new dbShopBonDataContext();
        private List<Product> LayNHMoi(int count)
        {
            return db.Products.OrderByDescending(a => a.Created_At).Take(count).ToList();
        }
        // GET: Site
        public ActionResult Index(int ? page)
        {
            //int SoMauTin = db.Products.Count();
            //ViewBag.SoMauTin = SoMauTin;
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var NHMoi = LayNHMoi(500);
           

            return View(NHMoi.ToPagedList(pageNum,pageSize));
        }
        public ActionResult ChuDe()
        {
            var ChuDe = from cd in db.Categories select cd;
            return PartialView(ChuDe);
        }
        public ActionResult DeTai()
        {
            var DeTai = from dt in db.Topics select dt;
            return PartialView(DeTai);
        }
        public ActionResult SanPhamTheoLoai(int id)
        {
            var SanPham = from sp in db.Products where sp.CatId == id select sp;

            return View(SanPham);
        }
        public ActionResult DeTaiMoi(string id)
        {
            var DeTai = from dt in db.Topics where dt.Slug == id select dt;  

            return View(DeTai);
        }
        public ActionResult Details(int id)
        {
            var SanPham = from sp in db.Products
                          where sp.Id == id
                          select sp;

            return View(SanPham.Single());
        }
        public ActionResult Menu()
        {
            User kh = (User)Session["UserName"];
            //var user = from us in db.Users select us;
            return PartialView(kh);

           
        }


    }
}