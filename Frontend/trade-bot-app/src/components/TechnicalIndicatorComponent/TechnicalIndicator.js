import React, { useEffect, useRef } from "react";
import { createChart } from "lightweight-charts";
import { RSI, MACD, EMA } from "technicalindicators";
import "./TechnicalIndicator.css";

const TechnicalIndicator = ({ candleData }) => {
  const rsiChartRef = useRef(null);
  const macdChartRef = useRef(null);
  const emaChartRef = useRef(null);

  useEffect(() => {
    if (!candleData.length) return;
    const closePrices = candleData.map((candle) => candle.close);

    const rsiValues = RSI.calculate({ values: closePrices, period: 14 }).map((value, index) => ({
      time: candleData[index + 13].time, 
      value,
    }));

    const macdValues = MACD.calculate({
      values: closePrices,
      fastPeriod: 12,
      slowPeriod: 26,
      signalPeriod: 9,
      SimpleMAOscillator: false,
      SimpleMASignal: false,
    }).map((value, index) => ({
      time: candleData[index + 25].time, 
      value: value.MACD,
    }));

    const emaValues = EMA.calculate({ values: closePrices, period: 14 }).map((value, index) => ({
      time: candleData[index + 13].time,
      value,
    }));

    // Create RSI Chart
    const rsiChart = createChart(rsiChartRef.current, { width: 750, height: 200 });
    const rsiSeries = rsiChart.addLineSeries({ color: "#ff5733", lineWidth: 2 });
    rsiSeries.setData(rsiValues);

    // Create MACD Chart
    const macdChart = createChart(macdChartRef.current, { width: 750, height: 200 });
    const macdSeries = macdChart.addLineSeries({ color: "#33b5ff", lineWidth: 2 });
    macdSeries.setData(macdValues);

    // Create EMA Chart
    const emaChart = createChart(emaChartRef.current, { width: 750, height: 200 });
    const emaSeries = emaChart.addLineSeries({ color: "#4CAF50", lineWidth: 2 });
    emaSeries.setData(emaValues);

    return () => {
      rsiChart.remove();
      macdChart.remove();
      emaChart.remove();
    };
  }, [candleData]);

  return (
    <div className="technical-charts">
        <div>
            <p style={{margin:'2px', textAlign:'start'}} >RSI</p>  
            <div ref={rsiChartRef} className="chart-container" />
        </div>
        <div>
            <p style={{margin:'2px', textAlign:'start'}} >MACD</p>  
            <div ref={macdChartRef} className="chart-container" />
        </div>
        <div>
            <p style={{margin:'2px', textAlign:'start'}} >EMA</p>  
            <div ref={emaChartRef} className="chart-container" />
        </div>
    </div>
  );
};

export default TechnicalIndicator;
