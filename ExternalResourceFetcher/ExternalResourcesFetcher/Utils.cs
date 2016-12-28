using System;
using System.IO;
using System.Xml.Serialization;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace ExternalResourcesFetcher
{
  public class Utils
  {
    /// <summary>
    /// Returns message string from passed exception and all its inner exceptions.
    /// </summary>
    /// <remarks>
    /// Messages of inner exceptions are separated by "\n" (new line).
    /// </remarks>
    /// <param name="p_Exception">
    /// Exception to be processed
    /// </param>
    /// <param name="p_Offset">
    /// Inner exception offset for display
    /// </param>
    /// <returns>
    /// Concatenation of message of passed exception and all its inner exceptions.
    /// </returns>
    public static string
      GetExceptionMessage(Exception p_Exception, string p_Offset)
    {
      string strResult;

      strResult = p_Offset + p_Exception.Message + ": " + p_Exception.Source + "\n";

      if (p_Exception.InnerException != null)
      {
        strResult += "---\n" + GetExceptionMessage(p_Exception.InnerException, p_Offset + "  ");
      }

      return strResult;
    }

    /// <summary>
    /// Returns message string from passed exception and all its inner exceptions.
    /// </summary>
    /// <remarks>
    /// Messages of inner exceptions are separated by "\n" (new line).
    /// </remarks>
    /// <param name="p_Exception">
    /// Exception to be processed
    /// </param>
    /// <returns>
    /// Concatenation of message of passed exception and all its inner exceptions.
    /// </returns>
    public static string
      GetExceptionMessage(Exception p_Exception)
    {
      string strMess = GetExceptionMessage(p_Exception, "");
      strMess += "\n-----\n" + p_Exception.StackTrace + "\n";
      return strMess;
    }

    public static string GetLogFileName(string targetName)
    {
      string fileName = null;

      if (LogManager.Configuration != null && LogManager.Configuration.ConfiguredNamedTargets.Count != 0)
      {
        Target target = LogManager.Configuration.FindTargetByName(targetName);
        if (target == null)
        {
          throw new Exception("Could not find target named: " + targetName);
        }

        FileTarget fileTarget = null;
        WrapperTargetBase wrapperTarget = target as WrapperTargetBase;

        // Unwrap the target if necessary.
        if (wrapperTarget == null)
        {
          fileTarget = target as FileTarget;
        }
        else
        {
          fileTarget = wrapperTarget.WrappedTarget as FileTarget;
        }

        if (fileTarget == null)
        {
          throw new Exception("Could not get a FileTarget from " + target.GetType());
        }

        var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
        fileName = fileTarget.FileName.Render(logEventInfo);
      }
      else
      {
        throw new Exception("LogManager contains no Configuration or there are no named targets");
      }

      if (!File.Exists(fileName))
      {
        throw new Exception("File " + fileName + " does not exist");
      }

      return fileName;
    }

  }

  /// <summary>
  /// Denotes type of the resource
  /// </summary>
  public enum ResourceType
  {
    /// <summary>
    /// Classic resource
    /// </summary>
    [XmlEnum("0")]
    Normal = 0,

    /// <summary>
    /// List of resources
    /// </summary>
    [XmlEnum("1")]
    List = 1
  }
}
