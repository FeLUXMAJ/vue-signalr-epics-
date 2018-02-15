//
// CAValueRecord.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using EPICSsharp.CA.Constants ;
using System.Reflection ;

namespace EPICSsharp.CA.Server.RecordTypes
{


  // A double record which handles the limits of the value and set the alarm accordingly.
  public abstract class CAValueRecord<TType> : CARecord<TType> where TType : IComparable<TType>
  {

    // Defines the value on which the High High alarm will be triggered.

    [CAField("HIHI")]
    public TType HighHighAlarmLimit { get ; set ; }

    // Defines the value on which the High alarm will be triggered.

    [CAField("HIGH")]
    public TType HighAlarmLimit { get ; set ; }

    // Defines the value on which the Low Low alarm will be triggered.

    [CAField("LOLO")]
    public TType LowLowAlarmLimit { get ; set ; }

    // Defines the value on which the Low alarm will be triggered.

    [CAField("LOW")]
    public TType LowAlarmLimit { get ; set ; }

    // Defines the value of the severity for a low low alarm.

    [CAField("LLSV")]
    public AlarmSeverity LowLowAlarmSeverity { get ; set ; }

    // Defines the value of the severity for an high low alarm.

    [CAField("HSV")]
    public AlarmSeverity HighAlarmSeverity { get ; set ; }

    // Defines the value of the severity for a low alarm.

    [CAField("LSV")]
    public AlarmSeverity LowAlarmSeverity { get ; set ; }

    // Defines the value of the severity for an High High alarm.

    [CAField("HHSV")]
    public AlarmSeverity HighHighAlarmSeverity { get ; set ; }

    string engineeringUnits = "" ;

    // Defines the value of the Engineering Units.

    [CAField("EGU")]
    public string EngineeringUnits
    {
      get
      {
        return engineeringUnits ;
      }
      set
      {
        if ( value.Length > 8 )
          throw new Exception("Cannot have more than 8 characters for the engineering unit.") ;
        engineeringUnits = value ;
      }
    }

    // Defines the Display Precision.

    [CAField("PREC")]
    public short DisplayPrecision { get ; set ; }

    // Defines the high display limit.

    [CAField("HIGHDISP")]
    public TType HighDisplayLimit { get ; set ; }

    // Defines the low display limit.

    [CAField("LOWDISP")]
    public TType LowDisplayLimit { get ; set ; }

    // Defines the High Operating Range.

    [CAField("HOPR")]
    public TType HighOperatingRange { get ; set ; }

    // Defines the Low Operating Range.

    [CAField("LOPR")]
    public TType LowOperatingRange { get ; set ; }

    // Initialize the record with default alarm limits which are the max and min double values.

    internal CAValueRecord ( )
    {
      Dictionary<string, TType> constants = typeof(TType).GetFields(
        BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
      ).Where(
         row => (
            row.IsLiteral 
         && !row.IsInitOnly 
         && row.FieldType == typeof(TType)
         )
       ).ToDictionary(
         key => key.Name, 
         val => (TType) val.GetValue(null)
       ) ;

      LowLowAlarmLimit   = constants["MinValue"] ;
      LowAlarmLimit      = constants["MinValue"] ;
      HighAlarmLimit     = constants["MaxValue"] ;
      HighHighAlarmLimit = constants["MaxValue"] ;

      LowDisplayLimit  = constants["MinValue"] ;
      HighDisplayLimit = constants["MaxValue"] ;

      LowLowAlarmSeverity   = AlarmSeverity.MAJOR ;
      HighHighAlarmSeverity = AlarmSeverity.MAJOR ;
      LowAlarmSeverity      = AlarmSeverity.MINOR ;
      HighAlarmSeverity     = AlarmSeverity.MINOR ;

      EngineeringUnits = "" ;
      DisplayPrecision = 0 ;
    }

    internal override void ProcessRecord ( )
    {
      if ( Value.CompareTo(LowLowAlarmLimit) <= 0 )
        TriggerAlarm(LowLowAlarmSeverity, AlarmStatus.LOLO) ;
      else if ( Value.CompareTo(LowAlarmLimit) <= 0 )
        TriggerAlarm(LowAlarmSeverity, AlarmStatus.LOW) ;
      else if ( Value.CompareTo(HighHighAlarmLimit) >= 0 )
        TriggerAlarm(HighHighAlarmSeverity, AlarmStatus.HIHI) ;
      else if ( Value.CompareTo(HighAlarmLimit) >= 0 )
        TriggerAlarm(HighAlarmSeverity, AlarmStatus.HIGH) ;
      else
        TriggerAlarm(AlarmSeverity.NO_ALARM, AlarmStatus.NO_ALARM) ;

      base.ProcessRecord() ;
    }

  }

}
