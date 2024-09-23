using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBQA.Models;

namespace QLBQA.Controllers
{
    public class PageController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();
        // GET: Page
        public ActionResult Index()
        {
            return View();
        }
        // GET: Posts/Details/5
        //public ActionResult Details(string alias)
        //{
        //    if (alias == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Page page = db.Pages.Where(p=>p.Alias==alias).FirstOrDefault();
        //    if (page == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(page);
        //}
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Page page = db.Pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            return View(page);
        }
    }
}