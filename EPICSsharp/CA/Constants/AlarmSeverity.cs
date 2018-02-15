//
// AlarmSeverity.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Constants
{

  // Defines the severity of the current alarm.

  public enum AlarmSeverity : ushort
  {

    // There is no alarm. The value is in the normal range,
    // ie between LowWarnLimit and HighWarnLimit.

    NO_ALARM = 0,

    // The alarm is minor. The value is in the warning range, 
    // either between LowWarnLimit and LowAlarmLimit or between
    // HighWarnLimit and HighAlarmLimit.

    MINOR = 1,

    // The alarm is major. The value is either lower than the
    // LowAlarmLimit or higher than the HighAlarmLimit.

    MAJOR = 2,

    // Invalid severity.

    INVALID = 3

  }

}
