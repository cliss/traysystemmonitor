using System;

using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{

   /// <summary>
   /// Shows the remaining percentage of battery life.
   /// </summary>
   class BatteryStatus : EXT.IStatusProvider
   {

      #region Private Fields

      private readonly System.Drawing.Image _image = null;

      #endregion Private Fields

      #region Public Methods

      /// <summary>
      /// Constructor
      /// </summary>
      public BatteryStatus()
      {
         Casey.Utility.ResourceManager rm = new Casey.Utility.ResourceManager();
         _image = rm.GetImageBySuffix("Images." + this.GetType().Name + ".png");
      }

      #endregion Public Methods

      #region IStatusProvider Members

      string Casey.TraySystemMonitor.Ext.IStatusProvider.Name
      {
         get { return "Battery Remaining"; }
      }

      string Casey.TraySystemMonitor.Ext.IStatusProvider.ShortName
      {
         get { return "Batt"; }
      }

      string Casey.TraySystemMonitor.Ext.IStatusProvider.Unit
      {
         get { return "%"; }
      }

      Casey.TraySystemMonitor.Ext.StatusType Casey.TraySystemMonitor.Ext.IStatusProvider.Type
      {
         get { return EXT.StatusType.Percentage; }
      }

      System.Drawing.Image EXT.IStatusProvider.Image
      {
         get { return _image; }
      }

      float Casey.TraySystemMonitor.Ext.IStatusProvider.NextValue()
      {
         System.Windows.Forms.PowerStatus ps = System.Windows.Forms.SystemInformation.PowerStatus;
         return ps.BatteryLifePercent * 100;
      }

      #endregion

      #region IDisposable Members

      void IDisposable.Dispose()
      {
         _image.Dispose();
      }

      #endregion

   }
}
