using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopBon.Models;
using PagedList.Mvc;
using PagedList;
using System.IO;

namespace ShopBon.Controllers
{
    public class AdminController : Controller
    {
        dbShopBonDataContext db = new dbShopBonDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Product(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.Products.ToList().OrderBy(n => n.Id).ToPagedList(pageNumber, pageSize));

        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if(String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải Nhập Tên Đăng Nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải Nhập Mật khẩu";
            }    
            else
            { 
            Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
            if (ad != null)
            {
                Session["TaiKhoanAdmin"] = ad;
                return RedirectToAction("Index", "Admin");
            }
            else
                ViewBag.ThongBao = "Tên Đăng Nhập Hoặc Mật Khẩu Không Đúng";    
            }return View();
    }
        [HttpGet]
        public ActionResult ThemMoi()
        {
            ViewBag.CatId = new SelectList(db.Categories.ToList().OrderBy(n => n.Name), "Id", "Name");


            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoi( Product product, HttpPostedFileBase fileUpload)
        {
            ViewBag.CatId = new SelectList(db.Categories.ToList().OrderBy(n => n.Name), "Id", "Name");
            if (fileUpload == null)
            {
                ViewBag.ThongBao = "Vui Lòng Chọn Ảnh";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Public/Images/"), fileName);
                    if (System.IO.File.Exists(path))
                    { 
                        ViewBag.ThongBao = "Hình Ảnh Đã Tồn Tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    product.Img = fileName;
                    db.Products.InsertOnSubmit(product);
                    //UpdateModel(product);
                    db.SubmitChanges();
                }
                return RedirectToAction("Product");
            }

        }
        public ActionResult ChiTietSanPham(int id)
        {
            Product product = db.Products.SingleOrDefault(n => n.Id == id);
            ViewBag.Id = product.Id;
            if(product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
       [HttpGet]
       public ActionResult Xoa(int id)
        {
            Product product = db.Products.SingleOrDefault(n => n.Id == id);
            ViewBag.Id = product.Id;
            if(product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
        [HttpPost,ActionName("Xoa")]
        public ActionResult XacNhanXoa(int id)
        {
            Product product = db.Products.SingleOrDefault(n => n.Id == id);
            ViewBag.Id = product.Id;
            if(product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Products.DeleteOnSubmit(product);
            db.SubmitChanges();
            return RedirectToAction("Product");
        }
        [HttpGet]
        public ActionResult Sua(int id)
        {
            Product product = db.Products.SingleOrDefault(n => n.Id == id);
           if(product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.CatId = new SelectList(db.Categories.ToList().OrderBy(n => n.Name), "Id", "Name");
            return View(product);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Sua(Product product,HttpPostedFileBase fileUpload)
        {
            ViewBag.CatId = new SelectList(db.Categories.ToList().OrderBy(n => n.Name), "Id", "Name");
            if (fileUpload == null)
            {
                ViewBag.ThongBao = "Vui Lòng Chọn Ảnh";
                return View();
            }
            else
            {
                if(ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Public/Images/"), fileName);
                    if(System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình Ảnh Đã Tồn Tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    product.Img = fileName;
                    UpdateModel(product);
                    db.SubmitChanges();
                }
                return RedirectToAction("Product");
            }
        }
}
    }
