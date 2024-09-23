using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QLBQA.Models
{
    public class LoginModel
    {
        [Key]
        [Display(Name = "Địa chỉ Email")]
        [MaxLength(100)]
        [Required(ErrorMessage = "Bạn cần nhập Email.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
       // [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại chỉ có thể chứa các ký tự số.")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Bạn cần nhập mật khẩu.")]
        [MinLength(8, ErrorMessage = "Bạn cần nhập mật khẩu tối thiểu 8 ký tự")]
        public string Password { get; set; }
    }
}
