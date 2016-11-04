using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;

public interface IMessaging 
{
    void Publish(ChannelEvent channelEvent);
}

[HubName("messaging")]
public class MessagingHub : Hub
{
    public async Task Subscribe(string channel)
    {
        await Groups.Add(Context.ConnectionId, channel);

        var ev = new ChannelEvent
        {
            ChannelName = Constants.AdminChannel,
            Name = "user.subscribed",
            Data = new
            {
                Context.ConnectionId,
                ChannelName = channel
            }
        };

        Publish(ev);
    }

    public async Task Unsubscribe(string channel)
    {
        await Groups.Remove(Context.ConnectionId, channel);

        var ev = new ChannelEvent
        {
            ChannelName = Constants.AdminChannel,
            Name = "user.unsubscribed",
            Data = new
            {
                Context.ConnectionId,
                ChannelName = channel
            }
        };

        Publish(ev);
    }


    public void Publish(ChannelEvent channelEvent)
    {
        Clients.Group(channelEvent.ChannelName).OnEvent(channelEvent.ChannelName, channelEvent);
    }

    public override Task OnConnected()
    {
        var ev = new ChannelEvent
        {
            ChannelName = Constants.AdminChannel,
            Name = "user.connected",
            Data = new
            {
                Context.ConnectionId,
            }
        };

        Publish(ev);

        return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
        var ev = new ChannelEvent
        {
            ChannelName = Constants.AdminChannel,
            Name = "user.disconnected",
            Data = new
            {
                Context.ConnectionId,
            }
        };

        Publish(ev);

        return base.OnDisconnected(stopCalled);
    }
}