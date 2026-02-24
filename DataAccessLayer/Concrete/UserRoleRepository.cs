using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;

        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Role> GetUserRoles(int userId)
        {
            return _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToList();
        }

        public void AddUserRole(int userId, int roleId)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };
            _context.UserRoles.Add(userRole);
            _context.SaveChanges();
        }

        public void RemoveUserRole(int userId, int roleId)
        {
            var userRole = _context.UserRoles
                .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                _context.SaveChanges();
            }
        }
        

        public bool HasRole(int userId, string roleName)
        {
            return _context.UserRoles
                .Include(ur => ur.Role)
                .Any(ur => ur.UserId == userId && ur.Role.Name == roleName);
        }
    }
}