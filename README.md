# 📈 Engineering Economics Trading Bot

A high-performance cryptocurrency trading bot for **backtesting** and **live trading** using real-time market data from Binance. Built with C# (.NET 8) and featuring a SignalR-powered frontend, this bot supports multiple technical indicators and timeframes for dynamic trading decisions.

---

## 🧠 Overview

Supports real-time and historical analysis for the following cryptocurrency pairs:

- **BTCUSDT**
- **ETHUSDT**
- **AVAXUSDT**
- **SOLUSDT**
- **RENDERUSDT**
- **FETUSDT**

### 🧮 Technical Indicators
- **RSI** (Relative Strength Index)
- **MACD** (Moving Average Convergence Divergence)
- **EMA** (Exponential Moving Average)

---

## ⚙️ Features

### ✅ Backtesting
- Historical simulation using OHLCV data
- Timeframes: `15m`, `1h`, `4h`, `1d`
- Strategy:
  - RSI, MACD, EMA
- Fully customizable thresholds
- Trade logging with profit/loss tracking

### 🔴 Live Trading
- Real-time execution via Binance WebSocket API
- Interval: `1-minute`
- Dynamic indicator calculation
- Trade signal generation and live order tracking
- Frontend updates via SignalR

---

## 📊 Frontend Visualization

- Real-time candlestick charts
- RSI, MACD, EMA indicator overlays
- Trade logs with timestamps and PnL
- User-configurable strategy parameters
- Capital tracking & live trade visualization

---

## 🧪 Strategy Logic

### 📥 Buy Signal
- `RSI < BuyThreshold` → Oversold condition  
- `MACD > Signal - 0.1` → Bullish momentum  
- `Close Price > 85% of EMA` → Strong upward trend  

### 📤 Sell Signal
- `RSI > SellThreshold` → Overbought condition  
- `MACD < Signal + 0.1` → Bearish momentum  

---

## 🔁 Backtesting Methodology

- Uses: `_binanceService.GetHistoricalData()`
- Calculates RSI, MACD, EMA
- Simulates trades with virtual capital
- Logs:
  - Final capital
  - Trade records
  - Net profit/loss

---

## 🌐 Live Trading Methodology

- Connects to Binance WebSocket streams
- Builds 1-minute candlesticks from real-time trades
- Updates indicators in real time
- Executes trades on signal triggers
- Sends updates to frontend using SignalR

---

## 📈 Sample Results

### 🪙 BTCUSDT
- `1d`: **10,665.13 USDT** (RSI: 50/50)  
- `4h`: **10,252.25 USDT** (RSI: 40/60)  

### 🪙 ETHUSDT
- `4h`: **14,229.95 USDT** (RSI: 40/60)  
- `1h`: **11,588.37 USDT** (RSI: 30/70)  

### 🪙 FETUSDT
- `15m`: **14,553.19 USDT** (RSI: 40/60)  
- **Live Trading**: Final capital **1007.44 USDT** (Buy & Sell executed in 30 mins)  

### 🪙 RENDERUSDT
- `15m`: **22,985.39 USDT** (RSI: 50/50)  

> ⚠️ Note: High-volatility coins like FETUSDT tend to perform better due to more frequent trade signals.

---

## 🛠 Technologies Used

- **Backend**: C# (.NET 8), Binance API, WebSocket
- **Frontend**: SignalR, Charts (JS framework)
- **Indicators**: RSI, MACD, EMA
- **Visualization**: Real-time graphs, capital logs, live trades

---

## 📋 Future Improvements

- ✅ Stop-loss & take-profit support  
- ✅ Paper trading or sandbox integration  
- ✅ Trade overlays on historical price chart  
- ✅ Portfolio-level strategy and coin rotation logic  
- ✅ Machine Learning-based indicator tuning  
