using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLBQA.Models
{
    public class CheckOut
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone  { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        public string Address  { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
        public int? TinhThanh  { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public int? QuanHuyen  { get; set; }
        [Required(ErrorMessage ="Vui lòng chọn Phường/Xã")]
        public int? PhuongXa  { get; set; }

        public double TotalMoney { get; set; }
        public int PaymentID  { get; set; }
        public string Note  { get; set; }
    }
}