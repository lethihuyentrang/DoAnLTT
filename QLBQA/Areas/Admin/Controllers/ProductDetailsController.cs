using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBQA.Filter;
using QLBQA.Models;

namespace QLBQA.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductDetailsController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();

        // GET: Admin/ProductDetails
        public ActionResult Index()
        {
            var productDetails = db.ProductDetails.Include(p => p.Color).Include(p => p.Product).Include(p => p.Size);
            return View(productDetails.ToList());
        }

        // GET: Admin/ProductDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductDetail productDetail = db.ProductDetails.Find(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            return View(productDetail);
        }

        // GET: Admin/ProductDetails/Create
        public ActionResult Create()
        {
            ViewBag.Colors = db.Colors.OrderBy(c => c.ColorCode);
            ViewBag.Products = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.Sizes = db.Sizes;
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");

            return View();
        }
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<ProductDetail> productDetails)
        {
            if (ModelState.IsValid)
            {
               

                foreach (var productDetail in productDetails)
                {
                    db.ProductDetails.Add(productDetail);
                    db.SaveChanges();
                    
                }
                int addedProductIds = productDetails[0].ProductID;
                // Trả về danh sách ProductDetailID như một phản hồi JSON
                return Json(addedProductIds);
            }
            else
            {
                Console.WriteLine("Loi khong chuyen den Edit duoc");
                // Nếu ModelState không hợp lệ, hiển thị lại view Create với dữ liệu đã nhập và thông báo lỗi
                ViewBag.Colors = db.Colors;
                ViewBag.Products = new SelectList(db.Products, "ProductID", "ProductName");
                ViewBag.Sizes = db.Sizes;
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");

                return View(productDetails);
            }
           
        }

 

        // GET: Admin/ProductDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productDetail = db.ProductDetails.Where(pd => pd.ProductID == id).ToList();
            if (productDetail == null)
            {
                return HttpNotFound();
            }

            return View(productDetail);
        }

        // POST: Admin/ProductDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ProductDetailID,ProductID,ColorID,SizeID,Quantity")] ProductDetail productDetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(productDetail).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "ColorName", productDetail.ColorID);
        //    ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productDetail.ProductID);
        //    ViewBag.SizeID = new SelectList(db.Sizes, "SizeID", "SizeName", productDetail.SizeID);
        //    return RedirectToAction("Index", "Products");
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(List<ProductDetail> productDetails)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            foreach (var productDetail in productDetails)
        //            {
        //                // Cập nhật hoặc thực hiện các thay đổi cần thiết cho mỗi ProductDetail
        //                db.Entry(productDetail).State = EntityState.Modified;
        //            }
        //            // Lưu thay đổi vào cơ sở dữ liệu
        //            db.SaveChanges();
        //            // Trả về kết quả thành công
        //            return Json(new { success = true });
        //        }
        //        catch (Exception ex)
        //        {
        //            // Xử lý ngoại lệ nếu có
        //            return Json(new { success = false, errorMessage = ex.Message });
        //        }
        //    }
        //    // Trả về kết quả thất bại nếu ModelState không hợp lệ
        //    return Json(new { success = false, errorMessage = "Dữ liệu không hợp lệ" });
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(List<ProductDetail> productDetails)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var productDetail in productDetails)
                    {
                        db.Entry(productDetail).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    return Json(new { success = true }); // Trả về kết quả thành công dưới dạng JSON
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có
                    return Json(new { success = false, errorMessage = ex.Message }); // Trả về thông báo lỗi dưới dạng JSON
                }
            }
            // Nếu ModelState không hợp lệ, trả về view chỉnh sửa với model
            return View(productDetails);
        }





        // GET: Admin/ProductDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductDetail productDetail = db.ProductDetails.Find(id);
            if (productDetail == null)
            {
                return HttpNotFound();
            }
            return View(productDetail);
        }

        // POST: Admin/ProductDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            ProductDetail productDetail = db.ProductDetails.Find(id);
            var pID = productDetail.ProductID;
            db.ProductDetails.Remove(productDetail);
            db.SaveChanges();
            //TempData["ProductId"] = productDetail.Product.ProductID;
            //TempData["ProductName"] = productDetail.Product.ProductName;
            return RedirectToAction("Edit2", new { id  = pID});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Edit2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Lấy danh sách chi tiết sản phẩm với ProductID tương ứng
            var productDetails = db.ProductDetails.Where(pd => pd.ProductID == id).ToList();
            TempData["ProductId"] = id;
            TempData["ProductName"] = db.Products.Where(pd => pd.ProductID == id).Select(p=>p.ProductName).FirstOrDefault();
            //// Kiểm tra xem danh sách có trống không
            //if (productDetails == null || productDetails.Count == 0)
            //{
            //    // Nếu danh sách trống, trả về trạng thái HttpNotFound
            //    return HttpNotFound();
            //}

            // Nếu danh sách không trống, trả về view "Edit2" với danh sách chi tiết sản phẩm
            return View(productDetails);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2(List<ProductDetail> productDetails)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var productDetail in productDetails)
                    {
                        db.Entry(productDetail).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    
                    return Json(new { success = true }); 
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errorMessage = ex.Message }); 
                }
            }
            return View(productDetails);
        }
        public ActionResult Create2()
        {   

            ViewBag.Colors = db.Colors.OrderBy(c => c.ColorCode);
            ViewBag.Products = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.Sizes = db.Sizes;
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(List<ProductDetail> productDetails)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    foreach (var productDetail in productDetails)
                    {
                        // Kiểm tra xem có bản ghi trùng lặp không
                        var existingRecord = db.ProductDetails.FirstOrDefault(
                            pd => pd.ProductID == productDetail.ProductID &&
                                  pd.ColorID == productDetail.ColorID &&
                                  pd.SizeID == productDetail.SizeID);

                        if (existingRecord != null)
                        {
                            continue;
                        }

                        // Nếu không có bản ghi trùng lặp, thêm mới vào cơ sở dữ liệu
                        db.ProductDetails.Add(productDetail);
                        db.SaveChanges();
                    }

                    int addedProductIds = productDetails[0].ProductID;
                    // Trả về danh sách ProductDetailID như một phản hồi JSON
                    return Json(addedProductIds);
                }
                catch (Exception ex)
                {
                    // Xử lý các ngoại lệ khác nếu cần
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo sản phẩm. Vui lòng thử lại sau.");
                    // Hiển thị lại view Create với dữ liệu đã nhập và thông báo lỗi
                    ViewBag.Colors = db.Colors;
                    ViewBag.Products = new SelectList(db.Products, "ProductID", "ProductName");
                    ViewBag.Sizes = db.Sizes;
                    ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");

                    return View(productDetails);
                }
            }
            else
            {
                // Xử lý trường hợp ModelState không hợp lệ (nếu cần)
                ViewBag.Colors = db.Colors;
                ViewBag.Products = new SelectList(db.Products, "ProductID", "ProductName");
                ViewBag.Sizes = db.Sizes;
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");

                return View(productDetails);
            }
        }

        public ActionResult CancelDelete(int? id)
        {
            Product product = db.Products.Find(id);

            TempData["ProductId"] = id;
            TempData["ProductName"] = product.ProductName;
            return RedirectToAction("Edit2", new { id = id });
        }

        public ActionResult CancelCreate(int? id)
        {
            Product product = db.Products.Find(id);
            TempData["ProductId"] = id;
            TempData["ProductName"] = product.ProductName;
            return RedirectToAction("Edit2", new { id = id});

        }

    }
}
