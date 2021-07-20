using Facebook;
using ServiceStack.Text;
using ShopBon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopBon.Controllers
{
    public class NguoiDungController : Controller
    { dbShopBonDataContext db = new dbShopBonDataContext();

        

        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangKi()
        {
            return View("DangKi");
        }
        [HttpPost]
        public ActionResult DangKi(FormCollection collection,User kh )
        {
            var hoten = collection["HoTenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            var nhaplaimatkhau = collection["NhapLaiMatKhau"];
            var diachi = collection["DiaChi"];
            var email = collection["Email"];
            var dienthoai = collection["DienThoai"];
            var anhdaidien = collection["AnhDaiDien"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ Tên Khách Hàng Không Được Để Trống";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập Tên Đăng Nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải Nhập Mật Khẩu";
            }
            else if (String.IsNullOrEmpty(nhaplaimatkhau))
            {
                ViewData["Loi4"] = "Phải Nhập Lại Mật Khẩu";

            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi5"] = "Phải Nhập Địa Chỉ";

            }
            if (string.IsNullOrEmpty(email))
            {
                ViewData["Loi6"] = "Email Không Được Bỏ trống";
            }
             if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = "Phải Nhập Điện Thoại";
            }

            
            else
            {
                kh.FullName = hoten;
                kh.UserName = tendn;
                kh.Password = matkhau;
                kh.Email = email;
                kh.Address = diachi;
                kh.Phone = dienthoai;   
                kh.Created_At = DateTime.Parse(ngaysinh);
                db.Users.InsertOnSubmit(kh);
                db.SubmitChanges();

                return RedirectToAction("DangNhap");                                                                                                                                                             
                
            }
            return this.DangKi();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View("DangNhap");
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            if(String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải Nhập Tên Đăng Nhập";
            }
            else if(String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập Mật Khẩu";
            }
            else
            {
                User kh = db.Users.SingleOrDefault(n => n.UserName == tendn && n.Password == matkhau);
                if (kh != null)
                {
                    //ViewBag.ThongBao = "Chúc Mừng Đăng Nhập Thành Công";
                    Session["UserName"] = kh;
                    return RedirectToAction("Index", "Site");
                }
                else
                    ViewBag.ThongBao = "Tên Đăng Nhập Hoặc Mật Khẩu Không Đúng";
            }
            return View();
        }
       
       
        
    }
}