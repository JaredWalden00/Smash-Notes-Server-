using AutoMapper;
using Smash_Notes.Shared.Dto.Blog;

namespace Smash_Notes.Server
{
    public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<BlogPost, GetBlogPostDto>();
			CreateMap<AddBlogPostDto, BlogPost>();
            CreateMap<UpdateBlogPostDto, BlogPost>();
        }
	}
}