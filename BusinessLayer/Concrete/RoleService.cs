using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Constants;
using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.Role;
using EntityLayer.DTOs.User;
using EntityLayer.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class RoleService : IRoleService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public RoleService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task AssignRoleAsync(AssignRoleDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            var role = await _roleRepository.GetByNameAsync(dto.RoleName) ?? throw new BusinessException(ErrorKeys.RoleNotFound);

            if (await _userRoleRepository.HasRoleAsync(dto.UserId, dto.RoleName))
                throw new BusinessException(ErrorKeys.UserAlreadyHasRole);

            await _userRoleRepository.AddUserRoleAsync(dto.UserId, role.Id);
        }

        public async Task RemoveRoleAsync(RemoveRoleDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            var role = await _roleRepository.GetByNameAsync(dto.RoleName) ?? throw new BusinessException(ErrorKeys.RoleNotFound);

            if (!await _userRoleRepository.HasRoleAsync(dto.UserId, dto.RoleName))
                throw new BusinessException(ErrorKeys.UserDoesNotHaveRole);

            await _userRoleRepository.RemoveUserRoleAsync(dto.UserId, role.Id);
        }

        public async Task<PagedResponse<UserListDto>> GetUserRolesAsync(RoleFilterDto filter)
        {
            var query = _userRepository.GetQueryable()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == filter.Name));
            }

            var totalRecords = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                }).ToListAsync();

            return new PagedResponse<UserListDto>(users, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<List<string>> GetRolesAsync(int userId)
        {
            var roles = await _userRoleRepository.GetRolesAsync(userId);
            return roles.Select(r => r.Name).ToList();
        }
    }
}