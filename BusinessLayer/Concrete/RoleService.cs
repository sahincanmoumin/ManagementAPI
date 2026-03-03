using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Constants;
using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.Role;
using EntityLayer.DTOs.User;
using EntityLayer.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

        public void AssignRole(AssignRoleDto dto)
        {
            var user = _userRepository.GetById(dto.UserId) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            var role = _roleRepository.GetByName(dto.RoleName) ?? throw new BusinessException(ErrorKeys.RoleNotFound);

            if (_userRoleRepository.HasRole(dto.UserId, dto.RoleName))
                throw new BusinessException(ErrorKeys.UserAlreadyHasRole);

            _userRoleRepository.AddUserRole(dto.UserId, role.Id);
        }

        public void RemoveRole(RemoveRoleDto dto)
        {
            var user = _userRepository.GetById(dto.UserId) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            var role = _roleRepository.GetByName(dto.RoleName) ?? throw new BusinessException(ErrorKeys.RoleNotFound);

            if (!_userRoleRepository.HasRole(dto.UserId, dto.RoleName))  
                    throw new BusinessException(ErrorKeys.UserDoesNotHaveRole);

            _userRoleRepository.RemoveUserRole(dto.UserId, role.Id);
        }

        public PagedResponse<UserListDto> GetUserRoles(RoleFilterDto filter)
        {
            var query = _userRepository.GetQueryable()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == filter.Name));
            }

            var totalRecords = query.Count();

            var users = query
                .OrderBy(u => u.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                }).ToList();

            return new PagedResponse<UserListDto>(users, totalRecords, filter.PageNumber, filter.PageSize);
        }
        public List<string> GetRoles(int userId)

        {

            var roles = _userRoleRepository.GetRoles(userId);

            return roles.Select(r => r.Name).ToList();

        }
    }
}