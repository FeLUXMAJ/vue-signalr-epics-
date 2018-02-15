//
// GenericChannel.cs
//

using EPICSsharp.CA.Common ;
using EPICSsharp.CA.Constants ;
using System ;
using System.ComponentModel ;
using System.Linq ;
using System.Threading.Tasks ;

namespace EPICSsharp.CA.Client
{

  public delegate void ChannelValueDelegate<TType> ( Channel<TType> sender, TType newValue ) ;

  public class Channel<TType> : Channel
  {

    internal Channel ( CAClient client, string channelName ) : 
    base(client,channelName)
    {
      MonitoredType = typeof(TType) ;
    }

    private event ChannelValueDelegate<TType> PrivMonitorChanged ;

    public TType Get ( )
    {
      return base.Get<TType>() ;
    }

    public async Task<TType> GetAsync ( )
    {
      return await base.GetAsync<TType>() ;
    }

    public void Put ( TType newValue )
    {
      base.Put<TType>(newValue) ;
    }

    public async Task PutAsync ( TType newValue )
    {
      await base.PutAsync<TType>(newValue) ;
    }

    public void PutNoWait ( TType newValue )
    {
      base.PutNoWait<TType>(newValue) ;
    }

    //
    // Event-Monitor which calls as soon a change on the channel happened which fits into the defined
    // Monitormask (channel.MonitorMask). The properties 'channel.MonitorMask' and 'channel.MonitorDataCount'
    // do touch the behavior of this event and can't be changed when a monitor is already connected.
    // 
    //   CAClient client = new CAClient();
    //   Channel channel = client.CreateChannel("SEILER_C:CPU") ;
    //   channel.MonitorMask = MonitorMask.VALUE ;
    //   channel.MonitorDataCount = 1 ;
    //   channel.MonitorChanged += new ChannelValueDelegate(channel_MonitorChanged) ;
    //

    [Browsable(true),EditorBrowsable(EditorBrowsableState.Always)]
    public new event ChannelValueDelegate<TType> MonitorChanged
    {
      add
      {
        if ( PrivMonitorChanged == null )
        {
          AfterConnect(SendMonitor) ;
        }
        else if ( RawData != null )
        {
          value(
            this, 
            DecodeData<TType>(MonitoredElements)
          ) ;
        }
        PrivMonitorChanged += value ;
      }
      remove
      {
        PrivMonitorChanged -= value ;
        if ( PrivMonitorChanged == null )
        {
          if ( ChannelDataCount != 0 )
          {
            DataPacket p = DataPacket.Create(16) ;
            p.Command    = (ushort) CommandID.CA_PROTO_EVENT_CANCEL ;
            p.DataType   = (ushort) TypeHandling.Lookup[typeof(TType)] ;
            p.DataCount  = ChannelDataCount ;
            p.Parameter1 = SID ;
            p.Parameter2 = CID ;
            if ( ioc != null )
              ioc.Send(p) ;
          }
        }
      }
    }

    internal override void Disconnect ( )
    {
      if ( Disposed )
        return ;
      if ( ioc != null )
        ioc.RemoveChannel(this) ;
      lock ( ConnectionLock )
      {
        Status = ChannelStatus.DISCONNECTED ;
        ioc = null ;
        SID = 0 ;

        if ( PrivMonitorChanged != null )
        {
          AfterConnect(SendMonitor) ;
        }
      }
    }

    void SendMonitor ( Channel action )
    {
      if ( ChannelDataCount == 0 )
        return ;

      // Console.WriteLine("Sending new event add") ;
      DataPacket p = DataPacket.Create(16 + 16) ;
      p.Command = (ushort) CommandID.CA_PROTO_EVENT_ADD ;
      Type t = typeof(TType) ;
      if ( t.IsArray )
        t = t.GetElementType() ;
      else if ( t.IsGenericType )
      {
        if ( t.GetGenericArguments().First() == typeof(object) )
          t = t.GetGenericTypeDefinition().MakeGenericType(
            new Type[] { 
              channelDefinedType 
            }
          ) ;
      }
      p.DataType   = (ushort) TypeHandling.Lookup[t] ;
      p.DataCount  = ChannelDataCount ;
      p.Parameter1 = SID ;
      p.Parameter2 = CID ;

      p.SetUInt16(
        12 + 16, 
        (ushort) MonitorMask
      ) ;

      if ( ioc != null )
        ioc.Send(p) ;
    }

    internal override void UpdateMonitor ( DataPacket packet )
    {
      if ( Client.Configuration.DebugTiming )
      {
        lock ( ElapsedTimings )
        {
          if ( ! ElapsedTimings.ContainsKey("MonitorUpdate") )
            ElapsedTimings.Add(
              "MonitorUpdate", 
              Stopwatch.Elapsed
            ) ;
        }
      }
      RawData = packet ;
      if ( PrivMonitorChanged != null )
      {
        PrivMonitorChanged(
          this, 
          DecodeData<TType>(MonitoredElements)
        ) ;
      }
      else
        base.UpdateMonitor(packet) ;
    }

  }

}
