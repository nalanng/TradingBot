using Newtonsoft.Json;
using Skender.Stock.Indicators;

namespace TradingBot.Services
{
    public class BinanceService
    {
        private static readonly string apiUrl = "https://api.binance.com/api/v3/klines";
        private readonly HttpClient _httpClient;

        public BinanceService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Quote>> GetHistoricalData(string symbol, string interval, string range)
        {
            if (range == "last250")
            {
                return await FetchLast250QuotesAsync(symbol, interval);
            }
            else if (range == "4months")
            {
                return await Fetch4MonthsQuotesAsync(symbol, interval);
            }
            else if(range == "35")
            {
                return await FetchLast35QuotesAsync(symbol, interval);
            }
            else
            {
                throw new ArgumentException("Invalid range. Use 'last100' or '4months'.");
            }
        }

        private async Task<List<Quote>> FetchLast35QuotesAsync(string symbol, string interval)
        {
            List<Quote> quotes = new List<Quote>();
            string url = $"{apiUrl}?symbol={symbol}&interval={interval}&limit=35";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<List<List<object>>>(response);

                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        quotes.Add(new Quote
                        {
                            Date = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(item[0])).UtcDateTime,
                            Open = Convert.ToDecimal(item[1]),
                            High = Convert.ToDecimal(item[2]),
                            Low = Convert.ToDecimal(item[3]),
                            Close = Convert.ToDecimal(item[4]),
                            Volume = Convert.ToDecimal(item[5]),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching last 35 quotes: {ex.Message}");
                throw;
            }
            return quotes.OrderBy(q => q.Date).ToList();
        }

        private async Task<List<Quote>> FetchLast250QuotesAsync(string symbol, string interval)
        {
            List<Quote> quotes = new List<Quote>();
            string url = $"{apiUrl}?symbol={symbol}&interval={interval}&limit=250";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<List<List<object>>>(response);

                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        quotes.Add(new Quote
                        {
                            Date = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(item[0])).UtcDateTime,
                            Open = Convert.ToDecimal(item[1]),
                            High = Convert.ToDecimal(item[2]),
                            Low = Convert.ToDecimal(item[3]),
                            Close = Convert.ToDecimal(item[4]),
                            Volume = Convert.ToDecimal(item[5]),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching last 250 quotes: {ex.Message}");
                throw;
            }
            return quotes.OrderBy(q => q.Date).ToList();
        }

        private async Task<List<Quote>> Fetch4MonthsQuotesAsync(string symbol, string interval)
        {
            List<Quote> quotes = new List<Quote>();
            long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long startTime = DateTimeOffset.UtcNow.AddMonths(-4).ToUnixTimeMilliseconds();
            long currentStartTime = startTime;


            while (currentStartTime < endTime)
            {
                string url = $"{apiUrl}?symbol={symbol}&interval={interval}&startTime={currentStartTime}&endTime={endTime}&limit=1000";

                try
                {
                    var response = await _httpClient.GetStringAsync(url);
                    var data = JsonConvert.DeserializeObject<List<List<object>>>(response);

                    if (data == null || data.Count == 0)
                        break;

                    foreach (var item in data)
                    {
                        quotes.Add(new Quote
                        {
                            Date = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(item[0])).UtcDateTime,
                            Open = Convert.ToDecimal(item[1]),
                            High = Convert.ToDecimal(item[2]),
                            Low = Convert.ToDecimal(item[3]),
                            Close = Convert.ToDecimal(item[4]),
                            Volume = Convert.ToDecimal(item[5]),
                        });
                    }

                    currentStartTime = Convert.ToInt64(data.Last()[0]) + 1;

                    if (data.Count < 1000)
                        break;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error fetching historical quotes: {ex.Message}");
                    throw;
                }
            }
            return quotes.OrderBy(q => q.Date).ToList();
        }

    }
}
