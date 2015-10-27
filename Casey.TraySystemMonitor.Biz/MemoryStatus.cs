using System.Runtime.InteropServices;

using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{

   /// <summary>
   /// MemoryStatus provides the percentage 
   /// of total physical memory used
   /// </summary>
   class MemoryStatus : EXT.IStatusProvider
   {

      #region Private Types

      /// <summary>
      /// The MEMORYSTATUS structure contains information about 
      /// the current state of both physical and virtual memory.
      /// </summary>
      /// <remarks>Definition courtesy of http://www.pinvoke.net</remarks>
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
      private class MEMORYSTATUS
      {
         /// <summary>
         /// Size of the MEMORYSTATUS data structure, in bytes. 
         /// You do not need to set this member before calling 
         /// the GlobalMemoryStatus function; the function sets it.
         /// </summary>
         public uint dwLength;

         /// <summary>
         /// Number between 0 and 100 that specifies the approximate 
         /// percentage of physical memory that is in use (0 indicates 
         /// no memory use and 100 indicates full memory use).
         /// Windows NT:  Percentage of approximately the last 1000 
         /// pages of physical memory that is in use.
         /// </summary>
         public uint dwMemoryLoad;

         /// <summary>
         /// Total size of physical memory, in bytes.
         /// </summary>
         public uint dwTotalPhys;

         /// <summary>
         /// Size of physical memory available, in bytes
         /// </summary>
         public uint dwAvailPhys;

         /// <summary>
         /// Size of the committed memory limit, in bytes.
         /// </summary>
         public uint dwTotalPageFile;

         /// <summary>
         /// Size of available memory to commit, in bytes.
         /// </summary>
         public uint dwAvailPageFile;

         /// <summary>
         /// Total size of the user mode portion of the virtual 
         /// address space of the calling process, in bytes.
         /// </summary>
         public uint dwTotalVirtual;

         /// <summary>
         /// Size of unreserved and uncommitted memory in the user 
         /// mode portion of the virtual address space of the calling 
         /// process, in bytes.
         /// </summary>
         public uint dwAvailVirtual;

      }

      #endregion Private Types

      #region Private Fields

      private readonly System.Drawing.Image _image = null;

      #endregion Private Fields

      #region Public Methods

      /// <summary>
      /// Constructor
      /// </summary>
      public MemoryStatus()
      {
         Casey.Utility.ResourceManager rm = new Casey.Utility.ResourceManager();
         _image = rm.GetImageBySuffix("Images." + this.GetType().Name + ".png");
      }

      #endregion Public Methods

      #region Private Methods

      /// <summary>
      /// Gets the global memory status by filling in 
      /// the provided MEMORYSTATUS object.
      /// </summary>
      /// <param name="lpBuffer"></param>
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern void GlobalMemoryStatus([In, Out] MEMORYSTATUS lpBuffer);

      #endregion Private Methods

      #region IStatusProvider Members

      string EXT.IStatusProvider.Name
      {
         get { return "Memory Usage"; }
      }

      string EXT.IStatusProvider.ShortName
      {
         get { return "Mem"; }
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
         MEMORYSTATUS status = new MEMORYSTATUS();
         GlobalMemoryStatus(status);
         return (float)status.dwMemoryLoad;
      }

      #endregion

      #region IDisposable Members

      void System.IDisposable.Dispose()
      {
         _image.Dispose();
      }

      #endregion

   }
}
