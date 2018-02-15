//
// DataFilter.cs
//

using EPICSsharp.CA.Common ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.Common.Pipes
{

  public delegate void ReceiveDataDelegate ( DataPacket packet ) ;

  abstract class DataFilter : IDisposable
  {

    public event ReceiveDataDelegate ReceiveData ;

    public DataPipe Pipe ;

    public abstract void ProcessData(DataPacket packet) ;

    // Sends the DataPacket further in the chain
    // <param name="packet">

    public void SendData ( DataPacket packet )
    {
      if ( ReceiveData != null )
        ReceiveData(packet) ;
    }

    public virtual void Dispose ( )
    {
    }

  }

}
