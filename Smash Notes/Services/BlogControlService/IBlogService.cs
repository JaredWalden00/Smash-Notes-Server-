using Smash_Notes.Shared.Dto.Blog;
using static System.Net.WebRequestMethods;

namespace Smash_Notes.Server.BlogControlService
{
    public interface IBlogService
	{
		Task<ServiceResponse<List<GetBlogPostDto>>> GetAllBlogPost();
		Task<ServiceResponse<GetBlogPostDto>> CreateNewBlogPost(AddBlogPostDto request);
		Task<ServiceResponse<GetBlogPostDto>> GetBlogPostById(int id);
		Task<ServiceResponse<GetBlogPostDto>> UpdateBlogPost(UpdateBlogPostDto blogPost);
    }
}
