using System;
using System.Collections.Generic;

namespace WebMVC.ViewModels
{
    public class CartViewModel
    {
        public List<OrderDetailViewModel> OrderDetails { get; set; } = new List<OrderDetailViewModel>();
        public string ShippingAddress { get; set; }
        public decimal Total { get; set; }
        public string VoucherCode { get; set; }
        public int? VoucherId { get; set; } // ID của voucher, nullable để khớp với bảng Order
    }

    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; } // ID của biến thể sản phẩm
        public int Quantity { get; set; }
        public ProductVariantViewModel ProductVariant { get; set; }
    }

    public class ProductVariantViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Version { get; set; }
        public string Image { get; set; }
        public decimal SellingPrice { get; set; }
    }
}