using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PartyGame.Entities;
using PartyGame.Models.AccountModels;
using PartyGame.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace PartyGame.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto registerUserDto, string Role); 
        Task<LoginResultDto> Login(LoginUserDto loginUserDto);
        Task<AccountDetailsDto> GetAccountDetails();
        string RefreshSession();

    }

    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessorService _contextAccessorService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IAccountTokenService _accountTokenService;

        public AccountService(IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher,
            IHttpContextAccessorService contextAccessorService, IMapper mapper,
            IOptions<AuthenticationSettings> authenticationSettings,
            IAccountTokenService accountTokenService)
        {
            _accountRepository =accountRepository;
            _contextAccessorService = contextAccessorService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings.Value;
            _accountTokenService = accountTokenService;
        }
        public async void RegisterUser(RegisterUserDto registerUserDto,string role)
        {
            if (await _accountRepository.GetUserByNicknameOrEmailAsync(registerUserDto.Nickname) is not null)
            {
                throw new BadHttpRequestException("Nickname is already used");
            }


            if (await _accountRepository.GetUserByNicknameOrEmailAsync(registerUserDto.Email) is not null)
            {
                throw new BadHttpRequestException("Email is already used");
            }

            var newUser = new User()
            {
                Email = registerUserDto.Email,
                Nickname = registerUserDto.Nickname,
                CreatedAt = DateTime.Now,
                Role = role,

            };

            var passwordHash = _passwordHasher.HashPassword(newUser, registerUserDto.Password);
            newUser.PasswordHash = passwordHash;


            _accountRepository.CreateAsync(newUser);
        }

        public async Task<LoginResultDto> Login(LoginUserDto loginUserDto)
        {
            var user = await _accountRepository.GetUserByNicknameOrEmailAsync(loginUserDto.NicknameOrEmail);

            if (user is null)
            {
                throw new KeyNotFoundException("Invalid nickname or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new KeyNotFoundException("Invalid nickname or password");
            }

            string token = _accountTokenService.GenerateAccountToken(user);
            string refreshToken = _accountTokenService.GenerateRefreshToken(user);

            return new LoginResultDto
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<AccountDetailsDto> GetAccountDetails()
        {
            AccountDetailsFromTokenDto tokenData = _contextAccessorService.GetAuthenticatedUserProfile();

            User account = await _accountRepository.GetAsync(tokenData.UserId);

            AccountDetailsDto accountDetailsDto = _mapper.Map<AccountDetailsDto>(account);

            return accountDetailsDto;
        }

        public string RefreshSession()
        {
            throw new NotImplementedException();
        }
    }
}
