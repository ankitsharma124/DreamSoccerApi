using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;
using DreamSoccer.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DreamSoccer.Repository.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<string>> LoginAsync(string email, string password)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Password is wrong";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            return response;
        }

        public async Task<ServiceResponse<int>> RegisterAsync(User user, string password)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (await UserExistAsync(user.Email))
            {
                response.Success = false;
                response.Message = "User is already registered";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExistAsync(string email)
        {
            if (await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string strPassword, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(strPassword));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Name, user.Email.ToString ()),
                new Claim (ClaimTypes.Role, user.Role.ToString ())
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
            );

            SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}