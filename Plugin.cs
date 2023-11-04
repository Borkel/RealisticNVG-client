using EFT;
using Aki.Reflection.Patching;
using BepInEx;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;

namespace BorkelRNVG
{
    [BepInPlugin("com.borkel.nvgmasks", "my very humble attempt at replacing the damn nvg masks", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public Texture2D[] masks;
        public AssetBundle bundle;
        private void Awake()
        {
            //directory contains string of path where the .dll is located, for me it is C:\SPTarkov3.7.1\BepInEx\plugins
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //realmasks is the bundle
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
                    Logger.LogMessage($"mask 0: {masks[0].name}"); //mask 0: mask_anvis
                    Logger.LogMessage($"mask 1: {masks[1].name}"); //mask 1: mask_binocular
                    Logger.LogMessage($"mask 2: {masks[2].name}"); //mask 2: mask_old_monocular

                    //this is just to test that the bundle and the assets are correctly loaded by the .dll
                    //it extracts the .png assets from the bundle, it does it correctly
                    /*foreach (var mask in masks)
                    {
                        // Get the texture name
                        string textureName = mask.name + ".png";

                        // Combine the directory path with the texture name
                        string outputPath = Path.Combine(directory, textureName);

                        // Create a new Texture2D in ARGB32 format and copy the content
                        Texture2D newTexture = new Texture2D(mask.width, mask.height, TextureFormat.ARGB32, false);
                        newTexture.SetPixels(mask.GetPixels());
                        newTexture.Apply();

                        // Encode the new texture to PNG and save it to the directory
                        byte[] bytes = newTexture.EncodeToPNG();
                        File.WriteAllBytes(outputPath, bytes);

                        Logger.LogMessage($"Extracted {textureName} to {outputPath}");
                    }*/
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
    public class GetAssetReturnPatch : ModulePatch
    {
        public static UnityEngine.Object asset { get; set; } // Number we want to equal the result of CalculateInt

        // Inherit GetTargetMethod and have it return with the MethodBase for our requested method
        protected override MethodBase GetTargetMethod()
        {   // BindingFlags.NonPublic as the method is private
            return typeof(AssetsManagerClass).GetMethod("method_0", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        // Create postfix method with PatchPostfix attribute and ref matching the type of the method's result
        [PatchPostfix]
        static void Postfix(ref UnityEngine.Object __result)
        {
            asset = __result; // Get the return value from CalculateInt and set it to our field
            //Logger.LogMessage($"ASSET NAME: {asset.name}");
            Logger.LogMessage($"ASSET------------------------------------");
        }
    }
}
