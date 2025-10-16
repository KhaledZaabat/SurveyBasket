using SurveyBasket.Services.Authentication;

namespace SurveyBasket.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService _service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
        => (await _service.LoginAsync(request, cancellationToken))
            .ToActionResult(context: HttpContext);

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
        => (await _service.RegisterAsync(request, cancellationToken))
            .ToActionResult(context: HttpContext);

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
        => (await _service.RefreshAsync(request, cancellationToken))
            .ToActionResult(context: HttpContext);

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailRequest request,
        CancellationToken cancellationToken)
        => (await _service.ConfirmEmailAsync(request, cancellationToken))
            .ToActionResult(context: HttpContext);


    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail(
    [FromBody] ResendConfirmationEmailRequest request,
    CancellationToken cancellationToken)
    => (await _service.ResendConfirmationEmailAsync(request, cancellationToken))
        .ToActionResult(context: HttpContext);





}


