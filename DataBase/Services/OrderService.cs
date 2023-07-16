using DataBase.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Expressions;

namespace DataBase.Services
{
    public class OrderServices : IOrderService
    {
        private Context _context { get; set; }
        public OrderServices(Context context)
        {
            _context = context;
        }
        public OrderServices()
        {

        }

        public bool Remove(Order entity)
        {
            try
            {
                _context.orders.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public IEnumerable<Order> GetAll()
        {
            var query = _context.orders.Include(t => t.User);
            return query;
        }
        public bool RemoveRange(List<Order> obj)
        {
            try
            {
                _context.orders.RemoveRange(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> AddAsync(Order entity)
        {
            try
            {
                await _context.orders.AddAsync(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<Order> FindFirstOrDefaultObjcetAsync(Expression<Func<Order, bool>> expression)
        {
            IQueryable<Order> all = (IQueryable<Order>)_context.orders;
            var filter = all.Where(expression);
            return await filter.FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(Order obj)
        {
            _context.orders.Update(obj);
            _context.SaveChanges();
        }
    }
}
