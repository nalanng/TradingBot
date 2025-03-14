using Microsoft.AspNetCore.Mvc;
using Skender.Stock.Indicators;
using TradingBot.Services;

namespace TradingBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveBacktestController : ControllerBase
    {
        private readonly BinanceWebSocketService _binanceWebSocketService;
        private readonly BinanceService _binanceService;    

        public LiveBacktestController(BinanceWebSocketService binanceWebSocketService, BinanceService binanceService)
        {
            _binanceWebSocketService = binanceWebSocketService;
            _binanceService = binanceService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartTradingBot(
            string symbol = "ethusdt",
            string interval = "1m",
            int buyRsiThreshold = 20,
            int sellRsiThreshold = 80,
            int emaPeriod = 14)
        {
            try
            {
                _binanceWebSocketService.DisconnectSocket();
                var quotes = await _binanceService.GetHistoricalData(symbol, interval, "35");

                _binanceWebSocketService.ConnectSocket(
                    symbol.ToLower(),
                    interval,
                    buyRsiThreshold,
                    sellRsiThreshold,
                    emaPeriod,
                    quotes);

                return Ok($"Started tracking {symbol.ToUpper()} on Binance Testnet.");
            }
            catch (HttpRequestException httpEx)
            {
                // Handle HTTP-specific exceptions (e.g., API request failures)
                return BadRequest($"API Error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                // Catch all other exceptions
                return BadRequest($"Error starting bot: {ex.Message}");
            }
        }
    }
}
