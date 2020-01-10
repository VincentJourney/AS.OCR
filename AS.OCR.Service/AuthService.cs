using AS.OCR.Commom.Configuration;
using AS.OCR.IDAO;
using AS.OCR.IService;
using AS.OCR.Model.Entity;
using AS.OCR.Model.Response;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AS.OCR.Service
{
    public class AuthService : Infrastructure, IAuthService
    {
        private ILogger _logger;
        private IAccountDAO _accountDAO { get; }
        public AuthService(ILoggerFactory loggerFactory, IAccountDAO accountDAO)
        {
            _logger = loggerFactory.CreateLogger(typeof(AuthService));
            _accountDAO = accountDAO;
        }
        public TokenResponse CreateToken(string appid, string appsecret)
        {
            Account account = _accountDAO.GetByAppId(appid);
            if (account == null || account.AppSecret != appsecret || account.Status == 0)
                throw new Exception("账号不存在或账号未启用");

            var RedisKey = $"GetToken_AppId_{appid}";
            if (PooledRedisClientHelper.ContainsKey(RedisKey))
                return PooledRedisClientHelper.GetT<TokenResponse>(RedisKey);

            var token = BuildToken(account);
            PooledRedisClientHelper.Set<TokenResponse>(RedisKey, token, TimeSpan.FromMinutes(30));
            return token;
        }

        private TokenResponse BuildToken(Account account)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.UserData, account.Id.ToString()),
                new Claim(ClaimTypes.Sid,account.AppId),
                new Claim(ClaimTypes.PrimarySid, account.AppSecret)
            };

            DateTime expiredtime = DateTime.Now.AddMinutes(30);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationUtil.TokenKey));
            var token = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: claims,
                expires: expiredtime,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new TokenResponse
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiryTime = expiredtime
            };
        }
    }
}
