using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Migrately.Models.Domain;
using Migrately.Services.Interfaces;
using Migrately.Data.Providers;
using Migrately.Data;
using Migrately.Models.Requests;
using Migrately.Models;
using System.Reflection.Metadata;
using System.Reflection;
using static System.Reflection.Metadata.BlobBuilder;

namespace Migrately.Services
{
    public class BlogService : IBlogService
    {
        private IDataProvider _data = null;
        public BlogService(IDataProvider data)
        {
            _data = data;
        }
        public int AddBlog(BlogAddRequest model)
        {
            string procName = "[dbo].[Blogs_Insert]";

            int id = 0;

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                col.AddWithValue("@IsDeleted", model.IsDeleted);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                col.Add(idOut);

            }, returnParameters: delegate (SqlParameterCollection returnCol)
            {
                object rawIdobject = returnCol["@Id"].Value;
                int.TryParse(rawIdobject.ToString(), out id);
            });

            return id;

        } 
        public Paged<Blog> GetAllBlogsByPage(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Blogs_SelectAll]";
            Paged<Blog> pagedList = null;
            List<Blog> blogList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, (SqlParameterCollection inputParams) =>
            {
                inputParams.AddWithValue("@PageIndex", pageIndex);
                inputParams.AddWithValue("@PageSize", pageSize);

            }, (IDataReader reader, short set) =>
            {
                int index = 0;
                Blog blog = null;
                blog = MapSingleBlog(reader, ref index);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(index++);
                }

                if (blogList == null)
                {
                    blogList = new List<Blog>();
                }

                blogList.Add(blog);
            });

            if (blogList != null)
            {
                pagedList = new Paged<Blog>(blogList, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }
        public Paged<Blog> GetBlogByAuthorId(int authorId, int pageIndex, int pageSize)
        {

            string procName = "[dbo].[Blogs_Select_ByCreatedBy]";
            Paged<Blog> pagedList = null;
            List<Blog> blogList = null; 
            int totalCount = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {   
                col.AddWithValue("@AuthorId", authorId);
                col.AddWithValue("@PageIndex", pageIndex);
                col.AddWithValue("@PageSize", pageSize);
                

            }, delegate (IDataReader reader, short set)
            {
                int index = 0;
                Blog blog = null;
                blog = MapSingleBlog(reader, ref index);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(index++);
                }

                if (blogList == null)
                {
                    blogList = new List<Blog>();
                }

                blogList.Add(blog);
            });

            if (blogList != null)
            {
                pagedList = new Paged<Blog>(blogList, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }
        public Blog GetBlogById(int id)
        {
            
            string procName = "[dbo].[Blogs_SelectById]";
            Blog blog = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);

            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                blog = MapSingleBlog(reader, ref startingIndex);

            });

            return blog;
        }
        public void UpdateBlog(BlogUpdateRequest model)
        {
            string procName = "[dbo].[Blogs_Update]";

            _data.ExecuteNonQuery(procName, inputParamMapper: (SqlParameterCollection col) =>
            {

                AddCommonParams(model, col);
                col.AddWithValue("@Id", model.Id);

            }, returnParameters: null);

        }
        public void UpdateIsDeletedBlog(BlogIsDeletedUpdateRequest model)
        {
            string procName = "[dbo].[Blogs_Delete]";

            _data.ExecuteNonQuery(procName, inputParamMapper: (SqlParameterCollection col) =>
            {
                col.AddWithValue("@Id", model.Id);
             

            }, returnParameters: null);

        }
        public Paged<Blog> GetByBlogType(int pageIndex, int pageSize, int blogTypeId)
        {
            Paged<Blog> pagedList = null;
            List<Blog> list = null;
            int totalCount = 0;

            string procName = "[dbo].[Blogs_Select_BlogCategory]";
            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
                paramCollection.AddWithValue("@BlogTypeId", blogTypeId);
            },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    Blog blog = MapSingleBlog(reader, ref startingIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                    }
                    if (list == null)
                    {
                        list = new List<Blog>();
                    }
                    list.Add(blog);
                });
            if (list != null)
            {
                pagedList = new Paged<Blog>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
          
        } 


        public Paged<Blog> SearchBlogs(int pageIndex, int pageSize, string query)
        {
            Paged<Blog> pagedList = null;
            List<Blog> list = null;
            int totalCount = 0;

            _data.ExecuteCmd("[dbo].[Blogs_SearchPagination]",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@Query", query);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    Blog aBlog = MapSingleBlog(reader, ref startingIndex);
                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                    }

                    if (list == null)
                    {
                        list = new List<Blog>();
                    }

                    list.Add(aBlog);
                });
            if (list != null)
            {
                pagedList = new Paged<Blog>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

      
        public List<BlogType> GetAllBlogTypes()
        {
            List<BlogType> list = null;
            string procName = "[dbo].[BlogTypes_SelectAll]";
            _data.ExecuteCmd(procName, 
                inputParamMapper: null,
               singleRecordMapper: delegate (IDataReader reader, short set)
               {
                   int startingIndex = 0;
                   BlogType blog = MapSingleLookUp(reader, ref startingIndex);
                   list ??= new List<BlogType>();
                   list.Add(blog);
               }
            );
            return list;
        }
        private static BlogType MapSingleLookUp(IDataReader reader, ref int startingIndex)
        {
            startingIndex = 0;
            BlogType blogType = new BlogType();
            blogType.Id = reader.GetSafeInt32(startingIndex++);
            blogType.Name = reader.GetSafeString(startingIndex++);
            return blogType;
        }
        private static Blog MapSingleBlog(IDataReader reader,ref int startingIndex)
        {
           
                Blog blog = new Blog();
                AuthorUser author = new AuthorUser();
                blog.BlogType = new LookUp();



            blog.Id = reader.GetSafeInt32(startingIndex++);
            blog.BlogType.Id = reader.GetSafeInt32(startingIndex++);
            blog.AuthorId = reader.GetSafeInt32(startingIndex++);
            blog.BlogType.Name = reader.GetSafeString(startingIndex++);
            author.FirstName = reader.GetSafeString(startingIndex++);
            author.LastName = reader.GetSafeString(startingIndex++);
            author.AvatarUrl = reader.GetSafeUri(startingIndex++);
            blog.Title = reader.GetSafeString(startingIndex++);
            blog.Subject = reader.GetSafeString(startingIndex++);
            blog.Content = reader.GetSafeString(startingIndex++);
            blog.ImageUrl = reader.GetSafeUri(startingIndex++);
            blog.IsPublished = reader.GetSafeBool(startingIndex++);
            blog.IsDeleted = reader.GetSafeBool(startingIndex++);
            blog.DateCreated = reader.GetSafeDateTime(startingIndex++);
            blog.DateModified = reader.GetSafeDateTime(startingIndex++);
            blog.DatePublished = reader.GetSafeDateTime(startingIndex++);



            return blog;

        }
        private static void AddCommonParams(BlogAddRequest model,
            SqlParameterCollection collect)
        {
            collect.AddWithValue("@BlogTypeId", model.BlogTypeId);
            collect.AddWithValue("@AuthorId", model.AuthorId);
            collect.AddWithValue("@Title", model.Title);
            collect.AddWithValue("@Subject", model.Subject);
            collect.AddWithValue("@Content", model.Content);
            collect.AddWithValue("@IsPublished", model.IsPublished);
            collect.AddWithValue("@ImageUrl", model.ImageUrl);
            
        }

        
    }
}
    

