using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Forms.VisualStyles;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;

using Microsoft.Win32;

using NAudio.CoreAudioApi;
using NAudio.Wave;


namespace AudioControl_V2
{
    public partial class Form1 : Form
    {
        List<CoreAudioDevice> AudioDevices = null; // MicroPhone Device 리스트
        CoreAudioDevice defaultMicDeivce = null; // 현재 MicroPhone Device
        CoreAudioController coreAudioController = null; // MicroPhone Device 컨트롤러

        List<IDisposable> disposables = null; // 옵저버 패턴
        Action<DeviceVolumeChangedArgs> actionVolumeChange = null; // 볼륨 변경
        Action<DeviceMuteChangedArgs> actionMuteChange = null; // 음소거 변경
        Action<DefaultDeviceChangedArgs> actionDefalutDeviceChange = null; // 기본 장치 변경 
        Action<DeviceStateChangedArgs> actionStateDevice = null; // 상태 변경

        // 마이크 스펙트럼
        MMDeviceEnumerator mMDeviceEnumerator = null;
        WaveInEvent waveIn = null;
        bool gbIsStopWave = false;
        private static double audioValueMax = 0;
        private static double audioValueLast = 0;
        private static int RATE = 44100;
        private static int BUFFER_SAMPLES = 1024;


        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
            cbb_MicrophoneList.SelectedIndexChanged += Cbb_MicrophoneList_SelectedIndexChanged; // 콤보박스 아이템 변경 이벤트 추가
            tb_VoluneControl.Scroll += tb_VoluneControl_Scroll;
            pb_Microphone.Click += pb_Microphone_Click;

            //userControl31.Value = 0;
            //userControl21.Slider.ValueChanged += Slider_ValueChanged;
        }

        private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (defaultMicDeivce != null)
                defaultMicDeivce.Volume = (int)e.NewValue;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pb_Microphone.Cursor = Cursors.Hand;

            cbb_MicrophoneList.Cursor = Cursors.Hand;
            cbb_MicrophoneList.DropDownWidth = this.Width - 40;
            cbb_MicrophoneList.ItemHeight = 20;

            tb_VoluneControl.Cursor = Cursors.Hand;
            tb_VoluneControl.Maximum = 100; // 기본 트랙바 max는 10

            pb_ForegroundImage.Width = 0; // 마이크 사운드 볼륨 이미지 길이 0으로 설정

            actionVolumeChange = OnVolumeChanged; // 볼륨 변경
            actionMuteChange = OnMuteChanged; // 음소거 변경
            actionDefalutDeviceChange = OnDefaultChanged; // 기본 장치 변경 
            actionStateDevice = OnStateChanged; // 상태 변경

            disposables = new List<IDisposable>(); // 옵저버 리스트
            mMDeviceEnumerator = new MMDeviceEnumerator(); // 마이크 에뮬레이터
            coreAudioController = new CoreAudioController(); // 마이크 스위처

