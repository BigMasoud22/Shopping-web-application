using DataBase.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Expressions;

namespace DataBase.Services
{
    public class ShoppingCartItemService : IShoppingCartService
    {
        private Context _context { get; set; }
        public ShoppingCartItemService(Context context)
        {
            _context = context;
        }
        public ShoppingCartItemService()
        {

        }

        public bool Remove(ShoppingCartItem entity)
        {
            try
            {
                _context.carts.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public IEnumerable<ShoppingCartItem> GetAll()
        {
            var query = _context.carts.Include(t => t.User);
            return query;
        }
        public bool RemoveRange(List<ShoppingCartItem> obj)
        {
            try
            {
                _context.carts.RemoveRange(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> AddAsync(ShoppingCartItem entity)
        {
            try
            {
                await _context.carts.AddAsync(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<ShoppingCartItem> FindFirstOrDefaultObjcetAsync(Expression<Func<ShoppingCartItem, bool>> expression)
        {
            var founded = _context.carts.Where(expression).ToList().FirstOrDefault();
            return founded;
        }
        public async Task UpdateAsync(ShoppingCartItem obj)
        {
            _context.carts.Update(obj);
            _context.SaveChanges();
        }

        public int increamentCount(ShoppingCartItem cart, int count)
        {
            try
            {
                if (cart != null)
                {
                    cart.Count = count;
                    _context.SaveChanges();
                    return cart.Count;
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
                throw;
            }
        }

        public IEnumerable<ShoppingCartItem> FindAllUserCarts(string userId)
        {
            return _context.carts.Include(c => c.Product).Include(c => c.User).Where(c => c.UserId == userId);
        }
    }
}
