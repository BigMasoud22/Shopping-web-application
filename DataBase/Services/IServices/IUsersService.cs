using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataBase.Services.IServices
{
    public interface IUsersService
    {
        public bool RemoveRange(List<User> Users);
        public bool Remove(User User);
        public Task<bool> AddAsync(User User);
        public void Update(User User);
        public void Update(User User,UserAddress address);
        public void UpsertAddress(string UserId,UserAddress address);
        public User FindFirstOrDefaultObjcet(Expression<Func<User, bool>> expression);
        public bool IsSuperAdminExitst();
        public bool SetComments(Comments comment,int productId, string Id);

    }
}
