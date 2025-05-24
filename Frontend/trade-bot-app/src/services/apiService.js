const BASE_URL = "http://localhost:5251/api";

const apiRequest = async (endpoint, method = "GET", body = null) => {
  const headers = {
    "Content-Type": "application/json",
  };

  const options = {
    method,
    headers,
  };

  if (body) {
    options.body = JSON.stringify(body);
  }

  try {
    const response = await fetch(`${BASE_URL}${endpoint}`, options);
    if (!response.ok) {
      throw new Error(`API Error: ${response.status}`);
    }
    return await response.json();
  } catch (error) {
    console.error("API Request Failed:", error);
    throw error;
  }
};

// Service Functions

export const getHistoricalQuotes = (symbol, interval, range) =>
  apiRequest(`/Binance/GetHistoricalQuotes?symbol=${symbol}&interval=${interval}&range=${range}`);

export const runBacktest = (params) =>
  apiRequest(
    `/Backtest/run-backtest?symbol=${params.symbol}&interval=${params.backtestInterval}&buyRsiThreshold=${params.buyRsiThreshold}&sellRsiThreshold=${params.sellRsiThreshold}&emaPeriod=${params.emaPeriod}`
  );

  export const runLiveTrade = (params) =>
    apiRequest(
      `/LiveBacktest/start?symbol=${params.symbol}&interval=${params.backtestInterval}&buyRsiThreshold=${params.buyRsiThreshold}&sellRsiThreshold=${params.sellRsiThreshold}&emaPeriod=${params.emaPeriod}`,
      "POST"
    );
  