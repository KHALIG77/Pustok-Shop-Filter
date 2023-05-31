using Newtonsoft.Json;
using PustokStart.Models;

namespace PustokStart.ViewModels
{
	public class BookDetailViewModel
	{
		public Book Book {  get; set; }
		public int CommentCount {get; set; }
		public int TotalCommentCount {get; set; }

		public List<Book> RelatedBook { get; set; }

        public BookComment Comment { get; set; }

	}
}
