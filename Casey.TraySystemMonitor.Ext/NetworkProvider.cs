using System.Collections.Generic;   // IList, List
using System.Diagnostics;           // PerformanceCounter

namespace Casey.TraySystemMonitor.Ext
{

   /// <summary>
   /// Network provider that handles the performance counters
   /// required to get a network statistic
   /// </summary>
   public class NetworkProvider : System.IDisposable
   {

      /// <summary>List of all the performance counters</summary>
      private IList<PerformanceCounter> _counters = new List<PerformanceCounter>();
      /// <summary>Name of the particular counter to poll</summary>
      private readonly string _counterName = null;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="counterName">Name of the counter to poll</param>
      public NetworkProvider(string counterName)
      {
         _counterName = counterName;
         RefreshCounters();
      }

      /// <summary>
      /// Gets the next value from the counters
      /// </summary>
      /// <returns>Next value from the counters</returns>
      public float NextValue()
      {
         float retVal = 0;

         RefreshCounters();

         foreach (PerformanceCounter pc in _counters)
         {
            try
            {
               retVal += pc.NextValue();
            }
            catch (System.InvalidOperationException)
            {
            }
         }

         return retVal;
      }

      /// <summary>
      /// Re-creates the _counters list if the number
      /// of valid PerformanceCounters has changed.
      /// </summary>
      private void RefreshCounters()
      {
         PerformanceCounterCategory cat = new PerformanceCounterCategory("Network Interface");
         if (cat.GetInstanceNames().Length != _counters.Count)
         {
            _counters.Clear();
            foreach (string instance in cat.GetInstanceNames())
            {
               _counters.Add(new PerformanceCounter("Network Interface", _counterName, instance));
            }
         }
      }

      #region IDisposable Members

      public void Dispose()
      {
         foreach (PerformanceCounter pc in _counters)
         {
            pc.Dispose();
         }
      }

      #endregion

   }
}
