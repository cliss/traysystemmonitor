using System.Collections.Generic;   // IList, List
using System.Reflection;            // Assembly
using System.IO;
// Directory

using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{

   /// <summary>
   /// Utility class to find and load all available
   /// Status Providers in the current directory.
   /// </summary>
   public static class Loader
   {

      /// <summary>
      /// Finds all the available providers in the
      /// current directory, instantiates them, adds
      /// them to a list, and returns them.
      /// </summary>
      /// <returns>List of status providers in the 
      /// current directory.</returns>
      public static IList<EXT.IStatusProvider> FindAllProviders()
      {
         List<EXT.IStatusProvider> retVal = new List<EXT.IStatusProvider>();
         List<string> files = new List<string>();
         files.AddRange(Directory.GetFiles(".", "*.dll"));
         files.AddRange(Directory.GetFiles(".", "*.exe"));
         // Try every file
         foreach (string file in files)
         {
             try
             {
                 // Load the assembly.  Try each type
                 Assembly a = Assembly.LoadFile(Path.GetFullPath(file));
                 foreach (System.Type t in a.GetTypes())
                 {
                     // See if this type is a status provider
                     System.Type iFace = t.GetInterface(typeof (EXT.IStatusProvider).Name);
                     if (iFace != null)
                     {
                         // Instantiate it and add it to the list
                         EXT.IStatusProvider plugin =
                             a.CreateInstance(t.FullName) as EXT.IStatusProvider;
                         retVal.Add(plugin);
                     }
                 }
             }
             catch
             {
                 // Silence load errors.
             }
         }

         return retVal; 
      }

   }
}
