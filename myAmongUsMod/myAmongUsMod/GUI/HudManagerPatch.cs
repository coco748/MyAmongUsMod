using HarmonyLib;
using Reactor.Unstrip;
using System.IO;
using UnityEngine;

namespace StandMod.GUI
{
    /*
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    public class HudManagerPatchStart
    {
        public static void Postfix()
        {
            AssetBundle bundle=AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + "\\Assets\\standmedia");
            Sprite texture=bundle.LoadAsset<Sprite>("chuck.png");
            CooldownButton button = new CooldownButton(StandButtonUtils.OnClick, 10,
                "myAmongUsMod.resources.chuck.png", 250, new UnityEngine.Vector2(0.1f, 0.1f), Category.Everyone, HudManager.Instance , null/*texture*\/ );

            CooldownButton.HudUpdate();
            MyPlugin.ManualLog.LogWarning("Button added !");
        }

    }
*/
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerPatchUpdate
    {
        public static void Postfix()
        {
            CooldownButton.HudUpdate();
        }
    }

}
