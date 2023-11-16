using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smash_Notes.Shared
{
    public class BlogPostCharacterRequest
    {
        public int BlogId { get; set; }
        public List<int>? CharacterIds { get; set; }
    }
}
