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
    public class BlogController : Controller
    {
        // GET: Blog
        private QLBQA_DB db = new QLBQA_DB();
        public ActionResult Index(int? page)
        {

            var posts = db.Posts.Where(p => p.Pulished == true).Select(s => s);
            posts = posts.OrderByDescending(s => s.CreateDate); 
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int? id)
        {
            ViewBag.HotNews = db.Posts.Where(p => p.isHot == true && p.Pulished == true).Take(5).ToList();
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
    }
}