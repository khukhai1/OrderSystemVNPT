using OrderSystem.Models;

namespace OrderSystem.Data;

public static class ProductData
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = "P01", Name = "Ao thun", Price = 150000, Category = "clothing", Stock = 20 },
        new Product { Id = "P02", Name = "Quan jean", Price = 450000, Category = "clothing", Stock = 10 },
        new Product { Id = "P03", Name = "Tai nghe", Price = 890000, Category = "electronics", Stock = 5 },
        new Product { Id = "P04", Name = "Sac du phong", Price = 350000, Category = "electronics", Stock = 8 },
    };

    public static List<Product> GetAll() => _products;

    public static Product? FindById(string id) => _products.FirstOrDefault(p => p.Id == id);

    public static void ResetStock()
    {
        _products[0].Stock = 20;
        _products[1].Stock = 10;
        _products[2].Stock = 5;
        _products[3].Stock = 8;
    }
}
