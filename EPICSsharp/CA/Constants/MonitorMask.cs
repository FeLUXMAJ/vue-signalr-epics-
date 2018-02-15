//
// MonitorMask.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Constants
{

  // Monitor Mask allows to define what a Monitor shall monitor

  public enum MonitorMask : ushort
  {

    // Value type
    VALUE = 0x01,

    // Log type
    LOG = 0x02,

    // Value and log together
    VALUE_LOG = MonitorMask.VALUE | MonitorMask.LOG,

    // Alarm status type
    ALARM = 0x04,

    // Value and alarm together
    VALUE_ALARM = MonitorMask.VALUE | MonitorMask.ALARM,

    // Log and alarm together
    LOG_ALARM = MonitorMask.LOG | MonitorMask.ALARM,

    // All three (value, log and alarm) together
    ALL = MonitorMask.VALUE | MonitorMask.LOG | MonitorMask.ALARM

  }

}
