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
    public class PostsController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();

        // GET: Admin/Posts
        public ActionResult Index(int? page)
        {

            var posts = db.Posts.Select(s => s);
            posts = posts.OrderByDescending(s => s.CreateDate);
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Admin/Posts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostID,Title,SContents,Contents,Thumb,Pulished,Alias,CreateDate,Author,AccountID,Tags,CatID,isHot,isNewfeed,MetaDesc,MetaKey,Views")] Post post)
        {
            if (ModelState.IsValid)
            {
                var f = Request.Files["ImageFile"];
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(f.FileName);
                    string UploadPath = Server.MapPath("~/Content/images/" + "thumb_post_" + FileName);
                    f.SaveAs(UploadPath);
                    post.Thumb = FileName;
                    // Chuyển hướng đến hành động Create của Controller Image và truyền ID của sản phẩm

                }
                db.Posts.Add(post);
                db.SaveChanges();

                return RedirectToAction("Index",1);
            }

            return View(post);
        }
            // GET: Admin/Posts/Edit/5
            public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostID,Title,SContents,Contents,Thumb,Pulished,Alias,CreateDate,Author,AccountID,Tags,CatID,isHot,isNewfeed,MetaDesc,MetaKey,Views")] Post post, HttpPostedFileBase ImageFile)
        {

            if (ModelState.IsValid)
            {
                var f = Request.Files["ImageFile"];
                if (f != null && f.ContentLength > 0)
                {
                    string FileName = System.IO.Path.GetFileName(f.FileName);
                    string UploadPath = Server.MapPath("~/Content/images/" + "thumb_post_" + FileName);
                    f.SaveAs(UploadPath);
                    post.Thumb = FileName;

                }
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index",1);
            }

            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index",1);
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
