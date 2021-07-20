using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopBon.Models
{
    public class GioHang
    {
        dbShopBonDataContext data = new dbShopBonDataContext();
        public int iMaSanPham { get; set; }
        public string sName { get; set; }
        public string sImg { get; set; }
        public Double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public Double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        public GioHang(int MaSanPham)
        {
            iMaSanPham = MaSanPham;
            Product product = data.Products.Single(n => n.Id == iMaSanPham);
            sName = product.Name;
            sImg = product.Img;
            dDonGia = double.Parse(product.PriceBuy.ToString());
            iSoLuong = 1;
        }
    }
}