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
using QLBQA.Filter;

namespace QLBQA.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductsController : Controller
    {

        private QLBQA_DB db = new QLBQA_DB();
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
                var products = db.Products.Include(a => a.Category).Where(a => a.CatID == id).ToList();
                return PartialView("_ProductList", products);
            }



        }
        // GET: Admin/Products
        public ActionResult Index(int? page, string searchString, string currentFilter)
        {
            //return View(db.Customers.ToList());

            var cats = db.Categories.ToList();
            ViewBag.Cats = cats;
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
                products = products.Where(s => s.ProductName.Contains(searchString));
            }
            products = products.OrderBy(s => s.ProductID);
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }
        // GET: Admin/Products/Details/5
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
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,ShortDesc,Description,CatID,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitsInStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                var f = Request.Files["ImageFile"];
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(f.FileName);
                    string UploadPath = Server.MapPath("~/Content/images/" + "thumb" + "_" + FileName);
                    f.SaveAs(UploadPath);
                    product.Thumb = FileName;
                    // Chuyển hướng đến hành động Create của Controller Image và truyền ID của sản phẩm

                }
                db.Products.Add(product);
                db.SaveChanges();
                int productId = product.ProductID;
                string productName = product.ProductName;

                // Lưu ProductID và ProductName vào TempData để truyền sang trang Create của Images
                TempData["ProductId"] = productId;
                TempData["ProductName"] = productName;

                return RedirectToAction("Create", "Images");
            }

            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", product.CatID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", product.CatID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,ShortDesc,Description,CatID,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitsInStock")] Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var a = product;
                // Kiểm tra xem có hình ảnh mới được tải lên không
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Xóa hình ảnh cũ
                    string oldImagePath = Server.MapPath("~/Content/images/thumb_" + product.Thumb);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Lưu hình ảnh mới
                    string FileName = System.IO.Path.GetFileName(ImageFile.FileName);
                    string UploadPath = Server.MapPath("~/Content/images/" + "thumb" + "_" + FileName);
                    ImageFile.SaveAs(UploadPath);
                    product.Thumb = FileName;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                int productId = product.ProductID;
                string productName = product.ProductName;

                // Lưu ProductID và ProductName vào TempData để truyền sang trang Edit của Images
                TempData["ProductId"] = productId;
                TempData["ProductName"] = productName;
                return RedirectToAction("Edit", "Images");
            }
            ViewBag.CatID = new SelectList(db.Categories, "CatID", "CatName", product.CatID);
            return View(product);
        }



        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
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
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrMsg"] = "Không thể xóa sản phẩm này";
                return RedirectToAction("Index");

            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
