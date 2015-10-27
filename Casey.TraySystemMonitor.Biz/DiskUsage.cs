using System;

using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{

   /// <summary>
   /// Gets a readout of the total disk usage, both writes and reads.
   /// </summary>
   class DiskUsage : EXT.IStatusProvider
   {

      #region Private Fields

      private System.Diagnostics.PerformanceCounter _read =
         new System.Diagnostics.PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total", true);
      private System.Diagnostics.PerformanceCounter _write =
         new System.Diagnostics.PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total", true);
      private readonly System.Drawing.Image _image = null;

      #endregion Private Fields

      #region Public Methods

      /// <summary>
      /// Constructor
      /// </summary>
      public DiskUsage()
      {
         Casey.Utility.ResourceManager rm = new Casey.Utility.ResourceManager();
         _image = rm.GetImageBySuffix("Images." + this.GetType().Name + ".png");
      }

      #endregion Public Methods

      #region IStatusProvider Members

      string Casey.TraySystemMonitor.Ext.IStatusProvider.Name
      {
         get { return "Disk Usage"; }
      }

      string Casey.TraySystemMonitor.Ext.IStatusProvider.ShortName
      {
         get { return "Disk"; }
      }

      string Casey.TraySystemMonitor.Ext.IStatusProvider.Unit
      {
         get { return " kbytes/sec"; }
      }

      Casey.TraySystemMonitor.Ext.StatusType Casey.TraySystemMonitor.Ext.IStatusProvider.Type
      {
         get { return EXT.StatusType.Readout; }
      }

      System.Drawing.Image EXT.IStatusProvider.Image
      {
         get { return _image; }
      }

      float Casey.TraySystemMonitor.Ext.IStatusProvider.NextValue()
      {
         return (_read.NextValue() + _write.NextValue()) / (float)Math.Pow(2, 10);
      }

      #endregion

      #region IDisposable Members

      void IDisposable.Dispose()
      {
         _read.Dispose();
         _write.Dispose();
         _image.Dispose();
      }

      #endregion

   }
}
