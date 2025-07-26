using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhoneStoreMVC.ViewModels
{
    public class OrderDetailViewModel
    {
        [JsonPropertyName("id")]
        public int OrderId { get; set; }

        [JsonPropertyName("orderDate")]
        public DateTime? OrderDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("totalAmount")]
        public decimal? TotalAmount { get; set; }

        [JsonPropertyName("shippingAddress")]
        public string? ShippingAddress { get; set; }

        public List<OrderProductDetailViewModel> Products { get; set; } = new();
    }

    public class OrderProductDetailViewModel
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string? VersionName { get; set; }

        [JsonPropertyName("color")]
        public string? ColorName { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        public decimal? SubTotal => UnitPrice * Quantity;
    }
}