using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using DirectShowLib;
using OpenCvSharp;

namespace CameraCaptureWPF
{
    /// <summary>
    /// WindowOption.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowOption : System.Windows.Window
    {
        List<DsDevice> cameraDevices = null;
        MainWindow _mainWindow = null;

        public WindowOption(MainWindow mainWindow)
        {
            InitializeComponent();
            this._mainWindow = mainWindow;
            this.Loaded += WindowOption_Loaded;
            chkb_IsStayOnTop.Click += (s, e) => { this.Topmost = (bool)chkb_IsStayOnTop.IsChecked; _mainWindow.Topmost = (bool)chkb_IsStayOnTop.IsChecked; }; // 체크박스 이벤트
            cbb_ListCamera.SelectionChanged += Cbb_ListCamera_SelectionChanged;
        }

        private void WindowOption_Loaded(object sender, RoutedEventArgs e)
        {
            chkb_IsStayOnTop.Cursor = Cursors.Hand;
            cbb_ListCamera.Cursor = Cursors.Hand;

            chkb_IsStayOnTop.IsChecked = _mainWindow.Topmost; // 부모폼의 Topmost 설정 받아오기
            this.Topmost = _mainWindow.Topmost;        // 부모폼의 Topmost 설정 받아오기

            cameraDevices = _mainWindow.cameraDevices; // 부모폼에 있는 카메라 리스트를 가져오기(폼이 열릴 때마다 오픈하면 메인폼과 옵션폼의 카메라 리스트가 달라질 수 있음)

            //GetCameraList();
            SetUi();
        }

        private void Cbb_ListCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbb_ListCamera.SelectedIndex > -1)
                _mainWindow.SelectDeviceIndex = cbb_ListCamera.SelectedIndex; // 부모 폼에서 카메라 디바이스 선택해 변경하기 위해
        }

        // 카메라 리스트 가져오기
        private void GetCameraList() => cameraDevices.AddRange(from DsDevice dsDevice in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)
                                                               where !dsDevice.DevicePath.Contains("device:sw")
                                                               select dsDevice);

        // ui 설정
        private void SetUi(int index = 0)
        {
            if (cameraDevices.Count > 0 && index >= 0)
            {
                cbb_ListCamera.Items.Clear();
                foreach (var cameraDevice in cameraDevices)
                    cbb_ListCamera.Items.Add(cameraDevice.Name);

                if (_mainWindow.SelectDeviceIndex > -1)
                    cbb_ListCamera.SelectedIndex = _mainWindow.SelectDeviceIndex;
            }
        }
    }
}
