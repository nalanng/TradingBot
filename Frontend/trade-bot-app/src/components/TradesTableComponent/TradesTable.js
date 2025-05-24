import React from "react";
import './TradesTable.css'
const TradesTable = ({ trades, symbol }) => {
  const assetName = symbol.slice(0, -4);

  return (
    <div>
      <h3 className="section-title">Market Trades</h3>
      <div className="trades-container">
        <table className="trades-table">
          <thead>
            <tr>
              <th>Price (USDT)</th>
              <th>Amount ({assetName})</th>
              <th>Time</th>
            </tr>
          </thead>
          <tbody>
            {trades.map((trade, index) => (
              <tr key={index} className={index % 2 === 0 ? "row-even" : "row-odd"}>
                <td style={{ color: trade.isBuy ? "#4CAF50" : "#F44336" }}>{trade.price.toFixed(2)}</td>
                <td>{trade.quantity.toFixed(4)}</td>
                <td>{trade.tradeTime.split(" ")[1]}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default TradesTable;
