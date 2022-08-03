using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CameraCaptureWPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : System.Windows.Application
    {
        Mutex mutex;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if !DEBUG
            string mutexName = "Dunamu Camera";
            bool createnew;

            mutex = new Mutex(true, mutexName, out createnew);

            if (!createnew)
                Shutdown();

            if (e.Args.Length > 0)
            {
                if (e.Args[0].Equals("Futurewiz_Cam", StringComparison.OrdinalIgnoreCase))
                {
                    MainWindow window = new MainWindow();
                    window.Show();
                }
                else
                {
                    MessageBox.Show("정상적인 호출이 아닙니다.", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Current.Shutdown();
                }
            }
            else
                Current.Shutdown();
#else
            MainWindow window = new MainWindow();
            window.Show();
#endif

        }
    }
}