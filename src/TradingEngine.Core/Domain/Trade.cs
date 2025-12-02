namespace TradingEngine.Core.Domain;

public class Trade
{
  public required Guid BuyOrderId { get; set; }
  public required Guid SellOrderId { get; set; }
  public required decimal Price { get; set; }
  public required int Quantity { get; set; }
  public required DateTime Timestamp { get; set; }

  public static Trade Create(Guid buyOrderId, Guid sellOrderId, decimal price, int quantity)
  {
    return new Trade
    {
      BuyOrderId = buyOrderId,
      SellOrderId = sellOrderId,
      Price = price,
      Quantity = quantity,
      Timestamp = DateTime.UtcNow
    };
  }
}