using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Extensions;
using UnityEngine;
using System;
using UnhollowerBaseLib.Attributes;
using StandMod.RPC;
using BepInEx.Logging;

namespace StandMod
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class StandModPlugin : BasePlugin
    {
        public const string Id = "coco748.StandMod";
        public static ManualLogSource ManualLog => PluginSingleton<StandModPlugin>.Instance.Log;
        public static MyPlayerList myPlayerList;


        public Harmony Harmony { get; } = new Harmony(Id);

        public ConfigEntry<string> Name { get; private set; }

        public override void Load()
        {
            RegisterInIl2CppAttribute.Register();
            RegisterCustomRpcAttribute.Register(this);

            var gameObject = new GameObject(nameof(ReactorPlugin)).DontDestroy();
            gameObject.AddComponent<ExampleComponent>().Plugin = this;
            Harmony.PatchAll();
            System.Console.WriteLine("StandMod Loaded !");
        }
    }

    [RegisterInIl2Cpp]
    public class ExampleComponent : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public StandModPlugin Plugin { get; internal set; }

        public ExampleComponent(IntPtr ptr) : base(ptr)
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3) && AmongUsClient.Instance && PlayerControl.LocalPlayer)
            {
                Plugin.Log.LogWarning("Sending example rpc");
                Rpc<MyRPC>.Instance.Send(new MyRPC.Data("Cześć :)"));
                Rpc<MyRPC>.Instance.SendTo(AmongUsClient.Instance.HostId, new MyRPC.Data("host :O"));
            }
        }
    }
}
