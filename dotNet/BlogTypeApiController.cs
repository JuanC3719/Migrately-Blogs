using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Migrately.Services.Interfaces;
using Migrately.Web.Controllers;
using Migrately.Web.Models.Responses;
using Migrately.Models.Domain;
using System;
using System.Collections.Generic;

namespace Migrately.Web.Api.Controllers
{
    [Route("api/blogtype")]
    [ApiController]
    public class BlogTypeApiController : BaseApiController
    {
        private readonly IBlogTypeService _blogTypeService;
        private readonly IAuthenticationService<int> _authService;

        public BlogTypeApiController(
            IBlogTypeService blogTypeService,
            IAuthenticationService<int> authService,
            ILogger<BlogTypeApiController> logger) : base(logger)
        {
            _blogTypeService = blogTypeService ?? throw new ArgumentNullException(nameof(blogTypeService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet("all")]
        public ActionResult<ItemsResponse<BlogType>> GetAll()
        {
            int statusCode = 200;
            BaseResponse response;

            try
            {
                List<BlogType> list = _blogTypeService.GetAll();

                if (list == null)
                {
                    statusCode = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<BlogType> { Items = list };
                }
            }
            catch (Exception ex)
            {
                statusCode = 500;
                response = new ErrorResponse(ex.Message);
                Logger.LogError(ex, "An error occurred while processing the request.");
            }

            return StatusCode(statusCode, response);
        }
    }
}
