using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections;

namespace Osiris_I18n
{
    public class Other_Patch
    {

        public Other_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(AvatarSelection), "SetAvatarSettings", PatchType.postfix, GetType().GetMethod("AvatarSelection_SetAvatarSettings_Patch")));
            PatcherManager.Add(new Patcher(typeof(BuildableObject), "BuildOwnershipString", PatchType.postfix, GetType().GetMethod("BuildableObject_BuildOwnershipString_Patch")));
            PatcherManager.Add(new Patcher(typeof(BuildableObject), "ownershipString", PropertyType.set, PatchType.prefix, GetType().GetMethod("BuildableObject_ownershipString_Patch")));
            PatcherManager.Add(new Patcher(typeof(CraftingTable), "LoadCraftingTableWithUpgrades", PatchType.postfix, GetType().GetMethod("CraftingTable_LoadCraftingTableWithUpgrades_Patch")));
            PatcherManager.Add(new Patcher(typeof(CreateAvatar), "SetClassDescription", new Type[] { typeof(AvatarClass) }, PatchType.postfix, GetType().GetMethod("CreateAvatar_SetClassDescription_Patch")));
            PatcherManager.Add(new Patcher(typeof(DisplayWeather), "ShowWeather", PatchType.postfix, GetType().GetMethod("DisplayWeather_ShowWeather_Patch")));
            PatcherManager.Add(new Patcher(typeof(LoadingScreenHint), "GetRandomHint", PatchType.prefix, GetType().GetMethod("LoadingScreenHint_GetRandomHint_Patch")));
            PatcherManager.Add(new Patcher(typeof(LoadingScreenHint), "Start", PatchType.postfix, GetType().GetMethod("LoadingScreenHint_Start_Patch")));
            PatcherManager.Add(new Patcher(typeof(LoadLevelManager), "SetStatusSmall", new Type[] { typeof(string) }, PatchType.postfix, GetType().GetMethod("LoadLevelManager_SetStatusSmall_Patch")));
            PatcherManager.Add(new Patcher(typeof(LocalizationUIHelper), "OnLocalizationUpdate", PatchType.postfix, GetType().GetMethod("LocalizationUIHelper_OnLocalizationUpdate_Patch")));
            PatcherManager.Add(new Patcher(typeof(MainMenu), "BringInSelectPlayer", PatchType.postfix, GetType().GetMethod("MainMenu_BringInSelectPlayer_Patch")));
            PatcherManager.Add(new Patcher(typeof(Narator), "DismissLocationInfo", PatchType.postfix, GetType().GetMethod("Narator_DismissLocationInfo_Patch")));
            PatcherManager.Add(new Patcher(typeof(Narator), "DisplayZoneInfo", PatchType.prefix, GetType().GetMethod("Narator_DisplayZoneInfo_Patch")));
            PatcherManager.Add(new Patcher(typeof(PDA_SelectionCircleManager), "OnEnable", PatchType.postfix, GetType().GetMethod("PDA_SelectionCircleManager_OnEnable_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerMovement), "CheckGroundStatus", PatchType.postfix, GetType().GetMethod("PlayerMovement_CheckGroundStatus_Patch")));
            PatcherManager.Add(new Patcher(typeof(PopupMessage), "SetCustom", PatchType.prefix, GetType().GetMethod("PopupMessage_SetCustom_Patch")));
            PatcherManager.Add(new Patcher(typeof(PulseColor), "CycleText", PatchType.postfix, GetType().GetMethod("PulseColor_CycleText_Patch")));
            PatcherManager.Add(new Patcher(typeof(SelectAvatar), "SelectExistingAvatar", PatchType.postfix, GetType().GetMethod("SelectAvatar_SelectExistingAvatar_Patch")));
            PatcherManager.Add(new Patcher(typeof(UIBuildHint), "RefreshButton", PatchType.postfix, GetType().GetMethod("UIBuildHint_RefreshButton_Patch")));
            PatcherManager.Add(new Patcher(typeof(UIHintDisplay), "ActivateInfo", PatchType.postfix, GetType().GetMethod("UIHintDisplay_ActivateInfo_Patch")));
            PatcherManager.Add(new Patcher(typeof(UniverseOptionsManager), "InitializeSettings", PatchType.postfix, GetType().GetMethod("UniverseOptionsManager_InitializeSettings_Patch")));
            PatcherManager.Add(new Patcher(typeof(UniverseOptionsManager.Setting), "Set", PatchType.postfix, GetType().GetMethod("UniverseOptionsManager__Setting_Set_Patch")));
            PatcherManager.Add(new Patcher(typeof(VehicleGUI), "Awake", PatchType.postfix, GetType().GetMethod("VehicleGUI_Awake_Patch")));
            PatcherManager.Add(new Patcher(typeof(VehicleHUDDisplay), "ActivateHUDInterplanetaryTravel", PatchType.postfix, GetType().GetMethod("VehicleHUDDisplay_ActivateHUDInterplanetaryTravel_Patch")));
            PatcherManager.Add(new Patcher(typeof(VehicleHUDDisplay), "ActivateHUDLowOrbitTravel", PatchType.postfix, GetType().GetMethod("VehicleHUDDisplay_ActivateHUDLowOrbitTravel_Patch")));
            PatcherManager.Add(new Patcher(typeof(VitalReadout), "Awake", PatchType.postfix, GetType().GetMethod("VitalReadout_Awake_Patch")));
            PatcherManager.Add(new Patcher(typeof(VitalReadout), "UpdateVitalsOxygen", PatchType.postfix, GetType().GetMethod("VitalReadout_UpdateVitalsOxygen_Patch")));
            PatcherManager.Add(new Patcher(typeof(World), "PlanetDisplayName", PatchType.postfix, GetType().GetMethod("World_PlanetDisplayName_Patch")));

            PatcherManager.Add(new Patcher(Patcher.Find("UIVitalDisplay", "ActivateVitalDisplay", ts: new Type[] { typeof(VitalReadout) }), PatchType.postfix, GetType().GetMethod("UIVitalDisplay_ActivateVitalDisplay_Patch")));
        }

        //人物选择界面信息
        public static void AvatarSelection_SetAvatarSettings_Patch(AvatarSelection __instance)
        {
            __instance.textName.text = __instance.textName.text.Replace("Sol ", LoadLocalization.Instance.GetLocalizedString("Sol "));
            string[] classtexts = __instance.textClass.text.Split(' ');
            __instance.textClass.text = __instance.textClass.text.Replace("Level ", LoadLocalization.Instance.GetLocalizedString("Level ")).Replace(classtexts[classtexts.Length - 2], LoadLocalization.Instance.GetLocalizedString(classtexts[classtexts.Length - 2])).Replace(classtexts[classtexts.Length - 1], LoadLocalization.Instance.GetLocalizedString(classtexts[classtexts.Length - 1]));
            string diftext = __instance.textDifficulty.text.Replace("Difficulty: ", "").Trim();
            __instance.textDifficulty.text = LoadLocalization.Instance.GetLocalizedString("Difficulty: ") + LoadLocalization.Instance.GetLocalizedString(diftext);
        }
        //人物选择界面信息

        public static void BuildableObject_BuildOwnershipString_Patch(ref StringBuilder __result)
        {
            string str = __result.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                __result.Clear();
                if (str.Contains("Colony"))
                {
                    string text = str.Replace("Owned by Colony", "").Trim();
                    __result.AppendFormat(LoadLocalization.Instance.GetLocalizedString("Owned by Colony {0}"), text);
                }
                if (str.Contains("Me"))
                {
                    __result.Append(LoadLocalization.Instance.GetLocalizedString(str));
                }
                if (str.Contains("Avatar"))
                {
                    string text = str.Replace("Owned by Avatar", "").Trim();
                    __result.AppendFormat(LoadLocalization.Instance.GetLocalizedString("Owned by Avatar {0}"), text);
                }
                if (str.Contains("Unowned"))
                {
                    __result.Append(LoadLocalization.Instance.GetLocalizedString(str));
                }
                else
                {
                    string text = str.Replace("Owned by", "").Trim();
                    __result.AppendFormat(LoadLocalization.Instance.GetLocalizedString("Owned by {0}"), text);
                }
            }
        }

        public static void BuildableObject_ownershipString_Patch(ref string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("Me"))
                {
                    value = LoadLocalization.Instance.GetLocalizedString(value);
                }
                if (value.Contains("Avatar"))
                {
                    string text = value.Replace("Owned by Avatar", "").Trim();
                    value = LoadLocalization.Instance.GetLocalizedString("Owned by Avatar {0}", text);
                }
                if (value.Contains("Unowned"))
                {
                    value = LoadLocalization.Instance.GetLocalizedString(value);
                }
                else
                {
                    string text = value.Replace("Owned by", "").Trim();
                    value = LoadLocalization.Instance.GetLocalizedString("Owned by {0}", text);
                }
            }
        }

        //升级信息
        public static void CraftingTable_LoadCraftingTableWithUpgrades_Patch(string name, ref List<UpgradeOption> upgrades)
        {
            foreach (UpgradeOption uo in upgrades)
            {
                uo.displayName = LoadLocalization.Instance.GetLocalizedString(uo.displayName);
                uo.description = LoadLocalization.Instance.GetLocalizedString(uo.description);
            }
        }
        //升级信息

        public static void CreateAvatar_SetClassDescription_Patch(CreateAvatar __instance)
        {
            __instance.text_classDescription.text = LoadLocalization.Instance.GetLocalizedString(__instance.text_classDescription.text.Trim()) + "\n";
        }

        //天气
        public static void DisplayWeather_ShowWeather_Patch(DisplayWeather __instance)
        {
            if (!string.IsNullOrEmpty(__instance.text.text))
            {
                if (!__instance.text.text.Contains("<color="))
                {
                    __instance.text.text = LoadLocalization.Instance.GetLocalizedString(__instance.text.text);
                }
                else
                {
                    string text = __instance.text.text.Remove(__instance.text.text.IndexOf("<"));
                    int fcut = __instance.text.text.IndexOf(">") + 1;
                    string text2 = __instance.text.text.Substring(fcut, __instance.text.text.Length - fcut - 8).Trim();
                    __instance.text.text = __instance.text.text.Replace(text, LoadLocalization.Instance.GetLocalizedString(text)).Replace(text2, LoadLocalization.Instance.GetLocalizedString(text2));
                }
            }
        }
        //天气

        //游戏加载界面提示
        public static bool LoadingScreenHint_GetRandomHint_Patch(ref string __result)
        {
            string[] array = LoadLocalization.Instance.GetLocalizedString("Game_Hints").Split('\n');
            __result = array[UnityEngine.Random.Range(0, array.Length)].Trim();
            return false;
        }

        public static void LoadingScreenHint_Start_Patch(LoadingScreenHint __instance)
        {
            if (__instance.textObj.text.Contains("Hint"))
            {
                string text = __instance.textObj.text.Replace("Hint: ", "").Trim();
                __instance.textObj.text = LoadLocalization.Instance.GetLocalizedString("Hint: ") + text;
            }
        }
        //游戏加载界面提示

        public static void LoadLevelManager_SetStatusSmall_Patch(LoadLevelManager __instance)
        {
            __instance.statusSmall.text = LoadLocalization.Instance.GetLocalizedString(__instance.statusSmall.text);
        }

        //文本修正
        public static void LocalizationUIHelper_OnLocalizationUpdate_Patch(LocalizationUIHelper __instance)
        {
            __instance.textComponent.text = LoadLocalization.Instance.GetLocalizedString(__instance.unLocalizedText.Trim());
        }
        //文本修正

        public static void MainMenu_BringInSelectPlayer_Patch(MainMenu __instance)
        {
            foreach (Button button in __instance.GetComponentsInChildren<Button>())
            {
                Text text = button.GetComponentInChildren<Text>();
                if (text.text.Replace("\r", "").Contains("Create New\nCharacter"))
                    text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
            }
        }

        public static IEnumerator Narator_DismissLocationInfo_Patch(IEnumerator values, Narator __instance)
        {
            int index = 0;
            do
            {
                switch (index)
                {
                    case 2:
                        if (__instance.textPlayerDay.text.Contains("Sol"))
                        {
                            string text = __instance.textPlayerDay.text.Replace("Sol", "").Trim();
                            __instance.textPlayerDay.text = LoadLocalization.Instance.GetLocalizedString("Sol ") + text;
                        }
                        else
                        {
                            __instance.textPlayerDay.text = LoadLocalization.Instance.GetLocalizedString(__instance.textPlayerDay.text);
                        }
                        break;
                    case 4:
                        __instance.textDetail.text = LoadLocalization.Instance.GetLocalizedString(__instance.textDetail.text);
                        break;
                    default:
                        break;
                }
                yield return values.Current;
                index++;
            } while (values.MoveNext());
        }

        public static void Narator_DisplayZoneInfo_Patch(ref string zoneName)
        {
            zoneName = LoadLocalization.Instance.GetLocalizedString(zoneName);
        }

        public static void PDA_SelectionCircleManager_OnEnable_Patch(PDA_SelectionCircleManager __instance)
        {
            __instance.worldLocation.text = __instance.worldLocation.text.Replace("Lat", LoadLocalization.Instance.GetLocalizedString("Lat")).Replace("Long", LoadLocalization.Instance.GetLocalizedString("Long"));
            if (__instance.GetType().GetField("lengthOfDay") != null)
            {
                Text lengthOfDay = Traverse.Create(__instance).Field("lengthOfDay").GetValue<Text>();
                if (lengthOfDay != null)
                    lengthOfDay.text = lengthOfDay.text.Replace("Length of Day: ", LoadLocalization.Instance.GetLocalizedString("Length of Day: "));
            }
            if (__instance.GetType().GetField("locationType") != null)
            {
                Text locationType = Traverse.Create(__instance).Field("locationType").GetValue<Text>();
                if (locationType != null)
                    locationType.text = LoadLocalization.Instance.GetLocalizedString(locationType.text);
            }
        }

        public static void PlayerMovement_CheckGroundStatus_Patch()
        {
            if (!string.IsNullOrEmpty(PlayerGUI.Me.slopeText.text))
            {
                PlayerGUI.Me.slopeText.text = PlayerGUI.Me.slopeText.text.Replace("Slope: ", LoadLocalization.Instance.GetLocalizedString("Slope: "));
            }
        }

        public static void PopupMessage_SetCustom_Patch(ref string title, ref string description, Sprite iconSprite)
        {
            title = LoadLocalization.Instance.GetLocalizedString(title);
            description = LoadLocalization.Instance.GetLocalizedString(description);
        }

        //套装受损提示
        public static void PulseColor_CycleText_Patch(PulseColor __instance)
        {
            Text t = Traverse.Create(__instance).Field("m_text").GetValue<Text>();
            t.text = LoadLocalization.Instance.GetLocalizedString(t.text);
            Traverse.Create(__instance).Field("m_text").SetValue(t);
        }
        //套装受损提示

        public static void SelectAvatar_SelectExistingAvatar_Patch()
        {
            MainMenu.Me.launchButtonText.text = LoadLocalization.Instance.GetLocalizedString(MainMenu.Me.launchButtonText.text);
        }

        //升级信息
        public static void UIBuildHint_RefreshButton_Patch(UIBuildHint __instance)
        {
            if (!__instance.hasUnlocked)
            {
                Text t = __instance.GetComponent<Button>().GetComponentInChildren<Text>();
                string text = t.text;
                if (!text.Contains(":\n") && text.Contains("<color="))
                {
                    int fcut = text.IndexOf(">") + 1;
                    string s = text.Substring(fcut, text.Length - fcut - 8);
                    //string s = text.Substring(fcut).Remove(text.LastIndexOf("</") - fcut);
                    t.text = text.Substring(0, fcut) + "需求:\n\n" + LoadLocalization.Instance.GetLocalizedString(s) + "</color>";
                }
            }
        }
        //升级信息

        public static void UIHintDisplay_ActivateInfo_Patch(UIHintDisplay __instance)
        {
            __instance.infoHarvestText.text = LoadLocalization.Instance.GetLocalizedString(__instance.infoHarvestText.text);
            __instance.hardnessLabel.rectTransform.sizeDelta = new Vector2(__instance.hardnessLabel.preferredWidth, __instance.hardnessLabel.preferredHeight);
        }

        //世界自定义设置
        public static void UniverseOptionsManager_InitializeSettings_Patch(UniverseOptionsManager __instance)
        {
            __instance.buttonUpdate.GetComponentInChildren<Text>().text = LoadLocalization.Instance.GetLocalizedString(__instance.buttonUpdate.GetComponentInChildren<Text>().text);
        }

        public static void UniverseOptionsManager__Setting_Set_Patch(UniverseOptionsManager.Setting __instance, ref float value, ref string detail)
        {
            int cut = -1;
            if (detail.LastIndexOf("- x") > 0)
            {
                cut = detail.LastIndexOf("- x");
            }
            if (detail.LastIndexOf("- +") > 0)
            {
                cut = detail.LastIndexOf("- +");
            }
            if (cut > 0)
            {
                cut += 3;
                string text = detail.Remove(cut);
                string ftext = detail.Substring(cut);
                __instance.description.text = LoadLocalization.Instance.GetLocalizedString(text) + ftext;
                return;
            }
            __instance.description.text = detail;
        }
        //世界自定义设置

        //载具
        public static void VehicleGUI_Awake_Patch(VehicleGUI __instance)
        {
            foreach (MonoBehaviour monoBehaviour in __instance.gameObject.GetComponentsInChildren<MonoBehaviour>())
            {
                foreach (Text t in monoBehaviour.GetComponentsInChildren<Text>())
                {
                    string text = t.text;
                    switch (text)
                    {
                        case "HLTH":
                            t.text = LoadLocalization.Instance.GetLocalizedString("v-" + text);
                            break;
                        case "FUEL":
                            t.text = LoadLocalization.Instance.GetLocalizedString("v-" + text);
                            break;
                        case "PWR":
                            t.text = LoadLocalization.Instance.GetLocalizedString("v-" + text);
                            break;
                        case "Loc:":
                            t.text = LoadLocalization.Instance.GetLocalizedString(text);
                            break;
                        case "Alt:":
                            t.text = LoadLocalization.Instance.GetLocalizedString(text);
                            break;
                        case "Throttle Up":
                            t.text = LoadLocalization.Instance.GetLocalizedString(text);
                            break;
                        case "Throttle Down":
                            t.text = LoadLocalization.Instance.GetLocalizedString(text);
                            break;
                        case "Exit Vehicle":
                            t.text = LoadLocalization.Instance.GetLocalizedString(text);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void VehicleHUDDisplay_ActivateHUDInterplanetaryTravel_Patch(VehicleHUDDisplay __instance)
        {
            __instance.gravity.text = __instance.gravity.text.Replace("Gravity: ", LoadLocalization.Instance.GetLocalizedString("Gravity: "));
            __instance.temperature.text = __instance.temperature.text.Replace("Temperature: ", LoadLocalization.Instance.GetLocalizedString("Temperature: "));
            __instance.atmosphere.text = __instance.atmosphere.text.Replace("Atmosphere: ", LoadLocalization.Instance.GetLocalizedString("Atmosphere: "));
            __instance.surface.text = __instance.surface.text.Replace("Surface: ", LoadLocalization.Instance.GetLocalizedString("Surface: "));
            __instance.faction.text = __instance.faction.text.Replace("Faction: ", LoadLocalization.Instance.GetLocalizedString("Faction: "));
            __instance.distance.text = __instance.distance.text.Replace("Distance: ", LoadLocalization.Instance.GetLocalizedString("Distance: ")).Replace(" Kilometers", LoadLocalization.Instance.GetLocalizedString(" Kilometers"));
        }

        public static void VehicleHUDDisplay_ActivateHUDLowOrbitTravel_Patch(VehicleHUDDisplay __instance)
        {
            if (!string.IsNullOrEmpty(__instance.travelText.text))
            {
                if (__instance.travelText.text.Contains("Enter"))
                {
                    string text = __instance.travelText.text.Replace("Press", "").Replace("to Enter Planet", "").Trim();
                    __instance.travelText.text = LoadLocalization.Instance.GetLocalizedString("Press {0} to Enter Planet", text);
                }
                else if (__instance.travelText.text.Contains("Travel"))
                {
                    string text = __instance.travelText.text.Replace("Press", "").Replace("to Travel To Planet", "").Trim();
                    __instance.travelText.text = LoadLocalization.Instance.GetLocalizedString("Press {0} to Travel To Planet", text);
                }
                else
                {
                    string text = __instance.travelText.text.Replace("Press", "").Replace("to Exit Planet", "").Trim();
                    __instance.travelText.text = LoadLocalization.Instance.GetLocalizedString("Press {0} to Exit Planet", text);
                }
            }
        }
        //载具

        //玩家状态HUD
        public static void VitalReadout_Awake_Patch(VitalReadout __instance)
        {
            foreach (MonoBehaviour monoBehaviour in __instance.GetComponents<MonoBehaviour>())
            {
                foreach (Text text in monoBehaviour.GetComponentsInChildren<Text>())
                {
                    switch (text.text.Replace("\r", "").Trim())
                    {
                        case "HOV":
                            text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                            break;
                        case "HYD\n---":
                            text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                            break;
                        case "NTR":
                            text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                            break;
                        case "HLTH":
                            text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                            break;
                        case "HLTH\n---":
                            text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                            break;
                    }
                }
            }
        }

        public static void VitalReadout_UpdateVitalsOxygen_Patch(VitalReadout __instance)
        {
            __instance.vitalExtraText[0].text = __instance.vitalExtraText[0].text.Replace("Atmos O2: ", LoadLocalization.Instance.GetLocalizedString("Atmos O2: "));
        }
        //玩家状态HUD

        public static void World_PlanetDisplayName_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        public static void UIVitalDisplay_ActivateVitalDisplay_Patch(Text ___titleText, Text ___descriptionText)
        {
            switch (___titleText.text)
            {
                case "Health":
                    ___descriptionText.text = ___descriptionText.text.Replace("Health: ", LoadLocalization.Instance.GetLocalizedString("Health: ")).Replace("Suit Integrity: ", LoadLocalization.Instance.GetLocalizedString("Suit Integrity: ")).Replace("Maintain your suit to stay alive.", LoadLocalization.Instance.GetLocalizedString("Maintain your suit to stay alive."));
                    break;
                case "Oxygen":
                    ___descriptionText.text = ___descriptionText.text.Replace("Tank: ", LoadLocalization.Instance.GetLocalizedString("Tank: ")).Replace("Atmosphere Oxygen: ", LoadLocalization.Instance.GetLocalizedString("Atmosphere Oxygen: ")).Replace("Tank can replenish based on Oxygen in atmosphere.", LoadLocalization.Instance.GetLocalizedString("Tank can replenish based on Oxygen in atmosphere."));
                    break;
                case "Hunger/Thirst":
                    ___descriptionText.text = ___descriptionText.text.Replace("Hydration: ", LoadLocalization.Instance.GetLocalizedString("Hydration: ")).Replace("Nutrition: ", LoadLocalization.Instance.GetLocalizedString("Nutrition: ")).Replace("Drink water and eat food to maintain your thirst and hunger levels.", LoadLocalization.Instance.GetLocalizedString("Drink water and eat food to maintain your thirst and hunger levels."));
                    break;
                case "Hover":
                    ___descriptionText.text = ___descriptionText.text.Replace("Hover Fuel: ", LoadLocalization.Instance.GetLocalizedString("Hover Fuel: ")).Replace("Hover fuel will recharge when used.", LoadLocalization.Instance.GetLocalizedString("Hover fuel will recharge when used."));
                    break;
                case "Pressure":
                    ___descriptionText.text = ___descriptionText.text.Replace("Suit Pressure: ", LoadLocalization.Instance.GetLocalizedString("Suit Pressure: ")).Replace("Outside Pressure: ", LoadLocalization.Instance.GetLocalizedString("Outside Pressure: ")).Replace("Keep your suit patched up to keep internal pressure regulated.", LoadLocalization.Instance.GetLocalizedString("Keep your suit patched up to keep internal pressure regulated."));
                    break;
                case "Temperature":
                    ___descriptionText.text = ___descriptionText.text.Replace("Outside Temperature: ", LoadLocalization.Instance.GetLocalizedString("Outside Temperature: ")).Replace("Suit Temperature: ", LoadLocalization.Instance.GetLocalizedString("Suit Temperature: ")).Replace("Keep your suit patched up to keep internal temperature regulated.", LoadLocalization.Instance.GetLocalizedString("Keep your suit patched up to keep internal temperature regulated."));
                    break;
                case "Battery":
                    ___descriptionText.text = ___descriptionText.text.Replace("Remaining Energy: ", LoadLocalization.Instance.GetLocalizedString("Remaining Energy: ")).Replace("Replace Battery to replenish power.", LoadLocalization.Instance.GetLocalizedString("Replace Battery to replenish power."));
                    break;
                default:
                    break;
            }
            ___titleText.text = LoadLocalization.Instance.GetLocalizedString(___titleText.text);
        }

    }

    //世界自定义设置
    /*[HarmonyPatch(typeof(UniverseOptionsManager), nameof(UniverseOptionsManager.SaveLocalUniverse))]
        class UniverseOptionsManager_SaveLocalUniverse_Patch
        {
            public static void Postfix(UniverseOptionsManager __instance)
            {
                if (!string.IsNullOrEmpty(UniverseOptionsManager.ServerInfo.universeName))
                {
                    MultiplayerController.Me.universe.Replace(UniverseOptionsManager.ServerInfo.universeName, "");
                    UniverseOptionsManager.ServerInfo.universeName = LoadLocalization.Instance.GetLocalizedString(UniverseOptionsManager.ServerInfo.universeName);
                }
            }
        }*/

}
