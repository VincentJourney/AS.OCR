using AS.OCR.Commom.Util;
using AS.OCR.Dapper;
using AS.OCR.Model.Entity;
using AS.OCR.Model.Response;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AS.OCR.Service
{
    public class AuthService : Infrastructure
    {
        private static readonly AccountDAO accountDAO = new AccountDAO();

        public TokenResponse CreateToken(string appid, string appsecret)
        {
            //PooledRedisClientHelper.Set<string>()
            var account = accountDAO.GetByAppId(appid);
            if (account == null || account.AppSecret != appsecret || account.Status == 0)
                throw new Exception("账号不存在或账号未启用");

            return BuildToken(account);
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
