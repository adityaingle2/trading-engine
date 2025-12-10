using TradingEngine.Core.Domain;

namespace TradingEngine.Core.Messaging;

public abstract record EngineMessage(string Symbol, long Timestamp);

public record NewOrderMessage(
  string Symbol,
  long Timestamp,
  Order Order
) : EngineMessage(Symbol, Timestamp);

public record CancelOrderMessage(
  string Symbol,
  long Timestamp,
  Guid OrderId
) : EngineMessage(Symbol, Timestamp);

public record UpdateOrderMessage(
  string Symbol,
  long Timestamp,
  Guid ExistingOrderId,
  decimal? NewPrice = null,
  int? NewQuantity = null
) : EngineMessage(Symbol, Timestamp);



