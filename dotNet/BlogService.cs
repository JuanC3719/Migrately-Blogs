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
        private readonly IDataProvider _data;

        public BlogService(IDataProvider data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public int AddBlog(BlogAddRequest model)
        {
            const string procName = "[dbo].[Blogs_Insert]";
            int id = 0;

            _data.ExecuteNonQuery(procName, inputParamMapper: col =>
            {
                AddCommonParams(model, col);
                col.AddWithValue("@IsDeleted", model.IsDeleted);

                var idOut = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                col.Add(idOut);

            }, returnParameters: returnCol =>
            {
                object rawIdObject = returnCol["@Id"].Value;
                int.TryParse(rawIdObject.ToString(), out id);
            });

            return id;
        }

        public Paged<Blog> GetAllBlogsByPage(int pageIndex, int pageSize)
        {
            const string procName = "[dbo].[Blogs_SelectAll]";
            Paged<Blog> pagedList = null;
            List<Blog> blogList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, inputParams =>
            {
                inputParams.AddWithValue("@PageIndex", pageIndex);
                inputParams.AddWithValue("@PageSize", pageSize);

            }, (IDataReader reader, short set) =>
            {
                int index = 0;
                Blog blog = MapSingleBlog(reader, ref index);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(index++);
                }

                blogList ??= new List<Blog>();
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
            const string procName = "[dbo].[Blogs_Select_ByCreatedBy]";
            Paged<Blog> pagedList = null;
            List<Blog> blogList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, col =>
            {
                col.AddWithValue("@AuthorId", authorId);
                col.AddWithValue("@PageIndex", pageIndex);
                col.AddWithValue("@PageSize", pageSize);

            }, (IDataReader reader, short set) =>
            {
                int index = 0;
                Blog blog = MapSingleBlog(reader, ref index);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(index++);
                }

                blogList ??= new List<Blog>();
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
            const string procName = "[dbo].[Blogs_SelectById]";
            Blog blog = null;

            _data.ExecuteCmd(procName, col =>
            {
                col.AddWithValue("@Id", id);

            }, (IDataReader reader, short set) =>
            {
                int startingIndex = 0;
                blog = MapSingleBlog(reader, ref startingIndex);
            });

            return blog;
        }

        public void UpdateBlog(BlogUpdateRequest model)
        {
            const string procName = "[dbo].[Blogs_Update]";

            _data.ExecuteNonQuery(procName, inputParamMapper: col =>
            {
                AddCommonParams(model, col);
                col.AddWithValue("@Id", model.Id);

            }, returnParameters: null);
        }

        public void UpdateIsDeletedBlog(BlogIsDeletedUpdateRequest model)
        {
            const string procName = "[dbo].[Blogs_Delete]";

            _data.ExecuteNonQuery(procName, inputParamMapper: col =>
            {
                col.AddWithValue("@Id", model.Id);

            }, returnParameters: null);
        }

        public Paged<Blog> GetByBlogType(int pageIndex, int pageSize, int blogTypeId)
        {
            Paged<Blog> pagedList = null;
            List<Blog> list = null;
            int totalCount = 0;

            const string procName = "[dbo].[Blogs_Select_BlogCategory]";
            _data.ExecuteCmd(procName, paramCollection =>
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
                paramCollection.AddWithValue("@BlogTypeId", blogTypeId);

            }, (reader, recordSetIndex) =>
            {
                int startingIndex = 0;
                Blog blog = MapSingleBlog(reader, ref startingIndex);

                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }

                list ??= new List<Blog>();
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
                param =>
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

                    list ??= new List<Blog>();
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
            const string procName = "[dbo].[BlogTypes_SelectAll]";
            _data.ExecuteCmd(procName, 
                inputParamMapper: null,
                singleRecordMapper: (IDataReader reader, short set) =>
                {
                    int startingIndex = 0;
                    BlogType blogType = MapSingleLookUp(reader, ref startingIndex);
                    list ??= new List<BlogType>();
                    list.Add(blogType);
                }
            );

            return list;
        }

        private static BlogType MapSingleLookUp(IDataReader reader, ref int startingIndex)
        {
            BlogType blogType = new BlogType
            {
                Id = reader.GetSafeInt32(startingIndex++),
                Name = reader.GetSafeString(startingIndex++)
            };
            return blogType;
        }

        private static Blog MapSingleBlog(IDataReader reader, ref int startingIndex)
        {
            Blog blog = new Blog
            {
                Id = reader.GetSafeInt32(startingIndex++),
                BlogType = new LookUp
                {
                    Id = reader.GetSafeInt32(startingIndex++),
                    Name = reader.GetSafeString(startingIndex++)
                },
                AuthorId = reader.GetSafeInt32(startingIndex++),
                Title = reader.GetSafeString(startingIndex++),
                Subject = reader.GetSafeString(startingIndex++),
                Content = reader.GetSafeString(startingIndex++),
                ImageUrl = reader.GetSafeUri(startingIndex++),
                IsPublished = reader.GetSafeBool(startingIndex++),
                IsDeleted = reader.GetSafeBool(startingIndex++),
                DateCreated = reader.GetSafeDateTime(startingIndex++),
                DateModified = reader.GetSafeDateTime(startingIndex++),
                DatePublished = reader.GetSafeDateTime(startingIndex++)
            };
        
            return blog;
        }


        private static void AddCommonParams(BlogAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@BlogTypeId", model.BlogTypeId);
            col.AddWithValue("@AuthorId", model.AuthorId);
            col.AddWithValue("@Title", model.Title);
            col.AddWithValue("@Subject", model.Subject);
            col.AddWithValue("@Content", model.Content);
            col.AddWithValue("@IsPublished", model.IsPublished);
            col.AddWithValue("@ImageUrl", model.ImageUrl);
        }
    }
}
