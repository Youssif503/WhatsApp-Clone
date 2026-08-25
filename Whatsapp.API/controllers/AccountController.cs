using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using noon.Application.DTOs;
using Whatsapp.API.Helpers;
using Whatsapp.BLL.DTOs;
using Whatsapp.BLL.Services;
using Whatsapp.DAL.models;

namespace Whatsapp.API.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly JWToptions _jwt;
        private readonly AccountService _accountService;
        private readonly SignInManager<User> _signInManager;
        public AccountController(ILogger<AccountController> logger, 
            UserManager<User> userManager, IOptions<JWToptions> jwt,
            AccountService accountService,
            SignInManager<User> signInManager)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._jwt = jwt.Value;
            this._accountService = accountService;
            this._signInManager = signInManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserDto UserDto)
        {
            var userFromDb = await _userManager.FindByEmailAsync(UserDto.Email);
            if (userFromDb != null)
            {
                return BadRequest(Response<string>.Fail("Invalid Email Or Password"));
            }

            var newUser = new User
            {
                Email = UserDto.Email,
                UserName = UserDto.Email.Trim().Split("@")[0],
                First_Name = UserDto.FirstName,
                Last_Name = UserDto.LastName,
            };
            var result = await _userManager.CreateAsync(newUser,UserDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(Response<string>.Fail(result.Errors.First().Description));
            }
            
            var token  = await _accountService.GenerateAuthTokenAsync(newUser);
            return StatusCode(StatusCodes.Status201Created, Response<AuthTokenDto>.Success(token,"Account Created Successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginUserDto UserDto)
        {
            var userFromdb  = await _userManager.FindByEmailAsync(UserDto.Email);
            if (userFromdb == null)
                return Unauthorized(Response<string>.Fail("Email Not Found"));
            
            var result = await 
                _signInManager.CheckPasswordSignInAsync(userFromdb, UserDto.Password,false);
            
            if(!result.Succeeded)
                return BadRequest(Response<string>.Fail("Email Or Password Incorrect"));
            
            var token = await _accountService.GenerateAuthTokenAsync(userFromdb);
            return Ok(Response<AuthTokenDto>.Success(token,"Login Successfully"));
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest("Refresh token is required");

            var token = await _accountService.RefreshTokenAsync(request.RefreshToken);

            if (token is null)
                return Unauthorized("Invalid or expired refresh token");

            return Ok(Response<AuthTokenDto>.Success(token,"Refresh Token  Created Successfully"));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null)
                return Unauthorized(Response<string>.Fail("User Is Not LoggedIn"));
            
            bool result = 
                await _accountService.RevokeAllRefreshTokensAsync(userId);
            
            if (!result)
                return BadRequest(Response<string>.Fail("Something went wrong..."));
            
            return NoContent();
        }
    }
}
