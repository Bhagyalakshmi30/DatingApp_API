using DatingApp.DTO;
using DatingApp.Models;

namespace DatingApp.Interface
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task <bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);      
        Task<AppUser> GetUserByUsernameAsyc(string username);
        Task <IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto> GetMemberAsync(string username);



    }
}
