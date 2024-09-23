using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBQA.Models;
using PagedList;
namespace QLBQA.Controllers
{
    public class ProductController : Controller
    {

        private  QLBQA_DB db = new QLBQA_DB();

        public ActionResult Index(int? page, string searchString, string currentFilter)
        {
            //return View(db.Customers.ToList());
            var colors = db.Colors.ToList();
            ViewBag.Colors = colors;
            var cats = db.Categories.ToList();
            ViewBag.Cats = cats;
            ViewBag.HotProduct = db.Products.Where(a => a.BestSellers == true && a.Active == true).ToList();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var products = db.Products.Include(a => a.Category).Select(s => s);
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.ProductName.Contains(searchString) && s.Active == true);
            }
            products = products.OrderBy(s => s.ProductID);
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }
        //public ActionResult Index()
        //{
        //    ViewBag.Colors = db.Colors.Select(c => c);
        //    var products = db.Products.Where(p => p.Active == true).Select(p => p);
        //    return View(products);
        //}
        public ActionResult ReloadPage()
        {
            // Trả về một đoạn mã JavaScript để tải lại trang
            return Content("<script>window.location.reload();</script>");
        }
        public ActionResult SelectFollowingCat(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("ReloadPage");
            }
            else
            {
                var colors = db.ProductDetails.Where(c => c.ProductID == id).Select(c => c.Color).ToList();
                var distinctColors = colors.Distinct().ToList();
                ViewBag.Colors = distinctColors;
                var products = db.Products.Include(a => a.Category).Where(a => a.CatID == id && a.Active == true).ToList();
                return PartialView("_ProductListUser", products);
            }
        }
        //public ActionResult ListProductByCat(int? id)
        //{
        //    ViewBag.CatSelected = db.Categories.Where(c => c.CatID == id).Select(c=>c.CatID);
        //    return View("Index");
        //}
        public ActionResult FilterProduct(FilterProduct filters)
        {
            var products = db.Products.Where(p => p.Active == true).ToList();
       
            if (filters.isDiscount)
            {
                products = products.Where(p => p.Discount > 0).ToList();               
            }
            if(filters.isHot)
            {
                products = products.Where(p => p.BestSellers == true).ToList();
            }
            if (filters.isInStock)
            {
                products = products.Where(p => p.UnitsInStock > 0).ToList();
            }
            if (filters.isLowPrice)
            {

                products = products.Where(p => (p.Discount > 0 && p.Discount <= 200000) || p.Price <= 200000).ToList();

            }
            if (filters.isMediumPrice)
            {
                products = products.Where(p => (p.Discount > 200000 && p.Discount <= 500000) || ( p.Discount == 0 && p.Price > 200000 && p.Price <= 500000)).ToList();

            }
            if (filters.isHighPrice)
            {
                products = products.Where(p => (p.Discount > 500000 && p.Discount <= 1000000) || ( p.Discount == 0 && p.Price > 500000 && p.Price <= 1000000)).ToList();

            }
            if (filters.isVeryHighPrice)
            {
                products = products.Where(p => (p.Discount > 1000000) || (p.Discount == 0 && p.Price > 1000000)).ToList();

            }
            if (filters.isSSize)
            {
                var listSize = db.ProductDetails
                                .Where(p => p.SizeID == 1)
                                .Select(p => p.ProductID)
                                .ToList();

                products = products.Where(p => listSize.Contains(p.ProductID)).ToList();
            }
            if (filters.isMSize)
            {
                var listSize = db.ProductDetails
                                .Where(p => p.SizeID == 2)
                                .Select(p => p.ProductID)
                                .ToList();

                products = products.Where(p => listSize.Contains(p.ProductID)).ToList();
            }

            if (filters.isLSize)
            {
                var listSize = db.ProductDetails
                                .Where(p => p.SizeID == 3)
                                .Select(p => p.ProductID)
                                .ToList();

                products = products.Where(p => listSize.Contains(p.ProductID)).ToList();
            }
            if (filters.isXLSize)
            {
                var listSize = db.ProductDetails
                                .Where(p => p.SizeID == 4)
                                .Select(p => p.ProductID)
                                .ToList();

                products = products.Where(p => listSize.Contains(p.ProductID)).ToList();
            }
            if (filters.is2XLSize)
            {
                var listSize = db.ProductDetails
                                .Where(p => p.SizeID == 5)
                                .Select(p => p.ProductID)
                                .ToList();

                products = products.Where(p => listSize.Contains(p.ProductID)).ToList();
            }
            if (filters.is3XLSize)
            {
                var listSize = db.ProductDetails
                                .Where(p => p.SizeID == 6)
                                .Select(p => p.ProductID)
                                .ToList();

                products = products.Where(p => listSize.Contains(p.ProductID)).ToList();
            }
            var page = filters.page;
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            var pagedProducts = products.OrderBy(p => p.ProductID).Where(a=> a.Active == true).ToPagedList(pageNumber, pageSize);
            ViewBag.Filter = filters;
            if (filters.CatID == 0)
            {
                return PartialView("_ProductListUser", pagedProducts);
            }
            else
            {
                pagedProducts = products.OrderBy(p => p.ProductID).Where(a => a.CatID == filters.CatID && a.Active == true).ToPagedList(pageNumber, pageSize);
                return PartialView("_ProductListUser", pagedProducts);
            }        
            // Trả về partial view chứa danh sách sản phẩm đã phân trang
            return PartialView("_ProductListUser", pagedProducts);

        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var colors = db.ProductDetails.Where(c => c.ProductID == id).Select(c => c.Color).ToList();
            var distinctColors = colors.Distinct().ToList();
            ViewBag.Colors = distinctColors;
            var sizes = db.ProductDetails.Where(c => c.ProductID == id).Select(c => c.Size).ToList();
            var distinctSizes = sizes.Distinct().ToList();
            ViewBag.Sizes = distinctSizes;
            var images = db.Products
                .Where(c => c.ProductID == id)
                .Select(c => c.Images)
                .ToList();
            ViewBag.Images = images;
            ViewBag.RelatedProduct = db.Products.Include(a => a.Category).Where(a => a.CatID == product.CatID && a.Active == true && a.ProductID !=id).ToList();
            //ViewBag.ProductDetailID = db.ProductDetails.Where(x=>x.ProductID==id).Select(x=>x.ProductDetailID).FirstOrDefault();
            //TempData["message"] = ViewBag.Message;
            return View(product);
        }
    }
}