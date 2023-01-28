
namespace ССP.Tools.Loggers
{
    public interface ILogger
    {
        void Log<T1, T2, T3>(string name1, T1 value1, string name2, T2 value2, string name3, T2 value3, bool separatorLine);

        void Log<T1, T2>(string name1, T1 value1, string name2, T2 value2, bool separatorLine);

        void Log<T>(string name, T value, bool separatorLine);

        void Log<T>(T value, bool separatorLine);
    }
}
