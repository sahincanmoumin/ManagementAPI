using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Role;

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
            var user = _userRepository.GetById(dto.UserId);
            if (user == null)
                throw new Exception("User not found");  

            var role = _roleRepository.GetByName(dto.RoleName);
            if (role == null)
                throw new Exception($"Role '{dto.RoleName}' not found");

            if (_userRoleRepository.HasRole(dto.UserId, dto.RoleName))
                throw new Exception($"User already has '{dto.RoleName}' role");

            _userRoleRepository.AddUserRole(dto.UserId, role.Id);
        }

        public void RemoveRole(RemoveRoleDto dto)
        {
            var user = _userRepository.GetById(dto.UserId);
            if (user == null)
                throw new Exception("User not found");

            var role = _roleRepository.GetByName(dto.RoleName);
            if (role == null)
                throw new Exception($"Role '{dto.RoleName}' not found");

            if (!_userRoleRepository.HasRole(dto.UserId, dto.RoleName))
                throw new Exception($"User doesn't have '{dto.RoleName}' role");

            _userRoleRepository.RemoveUserRole(dto.UserId, role.Id);
        }

        public List<string> GetUserRoles(int userId)
        {
            var roles = _userRoleRepository.GetUserRoles(userId);
            return roles.Select(r => r.Name).ToList();
        }
    }
}