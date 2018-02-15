//
// caserver.cs
//

using EPICSsharp.CA.Server.RecordTypes ;
using EPICSsharp.Common.Pipes ;
using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Linq ;
using System.Net ;
using System.Reflection ;
using System.Text ;
using System.Threading ;
using System.Threading.Tasks ;

namespace EPICSsharp.CA.Server
{

  public class CAServer : IDisposable
  {

    private DataPipe m_udpPipe ;
    
    internal CARecordCollection m_records = new CARecordCollection() ;
    
    internal CARecordCollection Records { get { return m_records ; } }
    
    CaServerListener m_listener ;
    
    bool m_disposed = false ;

    internal List<DataPipe> m_tcpConnections = new List<DataPipe>() ;

    public int TcpPort { get ; private set ; }

    public int UdpPort { get ; private set ; }

    public int BeaconPort { get ; private set ; }

    public CAServer ( IPAddress ipAddress = null, int tcpPort = 5064, int udpPort = 5064, int beaconPort = 0 )
    {
      if ( ipAddress == null )
        ipAddress = IPAddress.Any ;

      if ( beaconPort == 0 )
        beaconPort = udpPort + 1 ;

      this.TcpPort    = tcpPort ;
      this.UdpPort    = udpPort ;
      this.BeaconPort = beaconPort ;

      m_listener = new CaServerListener(
        this, 
        new IPEndPoint(ipAddress, tcpPort)
      ) ;
      m_udpPipe = DataPipe.CreateServerUdp(
        this, 
        ipAddress, 
        udpPort
      ) ;
    }

    public CAType CreateRecord<CAType> ( string name ) where CAType : CARecord
    {
      CAType result = null ;
      try
      {
        result = (CAType) (
          typeof(CAType)
        ).GetConstructor(
          BindingFlags.Public | BindingFlags.Instance, 
          null, 
          new Type[] { }, 
          null
        ).Invoke(
          new object[] { }
        ) ;
      }
      catch ( Exception ex )
      {
        throw ex.InnerException ;
      }
      result.Name = name ;
      m_records.Add(result) ;
      return result ;
    }

    public CAType CreateArrayRecord<CAType> ( string name, int size ) where CAType : CAArrayRecord
    {
      CAType result = null ;
      try
      {
        result = (CAType) (
          typeof(CAType)
        ).GetConstructor(
          BindingFlags.Public | BindingFlags.Instance, 
          null, 
          new Type[] { 
            typeof(int) 
          }, 
          null
        ).Invoke(
          new object[] { size }
        ) ;
      }
      catch ( Exception ex )
      {
        throw ex.InnerException ;
      }
      result.Name = name ;
      m_records.Add(result) ;
      return result ;
    }

    internal void RegisterClient ( DataPipe chain )
    {
      lock ( m_tcpConnections )
      {
        m_tcpConnections.Add(chain) ;
      }
    }

    internal void DisposeClient ( DataPipe chain )
    {
      lock ( m_tcpConnections )
      {
        m_tcpConnections.Remove(chain) ;
      }
    }

    public void Dispose ( )
    {
      if ( m_disposed )
        return ;
      m_disposed = true ;

      List<DataPipe> toDrop ;
      lock (m_tcpConnections)
      {
        toDrop = m_tcpConnections.ToList() ;
      }
      toDrop.ForEach(row => row.Dispose()) ;

      m_listener.Dispose() ;
      m_udpPipe.Dispose() ;
      m_records.Dispose() ;
    }

  }

}
