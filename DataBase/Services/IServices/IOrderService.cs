using Models;
using System.Linq.Expressions;

namespace DataBase.Services.IServices
{
    public interface IOrderService
    {
        public IEnumerable<Order> GetAll();
        public bool RemoveRange(List<Order> ShoppingCarts);
        public bool Remove(Order ShoppingCart);
        public Task<bool> AddAsync(Order ShoppingCart);
        public Task UpdateAsync(Order ShoppingCart);
        public Task<Order> FindFirstOrDefaultObjcetAsync(Expression<Func<Order, bool>> expression);
    }
}
