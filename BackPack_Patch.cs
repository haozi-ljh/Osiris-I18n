using HarmonyLib;
using System;
using UnityEngine;

namespace Osiris_I18n
{
    class BackPack_Patch
    {

        [HarmonyPatch(typeof(ItemInfo), "CopyElements", new Type[] { typeof(ItemInfo) })]
        class ItemInfo_CopyElements_Patch
        {
            public static void Postfix(ItemInfo __instance)
            {
                __instance.specialDisplayName = LoadLocalization.Instance.GetLocalizedString(__instance.specialDisplayName);
                __instance.itemDescription = LoadLocalization.Instance.GetLocalizedString(__instance.itemDescription);
            }
        }
        
        [HarmonyPatch(typeof(ItemInfo), "DescriptionInfo", new Type[] { typeof(string) })]
        class ItemInfo_DescriptionInfo_Patch
        {
            public static void Prefix(ref string info)
            {
                info = LoadLocalization.Instance.GetLocalizedString(info);
            }
        }

        [HarmonyPatch(typeof(PlayerGUI), nameof(PlayerGUI.AddVitalWarning))]
        class PlayerGUI_UpdateModeOverlayUI_Patch
        {
            public static void Prefix(string key, ref string message, Color color)
            {
                if (key.Equals("Overweight"))
                {
                    if (message.IndexOf("Immobilized") >= 0)
                    {
                        string percentage = message.Replace("Immobilized: Backpack at", "").Replace("Capacity", "").Trim();
                        message = LoadLocalization.Instance.GetLocalizedString("Immobilized: Backpack at {0} Capacity", percentage);
                    }
                    else if(message.IndexOf("Severely") >= 0)
                    {
                        string percentage = message.Replace("Severely Encumbered: Backpack at", "").Replace("Capacity", "").Trim();
                        message = LoadLocalization.Instance.GetLocalizedString("Severely Encumbered: Backpack at {0} Capacity", percentage);
                    }
                    else
                    {
                        string percentage = message.Replace("Encumbered: Backpack at", "").Replace("Capacity", "").Trim();
                        message = LoadLocalization.Instance.GetLocalizedString("Encumbered: Backpack at {0} Capacity", percentage);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerInventory), "UpdateInventoryWeight")]
        class PlayerInventory_UpdateInventoryWeight_Patch
        {
            public static void Prefix()
            {
                PlayerGUI.Me.weightText.fontSize = 20;
            }
        }

        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.ActivateGeyser))]
        class PlayerTool_ActivateGeyser_Patch
        {
            public static void Postfix(PlayerTool __instance, bool __result)
            {
                if (__result)
                {
                    string gastype = LoadLocalization.Instance.GetLocalizedString(__instance.textStorageDetected.text.Replace("Detected", "").Trim());
                    __instance.textStorageDetected.text = LoadLocalization.Instance.GetLocalizedString("{0}\n Detected", gastype);
                }
            }
        }
        
        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.ActivatePuddle))]
        class PlayerTool_ActivatePuddle_Patch
        {
            public static void Postfix(PlayerTool __instance, bool __result)
            {
                if (__result)
                {
                    string puddlename = __instance.textStorageDetected.text.Replace("Detected", "").Trim();
                    __instance.textStorageDetected.text = LoadLocalization.Instance.GetLocalizedString("{0}\n Detected", puddlename);
                }
            }
        }
        
        [HarmonyPatch(typeof(PlayerTool), "ActivateRefuelling", new Type[] { typeof(VehicleManager) })]
        class PlayerTool_ActivateRefuelling_Vehicle_Patch
        {
            public static void Postfix(PlayerTool __instance, bool __result)
            {
                if (__result)
                {
                    string fuel = LoadLocalization.Instance.GetLocalizedString(__instance.textStorageDetected.text.Replace("Required Fuel:", "").Trim());
                    __instance.textStorageDetected.text = LoadLocalization.Instance.GetLocalizedString("Required Fuel:") + "\n" + fuel;
                }
            }
        }
        
        [HarmonyPatch(typeof(PlayerTool), "ActivateRefuelling", new Type[] { typeof(FuelStation) })]
        class PlayerTool_ActivateRefuelling_fuelStation_Patch
        {
            public static void Postfix(PlayerTool __instance, bool __result, FuelStation fuelStation)
            {
                if (__result && !string.IsNullOrEmpty(fuelStation.fuelType))
                {
                    string fuel = LoadLocalization.Instance.GetLocalizedString(__instance.textStorageDetected.text.Replace("Required Fuel:", "").Trim());
                    __instance.textStorageDetected.text = LoadLocalization.Instance.GetLocalizedString("Required Fuel:") + "\n" + fuel;
                }
                else
                {
                    __instance.textStorageDetected.text = LoadLocalization.Instance.GetLocalizedString(__instance.textStorageDetected.text);
                }
            }
        }

    }
}
