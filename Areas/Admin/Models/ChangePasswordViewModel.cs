using System.ComponentModel.DataAnnotations;

namespace Ecommerce_web_app.Areas.Admin.Models
{
	public class ChangePasswordViewModel
	{
		[Key]
		public int UserId { get; set; }
		public string Email { get; set; }
		[Display(Name = "Mật khẩu hiện tại")]
		public string PasswordNow { get; set; }
		[Display(Name = "Mật khẩu mới")]
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
		public string Password { get; set; }
		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
		[Display(Name = " Nhập lại mật khẩu mới")]
		[Compare("Password", ErrorMessage = "Mật khẩu không giống nhau")]
		public string ConfirmPassword { get; set; }	
	}
}
