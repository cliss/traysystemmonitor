using System;
using System.Management;

using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{
   class WirelessStrength : EXT.IStatusProvider
   {

      #region Private Fields

      private const float MIN_STRENGTH = -40;
      private const float MAX_STRENGTH = -90;
      private readonly System.Drawing.Image _image = null;
      private float _max = MIN_STRENGTH + MAX_STRENGTH * -1;

      #endregion Private Fields

      #region Public Methods

      /// <summary>
      /// Constructor
      /// </summary>
      public WirelessStrength()
      {
         Casey.Utility.ResourceManager rm = new Casey.Utility.ResourceManager();
         _image = rm.GetImageBySuffix("Images." + this.GetType().Name + ".png");
      }

      #endregion Public Methods

      #region IStatusProvider Members

      string Casey.TraySystemMonitor.Ext.IStatusProvider.Name
      {
         get { return "Wireless Strength"; }
      }

      string Casey.TraySystemMonitor.Ext.IStatusProvider.ShortName
      {
         get { return "Wireless"; }
      }

      string EXT.IStatusProvider.Unit
      {
         get { return "%"; }
      }

      Casey.TraySystemMonitor.Ext.StatusType Casey.TraySystemMonitor.Ext.IStatusProvider.Type
      {
         get { return EXT.StatusType.Percentage; ; }
      }

      System.Drawing.Image Casey.TraySystemMonitor.Ext.IStatusProvider.Image
      {
         get { return _image; }
      }

      float Casey.TraySystemMonitor.Ext.IStatusProvider.NextValue()
      {
         ManagementScope scope = new ManagementScope("root\\wmi");
         ObjectQuery query = new ObjectQuery(
            "SELECT * FROM MSNdis_80211_ReceivedSignalStrength WHERE active=true");
         double strength = 0;
         int numFound = 0;
         using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
         {
            try
            {
               foreach (ManagementObject obj in searcher.Get())
               {
                  // Strength is from -40 to -90; add -90 to reset band to be from
                  // 0 to 50.
                  double thisStrength = Convert.ToDouble(obj["Ndis80211ReceivedSignalStrength"]) + _max + Math.Abs(MIN_STRENGTH);
                  if (thisStrength > _max)
                  {
                     _max = (float)thisStrength;
                     System.Diagnostics.Trace.WriteLine("Increasing max to " + _max.ToString());
                  }
                  strength += thisStrength;
                  ++numFound;
               }
            }
            catch (System.Management.ManagementException)
            {
            }
         }

         float retVal = Math.Abs((float)strength / numFound / _max * 100);
         if (retVal < 0)
         {
            retVal = 0;
         }
         else if (retVal > 100)
         {
            retVal = 100;
         }

         return retVal;
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
