using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Migrately.Services.Interfaces;
using Migrately.Services;
using Migrately.Web.Controllers;
using Migrately.Web.Models.Responses;
using System.Collections.Generic;
using System;
using Migrately.Models.Domain;

namespace Migrately.Web.Api.Controllers
{
    [Route("api/blogtype")]
    [ApiController]
    public class BlogTypeApiController : BaseApiController
    {
        private IBlogTypeService _blogTypeService;
        private IAuthenticationService<int> _authService;

        public BlogTypeApiController(IBlogTypeService blogTypeService
            , IAuthenticationService<int> authService
            , ILogger<ExamplesApiController> logger) : base(logger)
        {
            _blogTypeService = blogTypeService;
            _authService = authService;

        }

        [HttpGet("all")]
        public ActionResult<ItemsResponse<BlogType>> GetAll()
        {
            int code = 200;
            BaseResponse response = null;//do not declare an instance.

            try
            {
                List<BlogType> list = _blogTypeService.GetAll();

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemsResponse<BlogType> { Items = list };
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
    }
}
