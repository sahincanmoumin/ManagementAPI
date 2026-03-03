using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.User;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IUserService
    {
        User GetById(int id);
        void UpdateUser(int id, UpdateUserDto dto);
        decimal GetBalance(int id);
        PagedResponse<UserListDto> GetAllUsers(UserFilterDto filter);

    }
}