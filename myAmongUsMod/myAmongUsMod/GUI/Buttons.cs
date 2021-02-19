using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Extensions;
using Reactor.Unstrip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;
using static GameData;

namespace StandMod.GUI
{    
    // copied and modified from https://gist.github.com/gabriel-nsiqueira/827dea0a1cdc2210db6f9a045ec4ce0a
    // modified by RedFrog6002

    public enum Category
    {
        Everyone,
        OnlyCrewmate,
        OnlyImpostor,
        OnlyCustom
    }

    //In HudManager.Start, Initialize the class
    //HudManager.Update, call CooldownButton.HudUpdate
    public class CooldownButton
    {
        public static List<CooldownButton> buttons = new List<CooldownButton>();
        public KillButtonManager killButtonManager;
        private Color startColorButton = new Color(255, 255, 255);
        private Color startColorText = new Color(255, 255, 255);
        public Vector2 PositionOffset = Vector2.zero;
        public float MaxTimer = 0f;
        public float Timer = 0f;
        public float EffectDuration = 0f;
        public bool isEffectActive;
        public bool hasEffectDuration;
        public bool enabled = true;
        public Category category;
        public Func<PlayerInfo, bool> customCategory;
        private string ResourceName;
        private Action OnClick;
        private Action OnEffectEnd;
        private HudManager hudManager;
        private float pixelsPerUnit;
        private bool canUse;

        public CooldownButton(Action OnClick, float Cooldown, string ImageEmbededResourcePath, float PixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager, float EffectDuration, Action OnEffectEnd, Sprite sprite = null)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.OnEffectEnd = OnEffectEnd;
            this.PositionOffset = PositionOffset;
            this.EffectDuration = EffectDuration;
            this.category = category;
            pixelsPerUnit = PixelsPerUnit;
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            ResourceName = ImageEmbededResourcePath;
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start(sprite);
        }

