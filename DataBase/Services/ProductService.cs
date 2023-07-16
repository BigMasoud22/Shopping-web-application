using DataBase.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Expressions;

namespace DataBase.Services
{
    public class ProductServices : IProductService
    {
        public ProductServices()
        {

        }
        private Context _context { get; set; }
        public ProductServices(Context context)
        {
            _context = context;
        }

        public bool Remove(Product entity)
        {
            try
            {
                _context.products.Remove(entity);
                var isSaved = _context.SaveChanges();
                if (isSaved > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public IEnumerable<Product> GetAll()
        {
            List<Product> query = _context.products.Include(p => p.Category).Include(p=>p.Images).ToList();
            return query;
        }
        public IEnumerable<Product> GetAll(Expression<Func<Product,bool>> expression)
        {
            var query =_context.products.Where(expression).Include(p=>p.Category).Include(p=>p.Images);
            return query;
        }
        public bool RemoveRange(List<Product> products)
        {
            try
            {
                _context.products.RemoveRange(products);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<int> AddAsync(Product entity)
        {
            try
            {
                await _context.products.AddAsync(entity);
                var isSaved = _context.SaveChanges();
                return entity.id;
            }
            catch (Exception)
            {
                return -1;
                throw;
            }
        }
        public async Task<Product> FindFirstOrDefaultObjcetAsync(Expression<Func<Product, bool>> expression)
        {
            var all = _context.products.Include(p=>p.Category).Include(p=>p.Images);
            var filtered = all.Where(expression);
            return await filtered.FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(Product obj)
        {
            _context.products.Update(obj);
            _context.SaveChanges();
        }

        public List<Comments> FetchProductsComments(int productId)
        {
            var commnets = _context.comments.Where(c=>c.ProductId==productId).Include(c=>c.User).ToList();
            return commnets;
        }
    }
}
