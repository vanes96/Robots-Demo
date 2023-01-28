
namespace ССP.Tools.Loggers
{
    public class FileLogger : ILogger
    {
        private const string SeparatorLine = "-------------------------";

        private static FileLogger _instance { get; set; }

        public static FileLogger Instance
        {
            get
            {
                if (_instance == null)
                    return new FileLogger();
                else
                    return _instance;
            }
        }

        public void Log<T1, T2, T3>(string name1, T1 value1, string name2, T2 value2, string name3, T2 value3, bool separatorLine = false)
        {
            //throw new NotImplementedException();
        }

        public void Log<T1, T2>(string name1, T1 value1, string name2, T2 value2, bool separatorLine = false)
        {
            //throw new NotImplementedException();
        }

        public void Log<T>(string name, T value, bool separatorLine = false)
        {
            //throw new NotImplementedException();
        }

        public void Log<T>(T value, bool separatorLine = false)
        {
            //throw new NotImplementedException();
        }
    }
}
