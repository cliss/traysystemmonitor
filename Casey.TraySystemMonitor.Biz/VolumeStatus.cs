using System;
using System.Runtime.InteropServices;     // DllImport, CharSet, etc.

using EXT = Casey.TraySystemMonitor.Ext;

namespace Casey.TraySystemMonitor.Biz
{

   /// <summary>
   /// Gets the current volume as a percentage.
   /// </summary>
   /// <remarks>Some code by 
   /// http://www.csharp-home.com/index/tiki-read_article.php?articleId=134</remarks>
   class VolumeStatus : EXT.IStatusProvider
   {

      #region Private Fields

      private readonly System.Drawing.Image _image = null;

      #endregion Private Fields

      #region Public Methods

      /// <summary>
      /// Constructor
      /// </summary>
      public VolumeStatus()
      {
         Casey.Utility.ResourceManager rm = new Casey.Utility.ResourceManager();
         _image = rm.GetImageBySuffix("Images." + this.GetType().Name + ".png");
      }

      #endregion Public Methods

      #region IStatusProvider Members

      string Casey.TraySystemMonitor.Ext.IStatusProvider.Name
      {
         get { return "Volume"; }
      }

      string Casey.TraySystemMonitor.Ext.IStatusProvider.ShortName
      {
         get { return "Vol"; }
      }

      string Casey.TraySystemMonitor.Ext.IStatusProvider.Unit
      {
         get { return "%"; }
      }

      Casey.TraySystemMonitor.Ext.StatusType Casey.TraySystemMonitor.Ext.IStatusProvider.Type
      {
         get { return EXT.StatusType.Percentage; }
      }

      System.Drawing.Image EXT.IStatusProvider.Image
      {
         get { return _image; }
      }

      float Casey.TraySystemMonitor.Ext.IStatusProvider.NextValue()
      {
         return AudioMixerHelper.GetVolume();
      }

      #endregion

      #region IDisposable Members

      void IDisposable.Dispose()
      {
         _image.Dispose();
      }

      #endregion

      #region Private Classes

      private class AudioMixerHelper
      {
         private const int MMSYSERR_NOERROR = 0;
         private const int MAXPNAMELEN = 32;
         private const int MIXER_LONG_NAME_CHARS = 64;
         private const int MIXER_SHORT_NAME_CHARS = 16;
         private const int MIXER_GETLINEINFOF_COMPONENTTYPE = 0x3;
         private const int MIXER_GETCONTROLDETAILSF_VALUE = 0x0;
         private const int MIXER_GETLINECONTROLSF_ONEBYTYPE = 0x2;
         private const int MIXER_SETCONTROLDETAILSF_VALUE = 0x0;
         private const int MIXERLINE_COMPONENTTYPE_DST_FIRST = 0x0;
         private const int MIXERLINE_COMPONENTTYPE_SRC_FIRST = 0x1000;
         private const int MIXERLINE_COMPONENTTYPE_DST_SPEAKERS = (MIXERLINE_COMPONENTTYPE_DST_FIRST + 4);
         private const int MIXERLINE_COMPONENTTYPE_SRC_MICROPHONE = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 3);
         private const int MIXERLINE_COMPONENTTYPE_SRC_LINE = (MIXERLINE_COMPONENTTYPE_SRC_FIRST + 2);
         private const int MIXERCONTROL_CT_CLASS_FADER = 0x50000000;
         private const int MIXERCONTROL_CT_UNITS_UNSIGNED = 0x30000;
         private const int MIXERCONTROL_CONTROLTYPE_FADER = (MIXERCONTROL_CT_CLASS_FADER | MIXERCONTROL_CT_UNITS_UNSIGNED);
         private const int MIXERCONTROL_CONTROLTYPE_VOLUME = (MIXERCONTROL_CONTROLTYPE_FADER + 1);

