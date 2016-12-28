using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace ExternalResourcesFetcher
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
      try
      {
        ServiceBase[] ServicesToRun;

        ServicesToRun = new ServiceBase[] { new ResourceFetcher() };
        ServiceBase.Run(ServicesToRun);
      }
      catch (Exception p_Exception)
      {
        EventLog.WriteEntry(
          "ExternalResourcesFetcher",
          "ServiceProcess error.\n" + p_Exception.Message,
          EventLogEntryType.Error
          );

        throw new Exception("ServiceProcess error.", p_Exception);
      }
    }
  }
}
