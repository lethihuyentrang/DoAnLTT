using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using QLBQA.Filter;
using QLBQA.Models;

namespace QLBQA.Areas.Admin.Controllers
{
    public class AccountsController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();


        //public ActionResult Index()
        //{
        //    ViewBag.QuyenTruyCap = new SelectList(db.Roles, "RoleID", "RoleName");
        //    var QLBQA_context = db.Accounts.Include(a => a.Role);
        //    return View(QLBQA_context.ToList());
        //}
        [AdminAuthorize]
        public ActionResult Index()
        {
            var roles = db.Roles.ToList();
            ViewBag.Roles = roles;
            var QLBQA_context = db.Accounts.Include(a => a.Role);
            return View(QLBQA_context.ToList());
        }

        [AdminAuthorize]
        private string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }

        }
        [AdminAuthorize]
        // GET: Admin/Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }
        [AdminAuthorize]
        // GET: Admin/Accounts/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName");
            return View();
        }
        [AdminAuthorize]
        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountID,Phone,Email,Password,Salt,Active,FullName,RoleID,LastLogin,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", account.RoleID);
            return View(account);
        }
        [AdminAuthorize]
        // GET: Admin/Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", account.RoleID);
            return View(account);
        }
        [AdminAuthorize]
        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountID,Phone,Email,Password,Salt,Active,FullName,RoleID,LastLogin,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", account.RoleID);
            return View(account);
        }

        [AdminAuthorize]// GET: Admin/Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }
        [AdminAuthorize]
        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [AdminAuthorize]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //public ActionResult SelectFollowingRole(int id)
        //{   
        //    if(id==0)
        //    {
        //        var accounts = db.Accounts.Include(a => a.Role).ToList();
        //        return View(accounts);
        //    }
        //    else
        //    {
        //        var accounts = db.Accounts.Include(a => a.Role).Where(a => a.RoleID == id).ToList();
        //        return PartialView("_AccountList", accounts);
        //    }

        //}
        [AdminAuthorize]
        public ActionResult SelectFollowingRole(int? id)
        {
            var accounts = id.HasValue ? db.Accounts.Include(a => a.Role).Where(a => a.RoleID == id).ToList() : db.Accounts.Include(a => a.Role).ToList();
            return PartialView("_AccountList", accounts);
        }



    }
}
