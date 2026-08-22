namespace OrderSystem.Models;

public class InvoiceLine
{
    public string ProductId { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class Invoice
{
    public List<InvoiceLine> Lines { get; set; } = new();
    public decimal Subtotal { get; set; }    //Tong tien chua giam gia

    public decimal CategoryDiscount { get; set; } //Giam do mua >= 3 sp cung catgetory
    public string? CouponCode { get; set; }  //Ma coupon
    public decimal CouponDiscount { get; set; }  //So tien giam tu coupon
    public decimal Total { get; set; }  //Tong tien sau giam gia

    public string? Region { get; set; }  //Khu vuc
    public decimal TaxRate { get; set; }  //Ty le thue
    public decimal TaxAmount { get; set; }  //Tien thue
    public decimal FinalTotal { get; set; } //Tong tien cuoi cung
    public bool StockUpdated { get; set; }  //Trang thai cap nhat ton kho
}
