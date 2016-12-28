using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;

namespace ExternalResourcesFetcher
{
  public partial class ResourceFetcher : ServiceBase
  {
    /// <summary>
    /// Service timer
    /// </summary>
    private System.Timers.Timer timerMain;

    /// <summary>
    /// Logger object 
    /// </summary>
    private Logger logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// User configurations, from XML
    /// </summary>
    private UserConfigurations ucConfigurations;

    public ResourceFetcher()
    {
      InitializeComponent();
    }

    protected override void OnStart(string[] p_Args)
    {
      timerMain.Start();
    }

    private void timerMain_Elapsed(object p_Sender, System.Timers.ElapsedEventArgs p_E)
    {
      try
      {
        ucConfigurations =
          UserConfigurations.DeserializeFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configurations.xml"));
      }
      catch (Exception p_Exception)
      {
        logger.Error("Loading service configurations failed! \n" + Utils.GetExceptionMessage(p_Exception));
        return;
      }

      try
      {
        using (FileTarget target = (FileTarget)LogManager.Configuration.FindTargetByName("file"))
        {
          target.MaxArchiveFiles = ucConfigurations.MaxArchiveFiles; 
        }
      }
      catch (Exception p_Exception)
      {
        logger.Error(Utils.GetExceptionMessage(p_Exception));
        return;
      }

      int hour = DateTime.Now.Hour;
      int minute = DateTime.Now.Minute;

      if (minute == ucConfigurations.StartTime.Minute && hour == ucConfigurations.StartTime.Hour)
      {
        Run(0);
      }
    }

    private void Run(int p_RunNumber)
    {
      logger.Info("Service started successfully.");

      try
      {
        if (Directory.Exists(ucConfigurations.ResourceDirectory))
        {
          //clear output folder
          Directory.Delete(ucConfigurations.ResourceDirectory, true);
          logger.Trace("Output folder deleted successfully.");
        }
        Directory.CreateDirectory(ucConfigurations.ResourceDirectory);
        logger.Trace("Output folder created successfully.");

        Parallel.ForEach(ucConfigurations.Resources, (currentResource) =>
        {
          Job job = new Job(currentResource, ucConfigurations.ResourceDirectory);
          job.StartJob();
        });

        logger.Info("All jobs executed successfully.");
      }
      catch (Exception p_Exception)
      {
        logger.Error("ResourceFetcher.OnStart Failed!" + Utils.GetExceptionMessage(p_Exception));

        if (p_RunNumber < ucConfigurations.Retries)
        {
          logger.Info(string.Format("Service will retry in {0} minutes", ucConfigurations.RetryTimeout));
          Thread.Sleep(1000*60*ucConfigurations.RetryTimeout);
          p_RunNumber++;
          Run(p_RunNumber);
        }
        else
        {
          try
          {
            using (MailMessage message = new MailMessage())
            {
              message.Subject = "External resource fetcher ended with failures.";
              message.Body = "Service external resource fetcher failed. Errors is in the attachment.";
              message.From = new MailAddress(ucConfigurations.EmailFrom);
              using (Attachment aFileAttachment = new Attachment(Utils.GetLogFileName("file")))
              {
                message.Attachments.Add(aFileAttachment);

                foreach (string email in ucConfigurations.EmailsTo)
                {
                  message.To.Add(email);
                }

                using (SmtpClient smtp = new SmtpClient(ucConfigurations.Smtp))
                {
                  smtp.Credentials = new NetworkCredential(ucConfigurations.SmtpUserName, ucConfigurations.SmtpPassword);
                  smtp.Send(message);
                }
              }
            }

          }
          catch (Exception exception)
          {
            logger.Error("ResourceFetcher.SendMail Failed! \n" + Utils.GetExceptionMessage(exception));
          }
        }
      }
    }
  }
}