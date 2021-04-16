using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using DirectShowLib;
using OpenCvSharp.Flann;
using System.Windows.Media;
using System.Diagnostics;

namespace CameraCaptureWPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // 캠 캡쳐
        private VideoCapture videoCapture = null;
        // 디스패쳐 타이머
        private DispatcherTimer _dispatcherTimer = null;
        // 캠 리스트
        public List<DsDevice> cameraDevices = null;
        // 옵션 폼
        private WindowOption windowOption = null;

        // 옵션 폼에서 설정시 변경에 필요한 변수
        private int selectDeviceIndex = -1;
        public int SelectDeviceIndex { get => selectDeviceIndex; set => SetUi2(value); }

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
            btn_OpenOption.Click += Btn_OpenOption_Click;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true; // 기본 탑
            btn_OpenOption.Cursor = Cursors.Hand;

            cameraDevices = new List<DsDevice>();

            GetCameraList();

            if (cameraDevices.Count > 0)
            {
                InitSetting();
                SetUi2();
            }
            else
            {
                MessageBox.Show("등록된 카메라가 없습니다.", "주의", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            TimerStop(ref _dispatcherTimer);

            if (videoCapture != null)
            {
                if (videoCapture.IsOpened())
                    videoCapture.Release();

                videoCapture.Dispose();
            }

            if (windowOption != null)
                windowOption.Close();
        }
        private void Btn_OpenOption_Click(object sender, RoutedEventArgs e)
        {
            if (windowOption == null) // null 일때만 옵션 창 오픈
            {
                windowOption = new WindowOption(this);
                windowOption.Left = this.Left + this.Width;
                windowOption.Top = this.Top;
                windowOption.Closed += (s, args) => windowOption = null; // 닫히면 null로 초기화
                windowOption.Show();
            }
            else
            {
                windowOption.Left = this.Left + this.Width;
                windowOption.Top = this.Top;
                windowOption.Activate();
            }
        }

        // 타이머 초기화
        public void InitSetting()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
        }

        // 타이머 동작
        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (videoCapture != null)
            {
                using (Mat mat = new Mat())
                {
                    if (videoCapture.Read(mat))
                    {
                        var wb = WriteableBitmapConverter.ToWriteableBitmap(mat, 1024, 1024, PixelFormats.Bgr24, null);
                        img_Camera.Source = wb;
                    }
                }
            }
        }

        // 카메라 리스트 가져오기
        private void GetCameraList() => cameraDevices.AddRange(from DsDevice dsDevice in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)
                                                               where !dsDevice.DevicePath.Contains("device:sw")
                                                               select dsDevice);

        // ui 설정
        private void SetUi2(int index = 0)
        {
            TimerStop(ref _dispatcherTimer);

            if (cameraDevices.Count > 0 && index >= 0)
            {
                selectDeviceIndex = index;

                if (videoCapture != null &&videoCapture.IsOpened())
                    videoCapture.Release();

                videoCapture = VideoCapture.FromCamera(index, VideoCaptureAPIs.ANY);
                if (videoCapture.Open(index, VideoCaptureAPIs.ANY))
                    this.Title = $"Camera - {cameraDevices[index].Name}";
                else
                    this.Title = $"Camera - 연결에 실패했습니다.";
            }
            TimerStart(ref _dispatcherTimer);
        }

        private void TimerStart(ref DispatcherTimer dispatcherTimer)
        {
            if (dispatcherTimer != null && !dispatcherTimer.IsEnabled)
                dispatcherTimer.Start();
        }

        private void TimerStop(ref DispatcherTimer dispatcherTimer)
        {
            if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();
        }
    }
}
