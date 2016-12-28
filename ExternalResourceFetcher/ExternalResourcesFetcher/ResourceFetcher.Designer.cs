namespace ExternalResourcesFetcher
{
  partial class ResourceFetcher
  {
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.timerMain = new System.Timers.Timer();
      ((System.ComponentModel.ISupportInitialize)(this.timerMain)).BeginInit();
      // 
      // timerMain
      // 
      this.timerMain.Enabled = true;
      this.timerMain.Interval = 60000;
      this.timerMain.Elapsed += new System.Timers.ElapsedEventHandler(this.timerMain_Elapsed);

      this.eventLog = new System.Diagnostics.EventLog();
      ((System.ComponentModel.ISupportInitialize)(this.eventLog)).BeginInit();
      // 
      // ResourceFetcher
      // 
      this.ServiceName = "ResourceFetcher";
      ((System.ComponentModel.ISupportInitialize)(this.eventLog)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.timerMain)).EndInit();

    }

    #endregion

    private System.ComponentModel.IContainer components;
    private System.Diagnostics.EventLog eventLog;
  }
}
