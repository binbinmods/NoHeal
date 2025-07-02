// These are your imports, mostly you'll be needing these 5 for every plugin. Some will need more.

using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using static Obeliskial_Essentials.CardDescriptionNew;
using System;
using System.Runtime.CompilerServices;
using BepInEx.Bootstrap;
using System.Collections.Generic;


// The Plugin csharp file is used to 


// Make sure all your files have the same namespace and this namespace matches the RootNamespace in the .csproj file
namespace NoHeal
{
    // These are used to create the actual plugin. If you don't need Obeliskial Essentials for your mod, 
    // delete the BepInDependency and the associated code "RegisterMod()" below.

    // If you have other dependencies, such as obeliskial content, make sure to include them here.
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials", BepInDependency.DependencyFlags.SoftDependency)] // this is the name of the .dll in the !libs folder.
    [BepInProcess("AcrossTheObelisk.exe")] //Don't change this
    // [BepInIncompatibility("com.stiffmeds.obeliskialessentials")]

    // If PluginInfo isn't working, you are either:
    // 1. Using BepInEx v6
    // 2. Have an issue with your csproj file (not loading the analyzer or BepInEx appropriately)
    // 3. You have an issue with your solution file (not referencing the correct csproj file)


    public class Plugin : BaseUnityPlugin
    {

        // If desired, you can create configs for users by creating a ConfigEntry object here, 
        // and then use config = Config.Bind() to set the title, default value, and description of the config.
        // It automatically creates the appropriate configs.
        public static ConfigEntry<bool> EnableMod { get; set; }
        public static ConfigEntry<bool> EnableDebugging { get; set; }

        public static ConfigEntry<bool> OnlyImmortalPurples { get; set; }
        public static bool EssentialsInstalled = false;
        public static string debugBase = $"{PluginInfo.PLUGIN_GUID} ";

        internal int ModDate = 20250605; //int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;
        private void Awake()
        {

            Log = Logger;
            Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");

            EnableMod = Config.Bind(new ConfigDefinition(PluginInfo.PLUGIN_NAME, "EnableMod"), true, new ConfigDescription("Enables the mod. If false, the mod will not work then next time you load the game."));
            EnableDebugging = Config.Bind(new ConfigDefinition(PluginInfo.PLUGIN_NAME, "EnableDebugging"), true, new ConfigDescription("Enables the debugging"));
            OnlyImmortalPurples = Config.Bind(new ConfigDefinition(PluginInfo.PLUGIN_NAME, "OnlyImmortalPurples"), false, new ConfigDescription("Makes it so that only purple (corrupted) pets are immortal"));

            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} second test (pre-register)");
            // OnlyImmortalPurples.Value = true;
            EssentialsInstalled = Chainloader.PluginInfos.ContainsKey("com.stiffmeds.obeliskialessentials");
            // LogDebug($"EssentialsInstalled - {string.Join(", ", Chainloader.PluginInfos.Keys)} - Is Essentials Installed: {EssentialsInstalled}, Is OnlyPurples Enabled: {OnlyImmortalPurples.Value}");
            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} Config Values. Spawn: " + GuaranteedSpawn.Value + " Jade: " + OnlySpawnJades.Value + " Percent: " + PercentChanceToSpawn.Value);

            // Register with Obeliskial Essentials
            if (EssentialsInstalled)
            {
                RegisterMod(
                    _name: PluginInfo.PLUGIN_NAME,
                    _author: "binbin",
                    _description: "Slaughter Prevention",
                    _version: PluginInfo.PLUGIN_VERSION,
                    _date: ModDate,
                    _link: @"https://github.com/binbinmods/NoHeal"
                );

                // List<string> pets = ["betty", "champy", "chompy", "chumpy", "asmody", "cuby", "cubyd"];
                // foreach (string pet in pets)
                // {
                //     AddTextToCardDescription("Immortal", TextLocation.ItemBeforeActivation, pet + "rare");
                // }
            }

            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} third test (pre patch)");

            // apply patches
            if (EnableMod.Value)
                harmony.PatchAll();

            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} fourth test(post patch)");

        }

        internal static void LogDebug(string msg)
        {
            if (EnableDebugging.Value)
            {
                Log.LogDebug(debugBase + msg);
            }

        }
        internal static void LogInfo(string msg)
        {
            Log.LogInfo(debugBase + msg);
        }
        internal static void LogError(string msg)
        {
            Log.LogError(debugBase + msg);
        }
    }
}