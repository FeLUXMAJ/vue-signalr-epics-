//
// EpicsEvent.cs
//

using System ;

namespace EPICSsharp.CA.Server
{

  internal class EpicsEvent
  {

    public EventHandler Handler { get ; set ; }

    public RecordTypes.CARecord Record { get ; set ; }

    public int DataCount { get ; set ; }

    public Constants.EpicsType EpicsType { get ; set ; }

    public uint SID { get ; set ; }

  }

}
