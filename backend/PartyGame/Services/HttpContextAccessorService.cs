using Microsoft.AspNetCore.Http;
using PartyGame.Entities;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using PartyGame.Models.AccountModels;

namespace PartyGame.Services
{
    public interface IHttpContextAccessorService
    {
        string GetTokenFromHeader();
        AccountDetailsFromTokenDto GetAuthenticatedUserProfile();
        bool IsUserLoggedIn();
        bool IsTokenExist();
    }

    public class HttpContextAccessorService : IHttpContextAccessorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextAccessorService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetTokenFromHeader()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new KeyNotFoundException("Authorization header or token is missing.");
            }

            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                throw new KeyNotFoundException("Token was not found in the Authorization header.");
            }

            return token;
        }

        public AccountDetailsFromTokenDto GetAuthenticatedUserProfile()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var nickname = user.FindFirst(ClaimTypes.Name)?.Value;

            return new AccountDetailsFromTokenDto
            {
                UserId = userId ?? throw new KeyNotFoundException("User ID not found in claims."),
                Email = email ?? throw new KeyNotFoundException("Email not found in claims."),
                Role = role ?? throw new KeyNotFoundException("Role not found in claims."),
                Nickname = nickname ?? throw new KeyNotFoundException("Nickname not found in claims.")
            };
        }

        public bool IsUserLoggedIn()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return false;
            }

            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            return !string.IsNullOrEmpty(role); 
        }

        public bool IsTokenExist()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (authorizationHeader.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }

    }
}