            GetDevices();
            SetUI();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (defaultMicDeivce != null)
                defaultMicDeivce.Dispose();
            if (coreAudioController != null)
                coreAudioController.Dispose();
            if (mMDeviceEnumerator != null)
                mMDeviceEnumerator.Dispose();
            if (waveIn != null)
                waveIn.Dispose();
        }

        private void GetDevices()
        {
            if (AudioDevices == null)
                AudioDevices = new List<CoreAudioDevice>();
            else
                AudioDevices.Clear();

            // UnKnown 장비 제외하고 리스트에 추가            
            foreach (CoreAudioDevice coreAudioDevice in coreAudioController.GetCaptureDevices().Where(coreAudioDevice => !coreAudioDevice.FullName.Contains("Unknown") && coreAudioDevice.State.Equals(AudioSwitcher.AudioApi.DeviceState.Active)))
                AudioDevices.Add(coreAudioDevice);

            // 1개라도 있을 때만
            if (AudioDevices.Count > 0)
            {
                ObservableClear();
                // 현재 선택된 녹음 장치 가져오기
                defaultMicDeivce = coreAudioController.DefaultCaptureDevice;
                for (int i = 0; i < AudioDevices.Count; i++)
                {
                    disposables.Add(ObservableExtensions.Subscribe(AudioDevices[i].VolumeChanged, actionVolumeChange));
                    disposables.Add(ObservableExtensions.Subscribe(AudioDevices[i].MuteChanged, actionMuteChange));
                    disposables.Add(ObservableExtensions.Subscribe(AudioDevices[i].DefaultChanged, actionDefalutDeviceChange));
                    disposables.Add(ObservableExtensions.Subscribe(AudioDevices[i].StateChanged, actionStateDevice));
                }
            }
        }

        private void SetUI()
        {
            try
            {
                lock (cbb_MicrophoneList)
                {
                    this.InvokeOnUiThreadIfRequired(() => cbb_MicrophoneList.Items.Clear());
                    //기본 선택된 녹음 장치가 있으면 트랙바 볼륨 설정.
                    if (AudioDevices != null && AudioDevices.Count > 0 && defaultMicDeivce != null)
                    {
                        // ((ToolStripMenuItem)cms_MicrophoneList.Items[0]).Checked = true;
                        Action<int> actAddList = (count) => {
                            for (int i = 0; i < count; i++)
                                cbb_MicrophoneList.Items.Add(AudioDevices[i].FullName);

                            cbb_MicrophoneList.SelectedIndex = AudioDevices.IndexOf(defaultMicDeivce);                            
                        };

                        this.InvokeOnUiThreadIfRequired(() => actAddList(AudioDevices.Count));

                        if (waveIn == null)
                        {
                            int? devicenum = GetNAudioDeviceNumver();
                            if (devicenum != null)
                            {
                                waveIn = new WaveInEvent();
                                waveIn.DeviceNumber = (int)devicenum;
                                waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
                                waveIn.BufferMilliseconds = (int)((double)BUFFER_SAMPLES / (double)RATE * 1000.0);
                                waveIn.DataAvailable += WaveIn_DataAvailable;
                                waveIn.RecordingStopped += (sender, e) => gbIsStopWave = false; pb_ForegroundImage.Width = 0;
                            }
                        }

                        if ((int)defaultMicDeivce.Volume > -1)
                        {
                            this.InvokeOnUiThreadIfRequired(() => tb_VoluneControl.Value = (int)defaultMicDeivce.Volume);
                            //this.InvokeOnUiThreadIfRequired(() => userControl21.Slider.Value = (int)defaultMicDeivce.Volume);
                            
                        }

                        if (defaultMicDeivce.IsMuted)
                        {
                            this.InvokeOnUiThreadIfRequired(() => pb_Microphone.BackColor = Color.Red); // 실제 적용시 이미지 변경                        
                            if (waveIn != null)
                                waveIn.StopRecording();
                        }
                        else
                        {
                            this.InvokeOnUiThreadIfRequired(() => pb_Microphone.BackColor = Color.Green); // 실제 적용시 이미지 변경                        
                            if (waveIn != null && !gbIsStopWave)
                            {
                                waveIn.StartRecording();
                                gbIsStopWave = true;
                            }
                        }
                    }
                    else
                    {                        
                        cbb_MicrophoneList.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception e)
            {
                textBox1.AppendText(e.ToString() + Environment.NewLine);
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            float max = 0;
            // interpret as 16 bit audio
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index + 0]);
                // to floating point
                var sample32 = sample / 32768f;
                // absolute value 
                if (sample32 < 0) sample32 = -sample32;
                // is this the max value?
                if (sample32 > max) max = sample32;
            }
            // calculate what fraction this peak is of previous peaks
            if (max > audioValueMax)
                audioValueMax = (double)max;

            audioValueLast = max;

            double frac = audioValueLast / audioValueMax;
            this.InvokeOnUiThreadIfRequired(() => pb_ForegroundImage.Width = (int)(frac * pb_BackgroudImage.Width));
            //this.InvokeOnUiThreadIfRequired(() => userControl31.Value = (int)(frac * pb_BackgroudImage.Width));
        }

        // 콤보박스 아이템 변경시 기본 장치 변경
        private void Cbb_MicrophoneList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbb_MicrophoneList.SelectedIndex > -1)
            {                
                if (!defaultMicDeivce.FullName.Equals(cbb_MicrophoneList.Text))
                {
                    CoreAudioDevice coreAudioDevice = AudioDevices.Find(x => x.FullName.Equals(cbb_MicrophoneList.Text));

                    if (coreAudioDevice != null)
                        coreAudioController.SetDefaultDevice(coreAudioDevice);
                }
            }
            
        }

        // 볼륨 조절 옵저버
        private void OnVolumeChanged(DeviceVolumeChangedArgs deviceVolumeChangedArgs)
        {
            if (deviceVolumeChangedArgs.Device.FullName.Equals(defaultMicDeivce.FullName))
            {
                this.InvokeOnUiThreadIfRequired(() => tb_VoluneControl.Value = (int)deviceVolumeChangedArgs.Volume);
                //this.InvokeOnUiThreadIfRequired(() => userControl21.Slider.Value = (int)deviceVolumeChangedArgs.Volume);
            }
        }

        // 음소거 옵저버
        private void OnMuteChanged(DeviceMuteChangedArgs deviceMuteChangedArgs)
        {
            if (deviceMuteChangedArgs.Device.FullName.Equals(defaultMicDeivce.FullName))
            {
                // TODO : 이미지로 변경
                if (deviceMuteChangedArgs.IsMuted)
                    this.InvokeOnUiThreadIfRequired(() => pb_Microphone.BackColor = Color.Red); // 실제 적용시 이미지 변경
                else
                    this.InvokeOnUiThreadIfRequired(() => pb_Microphone.BackColor = Color.Green); // 실제 적용시 이미지 변경
            }
        }

        // 기본 장치 변경 옵저버
        private void OnDefaultChanged(DefaultDeviceChangedArgs defaultDeviceChangedArgs)
        {
            if (((CoreAudioDevice)defaultDeviceChangedArgs.Device).IsDefaultDevice)
            {
                if (waveIn != null)
                {
                    waveIn.StopRecording();
                    waveIn.DataAvailable -= WaveIn_DataAvailable;
                    waveIn = null;
                }
                GetDevices();
                SetUI();
            }
        }

        // 상태 변경 옵저버 ex) 사용 -> 사용 안 함
        private void OnStateChanged(DeviceStateChangedArgs deviceStateChangedArgs)
        {
            // TODO : 메뉴 리스트 초기화
            // 1. Device State에 따라 메뉴 리스트에서 enable과 active시 추가
            this.InvokeOnUiThreadIfRequired(() => pb_ForegroundImage.Width = 0);
            if (((CoreAudioDevice)deviceStateChangedArgs.Device).IsDefaultDevice)
            {
                if (waveIn != null)
                {
                    waveIn.StopRecording();
                    waveIn.DataAvailable -= WaveIn_DataAvailable;
                    waveIn = null;
                }
            }
            GetDevices();
            SetUI();
        }

        // 트랙바로 마이크 사운드 조절
        private void tb_VoluneControl_Scroll(object sender, EventArgs e)
        {
            if (defaultMicDeivce != null)
                defaultMicDeivce.Volume = tb_VoluneControl.Value;
        }

        // 이미지 클릭시 음소거, 음소거 해제
        private void pb_Microphone_Click(object sender, EventArgs e)
        {
            if (defaultMicDeivce != null)
                defaultMicDeivce.Mute(!defaultMicDeivce.IsMuted);
        }

        private int? GetNAudioDeviceNumver()
        {
            MMDevice defalutMicDevice = mMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, NAudio.CoreAudioApi.Role.Multimedia);
            int? deviceIndex = 0;
            if (defalutMicDevice != null)
            {
                for (int i = 0; i < WaveIn.DeviceCount; i++)
                {
                    if (defalutMicDevice.FriendlyName.Equals(WaveIn.GetCapabilities(i).ProductName))
                    {
                        deviceIndex = i;
                        break;
                    }
                }
            }
            else
                return null;

            return deviceIndex;
        }

        private void SetRegistrykey(string microPhoneName, string ID)
        {
            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\FutureWiz\UNITEDV1"))
            {
                registryKey.SetValue("lmdvmc", microPhoneName); // ex) 마이크 배열(인텔® 스마트 사운드 기술)
                registryKey.SetValue("lmdvmcdisplayname", $"@device_cm_{{{Guid.NewGuid()}}}wave_{{{ID.ToUpper()}}}"); // ex) @device_cm_{33D9A762-90C8-11D0-BD43-00A0C911CE86}\wave_{A0E9964E-C49C-4709-B2A7-177ABD64EBEB}
                registryKey.SetValue("lmdvmcid", ID.ToUpper()); // ex) A0E9964E-C49C-4709-B2A7-177ABD64EBEB
                registryKey.Close();
            }
        }

        private void ObservableClear()
        {
            if (disposables != null)
            {
                if (disposables.Count > 0)
                    foreach (var disposable in disposables)
                        disposable.Dispose();

                disposables.Clear();
            }
        }
    }

    public static class ControlExtensions
    {
        /// <summary>
        /// Executes the Action asynchronously on the UI thread, does not block execution on the calling thread.
        /// </summary>
        /// <param name="control">the control for which the update is required</param>
        /// <param name="action">action to be performed on the control</param>
        public static void InvokeOnUiThreadIfRequired(this System.Windows.Forms.Control control, Action action)
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
    }
}
