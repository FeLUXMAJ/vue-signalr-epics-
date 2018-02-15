//
// CAClient.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Net ;
using System.Threading ;
using EPICSsharp.Common.Pipes ;
using EPICSsharp.CA.Constants ;

namespace EPICSsharp.CA.Client
{

  public class CAClient : IDisposable
  {

    private readonly CAConfiguration configuration = new CAConfiguration() ;

    // Allows configuring the channel access client.

    public CAConfiguration Configuration { get { return configuration ; } }

    internal DataPipe Udp ;

    internal Dictionary<uint, Channel> Channels = new Dictionary<uint, Channel>() ;

    internal Dictionary<IPEndPoint, DataPipe> Iocs = new Dictionary<IPEndPoint, DataPipe>() ;

    private bool disposed = false ;

    internal Searcher Searcher ;

    readonly Thread echoThread ;

    // Creates a new epics client

    public CAClient ( )
    {
      Udp = DataPipe.CreateClientUdp(this) ;
      Searcher = new Searcher(this) ;
      echoThread = new Thread(Echoer){
        IsBackground = true
      } ;
      echoThread.Start() ;
    }

    private void Echoer ( )
    {
      while ( !disposed )
      {
        Thread.Sleep(10000) ;
        List<TcpReceiver> toEcho ;
        lock (Iocs)
        {
          DateTime now = DateTime.Now ;
          toEcho = Iocs.Values.Where(
            row => (row.LastMessage - now).TotalSeconds > 20
          ).Select(
            row => (TcpReceiver) row[0]
          ).ToList() ;
          toEcho = Iocs.Select(
            row => (TcpReceiver) row.Value[0]
          ).ToList() ;
        }
        foreach ( var i in toEcho )
        {
          if( i.Pipe.GeneratedEcho == true )
            i.Dispose() ;
          else
            i.Echo() ;
        }
      }
    }

    // Creates a new Channel based on the channel name.
    // <param name="channelName">Name of the channel.
    // <returns></returns>

    public Channel CreateChannel ( string channelName )
    {
      if ( disposed )
        throw new ObjectDisposedException(this.GetType().Name) ;
      Channel channel = new Channel(
        this, 
        channelName
      ) ;
      lock (Channels)
      {
        Channels.Add(
          channel.CID, 
          channel
        ) ;
      }
      return channel ;
    }

    // Creates a new Generic Channel of the given type.

    public Channel<TChannelType> CreateChannel<TChannelType> ( string channelName )
    {
      if ( disposed )
        throw new ObjectDisposedException(this.GetType().Name) ;
      Channel<TChannelType> channel = new Channel<TChannelType>(this,channelName) ;
      lock (Channels)
      {
        Channels.Add(channel.CID,channel) ;
      }
      return channel ;
    }

    CountdownEvent multiActionCountDown ;

    // Connects to the channel (search, and create the virtual circuit).
    // The function blocks till all the channels specified are connected or until the timeout expires.
    // If a channel is already connected it will not block.

    public void MultiConnect ( IEnumerable<Channel> channelsToConnect )
    {
      if ( disposed )
        throw new ObjectDisposedException(this.GetType().Name) ;
      var list = channelsToConnect.Where(row => row.Status != ChannelStatus.CONNECTED) ;
      using (
        multiActionCountDown = new CountdownEvent(
          list.Count()
        )
      ) {
        foreach ( Channel c in list )
        {
          c.HasValue = false;
          c.AfterConnect(
            e => multiActionCountDown.Signal()
          ) ;
        }
        multiActionCountDown.Wait(
          Configuration.WaitTimeout
        ) ;
      }
    }

    // Gets all the channels in paralell and returns the values as a list of objects.
    // If the value is null it means the channel didn't gave back the value in time.
    // <typeparam name="TType"></typeparam>
    // <param name="channels">
    // <returns></returns>

    public object[] MultiGet<TType> ( IEnumerable<Channel> channels )
    {
      if ( disposed )
        throw new ObjectDisposedException(this.GetType().Name) ;
      if ( channels != null )
      {
        using (
          multiActionCountDown = new CountdownEvent(
            channels.Count()
          )
        ) {
          foreach ( Channel channel in channels )
          {
            channel.HasValue = false;
            channel.AfterReadNotify += MultiGetAfterReadNotify;
            if ( channel.Status == ChannelStatus.CONNECTED )
              channel.SendReadNotify<TType>() ;
            else
              channel.AfterConnect(
                e => e.SendReadNotify<TType>()
              ) ;
          }
          multiActionCountDown.Wait(
            Configuration.WaitTimeout
          ) ;
        }
        return channels.Select(
          row => 
          row.HasValue 
          ? (object) row.DecodeData<TType>(1) 
          : null
        ).ToArray() ;
      }
      return new object[]{} ;
    }

    void MultiGetAfterReadNotify ( Channel sender, object newValue )
    {
      sender.AfterReadNotify -= MultiGetAfterReadNotify ;
      try
      {
        multiActionCountDown.Signal() ;
      }
      catch
      {
      }
    }

    internal Channel GetChannelByCid ( uint cid )
    {
      lock (Channels)
      {
        if ( ! Channels.ContainsKey(cid) )
          return null ;
        return Channels[cid] ;
      }
    }

    internal DataPipe GetIocConnection ( IPEndPoint iPEndPoint )
    {
      DataPipe ioc ;
      lock ( Iocs )
      {
        if ( !Iocs.ContainsKey(iPEndPoint) )
        {
          ioc = DataPipe.CreateClientTcp(this,iPEndPoint) ;
          Iocs.Add(iPEndPoint,ioc) ;
        }
        else
          ioc = Iocs[iPEndPoint] ;
      }
      return ioc ;
    }

    internal void RemoveIocConnection ( DataPipe ioc )
    {
      lock (Iocs)
      {
        try
        {
          IPEndPoint ip = Iocs.Where(
            row => row.Value == ioc
          ).Select(
            row => row.Key
          ).First() ;
          Iocs.Remove(ip) ;
        }
        catch
        {
        }
      }
    }

    // Close all channels, and disconnect from the IOCs.

    public void Dispose ( )
    {
      if ( disposed )
        return ;
      disposed = true ;
      Udp.Dispose() ;
      Searcher.Dispose() ;

      List<Channel> channelsList ;

      lock ( Channels )
      {
        channelsList = Channels.Values.ToList() ;
      }
      foreach ( var channel in channelsList )
        channel.Dispose() ;

      List<DataPipe> dataPipesList ;
      lock (Iocs)
      {
        dataPipesList = Iocs.Values.ToList() ;
      }
      foreach (var dataPipe in dataPipesList )
        dataPipe.Dispose() ;
    }

  }

}
