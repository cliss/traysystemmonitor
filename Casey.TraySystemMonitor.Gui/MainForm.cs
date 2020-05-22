using System;
using System.Drawing;               // Graphics, SolidBrush, etc.
using System.Windows.Forms;         // Form, etc.
using System.Xml.Serialization;     // XmlSerializer

using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Gui
{

    /// <summary>
    /// The notify icon that is the main UI for this project
    /// </summary>
    public partial class MainForm : Form
    {

        #region Private Fields

        /// <summary>Name of the preferences file</summary>
        private const string PREFS_FILE = "prefs.xml";
        /// <summary>Number of ticks before a graph is decreased</summary>
        private const int FALL_TIMEOUT = 30;
        /// <summary>Minimum graph maximum</summary>
        private const int SMALLEST_MAXIMUM = 100;
        /// <summary>Preferences object</summary>
        private Preferences _prefs = null;
        /// <summary>Provider displaying on the left</summary>
        private EXT.IStatusProvider _left = null;
        /// <summary>Provider displaying on the right</summary>
        private EXT.IStatusProvider _right = null;
        /// <summary>List of all the known providers</summary>
        System.Collections.Generic.IList<EXT.IStatusProvider> _providers = null;
        /// <summary>Maximum value for the left side</summary>
        private int _leftMax = 100;
        /// <summary>Maximum value for the right side</summary>
        private int _rightMax = 100;
        /// <summary>Number of ticks the left is below the midline</summary>
        private int _leftBelowMidline = 0;
        /// <summary>Number of ticks the right is below the midline</summary>
        private int _rightBelowMidline = 0;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="providers">List of all the known providers</param>
        public MainForm(System.Collections.Generic.IList<EXT.IStatusProvider> providers)
        {
            InitializeComponent();

            _providers = providers;

            // Establish the preferences, either by reading the existing
            // preferences XML or by creating a new one.
            if (System.IO.File.Exists(PREFS_FILE))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Preferences));
                using (System.IO.StreamReader sr = new System.IO.StreamReader(PREFS_FILE))
                {
                    _prefs = (Preferences)ser.Deserialize(sr);
                }
            }
            else
            {
                _prefs = new Preferences();
                if (_providers.Count > 1)
                {
                    _prefs.LeftProvider = _providers[0].GetType().FullName;
                    _prefs.RightProvider = _providers[0].GetType().FullName;
                }
                else
                {
                    throw new ApplicationException("Could not find at least 2 status providers");
                }
            }

            // Find the two active providers
            FindActiveProviders();

            // Establish the context menu
            if (!_prefs.FancyMenu)
            {
                throw new NotSupportedException("Currently not supported in .NET Core");
                //_notifyIcon.ContextMenu = new ContextMenu();
                //_notifyIcon.ContextMenu.Popup += OnContextMenuPopUp;
            }
            else
            {
                _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
                _notifyIcon.ContextMenuStrip.ImageScalingSize = new Size(36, 36);
                _notifyIcon.ContextMenuStrip.Opening += OnContextMenuStripOpening;
            }

            // Set up the update timer
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        #endregion Public Methods

        #region Private Methods

        void OnContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _notifyIcon.ContextMenuStrip.Items.Clear();

            // Fill the available providers
            ToolStripMenuItem left = new ToolStripMenuItem("Left");
            ToolStripMenuItem right = new ToolStripMenuItem("Right");
            AddMenuItems(left, MenuTag.DisplaySide.Left, _left);
            AddMenuItems(right, MenuTag.DisplaySide.Right, _right);

            // Left/right sides and separator
            _notifyIcon.ContextMenuStrip.Items.Add(left);
            _notifyIcon.ContextMenuStrip.Items.Add(right);
            _notifyIcon.ContextMenuStrip.Items.Add("-");

            // Center line
            ToolStripMenuItem item = new ToolStripMenuItem("Center line", null, new EventHandler(OnCenterlineToolStripItemClick));
            item.Tag = _prefs.Centerline;
            if (_prefs.Centerline)
            {
                item.Font = new Font(item.Font, FontStyle.Bold);
            }
            _notifyIcon.ContextMenuStrip.Items.Add(item);
            _notifyIcon.ContextMenuStrip.Items.Add("-");

            // About/Exit
            _notifyIcon.ContextMenuStrip.Items.Add("About...", null, new EventHandler(OnAboutMenuClick));
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, new EventHandler(OnExitMenuClick));

            e.Cancel = false;
        }

        /// <summary>
        /// Handles a context menu opening by generating a new menu
        /// </summary>
        /// <param name="sender">Menu that sent popup</param>
        /// <param name="e">Event arguments</param>
        void OnContextMenuPopUp(object sender, EventArgs e)
        {
            throw new NotSupportedException("Currently not supported in .NET Core");
            //ContextMenu trayMenu = _notifyIcon.ContextMenu;
            //trayMenu.MenuItems.Clear();

            //// Fill the available providers
            //MenuItem left = new MenuItem("Left");
            //MenuItem right = new MenuItem("Right");
            //AddMenuItems(left, MenuTag.DisplaySide.Left, _left);
            //AddMenuItems(right, MenuTag.DisplaySide.Right, _right);

            //// Left/right sides and separator
            //trayMenu.MenuItems.Add(left);
            //trayMenu.MenuItems.Add(right);
            //trayMenu.MenuItems.Add("-");

            //// Center line
            //MenuItem center = new MenuItem("Center line", new EventHandler(OnCenterlineMenuClick));
            //center.Checked = _prefs.Centerline;
            //trayMenu.MenuItems.Add(center);
            //trayMenu.MenuItems.Add("-");

            //// About/Exit
            //trayMenu.MenuItems.Add(new MenuItem("&About", new EventHandler(OnAboutMenuClick)));
            //trayMenu.MenuItems.Add(new MenuItem("E&xit", new EventHandler(OnExitMenuClick)));
        }

        /// <summary>
        /// Handles a click on one of the provider menu items
        /// </summary>
        /// <param name="sender">Menu item that sent click</param>
        /// <param name="e">Event arguments</param>
        void OnProviderMenuItemClick(object sender, EventArgs e)
        {
            throw new NotSupportedException("Currently not supported in .NET Core");
            //MenuItem item = sender as MenuItem;
            //if (item != null)
            //{
            //    MenuTag tag = item.Tag as MenuTag;
            //    HandleProviderSwitch(tag);
            //}
        }

        /// <summary>
        /// Handles a click on one of the provider menu items
        /// </summary>
        /// <param name="sender">Menu item that sent click</param>
        /// <param name="e">Event arguments</param>
        void OnProviderToolStripItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                MenuTag tag = item.Tag as MenuTag;
                HandleProviderSwitch(tag);
            }
        }

        /// <summary>
        /// Handles a timer tick
        /// </summary>
        /// <param name="sender">Timer that sent tick</param>
        /// <param name="e">Event arguments</param>
        void OnTimerTick(object sender, EventArgs e)
        {
            int seconds = DateTime.Now.Second;
            float leftVal = _left.NextValue();
            float rightVal = _right.NextValue();

            // If the left is below the midline and not a percentage
            if (leftVal < _leftMax / 5 
                && _left.Type != EXT.StatusType.Percentage
                && (_leftMax / 10) > SMALLEST_MAXIMUM)
            {
                ++_leftBelowMidline;
                if (_leftBelowMidline > FALL_TIMEOUT)
                {
                    _leftMax /= 10;
                    System.Diagnostics.Trace.WriteLine("Decreasing left to " + _rightMax);
                    _leftBelowMidline = 0;
                }
            }
            else  // Otherwise we're over the midline
            {
                _leftBelowMidline = 0;
                // Increase graph maximum if need be.  The maximum
                // is increased tenfold if the value is at least
                // fivefold the current maximum.
                while (leftVal > _leftMax * 5)
                {
                    _leftMax *= 10;
                    System.Diagnostics.Trace.WriteLine("Increasing left to " + _leftMax.ToString());
                }
            }

            // If the right is below the midline and not a percentage
            if (rightVal < _rightMax / 5 
                && _right.Type != EXT.StatusType.Percentage
                && (_rightMax / 10) > SMALLEST_MAXIMUM)
            {
                ++_rightBelowMidline;
                if (_rightBelowMidline > FALL_TIMEOUT)
                {
                    _rightMax /= 10;
                    System.Diagnostics.Trace.WriteLine("Decreasing right to " + _rightMax);
                    _rightBelowMidline = 0;
                }
            }
            else  // Otherwise we're over the midline
            {
                _rightBelowMidline = 0;
                // Increase graph maximum if need be.  The maximum
                // is increased tenfold if the value is at least
                // fivefold the current maximum.
                while (rightVal > _rightMax * 5)
                {
                    _rightMax *= 10;
                    System.Diagnostics.Trace.WriteLine("Increasing right to " + _rightMax.ToString());
                }
            }

            GenerateTrayIcon(leftVal, rightVal);
            _notifyIcon.Text = string.Format(
               "{0}: {1}{2}\n{3}: {4}{5}",
               _left.ShortName,
               (int)leftVal,
               _left.Unit,
               _right.ShortName,
               (int)rightVal,
               _right.Unit);
        }

        /// <summary>
        /// Handles a click on the About menu item
        /// </summary>
        /// <param name="sender">Menu item that sent click</param>
        /// <param name="e">Event arguments</param>
        void OnAboutMenuClick(object sender, EventArgs e)
        {
            MessageBox.Show(
               "Tray System Monitor\nA small tool to help monitor system resources.\n\n" +
               "Copyright (C) 2007-2015 Casey Liss\ncasey@caseyliss.com\n\n" +
               "Some icons from http://commons.wikimedia.org/wiki/Crystal_Clear",
               "About Tray System Monitor",
               MessageBoxButtons.OK,
               MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles an exit menu item click
        /// </summary>
        /// <param name="sender">Menu item that sent click</param>
        /// <param name="e">Event arguments</param>
        void OnExitMenuClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles clicking on the centerline menu item
        /// </summary>
        /// <param name="sender">Menu item that sent click</param>
        /// <param name="e">Event arguments</param>
        void OnCenterlineMenuClick(object sender, EventArgs e)
        {
            throw new NotSupportedException("Currently not supported in .NET Core");
            //MenuItem item = sender as MenuItem;
            //if (item != null)
            //{
            //    // Note the Checked property hasn't changed yet.
            //    _prefs.Centerline = !item.Checked;
            //    UpdatePreferences();
            //}
        }

        /// <summary>
        /// Handles clicking on the centerline menu item
        /// </summary>
        /// <param name="sender">Menu item that sent click</param>
        /// <param name="e">Event arguments</param>
        void OnCenterlineToolStripItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                // Note the Checked property hasn't changed yet.
                _prefs.Centerline = !(bool)item.Tag;
                UpdatePreferences();
            }
        }

        ///// <summary>
        ///// Adds all the providers to the given menu
        ///// </summary>
        ///// <param name="root">Root menu item</param>
        ///// <param name="side">Which side we're on</param>
        ///// <param name="current">Current status provider on this side</param>
        //void AddMenuItems(
        //   MenuItem root,
        //   MenuTag.DisplaySide side,
        //   EXT.IStatusProvider current)
        //{
        //    foreach (EXT.IStatusProvider provider in _providers)
        //    {
        //        MenuItem item = new MenuItem(provider.Name);
        //        item.Tag = new MenuTag(side, provider);
        //        item.Checked = ReferenceEquals(current, provider);
        //        item.Enabled = !ReferenceEquals(current, provider);
        //        item.Click += OnProviderMenuItemClick;

        //        root.MenuItems.Add(item);
        //    }
        //}

        /// <summary>
        /// Adds all the providers to the given menu
        /// </summary>
        /// <param name="root">Root menu item</param>
        /// <param name="side">Which side we're on</param>
        /// <param name="current">Current status provider on this side</param>
        void AddMenuItems(
           ToolStripMenuItem root,
           MenuTag.DisplaySide side,
           EXT.IStatusProvider current)
        {
            foreach (EXT.IStatusProvider provider in _providers)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(provider.Name, provider.Image);
                item.Tag = new MenuTag(side, provider);
                item.Checked = ReferenceEquals(current, provider);
                item.Enabled = !item.Checked;
                item.Click += OnProviderToolStripItemClick;

                root.DropDownItems.Add(item);
            }
        }

        /// <summary>
        /// Generates the tray icon based off passed values
        /// </summary>
        /// <param name="leftValue">Value of the left provider</param>
        /// <param name="rightValue">Value of the right provider</param>
        void GenerateTrayIcon(float leftValue, float rightValue)
        {
            Bitmap bmp = new Bitmap(_notifyIcon.Icon.Width, _notifyIcon.Icon.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(_prefs.ForeColor()))
            {
                RectangleF bounds = g.VisibleClipBounds;
                g.Clear(_prefs.BackColor());
                float leftHeight = (leftValue / _leftMax) * bounds.Height;
                float rightHeight = (rightValue / _rightMax) * bounds.Height;
                int centerOffset = _prefs.Centerline ? 1 : 0;
                // Draw the left graph
                g.FillRectangle(
                   brush,
                   0,
                   bounds.Height - leftHeight,
                   bounds.Width / 2 - centerOffset,
                   leftHeight);
                // Draw the right graph
                g.FillRectangle(
                   brush,
                   bounds.Width / 2 + 1 + centerOffset,
                   bounds.Height - rightHeight,
                   bounds.Width / 2,
                   rightHeight);
            }

            // Save off the old icon, replace it with the new one,
            // and then destroy the old one.
            IntPtr oldIcon = _notifyIcon.Icon.Handle;
            _notifyIcon.Icon = Icon.FromHandle(bmp.GetHicon());
            DestroyIcon(oldIcon);
        }

        /// <summary>
        /// Stores the preferences object to disk.
        /// </summary>
        void UpdatePreferences()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Preferences));
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(PREFS_FILE))
                {
                    ser.Serialize(sw, _prefs);
                }
            }
        }

        /// <summary>
        /// Finds the active providers based on the preferences object.
        /// </summary>
        void FindActiveProviders()
        {
            foreach (EXT.IStatusProvider provider in _providers)
            {
                if (provider.GetType().FullName == _prefs.LeftProvider)
                {
                    _left = provider;
                }
                if (provider.GetType().FullName == _prefs.RightProvider)
                {
                    _right = provider;
                }

            }

            // Handle the case of a provider missing
            if (_left == null)
            {
                _left = _providers[0];
            }
            if (_right == null)
            {
                _right = _providers[1];
            }
        }

        /// <summary>
        /// Handles a double-click of the tray icon
        /// </summary>
        /// <param name="sender">Object that sent doubleclick</param>
        /// <param name="e">Event arguments</param>
        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process taskMan = null;
            try
            {
                taskMan = new System.Diagnostics.Process();
                taskMan.StartInfo = new System.Diagnostics.ProcessStartInfo("taskmgr.exe");
                taskMan.Start();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Could not start task manager: " + ex.Message);
            }
            finally
            {
                if (taskMan == null)
                {
                    taskMan.Dispose();
                }
            }
        }

        /// <summary>
        /// Handles the switch of a provider
        /// </summary>
        /// <param name="tag">Information about the requested new provider</param>
        void HandleProviderSwitch(MenuTag tag)
        {
            if (tag.Side == MenuTag.DisplaySide.Left)
            {
                _left = tag.Provider;
                _leftMax = _left.Type == EXT.StatusType.Percentage ?
                   100 : 10;
                _prefs.LeftProvider = _left.GetType().FullName;
            }
            else
            {
                _right = tag.Provider;
                _rightMax = _right.Type == EXT.StatusType.Percentage ?
                   100 : 10;
                _prefs.RightProvider = _right.GetType().FullName;
            }

            UpdatePreferences();
        }

        /// <summary>
        /// Destroys an icon.  Used to release GDI+ resources.
        /// </summary>
        /// <param name="hIcon">Handle to the icon to be destroyed</param>
        /// <returns>Success flag</returns>
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        #endregion Private Methods

        #region Private Classes

        /// <summary>
        /// Defines a tag for a menu item
        /// </summary>
        private class MenuTag
        {
            public enum DisplaySide
            {
                Left = 0,
                Right
            }
            private DisplaySide _side;
            private EXT.IStatusProvider _provider;

            public MenuTag(DisplaySide side, EXT.IStatusProvider provider)
            {
                _side = side;
                _provider = provider;
            }

            public DisplaySide Side
            {
                get { return _side; }
            }

            public EXT.IStatusProvider Provider
            {
                get { return _provider; }
            }
        }

        #endregion Private Classes

    }
}
