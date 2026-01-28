using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SOL.MS.Security.Application.Common;

namespace SOL.MS.Security.Api.Filters
{
    public class ValidateModelStateFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                var result = Result.Failure<object>(errors);

                context.Result = new BadRequestObjectResult(result);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
