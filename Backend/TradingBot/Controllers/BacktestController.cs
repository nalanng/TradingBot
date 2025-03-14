using Microsoft.AspNetCore.Mvc;
using TradingBot.Services;

namespace TradingBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BacktestController : ControllerBase
    {
        private readonly BacktestService backtestService;

        public BacktestController(BacktestService backtestService)
        {
            this.backtestService = backtestService;
        }

        [HttpGet("run-backtest")]
        public async Task<IActionResult> RunBacktest(string symbol, string interval, int buyRsiThreshold, int sellRsiThreshold, int emaPeriod)
        {
            try
            {
                var (finalCapital, trades) = await backtestService.RunBacktest(symbol, interval, buyRsiThreshold, sellRsiThreshold, emaPeriod);

                return Ok(new
                {
                    finalCapital,
                    trades
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error running RSI MACD backtest", error = ex.Message });
            }
        }

    }
}
