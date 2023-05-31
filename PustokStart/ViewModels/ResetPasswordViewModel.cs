using System.ComponentModel.DataAnnotations;

namespace PustokStart.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required]
		public string Email {get; set;}
		[Required]
		[MaxLength(20)]
		[DataType(DataType.Password)]
		public string Password { get; set;}
		[Compare(nameof(Password))]
		public string ConfirmPassword { get; set;}
		public string Token {get; set;}
	}
}
