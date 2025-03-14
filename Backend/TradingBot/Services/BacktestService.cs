using Microsoft.AspNetCore.SignalR;
using Skender.Stock.Indicators;
using TradingBot.Hubs;

namespace TradingBot.Services
{
    public class BacktestService
    {
        private readonly BinanceService _binanceService;
        private readonly TechnicalIndicators _technicalIndicators;
        private readonly IHubContext<TradeHub> _hubContext;

        public BacktestService(
            BinanceService binanceService,
            TechnicalIndicators technicalIndicators,
             IHubContext<TradeHub> hubContext)
        {
            _binanceService = binanceService;
            _technicalIndicators = technicalIndicators;
            _hubContext = hubContext;
        }

        public async Task<(decimal finalCapital, List<Trade> trades)> RunBacktest(
            string symbol,
            string interval,
            int buyRsiThreshold,
            int sellRsiThreshold,
            int emaPeriod)
        {
            var quotes = await _binanceService.GetHistoricalData(symbol, interval, "4months");

            var rsiResults = _technicalIndicators.CalculateRsi(quotes);
            var macdResults = _technicalIndicators.CalculateMacd(quotes);
            var emaResults = _technicalIndicators.CalculateEMA(quotes, emaPeriod);

            decimal startingCapital = 10000;
            decimal currentBalance = startingCapital;
            decimal currentPosition = 0; 
            decimal initialPrice = 0;

            List<Trade> trades = new List<Trade>();

            int length = quotes.Count();
            for (int i = Math.Max(emaPeriod, 14); i < length; i++) 
            {
                var rsi = rsiResults.ElementAt(i);
                var macd = macdResults.ElementAt(i);
                var ema = emaResults.ElementAt(i);
                var quote = quotes.ElementAt(i);

                if (!ema.Ema.HasValue || macd.Macd == null || macd.Signal == null)
                    continue;

                // Buy Signal
                if (rsi.Rsi <= buyRsiThreshold &&
                     (decimal)macd.Macd > (decimal)macd.Signal - 0.1m &&
                    quote.Close >= (decimal)ema.Ema.Value * 0.85m &&
                    currentPosition == 0)                 
                {
                    initialPrice = quote.Close;
                    currentPosition = currentBalance / initialPrice;
                    currentBalance = 0;

                    trades.Add(new Trade
                    {
                        Type = "Buy",
                        Date = quote.Date,
                        Price = initialPrice,
                        Quantity = currentPosition,
                        Capital = currentBalance,
                        Profit = null 
                    });

                    Console.WriteLine($"Buying at {initialPrice} on {quote.Date}");
                }

                // Sell Signal
                else if (rsi.Rsi >= sellRsiThreshold &&
                        (decimal)macd.Macd < (decimal)macd.Signal + 0.1m &&
                         currentPosition > 0)              
                {
                    decimal sellPrice = quote.Close;
                    decimal sellCapital = currentPosition * sellPrice;
                    decimal profit = sellCapital - startingCapital;

                    trades.Add(new Trade
                    {
                        Type = "Sell",
                        Date = quote.Date,
                        Price = sellPrice,
                        Quantity = currentPosition,
                        Capital = sellCapital,
                        Profit = profit
                    });

                    currentBalance = sellCapital;
                    currentPosition = 0;

                    Console.WriteLine($"Selling at {sellPrice} on {quote.Date} with profit: {profit}");
                }
            }

            return (currentBalance, trades);
        }

            public async Task<(decimal finalCapital, decimal finalPosition, List<Trade> trades)> RunBacktestOnline(
                string symbol,
                string interval,
                int buyRsiThreshold,
                int sellRsiThreshold,
                int emaPeriod,
                List<Quote> quotes,
                decimal finalCapital,
                decimal currentPosition,
                List<Trade> trades)   
            {

                var rsiResults = _technicalIndicators.CalculateRsi(quotes);
                var macdResults = _technicalIndicators.CalculateMacd(quotes);
                var emaResults = _technicalIndicators.CalculateEMA(quotes, emaPeriod);

                decimal startingCapital = finalCapital;

                int length = quotes.Count();
                for (int i = Math.Max(emaPeriod, 14); i < length; i++)
                {
                    var rsi = rsiResults.ElementAt(i);
                    var macd = macdResults.ElementAt(i);
                    var ema = emaResults.ElementAt(i);
                    var quote = quotes.ElementAt(i);

                    if (!ema.Ema.HasValue || macd.Macd == null || macd.Signal == null)
                        continue;

                Console.WriteLine(rsi.Rsi);
                Console.WriteLine(macd.Macd);
                Console.WriteLine(macd.Signal);
                Console.WriteLine(quote.Close);
                Console.WriteLine(ema.Ema.Value);
                // Buy Signal
                int buyConditionsMet = 0;
                if (rsi.Rsi <= buyRsiThreshold) buyConditionsMet++;
                if (macd.Macd > macd.Signal) buyConditionsMet++;
                if (quote.Close >= (decimal)ema.Ema.Value * 0.95m) buyConditionsMet++;

                if (buyConditionsMet >= 2 && currentPosition == 0)
                {
                    decimal initialPrice = quote.Close;
                    currentPosition = finalCapital / initialPrice;
                    finalCapital = 0;

                    if (!trades.Any(t => t.Date == quote.Date))
                    {
                        trades.Add(new Trade
                        {
                            Type = "Buy",
                            Date = quote.Date,
                            Price = initialPrice,
                            Quantity = currentPosition,
                            Capital = finalCapital,
                            Profit = null
                        });
                    }
                    Console.WriteLine("Buy");

                    var backtest = new
                    {
                        finalCapital = finalCapital,
                        trades = trades.OrderByDescending(t => t.Date).Take(25).ToList()
                    };

                    await _hubContext.Clients.All.SendAsync("ReceiveTrade", backtest);
                }

                // Sell Signal
                int sellConditionsMet = 0;
                if (rsi.Rsi >= sellRsiThreshold) sellConditionsMet++;
                if (macd.Macd < macd.Signal) sellConditionsMet++;
                if (currentPosition > 0) sellConditionsMet++;

                if (sellConditionsMet == 3)
                {
                    decimal sellPrice = quote.Close;
                    decimal sellCapital = currentPosition * sellPrice;
                    decimal profit = sellCapital - startingCapital;

                    if (!trades.Any(t => t.Date == quote.Date))
                    {
                        trades.Add(new Trade
                        {
                            Type = "Sell",
                            Date = quote.Date,
                            Price = sellPrice,
                            Quantity = currentPosition,
                            Capital = sellCapital,
                            Profit = profit
                        });

                        finalCapital = sellCapital;
                        currentPosition = 0;
                    }
                    Console.WriteLine("Sell");

                    var backtest = new
                    {
                        finalCapital = finalCapital,
                        trades = trades.OrderByDescending(t => t.Date).Take(25).ToList()
                    };

                    await _hubContext.Clients.All.SendAsync("ReceiveTrade", backtest);
                }

            }

            return (finalCapital, currentPosition, trades);
            }

    }


}

public class Trade
{
    public string Type { get; set; } 
    public DateTime Date { get; set; }
    public decimal Price { get; set; } 
    public decimal Quantity { get; set; } 
    public decimal Capital { get; set; }
    public decimal? Profit { get; set; } 
}