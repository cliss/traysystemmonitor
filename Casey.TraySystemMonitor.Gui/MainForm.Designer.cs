namespace Casey.TraySystemMonitor.Gui
{
   partial class MainForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
            foreach (Casey.TraySystemMonitor.Ext.IStatusProvider provider in _providers)
            {
               provider.Dispose();
            }
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this._notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
         this._timer = new System.Windows.Forms.Timer(this.components);
         this.SuspendLayout();
         // 
         // _notifyIcon
         // 
         this._notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("_notifyIcon.Icon")));
         this._notifyIcon.Visible = true;
         this._notifyIcon.DoubleClick += new System.EventHandler(this.OnNotifyIconDoubleClick);
         // 
         // _timer
         // 
         this._timer.Interval = 1000;
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(129, 273);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
         this.Name = "MainForm";
         this.Opacity = 0;
         this.ShowInTaskbar = false;
         this.Text = "Form1";
         this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.NotifyIcon _notifyIcon;
      private System.Windows.Forms.Timer _timer;
   }
}

