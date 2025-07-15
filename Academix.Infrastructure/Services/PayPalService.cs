using Academix.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Academix.Infrastructure.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayPalService> _logger;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _baseUrl;

        public PayPalService(HttpClient httpClient, IConfiguration configuration, ILogger<PayPalService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            
            _clientId = _configuration["PayPal:ClientId"] ?? "";
            _clientSecret = _configuration["PayPal:ClientSecret"] ?? "";
            _baseUrl = _configuration["PayPal:BaseUrl"] ?? "https://api-m.sandbox.paypal.com";
        }

        public async Task<PayPalCreateOrderResult> CreateOrderAsync(decimal amount, string currency = "USD", string? description = null)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new PayPalCreateOrderResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Failed to get PayPal access token"
                    };
                }

                var orderRequest = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
                    {
                        new
                        {
                            amount = new
                            {
                                currency_code = currency,
                                value = amount.ToString("F2")
                            },
                            description = description ?? "Payment"
                        }
                    },
                    application_context = new
                    {
                        return_url = _configuration["PayPal:ReturnUrl"] ?? "http://localhost:3000/payment/success",
                        cancel_url = _configuration["PayPal:CancelUrl"] ?? "http://localhost:3000/payment/cancel"
                    }
                };

                var json = JsonSerializer.Serialize(orderRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.PostAsync($"{_baseUrl}/v2/checkout/orders", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var orderId = doc.RootElement.GetProperty("id").GetString();
                    
                    var approvalUrl = "";
                    if (doc.RootElement.TryGetProperty("links", out var links))
                    {
                        foreach (var link in links.EnumerateArray())
                        {
                            if (link.GetProperty("rel").GetString() == "approve")
                            {
                                approvalUrl = link.GetProperty("href").GetString();
                                break;
                            }
                        }
                    }

                    _logger.LogInformation("PayPal order created successfully. OrderId: {OrderId}", orderId);

                    return new PayPalCreateOrderResult
                    {
                        IsSuccess = true,
                        OrderId = orderId,
                        ApprovalUrl = approvalUrl
                    };
                }
                else
                {
                    _logger.LogError("PayPal order creation failed. Status: {Status}, Response: {Response}", 
                        response.StatusCode, responseContent);
                    
                    return new PayPalCreateOrderResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"PayPal API error: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order");
                return new PayPalCreateOrderResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PayPalCaptureResult> CaptureOrderAsync(string orderId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new PayPalCaptureResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Failed to get PayPal access token"
                    };
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var content = new StringContent("{}", Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/v2/checkout/orders/{orderId}/capture", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var status = doc.RootElement.GetProperty("status").GetString();
                    
                    var transactionId = "";
                    var paymentId = "";
                    var capturedAmount = 0m;

                    if (doc.RootElement.TryGetProperty("purchase_units", out var units))
                    {
                        var firstUnit = units.EnumerateArray().FirstOrDefault();
                        if (firstUnit.TryGetProperty("payments", out var payments) &&
                            payments.TryGetProperty("captures", out var captures))
                        {
                            var firstCapture = captures.EnumerateArray().FirstOrDefault();
                            transactionId = firstCapture.GetProperty("id").GetString();
                            paymentId = firstCapture.GetProperty("id").GetString();
                            
                            if (firstCapture.TryGetProperty("amount", out var amount))
                            {
                                decimal.TryParse(amount.GetProperty("value").GetString(), out capturedAmount);
                            }
                        }
                    }

                    _logger.LogInformation("PayPal order captured successfully. OrderId: {OrderId}, TransactionId: {TransactionId}", 
                        orderId, transactionId);

                    return new PayPalCaptureResult
                    {
                        IsSuccess = true,
                        TransactionId = transactionId,
                        PaymentId = paymentId,
                        CapturedAmount = capturedAmount,
                        Status = status,
                        CapturedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    _logger.LogError("PayPal order capture failed. OrderId: {OrderId}, Status: {Status}, Response: {Response}", 
                        orderId, response.StatusCode, responseContent);
                    
                    return new PayPalCaptureResult
                    {
                        IsSuccess = false,
                        ErrorMessage = $"PayPal capture failed: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing PayPal order {OrderId}", orderId);
                return new PayPalCaptureResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PayPalOrderDetails> GetOrderDetailsAsync(string orderId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception("Failed to get PayPal access token");
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync($"{_baseUrl}/v2/checkout/orders/{orderId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var status = doc.RootElement.GetProperty("status").GetString();
                    var amount = 0m;
                    var currency = "USD";

                    if (doc.RootElement.TryGetProperty("purchase_units", out var units))
                    {
                        var firstUnit = units.EnumerateArray().FirstOrDefault();
                        if (firstUnit.TryGetProperty("amount", out var amountObj))
                        {
                            decimal.TryParse(amountObj.GetProperty("value").GetString(), out amount);
                            currency = amountObj.GetProperty("currency_code").GetString() ?? "USD";
                        }
                    }

                    return new PayPalOrderDetails
                    {
                        Id = orderId,
                        Status = status ?? "",
                        Amount = amount,
                        Currency = currency,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    throw new Exception($"PayPal API error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PayPal order details for {OrderId}", orderId);
                throw;
            }
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            try
            {
                var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authValue}");

                var requestBody = "grant_type=client_credentials";
                var content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await _httpClient.PostAsync($"{_baseUrl}/v1/oauth2/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    return doc.RootElement.GetProperty("access_token").GetString();
                }
                else
                {
                    _logger.LogError("Failed to get PayPal access token. Status: {Status}, Response: {Response}", 
                        response.StatusCode, responseContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PayPal access token");
                return null;
            }
        }
    }
} 