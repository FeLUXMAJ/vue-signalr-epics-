//
// UdpReceiver.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Net.Sockets ;
using System.Net ;
using EPICSsharp.CA.Common ;

namespace EPICSsharp.Common.Pipes
{

  internal class UdpReceiver : DataFilter
  {

    private UdpClient m_udpClient ;

    private byte[] m_buff = new byte[8192 * 3] ;

    private bool m_disposed = false ;

    private IPAddress m_address = null ;

    private int m_udpPortNumber = 0 ;

    private const int SIO_UDP_CONNRESET = -1744830452 ;

    public UdpReceiver ( ) : 
    this(null,0)
    { }

    public UdpReceiver ( IPAddress address, int port )
    {
      m_udpPortNumber = port ;
      m_address       = address ;
      InitUdp(m_address,m_udpPortNumber) ;
    }

    void InitUdp ( IPAddress address = null, int port = 0 )
    {
      if ( address == null )
        m_udpClient = new UdpClient(port) ;
      else
        m_udpClient = new UdpClient(
          new IPEndPoint(address,port)
        ) ;
      try
      {
        m_udpClient.Client.IOControl(
          SIO_UDP_CONNRESET, 
          new byte[] { 0, 0, 0, 0 }, 
          null
        ) ;
      }
      catch
      {
      }
      m_udpClient.BeginReceive(
        GotUdpMessage, 
        null
      ) ;
    }

    public void Send ( DataPacket packet )
    {
      m_udpClient.Send(
        packet.Data, 
        packet.Data.Length, 
        packet.Destination
      ) ;
    }

    public void Send ( IPEndPoint destination, byte[] buff )
    {
      if ( m_disposed )
        return ;
      try
      {
        m_udpClient.Send(
          buff, 
          buff.Length, 
          destination
        ) ;
      }
      catch ( Exception ex )
      {
        Console.WriteLine(
          ex.ToString()
        ) ;
      }
    }

    void GotUdpMessage ( IAsyncResult asyncResult )
    {
      Pipe.LastMessage = DateTime.Now ;
      IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any,0) ;
      byte[] buff ;

      try
      {
        buff = m_udpClient.EndReceive(
          asyncResult, 
          ref ipeSender
        ) ;
      }
      catch ( ObjectDisposedException )
      {
        (
          (PacketSplitter) this.Pipe[1]
        ).Reset() ;
        // Stop receiving
        return ;
      }
      catch ( Exception )
      {
        try
        {
          m_udpClient.BeginReceive(GotUdpMessage, null) ;
        }
        catch
        {
          (
            (PacketSplitter) this.Pipe[1]
          ).Reset() ;
          try
          {
            m_udpClient.Close() ;
          }
          catch
          {
          }
          InitUdp(this.m_address,m_udpPortNumber) ;
          // udp = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0)) ;
          m_udpClient.BeginReceive(GotUdpMessage, null) ;
        }
        return ;
      }

      // Get the data back

      DataPacket packet = DataPacket.Create(buff) ;
      packet.Sender = (IPEndPoint) ipeSender ;

      // Start Accepting again
      // udp.BeginReceive(GotUdpMessage, null) ;
      try
      {
        m_udpClient.BeginReceive(
          GotUdpMessage, 
          null
        ) ;
      }
      catch
      {
        (
          (PacketSplitter) this.Pipe[1]
        ).Reset() ;
        try
        {
          m_udpClient.Close() ;
        }
        catch
        {
        }
        InitUdp(
          this.m_address, 
          m_udpPortNumber
        ) ;
        m_udpClient.BeginReceive(
          GotUdpMessage, 
          null
        ) ;
      }

      SendData(packet) ;
    }

    public override void Dispose ( )
    {
      if ( m_disposed )
        return ;

      m_disposed = true ;
      m_udpClient.Close() ;

    }

    public override void ProcessData ( DataPacket packet )
    {
      throw new NotImplementedException() ;
    }

  }

}
