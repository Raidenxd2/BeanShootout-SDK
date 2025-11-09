#if KILLITMYSELF_FULL
using System;
using UnityCommandLineParser;
#endif
using UnityEngine;

namespace KillItMyself.Runtime
{
    public static class GameSettings
    {
#if KILLITMYSELF_FULL
        [CommandLineArgument("gs_ma", "Sets 'Max Ammo' (Game Settings)")]
#endif
        public static int MaxAmmo = 75;
#if KILLITMYSELF_FULL
        [CommandLineArgument("gs_sm", "Sets 'Show Minimap' (Game Settings)")]
#endif
        public static bool ShowMinimap = true;

#if KILLITMYSELF_FULL
        [CommandLineArgument("gs_mp", "Sets 'Max Players' (Game Settings)")]
#endif
        public static int MaxPlayers = 4;
#if KILLITMYSELF_FULL
        [CommandLineArgument("gs_fnop", "Sets 'Full screen when theres no other players' (Game Settings)")]
#endif
        public static bool FullscreenNoOtherPlayers = false;

#if KILLITMYSELF_FULL
        [CommandLineArgument("gs_sa", "Sets 'Share ammo between all players' (Game Settings)")]
#endif
        public static bool SharedAmmo = false;

        public static PlayerMovementSettingsSO MovementSettings;
        public static int MovementSettingsIndex;

#if KILLITMYSELF_FULL
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void CheckArg()
        {
            // if (Environment.CommandLine.Contains("-"))
        }
#endif

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        public static void ResetValues()
        {
            MaxAmmo = 75;
            ShowMinimap = true;
            MaxPlayers = 4;
            FullscreenNoOtherPlayers = false;
            SharedAmmo = false;
            MovementSettings = null;
        }
#endif
    }
}