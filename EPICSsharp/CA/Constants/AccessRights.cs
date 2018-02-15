//
// AccessRights.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Constants
{

  // Access rights to a channel.

  public enum AccessRights : ushort 
  {

      // You can neither read from nor write to the channel.

      NoAccess = 0,

      // You can read from, but not write to the channel.

      ReadOnly = 1,

      // You can write to, but not read from the channel.

      WriteOnly = 2,

      // You can both, read from and write to the channel.

      ReadAndWrite = 3

  }

}
