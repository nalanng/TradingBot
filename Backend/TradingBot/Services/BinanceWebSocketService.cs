using Newtonsoft.Json;
using Skender.Stock.Indicators;
using TradingBot.Services;
using WebSocketSharp;

public class BinanceWebSocketService
{
    private WebSocket _socket;
    private List<Quote> _quotes = new List<Quote>();
    private BacktestService backtestRsiMacd;
    private BinanceService binanceService;

    public BinanceWebSocketService(BacktestService backtestRsiMacd, BinanceService binanceService)
    {
        this.backtestRsiMacd = backtestRsiMacd;
        this.binanceService = binanceService;
    }

    public async void ConnectSocket(string symbol, string interval, int buyRsiThreshold, int sellRsiThreshold, int emaPeriod, List<Quote> quotes)
    {
        _socket = new WebSocket($"wss://stream.binance.com:9443/ws/{symbol}@trade");

        decimal capital = 1000;
        decimal position = 0;
        List<Trade> tradeList = new List<Trade>();

        DateTime lastProcessedTime = DateTime.MinValue;
        TimeSpan intervalDuration = interval switch
        {
            "1s" => TimeSpan.FromSeconds(1),
            "1m" => TimeSpan.FromMinutes(1),
            "1h" => TimeSpan.FromHours(1),
            "1d" => TimeSpan.FromDays(1),
            "4h" => TimeSpan.FromHours(4),
            _ => TimeSpan.FromSeconds(1)
        };

        _socket.OnMessage += async (sender, e) =>
        {
            try
            {
                var tradeData = JsonConvert.DeserializeObject<TradeData>(e.Data);
                var now = DateTime.UtcNow;

                if (now - lastProcessedTime < intervalDuration)
                    return;

                lastProcessedTime = now;

                var newQuote = new Quote
                {
                    Date = now,
                    Open = tradeData.Price,
                    High = tradeData.Price,
                    Low = tradeData.Price,
                    Close = tradeData.Price,
                    Volume = tradeData.Quantity
                };
                quotes.Add(newQuote);

                if (quotes.Count > 0)
                {
                    if (quotes.Count > 35)
                        quotes.RemoveAt(0);

                    try
                    {
                        var (finalCapital, finalPosition, trades) = await backtestRsiMacd.RunBacktestOnline(
                            symbol,
                            interval,
                            buyRsiThreshold,
                            sellRsiThreshold,
                            emaPeriod,
                            quotes,
                            capital,
                            position,
                            tradeList);

                        Console.WriteLine(trades.Count());

                        capital = finalCapital;
                        position = finalPosition;
                        tradeList = trades;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"RunBacktestOnline error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing trade data: {ex.Message}");
            }
        };

        _socket.Connect();
    }


    public void DisconnectSocket()
    {
        _socket?.Close();
        Console.WriteLine("WebSocket disconnected.");
    }

    public List<Quote> GetQuotes()
    {
        return _quotes;
    }
}

public class KlineMessage
{
    [JsonProperty("k")]
    public KlineData Kline { get; set; }
}

public class KlineData
{
    [JsonProperty("t")]
    public long StartTime { get; set; }

    [JsonProperty("T")]
    public long CloseTime { get; set; }

    [JsonProperty("o")]
    public string Open { get; set; }

    [JsonProperty("h")]
    public string High { get; set; }

    [JsonProperty("l")]
    public string Low { get; set; }

    [JsonProperty("c")]
    public string Close { get; set; }

    [JsonProperty("v")]
    public string Volume { get; set; }

    [JsonProperty("x")]
    public bool IsFinal { get; set; }
}

public class TradeData
{
    [JsonProperty("s")]
    public string Symbol { get; set; } 

    [JsonProperty("p")]
    public decimal Price { get; set; }

    [JsonProperty("q")]
    public decimal Quantity { get; set; } 

    [JsonProperty("T")]
    public long TradeTime { get; set; }

    [JsonProperty("m")]
    public bool IsBuyerMaker { get; set; }

    [JsonProperty("t")]
    public long TradeId { get; set; }
}
