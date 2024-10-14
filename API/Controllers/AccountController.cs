using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [AllowAnonymous]
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
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
    {
      if (await _userManager.Users.AnyAsync(user => user.UserName == registerDto.Username))
      {
        return BadRequest("Username is already taken");
      }
      if (await _userManager.Users.AnyAsync(user => user.Email == registerDto.Email))
      {
        return BadRequest("Email is already taken");
      }

      var user = new AppUser
      {
        DisplayName = registerDto.DisplayName,
        Email = registerDto.Email,
        UserName = registerDto.Username
      };

      var result = await _userManager.CreateAsync(user, registerDto.Password);

      if (result.Succeeded)
      {
        return new UserDTO
        {
          DisplayName = user.DisplayName,
          Image = null,
          Token = _tokenService.CreateToken(user),
          Username = user.UserName
        };
      }

      return BadRequest(result.Errors);
    }
  }
}