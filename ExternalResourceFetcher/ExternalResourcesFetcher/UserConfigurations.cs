using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace ExternalResourcesFetcher
{
  public class UserConfigurations
  {
    /// <summary>
    /// DateTime property used for service start time
    /// </summary>
    [XmlIgnore]
    public DateTime StartTime;

    /// <summary>
    /// Start time in string format, used for XML parsing 
    /// </summary>
    [XmlElement(DataType = "string", ElementName = "Time")]
    public String TimeString
    {
      get { return StartTime.ToString("HH:mm"); }
      set { StartTime = DateTime.ParseExact(value, "HH:mm", CultureInfo.InvariantCulture); }
    }

    /// <summary>
    /// Path to output directory
    /// </summary>
    public string ResourceDirectory;

    /// <summary>
    /// Number of retries, if service execution failed
    /// </summary>
    public int Retries;

    /// <summary>
    /// Timeout in minutes, if service execution failed
    /// </summary>
    public int RetryTimeout;

    /// <summary>
    /// Max of log archive files, each day is a new file. 
    /// </summary>
    public int MaxArchiveFiles;

    /// <summary>
    /// Smtp server
    /// </summary>
    public string Smtp;

    /// <summary>
    /// User name for smtp server
    /// </summary>
    public string SmtpUserName;

    /// <summary>
    /// Password for user on smtp server
    /// </summary>
    public string SmtpPassword;

    /// <summary>
    /// List of emails, to send errors, when service execution failed.
    /// </summary>
    [XmlArray("EmailsTo")]
    [XmlArrayItem("Email")]
    public List<string> EmailsTo;

    /// <summary>
    /// Email from
    /// </summary>
    public string EmailFrom;

    /// <summary>
    /// List of resource object
    /// </summary>
    public List<Resource> Resources;

    /// <summary>
    /// Default CTOR
    /// </summary>
    public UserConfigurations()
    {
      StartTime = new DateTime();
      Retries = -1;
      RetryTimeout = -1;
      MaxArchiveFiles = -1;
      Smtp = string.Empty;
      SmtpUserName = string.Empty;
      SmtpPassword = string.Empty;
      EmailsTo = new List<string>();
      EmailFrom = string.Empty;
      ResourceDirectory = string.Empty;
      Resources = new List<Resource>();
    }

    public void SerializeToFile(string p_FileName)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(GetType());
      using (TextWriter writer = new StreamWriter(p_FileName))
      {
        xmlSerializer.Serialize(writer, this);
      }
    }

    public static UserConfigurations DeserializeFromFile(string p_FileName)
    {
      using (FileStream fileStream = new FileStream(p_FileName, FileMode.Open))
      {
        try
        {
          XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserConfigurations));
          UserConfigurations userConfiguration = (UserConfigurations)xmlSerializer.Deserialize(fileStream);
          return userConfiguration;
        }
        finally
        {
          fileStream.Close();
        } 
      }
    }
  }


  public class Resource
  {
    /// <summary>
    /// UserName for Url credentials
    /// </summary>
    public string UserName;

    /// <summary>
    /// Password of user, for Url credentials
    /// </summary>
    public string Password;

    /// <summary>
    /// Url address of resource
    /// </summary>
    public string Url;

    /// <summary>
    /// Resource type
    /// </summary>
    public ResourceType Type;

    /// <summary>
    /// CTOR with input parameters
    /// </summary>
    /// <param name="p_UserName">User name</param>
    /// <param name="p_Password">Pasword for user</param>
    /// <param name="p_Url">Url address</param>
    public Resource(string p_UserName, string p_Password, string p_Url, ResourceType p_ResourceType)
    {
      UserName = p_UserName;
      Password = p_Password;
      Url = p_Url;
      Type = p_ResourceType;
    }

    /// <summary>
    /// Default CTOR
    /// </summary>
    public Resource()
    {
      UserName = "";
      Password = "";
      Url = "";
      Type = ResourceType.Normal;
    }
  }
  
}