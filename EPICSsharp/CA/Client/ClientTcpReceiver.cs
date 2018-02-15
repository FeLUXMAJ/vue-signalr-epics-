//
// ClientTcpReceiver.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Net.Sockets ;
using System.Net ;
using EPICSsharp.CA.Constants ;
using EPICSsharp.CA.Common ;
using EPICSsharp.Common.Pipes ;

namespace EPICSsharp.CA.Client
{

  internal class ClientTcpReceiver : TcpReceiver, IDisposable
  {

    // Hmm, these fields are also defined in the BASE CLASS ??? !!!

    private Socket m_socket ;

    private bool m_disposed = false ;

    private byte[] m_buffer = new byte[8192 * 3] ;

    private static readonly DataPacket m_echo_dataPacket ;

    internal Dictionary<string, uint> ChannelSID = new Dictionary<string, uint>() ;

    internal Dictionary<uint, Channel> PendingIo = new Dictionary<uint, Channel>() ;

    internal List<Channel> ConnectedChannels = new List<Channel>() ;

    public CAClient Client ;

    static ClientTcpReceiver ( )
    {
      m_echo_dataPacket = DataPacket.Create(16) ;
      m_echo_dataPacket.Command    = (ushort) CommandID.CA_PROTO_ECHO ;
      m_echo_dataPacket.DataType   = 0 ;
      m_echo_dataPacket.DataCount  = 0 ;
      m_echo_dataPacket.Parameter1 = 0 ;
      m_echo_dataPacket.Parameter2 = 0 ;
    }

    public void Init ( CAClient client, IPEndPoint dest )
    {
      this.Client = client ;
      this.m_destinationEndPoint = dest ;
      m_socket = new Socket(
        AddressFamily.InterNetwork, 
        SocketType.Stream, 
        ProtocolType.Tcp
      ) ;
      m_socket.SetSocketOption(
        SocketOptionLevel.Tcp, 
        SocketOptionName.NoDelay, 
        true
      ) ;

      m_socket.Connect(dest) ;
      base.Start(m_socket) ;

      DataPacket p = DataPacket.Create(16) ;
      p.Command    = (ushort) CommandID.CA_PROTO_VERSION ;
      p.DataType   = 1 ;
      p.DataCount  = (uint) CAConstants.CA_MINOR_PROTOCOL_REVISION ;
      p.Parameter1 = 0 ;
      p.Parameter2 = 0 ;
      Send(p) ;

      p = DataPacket.Create(
        16 
      + this.Client.Configuration.Hostname.Length 
      + TypeHandling.Padding(
          this.Client.Configuration.Hostname.Length
        )
      ) ;
      p.Command = (ushort) CommandID.CA_PROTO_HOST_NAME ;
      p.DataCount  = 0 ;
      p.DataType   = 0 ;
      p.Parameter1 = 0 ;
      p.Parameter2 = 0 ;
      p.SetDataAsString(
        this.Client.Configuration.Hostname
      ) ;
      Send(p) ;

      p = DataPacket.Create(
        16 
      + this.Client.Configuration.Username.Length 
      + TypeHandling.Padding(
          this.Client.Configuration.Username.Length
        )
      ) ;
      p.Command    = (ushort) CommandID.CA_PROTO_CLIENT_NAME ;
      p.DataCount  = 0 ;
      p.DataType   = 0 ;
      p.Parameter1 = 0 ;
      p.Parameter2 = 0 ;
      p.SetDataAsString(
        this.Client.Configuration.Username
      ) ;
      Send(p) ;
    }

    internal void AddChannel ( Channel channel )
    {
      lock ( ConnectedChannels )
      {
        if ( ! ConnectedChannels.Contains(channel) )
          ConnectedChannels.Add(channel) ;
      }
    }

    internal void RemoveChannel ( Channel channel )
    {
      lock ( ConnectedChannels )
      {
        ConnectedChannels.Remove(channel) ;
      }

      lock ( Client.Channels )
      {
        if ( 
          ! Client.Channels.Any(
            row => row.Value.ChannelName == channel.ChannelName
          )
        ) {
          ChannelSID.Remove(channel.ChannelName) ;
        }
      }
    }

    public override void Dispose ( )
    {
      if ( m_disposed )
        return ;
      lock ( Client.Iocs )
        Client.Iocs.Remove(m_destinationEndPoint) ;
      List<Channel> toDisconnect ;
      lock ( ConnectedChannels )
      {
        toDisconnect = ConnectedChannels.ToList() ;
      }
      foreach ( Channel channel in toDisconnect )
        channel.Disconnect() ;

      base.Dispose() ;
    }

    internal new void Echo ( )
    {
      Pipe.GeneratedEcho = true ;
      Send(m_echo_dataPacket) ;
    }

  }

}
