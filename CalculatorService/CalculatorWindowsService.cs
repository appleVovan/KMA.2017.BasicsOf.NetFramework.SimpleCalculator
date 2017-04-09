using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;

namespace Learning.Calculator.Service
{
    public class CalculatorWindowsService:ServiceBase
    {
        internal const string CurrentServiceName = "CalculatorService";
        internal const string CurrentServiceDisplayName = "Calculator Service";
        internal const string CurrentServiceSource = "CalculatorServiceSource";
        internal const string CurrentServiceLogName = "CalculatorServiceLogName";
        internal const string CurrentServiceDescription = "Calculator for learning purposes.";
        private ServiceHost _serviceHost=null;
        private EventLog _serviceEventLog;
        private void InitializeComponent()
        {
            _serviceEventLog = new EventLog();
            ((System.ComponentModel.ISupportInitialize)(_serviceEventLog)).BeginInit();
            ServiceName = CurrentServiceName;
            ((System.ComponentModel.ISupportInitialize)(_serviceEventLog)).EndInit();
            Logger.Logger.Initialize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Learning", CurrentServiceName));
        }
        public CalculatorWindowsService()
        {
            ServiceName = CurrentServiceName;
            InitializeComponent();
            try
            {
                if (!EventLog.SourceExists(CurrentServiceSource))
                    EventLog.CreateEventSource(CurrentServiceSource, CurrentServiceLogName);
                _serviceEventLog.Source = CurrentServiceSource;
                _serviceEventLog.Log = CurrentServiceLogName;
                _serviceEventLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 0);
                _serviceEventLog.MaximumKilobytes = 8192;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex, "Failed To Initialize Log");
            }
            try
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;
                try
                {
                    _serviceEventLog.WriteEntry("Initialization");
                }
                catch (Exception ex)
                {
                    Logger.Logger.Log(ex);
                }
            }
            catch (Exception ex)
            {
                _serviceEventLog.WriteEntry(string.Format("Initialization{0}{1}ex.StackTrace = {2}{1}ex.InnerException.Message = {3}", ex.Message, Environment.NewLine, ex.StackTrace, (ex.InnerException == null ? "null" : ex.InnerException.Message)),
                    EventLogEntryType.Error);
                Logger.Logger.Log(ex, "Initialization");
            }
        }

        protected override void OnStart(string[] args)
        {
            Logger.Logger.Log("OnStart");
            RequestAdditionalTime(120 * 1000);
#if DEBUG
            //Thread.Sleep(10000);
#endif
            try
            {
                if (_serviceHost!=null)
                    _serviceHost.Close();
            }
            catch
            {
            }
            try
            {
                _serviceHost = new ServiceHost(typeof(CalculatorService));
                _serviceHost.Open();
            }
            catch (Exception ex)
            {
                _serviceEventLog.WriteEntry(string.Format("Opening The Host: {0}", ex.Message), EventLogEntryType.Error);
                Logger.Logger.Log(ex, "OnStart");
                throw;
            }
            Logger.Logger.Log("Service Started");
        }

        protected override void OnStop()
        {
            Logger.Logger.Log("OnStop");
            RequestAdditionalTime(120 * 1000);
#if DEBUG
            //Thread.Sleep(10000);
#endif
            try
            {
                _serviceHost.Close();
            }
            catch (Exception ex)
            {
                _serviceEventLog.WriteEntry(string.Format("Trying To Stop The Host Listener{0}", ex.Message),
                                        EventLogEntryType.Error);
                Logger.Logger.Log(ex, "Trying To Stop The Host Listener");
            }
            _serviceEventLog.WriteEntry("Service Stopped", EventLogEntryType.Information);
            Logger.Logger.Log("Service Stopped");
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var ex = (Exception)args.ExceptionObject;
            _serviceEventLog.WriteEntry(string.Format("UnhandledException {0} ex.Message = {1}{0} ex.StackTrace = {2}", Environment.NewLine, ex.Message, ex.StackTrace),
                EventLogEntryType.Error);

            Logger.Logger.Log(ex, "UnhandledException");
        }
    }
}
