using Migrately.Models;
using Migrately.Models.Domain;
using Migrately.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrately.Services.Interfaces
{
    public interface IBlogService
    {
        public Blog GetBlogById(int id);
        int AddBlog(BlogAddRequest model);
        Paged<Blog> GetAllBlogsByPage(int pageIndex, int pageSize);
        void UpdateBlog(BlogUpdateRequest model);
        void UpdateIsDeletedBlog(BlogIsDeletedUpdateRequest model);
        Paged<Blog> GetByBlogType(int pageIndex, int pageSize, int blogTypeId);
        Paged<Blog> GetBlogByAuthorId(int authorId, int pageIndex, int pageSize);
        Paged<Blog> SearchBlogs(int pageIndex, int pageSize, string query);
        public List<BlogType> GetAllBlogTypes();
    }
}
