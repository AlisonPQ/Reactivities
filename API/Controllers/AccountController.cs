using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : ControllerBase
  {
    
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;
    
    public AccountController(UserManager<AppUser> userManager, TokenService tokenService)
    {
      _tokenService = tokenService;
      _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
    {
      var user = await _userManager.FindByEmailAsync(loginDto.Email);

      if (user == null) return Unauthorized();

      var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

      if (result)
      {
        return new UserDTO
        {
          DisplayName = user.DisplayName,
          Image = null,
          Token = _tokenService.CreateToken(user),
          Username = user.UserName
        };
      }

      return Unauthorized();
    }
  }
}