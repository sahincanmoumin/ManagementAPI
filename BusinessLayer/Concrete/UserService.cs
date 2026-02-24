using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BusinessLayer.Abstract;
using BusinessLayer.Utilities;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.User;
using EntityLayer.Entities;

namespace BusinessLayer.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetById(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                throw new Exception("User not found");
            return user;
        }

        public void UpdateUser(int id, UpdateUserDto dto)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                throw new Exception("User not found");

            if (!string.IsNullOrEmpty(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = PasswordHelper.HashPassword(dto.Password);

            _userRepository.Update(user);
        }

        public decimal GetBalance(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                throw new Exception("User not found");
            return user.Balance;
        }
        public List<User> GetAllUsers()
        {
            return _userRepository.GetAll();
        }
    }
}