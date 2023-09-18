using JwtAuthenticationManager.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManager
{
    public class JwtTokenHandler
    {
        public const string JWT_SECURITY_KEY = "7DübelinCözemeyeceğiBirKeyDeğeriYeralmalıBurada123qaZ*";
        public const int JWT_TOKEN_VALIDITY_MIN = 20;

        private readonly List<UserAccount> _users;

        public JwtTokenHandler()
        {
            _users = new List<UserAccount>
                {
                    new UserAccount { UserName= "Sezer", Password= "123456qaZ*", Role = "Administrator" },
                    new UserAccount { UserName = "Guest", Password= "123456", Role= "Guest" },
                };
        }

        public AuthenticationResponse? GenerateJwtToken(AuthenticationRequest authRequest)
        {
            if (string.IsNullOrWhiteSpace(authRequest?.UserName) || string.IsNullOrWhiteSpace(authRequest?.Password))
                return null;

            var userAccount = _users.Where(u => u.UserName == authRequest.UserName && u.Password == authRequest.Password).FirstOrDefault();
            if (userAccount == null) return null;

            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MIN);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, userAccount.UserName),
                // Web Api'da kullanım için yeterli bir tanımlama
                //new Claim(ClaimTypes.Role, userAccount.Role)

                // Fakat Ocelot configurration dosyasında Rol bazlı authentication yapabilmek için Role'u string bir değer olarak atamalıyız.
                // ClaimTypes.Role link halinde bir string değer olduğu için ocelot config dosyasında hataya sebebiyet veriyor.
                new Claim("Role", userAccount.Role)
            });

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials,
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return new AuthenticationResponse
            {
                UserName = userAccount.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token
            };

        }
    }
}
