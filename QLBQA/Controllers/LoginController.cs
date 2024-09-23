using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLBQA.Models;
using QLBQA.Extension;
namespace QLBQA.Controllers
{
    public class LoginController : Controller
    {

        private QLBQA_DB db = new QLBQA_DB();
        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(string phone, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var accounts = db.Accounts.ToList();
                    var customers = db.Customers.ToList();
                   
                    foreach (var item in accounts)
                    {
                        if (phone == item.Phone && password == item.Password)
                        {
                            if (item.RoleID == 1)
                            {
                                return RedirectToAction("Index", "Home", new { area = "Admin" });
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}