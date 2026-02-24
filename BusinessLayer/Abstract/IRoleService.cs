using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer.DTOs.Role;

namespace BusinessLayer.Abstract
{
    public interface IRoleService
    {
        void AssignRole(AssignRoleDto dto);
        void RemoveRole(RemoveRoleDto dto);
        
        List<string> GetUserRoles(int userId);
    }
}