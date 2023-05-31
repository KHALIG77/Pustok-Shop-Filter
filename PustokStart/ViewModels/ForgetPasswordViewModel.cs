using System.ComponentModel.DataAnnotations;

namespace PustokStart.ViewModels
{
	public class ForgetPasswordViewModel
	{
		[Required]
		[MaxLength(100)]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string Email {get; set;}
	}
}
