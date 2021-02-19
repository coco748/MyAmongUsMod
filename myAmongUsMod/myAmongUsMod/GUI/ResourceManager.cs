using Reactor.Unstrip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace StandMod.GUI
{
    public class ResourceManager
    {
        public static AssetBundle bundle = AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + "\\Assets\\standmedia");

        public Sprite GetStandSprite(int id)
        {
            Sprite res = bundle.LoadAsset<Sprite>("chuck.png");
            switch (id)
            {
                case 0:
                    res=bundle.LoadAsset<Sprite>("chuck.png");
                    break;
                case 1:
                    res = bundle.LoadAsset<Sprite>("magiciansred.png");
                    break;
                case 2:
                    res = bundle.LoadAsset<Sprite>("hermitpurple.png");
                    break;
                case 3:
                    res = bundle.LoadAsset<Sprite>("starplatinum.png");
                    break;
                case 4:
                    res = bundle.LoadAsset<Sprite>("world.png");
                    break;
                case 5:
                    res = bundle.LoadAsset<Sprite>("kingcrimson.png");
                    break;
                case 6:
                    res = bundle.LoadAsset<Sprite>("cream.png");
                    break;
                case 7:
                    res = bundle.LoadAsset<Sprite>("whitesnake.png");
                    break;
                case 8:
                    res = bundle.LoadAsset<Sprite>("yellowtemperance.png");
                    break;
                case 9:
                    res = bundle.LoadAsset<Sprite>("lovers.png");
                    break;
                case 10:
                    res = bundle.LoadAsset<Sprite>("redhotchilipepper.png");
                    break;
                case 11:
                    res = bundle.LoadAsset<Sprite>("harvest.png");
                    break;
                case 12:
                    res = bundle.LoadAsset<Sprite>("bitesthedust.png");
                    break;
                case 13:
                    res = bundle.LoadAsset<Sprite>("lock.png");
                    break;
                case 14:
                    res = bundle.LoadAsset<Sprite>("achtungbaby.png");
                    break;
                case 15:
                    res = bundle.LoadAsset<Sprite>("superfly.png");
                    break;
                case 16:
                    res = bundle.LoadAsset<Sprite>("talkinghead.png");
                    break;
            }
            return res;
        }
    }
}
