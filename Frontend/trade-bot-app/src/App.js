import React, { useEffect, useState } from "react";
import ChartSection from "./components/ChartComponent/Chart";
import TradesTable from "./components/TradesTableComponent/TradesTable";
import BacktestControls from "./components/IndicatorControlsComponent/IndicatorControls";
import BacktestResults from "./components/BackTestResultComponent/BacktestResultTable";
import CandleInfo from "./components/CandleInfoComponent/CandleInfo";
import TechnicalIndicator from "./components/TechnicalIndicatorComponent/TechnicalIndicator";
import "./CryptoDashboard.css";
import {
  startSignalRConnection,
  stopSignalRConnection,
  startWebSocketConnection,
  startTradeSocketConnection,
} from "./services/socketService";
import { getHistoricalQuotes, runBacktest, runLiveTrade } from "./services/apiService";

const CryptoDashboard = () => {
  const [candleData, setCandleData] = useState([]);
  const [trades, setTrades] = useState([]);
  const [backtestParams, setBacktestParams] = useState({
    symbol: "ETHUSDT",
    buyRsiThreshold: 30,
    sellRsiThreshold: 70,
    emaPeriod: 14,
    backtestInterval: "1h",
  });
  const [backtestResult, setBacktestResult] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [lastCandle, setLastCandle] = useState();
  const [liveBacktestResult, setLiveBacktestResult] = useState(null);
  const [activeTab, setActiveTab] = useState("backtest");
  const [selectedCoin, setSelectedCoin] = useState("ETHUSDT");
  const [selectedInterval, setSelectedInterval] = useState("1h");

  const availableCoins = ["ETHUSDT", "BTCUSDT", "AVAXUSDT", "SOLUSDT", "RENDERUSDT", "FETUSDT"];

  const fetchCandleData = async (coin, interval) => {
    setSelectedCoin(coin);
    setSelectedInterval(interval);

    try {
      const data = await getHistoricalQuotes(coin, interval, "last250");
      const formattedData = data.map((kline) => ({
        time: Math.floor(new Date(kline.date).getTime() / 1000),
        open: parseFloat(kline.open),
        high: parseFloat(kline.high),
        low: parseFloat(kline.low),
        close: parseFloat(kline.close),
      }));
      setCandleData(formattedData);
    } catch (error) {
      console.error("Failed to fetch candle data:", error);
    }
  };

  useEffect(() => {
    fetchCandleData(selectedCoin, selectedInterval);
  }, []);
  
  useEffect(() => {
    const candleSocket = startWebSocketConnection(
      selectedCoin,
      selectedInterval,
      (message) => {
        const kline = message.k;

        const newCandle = {
          time: Math.floor(kline.t / 1000),
          open: parseFloat(kline.o),
          high: parseFloat(kline.h),
          low: parseFloat(kline.l),
          close: parseFloat(kline.c),
        };

        setLastCandle(newCandle);

        setCandleData((prevData) => {
          const isDuplicate = prevData.some((candle) => candle.time === newCandle.time);
          if (isDuplicate) return prevData;

          const updatedData = [...prevData, newCandle];
          const sortedData = updatedData.sort((a, b) => a.time - b.time);

          if (sortedData.length > 100) sortedData.shift();

          return sortedData;
        });
      }
    );

    return () => candleSocket.close();
  }, [selectedCoin, selectedInterval]);

  useEffect(() => {
    const tradeSocket = startTradeSocketConnection(selectedCoin, (message) => {
      const newTrade = {
        price: parseFloat(message.p),
        quantity: parseFloat(message.q),
        tradeTime: new Date(message.T).toLocaleString(),
        isBuy: !message.m,
      };
      setTrades((prevTrades) => [newTrade, ...prevTrades.slice(0, 25)]);
    });

    return () => tradeSocket.close();
  }, [selectedCoin]);

  useEffect(() => {
    let signalR;
    const setupSignalR = async () => {
      try {
        signalR = await startSignalRConnection((message) => {
          setLiveBacktestResult(message);
        });
      } catch (error) {
        console.error("Error setting up SignalR:", error);
      }
    };

    setupSignalR();

    return () => {
      if (signalR) {
        stopSignalRConnection();
      }
    };
  }, []);

  const handleBacktest = async () => {
    setIsLoading(true);
    try {
      console.log(backtestParams)
      const result = await runBacktest(backtestParams);
      setBacktestResult(result);
    } catch (error) {
      console.error("Error running backtest:", error);
      setBacktestResult(null);
    } finally {
      setIsLoading(false);
    }
  };

  const handleLiveTrades = async () => {
    setIsLoading(true);
    try {
      await runLiveTrade(backtestParams);
      alert("Live backtest started successfully!");
    } catch (error) {
      console.error("Live backtest failed:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleTabChange = (tab) => {
    setActiveTab(tab);
  };

  return (
    <div className="dashboard-container">
      <header className="dashboard-header">
        <h1 className="dashboard-title">Crypto Dashboard</h1>
      </header>

      <CandleInfo lastCandle={lastCandle} />

      <ChartSection
        candleData={candleData}
        onCoinChange={(coin, interval) => fetchCandleData(coin, interval)}
        onIntervalChange={(coin, interval) => fetchCandleData(coin, interval)}
      />

      <div className="dashboard-content">
        <TradesTable trades={trades} symbol={selectedCoin} />
        <TechnicalIndicator candleData={candleData} />
      </div>

      <BacktestControls
        backtestParams={backtestParams}
        setBacktestParams={setBacktestParams}
        handleBacktest={handleBacktest}
        handleLiveTrades={handleLiveTrades}
        isLoading={isLoading}
        availableCoins={availableCoins}
      />

      <div className="tab-content">
        <div className="tabs-container">
          <button
            className={`tab-button ${activeTab === "backtest" ? "active" : ""}`}
            onClick={() => handleTabChange("backtest")}
          >
            Backtest Results
          </button>
          <button
            className={`tab-button ${activeTab === "liveTrades" ? "active" : ""}`}
            onClick={() => handleTabChange("liveTrades")}
          >
            Live Trades
          </button>
        </div>
        {activeTab === "backtest" && (
          <BacktestResults backtestResult={backtestResult} symbol={backtestParams.symbol} activeTab={activeTab}/>
        )}
        {activeTab === "liveTrades" && (
          <BacktestResults backtestResult={liveBacktestResult} symbol={backtestParams.symbol} activeTab={activeTab} />
        )}
      </div>
    </div>
  );
};

export default CryptoDashboard;
