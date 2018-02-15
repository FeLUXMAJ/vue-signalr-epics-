//
// CAByteArrayRecord.cs
//

namespace EPICSsharp.CA.Server.RecordTypes
{

  public class CAByteArrayRecord  : CAArrayRecord<byte>
  {

    public CAByteArrayRecord ( int size ) : 
    base(size)
    { }

  }

}
