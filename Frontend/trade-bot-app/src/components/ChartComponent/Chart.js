import React, { useEffect, useRef, useState } from "react";
import { createChart } from "lightweight-charts";
import "./Chart.css";

const availableCoins = ["ETHUSDT", "BTCUSDT", "AVAXUSDT", "SOLUSDT", "RENDERUSDT", "FETUSDT"];
const availableIntervals = ["1m", "15m", "1h", "4h", "1d"];

const CandlestickChart = ({ candleData, onCoinChange, onIntervalChange }) => {
  const chartContainerRef = useRef(null);
  const tooltipRef = useRef(null);
  const [selectedCoin, setSelectedCoin] = useState(availableCoins[0]);
  const [selectedInterval, setSelectedInterval] = useState(availableIntervals[2]);

  const handleCoinChange = (event) => {
    const newCoin = event.target.value;
    setSelectedCoin(newCoin);
    onCoinChange(newCoin, selectedInterval);
  };

  const handleIntervalChange = (event) => {
    const newInterval = event.target.value;
    setSelectedInterval(newInterval);
    onIntervalChange(selectedCoin, newInterval);
  };

  useEffect(() => {
    const chart = createChart(chartContainerRef.current, {
      width: 1200,
      height: 400,
      layout: {
        backgroundColor: "#ffffff",
        textColor: "#000000",
      },
      grid: {
        vertLines: { color: "#e1e1e1" },
        horzLines: { color: "#e1e1e1" },
      },
      timeScale: {
        borderColor: "#cccccc",
      },
    });

    const candlestickSeries = chart.addCandlestickSeries({
      upColor: "#4CAF50",
      downColor: "#F44336",
      borderDownColor: "#F44336",
      borderUpColor: "#4CAF50",
      wickDownColor: "#F44336",
      wickUpColor: "#4CAF50",
    });

    candlestickSeries.setData(candleData);

    // Tooltip setup
    chart.subscribeCrosshairMove((param) => {
      if (!param || !param.time) {
        tooltipRef.current.style.display = "none";
        return;
      }

      const timestamp = param.time;

      const time = new Date(timestamp * 1000).toLocaleTimeString();
      tooltipRef.current.style.display = "block";
      tooltipRef.current.style.left = `${param.point.x + 20}px`;
      tooltipRef.current.style.top = `${param.point.y + 20}px`;
      tooltipRef.current.innerHTML = `${time}`;
    });

    return () => chart.remove();
  }, [candleData]);

  return (
    <div style={{ position: "relative" }}>
      <div className="chart-controls">
        <select value={selectedCoin} onChange={handleCoinChange} className="dropdown">
          {availableCoins.map((coin) => (
            <option key={coin} value={coin}>
              {coin}
            </option>
          ))}
        </select>

        <select value={selectedInterval} onChange={handleIntervalChange} className="dropdown">
          {availableIntervals.map((interval) => (
            <option key={interval} value={interval}>
              {interval}
            </option>
          ))}
        </select>
      </div>

      <div ref={chartContainerRef} className="chart-container" />
      <div ref={tooltipRef} className="chart-tooltip"></div>
    </div>
  );
};

export default CandlestickChart;
