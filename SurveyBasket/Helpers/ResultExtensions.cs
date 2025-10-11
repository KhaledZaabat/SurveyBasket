namespace SurveyBasket.Helpers;

public static class ResultExtensions
{

    public static ActionResult<T> ToActionResult<T>(this Result<T> result, HttpContext? context = null)
    {
        return result switch
        {
            SuccessResult<T> success => new OkObjectResult(success.Value),

            FailureResult<T> failure => failure.ToProblem(context),

            _ => new ObjectResult(new { message = "Unknown error" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };
    }


    public static IActionResult ToActionResult(this Result result, HttpContext? context = null)
    {
        return result switch
        {
            SuccessResult => new OkObjectResult(new { message = "Operation successful" }),
            FailureResult failure => failure.ToProblem(context),
            _ => new ObjectResult(new { message = "Unknown error" })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };
    }

    public static ActionResult ToProblem<T>(this Result<T> result, HttpContext? context = null)
    {

        var problem = Results.Problem(statusCode: result.Error.StatusCode);

        ProblemDetails problemDetails = (problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails)!;
        problemDetails.Extensions["Errors"] = new
        {
            title = result.Error.Code,
            Description = result.Error.Description,
        };
        problemDetails.Extensions["Instance"] = context != null ? $"{context.Request.Method} {context.Request.Path}" : null;
        return new ObjectResult(problemDetails)
        {
            StatusCode = result.Error.StatusCode,
            ContentTypes = { "application/problem+json" }
        };




    }


    public static IActionResult ToProblem(this Result result, HttpContext? context = null)
    {

        var problem = Results.Problem(statusCode: result.Error.StatusCode);

        ProblemDetails problemDetails = (problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails)!;
        problemDetails.Extensions["Errors"] = new
        {
            title = result.Error.Code,
            Description = result.Error.Description,
        };
        problemDetails.Extensions["Instance"] = context != null ? $"{context.Request.Method} {context.Request.Path}" : null;

        return new ObjectResult(problemDetails)
        {
            StatusCode = result.Error.StatusCode,
            ContentTypes = { "application/problem+json" }
        };

    }
}
