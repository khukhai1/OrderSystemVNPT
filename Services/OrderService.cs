using OrderSystem.Data;
using OrderSystem.Models;

namespace OrderSystem.Services;

public class OrderService
{
    public Invoice CreateInvoice(List<OrderItem> orderItems)
    {
        var invoice = new Invoice();

        foreach (var item in orderItems)
        {
            var product = ProductData.FindById(item.ProductId);
            if (product == null)
                throw new Exception($"Khong tim thay san pham: {item.ProductId}");

            var line = new InvoiceLine
            {
                ProductId = product.Id,
                Name = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                LineTotal = product.Price * item.Quantity
            };

            invoice.Lines.Add(line);
        }

        invoice.Subtotal = invoice.Lines.Sum(l => l.LineTotal);
        invoice.Total = invoice.Subtotal;
        invoice.FinalTotal = invoice.Subtotal;

        return invoice;
    }
}
