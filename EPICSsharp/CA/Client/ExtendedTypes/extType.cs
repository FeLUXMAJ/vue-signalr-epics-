//
// extType.cs
//

using EPICSsharp.CA.Constants ;
using System ;

namespace EPICSsharp.CA.Client
{

  // Extended epics type serves severity, status and value

  public class ExtType<TType> : Decodable
  {

    internal ExtType ( )
    { }

    // Severity of the channel serving this value

    public AlarmSeverity Severity { get ; internal set ; }
    // AlarmStatus of the channel serving this value

    public AlarmStatus Status { get ; internal set ; }

    // current value, type transformation made by epics not c#

    public TType Value { get ; set ; }

    internal override void Decode ( Channel channel, uint nbElements )
    {
        Status   = (AlarmStatus)   channel.DecodeData<ushort>(1, 0) ;
        Severity = (AlarmSeverity) channel.DecodeData<ushort>(1, 2) ;
        int pos = 4 ;
        Type t = typeof(TType) ;
        if ( t.IsArray )
            t = t.GetElementType() ;
        if ( t == typeof(object) )
            t = channel.ChannelDefinedType ;
        // padding for "RISC alignment"
        if ( t == typeof(double) )
            pos += 4 ;
        else if ( t == typeof(byte) )
            pos++ ;
        Value = channel.DecodeData<TType>(nbElements, pos) ;
    }

    public override string ToString ( )
    {
      return String.Format(
        "Value:{0},Status:{1},Severity:{2}", 
        Value, Status, Severity
      ) ;
    }

  }

}
