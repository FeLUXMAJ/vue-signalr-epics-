//
// ScanAlgorithm.cs
//

namespace EPICSsharp.CA.Constants
{

  // The scanning algorith for a record in the CAServer.

  public enum ScanAlgorithm
  {

    // Scan with 10Hz (10 times per second = every 100ms).
    HZ10,

    // Scan with 5Hz (5 times per second = every 200ms).
    HZ5,

    // Scan with 2Hz (2 times per second = every 500ms).
    HZ2,

    // Scan every second (= 1Hz).
    SEC1,

    // Scan every 2 seconds (= 0.5Hz).
    SEC2,

    // Scan every 5 seconds (= 0.2Hz).
    SEC5,

    // Scan every 10 seconds (= 0.1Hz).
    SEC10,

    ON_CHANGE,

    PASSIVE

  }

}
