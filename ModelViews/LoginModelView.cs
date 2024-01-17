using System.ComponentModel.DataAnnotations;

namespace Ecommerce_web_app.ModelViews
{
	public class LoginModelView
	{
		[Key]
		[MaxLength(100)]
		[Required(ErrorMessage ="Vui lòng nhập Email")]
		[DataType(DataType.EmailAddress)]
		[EmailAddress]
		[Display(Name ="Email")]
		public string UserName{ get; set; }
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		[Display(Name = "Mật khẩu")]
		[MinLength(5, ErrorMessage="Bạn cần nhập mật khẩu tối thiểu 5 ký tự")]
		public string Password { get; set; }
	}
}
