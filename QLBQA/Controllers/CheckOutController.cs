using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBQA.Models;
using QLBQA.Extension;
using System.Configuration;
using DemoVNPay.Others;

namespace QLBQA.Controllers
{
    public class CheckOutController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();
        // GET: CheckOut
        public ActionResult Index(CheckOut muaHang)
        {
            ViewBag.GioHang = Session["GioHang"];
            return View(muaHang);
        }
        //[HttpPost]
        public ActionResult CODPayMent(CheckOut muaHang)
        {

            var cart = Session["GioHang"] as List<CartItem>;
            string taikhoanID = Session["CustomerId"] as string;
            CheckOut model = new CheckOut();
            if (taikhoanID != null)
            {
                int customerid = int.Parse(taikhoanID);
                var khachhang = db.Customers.SingleOrDefault(p => p.CustomerID == customerid);
                model.CustomerId = khachhang.CustomerID;
                model.FullName = khachhang.FullName;
                model.Phone = khachhang.Phone;
                model.Email = khachhang.Email;
                model.Address = khachhang.Address;

                khachhang.LocationID = muaHang.TinhThanh;
                khachhang.District = muaHang.QuanHuyen;
                khachhang.Ward = muaHang.PhuongXa;
                khachhang.Address = muaHang.Address;
                //Cập nhật lại thông tin cho khách hàng
                db.Entry(khachhang).State = EntityState.Modified;
                db.SaveChanges();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    Order donhang = new Order();
                    donhang.CustomerID = model.CustomerId;
                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusID = 1;
                    donhang.Deleted = false;
                    donhang.Paid = false;
                    donhang.Note = Ultilities.StripHTML(model.Note);
                    donhang.Total = Convert.ToInt32(muaHang.TotalMoney);
                    db.Orders.Add(donhang);
                    db.SaveChanges();

                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderID = donhang.OrderID;
                        orderDetail.ProductDetailID = item.productDetail.ProductDetailID;
                        orderDetail.Quantity = item.amount;

                        ProductDetail productDetail = db.ProductDetails.Where(p => p.ProductDetailID == item.productDetail.ProductDetailID).FirstOrDefault();
                        productDetail.Quantity -= item.amount; // cap nhat so luong
                        //if (productDetail.Product.Discount > 0)
                        //{
                        //    orderDetail.Total = orderDetail.Quantity * productDetail.Product.Discount;
                        //}
                        //else
                        //{
                        //    orderDetail.Total = orderDetail.Quantity * productDetail.Product.Price;
                        //}
                        db.Entry(productDetail).State = EntityState.Modified;
                        db.OrderDetails.Add(orderDetail);
                    }
                    db.SaveChanges();
                    Session["GioHang"] = null;
                    Session["CountCart"] = 0;
                    TempData["Success"] = "Đặt hàng thành công";
                    return Json(new { Url = Url.Action("SuccessPayment", "CheckOut") });
                }
            }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = false });
        }
        public ActionResult SuccessPayment()
        {
            return View();
        }
        public ActionResult ErrorPayment()
        {
            return View();
        }
        public string Payment(CheckOut muaHang)
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            vnpay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            vnpay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            vnpay.AddRequestData("vnp_Amount", (muaHang.TotalMoney *100).ToString()); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
           
            vnpay.AddRequestData("vnp_BankCode", "VNBANK"); 
            

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            vnpay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            vnpay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            vnpay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang" ); //Thông tin mô tả nội dung thanh toán
            vnpay.AddRequestData("vnp_OrderType", "fashion"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
           
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = vnpay.CreateRequestUrl(url, hashSecret);
            //Cap nhat thong tin cho khach hang
            //var khachhang = db.Customers.SingleOrDefault(p => p.CustomerID == muaHang.CustomerId);
            //khachhang.LocationID = muaHang.TinhThanh;
            //khachhang.District = muaHang.QuanHuyen;
            //khachhang.Ward = muaHang.PhuongXa;
            //khachhang.Address = muaHang.Address;
            //db.Entry(khachhang).State = EntityState.Modified;
            //db.SaveChanges();
            //Session["GioHang"] = null;
            //Session["CountCart"] = 0;
            
            return paymentUrl;
        }   

        public ActionResult PaymentConfirm()
        {
            CheckOut req = TempData["req"] as CheckOut;
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();
                
                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        List<CartItem> cart = Session["GioHang"] as List<CartItem>;
                        if (cart != null)
                        {
                            Order order = new Order();
                            order.CustomerID = req.CustomerId;
                            order.OrderDate = DateTime.Now;
                            order.TransactStatusID = 1;
                            order.Deleted = false;
                            order.Paid = true;
                            order.Total = Convert.ToInt32(req.TotalMoney);
                            foreach (var item in cart)
                            {
                                ProductDetail productDetail = db.ProductDetails.Where(p => p.ProductDetailID == item.productDetail.ProductDetailID).FirstOrDefault();
                                productDetail.Quantity -= item.amount; // cap nhat so luong
                                db.Entry(productDetail).State = EntityState.Modified;
                            }

                            cart.ForEach(x => order.OrderDetails.Add(new OrderDetail
                            {
                                ProductDetailID = x.productDetail.ProductDetailID,
                                Quantity = x.amount,
                                //Total = Convert.ToInt32(x.TotalMoney())

                            }));

                            order.PaymentID = req.PaymentID;
                            order.PaymentDate = DateTime.Now;

                            Random rd = new Random();

                            db.Orders.Add(order);
                            Session["TemporaryOrder"] = req;

                            var strSanPham = "";
                            var thanhtien = decimal.Zero;
                            var TongTien = decimal.Zero;
                            foreach (var sp in cart)
                            {
                                strSanPham += "<tr>";
                                strSanPham += "<td>" + sp.productDetail.Product.ProductName + "</td>";
                                strSanPham += "<td>" + sp.amount + "</td>";
                                strSanPham += "<td>" + string.Format("{0:#,##0}", sp.TotalMoney()) + "</td>";
                                strSanPham += "</tr>";
                                thanhtien += Convert.ToInt32(sp.TotalMoney()) * sp.amount;
                            }
                            TongTien = thanhtien;
                            //var fullname = db.Customers.Where(x => x.CustomerID == req.CustomerId).FirstOrDefault();



                            var khachhang = db.Customers.SingleOrDefault(p => p.CustomerID == req.CustomerId);
                            khachhang.LocationID = req.TinhThanh;
                            khachhang.District = req.QuanHuyen;
                            khachhang.Ward = req.PhuongXa;
                            khachhang.Address = req.Address;
                            db.Entry(khachhang).State = EntityState.Modified;
                            db.SaveChanges();
                            Session["GioHang"] = null;
                            Session["CountCart"] = 0;
                            //return RedirectToAction("SuccessPayment");

                        }

                        TempData["Success"] = "Đặt hàng thành công";


                        return RedirectToAction("SuccessPayment","CheckOut");
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {

                        return RedirectToAction("ErrorPayment", "CheckOut");
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    TempData["Success"] = "Đặt hàng thất bại";

                    return RedirectToAction("ErrorPayment", "CheckOut");
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return RedirectToAction("ErrorPayment", "CheckOut");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckOut req)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            TempData["req"] = req;
            if (ModelState.IsValid)
            {
                var url = Payment(req);
                code = new { Success = true, Code = 1, Url = url };
            }
            return Json(code);
        }
       
    }
}