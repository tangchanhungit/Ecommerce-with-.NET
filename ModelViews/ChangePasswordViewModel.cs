using System.ComponentModel.DataAnnotations;

namespace Ecommerce_web_app.ModelViews
{
	public class ChangePasswordViewModel
	{
		[Key]
		public int CustomerId { get; set; }
		[Display(Name = "Mật khẩu hiện tại")]
		public string PasswordNow { get; set; }
		[Display(Name = "Mật khẩu mới")]
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
		public string Password { get; set; }
		[Display(Name = "Nhập lại mật khẩu mới")]
		[Compare("Password",ErrorMessage = "Mật khẩu không trùng khớp")]
		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
		public string ConfirmPassword { get; set; }
	}
}
