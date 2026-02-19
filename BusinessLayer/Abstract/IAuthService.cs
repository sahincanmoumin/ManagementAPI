using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityLayer.DTOs.Auth;
using EntityLayer.Entities;

namespace BusinessLayer.Abstract
{
    public interface IAuthService
    {
        User Register(RegisterDto dto);
        string Login(LoginDto dto);
    }
}