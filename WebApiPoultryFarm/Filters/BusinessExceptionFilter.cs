using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiPoultryFarm.Api.Models;
using WebApiPoultryFarm.Share.Exeptions;

namespace WebApiPoultryFarm.Api.Filters
{
    /// <summary>
    /// Convert BusinessException into a 400 response with ApiResponse.Fail
    /// </summary>
    public class BusinessExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is BusinessException bex)
            {
                // Map BusinessException -> 400 BadRequest
                var payload = ApiResponse<string>.Fail(bex.Message);
                context.Result = new BadRequestObjectResult(payload);
                context.ExceptionHandled = true;
                return;
            }

            // For unhandled exceptions, hide internal error in prod
            var generic = ApiResponse<string>.Fail("Đã xảy ra lỗi không mong muốn.");
            context.Result = new ObjectResult(generic)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
