using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using backendProject.Data;
using backendProject.Data.Tables;
using backendProject.Objects;
using backendProject.Objects.ApiRequests;
using backendProject.Objects.ApiResponses;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backendProject.Controllers.AccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private TokenObject CreateAcessToken(Identity identity)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, identity.UniqueId.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, identity.Profile.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, identity.Profile.LastName),
                new Claim(JwtRegisteredClaimNames.Email, identity.Profile.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (identity.Admin != null)
            {
                claims.Add(new Claim("admin", "true"));
            }

            var date = DateTime.UtcNow;
            var date_expire = DateTime.UtcNow.AddMinutes(45);

            var securityToken = new JwtSecurityToken
            (
                issuer: Startup.Issuer,
                audience: Startup.Audience,
                claims: claims,
                expires: date_expire,
                notBefore: date,
                signingCredentials: new SigningCredentials(Startup.SecurityKey, SecurityAlgorithms.HmacSha256)
            );

            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(securityToken);

            return new TokenObject
            {
                Token = accessToken,
                ExpireUtc = date_expire
            };
        }

        private async Task<TokenObject> CreateRefreshToken(Identity identity)
        {
            var date = DateTime.UtcNow;
            var date_expire = DateTime.UtcNow.AddDays(30);

            var refreshToken = new RefreshToken
            {
                UniqueId = identity.UniqueId,
                IssuedUtc = date,
                ExpiresUtc = date_expire
            };

            await _dbContext.AddAsync(refreshToken);

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return new TokenObject
                {
                    Token = refreshToken.Token.ToString(),
                    ExpireUtc = date_expire
                };
            }

            return null;
        }

        private async Task<TokensResponse> GenerateTokens(Identity identity)
        {
            var accessToken = CreateAcessToken(identity);
            var refreshToken = await CreateRefreshToken(identity);

            return new TokensResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<Identity> AddOrGetIdentity(string issuer, string subjectid, string firstname, string lastname, string email)
        {
            var identity = await _dbContext.Identity.Include(x => x.Profile).Include(x => x.Admin).FirstOrDefaultAsync(x => x.Issuer.Equals(issuer) && x.SubjectId.Equals(subjectid));
            if (identity == null)
            {
                identity = new Identity
                {
                    Issuer = issuer,
                    SubjectId = subjectid,
                    Profile = new Profile
                    {
                        FirstName = firstname,
                        LastName = lastname,
                        Email = email
                    }
                };
                await _dbContext.AddAsync(identity);
                await _dbContext.SaveChangesAsync();
            }

            return identity;
        }

        private async Task<ActionResult> UseGoogleIdToken(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

                var identity = await AddOrGetIdentity("google", payload.Subject, payload.GivenName, payload.FamilyName, payload.Email);

                var tokens = await GenerateTokens(identity);

                return Ok(tokens);
            }
            catch (Google.Apis.Auth.InvalidJwtException)
            {
                return Unauthorized("Invalid google token!");
            }
        }

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<ActionResult> TokenRequest([FromBody]TokenRequest tokenRequest)
        {
            if (tokenRequest.Issuer.Equals("google"))
            {
                return await UseGoogleIdToken(tokenRequest.IdToken);
            }

            return Unauthorized("Invalid login request!");
        }

        [HttpPost("token/refresh")]
        [AllowAnonymous]
        public async Task<ActionResult> TokenRefreshRequest([FromBody]TokenRefreshRequest tokenRefreshRequest)
        {
            var refreshToken = await _dbContext.RefreshToken.FirstOrDefaultAsync(x => x.Token.Equals(new Guid(tokenRefreshRequest.Token)));

            if (refreshToken != null && refreshToken.ExpiresUtc > DateTime.UtcNow)
            {
                var identity = await _dbContext.Identity.Include(x => x.Profile).Include(x => x.Admin).FirstOrDefaultAsync(x => x.UniqueId.Equals(refreshToken.UniqueId));
                if (identity != null)
                {
                    _dbContext.Remove(refreshToken);
                    if (await _dbContext.SaveChangesAsync() > 0)
                    {
                        return Ok(await GenerateTokens(identity));
                    }
                }

                return Unauthorized("Unauthorized refresh token!");
            }
            else
            {
                if (refreshToken != null)
                {
                    _dbContext.Remove(refreshToken);
                    await _dbContext.SaveChangesAsync();
                }

                return Unauthorized("Unauthorized refresh token!");
            }
        }
    }
}
