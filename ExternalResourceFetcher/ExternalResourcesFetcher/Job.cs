using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using NLog;

namespace ExternalResourcesFetcher
{
  public class Job
  {
    public Resource m_Resource;

    public string m_OutputPath;

    private Logger logger = LogManager.GetLogger("logger");

    public Job(Resource p_Resource, string p_OutputPath)
    {
      m_Resource = p_Resource;
      m_OutputPath = p_OutputPath;
    }

    public void StartJob()
    {
      try
      {
        Parse(); 
      }
      catch
      {
        logger.Trace(string.Format("Parsing from {0} failed! ", m_Resource.Url));
        throw;
      }
    }

    private void Parse()
    {
      logger.Trace(string.Format("Parsing from {0} started.", m_Resource.Url));

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_Resource.Url);
      request.Credentials = new NetworkCredential(m_Resource.UserName, m_Resource.Password);
      request.Timeout = 50000; // milliseconds, adjust as needed
      request.ReadWriteTimeout = 100000; // milliseconds, adjust as needed

      using (WebResponse response = request.GetResponse())
      {
        using (Stream responseStream = response.GetResponseStream())
        {
          byte[] bufferDownload = new byte[2048];
          int nRead;

          using (MemoryStream memoryStream = new MemoryStream())
          {
            while ((nRead = responseStream.Read(bufferDownload, 0, bufferDownload.Length)) > 0)
            {
              memoryStream.Write(bufferDownload, 0, nRead);
            }
            memoryStream.Position = 0;

            
            switch (m_Resource.Type)
            {
               case ResourceType.Normal:
                if (m_Resource.Url.EndsWith(".zip"))
                {
                  using (ZipArchive archive = new ZipArchive(memoryStream))
                  {
                    archive.ExtractToDirectory(m_OutputPath);
                  }
                }
                else
                {
                  using (FileStream file = new FileStream(Path.Combine(m_OutputPath, m_Resource.Url.Split('/').Last()), FileMode.Create, FileAccess.Write))
                  {
                    memoryStream.WriteTo(file);
                    file.Close();
                  }
                }

                break;
               case ResourceType.List:
                string[] sUrlParts = m_Resource.Url.Split('/');
                m_Resource.Url = m_Resource.Url.Remove(m_Resource.Url.Length - sUrlParts[sUrlParts.Length - 1].Length, sUrlParts[sUrlParts.Length - 1].Length);

                using (StreamReader srList = new StreamReader(memoryStream))
                {
                  string sUrl;
                  while ((sUrl = srList.ReadLine()) != null)
                  {
                    Job newJob = new Job(new Resource(m_Resource.UserName, m_Resource.Password, m_Resource.Url + sUrl, (int)ResourceType.Normal),
                      m_OutputPath);
                    newJob.Parse();
                  } 
                }

                break;
            }

            memoryStream.Close();  
          }
        }
      }

      logger.Trace(string.Format("Parsing from {0} ended successfully.", m_Resource.Url));
    }
  }
}
