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
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using EntityLayer.DTOs.Pagination;

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
            var user = _userRepository.GetById(id) ?? throw new BusinessException(ErrorKeys.UserNotFound);
            return user;
        }

        public void UpdateUser(int id, UpdateUserDto dto)
        {
            var user = _userRepository.GetById(id) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            if (!string.IsNullOrEmpty(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = PasswordHelper.HashPassword(dto.Password);

            _userRepository.Update(user);
        }

        public decimal GetBalance(int id)
        {
            var user = _userRepository.GetById(id) ?? throw new BusinessException(ErrorKeys.UserNotFound);
            return user.Balance;
        }
        public PagedResponse<UserListDto> GetAllUsers(UserFilterDto filter)
        {
            var query = _userRepository.GetQueryable();

            if (filter.Id.HasValue)
            {
                query = query.Where(u => u.Id == filter.Id.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.UserName))
            {
                query = query.Where(u => u.Username.Contains(filter.UserName));
            }

            int totalRecords = query.Count();

            var users = query
                .OrderBy(u => u.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Balance = u.Balance 
                }).ToList();

            return new PagedResponse<UserListDto>(users, totalRecords, filter.PageNumber, filter.PageSize);
        }
    }   
}