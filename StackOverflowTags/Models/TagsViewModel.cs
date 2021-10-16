using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackOverflowTags.Models
{
    public class TagsViewModel
    {
        public List<Tag> Tags { get; set; }
        public int PopularitySum { get; set; }
    }
}
