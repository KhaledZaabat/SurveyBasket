namespace SurveyBasket.Helpers;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        return result switch
        {
            SuccessResult<T> success => controller.Ok(success.Value),
            FailureResult<T> failure => controller.Problem(
                statusCode: failure.Error.StatusCode,
                title: failure.Error.Code,
                detail: failure.Error.Description),
            _ => controller.StatusCode(500, new { message = "Unknown error" })
        };
    }

    public static ActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        return result switch
        {
            SuccessResult => controller.Ok(new { message = "Operation successful" }),
            FailureResult failure => controller.Problem(
                statusCode: failure.Error.StatusCode,
                title: failure.Error.Code,
                detail: failure.Error.Description),
            _ => controller.StatusCode(500, new { message = "Unknown error" })
        };
    }
}