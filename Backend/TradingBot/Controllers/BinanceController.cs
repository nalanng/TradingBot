using Microsoft.AspNetCore.Mvc;
using TradingBot.Services;

namespace TradingBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BinanceController : ControllerBase
    {
        private readonly BinanceService _binanceService;

        public BinanceController(BinanceService binanceService)
        {
            _binanceService = binanceService;
        }

        [HttpGet("GetHistoricalQuotes")]
        public async Task<IActionResult> GetHistoricalQuotes([FromQuery] string symbol, [FromQuery] string interval, [FromQuery] string range)
        {
            try
            {
                var quotes = await _binanceService.GetHistoricalData(symbol, interval, range);

                if (quotes == null || quotes.Count == 0)
                    return NotFound("No data found for the given parameters.");

                return Ok(quotes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
