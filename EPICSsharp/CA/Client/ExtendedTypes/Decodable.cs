//
// Decodable.cs
//

namespace EPICSsharp.CA.Client
{

  public abstract class Decodable
  {
    internal abstract void Decode ( Channel channel, uint nbElements ) ;
  }

}
