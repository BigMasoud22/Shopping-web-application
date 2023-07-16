namespace Models
{
    public class Category
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual IEnumerable<Product> Products { get; set; }
    }
}