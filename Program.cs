using OrderSystem.Data;
using OrderSystem.Models;
using OrderSystem.Services;

namespace OrderSystem;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var orderService = new OrderService();
        var promotionService = new PromotionService();
        var stockService = new StockService();

        while (true)
        {
            Console.WriteLine("\n========== HE THONG DON HANG & KHO HANG ==========");
            Console.WriteLine("1. Xem danh sach san pham");
            Console.WriteLine("2. Dat hang (Phan 1 - Tinh tong tien)");
            Console.WriteLine("3. Dat hang + Khuyen mai (Phan 2)");
            Console.WriteLine("4. Dat hang day du (Phan 3 - Khuyen mai + Thue + Tru kho)");
            Console.WriteLine("5. Reset kho hang");
            Console.WriteLine("0. Thoat");
            Console.Write("Chon: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    ShowProducts();
                    break;
                case "2":
                    RunPart1(orderService);
                    break;
                case "3":
                    RunPart2(orderService, promotionService);
                    break;
                case "4":
                    RunPart3(orderService, promotionService, stockService);
                    break;
                case "5":
                    ProductData.ResetStock();
                    Console.WriteLine("Da reset kho hang ve ban dau.");
                    break;
                case "0":
                    Console.WriteLine("Tam biet!");
                    return;
                default:
                    Console.WriteLine("Lua chon khong hop le.");
                    break;
            }

            if (choice != "0")
            {
                Console.Write("\nNhan phim bat ky de tro ve menu chinh...");
                Console.ReadKey();
            }
        }
    }

    static void ShowProducts()
    {
        Console.WriteLine("\n--- DANH SACH SAN PHAM ---");
        Console.WriteLine($"{"ID",-5} {"Ten",-15} {"Gia",-12} {"Loai",-15} {"Ton kho",-8}");
        Console.WriteLine(new string('-', 55));

        foreach (var p in ProductData.GetAll())
        {
            Console.WriteLine($"{p.Id,-5} {p.Name,-15} {p.Price,-12:N0} {p.Category,-15} {p.Stock,-8}");
        }
    }

    static List<OrderItem> InputOrderItems()
    {
        var items = new List<OrderItem>();
        Console.WriteLine("Nhap san pham (nhap 'done' de ket thuc):");

        while (true)
        {
            Console.Write("  Ma san pham (VD: P01): ");
            var id = Console.ReadLine()?.Trim().ToUpper();
            if (string.IsNullOrEmpty(id) || id == "DONE") break;

            var product = ProductData.FindById(id);
            if (product == null)
            {
                Console.WriteLine("  -> Khong tim thay san pham!");
                continue;
            }

            Console.Write("  So luong: ");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int qty) || qty <= 0)
            {
                Console.WriteLine("  -> So luong khong hop le!");
                continue;
            }

            items.Add(new OrderItem { ProductId = id, Quantity = qty });
            Console.WriteLine($"  -> Da them: {product.Name} x{qty}");
        }

        return items;
    }

    static void RunPart1(OrderService orderService)
    {
        Console.WriteLine("\n--- PHAN 1: TINH TONG TIEN ---");
        var items = InputOrderItems();
        if (items.Count == 0) { Console.WriteLine("Khong co san pham nao."); return; }

        var invoice = orderService.CreateInvoice(items);
        PrintInvoicePart1(invoice);
    }

    static void RunPart2(OrderService orderService, PromotionService promotionService)
    {
        Console.WriteLine("\n--- PHAN 2: AP DUNG KHUYEN MAI ---");
        var items = InputOrderItems();
        if (items.Count == 0) { Console.WriteLine("Khong co san pham nao."); return; }

        Console.Write("Nhap ma giam gia (SALE50K / SALE10PT, bo trong neu khong co): ");
        var coupon = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(coupon)) coupon = null;

        var invoice = orderService.CreateInvoice(items);
        promotionService.ApplyPromotions(invoice, coupon);
        PrintInvoicePart2(invoice);
    }

    static void RunPart3(OrderService orderService, PromotionService promotionService, StockService stockService)
    {
        Console.WriteLine("\n--- PHAN 3: DON HANG DAY DU ---");
        var items = InputOrderItems();
        if (items.Count == 0) { Console.WriteLine("Khong co san pham nao."); return; }

        Console.Write("Nhap ma giam gia (bo trong neu khong co): ");
        var coupon = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(coupon)) coupon = null;

        Console.Write("Khu vuc (noi_thanh / ngoai_thanh / mien_nui): ");
        var region = Console.ReadLine()?.Trim() ?? "noi_thanh";

        var invoice = orderService.CreateInvoice(items);
        promotionService.ApplyPromotions(invoice, coupon);
        stockService.ProcessOrder(invoice, items, region);
        PrintInvoiceFull(invoice);
    }

    static void PrintInvoicePart1(Invoice invoice)
    {
        Console.WriteLine("\nKet qua:");
        Console.WriteLine("{");
        Console.WriteLine("  lines: [");
        foreach (var line in invoice.Lines)
        {
            Console.WriteLine($"    {{ productId: \"{line.ProductId}\", name: \"{line.Name}\", " +
                $"unitPrice: {line.UnitPrice}, quantity: {line.Quantity}, lineTotal: {line.LineTotal} }},");
        }
        Console.WriteLine("  ],");
        Console.WriteLine($"  subtotal: {invoice.Subtotal}");
        Console.WriteLine("}");
    }

    static void PrintInvoicePart2(Invoice invoice)
    {
        Console.WriteLine("\nKet qua:");
        Console.WriteLine("{");
        Console.WriteLine("  lines: [");
        foreach (var line in invoice.Lines)
        {
            Console.WriteLine($"    {{ productId: \"{line.ProductId}\", name: \"{line.Name}\", " +
                $"unitPrice: {line.UnitPrice}, quantity: {line.Quantity}, lineTotal: {line.LineTotal} }},");
        }
        Console.WriteLine("  ],");
        Console.WriteLine($"  subtotal: {invoice.Subtotal},");
        Console.WriteLine($"  categoryDiscount: {invoice.CategoryDiscount},");
        Console.WriteLine($"  couponCode: \"{invoice.CouponCode}\",");
        Console.WriteLine($"  couponDiscount: {invoice.CouponDiscount},");
        Console.WriteLine($"  total: {invoice.Total}");
        Console.WriteLine("}");
    }

    static void PrintInvoiceFull(Invoice invoice)
    {
        Console.WriteLine("\nKet qua:");
        Console.WriteLine("{");
        Console.WriteLine("  lines: [");
        foreach (var line in invoice.Lines)
        {
            Console.WriteLine($"    {{ productId: \"{line.ProductId}\", name: \"{line.Name}\", " +
                $"unitPrice: {line.UnitPrice}, quantity: {line.Quantity}, lineTotal: {line.LineTotal} }},");
        }
        Console.WriteLine("  ],");
        Console.WriteLine($"  subtotal: {invoice.Subtotal},");
        Console.WriteLine($"  categoryDiscount: {invoice.CategoryDiscount},");
        Console.WriteLine($"  couponCode: \"{invoice.CouponCode}\",");
        Console.WriteLine($"  couponDiscount: {invoice.CouponDiscount},");
        Console.WriteLine($"  total: {invoice.Total},");
        Console.WriteLine($"  region: \"{invoice.Region}\",");
        Console.WriteLine($"  taxRate: {invoice.TaxRate.ToString(System.Globalization.CultureInfo.InvariantCulture)},");
        Console.WriteLine($"  taxAmount: {invoice.TaxAmount},");
        Console.WriteLine($"  finalTotal: {invoice.FinalTotal},");
        Console.WriteLine($"  stockUpdated: {invoice.StockUpdated.ToString().ToLower()}");
        Console.WriteLine("}");
    }
}
