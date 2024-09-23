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
    public class OrdersController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();

        // GET: Admin/Orders
        public ActionResult Index(int? page, string searchString, string currentFilter)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var orders = db.Orders.Select(s => s).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(s => s.Customer.FullName.Contains(searchString)).ToList();
            }
            orders = orders.OrderByDescending(s => s.OrderDate).ToList();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));
            //var orders = db.Orders.Include(o => o.Customer).Include(o => o.TransactStatus);
            //return View(orders.ToList());
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
          
            var OrderDetails = db.OrderDetails.Where(x => x.OrderID == id).ToList();
            List<ProductDetail> ProductDetails = new List<ProductDetail>();
            foreach (var item in OrderDetails)
            {
                var ProductDetail = db.ProductDetails.Where(x => x.ProductDetailID == item.ProductDetailID).FirstOrDefault();
                ProductDetails.Add(ProductDetail);
            }
            var Colors = db.Colors.ToList();
            var Sizes = db.Sizes.ToList();
            ViewBag.Colors = Colors;
            ViewBag.Sizes = Sizes;
            ViewBag.ProductDetails = ProductDetails;         
            ViewBag.OrderDetails = OrderDetails;
            ViewBag.TransactStatus = new SelectList(db.TransactStatus, "TransactStatusID", "Status", order.TransactStatusID);
            return View(order);
        }

        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName");
            ViewBag.TransactStatusID = new SelectList(db.TransactStatus, "TransactStatusID", "Status");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,OrderDate,ShipDate,TransactStatusID,Deleted,Paid,PaymentDate,PaymentID,Total,Note")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", order.CustomerID);
            ViewBag.TransactStatusID = new SelectList(db.TransactStatus, "TransactStatusID", "Status", order.TransactStatusID);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", order.CustomerID);
            ViewBag.TransactStatusID = new SelectList(db.TransactStatus, "TransactStatusID", "Status", order.TransactStatusID);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,OrderDate,ShipDate,TransactStatusID,Deleted,Paid,PaymentDate,PaymentID,Total,Note")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", order.CustomerID);
            ViewBag.TransactStatusID = new SelectList(db.TransactStatus, "TransactStatusID", "Status", order.TransactStatusID);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public void UpdateStatus(int orderID, int[] status)
        {
            var order = db.Orders.Where(x => x.OrderID == orderID).FirstOrDefault();
            if (status[0] == 0)
                order.Paid = false;
            else
                order.Paid = true;
            order.TransactStatusID = status[1];
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            //return View("Index");
        }
    }
}
