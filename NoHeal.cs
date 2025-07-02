using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.CardDescriptionNew;
using System;
using static NoHeal.CustomFunctions;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using static NoHeal.Plugin;
// using static MatchManager;

// Make sure your namespace is the same everywhere
namespace NoHeal
{

    [HarmonyPatch] //DO NOT REMOVE/CHANGE

    public class NoHealPatches
    {
        // [HarmonyReversePatch]
        // [HarmonyPatch(typeof(MatchManager), "CreateNPC")]
        // public static void CreateNPCReversePatch(NPCData _npcData,
        //     string effectTarget = "",
        //     int _position = -1,
        //     bool generateFromReload = false,
        //     string internalId = "",
        //     CardData _cardActive = null) =>
        //     //This is intentionally a stub
        //     throw new NotImplementedException("Reverse patch has not been executed.");
        public static string ogPet;
        public static string rarePet;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.DestroyedItemInThisTurn))]

        public static bool DestroyedItemInThisTurnPrefix(MatchManager __instance, int _charIndex, string _cardId)
        {
            if (!OnlyImmortalPurples.Value)
            {
                return true;
            }

            LogDebug("DestroyedItemInThisTurnPrefix");
            Hero targetHero = MatchManager.Instance.GetHero(_charIndex);
            if (targetHero == null) { return true; }
            bool isPet = Globals.Instance?.GetCardData(_cardId)?.CardType == Enums.CardType.Pet;
            LogDebug($"DestroyedItemInThisTurnPrefix is deleting pet: {isPet}");
            if (targetHero.Pet?.EndsWith("rare") ?? false)
            {
                ogPet = _cardId;
                rarePet = Globals.Instance?.GetCardData(_cardId)?.UpgradesToRare?.Id;
                if (ogPet.ToLower().StartsWith("harley"))
                {
                    rarePet = "harleyrare";
                }
                LogDebug($"Protecting Pet! {_cardId}");
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.CreatePet))]

        public static bool CreatePet(CardData cardPet, GameObject charGO, ref Hero _hero, NPC _npc, bool _fromEnchant = false, string _enchantName = "")
        {
            // LogDebug("CreatePet");
            if (!OnlyImmortalPurples.Value)
            {
                return true;
            }

            if (cardPet == null || _hero == null) { return true; }
            LogDebug($"Attempting to create pet {cardPet?.Id}, replacing {(_hero.Pet.IsNullOrWhiteSpace() ? _hero.Pet : "no pet")}");
            if (cardPet.Id.StartsWith("tombstone") && (_hero?.Pet?.EndsWith("rare") ?? false))
            {
                _hero.Pet = ogPet;
                LogDebug($"Protecting Pet! - ogpet = {ogPet}");
                return false;
            }
            return true;
        }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(CardData), nameof(CardData.SetDescriptionNew))]

        // public static void SetDescriptionNew(CardData __instance,
        //     bool forceDescription = false,
        //     Character character = null,
        //     bool includeInSearch = true)
        // {
        //     // LogDebug("CreatePet");

        //     if (__instance == null || !OnlyImmortalPurples.Value || !EssentialsInstalled) { return; }

        //     if (__instance.CardType == Enums.CardType.Pet && __instance.CardUpgraded == Enums.CardUpgraded.Rare)
        //     {
        //         LogDebug($"Attempting to set description for Corrupted Pet: {__instance.Id}");
        //         AddTextToCardDescription("Immortal", TextLocation.ItemBeforeActivation, __instance.Id);
        //     }
        //     return;
        // }

        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(Globals), nameof(Globals.CreateGameContent))]

        // public static void CreateGameContentPostfix()
        // {
        //     // LogDebug("CreatePet");

        //     if (__instance == null || !OnlyImmortalPurples.Value || !EssentialsInstalled) { return; }

        //     if (__instance.CardType == Enums.CardType.Pet && __instance.CardUpgraded == Enums.CardUpgraded.Rare)
        //     {
        //         LogDebug($"Attempting to set description for Corrupted Pet: {__instance.Id}");
        //         AddTextToCardDescription("Immortal", TextLocation.ItemBeforeActivation, __instance.Id);
        //     }
        //     return;
        // }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Globals), nameof(Globals.CreateGameContent))]
        public static void CreateGameContentPostfix(ref Globals __instance, ref Dictionary<string, CardData> ____CardsSource)
        {
            string cardToChange = "twilightslaughter";
            if (____CardsSource.TryGetValue(cardToChange, out CardData twilightslaughter) && !OnlyImmortalPurples.Value)
            {
                LogDebug($"CreateGameContentPostfix - Preventing twilight slaughter from killing pets");
                twilightslaughter.KillPet = false;
                ____CardsSource[cardToChange] = twilightslaughter;
            }
            else
            {
                LogDebug($"CreateGameContentPostfix - Twilight Slaughter not found in CardsSource");
            }
            cardToChange = "twilightslaughtera";
            if (____CardsSource.TryGetValue(cardToChange, out CardData twilightslaughtera) && !OnlyImmortalPurples.Value)
            {
                LogDebug($"CreateGameContentPostfix - Preventing twilight slaughter from killing pets");
                twilightslaughtera.KillPet = false;
                ____CardsSource[cardToChange] = twilightslaughtera;
            }
            {
                LogDebug($"CreateGameContentPostfix - TwilightSlaughterA not found in CardsSource");
            }
            if (EssentialsInstalled || OnlyImmortalPurples.Value)
            {
                List<string> pets = Globals.Instance.CardListByType[Enums.CardType.Pet];
                LogDebug($"CreateGameContent - Adding Immortal to pets - {string.Join(", ", pets)}");
                foreach (string pet in pets)
                {
                    if (pet.EndsWith("rare"))
                    {
                        AddTextToCardDescription("Immortal", TextLocation.ItemBeforeActivation, pet);
                        Globals.Instance.GetCardData(pet, false).SetDescriptionNew(true, null, true);
                        // ____CardsSource[pet].DescriptionNormalized = Globals.Instance.GetCardData(pet, false).DescriptionNormalized;
                    }
                }

            }

        }

    }

}
