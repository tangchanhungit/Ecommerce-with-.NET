using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_web_app.ModelViews
{
	public class RegisterViewModel
	{
		public int CustomerId { get; set; }
		[Display(Name = "Họ và Tên")]
		[Required(ErrorMessage = "Vui lòng nhập họ tên")]
		[Key]

		public string FullName { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập email")]
		[MaxLength(100)]
		[DataType(DataType.EmailAddress)]
		[Remote(action: "ValidateEmail", controller: "User")]
		public string Email { get; set; }
		[MaxLength(10)]
		[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
		[Display(Name = "Số điện thoại")]
		[DataType(DataType.PhoneNumber)]
		[Remote(action: "ValidatePhone", controller:"Account")]
		public string Phone { get; set; }
		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
		public string Password { get; set; }
		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
		[Display(Name = "Nhập lại mật khẩu")]
		[Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp")]
		public string ConfirmPassword { get; set; } 
	}
}
