using DataBase.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq.Expressions;
namespace DataBase.Services
{
    public class CategoryService : ICategoryService
    {
        private Context _context { get; set; }
        public CategoryService(Context context)
        {
            _context = context;
        }
        public CategoryService()
        {

        }

        public bool Remove(Category entity)
        {
            try
            {
                _context.categories.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public IEnumerable<Category> GetAll()
        {
            var query = _context.categories;
            return query;
        }
        public bool RemoveRange(List<Category> obj)
        {
            try
            {
                _context.categories.RemoveRange(obj);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<bool> AddAsync(Category entity)
        {
            try
            {
                await _context.categories.AddAsync(entity);
                var isSaved = _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<Category> FindFirstOrDefaultObjcetAsync(Expression<Func<Category, bool>> expression)
        {
            IQueryable<Category> all = (IQueryable<Category>)_context.categories;
            var filter = all.Where(expression);
            return await filter.FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(Category obj)
        {
            _context.categories.Update(obj);
            _context.SaveChanges();
        }
    }
}
