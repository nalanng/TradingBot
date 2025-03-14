using TradingBot.Hubs;
using TradingBot.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddSignalR();

// Add services for the bot and WebSocket client
builder.Services.AddSingleton<BacktestService>();
builder.Services.AddSingleton<BinanceService>();
builder.Services.AddSingleton<TechnicalIndicators>();
builder.Services.AddSingleton<BinanceWebSocketService>();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowSpecificOrigins");

app.UseHttpsRedirection();
app.UseAuthorization();

// Map controllers to handle HTTP requests
app.MapControllers();

// Map SignalR hub
app.MapHub<TradeHub>("/tradeHub");

app.Run();
