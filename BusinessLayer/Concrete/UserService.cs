using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using BusinessLayer.Utilities;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.User;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using EntityLayer.DTOs.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new BusinessException(ErrorKeys.UserNotFound);
            return user;
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            if (!string.IsNullOrEmpty(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = PasswordHelper.HashPassword(dto.Password);

            await _userRepository.UpdateAsync(user);
        }

        public async Task<decimal> GetBalanceAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new BusinessException(ErrorKeys.UserNotFound);
            return user.Balance;
        }

        public async Task<PagedResponse<UserListDto>> GetAllUsersAsync(UserFilterDto filter)
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

            int totalRecords = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Balance = u.Balance
                }).ToListAsync();

            return new PagedResponse<UserListDto>(users, totalRecords, filter.PageNumber, filter.PageSize);
        }
    }
}