using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PartyGame.Models.GameModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PartyGame.Services
{
    public interface IGameTokenService
    {
        string GenerateGuessSessionToken(StartDataDto startDataDto);
    }

    public class GameTokenService : IGameTokenService
    {

        private readonly AuthenticationSettings _authenticationSettings;
        public GameTokenService(IOptions<AuthenticationSettings> authenticationSettings)
        {
            _authenticationSettings = authenticationSettings.Value;
        }

        public string GenerateGuessSessionToken(StartDataDto startDataDto)
        {
            var expiration = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, startDataDto.Nickname),
                new Claim("difficulty", startDataDto.Difficulty),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expiration).ToUnixTimeSeconds().ToString()),
                new Claim("token_type","game"),
                
            };
            

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _authenticationSettings.JwtIssuer,
                audience: _authenticationSettings.JwtIssuer,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
