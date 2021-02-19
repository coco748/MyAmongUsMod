using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace StandMod.GUI
{
    public class EndGamePatch
    {
        /*
        [HarmonyPatch(typeof(EndGameManager), "SetEverythingUp")]
        public static class EndGameManagerPatch
        {
            public static bool Prefix(EndGameManager __instance)
            {
                CooldownButton.buttons.Clear();
                MyPlugin.ManualLog.LogWarning("Button removed!");
                return true;
            }
        }
        */
    }
}
