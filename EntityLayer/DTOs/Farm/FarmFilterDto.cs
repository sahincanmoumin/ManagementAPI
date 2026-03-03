using EntityLayer.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Farm
{
    public class FarmFilterDto : PaginationFilter
    {
        public string Name { get; set; }
    }
}
