using EntityLayer.DTOs.Farm;
using EntityLayer.DTOs.Pagination;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IFarmService
    {
        Farm CreateFarm(int userId, CreateFarmDto dto);
        Farm GetById(int id);
        PagedResponse<FarmListDto> GetUserFarms(int userId,FarmFilterDto filter);
        void UpdateFarm(int id, UpdateFarmDto dto);
        void DeleteFarm(int id);
    }
}