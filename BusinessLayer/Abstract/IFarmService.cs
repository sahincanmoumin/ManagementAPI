using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityLayer.DTOs.Farm;
using EntityLayer.Entities;

namespace BusinessLayer.Abstract
{
    public interface IFarmService
    {
        Farm CreateFarm(int userId, CreateFarmDto dto);
        Farm GetById(int id);
        List<Farm> GetUserFarms(int userId);
        void UpdateFarm(int id, UpdateFarmDto dto);
        void DeleteFarm(int id);
    }
}