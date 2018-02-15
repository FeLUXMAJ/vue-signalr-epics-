//
// DataPipe.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Reflection ;
using EPICSsharp.CA.Client ;
using EPICSsharp.CA.Server ;
using System.Net.Sockets ;
using System.Net ;

namespace EPICSsharp.Common.Pipes
{

  internal class DataPipe : IDisposable
  {

    private List<DataFilter> Filters = new List<DataFilter>() ;

    public DataFilter FirstFilter ;

    public DataFilter LastFilter ;

    public DateTime LastMessage = DateTime.Now ;

    // Adds the next Worker to the chain and register it to the previous Worker to the ReceiveData event.

    private void Add ( DataFilter filter )
    {
      Filters.Add(filter) ;
      if ( FirstFilter == null )
        FirstFilter = filter ;
      else
        LastFilter.ReceiveData += new ReceiveDataDelegate(filter.ProcessData) ;
      LastFilter = filter ;
    }

    private DataPipe ( )
    {
      GeneratedEcho = false ;
    }

    public DataFilter this[int key]
    {
      get
      {
        return Filters[key] ;
      }
    }

    internal static DataPipe CreateServerUdp ( CAServer server, IPAddress address, int udpPort )
    {
      DataPipe dataPipe = new DataPipe() ;
      dataPipe.Add(
        new UdpReceiver(
          address, 
          udpPort
        )
      ) ;
      dataPipe[0].Pipe = dataPipe ;
      AddToPipe(
        new Type[] { 
          typeof(PacketSplitter), 
          typeof(ServerHandleMessage) 
        }, 
        dataPipe
      ) ;
      (
        (ServerHandleMessage) dataPipe.LastFilter
      ).Server = server ;
      return dataPipe ;
    }

    internal static DataPipe CreateClientUdp ( CAClient client )
    {
      DataPipe dataPipe = PopulatePipe(
        new Type[] { 
          typeof(UdpReceiver), 
          typeof(PacketSplitter), 
          typeof(ClientHandleMessage) 
        }
      ) ;
      (
        (ClientHandleMessage) dataPipe.LastFilter
      ).Client = client ;
      return dataPipe ;
    }

    internal static DataPipe CreateClientTcp ( CAClient client, System.Net.IPEndPoint iPEndPoint )
    {
      DataPipe res = PopulatePipe(
        new Type[] { 
          typeof(ClientTcpReceiver), 
          typeof(PacketSplitter), 
          typeof(ClientHandleMessage) 
        }
      ) ;
      (
        (ClientTcpReceiver) res[0]
      ).Init(
        client, 
        iPEndPoint
      ) ;
      (
        (ClientHandleMessage) res.LastFilter
      ).Client = client ;
      return res ;
    }

    internal static DataPipe CreateServerTcp ( CAServer server, Socket client )
    {
      DataPipe res = PopulatePipe(
        new Type[] { 
          typeof(ServerTcpReceiver), 
          typeof(PacketSplitter), 
          typeof(ServerHandleMessage) 
        }
      ) ;
      // ((TcpReceiver)res[0]).Start(iPEndPoint) ;
      (
        (ServerHandleMessage) res.LastFilter
      ).Server = server ;
      (
        (ServerTcpReceiver) res.FirstFilter
      ).Init(client) ;
      return res ;
    }

    static DataPipe PopulatePipe ( Type[] types )
    {
      DataPipe pipe = new DataPipe() ;
      AddToPipe(types,pipe) ;
      return pipe ;
    }

    static void AddToPipe ( Type[] types, DataPipe pipe )
    {
      foreach ( Type t in types )
      {
        DataFilter dataFilter = (DataFilter) t.GetConstructor(
          new Type[]{}
        ).Invoke(
          new object[]{}
        ) ;
        dataFilter.Pipe = pipe ;
        pipe.Add(dataFilter) ;
      }

    }

    public void Dispose ( )
    {
      foreach ( var filter in this.Filters )
        filter.Dispose() ;
    }

    internal bool GeneratedEcho { get ; set ; }

  }

}
