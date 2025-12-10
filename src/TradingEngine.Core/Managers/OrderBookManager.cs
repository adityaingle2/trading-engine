using TradingEngine.Core.Domain;

namespace TradingEngine.Core.Managers;

public class OrderBookManager
{
  private readonly Dictionary<string, OrderBook> _books = new();

  public void AddBook(string symbol)
  {
    _books[symbol] = new OrderBook(symbol);
  }

  public OrderBook GetBook(string symbol)
  {
    return _books[symbol];
  }
}

