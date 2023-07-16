using System.Linq.Expressions;
using DataBase.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DataBase.Services
{
    public class UserService : IUsersService
    {
        private readonly Context _context = new Context(new DbContextOptions<Context>());
        public async Task<bool> AddAsync(User User)
        {
            try
            {
                await _context.users.AddAsync(User);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public User FindFirstOrDefaultObjcet(Expression<Func<User, bool>> expression)
        {
            var user = _context.users.Where(expression).FirstOrDefault();
            return user;
        }
        //public static IEnumerable<User> GetAll()
        //{
        //    string connectionString = "Server=.;DataBase=Shop;Trusted_Connection=True;Encrypt=False;";
        //    /*this*/
        //    string query = "SELECT u.*, o.Id as OrderId, o.OrderDate, o.OrderTotal,o.ShippingDate,Price  FROM AspNetUsers u LEFT JOIN Orders o ON u.Id = o.UserId";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlCommand command = new SqlCommand(query, connection);
        //        // Set the command timeout to prevent long-running queries from causing problems
        //        command.CommandTimeout = 60;
        //        // Open the connection to the database
        //        connection.Open();
        //        // Execute the query and retrieve the results
        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            List<User> users = new List<User>();
        //            while (reader.Read())
        //            {
        //                // Get the user's ID from the reader
        //                string userId = reader.GetString(reader.GetOrdinal("Id"));
        //                // If we haven't seen this user yet, create a new User object and add it to the list
        //                User user = users.FirstOrDefault(u => u.Id == userId);

        //                if (user == null)
        //                {
        //                    var username = reader.GetString(reader.GetOrdinal("UserName"));
        //                    var email = reader.GetString(reader.GetOrdinal("Email"));
        //                    var emailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed"));
        //                    var passwordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
        //                    var securityStamp = reader.GetString(reader.GetOrdinal("SecurityStamp"));
        //                    var phonenumberConfirmed = reader.GetBoolean(reader.GetOrdinal("PhoneNumberConfirmed"));
        //                    var twoFactorEnabled = reader.GetBoolean(reader.GetOrdinal("TwoFactorEnabled"));
        //                    var lockoutenabled = reader.GetBoolean(reader.GetOrdinal("LockoutEnabled"));
        //                    var accessfailedCount = reader.GetInt32(reader.GetOrdinal("AccessFailedCount"));
        //                    user = new User()
        //                    {
        //                        Id = userId,
        //                        UserName = username,
        //                        Email = email,
        //                        EmailConfirmed = emailConfirmed,
        //                        PasswordHash = passwordHash,
        //                        SecurityStamp = securityStamp,
        //                        TwoFactorEnabled = twoFactorEnabled,
        //                        LockoutEnabled = lockoutenabled,
        //                        AccessFailedCount = accessfailedCount,
        //                        Age = reader.GetInt32(reader.GetOrdinal("Age")),
        //                        Orders = new List<Order>(),
        //                    };
        //                    if (!reader.IsDBNull(reader.GetOrdinal("FullName"))) // check if the value is not null
        //                    {
        //                        string fullname = reader.GetString(reader.GetOrdinal("FullName"));
        //                        user.FullName = fullname;
        //                    }
        //                    users.Add(user);
        //                }
        //                // If the current row has an Order ID, add the order to the user's list of orders
        //                /*this*/
        //                if (!reader.IsDBNull(reader.GetOrdinal("OrderId")))
        //                {
        //                    Order order = new Order
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
        //                        UserId = userId,
        //                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
        //                        OrderTotal = reader.GetInt32(reader.GetOrdinal("OrderTotal")),
        //                        ShippingDate = reader.GetDateTime(reader.GetOrdinal("ShippingDate")),
        //                        Price = reader.GetDecimal(reader.GetOrdinal("Price"))
        //                        // TODO: Map other properties as needed
        //                    };
        //                    user.Orders.Add(order);
        //                }
        //            }
        //            return users;
        //            // TODO: Process the list of User objects as needed
        //        }
        //    }
        //}
        public IEnumerable<User> GetAll()
        {
            var all = _context.users.Include(u => u.Address).ToList();
            return all;
        }
        public bool Remove(User User)
        {
            try
            {
                _context.users.Remove(User);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public bool RemoveRange(List<User> Users)
        {
            try
            {
                _context.users.RemoveRange(Users);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public void Update(User User)
        {
            var user = _context.users.Find(User.Id);
            user.FullName = User.FullName;
            user.UserName = User.UserName;
            user.Email = User.Email;
            user.Age = User.Age;

            var isSuccess = _context.SaveChanges();
        }
        public void Update(User user, UserAddress address)
        {
            var mainUser = _context.users.Find(user.Id);
            mainUser.FullName = user.FullName;
            mainUser.UserName = user.UserName;
            mainUser.Email = user.Email;
            mainUser.Age = user.Age;

            if (mainUser.Address == null && address != null)
            {
                mainUser.Address = new UserAddress();
                mainUser.Address.PostCode = address.PostCode;
                mainUser.Address.Address = address.Address;
                mainUser.Address.City = address.City;
                mainUser.Address.UserId = mainUser.Id; // Set UserId to match the main user's Id
            }
            else if (mainUser.Address != null && address != null)
            {
                mainUser.Address.PostCode = address.PostCode;
                mainUser.Address.Address = address.Address;
                mainUser.Address.City = address.City;
                mainUser.Address.UserId = mainUser.Id; // Set UserId to match the main user's Id
            }

            var isSuccess = _context.SaveChanges();
        }
        public bool IsSuperAdminExitst()
        {
            try
            {
                var allRoles = _context.Roles.ToList();
                var superAdminExist = allRoles.FirstOrDefault(r => r.Name == ApplicationRoles.superAdmin);
                if (superAdminExist != null)
                {
                    var allUsers = _context.UserRoles.ToList();
                    var superAdmin = allUsers.FirstOrDefault(u => u.RoleId == superAdminExist.Id);
                    if (superAdmin != null)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void UpsertAddress(string UserId, UserAddress address)
        {
            var theUser = FindFirstOrDefaultObjcet(u => u.Id == UserId);
            if (theUser != null)
            {
                if (theUser.Address != null)
                {
                    theUser.Address.Address = address.Address;
                    theUser.Address.City = address.City;
                    theUser.Address.PostCode = address.PostCode;
                    theUser.Address.State = address.State;
                    var isok = _context.SaveChanges();
                }
                else
                {
                    var isok = _context.SaveChanges();
                }
            }
        }
        public bool SetComments(Comments comment, int productId, string Id)
        {
            var user = _context.users.Where(u => u.Id == Id).FirstOrDefault();
            var product = _context.products.Where(p => p.id == productId).FirstOrDefault();
            comment.ProductId = productId;
            if (user != null && product != null)
            {
                if (user.userComments != null)
                {
                    user.userComments.Add(comment);
                }
                else
                {
                    user.userComments = new List<Comments>();
                    user.userComments.Add(comment);
                }
                var isSaved = _context.SaveChanges();
                if (isSaved != 0)
                    return true;

                return false;
            }
            return false;
        }
    }
}