//
// EpicsType.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Constants
{

  // This enum represents the different CA value types and their corresponding intval

  internal enum EpicsType : ushort
  {

    // Plain string
    String = 0,

    // Plain 16bit integer
    Short = 1,

    // Plain 32bit floating point
    Float = 2,

    // Plain enumeration (using 16bit unsigned integer)
    Enum = 3,

    // Plain unsigned byte
    Byte = 4,

    // Plain 32bit integer
    Int = 5,

    // Plain 64bit floating point
    Double = 6,

    // Extends plain string by status and severity
    Status_String = 7,

    // Extends plain 16bit integer by status and severity
    Status_Short = 8,

    // Extends plain 32bit floating point by status and severity
    Status_Float = 9,

    // Extends plain enumeration by status and severity
    Status_Enum = 10,

    // Extends plain unsigned byte by status and severity
    Status_Byte = 11,

    // Extends plain 32bit integer by status and severity
    Status_Int = 12,

    // Extends plain 64bit floating point by status and severity
    Status_Double = 13,

    // Extends Status_String by timestamp
    Time_String = 14,

    // Extends Status_Short by timestamp
    Time_Short = 15,

    // Extends Status_Float by timestamp
    Time_Float = 16,

    // Extends Status_Enum by timestamp
    Time_Enum = 17,

    // Extends Status_Byte by timestamp
    Time_Byte = 18,

    // Extends Status_Int by timestamp
    Time_Int = 19,

    // Extends Status_Double by timestamp
    Time_Double = 20,

    // Extends Status_String by display bounds (not used since
    // a string cannot have display bounds)
    Display_String = 21,

    // Extends Status_Short by display bounds
    Display_Short = 22,

    // Extends Status_Float by display bounds
    Display_Float = 23,

    // Extends Status_Enum by a list of enumeration labels
    Labeled_Enum = 24,

    // Extends Status_Byte by display bounds
    Display_Byte = 25,

    // Extends Status_Int by display bounds
    Display_Int = 26,

    // Extends Status_Double by display bounds
    Display_Double = 27,

    // Extends Display_String by control bounds (not used since
    // a string cannot have control bounds)
    Control_String = 28,

    // Extends Display_Short by control bounds
    Control_Short = 29,

    // Extends Display_Float by control bounds
    Control_Float = 30,

    // Not used since parent type is Labeled_Enum
    Control_Enum = 31,

    // Extends Display_Byte by control bounds
    Control_Byte = 32,

    // Extends Display_Int by control bounds
    Control_Int = 33,

    // Extends Display_Double by control bounds
    Control_Double = 34,

    // Internal use within the library only!
    // Defines an unsigned 32 bit integer.
    // This is NOT a valid EPICS record type identifier used by CA!
    Internal_UInt = 0xFFFD,

    // Internal use within the library only!
    // Defines a signed 16 bit integer.
    // This is NOT a valid EPICS record type identifier used by CA!
    Internal_UShort = 0xFFFE,

    // Internal use within the library only!
    // Defines an invalid data type.
    // This is NOT a valid EPICS record type identifier used by CA!
    Invalid = 0xFFFF

  }

}
