using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace website.Hubs
{
    public class UserConnectedHub : Hub
    {
        public static int _userConnectionCount = 0;

        public Task SendCountAsync()
        {
            return Clients.All.SendAsync("sendCount", _userConnectionCount);
        }

        public override Task OnConnectedAsync()
        {
            _userConnectionCount += 1;
            SendCountAsync();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            _userConnectionCount -= 1;
            SendCountAsync();
            return base.OnDisconnectedAsync(exception);
        }

        public ChannelReader<int> CountDown(int from){
            var channel = Channel.CreateUnbounded<int>();
            
            _ = WriteToChannel(channel.Writer, from);

            return channel.Reader;

            async Task WriteToChannel(ChannelWriter<int> writer, int k)
            {
                for (int i = k - 1; i >= 0 ; i--)
                {
                    await writer.WriteAsync(i);
                    await Task.Delay(1000);
                }
                writer.Complete();
            }
        }
    }
}