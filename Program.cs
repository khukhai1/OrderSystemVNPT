using OrderSystem.Data;
using OrderSystem.Models;
using OrderSystem.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

using (var db = new AppDbContext())
{
    db.Database.EnsureCreated();
}

var orderService = new OrderService();
var promotionService = new PromotionService();
var stockService = new StockService();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/products", () => ProductData.GetAll());

app.MapPost("/api/order/part1", (List<OrderItem> items) =>
{
    var invoice = orderService.CreateInvoice(items);
    return Results.Ok(invoice);
});

app.MapPost("/api/order/part2", (OrderRequest request) =>
{
    var invoice = orderService.CreateInvoice(request.Items);
    promotionService.ApplyPromotions(invoice, request.CouponCode);
    return Results.Ok(invoice);
});

app.MapPost("/api/order/part3", (OrderRequest request) =>
{
    var invoice = orderService.CreateInvoice(request.Items);
    promotionService.ApplyPromotions(invoice, request.CouponCode);
    stockService.ProcessOrder(invoice, request.Items, request.Region ?? "noi_thanh");
    return Results.Ok(invoice);
});

app.MapPost("/api/reset", () =>
{
    ProductData.ResetStock();
    return Results.Ok(new { message = "Da reset kho hang" });
});

app.Run();

public class OrderRequest
{
    public List<OrderItem> Items { get; set; } = new();
    public string? CouponCode { get; set; }
    public string? Region { get; set; }
}
