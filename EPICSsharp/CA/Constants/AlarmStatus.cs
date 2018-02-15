//
// AlarmStatus.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Constants
{

  // Informs about the status of the device behind this Channel

  public enum AlarmStatus : ushort
  {

    // Device is working properly correctly

    NO_ALARM = 0,

    READ = 1,

    WRITE = 2,

    // Device is malfunctioning, and hit the upper Alarm Limit

    HIHI = 3,

    // Device is missbehaving, and hit the upper Warn Limit

    HIGH = 4,

    // Device is malfunctioning, and hit the lower Alarm Limit

    LOLO = 5,

    // Device is missbehaving, and hit the lower Warn Limit

    LOW = 6,

    STATE = 7,

    COS = 8,

    COMM = 9,

    TIMEOUT = 10,

    HARDWARE_LIMIT = 11,

    CALC = 12,

    SCAN = 13,

    LINK = 14,

    SOFT = 15,

    BAD_SUB = 16,

    // Undefined alarm status

    UDF = 17,

    DISABLE = 18,

    SIMM = 19,

    READ_ACCESS = 20,

    WRITE_ACCESS = 21

  }

}
