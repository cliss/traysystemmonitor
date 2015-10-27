using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Casey.TraySystemMonitor.Gui
{

   /// <summary>
   /// Preferences object for this application
   /// </summary>
   [Serializable]
   public class Preferences
   {

      #region Private Fields

      private int _backR = 102;
      private int _backG = 0;
      private int _backB = 0;
      private int _foreR = 255;
      private int _foreG = 102;
      private int _foreB = 0;
      private string _leftProvider = null;
      private string _rightProvider = null;
      private bool _drawCenterline = false;
      private bool _fancyMenu = true;

      #endregion Private Fields

      #region Public Properties

      /// <summary>
      /// Gets or sets the red potion of the background color
      /// </summary>
      public int BackRed
      {
         get { return _backR; }
         set { _backR = value; }
      }

      /// <summary>
      /// Gets or sets the green portion of the background color
      /// </summary>
      public int BackGreen
      {
         get { return _backG; }
         set { _backG = value; }
      }

      /// <summary>
      /// Gets or sets the blue portion of the background color
      /// </summary>
      public int BackBlue
      {
         get { return _backB; }
         set { _backB = value; }
      }

      /// <summary>
      /// Gets or sets the red portion of the foreground color
      /// </summary>
      public int ForeRed
      {
         get { return _foreR; }
         set { _foreR = value; }
      }

      /// <summary>
      /// Gets or sets the green portion of the foreground color
      /// </summary>
      public int ForeGreen
      {
         get { return _foreG; }
         set { _foreG = value; }
      }

      /// <summary>
      /// Gets or sets the green portion of the foreground color
      /// </summary>
      public int ForeBlue
      {
         get { return _foreB; }
         set { _foreB = value; }
      }

      /// <summary>
      /// Gets or sets the full type name of the left provider
      /// </summary>
      public string LeftProvider
      {
         get { return _leftProvider; }
         set { _leftProvider = value; }
      }

      /// <summary>
      /// Gets or sets the full type name of the right provider
      /// </summary>
      public string RightProvider
      {
         get { return _rightProvider; }
         set { _rightProvider = value; }
      }

      /// <summary>
      /// Gets or sets the flag to draw a centerline
      /// </summary>
      public bool Centerline
      {
         get { return _drawCenterline; }
         set { _drawCenterline = value; }
      }

      /// <summary>
      /// Gets or sets the flag to draw a fancy menu
      /// </summary>
      public bool FancyMenu
      {
         get { return _fancyMenu; }
         set { _fancyMenu = value; }
      }

      #endregion Public Properties

      #region Public Methods

      /// <summary>
      /// Gets the background color based on the stored RGB values
      /// </summary>
      /// <returns>Background color</returns>
      public Color BackColor()
      {
         return Color.FromArgb(_backR, _backG, _backB);
      }

      /// <summary>
      /// Gets the foreground color based on the stored RGB values
      /// </summary>
      /// <returns>Foreground color</returns>
      public Color ForeColor()
      {
         return Color.FromArgb(_foreR, _foreG, _foreB);
      }

      #endregion Public Methods

   }
}
