using System.Threading.Channels;
using TradingEngine.Core.Managers;
using TradingEngine.Core.Messaging;

namespace TradingEngine.Core.Engine;

public class MatchingEngine
{
    private readonly Channel<EngineMessage> _inputChannel;
    private readonly OrderBookManager _orderBookManager;
    private readonly CancellationToken _cancellationToken;

    public MatchingEngine(OrderBookManager manager, Channel<EngineMessage> channel, CancellationToken cancellationToken)
    {
      _orderBookManager = manager;
      _inputChannel = channel;
      _cancellationToken = cancellationToken;
    }

    public async Task RunAsync()
    {
        await foreach (var msg in _inputChannel.Reader.ReadAllAsync(_cancellationToken))
        {
            switch (msg)
            {
                case NewOrderMessage newOrder:
                    //_orderBookManager.ProcessNewOrder(newOrder);
                    break;
                case CancelOrderMessage cancelOrder:
                    //_orderBookManager.ProcessCancelOrder(cancelOrder);
                    break;
                case UpdateOrderMessage updateOrder:
                    //_orderBookManager.ProcessUpdateOrder(updateOrder);
                    break;
                default:
                    throw new InvalidOperationException("Unknown message type");
            }
        }
    }
}