using Skender.Stock.Indicators;
namespace TradingBot.Services;
public class TechnicalIndicators
{
    public IEnumerable<RsiResult> CalculateRsi(IEnumerable<Quote> quotes, int period = 14)
    {
        return quotes.GetRsi(period);
    }

    public IEnumerable<MacdResult> CalculateMacd(IEnumerable<Quote> quotes, int period = 14)
    {
        return quotes.GetMacd(period);
    }

    public IEnumerable<SmaResult> CalculateSMA(IEnumerable<Quote> quotes, int period = 14)
    {
        return quotes.GetSma(period);
    }

    public IEnumerable<EmaResult> CalculateEMA(IEnumerable<Quote> quotes, int period = 14)
    {
        return quotes.GetEma(period);
    }
    public IEnumerable<BollingerBandsResult> CalculateBollingerBands(IEnumerable<Quote> quotes, int period = 14, double standardDeviations = 2)
    {
        return quotes.GetBollingerBands(period, standardDeviations);
    }

    public IEnumerable<SuperTrendResult> CalculateSuperTrend(IEnumerable<Quote> quotes, int period = 14, double multiplier = 3)
    {
        return quotes.GetSuperTrend(period, multiplier);
    }
}
