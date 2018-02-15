//
// CARecordGeneric.cs
//

using EPICSsharp.CA.Constants ;

namespace EPICSsharp.CA.Server.RecordTypes
{

  // Generic CARecord allows to store a type in the VAL property
  // <typeparam name="TType"></typeparam>

  public abstract class CARecord<TType> : CARecord
  {

    // Stores the actual value of the record

    private TType m_currentValue ;

    // Access the value linked to the record

    [CAField("VAL")]
    public virtual TType Value
    {
      get
      {
        return m_currentValue ;
      }
      set
      {
        if (
           (
              m_currentValue == null 
           && value != null
           ) 
        || ! m_currentValue.Equals(value)
        ) {
          this.IsDirty = true ;
        }
        m_currentValue = value ;
        if ( 
           Scan == ScanAlgorithm.ON_CHANGE 
        && this.IsDirty
        ) {
          ProcessRecord() ;
        }
      }
    }

  }

}
