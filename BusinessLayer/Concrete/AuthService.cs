using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BusinessLayer.Abstract;
using BusinessLayer.Utilities;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Auth;
using EntityLayer.Entities;

namespace BusinessLayer.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly string _jwtSecretKey;

        public AuthService(IUserRepository userRepository, string jwtSecretKey)
        {
            _userRepository = userRepository;
            _jwtSecretKey = jwtSecretKey;
        }

        public User Register(RegisterDto dto)
        {
            var existingUser = _userRepository.GetByUsername(dto.Username);
            if (existingUser != null)
                throw new Exception("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Balance = 1000,
                CreatedAt = DateTime.Now
            };

            _userRepository.Add(user);
            return user;
        }

        public string Login(LoginDto dto)
        {
            var user = _userRepository.GetByUsername(dto.Username);
            if (user == null)
                throw new Exception("Invalid username or password");

            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                throw new Exception("Invalid username or password");

            return JwtHelper.GenerateToken(user.Id, user.Username, _jwtSecretKey);
        }
    }
}