using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Migrately.Models.Domain
{
    public record Blog
    {
        public int Id { get; init; }

        [Display(Name = "Blog Type")]
        public LookUp BlogType { get; init; }

        [Display(Name = "Author ID")]
        public int AuthorId { get; init; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; init; }

        public string Subject { get; init; }

        public string Content { get; init; }

        [Url]
        [Display(Name = "Image URL")]
        public Uri ImageUrl { get; init; }

        [Display(Name = "Created Date")]
        public DateTime DateCreated { get; init; }

        [Display(Name = "Modified Date")]
        public DateTime DateModified { get; init; }

        [Display(Name = "Published Date")]
        public DateTime DatePublished { get; init; }

        [Display(Name = "Is Published")]
        public bool IsPublished { get; init; }

        [Display(Name = "Is Deleted")]
        public bool IsDeleted { get; init; }
    }
}
