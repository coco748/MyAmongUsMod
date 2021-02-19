using System;
using System.Collections.Generic;
using System.Text;

namespace StandMod.StandRelated
{
    public class Stand
    {
        public int id;
        public string nom;
        public Role role;

        public Stand(int id)
        {
            switch (id)
            {
                case 0:
                    this.id = 0;
                    this.nom = "Standless";
                    this.role = Role.both;
                    break;
                case 1:
                    this.id = 1;
                    this.nom = "Magician's red";
                    this.role = Role.both;
                    break;
                case 2:
                    this.id = 2;
                    this.nom = "Hermit Purple";
                    this.role = Role.both;
                    break;
                case 3:
                    this.id = 3;
                    this.nom = "Star Platinum";
                    this.role = Role.crewmate;
                    break;
                case 4:
                    this.id = 4;
                    this.nom = "The World";
                    this.role = Role.impostor;
                    break;
                case 5:
                    this.id = 5;
                    this.nom = "King Crimson";
                    this.role = Role.both;
                    break;
                case 6:
                    this.id = 6;
                    this.nom = "Cream";
                    this.role = Role.impostor;
                    break;
                case 7:
                    this.id = 7;
                    this.nom = "White Snake";
                    this.role = Role.impostor;
                    break;
                case 8:
                    this.id = 8;
                    this.nom = "Yellow Temperance";
                    this.role = Role.both;
                    break;
                case 9:
                    this.id = 9;
                    this.nom = "Lovers";
                    this.role = Role.crewmate;
                    break;
                case 10:
                    this.id = 10;
                    this.nom = "Red Hot Chily Pepper";
                    this.role = Role.both;
                    break;
                case 11:
                    this.id = 11;
                    this.nom = "Harvest";
                    this.role = Role.crewmate;
                    break;
                case 12:
                    this.id = 12;
                    this.nom = "Bites The Dust";
                    this.role = Role.crewmate;
                    break;
                case 13:
                    this.id = 13;
                    this.nom = "The Lock";
                    this.role = Role.both;
                    break;
                case 14:
                    this.id = 14;
                    this.nom = "Achtung baby";
                    this.role = Role.both;
                    break;
                case 15:
                    this.id = 15;
                    this.nom = "Super Fly";
                    this.role = Role.crewmate;
                    break;
                case 16:
                    this.id = 16;
                    this.nom = "Talking Head";
                    this.role = Role.both;
                    break;
                default:
                    this.id = -1;
                    this.nom = null;
                    this.role = Role.both;
                    break;

            }
        }

        public override string ToString()
        {
            return "" + this.id + ":" + this.nom + "(" + role + ")";
        }

        public string ToStringShort()
        {
            return ""+this.id;
        }

    }

    public enum StandEnum
    {
        Standless,
        MagiciansRed,
        HermitPurple,
        StarPlatinum,
        TheWorld,
        KingCrimson,
        Cream,
        WhiteSnake,
        YellowTemperance,
        Lovers,
        RedHotChilyPepper,
        Harvest,
        BitesTheDust,
        TheLock,
        AchtungBaby,
        SuperFly,
        TalkingHead
    }

    public class StandList
    {
        public List<Stand> list;

        public StandList()
        {
            this.list = new List<Stand>();

            foreach (int i in StandEnum.GetValues(typeof(StandEnum)))
            {
                this.list.Add(new Stand(i));
            }
        }

        public List<Stand> GetStandPool()
        {
            return this.list;
        }

        public Stand GetRandomStand()
        {
            int rd = new Random().Next(this.list.Count);
            return this.list[rd];
        }

        public bool RemoveStand(Stand stand)
        {
            return this.list.Remove(stand);
        }

        public override string ToString()
        {
            string res = "";
            int cpt = 0;
            foreach (Stand stand in this.list)
            {
                res += cpt + "=" + stand + "\n";
                cpt++;
            }
            return res;
        }
    }


    public enum Role
    {
        both,
        crewmate,
        impostor,
    }
}
