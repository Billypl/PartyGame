using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Models.AccountModels;
using PartyGame.Services;

namespace PartyGame.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPut("register")]
        public async Task<IActionResult> AddNewUser([FromBody] RegisterUserDto registerUserDto)
        {
            _accountService.RegisterUser(registerUserDto,"User");

            return Ok(new
                {
                    Message = "User created"
                }
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            LoginResultDto tokens = await _accountService.Login(loginUserDto);
            return Ok(tokens);
        }

        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpGet]
        public async Task<IActionResult> GetUserData()
        {
            AccountDetailsDto accountDetailsDto =  await _accountService.GetAccountDetails();
            return Ok(accountDetailsDto);
        }
    }
}
