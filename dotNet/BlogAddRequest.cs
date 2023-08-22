using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrately.Models.Requests
{
    public class BlogAddRequest
    {
        [Required(ErrorMessage = "Blog Type ID is required")]
        public int BlogTypeId { get; init; }

        [Required(ErrorMessage = "Author ID is required")]
        public int AuthorId { get; init; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; init; }

        public string Subject { get; init; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; init; }

        [Display(Name = "Published")]
        public bool IsPublished { get; init; }

        [Url(ErrorMessage = "Invalid URL format")]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; init; }

        [Display(Name = "Deleted")]
        public bool IsDeleted { get; init; }
    }
}
