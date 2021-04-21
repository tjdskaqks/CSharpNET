using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DirectShowLib;

namespace CameraCaptureWPF
{
    /// <summary>
    /// WindowOption.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OptionWindow : System.Windows.Window
    {
        List<DsDevice> _CameraDevices = null; // 메인 폼에서 받아올 리스트
        MainWindow _MainWindow = null;

        public OptionWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this._MainWindow = mainWindow;

            this.Loaded += WindowOption_Loaded;
            chkb_IsStayOnTop.Click += (s, e) => { this.Topmost = (bool)chkb_IsStayOnTop.IsChecked; _MainWindow.Topmost = (bool)chkb_IsStayOnTop.IsChecked; }; // 체크박스 이벤트
            cbb_ListCamera.SelectionChanged += Cbb_ListCamera_SelectionChanged;
        }

        private void WindowOption_Loaded(object sender, RoutedEventArgs e)
        {
            chkb_IsStayOnTop.Cursor = Cursors.Hand;
            cbb_ListCamera.Cursor = Cursors.Hand;

            chkb_IsStayOnTop.IsChecked = _MainWindow.Topmost; // 부모폼의 Topmost 설정 받아오기
            this.Topmost = _MainWindow.Topmost;        // 부모폼의 Topmost 설정 받아오기

            _CameraDevices = _MainWindow._CameraDevices; // 부모폼에 있는 카메라 리스트를 가져오기(폼이 열릴 때마다 오픈하면 메인폼과 옵션폼의 카메라 리스트가 달라질 수 있음)

            SetOptionUI();
        }

        private void Cbb_ListCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbb_ListCamera.SelectedIndex > -1)
                _MainWindow.SelectDeviceIndex = cbb_ListCamera.SelectedIndex; // 부모 폼에서 카메라 디바이스 선택해 변경하기 위해
        }

        // 옵션 폼 UI 설정
        private void SetOptionUI(int index = 0)
        {
            if (_CameraDevices.Count > 0 && index >= 0)
            {
                cbb_ListCamera.Items.Clear();
                foreach (var cameraDevice in _CameraDevices)
                    cbb_ListCamera.Items.Add(cameraDevice.Name);

                if (_MainWindow.SelectDeviceIndex > -1)
                    cbb_ListCamera.SelectedIndex = _MainWindow.SelectDeviceIndex;
            }
        }
    }
}
