//
// CommandID.cs
//

using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;

namespace EPICSsharp.CA.Constants
{

  // Command IDs are used in the CA message header for indicating the
  // type of CA message. This determines the meaning of the other header fields.

  internal enum CommandID : ushort
  {

    // CA protocol version
    CA_PROTO_VERSION = 0x00,

    // Register monitor
    CA_PROTO_EVENT_ADD = 0x01,

    // Unregister monitor
    CA_PROTO_EVENT_CANCEL = 0x02,

    // Read channel value (without notification)
    CA_PROTO_READ = 0x03,

    // Write channel value (without notification)
    CA_PROTO_WRITE = 0x04,

    // Search for a channel
    CA_PROTO_SEARCH = 0x06,

    // Disable monitor events
    CA_PROTO_EVENTS_OFF = 0x08,

    // Enable monitor events
    CA_PROTO_EVENTS_ON = 0x09,

    // Error during operation
    CA_PROTO_ERROR = 0x0B,

    // Release channel resources
    CA_PROTO_CLEAR_CHANNEL = 0x0C,

    // Server beacon
    CA_PROTO_RSRV_IS_UP = 0x0D,

    // Channel not found
    CA_PROTO_NOT_FOUND = 0x0E,

    // Read channel value (with notification)
    CA_PROTO_READ_NOTIFY = 0x0F,

    // Repeater registration confirmation
    CA_PROTO_REPEATER_CONFIRM = 0x11,

    // Create channel
    CA_PROTO_CREATE_CHAN = 0x12,

    // Write channel value (with notification)
    CA_PROTO_WRITE_NOTIFY = 0x13,

    // Client user name
    CA_PROTO_CLIENT_NAME = 0x14,

    // Client host name
    CA_PROTO_HOST_NAME = 0x15,

    // Channel access rights
    CA_PROTO_ACCESS_RIGHTS = 0x16,

    // Ping CA server
    CA_PROTO_ECHO = 0x17,

    // Register client on repeater
    CA_PROTO_REPEATER_REGISTER = 0x18,

    // Channel creation failed
    CA_PROTO_CREATE_CH_FAIL = 0x1A,

    // Server is going down
    CA_PROTO_SERVER_DISCONN = 0x1B,

    // Invalid response
    CA_PROTO_BAD_RESPONSE = 0xFFFF     // unofficial

  }

}
