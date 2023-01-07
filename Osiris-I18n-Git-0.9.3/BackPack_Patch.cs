using System;
using UnityEngine;

namespace Osiris_I18n
{
    public class BackPack_Patch
    {

        public BackPack_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(ItemInfo), "CopyElements", new Type[] { typeof(ItemInfo) }, PatchType.postfix, GetType().GetMethod("ItemInfo_CopyElements_Patch")));
            PatcherManager.Add(new Patcher(typeof(ItemInfo), "DescriptionInfo", new Type[] { typeof(string) }, PatchType.prefix, GetType().GetMethod("ItemInfo_DescriptionInfo_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "AddVitalWarning", PatchType.prefix, GetType().GetMethod("PlayerGUI_UpdateModeOverlayUI_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerInventory), "UpdateInventoryWeight", PatchType.prefix, GetType().GetMethod("PlayerInventory_UpdateInventoryWeight_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerTool), "ActivateGeyser", PatchType.postfix, GetType().GetMethod("PlayerTool_ActivateGeyser_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerTool), "ActivatePuddle", PatchType.postfix, GetType().GetMethod("PlayerTool_ActivatePuddle_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerTool), "ActivateRefuelling", new Type[] { typeof(VehicleManager) }, PatchType.postfix, GetType().GetMethod("PlayerTool_ActivateRefuelling_Vehicle_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerTool), "ActivateRefuelling", new Type[] { typeof(FuelStation) }, PatchType.postfix, GetType().GetMethod("PlayerTool_ActivateRefuelling_fuelStation_Patch")));
        }

        public static void ItemInfo_CopyElements_Patch(ItemInfo __instance)
        {
            __instance.specialDisplayName = LoadLocalization.Instance.GetLocalizedString(__instance.specialDisplayName.Trim());
            __instance.itemDescription = LoadLocalization.Instance.GetLocalizedString(__instance.itemDescription.Trim());
        }

        public static void ItemInfo_DescriptionInfo_Patch(ref string info)
        {
            info = LoadLocalization.Instance.GetLocalizedString(info.Trim());
        }

        public static void PlayerGUI_UpdateModeOverlayUI_Patch(string key, ref string message, Color color)
        {
            if (key.Equals("Overweight"))
            {
                if (message.Contains("Immobilized"))
                {
                    string percentage = message.Replace("Immobilized: Backpack at", "").Replace("Capacity", "").Trim();
                    message = LoadLocalization.Instance.GetLocalizedString("Immobilized: Backpack at {0} Capacity", percentage);
                }
                else if (message.Contains("Severely"))
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

        public static void PlayerInventory_UpdateInventoryWeight_Patch()
        {
            PlayerGUI.Me.weightText.fontSize = 20;
        }

        public static void PlayerTool_ActivateGeyser_Patch(PlayerTool __instance, bool __result)
        {
            if (__result)
            {
                string gastype = LoadLocalization.Instance.GetLocalizedString(__instance.textStorageDetected.text.Replace("Detected", "").Trim());
                __instance.textStorageDetected.text = LoadLocalization.Instance.GetLocalizedString("{0}\n Detected", gastype);
            }
        }

        public static void PlayerTool_ActivatePuddle_Patch(PlayerTool __instance, bool __result)
        {
            if (__result)
            {
                string puddlename = __instance.textStorageDetected.text.Replace("Detected", "").Trim();
                __instance.textStorageDetected.text = LoadLocalization.Instance.GetLocalizedString("{0}\n Detected", puddlename);
            }
        }

        public static void PlayerTool_ActivateRefuelling_Vehicle_Patch(PlayerTool __instance, bool __result)
        {
            if (__result)
            {
                string fuel = LoadLocalization.Instance.GetLocalizedString(__instance.textStorageDetected.text.Replace("Required Fuel:", "").Trim());
                __instance.textStorageDetected.text = LoadLocalization.Instance.GetLocalizedString("Required Fuel:") + "\n" + fuel;
            }
        }

        public static void PlayerTool_ActivateRefuelling_fuelStation_Patch(PlayerTool __instance, bool __result, FuelStation fuelStation)
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
