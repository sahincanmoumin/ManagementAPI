using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using BusinessLayer.Utilities;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Auth;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using EntityLayer.Constants;

namespace BusinessLayer.Concrete
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly string _jwtSecretKey;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            string jwtSecretKey)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _jwtSecretKey = jwtSecretKey;
        }

        public async Task<User> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
                throw new BusinessException(ErrorKeys.UsernameAlreadyExists);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Balance = 1000,
                CreatedAt = DateTime.Now,
            };

            await _userRepository.AddAsync(user);

            var userRole = await _roleRepository.GetByNameAsync("User");
            if (userRole != null)
            {
                await _userRoleRepository.AddUserRoleAsync(user.Id, userRole.Id);
            }

            return user;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null)
                throw new BusinessException(ErrorKeys.InvalidCredentials);

            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                throw new BusinessException(ErrorKeys.InvalidCredentials);

            var roles = await _userRoleRepository.GetRolesAsync(user.Id);
            var roleNames = string.Join(",", roles.Select(r => r.Name));

            return JwtHelper.GenerateToken(user.Id, user.Username, roleNames, _jwtSecretKey);
        }
    }
}