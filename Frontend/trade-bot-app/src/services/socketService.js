import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

const BASE_SOCKET_URL = "http://localhost:5251";
const BINANCE_STREAM_URL = "wss://stream.binance.com:9443/ws";

let signalRConnection = null;

// SignalR Connection
export const startSignalRConnection = async (onReceiveTrade) => {
  if (!signalRConnection) {
    signalRConnection = new HubConnectionBuilder()
      .withUrl(`${BASE_SOCKET_URL}/tradeHub`)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();
  }

  try {
    await signalRConnection.start();
    console.log("SignalR Connected!");

    if (onReceiveTrade) {
      signalRConnection.on("ReceiveTrade", onReceiveTrade);
    }

    return signalRConnection;
  } catch (error) {
    console.error("SignalR Connection Error:", error);
    throw error;
  }
};

export const stopSignalRConnection = async () => {
  if (signalRConnection) {
    try {
      await signalRConnection.stop();
      console.log("SignalR Disconnected");
    } catch (error) {
      console.error("Error stopping SignalR connection:", error);
    }
  }
};

// WebSocket Connection
export const startWebSocketConnection = (symbol, interval, onMessage) => {
  const socket = new WebSocket(`${BINANCE_STREAM_URL}/${symbol.toLowerCase()}@kline_${interval}`);
  socket.onmessage = (event) => {
    const message = JSON.parse(event.data);
    if (onMessage) {
      onMessage(message);
    }
  };

  socket.onerror = (error) => {
    console.error("WebSocket Error:", error);
  };

  socket.onclose = () => {
    console.log("WebSocket Closed");
  };

  return socket;
};

export const startTradeSocketConnection = (symbol, onMessage) => {
  const socket = new WebSocket(`${BINANCE_STREAM_URL}/${symbol.toLowerCase()}@trade`);

  socket.onmessage = (event) => {
    const message = JSON.parse(event.data);
    if (onMessage) {
      onMessage(message);
    }
  };

  socket.onerror = (error) => {
    console.error("WebSocket Error:", error);
  };

  socket.onclose = () => {
    console.log("WebSocket Closed");
  };

  return socket;
};
