namespace TradingEngine.Core.Domain;

public record class Order
{
  public required Guid Id { get; set; } 
  public required OrderSide Side { get; set; } 
  public required decimal Price { get; set; } 
  public required int Quantity { get; set; } 
  public required DateTime Timestamp { get; set; } 
  public required Guid TraderId { get; set; } 
  public required OrderType Type { get; set; } 
  public required string Symbol { get; set; } 

  public static Order Create(Guid traderId, string symbol, decimal price, int quantity, OrderSide side, OrderType type) 
  {
    return new Order
    {
      Id = Guid.NewGuid(),
      Side = side,
      Price = price,
      Quantity = quantity,
      Timestamp = DateTime.UtcNow,
      TraderId = traderId,
      Type = type,
      Symbol = symbol
    };
  }
}


