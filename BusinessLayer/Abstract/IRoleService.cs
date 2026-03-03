using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.Role;
using EntityLayer.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IRoleService
    {
        void AssignRole(AssignRoleDto dto);
        void RemoveRole(RemoveRoleDto dto);
        PagedResponse<UserListDto> GetUserRoles(RoleFilterDto filter);
        List<string> GetRoles(int userId);


    }
}