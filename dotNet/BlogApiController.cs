using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Migrately.Models;
using Migrately.Models.Domain;
using Migrately.Models.Requests;
using Migrately.Services;
using Migrately.Services.Interfaces;
using Migrately.Web.Controllers;
using Migrately.Web.Models.Responses;
using Migrately.Web.StartUp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Migrately.Web.Api.Controllers
{
    
    [Route("api/blogs")]
    [ApiController]
    public class BlogApiController : BaseApiController
    {
        private IBlogService _blogService;
        private IAuthenticationService<int> _authService;

        public BlogApiController(IBlogService blogService
            , IAuthenticationService<int> authService
            , ILogger<ExamplesApiController> logger) : base(logger)
        {
            _blogService = blogService;
            _authService = authService;
        }
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Blog>> GetBlogById(int id)

        {

            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Blog blog = _blogService.GetBlogById(id);

                if (blog == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("No Records Found");
                }
                else
                {
                    response = new ItemResponse<Blog> { Item = blog };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(iCode, response);

        }

        [HttpGet("paginate")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<Blog>>> GetAllBlogsByPage(int pageIndex, int pageSize)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                Paged<Blog> blogList = _blogService.GetAllBlogsByPage(pageIndex, pageSize);

                if (blogList == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("No Records Found");
                }
                else
                {
                    response = new ItemResponse<Paged<Blog>> { Item = blogList };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(iCode, response); 
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> AddBlog(BlogAddRequest model)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {

                int id = _blogService.AddBlog(model);

                response = new SuccessResponse();

            }

            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(iCode, response);
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> UpdateBlog(BlogUpdateRequest model)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {

                _blogService.UpdateBlog(model);

                response = new SuccessResponse();
            }

            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(iCode, response);
        }

        [HttpPut("delete/{id:int}")]
        public ActionResult<SuccessResponse> UpdateIsDeletedBlog(BlogIsDeletedUpdateRequest model)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {

                _blogService.UpdateIsDeletedBlog(model);

                response = new SuccessResponse();
            }

            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(iCode, response);
        }

        [AllowAnonymous]
        [HttpGet("blogtypes")]
        public ActionResult<ItemResponse<Paged<Blog>>> GetByBlogType(int pageIndex, int pageSize, int blogTypeId)

        {

            int iCode = 200;
            BaseResponse response = null;

            try
            {
               Paged<Blog> page = _blogService.GetByBlogType(pageIndex, pageSize, blogTypeId);

                if (page == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("No Records Found");
                }
                else
                {
                    response = new ItemResponse<Paged<Blog>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(iCode, response);

        }

        [HttpGet("search")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<Blog>>> SearchBlogs(int pageIndex, int pageSize, string query)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<Blog> page = _blogService.SearchBlogs(pageIndex, pageSize, query);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<Blog>> { Item = page };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpGet("author/paginate")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<Blog>>> GetBlogByAuthorId(int authorId, int pageIndex, int pageSize)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                Paged<Blog> blogList = _blogService.GetBlogByAuthorId(authorId, pageIndex, pageSize);

                if (blogList == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("No Records Found"); 
                }
                else
                {
                    response = new ItemResponse<Paged<Blog>> { Item = blogList };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message); 
            }

            return StatusCode(iCode, response);
        }
        
       

    }
}
