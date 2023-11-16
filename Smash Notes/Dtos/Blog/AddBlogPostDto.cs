using System.ComponentModel.DataAnnotations;

namespace Smash_Notes.Shared.Dto.Blog
{
    public class AddBlogPostDto
    {
        public string Url { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(int.MaxValue)]
        public string Content { get; set; } = string.Empty;
    }
}
