using System.Diagnostics;

using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{

   /// <summary>
   /// CPU Monitor monitors CPU usage.
   /// </summary>
   class CpuMonitor : EXT.IStatusProvider
   {

      #region Private Fields

      private PerformanceCounter _counter = new PerformanceCounter(
         "Processor",
         "% Processor Time",
         "_Total", 
         true);
      private readonly System.Drawing.Image _image = null;

      #endregion Private Fields

      #region Public Methods

      /// <summary>
      /// Constructor
      /// </summary>
      public CpuMonitor()
      {
         Casey.Utility.ResourceManager rm = new Casey.Utility.ResourceManager();
         _image = rm.GetImageBySuffix("Images." + this.GetType().Name + ".png");
      }

      #endregion Public Methods

      #region IStatusProvider Members

      string EXT.IStatusProvider.Name
      {
         get { return "CPU Usage"; }
      }

      string EXT.IStatusProvider.ShortName
      {
         get { return "CPU"; }
      }

      string EXT.IStatusProvider.Unit
      {
         get { return "%"; }
      }

      EXT.StatusType EXT.IStatusProvider.Type
      {
         get { return EXT.StatusType.Percentage; }
      }

      System.Drawing.Image EXT.IStatusProvider.Image
      {
         get { return _image; }
      }

      float EXT.IStatusProvider.NextValue()
      {
         return _counter.NextValue();
      }

      #endregion

      #region IDisposable Members

      void System.IDisposable.Dispose()
      {
         _counter.Dispose();
         _image.Dispose();
      }

      #endregion

   }
}