         [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
         private static extern int mixerClose(int hmx);

         [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
         private static extern int mixerGetControlDetailsA(
            int hmxobj,
            ref MIXERCONTROLDETAILS pmxcd,
            int fdwDetails);

         [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
         private static extern int mixerGetLineControlsA(
            int hmxobj,
            ref MIXERLINECONTROLS pmxlc,
            int fdwControls);

         [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
         private static extern int mixerGetLineInfoA(
            int hmxobj,
            ref MIXERLINE pmxl,
            int fdwInfo);

         [DllImport("winmm.dll", CharSet = CharSet.Ansi)]
         private static extern int mixerOpen(
            out int phmx,
            int uMxId,
            int dwCallback,
            int dwInstance,
            int fdwOpen);

         private struct MIXERCONTROL
         {

            public int cbStruct;
            public int dwControlID;
            public int dwControlType;
            public int fdwControl;
            public int cMultipleItems;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_SHORT_NAME_CHARS)]
            public string szShortName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
            public string szName;
            public int lMinimum;
            public int lMaximum;
            [MarshalAs(UnmanagedType.U4, SizeConst = 10)]
            public int reserved;
         }

         private struct MIXERCONTROLDETAILS
         {
            public int cbStruct;
            public int dwControlID;
            public int cChannels;
            public int item;
            public int cbDetails;
            public IntPtr paDetails;
         }

         private struct MIXERCONTROLDETAILS_UNSIGNED
         {
            public int dwValue;
         }

         private struct MIXERLINE
         {
            public int cbStruct;
            public int dwDestination;
            public int dwSource;
            public int dwLineID;
            public int fdwLine;
            public int dwUser;
            public int dwComponentType;
            public int cChannels;
            public int cConnections;
            public int cControls;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_SHORT_NAME_CHARS)]
            public string szShortName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MIXER_LONG_NAME_CHARS)]
            public string szName;
            public int dwType;
            public int dwDeviceID;
            public int wMid;
            public int wPid;
            public int vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szPname;
         }

         private struct MIXERLINECONTROLS
         {
            public int cbStruct;
            public int dwLineID;
            public int dwControl;
            public int cControls;
            public int cbmxctrl;
            public IntPtr pamxctrl;
         }

         private static bool GetVolumeControl(
            int hmixer,
            int componentType,
            int ctrlType,
            out MIXERCONTROL mxc,
            out int vCurrentVol)
         {
            // This function attempts to obtain a mixer control.
            // Returns True if successful.
            MIXERLINECONTROLS mxlc = new MIXERLINECONTROLS();
            MIXERLINE mxl = new MIXERLINE();
            MIXERCONTROLDETAILS pmxcd = new MIXERCONTROLDETAILS();
            MIXERCONTROLDETAILS_UNSIGNED du = new
            MIXERCONTROLDETAILS_UNSIGNED();
            mxc = new MIXERCONTROL();
            int rc;
            bool retValue;
            vCurrentVol = -1;

            mxl.cbStruct = Marshal.SizeOf(mxl);
            mxl.dwComponentType = componentType;

            rc = mixerGetLineInfoA(hmixer, ref mxl,
            MIXER_GETLINEINFOF_COMPONENTTYPE);

            if (MMSYSERR_NOERROR == rc)
            {
               int sizeofMIXERCONTROL = 152;
               int ctrl = Marshal.SizeOf(typeof(MIXERCONTROL));
               mxlc.pamxctrl = Marshal.AllocCoTaskMem(sizeofMIXERCONTROL);
               mxlc.cbStruct = Marshal.SizeOf(mxlc);
               mxlc.dwLineID = mxl.dwLineID;
               mxlc.dwControl = ctrlType;
               mxlc.cControls = 1;
               mxlc.cbmxctrl = sizeofMIXERCONTROL;

               // Allocate a buffer for the control
               mxc.cbStruct = sizeofMIXERCONTROL;

               // Get the control
               rc = mixerGetLineControlsA(hmixer, ref mxlc, MIXER_GETLINECONTROLSF_ONEBYTYPE);

               if (MMSYSERR_NOERROR == rc)
               {
                  retValue = true;
                  // Copy the control into the destination structure
                  mxc = (MIXERCONTROL)Marshal.PtrToStructure(mxlc.pamxctrl, typeof(MIXERCONTROL));
               }
               else
               {
                  retValue = false;
               }

               int sizeofMIXERCONTROLDETAILS = Marshal.SizeOf(typeof(MIXERCONTROLDETAILS));

               int sizeofMIXERCONTROLDETAILS_UNSIGNED = Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_UNSIGNED));
               pmxcd.cbStruct = sizeofMIXERCONTROLDETAILS;
               pmxcd.dwControlID = mxc.dwControlID;
               pmxcd.paDetails = Marshal.AllocCoTaskMem(sizeofMIXERCONTROLDETAILS_UNSIGNED);
               pmxcd.cChannels = 1;
               pmxcd.item = 0;
               pmxcd.cbDetails = sizeofMIXERCONTROLDETAILS_UNSIGNED;

               rc = mixerGetControlDetailsA(hmixer, ref pmxcd, MIXER_GETCONTROLDETAILSF_VALUE);
               du = (MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(pmxcd.paDetails, typeof(MIXERCONTROLDETAILS_UNSIGNED));
               vCurrentVol = du.dwValue;
               return retValue;
            }

            retValue = false;
            return retValue;
         }

         public static float GetVolume()
         {
            int mixer;
            MIXERCONTROL volCtrl = new MIXERCONTROL();
            int currentVol;
            mixerOpen(out mixer, 0, 0, 0, 0);
            int type = MIXERCONTROL_CONTROLTYPE_VOLUME;
            GetVolumeControl(
               mixer,
               MIXERLINE_COMPONENTTYPE_DST_SPEAKERS,
               type,
               out volCtrl,
               out currentVol);
            mixerClose(mixer);
            return (float)currentVol / (float)volCtrl.lMaximum * 100.0f;
         }

      }

      #endregion Private Classes

   }
}
