namespace TradingEngine.Models;

public record class Order
{
  public required Guid Id { get; set; } // Unique identifier for the order
  public required OrderSide Side { get; set; } // Order side: Buy or Sell
  public required decimal Price { get; set; } // Order price
  public required int Quantity { get; set; } // Quantity of shares
  public required DateTime Timestamp { get; set; } // Timestamp (for price-time priority)
  public required Guid TraderId { get; set; } // Optional: additional metadata like trader ID
  public required OrderType Type { get; set; } // Order type: Market, Limit, Stop, Stop-Limit
  public required string Symbol { get; set; } // Trading symbol (e.g., stock ticker)
  public required DateTime LastUpdated { get; set; } // Last updated timestamp
}
