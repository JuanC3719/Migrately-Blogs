using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrately.Models.Domain
{
    public class BlogBase
    {
        public int Id { get; set; }
        public LookUp BlogType { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Uri ImageUrl { get; set; }
   
    }
}
