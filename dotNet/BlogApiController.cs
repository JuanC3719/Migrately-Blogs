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
        private readonly IBlogService _blogService;
        private readonly IAuthenticationService<int> _authService;

        public BlogApiController(IBlogService blogService, IAuthenticationService<int> authService, ILogger<BlogApiController> logger) : base(logger)
        {
            _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemResponse<Blog>>> GetBlogById(int id)
        {
            try
            {
                var blog = await _blogService.GetBlogByIdAsync(id);
                if (blog == null)
                {
                    return NotFound(new ErrorResponse("No Records Found"));
                }
                return Ok(new ItemResponse<Blog> { Item = blog });
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex, "Error getting blog by ID.");
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpGet("paginate")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemResponse<Paged<Blog>>>> GetAllBlogsByPage(int pageIndex, int pageSize)
        {
            try
            {
                var blogList = await _blogService.GetAllBlogsByPageAsync(pageIndex, pageSize);
                if (blogList == null)
                {
                    return NotFound(new ErrorResponse("No Records Found"));
                }
                return Ok(new ItemResponse<Paged<Blog>> { Item = blogList });
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex, "Error getting paginated blogs.");
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ItemResponse<int>>> AddBlog(BlogAddRequest model)
        {
            try
            {
                int id = await _blogService.AddBlogAsync(model);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex, "Error adding a blog.");
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SuccessResponse>> UpdateBlog(BlogUpdateRequest model)
        {
            try
            {
                await _blogService.UpdateBlogAsync(model);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex, "Error updating a blog.");
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpPut("delete/{id:int}")]
        public async Task<ActionResult<SuccessResponse>> UpdateIsDeletedBlog(BlogIsDeletedUpdateRequest model)
        {
            try
            {
                await _blogService.UpdateIsDeletedBlogAsync(model);
                return Ok(new SuccessResponse());
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex, "Error updating 'IsDeleted' status for a blog.");
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpGet("blogtypes")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemResponse<Paged<Blog>>>> GetByBlogType(int pageIndex, int pageSize, int blogTypeId)
        {
            try
            {
                var page = await _blogService.GetByBlogTypeAsync(pageIndex, pageSize, blogTypeId);
                if (page == null)
                {
                    return NotFound(new ErrorResponse("No Records Found"));
                }
                return Ok(new ItemResponse<Paged<Blog>> { Item = page });
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex, "Error getting blogs by blog type.");
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemResponse<Paged<Blog>>>> SearchBlogs(int pageIndex, int pageSize, string query)
        {
            try
            {
                var page = await _blogService.SearchBlogsAsync(pageIndex, pageSize, query);
                if (page == null)
                {
                    return NotFound(new ErrorResponse("App Resource not found."));
                }
                return Ok(new ItemResponse<Paged<Blog>> { Item = page });
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex, "Error searching blogs.");
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }

        [HttpGet("author/paginate")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemResponse<Paged<Blog>>>> GetBlogByAuthorId(int authorId, int pageIndex, int pageSize)
        {
            try
            {
                var blogList = await _blogService.GetBlogByAuthorIdAsync(authorId, pageIndex, pageSize);
                if (blogList == null)
                {
                    return NotFound(new ErrorResponse("No Records Found"));
                }
                return Ok(new ItemResponse<Paged<Blog>> { Item = blogList });
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex, "Error getting blogs by author ID.");
                return StatusCode(500, new ErrorResponse(ex.Message));
            }
        }
    }
}
