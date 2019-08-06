using System;
using System.Diagnostics;
using System.IO;

namespace CommonTrace.Common
{
    //类库内部使用的简单日志
    public interface ISimpleLog
    {
        void Log(object message);
        void LogEx(object ex);
    }

    internal class SimpleLog : ISimpleLog
    {
        public string Category { get; set; }

        public bool LogEnabled { get; set; }

        public bool LogFileEnabled { get; set; }

        public void Log(object message)
        {
            Trace.WriteLine(string.Format("[{0}][{1}] {2}", "SimpleLog", Category, message));
            if (LogFileEnabled)
            {
                if (message != null)
                {
                    LogFile(message.ToString(), "log");
                }
            }
        }

        public void LogEx(object ex)
        {
            if (ex == null)
            {
                return;
            }

            var content = string.Empty;
            if (ex is string theString)
            {
                content = theString;
            }
            else if (ex is Exception theEx)
            {
                content = theEx.Message;
            }
            Trace.WriteLine(string.Format("[{0}][{1}][Ex] {2}", "SimpleLog", Category, content));

            if (LogFileEnabled)
            {
                if (!string.IsNullOrWhiteSpace(content))
                {
                    LogFile(content, "logEx");
                }
            }
        }
        
        private void LogFile(string message, string filePrefix)
        {
            var now = DateHelper.Instance.GetDateNow();
            message = now.ToString("yyyyMMdd-HH:mm:ss ") + message;
            var fileName = string.Format("{0}_{1:yyyyMMdd}.log", filePrefix, now);
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            //async save file, ignore ex
            AsyncFile.Instance.AppendAllText(logFilePath, message, true);
        }
    }

    public class SimpleLogFactory
    {
        public SimpleLogFactory()
        {
            LogEnabledFunc = category => true;
            LogFileEnabledFunc = category => true;
            CreateLog = c => new SimpleLog() { Category = c, LogEnabled = LogEnabledFunc(c), LogFileEnabled = LogFileEnabledFunc(c) };
        }

        public Func<string, ISimpleLog> CreateLog { get; set; }

        public Func<string, bool> LogEnabledFunc { get; set; }

        public Func<string, bool> LogFileEnabledFunc { get; set; }
        
        public static SimpleLogFactory Instance = new SimpleLogFactory();
    }

    public static class SimpleLogFactoryExtensions
    {
        public static ISimpleLog CreateLogFor(this SimpleLogFactory factory, Type type)
        {
            return factory.CreateLog(type.Name);
        }

        public static ISimpleLog CreateLogFor<T>(this SimpleLogFactory factory)
        {
            return factory.CreateLogFor(typeof(T));
        }

        public static ISimpleLog CreateLogFor(this SimpleLogFactory factory, object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (instance is Type type)
            {
                return factory.CreateLogFor(type);
            }
            return factory.CreateLogFor(instance.GetType());
        }
    }
}
