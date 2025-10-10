using SurveyBasket.Services.Authentication;

namespace SurveyBasket.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService _service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        Result<AuthResponse> result = await _service.LoginAsync(request);

        return result switch
        {
            SuccessResult<AuthResponse> success => Ok(success.Value),
            FailureResult<AuthResponse> failure => StatusCode(failure.Error.StatusCode, new { failure.Error.Description }),
            _ => StatusCode(500, new { message = "Unknown error" })
        };
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        Result result = await _service.RegisterAsync(request);

        return result switch
        {
            SuccessResult => Ok(new { message = "User created successfully" }),
            FailureResult failure => StatusCode(failure.Error.StatusCode, new { failure.Error.Description }),
            _ => StatusCode(500, new { message = "Unknown error" })
        };
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
    {
        Result<AuthResponse> result = await _service.RefreshAsync(request);

        return result switch
        {
            SuccessResult<AuthResponse> success => Ok(success.Value),
            FailureResult<AuthResponse> failure => StatusCode(failure.Error.StatusCode, new { failure.Error.Description }),
            _ => StatusCode(500, new { message = "Unknown error" })
        };
    }
}

