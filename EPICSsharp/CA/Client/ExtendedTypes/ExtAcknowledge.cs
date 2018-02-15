//
// ExtAcknowledge.cs
//

namespace EPICSsharp.CA.Client
{

  // The extended epics Acknowledge type serves severity, status, value, 
  // precision (for double and float), unittype and a bunch of limits.

  public class ExtAcknowledge<TType> : ExtType<TType>
  {

    internal ExtAcknowledge( )
    { }

    // transient of the acknowledge message

    public short AcknowledgeTransient { get ; internal set ; }

    // Severity of the acknowledge serverity

    public short AcknowledgeSeverity { get ; internal set ; }

  }

}
