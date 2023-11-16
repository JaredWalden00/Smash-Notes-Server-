using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Smash_Notes.Server.Data;
using Smash_Notes.Shared;
using Microsoft.EntityFrameworkCore;
using Azure.Core;
using Smash_Notes.Shared.Dto.Blog;

namespace Smash_Notes.Server.CharacterControlService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public IHttpContextAccessor _httpContextAccessor { get; }
        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
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

        public async Task AddCharactersToBlogPost([FromBody] BlogPostCharacterRequest request)
        {
            var blogPost = await _context.BlogPosts
                .Include(b => b.Characters) //include the Characters navigation property
                .FirstOrDefaultAsync(b => b.Id == request.BlogId && b.User != null && b.User.Id == GetUserId()); //find blogpost

            if (blogPost != null)
            {
                var characters = await _context.Characters
                    .Where(c => request.CharacterIds.Contains(c.Id))
                    .ToListAsync(); //finds character based on character id's from request

                if (blogPost.Characters == null)
                {
                    blogPost.Characters = new List<Character>(); //initialize if null
                }

                blogPost.Characters.AddRange(characters); //adds characters to blogPosts character navigation prop

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<Character>> GimmeAllTheCharacters()
        {
            return await _context.Characters.ToListAsync();
        }

        public async Task<ServiceResponse<List<Character>>> GetBlogPostCharacters(int BlogId)
        {
            var serviceResponse = new ServiceResponse<List<Character>>();
            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(b => b.Id == BlogId && b.User!.Id == GetUserId());

            if (blogPost != null)
            {
                var charactersInBlogPost = _context.Characters
                    .Where(c => c.BlogPosts.Any(bp => bp.Id == BlogId))
                    .ToList();

                serviceResponse.Data = charactersInBlogPost;
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

        public async Task<ServiceResponse<List<GetBlogPostDto>>> GetBlogPostsByCharacterId(int charid)
        {
            var serviceResponse = new ServiceResponse<List<GetBlogPostDto>>();
            var blogPost = await _context.BlogPosts
                .Where(b => b.Characters.Any(c => c.Id == charid) && b.User!.Id == GetUserId()) //checks blogposts for if any of them have characters with Id equal to the charId.
                .OrderByDescending(c => c.DateCreated).ToListAsync();

            if (blogPost != null)
            {
                serviceResponse.Data = blogPost.Select(c => _mapper.Map<GetBlogPostDto>(c)).ToList(); //mapped as list of GetBlogPostDtos
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
