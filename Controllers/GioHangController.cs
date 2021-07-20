using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using ShopBon.Models;
using ShopBon.Others;

namespace ShopBon.Controllers
{
    public class GioHangController : Controller
    {
        dbShopBonDataContext data = new dbShopBonDataContext();
        // GET: GioHang
        public List<GioHang> LayGioHang()
        {
            List<GioHang> listgiohang = Session["GioHang"] as List<GioHang>;
            if (listgiohang == null)
            {
                listgiohang = new List<GioHang>();
                Session["GioHang"] = listgiohang;
            }
            return listgiohang;
        }
        public ActionResult ThemGioHang(int iMaSanPham, string strURL)
        {
            List<GioHang> listgiohang = LayGioHang();
            GioHang sanpham = listgiohang.Find(n => n.iMaSanPham == iMaSanPham);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMaSanPham);
                listgiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> listgiohang = Session["GioHang"] as List<GioHang>;
            if (listgiohang != null)
            {
                iTongSoLuong = listgiohang.Sum(n => n.iSoLuong);

            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> listgiohang = Session["GioHang"] as List<GioHang>;
            if (listgiohang != null)
            {
                iTongTien = listgiohang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        
        }
        public ActionResult GioHang()
        {
            List<GioHang> listgiohang = LayGioHang();
            if(listgiohang.Count == 0)
            {
                return RedirectToAction("Index", "Site");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(listgiohang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGioHang(int iMaSP)
        {
            List<GioHang> listGioHang = LayGioHang();
            GioHang sanpham = listGioHang.SingleOrDefault(n => n.iMaSanPham == iMaSP);
            if(sanpham!= null)
            {
                listGioHang.RemoveAll(n => n.iMaSanPham == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (listGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Site");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult UpdateGioHang(int iMaSP,FormCollection f)
        {
            List<GioHang> listGioHang = LayGioHang();
            GioHang sanpham = listGioHang.SingleOrDefault(n => n.iMaSanPham == iMaSP);
            if(sanpham!=null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> listGioHang = LayGioHang();
            listGioHang.Clear();
            return RedirectToAction("Index", "Site");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["UserName"] == null || Session["UserName"].ToString()=="")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if(Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Site");
            }
            List<GioHang> listGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(listGioHang);
        }
        public ActionResult DatHang(FormCollection collection)
        {
            Order ddh = new Order();
            User kh = (User)Session["UserName"];
            List<GioHang> gh = LayGioHang();
            ddh.UserId = kh.Id;
            ddh.Updated_At = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            ddh.DateOrder = DateTime.Parse(ngaygiao);

            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            data.Orders.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach(var item in gh)
            {
                Orderdetail ctdh = new Orderdetail();
                ctdh.OrderId = ddh.Id;
                ctdh.ProductId = item.iMaSanPham;
                ctdh.Soluong = item.iSoLuong;
                ctdh.PriceBuy =(double)(decimal) item.dDonGia;
                data.Orderdetails.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
        [HttpGet] 
        public ActionResult Payment()
        {
            
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOZ10820210718";
            string accessKey = "wxdlOijCj5X8KhFh";
            string serectkey = "O5T5rCDBOkIZ5EyhVlqyeFzdVn7QG2i6";
            
            string returnUrl = "https://localhost:44335/GioHang/DatHang";
           
            string notifyurl = "http://ba1adf48beba.ngrok.io/GioHang/DatHang"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
   
            string orderInfo = "test";
            string amount = "1000";
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        [HttpPost]
        public void SavePayment()
        {
            //cập nhật dữ liệu vào db
        }
    }
}