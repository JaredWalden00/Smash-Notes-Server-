using Microsoft.AspNetCore.Mvc;
using Smash_Notes.Shared.Dto.Blog;

namespace Smash_Notes.Server.CharacterControlService
{
    public interface ICharacterService
    {
        Task AddCharactersToBlogPost([FromBody] BlogPostCharacterRequest request);
        Task<List<Character>> GimmeAllTheCharacters();
        Task<ServiceResponse<List<Character>>> GetBlogPostCharacters(int id);
        Task<ServiceResponse<List<GetBlogPostDto>>> GetBlogPostsByCharacterId(int charid);
    }
}
