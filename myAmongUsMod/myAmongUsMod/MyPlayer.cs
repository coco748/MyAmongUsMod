using System;
using System.Collections.Generic;
using System.Text;
using StandMod.StandRelated;

namespace StandMod
{
    public class MyPlayer
    {
        public byte playerId;
        public Stand stand;

        public MyPlayer(byte playerId, Stand stand)
        {
            this.playerId = playerId;
            this.stand = stand;
        }

        public MyPlayer(PlayerControl player)
        {
            this.playerId = player.PlayerId;
            this.stand = null;
        }

        public override string ToString()
        {
            return playerId+": "+stand;
        }
    }

    public class MyPlayerList
    {
        public List<MyPlayer> list;

        public MyPlayerList()
        {
            this.list = new List<MyPlayer>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                this.list.Add(new MyPlayer(player));
            }
        }

        public PlayerControl FindById(int id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                {
                    return player;
                }
            }
            return null;
        }

        public PlayerControl GetHost()
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.AmOwner)
                {
                    return player;
                }
            }
            return null;
        }

        public PlayerControl GetMe()
        {
            return PlayerControl.LocalPlayer;
        }

        public List<MyPlayer> GetList()
        {
            return this.list;
        }

        public MyPlayer GetMyPlayerById(byte id)
        {
            return this.list.Find(
                delegate (MyPlayer player)
                {
                    return player.playerId == id;
                }
            );
        }

        public bool AddStand(Stand stand, byte playerID)
        {
            MyPlayer player = this.GetMyPlayerById(playerID);
            player.stand = stand;
            return true;
        }

        public override string ToString()
        {
            String res = "";
            foreach (MyPlayer player in this.list) res += player + "\n";
            return res;
        }

    }
}
