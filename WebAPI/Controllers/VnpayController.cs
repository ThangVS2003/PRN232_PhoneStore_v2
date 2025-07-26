using Microsoft.AspNetCore.Mvc;
using Service.IService;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using WebMVC.Controllers;
using System.Diagnostics;

namespace WebAPI.Controllers
{
    [Route("")] // Route gốc để endpoint là /CreatePaymentUrl
    public class VnpayController : Controller
    {
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;

        public VnpayController(IVnpay vnpay, IConfiguration configuration)
        {
            _vnpay = vnpay;
            _configuration = configuration;

            _vnpay.Initialize(
                _configuration["Vnpay:TmnCode"],
                _configuration["Vnpay:HashSecret"],
                _configuration["Vnpay:BaseUrl"],
                _configuration["Vnpay:ReturnUrl"]
            );
        }

        [HttpPost("CreatePaymentUrl")]
        public async Task<ActionResult> CreatePaymentUrl([FromBody] VnpayRequestDto request)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    return BadRequest("Không thể lấy địa chỉ IP.");
                }

                var paymentRequest = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = request.Amount,
                    Description = "Thanh toán giỏ hàng",
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                var paymentUrl = _vnpay.GetPaymentUrl(paymentRequest);
                return Created(paymentUrl, new { PaymentUrl = paymentUrl, PaymentId = paymentRequest.PaymentId });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi tạo URL thanh toán: {ex.Message}");
            }
        }

        [HttpGet("Callback")]
        public async Task<ActionResult<string>> Callback()
        {
            Console.WriteLine($"Callback received at: {DateTime.Now}, Query: {Request.QueryString}");

            if (!Request.QueryString.HasValue)
            {
                Console.WriteLine("Callback: No query string provided.");
                return NotFound("Không tìm thấy thông tin thanh toán.");
            }

            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";
                Console.WriteLine($"Callback: paymentResult.IsSuccess = {paymentResult.IsSuccess}");

                if (paymentResult.IsSuccess)
                {
                    // Comment phần xóa giỏ hàng để tránh lỗi CS1061
                    /*
                    var token = HttpContext.Request.Cookies["JwtToken"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        using var httpClient = new HttpClient();
                        httpClient.BaseAddress = new Uri("https://localhost:7026/");
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var cartResponse = await httpClient.GetFromJsonAsync<CartViewModel>("api/Cart", new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (cartResponse != null && cartResponse.OrderDetails.Any())
                        {
                            foreach (var item in cartResponse.OrderDetails)
                            {
                                await httpClient.DeleteAsync($"api/Cart/remove/{item.Id}");
                            }
                        }
                    }
                    */

                    var txnRef = Request.Query["vnp_TxnRef"].ToString();
                    var amount = double.Parse(Request.Query["vnp_Amount"].ToString()) / 100;
                    var redirectUrl = $"https://localhost:7211/Payment/Success?txnRef={txnRef}&amount={amount}";
                    Console.WriteLine($"Callback: Redirecting to {redirectUrl}");
                    return Redirect(redirectUrl);
                }

                Console.WriteLine($"Callback: Payment failed. Description: {resultDescription}");
                return BadRequest(resultDescription);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Callback error: {ex.Message}");
                return BadRequest($"Lỗi khi xử lý callback: {ex.Message}");
            }
        }

        [HttpGet("IpnAction")]
        public async Task<IActionResult> IpnAction()
        {
            Console.WriteLine($"IpnAction received at: {DateTime.Now}, Query: {Request.QueryString}");

            if (!Request.QueryString.HasValue)
            {
                Console.WriteLine("IpnAction: No query string provided.");
                return NotFound("Không tìm thấy thông tin thanh toán.");
            }

            try
            {
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                if (paymentResult.IsSuccess)
                {
                    // Comment phần xóa giỏ hàng để tránh lỗi CS1061
                    /*
                    var token = HttpContext.Request.Cookies["JwtToken"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        using var httpClient = new HttpClient();
                        httpClient.BaseAddress = new Uri("https://localhost:7026/");
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var cartResponse = await httpClient.GetFromJsonAsync<CartViewModel>("api/Cart", new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (cartResponse != null && cartResponse.OrderDetails.Any())
                        {
                            foreach (var item in cartResponse.OrderDetails)
                            {
                                await httpClient.DeleteAsync($"api/Cart/remove/{item.Id}");
                            }
                        }
                    }
                    */
                    Console.WriteLine("IpnAction: Payment successful.");
                    return Ok();
                }

                Console.WriteLine("IpnAction: Payment failed.");
                return BadRequest("Thanh toán thất bại.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IpnAction error: {ex.Message}");
                return BadRequest($"Lỗi khi xử lý IPN: {ex.Message}");
            }
        }
    }

    public class VnpayRequestDto
    {
        public double Amount { get; set; }
    }

    public class VnpayResponseDto
    {
        public string PaymentUrl { get; set; }
        public long PaymentId { get; set; }
    }
}