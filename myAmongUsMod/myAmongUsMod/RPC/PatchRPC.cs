using Hazel;
using StandMod.GUI;
using StandMod.StandRelated;
using Reactor;
using Reactor.Unstrip;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Namespace for everything related to the new RPC codes 
/// </summary>
namespace StandMod.RPC
{
    [RegisterCustomRpc]
    public class MyRPC : PlayerCustomRpc<StandModPlugin, MyRPC.Data>
    {
        //public MyRPCTools tools = new MyRPCTools();

        public MyRPC(StandModPlugin plugin) : base(plugin)
        {
        }

        public readonly struct Data
        {
            public readonly string Message;

            public Data(string message)
            {
                Message = message;
            }
        }

        public override RpcLocalHandling LocalHandling => RpcLocalHandling.None;

        public override void Write(MessageWriter writer, Data data)
        {
            writer.Write(data.Message);
        }

        public override Data Read(MessageReader reader)
        {
            return new Data(reader.ReadString());
        }

        public override void Handle(PlayerControl innerNetObject, Data data)
        {
            Plugin.Log.LogWarning($"{innerNetObject.Data.PlayerId} sent \"{data.Message}\""); //LOG
            MyRPCTools tools = new MyRPCTools();

            //check if MyPlugin.myPlayerList already exist
            if (StandModPlugin.myPlayerList == null) StandModPlugin.myPlayerList = new MyPlayerList();

            //PARSE ARGS FOR MESSAGES WITH ARGS
            string[] args = tools.ParseMessage(data.Message);
            switch (args[0])
            {
                case "Init":
                    tools.Init();
                    break;
                case "SetYourStand":
                    tools.SetYourStand(args);
                    break;
            }
        
        }
    }

    public class MyRPCTools
    {
        public string[] ParseMessage(string args)
        {
            return args.Split(':');
        }

        public void Init()
        {
            StandModPlugin.myPlayerList = new MyPlayerList();
        }

        public void SetYourStand(string[] args)
        {

            MyPlayer myself=StandModPlugin.myPlayerList.GetMyPlayerById(PlayerControl.LocalPlayer.PlayerId);
            if (!myself.playerId.ToString().Equals(args[1]))return;

            Stand myStand = new Stand(Int32.Parse(args[2]));
            myself.stand = myStand;
            
            StandModPlugin.ManualLog.LogInfo("I AM " + myStand + "NOW !");

            //INIT CLIENT HUD
            CooldownButton.buttons.Clear();
            Sprite texture = new ResourceManager().GetStandSprite(StandModPlugin.myPlayerList.GetMyPlayerById(PlayerControl.LocalPlayer.PlayerId).stand.id);
            CooldownButton button = new CooldownButton(StandButtonUtils.OnClick, 10,
                "", 250, new UnityEngine.Vector2(0.1f, 0.1f), Category.Everyone, HudManager.Instance, texture);
        }
    }
}
