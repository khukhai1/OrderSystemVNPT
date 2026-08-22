using OrderSystem.Data;
using OrderSystem.Models;

namespace OrderSystem.Services;

public class PromotionService
{
    public void ApplyPromotions(Invoice invoice, string? couponCode)
    {
        decimal categoryDiscount = CalculateCategoryDiscount(invoice);
        invoice.CategoryDiscount = categoryDiscount;

        decimal subtotalAfterCategory = invoice.Subtotal - categoryDiscount;

        invoice.CouponCode = couponCode;
        invoice.CouponDiscount = 0;

        if (!string.IsNullOrEmpty(couponCode))
        {
            switch (couponCode.ToUpper())
            {
                case "SALE50K":
                    if (subtotalAfterCategory >= 300000)
                    {
                        invoice.CouponDiscount = 50000;
                    }
                    break;

                case "SALE10PT":
                    invoice.CouponDiscount = Math.Round(subtotalAfterCategory * 0.10m);
                    break;
            }
        }

        decimal total = subtotalAfterCategory - invoice.CouponDiscount;
        invoice.Total = Math.Max(total, 0);
        invoice.FinalTotal = invoice.Total;
    }

    private decimal CalculateCategoryDiscount(Invoice invoice)
    {
        var categoryGroups = new Dictionary<string, List<InvoiceLine>>();

        foreach (var line in invoice.Lines)
        {
            var product = ProductData.FindById(line.ProductId);
            if (product == null) continue;

            if (!categoryGroups.ContainsKey(product.Category))
                categoryGroups[product.Category] = new List<InvoiceLine>();

            categoryGroups[product.Category].Add(line);
        }

        decimal totalDiscount = 0;

        foreach (var group in categoryGroups)
        {
            int totalQuantity = group.Value.Sum(l => l.Quantity);

            if (totalQuantity >= 3)
            {
                decimal categoryTotal = group.Value.Sum(l => l.LineTotal);
                totalDiscount += Math.Round(categoryTotal * 0.10m);
            }
        }

        return totalDiscount;
    }
}
