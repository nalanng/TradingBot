import React from "react";
import "./IndicatorControls.css";

const BacktestControls = ({
  backtestParams,
  setBacktestParams,
  handleBacktest,
  handleLiveTrades,
  isLoading,
  availableCoins,
}) => {
  const handleSymbolChange = (event) => {
    const selectedSymbol = event.target.value;
    setBacktestParams({ ...backtestParams, symbol: selectedSymbol });
  };

  return (
    <div className="common-container">
      <h3 className="section-title">Run Backtest and Live Trading</h3>
      <div className="backtest-controls">
        <div className="backtest-div">
        <div className="coin-selector">
          <select
            id="coin-select"
            value={backtestParams.symbol}
            onChange={handleSymbolChange}
            className="select-input"
          >
            {availableCoins.map((coin) => (
              <option key={coin} value={coin}>
                {coin}
              </option>
            ))}
          </select>
        </div>
        <input
          type="number"
          placeholder="Buy RSI Threshold"
          value={backtestParams.buyRsiThreshold}
          onChange={(e) =>
            setBacktestParams({ ...backtestParams, buyRsiThreshold: parseInt(e.target.value) })
          }
          className="input-box"
        />
        <input
          type="number"
          placeholder="Sell RSI Threshold"
          value={backtestParams.sellRsiThreshold}
          onChange={(e) =>
            setBacktestParams({ ...backtestParams, sellRsiThreshold: parseInt(e.target.value) })
          }
          className="input-box"
        />
        <input
          type="number"
          placeholder="EMA Period"
          value={backtestParams.emaPeriod}
          onChange={(e) =>
            setBacktestParams({ ...backtestParams, emaPeriod: parseInt(e.target.value) })
          }
          className="input-box"
        />
        <select
          value={backtestParams.backtestInterval}
          onChange={(e) =>
            setBacktestParams({ ...backtestParams, backtestInterval: e.target.value })
          }
          className="select-input"
        >
          <option value="1m">1 Minutes</option>
          <option value="15m">15 Minutes</option>
          <option value="1h">1 Hour</option>
          <option value="4h">4 Hours</option>
          <option value="1d">1 Day</option>
        </select>
        </div>
        <div className="backtest-div">
        <button onClick={handleBacktest} className="backtest-button" disabled={isLoading}>
          {isLoading ? "Running..." : "Run Backtest"}
        </button>
        <button onClick={handleLiveTrades} className="backtest-button" disabled={isLoading}>
          {isLoading ? "Starting..." : "Run Live Trades"}
        </button>
        </div>
      </div>
    </div>
  );
};

export default BacktestControls;
