using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CameraCaptureWPF
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if !DEBUG
            //if (e.Args.Length > 0)
            //{
            //    if (e.Args[0].Equals("test", StringComparison.OrdinalIgnoreCase))
            //    {
            //        MainWindow window = new MainWindow();
            //        window.Show();
            //    }
            //    else
            //    {
            //        MessageBox.Show("정상적인 호출이 아닙니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //        Current.Shutdown();
            //    }
            //}
            //else
            //    System.Windows.Application.Current.Shutdown();

            MainWindow window = new MainWindow();
            window.Show();
#else
            MainWindow window = new MainWindow();
            window.Show();
#endif

        }
    }
}
