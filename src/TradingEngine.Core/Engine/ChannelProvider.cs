using System.Threading.Channels;
using TradingEngine.Core.Messaging;

namespace TradingEngine.Core.Engine;

public static class ChannelProvider
{
  private static readonly Channel<EngineMessage> _channel =
    Channel.CreateUnbounded<EngineMessage>(new UnboundedChannelOptions
    {
      SingleReader = true,
      SingleWriter = true  
    });

  public static Channel<EngineMessage> GetChannel() => _channel;
}