        public CooldownButton(Action OnClick, float Cooldown, string ImageEmbededResourcePath, float PixelsPerUnit, Vector2 PositionOffset, Func<PlayerInfo, bool> customCategory, HudManager hudManager, float EffectDuration, Action OnEffectEnd, Sprite sprite = null)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.OnEffectEnd = OnEffectEnd;
            this.PositionOffset = PositionOffset;
            this.EffectDuration = EffectDuration;
            this.customCategory = customCategory;
            category = Category.OnlyCustom;
            pixelsPerUnit = PixelsPerUnit;
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            ResourceName = ImageEmbededResourcePath;
            hasEffectDuration = true;
            isEffectActive = false;
            buttons.Add(this);
            Start(sprite);
        }

        public CooldownButton(Action OnClick, float Cooldown, string ImageEmbededResourcePath, float pixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager, Sprite sprite = null)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.pixelsPerUnit = pixelsPerUnit;
            this.PositionOffset = PositionOffset;
            this.category = category;
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            ResourceName = ImageEmbededResourcePath;
            hasEffectDuration = false;
            buttons.Add(this);
            Start(sprite);
        }

        public CooldownButton(Action OnClick, float Cooldown, string ImageEmbededResourcePath, float pixelsPerUnit, Vector2 PositionOffset, Func<PlayerInfo, bool> customCategory, HudManager hudManager, Sprite sprite = null)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.pixelsPerUnit = pixelsPerUnit;
            this.PositionOffset = PositionOffset;
            this.customCategory = customCategory;
            category = Category.OnlyCustom;
            MaxTimer = Cooldown;
            Timer = MaxTimer;
            ResourceName = ImageEmbededResourcePath;
            hasEffectDuration = false;
            buttons.Add(this);
            Start(sprite);
        }

        private void Start(Sprite sprite = null)
        {
            killButtonManager = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.transform);
            startColorButton = killButtonManager.renderer.color;
            startColorText = killButtonManager.TimerText.Color;
            killButtonManager.gameObject.SetActive(true);
            killButtonManager.renderer.enabled = true;
            if (sprite == null)
            {
                Texture2D tex = GUIExtensions.CreateEmptyTexture();
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream myStream = assembly.GetManifestResourceStream(ResourceName);
                byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
                ImageConversion.LoadImage(tex, buttonTexture, false);
                killButtonManager.renderer.sprite = GUIExtensions.CreateSprite(tex);
            }
            else
                killButtonManager.renderer.sprite = sprite;
            PassiveButton button = killButtonManager.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction)listener);
            void listener()
            {
                if (Timer < 0f && canUse)
                {
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                    if (hasEffectDuration)
                    {
                        isEffectActive = true;
                        Timer = EffectDuration;
                        killButtonManager.TimerText.Color = new Color(0, 255, 0);
                    }
                    else
                    {
                        Timer = MaxTimer;
                    }
                    OnClick();
                }
            }
        }
        public bool CanUse()
        {
            if (PlayerControl.LocalPlayer.Data == null) return false;
            switch (category)
            {
                case Category.Everyone:
                    {
                        canUse = true;
                        break;
                    }
                case Category.OnlyCrewmate:
                    {
                        canUse = !PlayerControl.LocalPlayer.Data.IsImpostor;
                        break;
                    }
                case Category.OnlyImpostor:
                    {
                        canUse = PlayerControl.LocalPlayer.Data.IsImpostor;
                        break;
                    }
                case Category.OnlyCustom:
                    {
                        canUse = customCategory.Invoke(PlayerControl.LocalPlayer.Data);
                        break;
                    }
            }
            return true;
        }
        public static void HudUpdate()
        {
            buttons.RemoveAll(item => item.killButtonManager == null);
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].CanUse())
                    buttons[i].Update();
            }
        }
        private void Update()
        {
            if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
            {
                killButtonManager.gameObject.SetActive(false);
                killButtonManager.renderer.enabled = false;
                return;
            }
                if (killButtonManager.transform.localPosition.x > 0f)
                killButtonManager.transform.localPosition = new Vector3((killButtonManager.transform.localPosition.x + 1.3f) * -1, killButtonManager.transform.localPosition.y, killButtonManager.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);
            if (Timer < 0f)
            {
                killButtonManager.renderer.color = new Color(1f, 1f, 1f, 1f);
                if (isEffectActive)
                {
                    killButtonManager.TimerText.Color = startColorText;
                    Timer = MaxTimer;
                    isEffectActive = false;
                    OnEffectEnd();
                }
            }
            else
            {
                if (canUse)
                    Timer -= Time.deltaTime;
                killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
            }
            killButtonManager.gameObject.SetActive(canUse);
            killButtonManager.renderer.enabled = canUse;
            if (canUse)
            {
                killButtonManager.renderer.material.SetFloat("_Desat", 0f);
                killButtonManager.SetCoolDown(Timer, MaxTimer);
            }
        }

        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        public static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");

            var il2cppArray = (Il2CppStructArray<byte>)data;

            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }
        public void SetSprite(Sprite sprite)
        {
            killButtonManager.renderer.sprite = sprite;
        }

        public void EndAbility()
        {
            killButtonManager.TimerText.Color = startColorText;
            Timer = MaxTimer;
            isEffectActive = false;
        }


        public override string ToString()
        {
            return this.ResourceName;
        }
    }



    public class UseButton
    {
        public static List<UseButton> buttons = new List<UseButton>();
        public KillButtonManager killButtonManager;
        private Color startColorButton = new Color(255, 255, 255);
        public Vector2 PositionOffset = Vector2.zero;
        public Category category;
        public Func<PlayerInfo, bool> customCategory;
        private string ResourceName;
        private Action OnClick;
        private Func<bool> UpdateEnabled;
        private HudManager hudManager;
        private float pixelsPerUnit;
        private bool canUse;
        private bool useable;
        private KeyCode key = KeyCode.None;
        private bool haskey;
        private bool clicked = false;

        public UseButton(Action OnClick, string ImageEmbededResourcePath, float PixelsPerUnit, Vector2 PositionOffset, Category category, HudManager hudManager, Func<bool> EnabledUpdate, bool isenabled = false, KeyCode button = KeyCode.None, Sprite sprite = null)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.PositionOffset = PositionOffset;
            this.category = category;
            key = button;
            haskey = key != KeyCode.None;
            UpdateEnabled = EnabledUpdate;
            useable = isenabled;
            pixelsPerUnit = PixelsPerUnit;
            ResourceName = ImageEmbededResourcePath;
            buttons.Add(this);
            Start(sprite);
        }



        public UseButton(Action OnClick, string ImageEmbededResourcePath, float PixelsPerUnit, Vector2 PositionOffset, Func<PlayerInfo, bool> customCategory, HudManager hudManager, Func<bool> EnabledUpdate, bool isenabled = false, KeyCode button = KeyCode.None, Sprite sprite = null)
        {
            this.hudManager = hudManager;
            this.OnClick = OnClick;
            this.PositionOffset = PositionOffset;
            this.customCategory = customCategory;
            key = button;
            haskey = key != KeyCode.None;
            UpdateEnabled = EnabledUpdate;
            useable = isenabled;
            category = Category.OnlyCustom;
            pixelsPerUnit = PixelsPerUnit;
            ResourceName = ImageEmbededResourcePath;
            buttons.Add(this);
            Start(sprite);
        }

        private void Start(Sprite sprite = null)
        {
            killButtonManager = UnityEngine.Object.Instantiate(hudManager.KillButton, hudManager.transform);
            startColorButton = killButtonManager.renderer.color;
            killButtonManager.TimerText.gameObject.SetActive(false);
            killButtonManager.gameObject.SetActive(true);
            killButtonManager.renderer.enabled = true;
            if (sprite == null)
            {
                Texture2D tex = GUIExtensions.CreateEmptyTexture();
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream myStream = assembly.GetManifestResourceStream(ResourceName);
                byte[] buttonTexture = Reactor.Extensions.Extensions.ReadFully(myStream);
                ImageConversion.LoadImage(tex, buttonTexture, false);
                killButtonManager.renderer.sprite = GUIExtensions.CreateSprite(tex);
            }
            else
                killButtonManager.renderer.sprite = sprite;
            PassiveButton button = killButtonManager.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction)listener);
        }
        void listener()
        {
            if (canUse && useable)
            {
                killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                clicked = true;
                OnClick();
            }
        }
        public bool CanUse()
        {
            if (PlayerControl.LocalPlayer.Data == null) return false;
            switch (category)
            {
                case Category.Everyone:
                    {
                        canUse = true;
                        break;
                    }
                case Category.OnlyCrewmate:
                    {
                        canUse = !PlayerControl.LocalPlayer.Data.IsImpostor;
                        break;
                    }
                case Category.OnlyImpostor:
                    {
                        canUse = PlayerControl.LocalPlayer.Data.IsImpostor;
                        break;
                    }
                case Category.OnlyCustom:
                    {
                        canUse = customCategory.Invoke(PlayerControl.LocalPlayer.Data);
                        break;
                    }
            }
            useable = UpdateEnabled != null ? UpdateEnabled.Invoke() : useable;
            if (useable && canUse && Input.GetKeyDown(key) && haskey)
                listener();
            return true;
        }
        public static void HudUpdate()
        {
            buttons.RemoveAll(item => item.killButtonManager == null);
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].CanUse())
                    buttons[i].Update();
            }
        }
        private void Update()
        {
            if (killButtonManager.transform.localPosition.x > 0f)
                killButtonManager.transform.localPosition = new Vector3((killButtonManager.transform.localPosition.x + 1.3f) * -1, killButtonManager.transform.localPosition.y, killButtonManager.transform.localPosition.z) + new Vector3(PositionOffset.x, PositionOffset.y);
            if (!clicked)
            {
                if (useable)
                {
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    killButtonManager.renderer.color = new Color(1f, 1f, 1f, 0.3f);
                }
            }
            else
                clicked = false;
            killButtonManager.gameObject.SetActive(canUse);
            killButtonManager.renderer.enabled = canUse;
            if (canUse)
            {
                killButtonManager.renderer.material.SetFloat("_Desat", 0f);
            }
        }
        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
        public static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");

            var il2cppArray = (Il2CppStructArray<byte>)data;

            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }
        public void SetSprite(Sprite sprite)
        {
            killButtonManager.renderer.sprite = sprite;
        }
        public void SetUsable()
        {
            useable = true;
        }
        public void SetUnusable()
        {
            useable = false;
        }
    }
}
