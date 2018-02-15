//
// CAEnumRecord.cs
//

using EPICSsharp.CA.Constants ;

using System ;

namespace EPICSsharp.CA.Server.RecordTypes
{

  // A CARecord for serving enum types.
  // 
  // Channel Access enforces some restrictions on enumerations.
  // For details see the CheckEnumType method.
  // <typeparam name="TType">The enum type</typeparam>

  public class CAEnumRecord<TType> 
  : CARecord<TType> where TType : struct
  , IComparable
  , IFormattable
  , IConvertible
  {

    // The constructor will check the enum TType
    // to validate for the Channel Access constraints.

    public CAEnumRecord ( )
    {
      CheckEnumType() ;
    }

    // Check the enum type TType, if it can be used with Channel Access.
    // 
    // In the Channel Access protocol, enumerations are restricted to
    // the following constraints:
    // - The maximum number of constant definitions is 16
    // - The maximum length of an enumeration label is 26
    // - In CA enumerations internally use unsigned 16 bit integers,
    // while in C# you can use different integer types
    // (Int32 being the default).

    public static void CheckEnumType ( )
    {
      if ( ! typeof(TType).IsEnum )
      {
        throw new ArgumentException(String.Format("Not an enum type: {0}", typeof(TType).Name)) ;
      }
      CheckEnumTypeNames() ;
      CheckEnumTypeValues() ;
    }

    public static void CheckEnumTypeNames ( )
    {
      string[] names = Enum.GetNames(typeof(TType)) ;
      if ( names.Length > 16 )
      {
        throw new ArgumentException(
          String.Format(
            "Too many constants (> 16): {0}", 
            names.Length
          )
        ) ;
      }
      foreach ( string name in names )
      {
        if ( name.Length > 26 )
        {
          throw new ArgumentException(
            String.Format(
              "Enum constant too long (> 26): {0}", 
              name
            )
          ) ;
        }
      }
    }

    public static void CheckEnumTypeValues ( )
    {
      // From MSDN (http://msdn.microsoft.com/en-us/library/sbbt4032.aspx):
      // The approved types for an enum are byte, sbyte, short, ushort, int, uint, long, or ulong.

      switch ( Enum.GetUnderlyingType(typeof(TType)).Name )
      {
      // With these types, we need to check, if the constant values
      // actually fit into the 16 bits available in the CA protocol.
      // Sadly, I could not make this work any nicer.
      case "Int32":
        foreach ( TType v in (TType[]) Enum.GetValues(typeof(TType)) )
        {
          int mask = unchecked( (int) 0xffff0000 ) ;
          int val = (int) (object) v ;
          if ( ( mask & val ) != 0 )
          {
            throw new ArgumentException(String.Format("Enum value does not fit in 16 bits: {0}", v)) ;
          }
        }
        break ;
      case "UInt32":
        foreach ( TType v in (TType[]) Enum.GetValues(typeof(TType)) )
        {
          uint mask = 0xffff0000 ;
          uint val = (uint) (object) v ;
          if ( ( mask & val ) != 0 )
          {
            throw new ArgumentException(String.Format("Enum value does not fit in 16 bits: {0}", v)) ;
          }
        }
        break ;
      case "Int64":
        foreach ( TType v in (TType[]) Enum.GetValues(typeof(TType)) )
        {
          long mask = unchecked( (long) 0xffffffffffff0000 ) ;
          long val = (long) (object) v ;
          if ( ( mask & val ) != 0 )
          {
            throw new ArgumentException(String.Format("Enum value does not fit in 16 bits: {0}", v)) ;
          }
        }
        break ;
      case "UInt64":
        foreach ( TType v in (TType[]) Enum.GetValues(typeof(TType)) )
        {
          ulong mask = 0xffffffffffff0000 ;
          ulong val = (ulong) (object) v ;
          if ( ( mask & val ) != 0 )
          {
            throw new ArgumentException(String.Format("Enum value does not fit in 16 bits: {0}", v)) ;
          }
        }
        break ;
      default:
        // OK, the other types will fit in 16 bits
        break ;
      }
    }

    TType currentValue ;

    [CAField("VAL")]
    public override TType Value
    {
      get
      {
        return this.currentValue ;
      }
      set
      {
        if ( !currentValue.Equals(value) )
          this.IsDirty = true ;
        currentValue = value ;
        if ( Scan == ScanAlgorithm.ON_CHANGE && this.IsDirty )
          ProcessRecord() ;
      }
    }

  }

}
