using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace KillItMyself.Runtime
{
    public static class BeanLogger
    {
        public static bool VerboseLoggingEnabled;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void CheckArg()
        {
            if (Environment.CommandLine.Contains("-verbose") || Application.isEditor)
            {
                VerboseLoggingEnabled = true;
            }
        }
        
        public static void Log(string text, Object source)
        {
            string logString;
            if (VerboseLoggingEnabled)
            {
                StackFrame frame = new(1, true);
                string method = frame.GetMethod().Name;
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();
                logString = "[" + Time.realtimeSinceStartup + "] [LOG] [" + fileName + ":" + lineNumber + "] (" + method + ") " + text;
            }
            else
            {
                logString = "[LOG] " + text;
            }
            Debug.Log(logString, source);
        }

        public static void LogError(string text, Object source)
        {
            string logString;
            if (VerboseLoggingEnabled)
            {
                StackFrame frame = new(1, true);
                string method = frame.GetMethod().Name;
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();
                logString = "[" + Time.realtimeSinceStartup + "] [ERROR] [" + fileName + ":" + lineNumber + "] (" + method + ") " + text;
            }
            else
            {
                logString = "[ERROR] " + text;
            }
            Debug.LogError(logString, source);
        }

        public static void LogWarning(string text, Object source)
        {
            string logString;
            if (VerboseLoggingEnabled)
            {
                StackFrame frame = new(1, true);
                string method = frame.GetMethod().Name;
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();
                logString = "[" + Time.realtimeSinceStartup + "] [WARNING] [" + fileName + ":" + lineNumber + "] (" + method + ") " + text;
            }
            else
            {
                logString = "[WARNING] " + text;
            }
            Debug.LogWarning(logString, source);
        }
    }
}