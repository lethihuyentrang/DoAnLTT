using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBQA.Models
{
    public class RegisterModel
    {   
        [Key]
        public int CustomerId { get; set; }
        [Display(Name="Họ và tên")]
        [Required(ErrorMessage ="Vui lòng nhập họ tên")]
        public string Fullname { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập Email")]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        [Remote(action:"ValidateEmail",controller: "Account")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [MaxLength(11)]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Account")]
        public string Phone { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(8,ErrorMessage ="Bạn cần đặt mật khẩu tối thiểu 8 ký tự")]
        public string Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [MinLength(8, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 8 ký tự")]
        [System.ComponentModel.DataAnnotations.Compare("Password",ErrorMessage="Xác nhận mật khẩu thất bại")]
        public string ConfirmPassword { get; set; }
    }
}