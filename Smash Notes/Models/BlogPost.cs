using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smash_Notes.Shared
{
	public class BlogPost
	{
		public int Id { get; set; }
		public string Url { get; set; } = string.Empty;

		[Required]
		public string Title { get; set; } = string.Empty;

		[Required]
		[MaxLength(int.MaxValue)]
		public string Content { get; set; } = string.Empty;
		public User? User { get; set; }
        public List<Character>? Characters { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
	}
}
