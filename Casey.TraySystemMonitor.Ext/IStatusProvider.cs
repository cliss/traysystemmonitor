

namespace Casey.TraySystemMonitor.Ext
{

   /// <summary>
   /// Defines teh different types of status providers
   /// </summary>
   public enum StatusType
   {
      /// <summary>A percentage, or number between 0 and 100</summary>
      Percentage = 0,
      /// <summary>A temporary readout or snapshot, such as 45MB</summary>
      Readout,
      /// <summary>A counter that keeps growing with activity/time</summary>
      Counter
   }

   /// <summary>
   /// Defines a status provider.
   /// </summary>
   public interface IStatusProvider : System.IDisposable
   {
      /// <summary>Gets the long name for this provider</summary>
      string Name { get; }

      /// <summary>Gets the short/abbreviated name for this provider</summary>
      string ShortName { get; }

      /// <summary>
      /// Gets the unit of this provider, for example, 
      /// "%" or " bytes/sec"
      /// </summary>
      string Unit { get; }

      /// <summary>
      /// Gets the type for this provider
      /// </summary>
      StatusType Type { get; }

      /// <summary>
      /// Gets the image associated with this provider
      /// </summary>
      System.Drawing.Image Image { get; }

      /// <summary>
      /// Gets the next value for this provider
      /// </summary>
      /// <returns>Next value for this provider</returns>
      float NextValue();

   }
}
