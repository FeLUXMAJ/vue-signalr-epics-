//
// ChannelStatus.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Constants
{

  // Current connection status of a channel

  public enum ChannelStatus
  {
    // Channel was just created and is trying to be established.

    REQUESTED = 0,

    // Channel is connected to an IOC and able to work.

    CONNECTED = 1,

    // Channel was connected, lost connection, and is not working now. 
    // But will try to reconnect automatically

    DISCONNECTED = 2,

    DISPOSED = 3,

  }

}
