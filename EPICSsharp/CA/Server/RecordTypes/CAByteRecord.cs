//
// CAByteRecord.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Server.RecordTypes
{

  // A byte record which handles the limits of the value and set the alarm accordingly.

  public class CAByteRecord : CAValueRecord<byte>
  {
  }

}
