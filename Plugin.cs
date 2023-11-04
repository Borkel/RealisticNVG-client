using EFT;
using Aki.Reflection.Patching;
using Comfort.Common;
using BepInEx;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using BSG.CameraEffects;
using System.Xml.Linq;

namespace BorkelRNVG
{
    [BepInPlugin("com.borkel.nvgmasks", "my very humble attempt at replacing the damn nvg masks", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Texture2D[] masks;
        public AssetBundle bundle;
        private void Awake()
        {
            //directory contains string of path where the .dll is located, for me it is C:\SPTarkov3.7.1\BepInEx\plugins
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //realmasks is the bundle, Fontaine suggests to load the masks from PNGs isntead of a bundle, should change it
            string bundlePath = $"{directory}\\BorkelRNVG\\realmasks";
            Logger.LogMessage($"directory {directory} bundlepath {bundlePath}");
            //Load all assets from the bundle realmasks located in C:\SPTarkov3.7.1\BepInEx\plugins\BorkelRNVG
            if (File.Exists(bundlePath))
            {
                bundle = AssetBundle.LoadFromFile(bundlePath); //loads the modified bundle resources.assets
                if (bundle != null)
                {
                    Logger.LogMessage($"---BUNDLE LOADED---");
                    masks = bundle.LoadAllAssets<Texture2D>(); //loads all assets, including the important ones: mask_anvis, mask_binocular, mask_old_monocular
                    Logger.LogMessage($"Texture2D 0: {masks[0].name}"); //mask 0: mask_anvis
                    Logger.LogMessage($"Texture2D 1: {masks[1].name}"); //mask 1: mask_binocular
                    Logger.LogMessage($"Texture2D 2: {masks[2].name}"); //mask 2: mask_old_monocular
                    //new AccessNightVisiondPatch().Enable();
                    /*Type NVType = typeof(NightVision);
                    FieldInfo NVFieldInfo = NVType.GetField("Mask", BindingFlags.Public | BindingFlags.Instance);
                    if (!Singleton<GameWorld>.Instantiated)
                    {
                        Logger.LogError("Failed to load NightVision instance.");
                    }
                        Logger.LogMessage($"Value of Mask is: {NVFieldInfo.GetValue(Singleton<NightVision>.Instance)}");*/
                }
                else
                {
                    Logger.LogError("Failed to load the asset bundle.");
                }
            }
            else
            {
                Logger.LogError("Asset bundle not found.");
            }
        }
    }
    public class AccessNightVisionController : MonoBehaviour
    {
        private static NightVision NV; // field we'll use to hold the GameWorld instance
        // mono method called every frame
        void Update()
        {
            // check if game world is instantiated, instatiated = exists in scene
            if (!Singleton<NightVision>.Instantiated) return; // don't allow logic to run if not instantiated

            NV = Singleton<NightVision>.Instance;
            //Logger.LogMessage($"Value of Mask is: {NVFieldInfo.GetValue(Singleton<NightVision>.Instance)}");
            FieldInfo myMask = typeof(NightVision).GetField("Mask", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo NotMyAnvis = typeof(NightVision).GetField("AnvisMaskTexture", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo NotMyBino = typeof(NightVision).GetField("BinocularMaskTexture", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo NotMyMono = typeof(NightVision).GetField("OldMonocularMaskTexture", BindingFlags.Public | BindingFlags.Instance);
            Texture2D mask = (Texture2D)myMask.GetValue(NV);
            Texture2D anvis = (Texture2D)NotMyAnvis.GetValue(NV);
            Texture2D bino = (Texture2D)NotMyBino.GetValue(NV);
            Texture2D mono = (Texture2D)NotMyMono.GetValue(NV);
            if (mask.name == anvis.name)
            {
                myMask.SetValue(NV, Plugin.masks[0]);
            }
            else if (mask.name == bino.name)
            {
                myMask.SetValue(NV, Plugin.masks[1]);
            }
            else if (mask.name == mono.name)
            {
                myMask.SetValue(NV, Plugin.masks[2]);
            }
            NotMyAnvis.SetValue(NV, Plugin.masks[0]);
            NotMyBino.SetValue(NV, Plugin.masks[1]);
            NotMyMono.SetValue(NV, Plugin.masks[2]);

        }
    }
    public class AccessNightVisiondPatch : ModulePatch
    {
        // Inherit GetTargetMethod from ModulePatch
        protected override MethodBase GetTargetMethod()
        {   // Get method from type, use BindingFlags.Public as it's a public method
            return typeof(NightVision).GetMethod("SetMask", BindingFlags.Public | BindingFlags.Instance);
        }
        // Will be invoked after the "OnGameStarted" method is ran and the world is loaded
        // from here you can access objects in the game world
        [PatchPostfix]
        void PostFix()
        {
            // put logic here
            /*FieldInfo myMask=typeof(NightVision).GetField("Mask", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo myAnvis=typeof(NightVision).GetField("AnvisMaskTexture", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo myBi=typeof(NightVision).GetField("BinocularMaskTexture", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo myMono=typeof(NightVision).GetField("OldMonocularMaskTexture", BindingFlags.Public | BindingFlags.Instance);*/
            /*Logger.LogError("POSTFIX-BORKEL");
            Logger.LogError($"Mask: {AccessNightVisionController.Update().Mask.name}");
            if (AccessNightVisionController.Update().Mask.name == AccessNightVisionController.Update().AnvisMaskTexture.name)
            {
                AccessNightVisionController.Update().Mask = Plugin.masks[0];
            }
            else if (AccessNightVisionController.Update().Mask.name == AccessNightVisionController.Update().BinocularMaskTexture.name)
            {
                AccessNightVisionController.Update().Mask = Plugin.masks[1];
            }
            else if (AccessNightVisionController.Update().Mask.name == AccessNightVisionController.Update().OldMonocularMaskTexture.name)
            {
                AccessNightVisionController.Update().Mask = Plugin.masks[2];
            }
            AccessNightVisionController.Update().AnvisMaskTexture = Plugin.masks[0];
            AccessNightVisionController.Update().BinocularMaskTexture = Plugin.masks[1];
            AccessNightVisionController.Update().OldMonocularMaskTexture = Plugin.masks[2];*/
        }
    }
}

/*            if(__instance.Mask.name == __instance.AnvisMaskTexture.name)
            {
                __instance.Mask = masks[0];
            }
            else if (__instance.Mask.name == __instance.BinocularMaskTexture.name)
            {
                __instance.Mask = masks[1];
            }
            else if (__instance.Mask.name == __instance.OldMonocularMaskTexture.name)
            {
                __instance.Mask = masks[2];
            }
            __instance.AnvisMaskTexture = masks[0];
            __instance.BinocularMaskTexture = masks[1];
            __instance.OldMonocularMaskTexture = masks[2];
*/