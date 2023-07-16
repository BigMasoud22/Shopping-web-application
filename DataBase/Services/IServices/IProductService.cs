using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Services.IServices
{
    public interface IProductService
    {
        public IEnumerable<Product> GetAll();
        public IEnumerable<Product> GetAll(Expression<Func<Product, bool>> expression);
        public bool RemoveRange(List<Product> products);
        public bool Remove(Product product);
        public Task<int> AddAsync(Product product);
        public Task UpdateAsync(Product product);
        public Task<Product> FindFirstOrDefaultObjcetAsync(Expression<Func<Product, bool>> expression);
        public List<Comments> FetchProductsComments(int productId);

    }
}

