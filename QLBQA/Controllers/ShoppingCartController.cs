using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using QLBQA.Extension;
using QLBQA.Models;

namespace QLBQA.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart
        private QLBQA_DB db = new QLBQA_DB();
        public ActionResult Index()
        {
            List<int> productDetailIDs = new List<int>();
            var listGioHang = GioHang;
            return View(GioHang);
        }
        public List<CartItem> GioHang
        {
            get
            {
                List<CartItem> gh = Session["GioHang"] as List<CartItem>;
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        public ActionResult ReloadPage()
        {
            return Content("<script>window.location.reload();</script>");
        }
        [HttpPost]
        public ActionResult GetData(CartItem cartItem)
        {   
            if(cartItem.productDetail.ColorID==0 || cartItem.productDetail.SizeID == 0)
            {
                TempData["Error"] = "Bạn cần chọn màu sắc và kích cỡ";
                return Json(new { success = false });
            }
            List<CartItem> gioHang = GioHang;
            var productDetailID = db.ProductDetails
                .Where(p => p.ColorID == cartItem.productDetail.ColorID && p.SizeID == cartItem.productDetail.SizeID && p.ProductID == cartItem.productDetail.ProductID)
                .Select(p => p.ProductDetailID).FirstOrDefault();
            try
            {
                CartItem item = GioHang.SingleOrDefault(p => p.productDetail.ProductDetailID == productDetailID);
                if (item != null)
                {
                        item.amount += cartItem.amount;
                }
                else
                {
                    ProductDetail hh = db.ProductDetails.SingleOrDefault(p => p.ProductDetailID == productDetailID);
                    if(hh.Quantity <= 0)
                    {
                        TempData["Error"] = "Sản phẩm này đã hết hàng";
                        return Json(new { success = false });
                    }
                    item = new CartItem
                    {
                        amount = cartItem.amount,
                        productDetail = hh
                    };
                    gioHang.Add(item);
                }
                Session["GioHang"] = gioHang;
                TempData["Success"] = "Thêm vào giỏ hàng thành công";
                Session["CountCart"] = gioHang.Count();
                return Json(new { success = true });

            }
            catch
            {
                return RedirectToAction("Index", "Product");
            }
        }
        public ActionResult AddToCart(int productDetailID, int? amount)
        {
            List<CartItem> gioHang = GioHang;
            try
            {
                CartItem item = GioHang.SingleOrDefault(p => p.productDetail.ProductDetailID == productDetailID);
                if (item != null)
                {
                    if (amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    else
                    {
                        item.amount++;
                    }

                }
                else
                {
                    ProductDetail hh = db.ProductDetails.SingleOrDefault(p => p.ProductDetailID == productDetailID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        productDetail = hh
                    };
                    gioHang.Add(item);
                }
                Session["GioHang"] = gioHang;
                TempData["Success"] = "Thêm vào giỏ hàng thành công";

                return Json(new { success = true });
            }
            catch
            {
                return RedirectToAction("Index","Product");
            }    
        }

        [HttpPost]
        public ActionResult Remove(int ProductDetailID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.productDetail.ProductDetailID == ProductDetailID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                Session["GioHang"] = gioHang;
                TempData["Success"] = "Đã xóa khỏi giỏ hàng";
                Session["CountCart"] = gioHang.Count();
                return RedirectToAction("Index", "ShoppingCart");
            }
            catch
            {
                TempData["Error"] = "Xóa khỏi giỏ hàng thất bại";
                return RedirectToAction("Details", "Product");
            }
        }

        [HttpPost]
        public ActionResult UpdateCart(List<CartItem> cartItems)
        {
            try
            {   

                List<CartItem> gioHang = Session["GioHang"] as List<CartItem>;
                List<CartItem> gioHangMoi = new List<CartItem>();
                if (gioHang != null)
                {   
                    foreach(var cartitem in cartItems)
                    {
                        CartItem item = gioHang.SingleOrDefault(p => p.productDetail.ProductDetailID == cartitem.productDetail.ProductDetailID);
                        item.amount = cartitem.amount;
                        gioHangMoi.Add(item);
                    }

                }
                Session["GioHang"] = gioHangMoi;
                TempData["Success"] = "Cập nhật giỏ hàng thành công";
                return Json(new { success = true });
            }
            catch
            {
                TempData["Error"] = "Cập nhật giỏ hàng thất bại";
                return Json(new { success = false });
            }
        }

  
        public ActionResult CheckOut()
        {
            try
            {
                CheckOut checkOut = new CheckOut();
                var taikhoanID = Session["CustomerId"] as string;
                var CustomerID = int.Parse(taikhoanID);
                checkOut.CustomerId = CustomerID;
                Customer khachhang = db.Customers.Where(c => c.CustomerID == CustomerID).FirstOrDefault();
                checkOut.FullName = khachhang.FullName;
                checkOut.Email = khachhang.Email.Trim();
                checkOut.Phone = khachhang.Phone;
                checkOut.Address = khachhang.Address;
                checkOut.TinhThanh = khachhang.LocationID;
                checkOut.QuanHuyen = khachhang.District;
                checkOut.PhuongXa = khachhang.Ward;

                return RedirectToAction("Index", "CheckOut",checkOut);

            }
            catch
            {
                TempData["Error"] = "Thanh toán thất bại";
                return Json(new { success = false });
            }
        }
    }
}