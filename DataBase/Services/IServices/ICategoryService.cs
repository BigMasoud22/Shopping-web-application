using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Services.IServices
{
    public interface ICategoryService
    {
        public IEnumerable<Category> GetAll();
        public bool RemoveRange(List<Category> ShoppingCarts);
        public bool Remove(Category ShoppingCart);
        public Task<bool> AddAsync(Category ShoppingCart);
        public Task UpdateAsync(Category ShoppingCart);
        public Task<Category> FindFirstOrDefaultObjcetAsync(Expression<Func<Category, bool>> expression);
    }
}
