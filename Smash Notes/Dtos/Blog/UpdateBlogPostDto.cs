using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smash_Notes.Shared.Dto.Blog
{
    public class UpdateBlogPostDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(int.MaxValue)]
        public string Content { get; set; } = string.Empty;
    }
}
