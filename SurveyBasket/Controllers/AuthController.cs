using SurveyBasket.Helpers;
using SurveyBasket.Services.Authentication;

namespace SurveyBasket.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService _service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
         => (await _service.LoginAsync(request)).ToActionResult(this);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        => (await _service.RegisterAsync(request)).ToActionResult(this);

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
        => (await _service.RefreshAsync(request)).ToActionResult(this);



}

