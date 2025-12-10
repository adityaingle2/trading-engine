using TradingEngine.Core.Managers;

namespace TradingEngine.Core.Engine
{
  public class EngineRunner
  {
    private readonly MatchingEngine _matchingEngine;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public EngineRunner()
    {
      _matchingEngine = new MatchingEngine(
        OrderBookManager.Create(),
        ChannelProvider.GetChannel(),
        _cancellationTokenSource.Token); 
    }

    public Task StartAsync()
    {
      // Start the engine loop on a background thread from the thread pool
      return Task.Run(() => _matchingEngine.RunAsync());
    }

    public void Stop()
    {
      _cancellationTokenSource.Cancel();
    }
  }
}