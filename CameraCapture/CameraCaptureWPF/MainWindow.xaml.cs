using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DirectShowLib;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CameraCaptureWPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// Image와 OpenCV를 이용한 메인 캠 출력
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private VideoCapture _videoCapture = null; // 캠 캡쳐
        private DispatcherTimer _dispatcherTimer = null; // 디스패쳐 타이머
        public List<DsDevice> _CameraDevices = null; // 캠 리스트
        
        private OptionWindow _OptionWindow = null; // 옵션 서브 폼
        
        private int selectDeviceIndex = -1; // 옵션 폼에서 설정시 변경에 필요한 변수
        public int SelectDeviceIndex { get => selectDeviceIndex; set => SetMainUI(value); } // 옵션 폼에서 카메라 인덱스가 변경하면 SetUI에서 초기화

        public MainWindow()
        {
            InitializeComponent();

            // MainWindow_Loaded로 옮기지 않은 이유는 폼이 로드되고 또 ui를 설정하기에 느려 보이는 효과 때문. 
            _CameraDevices = new List<DsDevice>();
            GetCameraList();
            if (_CameraDevices.Count > 0)
            {
                InitSetting();
                SetMainUI();
            }
            else
                MessageBox.Show("등록된 카메라가 없습니다.", "주의", MessageBoxButton.OK, MessageBoxImage.Warning);

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
            btn_OpenOption.Click += Btn_OpenOption_Click;
        }   

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true; // 메인 폼 기본으로 항상 탑
            btn_OpenOption.Cursor = Cursors.Hand;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            TimerStop(ref _dispatcherTimer);

            if (_videoCapture != null)
            {
                if (_videoCapture.IsOpened())
                    _videoCapture.Release();

                _videoCapture.Dispose();
            }

            if (_OptionWindow != null)
                _OptionWindow.Close();
        }

        private void Btn_OpenOption_Click(object sender, RoutedEventArgs e)
        {
            if (_OptionWindow == null) // null 일때만 옵션 창 오픈
            {
                var ms = this;
                _OptionWindow = new OptionWindow(ref ms);
                _OptionWindow.Top = this.Top + (this.ActualHeight - _OptionWindow.Height) / 2;
                _OptionWindow.Left = this.Left + (this.ActualWidth - _OptionWindow.Width) / 2;
                _OptionWindow.Closed += (s, args) => _OptionWindow = null; // 닫히면 null로 초기화
                _OptionWindow.Show();
            }
            else
            {
                _OptionWindow.Top = this.Top + (this.ActualHeight - _OptionWindow.Height) / 2;
                _OptionWindow.Left = this.Left + (this.ActualWidth - _OptionWindow.Width) / 2;
                _OptionWindow.Activate();
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
            if (_videoCapture != null)
            {
                using (Mat mat = new Mat()) // 캠 캡쳐 후 이미지 소스 지정
                {
                    if (_videoCapture.Read(mat))
                    {
                        var wb = WriteableBitmapConverter.ToWriteableBitmap(mat, 1024, 1024, PixelFormats.Bgr24, null);
                        img_Camera.Source = wb;
                    }
                }
            }
        }

        // 카메라 리스트 가져오기 LINQ
        private void GetCameraList() => _CameraDevices.AddRange(from DsDevice dsDevice in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)
                                                               where !dsDevice.DevicePath.Contains("device:sw")
                                                               select dsDevice);

        // MainUI 설정
        private void SetMainUI(int index = 0)
        {
            TimerStop(ref _dispatcherTimer);

            if (_CameraDevices.Count > 0 && index >= 0)
            {
                selectDeviceIndex = index;

                if (_videoCapture != null && _videoCapture.IsOpened())
                    _videoCapture.Release();
                
                if (_videoCapture == null)
                    _videoCapture = new VideoCapture(index);
                else
                    if (!_videoCapture.IsOpened())
                        _videoCapture.Open(index);

                // FWCap -> 타이틀 바 변경 예정
                //if (_videoCapture != null && _videoCapture.IsOpened())
                //    this.Title = $"Futurewiz Camera - {_CameraDevices[index].Name}";
                //else
                //    this.Title = $"Futurewiz Camera - 연결에 실패했습니다.";
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
