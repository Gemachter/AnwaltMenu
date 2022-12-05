using System;
using AnwaltMenu;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
[assembly: MelonInfo(typeof(Mod), "Anwalt menu", "0.1.1", "Gemachter")]

namespace AnwaltMenu
{
    public class Mod : MelonMod
    {
        public static PlayerControl? pControl;
        public static float movementSpeed = 0f;
        public static bool infinityJump = false;
        public static bool noAttackCooldown = false;
        public static bool noDamge = false;
        public static bool ignoreLava = false;

        public override void OnInitializeMelon()
        {
            GUI.enabled = true;
        }
        public override void OnUpdate()
        {
            if (pControl != null) {
                if (infinityJump) pControl.isGrounded = true;
                typeof(PlayerControl).GetField("damagable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(Mod.pControl, !noDamge);
            }
        }
        public override void OnGUI()
        {
            if (pControl != null)
            {
                GUI.Window(0, new Rect(20, 20, 200, 300), MainWindow, "Anwalt Menu 0.1.1");
            }
        }
        public static void MainWindow(int windowID)
        {
            GUILayout.Label("Speed Hack");
            movementSpeed = pControl.movementSpeed = GUILayout.HorizontalSlider(pControl.movementSpeed, 1f, 1000f);
            infinityJump = GUILayout.Toggle(infinityJump, "Infinity Jump");
            noAttackCooldown = GUILayout.Toggle(noAttackCooldown, "No Attack Cooldown");
            noDamge = GUILayout.Toggle(noDamge, "No Damge");
            if (GUILayout.Toggle(ignoreLava, "Ignore Lava"))
            {
                if (!ignoreLava)
                {
                    typeof(PlayerControl).GetMethod("NormalJump", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(pControl, new object[] { });
                }
                ignoreLava = true;
            } else
            {
                if (ignoreLava)
                {
                    typeof(PlayerControl).GetMethod("NormalJump", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(pControl, new object[] { });
                }
                ignoreLava = false;
            }
            GUILayout.Label("Health");
            int newHealth = int.Parse(GUILayout.TextField(pControl.health.ToString()));
            if (newHealth != pControl.health)
            {
                pControl.health = newHealth;
                var uiManager = typeof(PlayerControl).GetField("uiManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(pControl);
                ((AnwaltUIManager)uiManager).ChangeLife(newHealth);
            }
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
    }
    [HarmonyPatch(typeof(PlayerControl), "Awake", new Type[] { })]
    public class MapLoadStacktracePatch
    {
        private static void Prefix(PlayerControl __instance)
        {
            if (Mod.movementSpeed != 0f) __instance.movementSpeed = Mod.movementSpeed;
            Mod.pControl = __instance;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), "resetAttaackCooldown", new Type[] { })]
    public class AttackCooldownPatch
    {
        private static bool Prefix(PlayerControl __instance)
        {
            if (Mod.noAttackCooldown) typeof(PlayerControl).GetField("attackEnabledCooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(Mod.pControl, true);
            return !Mod.noAttackCooldown;
        }
    }
    [HarmonyPatch(typeof(BodenIstLavaTest), "OnCollisionEnter2D", new Type[] { typeof(Collision2D) })]
    public class ignoreLavaPatch1
    {
        private static bool Prefix()
        {
            return !Mod.ignoreLava;
        }
    }
    [HarmonyPatch(typeof(DauerLava), "OnCollisionEnter2D", new Type[] { typeof(Collision2D) })]
    public class ignoreLavaPatch2
    {
        private static bool Prefix()
        {
            return !Mod.ignoreLava;
        }
    }

}
