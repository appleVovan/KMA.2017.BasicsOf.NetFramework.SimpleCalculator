using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Learning.Calculator.WPFCalculator
{
    public partial class App
    {
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Logger.Log(e.Exception);
            MessageBox.Show(String.Format(WPFCalculator.Properties.Strings.Exception_Default, Environment.NewLine, e.Exception.Message));
            e.Handled = true;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var culture = CultureInfo.GetCultureInfo("uk-UA");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Logger.Logger.Initialize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Learning", "WPFCalculator"));
            
            StartupUri = new Uri("/WPFCalculator;component/MainWindow.xaml", UriKind.Relative);
        }
    }
}
