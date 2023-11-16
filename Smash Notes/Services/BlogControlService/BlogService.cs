using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Smash_Notes.Server.Data;
using Smash_Notes.Shared.Dto.Blog;
using static System.Net.WebRequestMethods;

namespace Smash_Notes.Server.BlogControlService
{
    public class BlogService : IBlogService
	{
		private readonly IMapper _mapper;
		private readonly DataContext _context;
		public IHttpContextAccessor _httpContextAccessor { get; }
		public BlogService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
			_mapper = mapper;

		}

        private int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            else
            {
                // Handle the case where the user claim is missing or invalid.
                // You can log an error, throw an exception, or return a default value.
                // Here, I'm returning -1 as an example.
                return -1;
            }
        }


        public async Task<ServiceResponse<List<GetBlogPostDto>>> GetAllBlogPost()
		{
			var serviceResponse = new ServiceResponse<List<GetBlogPostDto>>();
			var dbBlogPost = await _context.BlogPosts
			.Where(c => c.User!.Id == GetUserId())
			.OrderByDescending(c => c.DateCreated).ToListAsync(); //filters in order of date created

			serviceResponse.Data = dbBlogPost.Select(c => _mapper.Map<GetBlogPostDto>(c)).ToList(); //maps the blogpost type to a list of GetBlogPostDto
            return serviceResponse;
		}

		public async Task<ServiceResponse<GetBlogPostDto>> CreateNewBlogPost(AddBlogPostDto request)
		{
			var serviceResponse = new ServiceResponse<GetBlogPostDto>();
			var blogPost = _mapper.Map<BlogPost>(request); //maps AddBlogPostDto to BlogPost
			blogPost.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId()); //sets the user navigation prop to the user with the given Id

			_context.Add(blogPost);
			await _context.SaveChangesAsync();

			var blogPostRequest = await _context.BlogPosts
				.FirstOrDefaultAsync(p => p.Id.Equals(blogPost.Id) && p.User!.Id == GetUserId()); //queries database for blogpost

			serviceResponse.Data = _mapper.Map<GetBlogPostDto>(blogPostRequest); //maps BlogPost to GetBlogPostDto
			serviceResponse.Message = "Blog Post Created.";

			return serviceResponse;
		}
        public async Task<ServiceResponse<GetBlogPostDto>> GetBlogPostById(int id)
        {
            var serviceResponse = new ServiceResponse<GetBlogPostDto>();
			var blogPost = await _context.BlogPosts
				.FirstOrDefaultAsync(b => b.Id == id && b.User!.Id == GetUserId());

			if (blogPost != null)
			{
                serviceResponse.Data = _mapper.Map<GetBlogPostDto>(blogPost);
				serviceResponse.Success = true;
				serviceResponse.Message = "Returned Post!";
            }
			else
			{
				serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Post not found.";
            }

			return serviceResponse;
        }

        public async Task<ServiceResponse<GetBlogPostDto>> UpdateBlogPost(UpdateBlogPostDto blogPost)
		{
            var serviceResponse = new ServiceResponse<GetBlogPostDto>();
            var updatePost = await _context.BlogPosts
                .FirstOrDefaultAsync(b => b.Id == blogPost.Id && b.User!.Id == GetUserId());
			if (updatePost != null)
			{
				updatePost.Title = blogPost.Title;
				updatePost.Content = blogPost.Content;
				await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetBlogPostDto>(updatePost);
                serviceResponse.Success = true;
                serviceResponse.Message = "Returned Post!";
            }
            else
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Post not found.";
            }
			return serviceResponse;
        }
    }
}
