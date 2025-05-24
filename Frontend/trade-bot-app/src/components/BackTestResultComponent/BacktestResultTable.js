import React from "react";
import "./BacktestResultTable.css";

const BacktestResults = ({ backtestResult, symbol }) => {
  return (
    <div className="backtest-results-container">
      <h3 className="section-title">
        Backtest Results for <span style={{ color: "#ffcc00" }}>{symbol}</span>
      </h3>
      {backtestResult && backtestResult.trades.length > 0 ? (
        <div>
          <div className="final-capital">
            <strong>Final Capital: </strong>
            {backtestResult.finalCapital ? backtestResult.finalCapital.toFixed(2) : "N/A"}
          </div>
          <table className="backtest-results-table">
            <thead>
              <tr>
                <th>Trade Date</th>
                <th>Type</th>
                <th>Price</th>
                <th>Quantity</th>
                <th>Profit</th>
              </tr>
            </thead>
            <tbody>
              {backtestResult.trades.map((trade, index) => (
                <tr
                  key={index}
                  className={index % 2 === 0 ? "row-even" : "row-odd"}
                  style={{
                    color: trade.type === "Buy" ? "#4CAF50" : "#F44336",
                  }}
                >
                  <td>{new Date(trade.date).toLocaleString()}</td>
                  <td>{trade.type}</td>
                  <td>{trade.price ? trade.price.toFixed(2) : "N/A"}</td>
                  <td>{trade.quantity ? trade.quantity.toFixed(4) : "0"}</td>
                  <td>
                    {trade.profit !== null && trade.profit !== undefined
                      ? trade.profit.toFixed(2)
                      : "N/A"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <p>No trades available</p>
      )}
    </div>
  );
};

export default BacktestResults;
