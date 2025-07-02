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
        [HarmonyPatch(typeof(Character), nameof(Character.HealReceivedFinal))]

        public static bool HealReceivedFinalPrefix(ref int __result, int heal, bool isIndirect = false, CardData cardAux = null)
        {
            __result = 0;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), "GlobalAuraCurseModificationByTraitsAndItems")]
        // [HarmonyPriority(Priority.Last)]
        public static void GlobalAuraCurseModificationByTraitsAndItemsPostfix(ref AtOManager __instance, ref AuraCurseData __result, string _type, string _acId, Character _characterCaster, Character _characterTarget)
        {
            // LogInfo($"GACM {subclassName}");

            Character characterOfInterest = _type == "set" ? _characterTarget : _characterCaster;
            // string traitOfInterest;
            // bool hasRust = characterOfInterest.EffectCharges("rust") >= 0;
            switch (_acId)
            {

                // trait2b:
                // Sharp on enemies reduces All Resistances by 1% per charge.

                // trait 4b:
                // Sharp on Monsters reduces All Damage by 0.5 per charge

                case "vitality":
                    __result.CharacterStatModified = Enums.CharacterStat.None;
                    __result.CharacterStatModifiedValuePerStack = 0;
                    break;
            }
        }


        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(Globals), nameof(Globals.CreateGameContent))]
        // public static void CreateGameContentPostfix(ref Globals __instance, ref Dictionary<string, CardData> ____CardsSource)
        // {
        //     string cardToChange = "twilightslaughter";
        //     if (____CardsSource.TryGetValue(cardToChange, out CardData twilightslaughter) && !OnlyImmortalPurples.Value)
        //     {
        //         LogDebug($"CreateGameContentPostfix - Preventing twilight slaughter from killing pets");
        //         twilightslaughter.KillPet = false;
        //         ____CardsSource[cardToChange] = twilightslaughter;
        //     }
        //     else
        //     {
        //         LogDebug($"CreateGameContentPostfix - Twilight Slaughter not found in CardsSource");
        //     }
        //     cardToChange = "twilightslaughtera";
        //     if (____CardsSource.TryGetValue(cardToChange, out CardData twilightslaughtera) && !OnlyImmortalPurples.Value)
        //     {
        //         LogDebug($"CreateGameContentPostfix - Preventing twilight slaughter from killing pets");
        //         twilightslaughtera.KillPet = false;
        //         ____CardsSource[cardToChange] = twilightslaughtera;
        //     }
        //     {
        //         LogDebug($"CreateGameContentPostfix - TwilightSlaughterA not found in CardsSource");
        //     }
        //     if (EssentialsInstalled || OnlyImmortalPurples.Value)
        //     {
        //         List<string> pets = Globals.Instance.CardListByType[Enums.CardType.Pet];
        //         LogDebug($"CreateGameContent - Adding Immortal to pets - {string.Join(", ", pets)}");
        //         foreach (string pet in pets)
        //         {
        //             if (pet.EndsWith("rare"))
        //             {
        //                 AddTextToCardDescription("Immortal", TextLocation.ItemBeforeActivation, pet);
        //                 Globals.Instance.GetCardData(pet, false).SetDescriptionNew(true, null, true);
        //                 // ____CardsSource[pet].DescriptionNormalized = Globals.Instance.GetCardData(pet, false).DescriptionNormalized;
        //             }
        //         }

        //     }

        // }

    }

}
