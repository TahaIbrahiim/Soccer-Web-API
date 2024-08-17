using Microsoft.AspNetCore.Mvc;
using SoccerAPI.Dto;
using SoccerAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLoginDto loginDto)
    {
        
        var user = AuthenticateUser(loginDto);

        if (user != null)
        {
            var token = _jwtTokenService.GenerateToken(user);
            return Ok(new { token });
        }

        return Unauthorized("Invalid credentials");
    }

    private User AuthenticateUser(UserLoginDto loginDto)
    {
        
        if (loginDto.Username == "taha" && loginDto.Password == "123456")
        {
           
            return new User
            {
                Username = "Taha",
                Role = "Admin" 
            };
        }

        
        return null;
    }
}


