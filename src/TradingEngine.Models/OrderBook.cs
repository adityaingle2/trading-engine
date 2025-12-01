namespace TradingEngine.Models;

public class OrderBook
{		
	private readonly SortedDictionary<decimal, LinkedList<Order>> _bids = new(Comparer<decimal>.Create((a, b) => b.CompareTo(a))); 	
	private readonly SortedDictionary<decimal, LinkedList<Order>> _asks = new(Comparer<decimal>.Default);	
	private readonly Dictionary<Guid, (decimal price, LinkedListNode<Order> node)> _orderLookup = new(); 
	public string TickerSymbol { get; } 
	
	private OrderBook(string tickerSymbol) 
	{
		TickerSymbol = tickerSymbol;
	}

	public static OrderBook Create(string tickerSymbol)
	{
		return new OrderBook(tickerSymbol);
	}

	public void AddOrder(Order order)
	{
		var book = GetBook(order.Side);
		if (book.TryGetValue(order.Price, out var existingPriceList)) 
		{
			existingPriceList.AddLast(order);   
			_orderLookup.Add(order.Id, (order.Price, existingPriceList.Last!));
			return;
		}
    
		var newPriceList = new LinkedList<Order>();
		newPriceList.AddLast(order); 
		book.Add(order.Price, newPriceList);
		_orderLookup.Add(order.Id, (order.Price, newPriceList.Last!));
	}

	public void RemoveOrder(Guid orderId)
	{
		if (!_orderLookup.TryGetValue(orderId, out var entry))		
			return; 		

		var book = GetBook(entry.node.Value.Side);
		if (book.TryGetValue(entry.price, out var priceList))
		{
			priceList.Remove(entry.node);			
			if (!priceList.Any())				
				book.Remove(entry.price);				
		}

		_orderLookup.Remove(orderId);		
	}

	public void UpdateOrder(Guid orderId, int quantity, decimal? price = null)
	{
		if (quantity <= 0 || (price.HasValue && price.Value <= 0))
			return;

		if (!_orderLookup.TryGetValue(orderId, out var entry))		
			return; 				

		if (!price.HasValue && quantity == entry.node.Value.Quantity) 
			return;

		var newOrder = entry.node.Value with 
		{ 
			Quantity = quantity, 
			Price = price.HasValue ? price.Value : entry.node.Value.Price, 
			Timestamp = DateTime.UtcNow, 
			LastUpdated = DateTime.UtcNow 
		};

		RemoveOrder(orderId);
		AddOrder(newOrder);
	}

	public void FillOrder(Guid orderId, int fillQuantity)
	{
		if (fillQuantity <= 0)
			return;

		if (!_orderLookup.TryGetValue(orderId, out var entry))		
			return;
		
		if (fillQuantity >= entry.node.Value.Quantity)
		{
			RemoveOrder(orderId);
			return;
		}

		entry.node.Value.Quantity -= fillQuantity;
		entry.node.Value.LastUpdated = DateTime.UtcNow;
	}

	public decimal? BestBid => _bids.Count > 0 ? _bids.First().Key : null;
	public decimal? BestAsk => _asks.Count > 0 ? _asks.First().Key : null;
	public LinkedListNode<Order>? FirstBid => _bids.Count > 0 ? _bids.First().Value.First : null;
	public LinkedListNode<Order>? FirstAsk => _asks.Count > 0 ? _asks.First().Value.First : null;
	public bool HasPriceLevel(decimal price, OrderSide side) => GetBook(side).ContainsKey(price);
	public IEnumerable<Order> GetOrdersAtPrice(decimal price, OrderSide side) => GetBook(side).TryGetValue(price, out var priceList) ? priceList : Enumerable.Empty<Order>();
	public int GetNumberOfOrdersAtPrice(decimal price, OrderSide side) => GetOrdersAtPrice(price, side).Count();
	public long GetTotalVolumeAtPrice(decimal price, OrderSide side) => GetOrdersAtPrice(price, side).Sum(s => s.Quantity);
	private SortedDictionary<decimal, LinkedList<Order>> GetBook(OrderSide side) => side == OrderSide.Buy ? _bids : _asks;	
}
