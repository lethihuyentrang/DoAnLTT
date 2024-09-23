using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using QLBQA.Filter;
using QLBQA.Models;

namespace QLBQA.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ThongKeController : Controller
    {
        // GET: Admin/ThongKe
        private QLBQA_DB db = new QLBQA_DB();
        public ActionResult Index()
        {
   
            return View();
        }

        public ActionResult Process(ThongKe tk)
        {
            List<OrderDetail> oDetails = new List<OrderDetail>();
            List<ProductDetail> pDetails = new List<ProductDetail>();
            List<Color> Colors = new List<Color>();
            List<Size> Sizes = new List<Size>();
            if (tk.ProductID == null && tk.DateStart != null && tk.DateEnd != null)
            {
                oDetails = db.OrderDetails.Where(x => x.Order.OrderDate >= tk.DateStart && x.Order.OrderDate <= tk.DateEnd).ToList();

            }
            else if (tk.ProductID != null && tk.DateStart == null && tk.DateEnd == null)
            {
                pDetails = db.ProductDetails.Where(x => x.ProductID == tk.ProductID).ToList();
                foreach (var item in pDetails)
                {
                    var oDetail = db.OrderDetails.Where(x => x.ProductDetailID == item.ProductDetailID).ToList();
                    if (oDetail != null)
                        oDetails.AddRange(oDetail);
                }


            }
            else if (tk.ProductID != null && tk.DateStart != null && tk.DateEnd != null)
            {
                pDetails = db.ProductDetails.Where(x => x.ProductID == tk.ProductID).ToList();
                foreach (var item in pDetails)
                {
                    var oDetail = db.OrderDetails.Where(x => x.ProductDetailID == item.ProductDetailID && x.Order.OrderDate >= tk.DateStart && x.Order.OrderDate <= tk.DateEnd).ToList();
                    if (oDetail != null)
                        oDetails.AddRange(oDetail);
                }

            }
            else
            {
                ViewBag.ErrMsg = "Vui lòng nhập dữ liệu hợp lệ";
                return RedirectToAction("Index");
            }
            foreach (var item in oDetails)
            {
                ProductDetail pd = db.ProductDetails.Where(x => x.ProductDetailID == item.ProductDetailID).FirstOrDefault();

                pDetails.Add(pd);
            }
            foreach (var item in pDetails)
            {
                Color cl = db.Colors.Where(x => x.ColorID == item.ColorID).FirstOrDefault();
                Size s = db.Sizes.Where(x => x.SizeID == item.SizeID).FirstOrDefault();
                Colors.Add(cl);
                Sizes.Add(s);
            }
            int? sum = 0;
            foreach(var item in oDetails)
            {
                var pDetail = pDetails.Where(x => x.ProductDetailID == item.ProductDetailID).FirstOrDefault();
                if (pDetail.Product.Discount > 0)
                {
                    sum += pDetail.Product.Discount * item.Quantity;
                }
                else
                {
                    sum += pDetail.Product.Price * item.Quantity;
                }
            }
            ViewBag.ProductDetails = pDetails;
            ViewBag.Colors = Colors;
            ViewBag.Sizes = Sizes;
            TempData["Sum"] = Convert.ToInt32(sum);
            TempData["OrderDetails"] = oDetails;
            return PartialView("_ThongKePartialView", oDetails);
            //return RedirectToAction("Index");
        }
    }
}