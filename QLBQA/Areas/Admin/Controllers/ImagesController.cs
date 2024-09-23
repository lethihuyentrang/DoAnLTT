    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using QLBQA.Models;
    using System.IO;
    using QLBQA.Filter;

namespace QLBQA.Areas.Admin.Controllers
    {
    [AdminAuthorize]
    public class ImagesController : Controller
        {
            private QLBQA_DB db = new QLBQA_DB();

            // GET: Admin/Images
            public ActionResult Index()
            {
                var images = db.Images.Include(i => i.Product);
                return View(images.ToList());
            }

            // GET: Admin/Images/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Image image = db.Images.Find(id);
                if (image == null)
                {
                    return HttpNotFound();
                }
                return View(image);
            }

            // GET: Admin/Images/Create
            public ActionResult Create()
            {
                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
                return View();
            }

            // POST: Admin/Images/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "ImageID,Description,ProductID")] Image image, IEnumerable<HttpPostedFileBase> ImageFiles)
            {
                if (ModelState.IsValid)
                {
                    if (ImageFiles != null && ImageFiles.Count() > 0)
                    {
                        foreach (var file in ImageFiles)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                string FileName = System.IO.Path.GetFileName(file.FileName);
                                string UploadPath = Server.MapPath("~/Content/images/" +  FileName);
                                file.SaveAs(UploadPath);

                                // Tạo mới đối tượng Image và lưu thông tin vào csdl
                                Image newImage = new Image();
                                newImage.Path = FileName;
                                newImage.Description = image.Description;
                                newImage.ProductID = image.ProductID;
                                db.Images.Add(newImage);

                                Console.WriteLine(newImage);
                            }
                        }
                        db.SaveChanges();
                    }
                    int productId = image.ProductID;
                    string productName = db.Products.Where(p => p.ProductID == productId).Select(p => p.ProductName).FirstOrDefault();

                    TempData["ProductId"] = productId;
                    TempData["ProductName"] = productName;
                    return RedirectToAction("Create", "ProductDetails");
                }

                ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", image.ProductID);
                return View(image);
            }


            // GET: Admin/Images/Edit/5
            //public ActionResult Edit(int? id)
            //{
            //    if (id == null)
            //    {
            //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //    }
            //    Image image = db.Images.Find(id);
            //    if (image == null)
            //    {
            //        return HttpNotFound();
            //    }
            //    ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", image.ProductID);
            //    return View(image);
            //}
            public ActionResult Edit()
            {
            

                // Truy cập productId từ TempData
                int productId = (int)TempData["ProductId"];
                var productName = TempData["ProductName"];

                TempData["ProductId"] = productId;


            // Lấy danh sách hình ảnh có ProductID tương ứng
                var images = db.Images.Where(pd => pd.ProductID == productId).ToList();

                if (images == null)
                {
                    return HttpNotFound();
                }

                return View(images);
            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(List<Image> Images)
        {   


                if (ModelState.IsValid)
                {
                    try
                    {

                        foreach (var image in Images)
                        {

                            if (!string.IsNullOrEmpty(image.Path))
                            {
                                string oldImagePath = Server.MapPath("~/Content/images/" + image.Path);
                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }

                                string sourceImagePath = "D:/Tai lieu do an/Image_Edited/" + image.Path; // Đường dẫn đến tệp ảnh nguồn
                                string destinationImagePath = Server.MapPath("~/Content/images/" + image.Path); // Đường dẫn đến thư mục lưu ảnh mới
                                System.IO.File.Copy(sourceImagePath, destinationImagePath); // Sao chép tệp ảnh từ nguồn đến đích
                            }

                            db.Entry(image).State = EntityState.Modified;
                        }
                        db.SaveChanges();

                        return RedirectToAction("Index", "Products");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Lỗi khi lưu hình ảnh: " + ex.Message);
                    }
                }
                return View(Images);
            
   
        }



       

        // GET: Admin/Images/Delete/5
        public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Image image = db.Images.Find(id);
                if (image == null)
                {
                    return HttpNotFound();
                }
                return View(image);
            }

            // POST: Admin/Images/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                Image image = db.Images.Find(id);
                Product product = db.Products.Find(image.ProductID);
                db.Images.Remove(image);
                db.SaveChanges();
                TempData["ProductId"] = image.ProductID;
                TempData["ProductName"] = product.ProductName;
            return RedirectToAction("Edit");
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }

        public ActionResult Create2()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: Admin/Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2([Bind(Include = "ImageID,Description,ProductID")] Image image, IEnumerable<HttpPostedFileBase> ImageFiles)
        {
            if (ModelState.IsValid)
            {
                if (ImageFiles != null && ImageFiles.Count() > 0)
                {
                    foreach (var file in ImageFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string FileName = System.IO.Path.GetFileName(file.FileName);
                            string UploadPath = Server.MapPath("~/Content/images/" + FileName);
                            file.SaveAs(UploadPath);

                            // Tạo mới đối tượng Image và lưu thông tin vào csdl
                            Image newImage = new Image();
                            newImage.Path = FileName;
                            newImage.Description = image.Description;
                            newImage.ProductID = image.ProductID;
                            db.Images.Add(newImage);

                            Console.WriteLine(newImage);
                        }
                    }
                    db.SaveChanges();
                }
                int productId = image.ProductID;
                string productName = db.Products.Where(p => p.ProductID == productId).Select(p => p.ProductName).FirstOrDefault();

                TempData["ProductId"] = productId;
                TempData["ProductName"] = productName;
                return RedirectToAction("Edit", "Images");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", image.ProductID);
            return View(image);
        }

        public ActionResult CancelDelete(int? id)
        {
            Image image = db.Images.Find(id);
            Product product = db.Products.Find(image.ProductID);
           
            TempData["ProductId"] = image.ProductID;
            TempData["ProductName"] = product.ProductName;
            return RedirectToAction("Edit");
        }
        public ActionResult CancelCreate(int? id)
        {
            Product product = db.Products.Find(id);
            TempData["ProductId"] = id;
            TempData["ProductName"] = product.ProductName;
            return RedirectToAction("Edit");
        }

    }
}
