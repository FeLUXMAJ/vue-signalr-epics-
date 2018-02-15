//
// extControlEnum.cs
//

using EPICSsharp.CA.Constants ;

namespace EPICSsharp.CA.Client
{

  public class ExtControlEnum : ExtType<int>
  {

    internal ExtControlEnum ( )
    { }

    internal override void Decode ( Channel channel, uint nbElements )
    {
      Status   = (AlarmStatus) channel.DecodeData<ushort>(1, 0) ;
      Severity = (AlarmSeverity)channel.DecodeData<ushort>(1, 2) ;
      NbStates = channel.DecodeData<ushort>(1, 4) ;
      States   = new string[NbStates] ;
      Value    = channel.DecodeData<ushort>(1, 422) ;
      for ( int i = 0 ; i < NbStates ; i++ )
      {
        States[i] = channel.DecodeData<string>(
          1, 
          6 + i * 26, 
          26
        ) ;
      }
    }

    public ushort NbStates { get ; set ; }

    public string[] States { get ; set ; }

  }

}
