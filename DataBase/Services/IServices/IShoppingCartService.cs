using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Services.IServices
{
    public interface IShoppingCartService
    {
        public IEnumerable<ShoppingCartItem> GetAll();
        public IEnumerable<ShoppingCartItem> FindAllUserCarts(string userId);
        public bool RemoveRange(List<ShoppingCartItem> ShoppingCarts);
        public bool Remove(ShoppingCartItem ShoppingCart);
        public Task<bool> AddAsync(ShoppingCartItem ShoppingCart);
        public Task UpdateAsync(ShoppingCartItem ShoppingCart);
        public Task<ShoppingCartItem> FindFirstOrDefaultObjcetAsync(Expression<Func<ShoppingCartItem, bool>> expression);
        public int increamentCount(ShoppingCartItem cart , int count);
    }
}
