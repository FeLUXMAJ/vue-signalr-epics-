//
// Searcher.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading ;
using EPICSsharp.Common.Pipes ;
using System.IO ;
using System.Net ;

namespace EPICSsharp.CA.Client
{

  class Searcher : IDisposable
  {

    Thread m_searchThread ;

    CAClient Client ;

    bool m_needToRun = true ;

    bool m_disposed = false ;

    List<Channel> m_channelsToSearch = new List<Channel>() ;

    internal Searcher ( CAClient client )
    {
      Client = client ;
      m_searchThread = new Thread(SearchChannels){
        IsBackground = true 
      } ;
      m_searchThread.Start() ;
    }

    internal void Add ( Channel channel )
    {
      lock ( m_channelsToSearch )
      {
        channel.SearchInvervalCounter = channel.SearchInverval = 1 ;
        if ( ! m_channelsToSearch.Contains(channel) )
          m_channelsToSearch.Add(channel) ;
      }
    }

    internal bool Contains ( Channel channel )
    {
      lock ( m_channelsToSearch )
        return m_channelsToSearch.Contains(channel) ;
    }

    internal void Remove ( Channel channel )
    {
      lock ( m_channelsToSearch )
        m_channelsToSearch.Remove(channel) ;
    }

    void SearchChannels ( )
    {
      while ( m_needToRun )
      {

        Thread.Sleep(50) ;

        lock ( m_channelsToSearch )
        {
          if ( m_channelsToSearch.Count == 0 )
            continue ;
        }

        MemoryStream mem = new MemoryStream() ;
        lock ( m_channelsToSearch )
        {
          foreach ( Channel c in m_channelsToSearch )
            c.SearchInvervalCounter-- ;

          foreach (
            Channel c in m_channelsToSearch.Where(
              row => 
                 row.SearchInvervalCounter <= 0 
              && (
                    this.Client.Configuration.MaxSearchSeconds == 0
                 || ( 
                      DateTime.Now - row.StartSearchTime 
                    ).TotalSeconds < this.Client.Configuration.MaxSearchSeconds 
                 )
            )
          ) {
            c.SearchInverval *= 2 ;
            if ( c.SearchInverval > 10 )
              c.SearchInverval = 10 ;
            c.SearchInvervalCounter = c.SearchInverval ;

            mem.Write(
              c.SearchPacket.Data, 
              0, 
              c.SearchPacket.Data.Length
            ) ;
            if ( mem.Length > 1400 )
            {
              SendBuffer(mem.ToArray()) ;
              mem.Dispose() ;
              mem = new MemoryStream() ;
            }
          }
        }
        if ( mem.Position != 0 )
          SendBuffer(
            mem.ToArray()
          ) ;
      }
    }

    void SendBuffer ( byte[] buff )
    {
      foreach ( IPEndPoint i in Client.Configuration.SearchAddresses )
      {
        ( 
          (UdpReceiver) Client.Udp[0]
        ).Send(
          i, 
          buff
        ) ;
      }
    }

    public void Dispose ( )
    {
      if ( m_disposed )
        return ;
      m_disposed = true ;
      m_needToRun = false ;
    }

  }

}
