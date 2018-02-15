//
// CAConstants.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Constants
{

  internal enum CAConstants : ushort
  {

    DO_REPLY = 10,

    DONT_REPLY = 5,

    // Minor revision of channel access protocol implemented in this library

    CA_MINOR_PROTOCOL_REVISION = 11

  }

}
