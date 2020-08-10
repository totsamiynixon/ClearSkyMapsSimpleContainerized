using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Areas.Admin.Application.Users.Queries.DTO;

namespace Web.Areas.Admin.Application.Users.Queries
{
    public interface IUserQueries
    {
        Task<IEnumerable<UserListItemDTO>> GetUsersAsync();
        Task<UserDetailsDTO> GetUserAsync(string userId);
    }
}