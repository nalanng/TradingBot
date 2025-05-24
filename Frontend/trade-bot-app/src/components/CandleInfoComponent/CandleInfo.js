import React from "react";
import './CandleInfo.css'
const CandleInfo = ({ lastCandle }) => {
  if (!lastCandle) return null;

  return (
    <div className="candle-info">
      <p>
        <strong>Time:</strong> {new Date(lastCandle.time * 1000).toLocaleString()}
      </p>
      <p>
        <strong>Open:</strong> {lastCandle.open.toFixed(2)} | 
        <strong> High:</strong> {lastCandle.high.toFixed(2)} | 
        <strong> Low:</strong> {lastCandle.low.toFixed(2)} | 
        <strong> Close:</strong> {lastCandle.close.toFixed(2)}
      </p>
    </div>
  );
};

export default CandleInfo;
