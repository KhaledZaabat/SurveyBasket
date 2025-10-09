using Microsoft.AspNetCore.Identity;
using SurveyBasket.Services.Authentication;

namespace SurveyBasket.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService _service) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {

            AuthResponse? response = await _service.Login(request);


            if (response == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }


            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {

            (bool succeded, IEnumerable<IdentityError>? errors) result = await _service.Register(request);



            if (result.succeded)
                return Ok(new { message = "User created successfully" });

            return BadRequest(result.errors);



        }


    }
}
