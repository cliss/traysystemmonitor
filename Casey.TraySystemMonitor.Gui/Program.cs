using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Casey.TraySystemMonitor.Gui
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         try
         {
            Application.Run(new MainForm(Biz.Loader.FindAllProviders()));
         }
         catch (Exception e)
         {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("error.log"))
            {
               sw.Write(e.ToString());
            }
            MessageBox.Show(
               "There has been an error:\n" + e.ToString(),
               "Error",
               MessageBoxButtons.OK,
               MessageBoxIcon.Error);
         }
      }
   }
}