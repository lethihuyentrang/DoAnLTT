using QLBQA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using QLBQA.Filter;

namespace QLBQA.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {

                
                int taikhoanID = Convert.ToInt32(Session["CustomerId"]);
                var admin = db.Customers.Where(x => x.CustomerID == taikhoanID).FirstOrDefault();
                return View(admin);
            

        }   

    }
}