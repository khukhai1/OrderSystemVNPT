using OrderSystem.Data;
using OrderSystem.Models;

namespace OrderSystem.Services;

public class StockService
{
    private static readonly object _stockLock = new object();

    private static readonly Dictionary<string, decimal> TaxRates = new()
    {
        { "noi_thanh", 0.08m },
        { "ngoai_thanh", 0.05m },
        { "mien_nui", 0.03m }
    };

    public bool ProcessOrder(Invoice invoice, List<OrderItem> orderItems, string region)
    {
        decimal taxRate = TaxRates.ContainsKey(region) ? TaxRates[region] : 0.08m;
        invoice.Region = region;
        invoice.TaxRate = taxRate;
        invoice.TaxAmount = Math.Round(invoice.Total * taxRate);
        invoice.FinalTotal = invoice.Total + invoice.TaxAmount;

        lock (_stockLock)
        {
            using var db = new AppDbContext();

            foreach (var item in orderItems)
            {
                var product = db.Products.Find(item.ProductId);
                if (product == null)
                {
                    invoice.StockUpdated = false;
                    return false;
                }

                if (product.Stock < item.Quantity)
                {
                    Console.WriteLine($"  [LOI] San pham {product.Name} chi con {product.Stock}, yeu cau {item.Quantity}");
                    invoice.StockUpdated = false;
                    return false;
                }
            }

            foreach (var item in orderItems)
            {
                var product = db.Products.Find(item.ProductId)!;
                product.Stock -= item.Quantity;
            }

            db.SaveChanges();
            invoice.StockUpdated = true;
            return true;
        }
    }
}
