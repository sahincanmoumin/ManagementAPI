using EntityLayer.DTOs.Pagination;

namespace EntityLayer.DTOs.User
{
    public class UserFilterDto : PaginationFilter
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
    }
}