using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;

using Microsoft.Win32;

using NAudio.CoreAudioApi;
using NAudio.Wave;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace AudioControlLibrary
{
    public partial class uc_AudioControl: UserControl
    {
        

        public uc_AudioControl()
        {
            InitializeComponent();
        }
    }

    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control">the control for which the update is required</param>
        /// <param name="action">action to be performed on the control</param>
        public static void InvokeOnUiThreadIfRequired(this Control control, Action action)
        {
            //If you are planning on using a similar function in your own code then please be sure to
            //have a quick read over https://stackoverflow.com/questions/1874728/avoid-calling-invoke-when-the-control-is-disposed
            //No action
            if (control.Disposing || control.IsDisposed || !control.IsHandleCreated)
            {
                return;
            }

            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        public static void WPFInvokeOnUiThreadIfRequired(this System.Windows.Controls.Control control, Action action)
        {
            if (!control.Dispatcher.CheckAccess())
            {
                control.Dispatcher.BeginInvoke(
                  System.Windows.Threading.DispatcherPriority.Normal,
                  new Action(
                    delegate ()
                    {
                        action();
                    }
                ));
            }
            else
            {
                action.Invoke();
            }
        }
    }
}
