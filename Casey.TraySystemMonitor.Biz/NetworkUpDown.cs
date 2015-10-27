using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{

   /// <summary>
   /// Gets the aggregate network usage, both up- and down-stream.
   /// </summary>
   class NetworkUpDown : EXT.IStatusProvider
   {

      #region Private Fields

      /// <summary>Network provider for upstream transmissions</summary>
      private EXT.NetworkProvider _up = new EXT.NetworkProvider("Bytes Sent/sec");
      /// <summary>Network provider for downstream transmissions</summary>
      private EXT.NetworkProvider _down = new EXT.NetworkProvider("Bytes Received/sec");
      /// <summary>Icon for the menu</summary>
      private readonly System.Drawing.Image _image = null;

      #endregion Private Fields

      #region Public Methods

      /// <summary>
      /// Constructor
      /// </summary>
      public NetworkUpDown()
      {
         Casey.Utility.ResourceManager rm = new Casey.Utility.ResourceManager();
         _image = rm.GetImageBySuffix("Images." + this.GetType().Name + ".png");
      }

      #endregion Public Methods

      #region IStatusProvider Members

      string EXT.IStatusProvider.Name
      {
         get { return "Network Up and Down"; }
      }

      string EXT.IStatusProvider.ShortName
      {
         get { return "Net Total"; }
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
         return _up.NextValue() + _down.NextValue();
      }

      #endregion

      #region IDisposable Members

      void System.IDisposable.Dispose()
      {
         _down.Dispose();
         _up.Dispose();
         _image.Dispose();
      }

      #endregion

   }
}
