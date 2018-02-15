//
// CAFieldAttribute.cs
//

using System ;

namespace EPICSsharp.CA.Server
{

  // Defines the binding between a C# property and an EPICS record field

  public class CAFieldAttribute : Attribute
  {

    public CAFieldAttribute ( string name )
    {
      Name = name ;
    }

    public string Name { get ; set ; }

  }

}
