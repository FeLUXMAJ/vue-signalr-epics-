//
// extTimeType.cs
//

using System ;
using EPICSsharp.CA.Constants ;

namespace EPICSsharp.CA.Client
{

  // Extended time epics type serves severity, status, value and time of last change.

  public class ExtTimeType<TType> : ExtType<TType>
  {

    internal ExtTimeType ( )
    { }

    // Time of the last change on channel as local datetime
    public DateTime Time { get ; private set ; }

    internal override void Decode ( Channel channel, uint nbElements )
    {
      Status   = (AlarmStatus) channel.DecodeData<ushort>(1, 0) ;
      Severity = (AlarmSeverity) channel.DecodeData<ushort>(1, 2) ;
      Time     = channel.DecodeData<DateTime>(1, 4) ;
      int pos = 12 ;
      Type t = typeof(TType) ;
      if ( t.IsArray )
          t = t.GetElementType() ;
      if ( t == typeof(object) )
          t = channel.ChannelDefinedType ;
      // padding for "RISC alignment"
      if ( t == typeof(byte) )
        pos += 3 ;
      else if ( t == typeof(double) )
        pos += 4 ;
      else if ( t == typeof(short) )
        pos += 2 ;
      Value = channel.DecodeData<TType>(nbElements,pos) ;
    }

    public override string ToString( )
    {
      return String.Format(
        "Value:{0},Status:{1},Severity:{2},Time:{3}", 
        Value, Status, Severity, Time.ToString()
      ) ;
    }

  }

}
