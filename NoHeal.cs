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

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(Character), nameof(Character.HealReceivedFinal))]

        // public static bool HealReceivedFinalPrefix(Character __instance, ref int __result, int heal, bool isIndirect = false, CardData cardAux = null)
        // {
        //     if (__instance.IsHero || (PreventNPCHealing.Value && !__instance.IsHero))
        //     {
        //         __result = 0;
        //         return false;
        //     }
        //     return true;
        // }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Character), nameof(Character.HealReceivedFinal))]

        public static void HealReceivedFinalPostfix(Character __instance, ref int __result, int heal, bool isIndirect = false, CardData cardAux = null)
        {
            if (IsCharacterValid(__instance))
            {
                __result = (SetHealToOne.Value && __result != 0) ? 1 : 0;
                // return false;
            }
            // return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), nameof(Character.SimpleHeal))]

        public static bool SimpleHealPrefix(Character __instance, ref int heal)
        {
            if (SetHealToOne.Value && heal != 0)
            {
                heal = 1;
            }
            if (IsCharacterValid(__instance) && !SetHealToOne.Value)
            {
                LogDebug($"Prevented healing for {__instance.Id} with heal {heal}");
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), nameof(Character.PercentHeal))]

        public static bool PercentHealPrefix(Character __instance, float _healPercent, bool _includeInStats)
        {
            if (IsCharacterValid(__instance))
            {
                // __result = (SetHealToOne.Value && __result != 0) ? 1 : 0;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AtOManager), nameof(AtOManager.ModifyHeroLife))]

        public static bool ModifyHeroLifePrefix(AtOManager __instance, ref int _flat, ref float _percent, int _heroIndex = -1)
        {
            if (DisableOutOfCombatHealing.Value && (_flat > 0 || _percent > 0.0f) && !SetHealToOne.Value)
            {
                // __result = (SetHealToOne.Value && __result != 0) ? 1 : 0;
                return false;
            }
            if (SetHealToOne.Value && (_flat > 0 || _percent > 0.0f))
            {
                _flat = 1;
                _percent = 0.0f;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), nameof(Character.HealAttacker))]

        public static bool HealAttackerPrefix(Hero theCasterHero, NPC theCasterNPC)
        {
            if ((theCasterHero != null || (PreventNPCHealing.Value && theCasterNPC != null)) && !SetHealToOne.Value)
            {
                // __result = (SetHealToOne.Value && __result != 0) ? 1 : 0;
                return false;
            }
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Item), "DoItemData")]

        public static void DoItemDataPrefix(
            Character target,
            string itemName,
            int auxInt,
            CardData cardItem,
            string itemType,
            ref ItemData itemData,
            Character character,
            int order,
            string castedCardId = "",
            Enums.EventActivation theEvent = Enums.EventActivation.None)
        {
            if (itemData != null && itemData.HealQuantity != 0)
            {
                itemData.HealQuantity = SetHealToOne.Value ? 1 : 0;
            }
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
                    if (characterOfInterest.IsHero || (PreventNPCHealing.Value && !characterOfInterest.IsHero))
                    {
                        if (SetHealToOne.Value)
                        {
                            // __result.CharacterStatAbsolute = true;
                            __result.CharacterStatModifiedValue = 1;
                            __result.CharacterStatModifiedValuePerStack = 0;
                        }
                        else
                        {
                            __result.CharacterStatModified = Enums.CharacterStat.None;
                            __result.CharacterStatModifiedValuePerStack = 0;
                        }


                    }
                    break;
            }



        }
        public static bool IsCharacterValid(Character __instance)
        {
            return __instance.IsHero || (PreventNPCHealing.Value && !__instance.IsHero);
        }

    }

}
