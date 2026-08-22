using OrderSystem.Models;

namespace OrderSystem.Data;

public static class ProductData
{
    public static List<Product> GetAll()
    {
        using var db = new AppDbContext();
        return db.Products.ToList();
    }

    public static Product? FindById(string id)
    {
        using var db = new AppDbContext();
        return db.Products.Find(id);
    }

    public static void SaveChanges(Product product)
    {
        using var db = new AppDbContext();
        db.Products.Update(product);
        db.SaveChanges();
    }

    public static void ResetStock()
    {
        using var db = new AppDbContext();
        var defaults = new Dictionary<string, int>
        {
            { "P01", 20 }, { "P02", 10 }, { "P03", 5 }, { "P04", 8 }
        };

        foreach (var kvp in defaults)
        {
            var product = db.Products.Find(kvp.Key);
            if (product != null)
            {
                product.Stock = kvp.Value;
            }
        }
        db.SaveChanges();
    }
}
