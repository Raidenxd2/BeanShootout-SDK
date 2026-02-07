using System;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public static class CommandLineArgs
    {
        public static bool SetLoadLevelLocalBuild;
        public static bool SetLoadBenchmarkScene;
        public static bool FastLoad;
        public static bool ShadowsPerfTest;
        public static bool UpdateLevelImages;
        public static bool VerboseLoggingEnabled;
#if UNITY_EDITOR || KILLITMYSELF_DEBUG
        public static bool DebugEnabled;
#endif
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void CheckArgs()
        {
            string cmd = Environment.CommandLine;
#if !UNITY_EDITOR
            string ver = Application.version;
#endif
            
            if (cmd.Contains("-loadlevellocalbuild"))
            {
                SetLoadLevelLocalBuild = true;
            }

            if (cmd.Contains("-benchmark"))
            {
                SetLoadBenchmarkScene = true;
            }

            if (cmd.Contains("-fastload"))
            {
                FastLoad = true;
            }
            
            if (cmd.Contains("-ShadowsPerfTest"))
            {
                ShadowsPerfTest = true;
            }
            
            if (cmd.Contains("-updatelevelimages"))
            {
                UpdateLevelImages = true;
            }
            
#if !UNITY_EDITOR
            if (cmd.Contains("-verbose") || ver.Contains("Dev") || ver.Contains("Beta") || ver.Contains("Alpha"))
            {
                VerboseLoggingEnabled = true;
            }
#else
            VerboseLoggingEnabled = true;
#endif

            if (PlayerPrefs.GetInt("VerboseLogging") == 1)
            {
                VerboseLoggingEnabled = true;
            }

            if (cmd.Contains("-forcenoverbose"))
            {
                VerboseLoggingEnabled = false;
            }
            
#if UNITY_EDITOR || KILLITMYSELF_DEBUG
            if (cmd.Contains("-debug"))
            {
                DebugEnabled = true;
            }
#endif
        }
    }
}