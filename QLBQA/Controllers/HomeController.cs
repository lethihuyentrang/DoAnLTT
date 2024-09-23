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
    public class HomeController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();
        public ActionResult Index()
        {
            ViewBag.Category = db.Categories.Where(p => p.Published == true).ToList();
            ViewBag.Product = db.Products.Where(p=>p.Active==true).ToList();
            ViewBag.Post = db.Posts.OrderByDescending(p=>p.CreateDate).Where(p => p.Pulished == true).ToList();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}