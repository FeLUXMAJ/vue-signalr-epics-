//
// CAConfiguration.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Net ;

namespace EPICSsharp.CA.Client
{

  public class CAConfiguration
  {

    internal CAConfiguration ( )
    {

      // Hmm, fails to load the System.Configuration.ConfigurationManager assemble ... ??? !!!
      // https://stackoverflow.com/questions/22992923/error-reading-configuration-could-not-load-type-from-assembly-system-confi
      // try
      // {
      //   if ( System.Configuration.ConfigurationManager.AppSettings["e#ServerList"] != null )
      //     SearchAddress = System.Configuration.ConfigurationManager.AppSettings["e#ServerList"] ;
      // }
      // catch ( System.Exception ex )
      // {
      // }

      if ( System.Diagnostics.Debugger.IsAttached )
        WaitTimeout = -1 ;
      else
        WaitTimeout = 5000 ;

      Hostname = Environment.MachineName ;
      Username = Environment.UserName ;
      MaxSearchSeconds = 0 ;
    }

    // Defines a timeout before the search of the channels ends.
    // Default 0 => will search all the time

    public int MaxSearchSeconds { get ; set ; }

    // Stores the time each operation took.

    public bool DebugTiming = false ;

    internal IPEndPoint[] SearchAddresses = new IPEndPoint[] { 
      new IPEndPoint(
        IPAddress.Broadcast, 
        5064
      ) 
    } ;

    public string Hostname { get ; internal set ; }

    public string Username { get ; internal set ; }

    // The timeout in miliseconds used for blocking opperations.
    // -1 == infinite

    public int WaitTimeout { get ; set ; }

    // The list of addresses used to search the channels.
    // Addresses must be separated by semi-columns (;) , and IP / ports must be separated by columns (:)

    public string SearchAddress
    {
      get
      {
        string searchAddress = "" ;
        for ( int i = 0 ; i < SearchAddresses.Length ; i++ )
        {
          if ( i != 0 )
            searchAddress += ";" ;
          searchAddress += (
            SearchAddresses[i].Address 
          + ":" 
          + SearchAddresses[i].Port 
          ) ;
        }
        return searchAddress ;
      }
      set
      {
        var endPointsList = new List<IPEndPoint>() ;
        value = value.Replace(' ',';').Replace(',',';') ;
        string[] parts = value.Split(';') ;
        foreach ( string part in parts )
        {
          string[] field = part.Split(':') ;
          IPAddress ip ;
          try
          {
            ip = IPAddress.Parse(field[0]) ;
          }
          catch
          {
            ip = Dns.GetHostEntry(field[0]).AddressList.First() ;
          }
          endPointsList.Add(
            field.Length == 1 
            ? new IPEndPoint(ip,5064) 
            : new IPEndPoint(
                ip, 
                int.Parse(field[1])
              )
          ) ;
        }
        SearchAddresses = endPointsList.ToArray() ;
      }
    }

  }

}
