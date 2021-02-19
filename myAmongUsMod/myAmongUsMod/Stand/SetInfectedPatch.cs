using HarmonyLib;
using StandMod.GUI;
using StandMod.RPC;
using Reactor;
using Reactor.Unstrip;
using System.IO;
using UnhollowerBaseLib;
using UnityEngine;

namespace StandMod.StandRelated
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    public class SetInfectedpatch
    {

        public static void Postfix(Il2CppReferenceArray<GameData.PlayerInfo> JPGEIBIBJPJ)
        {
            SetInfectedPatchTools tools = new SetInfectedPatchTools();
            StandModPlugin.myPlayerList = new MyPlayerList();

            tools.PickStandsForEveryone();
            tools.SendStandsToClients();

            //INIT SERVER HUD
            Sprite texture = new ResourceManager().GetStandSprite(StandModPlugin.myPlayerList.GetMyPlayerById(StandModPlugin.myPlayerList.GetMe().PlayerId).stand.id);
            CooldownButton button = new CooldownButton(StandButtonUtils.OnClick, 10,
                "", 250, new UnityEngine.Vector2(0.1f, 0.1f), Category.Everyone, HudManager.Instance, texture);

            StandModPlugin.ManualLog.LogWarning(StandModPlugin.myPlayerList);

            foreach (CooldownButton b in CooldownButton.buttons)
            {
                StandModPlugin.ManualLog.LogWarning(b);
            }

        }
    }

    public class SetInfectedPatchTools
    {
        public void PickStandsForEveryone()
        {
            //Ask to everyone to init their MyPlayerList and start HUD
            Rpc<MyRPC>.Instance.Send(new MyRPC.Data("Init"));
            
            StandList StandPool = new StandList();
            StandModPlugin.ManualLog.LogInfo("StandList: \n" + StandPool);
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                bool standIsOk = false;
                while (!standIsOk)
                {
                    Stand stand = StandPool.GetRandomStand();
                    if ((stand.role == Role.impostor) && (!player.Data.IsImpostor)) standIsOk = false;
                    else if ((stand.role == Role.crewmate) && (player.Data.IsImpostor)) standIsOk = false;
                    else
                    {
                        standIsOk = true;
                        StandPool.RemoveStand(stand);
                        StandModPlugin.myPlayerList.AddStand(stand, player.PlayerId);
                    }
                }
            }
        }


        public void SendStandsToClients()
        {
            foreach (MyPlayer player in StandModPlugin.myPlayerList.list)
            {
                if (player.playerId != StandModPlugin.myPlayerList.GetMe().PlayerId)
                {
                    Rpc<MyRPC>.Instance.Send(new MyRPC.Data("SetYourStand:"+ player.playerId + ":" + player.stand.ToStringShort()));
                }
            }
        }
    }
}
