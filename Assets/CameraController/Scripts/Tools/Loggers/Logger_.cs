using System;
using UnityEngine;

namespace ССP.Tools.Loggers
{
    public class Logger_ : ILogger
    {
        private const string SeparatorLine = "=========================";
        private static Logger_ _instance { get; set; }
        
        public static Logger_ Instance
        {
            get
            {
                if (_instance == null)
                    return new Logger_();
                else
                    return _instance;
            }
        }

        public static bool IsEnabled { get; set; }

        public void Log<T1, T2, T3>(string name1, T1 value1, string name2, T2 value2, string name3, T2 value3, bool separatorLine = false)
        {
            if (!IsEnabled)
                return;

            Debug.Log($"{name1} = {value1}");
            Debug.Log($"{name2} = {value2}");
            Debug.Log($"{name3} = {value3}\n");

            if (separatorLine)
                Debug.Log($"{SeparatorLine}\n");
        }

        public void Log<T1, T2>(string name1, T1 value1, string name2, T2 value2, bool separatorLine = false)
        {
            if (!IsEnabled)
                return;

            Debug.Log($"{name1} = {value1}");
            Debug.Log($"{name2} = {value2}\n");

            if (separatorLine)
                Debug.Log($"{SeparatorLine}\n");
        }

        public void Log<T>(string name, T value, bool separatorLine = false)
        {
            if (!IsEnabled)
                return;

            Debug.Log($"{name} = {value}\n");

            if (separatorLine)
                Debug.Log($"{SeparatorLine}\n");
        }

        public void Log<T>(T value, bool separatorLine = false)
        {
            if (!IsEnabled)
                return;

            Debug.Log($"{value}\n");

            if (separatorLine)
                Debug.Log($"{SeparatorLine}\n");
        }

        public Logger_()
        {
            IsEnabled = true;
        }
    }
}
