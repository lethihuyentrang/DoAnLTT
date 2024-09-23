using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using QLBQA.Extension;
using QLBQA.Models;

namespace QLBQA.Controllers
{
    //[Authorize]
    public class AccountsController : Controller
    {
        private QLBQA_DB db = new QLBQA_DB();
        


        // GET: Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Role);
            return View(accounts.ToList());
        }

        // GET: Accounts/Details/5
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

        // GET: Accounts/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName");
            return View();
        }

        // POST: Accounts/Create
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

        // GET: Accounts/Edit/5
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

        // POST: Accounts/Edit/5
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

        // GET: Accounts/Delete/5
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

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
        public bool ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = db.Customers.SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachhang != null)
                {
                    return false;
                }
                    
                return true;
            }
            catch
            {

                return true;
            }
        }
        [HttpGet]
        public ActionResult Dashboard()
        {
            var taikhoanID = Session["CustomerId"] as string;
            if (!string.IsNullOrEmpty(taikhoanID) && int.TryParse(taikhoanID, out int customerId))
            {
                var khachhang = db.Customers.FirstOrDefault(x => x.CustomerID == customerId);
                if (khachhang != null)
                {
                    var orders = db.Orders
                        .Include(x => x.TransactStatus)
                        .Include(x=>x.OrderDetails)
                        .Where(x => x.CustomerID == khachhang.CustomerID)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();

                    
                    AccountEdit accountEdit = new AccountEdit();
                    accountEdit.CustomerId = khachhang.CustomerID;
                    accountEdit.Email = khachhang.Email;
                    accountEdit.Password = khachhang.Password;
                    accountEdit.Phone = khachhang.Phone;
                    accountEdit.Fullname = khachhang.FullName;
                    accountEdit.Birthday = khachhang.Birthday;
                    accountEdit.Address = khachhang.Address;
                    accountEdit.Salt = khachhang.Salt;

                    ViewBag.accountEdit = accountEdit;
                    ViewBag.Orders = orders;
                    List<OrderDetail> OrderDetails = new List<OrderDetail>();
                    foreach (var item in orders)
                    {
                        List<OrderDetail> OrderDetail = db.OrderDetails.Where(o => o.OrderID == item.OrderID).ToList();
                        OrderDetails.AddRange(OrderDetail);
                    }
                    ViewBag.OrderDetails = OrderDetails;


                    return View(khachhang);
                }
            }

         
            return RedirectToAction("Login");
        }


        public bool ValidateEmail (string Email)
        {
            try
            {
                var khachhang = db.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                {
                    return false;
                }                  
                return true;
            }
            catch
            {

                return true;
            }
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Ultilities.IsValidEmail(taikhoan.Email);
                    if (!isEmail) return View(taikhoan);
                    if (!ValidateEmail(taikhoan.Email))
                    {
                        TempData["Error"] = "Email đã tồn tại";
                        return RedirectToAction("Register");
                    }
                    if (!ValidatePhone(taikhoan.Phone))
                    {
                        TempData["Error"] = "Số điện thoại đã tồn tại";
                        return RedirectToAction("Register");
                    }
                    if (!Ultilities.IsValidPhoneNumber(taikhoan.Phone))
                    {
                        TempData["Error"] = "Số điện thoại không hợp lệ";
                        return RedirectToAction("Register");
                    }
                    if (!Ultilities.IsValidEmail2(taikhoan.Email))
                    {
                        TempData["Error"] = "Email không hợp lệ";
                        return RedirectToAction("Register");
                    }                 
                    string salt = Ultilities.GetRandomKey();
                    Customer khachhang = new Customer
                    {
                        FullName = taikhoan.Fullname,
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now
                    };
                    Account tk = new Account
                    {
                        FullName = taikhoan.Fullname,
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now,
                        RoleID = 2
                    };
                    try
                    {
                        db.Accounts.Add(tk);
                        db.Customers.Add(khachhang);
                        db.SaveChanges();

                        // Lưu session Makh
                        Session["CustomerId"] = khachhang.CustomerID.ToString();
                        var taikhoanID = Convert.ToInt32(Session["CustomerId"]);

                        // Identify
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, khachhang.FullName),
                    new Claim("CustomerId", khachhang.CustomerID.ToString())
                };
                        var identity = new ClaimsIdentity(claims, "login");
                        var principal = new ClaimsPrincipal(identity);
                        System.Web.HttpContext.Current.User = principal;
                        TempData["Success"] = "Đăng ký thành công";

                        return RedirectToAction("Dashboard", "Accounts");
                        //return RedirectToAction("Index", "Home");
                    }
                    catch
                    {
                        TempData["Error"] = "Đã xảy ra lỗi trong quá trình đăng ký";
                        return RedirectToAction("Register", "Accounts");
                    }
                }
                else
                {
                    // Trường hợp ModelState không hợp lệ, trả về View với dữ liệu đã nhập
                    return View(taikhoan);
                }
            }
            catch
            {
                // Xử lý lỗi nếu có lỗi xảy ra trong quá trình xử lý
                // Ví dụ: Log lỗi, hiển thị thông báo cho người dùng, vv.
                return View(taikhoan);
            }
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = null)
        {   
           
            var taikhoanID = Session["CustomerId"] as string;
            if (taikhoanID != null)
            {
                if (Convert.ToInt32(Session["AccountLogin"]) != 1)
                {
                    return RedirectToAction("Dashboard", "Accounts");
                    //return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    
                    return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
                }
                
            }
            if(returnUrl != null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để mua hàng";
                ViewBag.ReturnUrl = returnUrl;
            }   
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel customer, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Ultilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);
                    var khachhang = db.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);
                    var account = db.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);
                    
                    if (khachhang == null) return RedirectToAction("Register");
                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if (khachhang.Password != pass)
                    {
                        TempData["Error"] = "Thông tin đăng nhập không chính xác";
                        return View(customer);
                    }
                    if (khachhang.Active == false || account.Active == false)
                    {
                        TempData["Error"] = "Tài khoản của bạn đã bị vô hiệu hóa";
                        return View(customer);
                    }
                    if (account.RoleID == 1)
                    {
                        Session["AccountLogin"] = 1;
                        Session["CustomerId"] = khachhang.CustomerID.ToString();
                        return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
                       
                    }
                    else
                    {

                        Session["CustomerId"] = khachhang.CustomerID.ToString();
                        var taikhoanID = Session["CustomerId"].ToString();

                        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, khachhang.FullName),
                new Claim("CustomerId", khachhang.CustomerID.ToString())
            };
                        var identity = new ClaimsIdentity(claims, "login");
                        var principal = new ClaimsPrincipal(identity);
                        System.Web.HttpContext.Current.User = principal;
                        TempData["Success"] = "Đăng nhập thành công";
                        Session["AccountLogin"] = 2;
                        if (!string.IsNullOrEmpty(returnUrl))
                        {

                            return Redirect(returnUrl);
                        }
                        else
                        {

                            return RedirectToAction("Dashboard", "Accounts");
                        }
                    }
                   
                   
                }
            }
            catch
            {
                // Xử lý lỗi nếu có lỗi xảy ra trong quá trình xử lý
                // Ví dụ: Log lỗi, hiển thị thông báo cho người dùng, vv.
                return RedirectToAction("Register", "Accounts");
            }
            return View(customer);
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("CustomerId");
            Session.Remove("GioHang");
            Session.Remove("CountCart");
            TempData["Success"] = "Đăng xuất thành công";
            Session["AccountLogin"] = 0;
            return RedirectToAction("Index", "Home");
        }


        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountEdit(AccountEdit accountEdit)
        {
            try
            {
               
                if (ModelState.IsValid)
                {   
                    var customer = db.Customers.Find(accountEdit.CustomerId);
                    if (customer == null) return RedirectToAction("Login", "Accounts");
                    if (!Ultilities.IsValidPhoneNumber(accountEdit.Phone))
                    {
                        TempData["Error"] = "Số điện thoại không hợp lệ";
                        return RedirectToAction("DashBoard");
                    }
                    if (!Ultilities.IsValidEmail2(accountEdit.Email))
                    {
                        TempData["Error"] = "Email không hợp lệ";
                        return RedirectToAction("DashBoard");
                    }
                    if (accountEdit.NewPassword == null)
                    {

                        customer.CustomerID = accountEdit.CustomerId;
                        customer.Email = accountEdit.Email;
                        customer.Password = customer.Password;
                        customer.Salt = customer.Salt;
                        customer.Address = accountEdit.Address;
                        customer.Phone = accountEdit.Phone;
                        customer.Birthday = accountEdit.Birthday;
                        customer.FullName = accountEdit.Fullname;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Success"] = "Chỉnh sửa thông tin thành công";
                        return RedirectToAction("DashBoard");
                    }
                    var pass = (accountEdit.CurrentPassword.Trim() + customer.Salt.Trim()).ToMD5();
                    if(pass == customer.Password)
                    {
                        customer.CustomerID = accountEdit.CustomerId;
                        customer.Email = accountEdit.Email;
                        customer.Password = (accountEdit.NewPassword.Trim() + accountEdit.Salt.Trim()).ToMD5();
                        customer.Salt = accountEdit.Salt;
                        customer.Address = accountEdit.Address;
                        customer.Phone = accountEdit.Phone;
                        customer.Birthday = accountEdit.Birthday;
                        customer.FullName = accountEdit.Fullname;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["Success"] = "Chỉnh sửa thông tin thành công";
                        return RedirectToAction("DashBoard");
                    }
                    else
                    {
                        TempData["Error"] = "Sai mật khẩu";
                        return RedirectToAction("DashBoard");
                    }
                  

                }
                TempData["Error"] = "Chỉnh sửa thông tin thất bại";
                return RedirectToAction("DashBoard");
            }
            catch
            {
                TempData["Error"] = "Chỉnh sửa thông tin thất bại";
                return RedirectToAction("DashBoard");
            }


         
        }

        public ActionResult GetBirthday()
        {
            var taikhoanID = Convert.ToInt32(Session["CustomerId"]);
            var customer = db.Customers.FirstOrDefault(x => x.CustomerID == taikhoanID);

            string formattedBirthday = string.Empty;
            if (customer.Birthday != null)
            {
                formattedBirthday = string.Format("{0:yyyy-MM-ddTHH:mm}", customer.Birthday, CultureInfo.InvariantCulture);

            }

            // Trả về chuỗi định dạng datetime-local để hiển thị trong TextBox
            return Content(formattedBirthday);
        }

        public ActionResult ViewOrder(int? orderid)
        {
            var OrderToView = db.Orders.Where(x => x.OrderID == orderid).FirstOrDefault();
            var OrderDetails = db.OrderDetails.Where(x => x.OrderID == orderid).ToList();
            List<ProductDetail> ProductDetails = new List<ProductDetail>();
            foreach(var item in OrderDetails)
            {
                var ProductDetail = db.ProductDetails.Where(x => x.ProductDetailID == item.ProductDetailID).FirstOrDefault();
                ProductDetails.Add(ProductDetail);
            }
            var Colors = db.Colors.ToList();
            var Sizes = db.Sizes.ToList();
            ViewBag.Colors = Colors;
            ViewBag.Sizes = Sizes;
            ViewBag.ProductDetails = ProductDetails;
            ViewBag.OrderToView = OrderToView;
            ViewBag.OrderDetails = OrderDetails;
            return View();
        }
    }
}
