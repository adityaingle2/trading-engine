namespace TradingEngine.Models;

public class OrderBook
{	
	#region Fields
	// Buy side price levels (sorted descending: highest price first)	
	private readonly SortedDictionary<decimal, LinkedList<Order>> _bids = new(Comparer<decimal>.Create((a, b) => b.CompareTo(a)));	

	// Sell side price levels (sorted ascending: lowest price first)
	private readonly SortedDictionary<decimal, LinkedList<Order>> _asks = new(Comparer<decimal>.Default);	

	// Fast lookup for cancels: OrderID → (price, node reference)
	private readonly Dictionary<Guid, (decimal price, LinkedListNode<Order> node)> _orderIndex = new(); 
	#endregion

	#region Properties
	// The ticker symbol for this order book
	public string Symbol { get; }
	#endregion

	#region  Constructor
	public OrderBook(string symbol)
	{
		Symbol = symbol;
	}
	#endregion

	#region Logic
	public void AddOrder(Order order)
	{
		var book = GetBook(order.Side);
		if (book.TryGetValue(order.Price, out var existingPriceList)) 
		{
			existingPriceList.AddLast(order);   
			_orderIndex.Add(order.Id, (order.Price, existingPriceList.Last!));
			return;
		}
    
		var newPriceList = new LinkedList<Order>();
		newPriceList.AddLast(order); 
		book.Add(order.Price, newPriceList);
		_orderIndex.Add(order.Id, (order.Price, newPriceList.Last!));
	}

	public void RemoveOrder(Guid orderId)
	{
		if (!_orderIndex.TryGetValue(orderId, out var entry))		
			return; 		

		var book = GetBook(entry.node.Value.Side);
		if (book.TryGetValue(entry.price, out var priceList))
		{
			priceList.Remove(entry.node);			
			if (!priceList.Any())				
				book.Remove(entry.price);				
		}

		_orderIndex.Remove(orderId);		
	}

	public void UpdateOrder(Guid orderId, int newQuantity, decimal? newPrice = null)
	{
		if (newQuantity <= 0 || (newPrice.HasValue && newPrice.Value <= 0))
			return; // Invalid parameters

		if (!_orderIndex.TryGetValue(orderId, out var entry))		
			return; // Order not found 					

		if (!newPrice.HasValue && newQuantity == entry.node.Value.Quantity) 
			return; // No changes

		var newOrder = entry.node.Value with 
		{ 
			Quantity = newQuantity, 
			Price = newPrice.HasValue ? newPrice.Value : entry.node.Value.Price, 
			Timestamp = DateTime.UtcNow, 
			LastUpdated = DateTime.UtcNow 
		};

		RemoveOrder(orderId);
		AddOrder(newOrder);
	}

	public void FillOrder(Guid orderId, int fillQuantity)
	{
		if (fillQuantity <= 0)
			return; // Invalid fill quantity

		if (!_orderIndex.TryGetValue(orderId, out var entry))		
			return; // Order not found
		
		if (fillQuantity >= entry.node.Value.Quantity)
		{
			RemoveOrder(orderId);
			return;
		}

		entry.node.Value.Quantity -= fillQuantity;
		entry.node.Value.LastUpdated = DateTime.UtcNow;
	}
	#endregion

	#region Helpers
	public decimal? BestBid => _bids.Count > 0 ? _bids.First().Key : null;
	public decimal? BestAsk => _asks.Count > 0 ? _asks.First().Key : null;
	public LinkedListNode<Order>? FirstBid => _bids.Count > 0 ? _bids.First().Value.First : null;
	public LinkedListNode<Order>? FirstAsk => _asks.Count > 0 ? _asks.First().Value.First : null;
	public bool HasPriceLevel(decimal price, OrderSide side) => GetBook(side).ContainsKey(price);
	public IEnumerable<Order> GetOrdersAtPrice(decimal price, OrderSide side) => GetBook(side).TryGetValue(price, out var priceList) ? priceList : Enumerable.Empty<Order>();
	public int GetNumberOfOrdersAtPrice(decimal price, OrderSide side) => GetOrdersAtPrice(price, side).Count();
	public long GetTotalVolumeAtPrice(decimal price, OrderSide side) => GetOrdersAtPrice(price, side).Sum(s => s.Quantity);
	private SortedDictionary<decimal, LinkedList<Order>> GetBook(OrderSide side) => side == OrderSide.Buy ? _bids : _asks;	
	#endregion
}
