using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;

using Microsoft.Win32;

using NAudio.CoreAudioApi;

using NAudio.Wave;

namespace AudioControlLibrary
{
    /// <summary>
    /// uc_WPFAudioControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class uc_WPFAudioControl : UserControl
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
        public uc_WPFAudioControl()
        {
            InitializeComponent();
            this.Loaded += Uc_WPFAudioControl_Loaded;
            this.Unloaded += Uc_WPFAudioControl_Unloaded;

            cbb_MicrophoneList.SelectionChanged += Cbb_MicrophoneList_SelectionChanged;
            sd_value.ValueChanged += Sd_value_ValueChanged;
            btn_StateMicrophone.Click += Btn_StateMicrophone_Click;
            btn_OpenSoundControl.Click += Btn_OpenSoundControl_Click;
        }

        private void Btn_OpenSoundControl_Click(object sender, RoutedEventArgs e)
        {
            // 제어판 - 소리 - 녹음 열기 (마지막 0은 재생탭, 1은 녹음탭)            
            System.Diagnostics.Process.Start("rundll32", "Shell32.dll,Control_RunDLL Mmsys.cpl,,1");
        }

        private void Uc_WPFAudioControl_Loaded(object sender, RoutedEventArgs e)
        {
            btn_StateMicrophone.Cursor = Cursors.Hand;

            cbb_MicrophoneList.Cursor = Cursors.Hand;                        

            sd_value.Cursor = Cursors.Hand;
            sd_value.Maximum = 100; // 기본 트랙바 max는 10
            sd_value.Value = 0;

            pb_MicrophoneValue.Value = 0;

            btn_OpenSoundControl.Cursor = Cursors.Hand;

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

        private void Uc_WPFAudioControl_Unloaded(object sender, RoutedEventArgs e)
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

        // 이미지 버튼 클릭시 음소거, 음소거 해제
        private void Btn_StateMicrophone_Click(object sender, RoutedEventArgs e)
        {
            if (defaultMicDeivce != null)
                defaultMicDeivce.Mute(!defaultMicDeivce.IsMuted);
        }

        // 트랙바로 마이크 사운드 조절
        private void Sd_value_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (defaultMicDeivce != null)
                defaultMicDeivce.Volume = (int)e.NewValue;
        }

        // 콤보박스 아이템 변경 이벤트 추가
        private void Cbb_MicrophoneList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbb_MicrophoneList.SelectedIndex > -1)
            {
                if (!defaultMicDeivce.FullName.Equals(cbb_MicrophoneList.SelectedItem.ToString()))
                {
                    CoreAudioDevice coreAudioDevice = AudioDevices.Find(x => x.FullName.Equals(cbb_MicrophoneList.SelectedItem.ToString()));

                    if (coreAudioDevice != null)
                        coreAudioController.SetDefaultDevice(coreAudioDevice);
                }
            }
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
                // 옵저버 해제
                ObservableClear();
                // 현재 선택된 녹음 장치 가져오기
                defaultMicDeivce = coreAudioController.DefaultCaptureDevice;
                // 옵저버 등록
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
                    this.WPFInvokeOnUiThreadIfRequired(() => cbb_MicrophoneList.Items.Clear());
                    //기본 선택된 녹음 장치가 있으면 트랙바 볼륨 설정.
                    if (AudioDevices != null && AudioDevices.Count > 0 && defaultMicDeivce != null)
                    {
                        // ((ToolStripMenuItem)cms_MicrophoneList.Items[0]).Checked = true;
                        Action<int> actAddList = (count) => {
                            for (int i = 0; i < count; i++)
                                cbb_MicrophoneList.Items.Add(AudioDevices[i].FullName);

                            cbb_MicrophoneList.SelectedIndex = AudioDevices.IndexOf(defaultMicDeivce);
                        };

                        this.WPFInvokeOnUiThreadIfRequired(() => actAddList(AudioDevices.Count));

                        if (waveIn == null)
                            SetWaveInEvent(ref waveIn);

                        if ((int)defaultMicDeivce.Volume > -1)
                            this.WPFInvokeOnUiThreadIfRequired(() => sd_value.Value = (int)defaultMicDeivce.Volume);

                        if (defaultMicDeivce.IsMuted)
                        {
                            this.WPFInvokeOnUiThreadIfRequired(() => btn_StateMicrophone.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("red")); // 실제 적용시 이미지 변경
                            if (waveIn != null)
                            {
                                waveIn.StopRecording();
                                gbIsStopWave = false;
                                this.WPFInvokeOnUiThreadIfRequired(() => pb_MicrophoneValue.Value = 0);
                            }
                        }
                        else
                        {
                            this.WPFInvokeOnUiThreadIfRequired(() => btn_StateMicrophone.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("green")); // 실제 적용시 이미지 변경                        
                            if (waveIn != null && !gbIsStopWave)
                            {
                                waveIn.StartRecording();
                                gbIsStopWave = true;
                            }
                        }
                    }
                    else
                        this.WPFInvokeOnUiThreadIfRequired(() => cbb_MicrophoneList.SelectedIndex = -1);
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void SetWaveInEvent(ref WaveInEvent waveIn)
        {
            int? devicenum = GetNAudioDeviceNumver();
            if (devicenum != null)
            {
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = (int)devicenum;
                waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
                waveIn.BufferMilliseconds = (int)((double)BUFFER_SAMPLES / (double)RATE * 1000.0);
                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.RecordingStopped += (sender, e) => gbIsStopWave = false; this.WPFInvokeOnUiThreadIfRequired(() => pb_MicrophoneValue.Value = 0);
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
            this.WPFInvokeOnUiThreadIfRequired(() => pb_MicrophoneValue.Value = (int)(frac * pb_MicrophoneValue.Width));
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
                this.WPFInvokeOnUiThreadIfRequired(() => sd_value.Value = (int)deviceVolumeChangedArgs.Volume);                
            }
        }

        // 음소거 옵저버
        private void OnMuteChanged(DeviceMuteChangedArgs deviceMuteChangedArgs)
        {
            if (deviceMuteChangedArgs.Device.FullName.Equals(defaultMicDeivce.FullName))
            {
                GetDevices();
                SetUI();
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
                //SetRegistrykey(defaultDeviceChangedArgs.Device.FullName, defaultDeviceChangedArgs.Device.Id.ToString());
                GetDevices();
                SetUI();
            }
        }

        // 상태 변경 옵저버 ex) 사용 -> 사용 안 함
        private void OnStateChanged(DeviceStateChangedArgs deviceStateChangedArgs)
        {
            // TODO : 메뉴 리스트 초기화
            // 1. Device State에 따라 메뉴 리스트에서 enable과 active시 추가
            this.WPFInvokeOnUiThreadIfRequired(() => pb_MicrophoneValue.Value = 0);
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

        // WaveIn 디바이스 순서 가져오기
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

        // 레지스터리 등록
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

        // 옵저버 해제
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
}
