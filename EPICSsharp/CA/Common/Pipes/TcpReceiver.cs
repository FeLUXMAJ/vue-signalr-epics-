//
// TcpReceiver.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Net.Sockets ;
using System.Net ;
using EPICSsharp.CA.Constants ;
using EPICSsharp.CA.Common ;

namespace EPICSsharp.Common.Pipes
{

  internal class TcpReceiver : DataFilter, IDisposable
  {

    private Socket m_socket ;

    private bool m_disposed = false ;

    private byte[] m_buffer = new byte[8192 * 3] ;

    protected IPEndPoint m_destinationEndPoint ;

    static readonly DataPacket m_echo_dataPacket ;

    public IPEndPoint Destination
    {
      get
      {
        return m_destinationEndPoint ;
      }
    }

    static TcpReceiver ( )
    {
      m_echo_dataPacket = DataPacket.Create(16) ;
      m_echo_dataPacket.Command    = (ushort)CommandID.CA_PROTO_ECHO ;
      m_echo_dataPacket.DataType   = 0 ;
      m_echo_dataPacket.DataCount  = 0 ;
      m_echo_dataPacket.Parameter1 = 0 ;
      m_echo_dataPacket.Parameter2 = 0 ;
    }

    protected void Start ( Socket socket )
    {
      m_socket = socket ;
      socket.BeginReceive(
        m_buffer, 
        0, 
        m_buffer.Length, 
        SocketFlags.None, 
        ReceiveTcpData, 
        null
      ) ;
    }

    void ReceiveTcpData ( IAsyncResult asyncResult )
    {
      int n = 0 ;
      try
      {
        if ( m_socket.Connected )
          n = m_socket.EndReceive(asyncResult) ;
        else
        {
          Dispose() ;
          return ;
        }
      }
      catch ( ObjectDisposedException )
      {
        Dispose() ;
        return ;
      }
      catch
      {
        Dispose() ;
        return ;
      }

      // Time to quit!
      if ( n == 0 )
      {
        Dispose() ;
        return ;
      }

      try
      {
        Pipe.LastMessage = DateTime.Now ;
        if ( n > 0 )
        {
          DataPacket p = DataPacket.Create(m_buffer,n) ;
          p.Sender = (IPEndPoint) m_socket.RemoteEndPoint ;
          this.SendData(p) ;
        }
        m_socket.BeginReceive(
          m_buffer, 
          0, 
          m_buffer.Length, 
          SocketFlags.None, 
          ReceiveTcpData, 
          null
        ) ;
      }
      catch ( ObjectDisposedException )
      {
        Dispose() ;
      }
      catch ( SocketException )
      {
        Dispose() ;
      }
      catch ( Exception ex )
      {
        Console.WriteLine(ex.ToString()) ;
        Dispose() ;
      }
    }

    internal void Send ( DataPacket packet )
    {
      if ( m_disposed )
        return ;
      try
      {
        m_socket.Send(packet.Data) ;
        // Pipe.LastMessage = DateTime.Now ;
      }
      catch
      {
        Dispose() ;
      }
    }

    public override void ProcessData ( DataPacket packet )
    {
      throw new NotImplementedException() ;
    }

    public override void Dispose ( )
    {
      if ( m_disposed )
        return ;
      try
      {
        m_socket.Disconnect(false) ;
      }
      catch
      {
      }
      try
      {
        m_socket.Dispose() ;
      }
      catch
      {
      }
      m_disposed = true ;
    }

    internal void Echo ( )
    {
      Pipe.GeneratedEcho = true ;
      Send(m_echo_dataPacket) ;
    }

    public IPEndPoint RemoteEndPoint { get ; set ; }

  }

}
