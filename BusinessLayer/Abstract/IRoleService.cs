using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.Role;
using EntityLayer.DTOs.User;

namespace BusinessLayer.Abstract
{
    public interface IRoleService
    {
        Task AssignRoleAsync(AssignRoleDto dto);
        Task RemoveRoleAsync(RemoveRoleDto dto);
        Task<PagedResponse<UserListDto>> GetUserRolesAsync(RoleFilterDto filter);
        Task<List<string>> GetRolesAsync(int userId);
    }
}