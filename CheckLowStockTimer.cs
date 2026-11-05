using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SmartInventory.Functions
{
    public class CheckLowStockTimer
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public CheckLowStockTimer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CheckLowStockTimer>();
            _httpClient = new HttpClient();
        }

        [Function("CheckLowStockTimer")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timer)
        {
            _logger.LogInformation("Timer triggered. Checking low stock...");

            try
            {
                var items = await _httpClient.GetFromJsonAsync<List<Item>>(
                    "https://smartinventory-backend.victoriustree-971c52f2.eastus.azurecontainerapps.io/api/items"
                );

                if (items == null)
                {
                    _logger.LogWarning("API returned no items.");
                    return;
                }

                foreach (var item in items)
                {
                    if (item.Quantity < item.LowStockThreshold)
                    {
                        _logger.LogWarning(
                            $"LOW STOCK ALERT: {item.Name} — Qty: {item.Quantity}, Threshold: {item.LowStockThreshold}"
                        );
                    }
                }

                _logger.LogInformation("Low stock check completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking low stock: {ex.Message}");
            }
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
    }
}
