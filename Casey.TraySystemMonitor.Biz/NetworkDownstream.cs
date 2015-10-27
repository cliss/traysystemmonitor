using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{

   /// <summary>
   /// Provider to get the downstream throughput on all devices
   /// </summary>
   class NetworkDownstream : EXT.IStatusProvider
   {

      #region Private Fields

      /// <summary>Network provider object</summary>
      private EXT.NetworkProvider _provider = 
         new EXT.NetworkProvider("Bytes Received/sec");
      private readonly System.Drawing.Image _image = null;

      #endregion Private Fields

      #region Public Methods

      /// <summary>
      /// Constructor
      /// </summary>
      public NetworkDownstream()
      {
         Casey.Utility.ResourceManager rm = new Casey.Utility.ResourceManager();
         _image = rm.GetImageBySuffix("Images." + this.GetType().Name + ".png");
      }

      #endregion Public Methods

      #region IStatusProvider Members

      string EXT.IStatusProvider.Name
      {
         get { return "Network Downstream"; }
      }

      string EXT.IStatusProvider.ShortName
      {
         get { return "Net Down"; }
      }

      string EXT.IStatusProvider.Unit
      {
         get { return " bytes/sec"; }
      }

      EXT.StatusType EXT.IStatusProvider.Type
      {
         get { return EXT.StatusType.Readout; }
      }

      System.Drawing.Image EXT.IStatusProvider.Image
      {
         get { return _image; }
      }

      float EXT.IStatusProvider.NextValue()
      {
         return _provider.NextValue();
      }

      #endregion

      #region IDisposable Members

      void System.IDisposable.Dispose()
      {
         _provider.Dispose();
      }

      #endregion

   }
}
