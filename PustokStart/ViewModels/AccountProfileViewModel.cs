using PustokStart.Models;

namespace PustokStart.ViewModels
{
	public class AccountProfileViewModel
	{
		public ProfileEditViewModel Profile { get; set; }	
		public List<Order> Orders { get; set; }	
	}
}
