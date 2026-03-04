using System.Threading.Tasks;
using EntityLayer.DTOs.Auth;
using EntityLayer.Entities;

namespace BusinessLayer.Abstract
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}