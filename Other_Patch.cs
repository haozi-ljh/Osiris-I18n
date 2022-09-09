using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Osiris_I18n
{

    //人物选择界面信息
    class AvatarSelection_Patch
    {

        [HarmonyPatch(typeof(AvatarSelection), nameof(AvatarSelection.SetAvatarSettings))]
        class AvatarSelection_SetAvatarSettings_Patch
        {
            public static void Postfix(AvatarSelection __instance)
            {
                __instance.textName.text = __instance.textName.text.Replace("Sol ", LoadLocalization.Instance.GetLocalizedString("Sol "));
                string[] classtexts = __instance.textClass.text.Split(' ');
                __instance.textClass.text = __instance.textClass.text.Replace("Level ", LoadLocalization.Instance.GetLocalizedString("Level ")).Replace(classtexts[classtexts.Length - 2], LoadLocalization.Instance.GetLocalizedString(classtexts[classtexts.Length - 2])).Replace(classtexts[classtexts.Length - 1], LoadLocalization.Instance.GetLocalizedString(classtexts[classtexts.Length - 1]));
                string diftext = __instance.textDifficulty.text.Replace("Difficulty: ", "").Trim();
                __instance.textDifficulty.text = LoadLocalization.Instance.GetLocalizedString("Difficulty: ") + LoadLocalization.Instance.GetLocalizedString(diftext);
            }
        }

    }

    class BuildableObject_Patch
    {

        [HarmonyPatch(typeof(BuildableObject), "BuildOwnershipString")]
        class BuildableObject_BuildOwnershipString_Patch
        {
            public static void Postfix(ref StringBuilder __result)
            {
                string str = __result.ToString();
                __result.Clear();
                if (!string.IsNullOrEmpty(str))
                {
                    if (str.IndexOf("Colony") >= 0)
                    {
                        string text = str.Replace("Owned by Colony", "").Trim();
                        __result.AppendFormat(LoadLocalization.Instance.GetLocalizedString("Owned by Colony {0}"), text);
                    }
                    if (str.IndexOf("Me") >= 0)
                    {
                        __result.Append(LoadLocalization.Instance.GetLocalizedString(str));
                    }
                    if (str.IndexOf("Avatar") >= 0)
                    {
                        string text = str.Replace("Owned by Avatar", "").Trim();
                        __result.AppendFormat(LoadLocalization.Instance.GetLocalizedString("Owned by Avatar {0}"), text);
                    }
                    if (str.IndexOf("Unowned") >= 0)
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
        }

        [HarmonyPatch(typeof(BuildableObject), "ownershipString", MethodType.Setter)]
        class BuildableObject_ownershipString_Patch
        {
            public static void Prefix(ref string value)
            {
                if (value.IndexOf("Me") >= 0)
                {
                    value = LoadLocalization.Instance.GetLocalizedString(value);
                }
                if (value.IndexOf("Avatar") >= 0)
                {
                    string text = value.Replace("Owned by Avatar", "").Trim();
                    value = LoadLocalization.Instance.GetLocalizedString("Owned by Avatar {0}", text);
                }
                if (value.IndexOf("Unowned") >= 0)
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

    }

    //升级信息
    class CraftingTable_Patch
    {

        [HarmonyPatch(typeof(CraftingTable), nameof(CraftingTable.LoadCraftingTableWithUpgrades))]
        class CraftingTable_LoadCraftingTableWithUpgrades_Patch
        {
            public static void Postfix(string name, ref List<UpgradeOption> upgrades)
            {
                foreach (UpgradeOption uo in upgrades)
                {
                    uo.displayName = LoadLocalization.Instance.GetLocalizedString(uo.displayName);
                    uo.description = LoadLocalization.Instance.GetLocalizedString(uo.description);
                }
            }
        }

    }

    public class CreateAvatar_Patch
    {

        [HarmonyPatch(typeof(CreateAvatar), "SetClassDescription", new Type[] { typeof(AvatarClass) })]
        class CreateAvatar_SetClassDescription_Patch
        {
            public static void Postfix(CreateAvatar __instance)
            {
                __instance.text_classDescription.text = LoadLocalization.Instance.GetLocalizedString(__instance.text_classDescription.text.Trim()) + "\n";
            }
        }

    }

    //天气
    class DisplayWeather_Patch
    {

        [HarmonyPatch(typeof(DisplayWeather), nameof(DisplayWeather.ShowWeather))]
        class DisplayWeather_ShowWeather_Patch
        {
            public static void Postfix(DisplayWeather __instance)
            {
                if (!string.IsNullOrEmpty(__instance.text.text))
                {
                    if (__instance.text.text.IndexOf("<color=") < 0)
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
        }

    }

    class Geyser_Patch
    {

        [HarmonyPatch(typeof(Geyser), "displayName", MethodType.Getter)]
        class Geyser_displayName_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

    }

    //游戏加载界面提示
    class LoadingScreenHint_Patch
    {

        [HarmonyPatch(typeof(LoadingScreenHint), "GetRandomHint")]
        class LoadingScreenHint_GetRandomHint_Patch
        {
            public static bool Prefix(ref string __result)
            {
                string[] array = LoadLocalization.Instance.GetLocalizedString("Game_Hints").Split('\n');
                __result = array[UnityEngine.Random.Range(0, array.Length)].Trim();
                return false;
            }
        }

        [HarmonyPatch(typeof(LoadingScreenHint), "Start")]
        class LoadingScreenHint_Start_Patch
        {
            public static void Postfix(LoadingScreenHint __instance)
            {
                if (__instance.textObj.text.IndexOf("Hint") >= 0)
                {
                    string text = __instance.textObj.text.Replace("Hint: ", "").Trim();
                    __instance.textObj.text = LoadLocalization.Instance.GetLocalizedString("Hint: ") + text;
                }
            }
        }

    }

    class LoadLevelManager_Patch
    {

        [HarmonyPatch(typeof(LoadLevelManager), "SetStatusSmall", new Type[] { typeof(string) })]
        class LoadLevelManager_SetStatusSmall_Patch
        {
            public static void Postfix(LoadLevelManager __instance)
            {
                __instance.statusSmall.text = LoadLocalization.Instance.GetLocalizedString(__instance.statusSmall.text);
            }
        }

    }

    //文本修正
    class LocalizationUIHelper_Patch
    {

        [HarmonyPatch(typeof(LocalizationUIHelper), nameof(LocalizationUIHelper.OnLocalizationUpdate))]
        class LocalizationUIHelper_OnLocalizationUpdate_Patch
        {
            public static void Postfix(LocalizationUIHelper __instance)
            {
                __instance.textComponent.text = LoadLocalization.Instance.GetLocalizedString(__instance.unLocalizedText.Trim());
            }
        }

    }

    class Narator_Patch
    {

        [HarmonyPatch(typeof(Narator), "DismissLocationInfo")]
        class Narator_DismissLocationInfo_Patch
        {
            [HarmonyPostfix]
            public static System.Collections.IEnumerator Postfix(System.Collections.IEnumerator values, Narator __instance)
            {
                int index = 0;
                do
                {
                    switch (index)
                    {
                        case 2:
                            if (__instance.textPlayerDay.text.IndexOf("Sol") >= 0)
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
        }

        [HarmonyPatch(typeof(Narator), nameof(Narator.DisplayZoneInfo))]
        class Narator_DisplayZoneInfo_Patch
        {
            public static void Prefix(ref string zoneName)
            {
                zoneName = LoadLocalization.Instance.GetLocalizedString(zoneName);
            }
        }

    }

    class PDA_SelectionCircleManager_Patch
    {
        [HarmonyPatch(typeof(PDA_SelectionCircleManager), "OnEnable")]
        class PDA_SelectionCircleManager_OnEnable_Patch
        {
            public static void Postfix(PDA_SelectionCircleManager __instance)
            {
                __instance.worldLocation.text = __instance.worldLocation.text.Replace("Lat", LoadLocalization.Instance.GetLocalizedString("Lat")).Replace("Long", LoadLocalization.Instance.GetLocalizedString("Long"));
                if (__instance.GetType().GetField("lengthOfDay") != null)
                {
                    Text lengthOfDay = Traverse.Create(__instance).Field("lengthOfDay").GetValue<Text>();
                    if(lengthOfDay != null)
                        lengthOfDay.text = lengthOfDay.text.Replace("Length of Day: ", LoadLocalization.Instance.GetLocalizedString("Length of Day: "));
                }
                if (__instance.GetType().GetField("locationType") != null)
                {
                    Text locationType = Traverse.Create(__instance).Field("locationType").GetValue<Text>();
                    if (locationType != null)
                        locationType.text = LoadLocalization.Instance.GetLocalizedString(locationType.text);
                }
            }
        }
    }

    class PlayerMovement_Patch
    {

        [HarmonyPatch(typeof(PlayerMovement), "CheckGroundStatus")]
        class PlayerMovement_CheckGroundStatus_Patch
        {
            public static void Postfix()
            {
                if (!string.IsNullOrEmpty(PlayerGUI.Me.slopeText.text))
                {
                    PlayerGUI.Me.slopeText.text = PlayerGUI.Me.slopeText.text.Replace("Slope: ", LoadLocalization.Instance.GetLocalizedString("Slope: "));
                }
            }
        }

    }

    class PopupMessage_Patch
    {

        [HarmonyPatch(typeof(PopupMessage), nameof(PopupMessage.SetCustom))]
        class PopupMessage_SetCustom_Patch
        {
            public static void Prefix(ref string title, ref string description, Sprite iconSprite)
            {
                title = LoadLocalization.Instance.GetLocalizedString(title);
                description = LoadLocalization.Instance.GetLocalizedString(description);
            }
        }

    }

    //套装受损提示
    class PulseColor_Patch
    {

        [HarmonyPatch(typeof(PulseColor), "CycleText")]
        class PulseColor_CycleText_Patch
        {
            public static void Postfix(PulseColor __instance)
            {
                Text t = Traverse.Create(__instance).Field("m_text").GetValue<Text>();
                t.text = LoadLocalization.Instance.GetLocalizedString(t.text);
                Traverse.Create(__instance).Field("m_text").SetValue(t);
            }
        }

    }

    class SelectAvatar_Patch
    {

        [HarmonyPatch(typeof(SelectAvatar), nameof(SelectAvatar.SelectExistingAvatar))]
        class SelectAvatar_SelectExistingAvatar_Patch
        {
            public static void Postfix()
            {
                MainMenu.Me.launchButtonText.text = LoadLocalization.Instance.GetLocalizedString(MainMenu.Me.launchButtonText.text);
            }
        }

    }

    class UIBuildHint_Patch
    {

        [HarmonyPatch(typeof(UIBuildHint), "RefreshButton")]
        class UIBuildHint_RefreshButton_Patch
        {
            public static void Postfix(UIBuildHint __instance)
            {
                if (!__instance.hasUnlocked)
                {
                    Text t = __instance.GetComponent<Button>().GetComponentInChildren<Text>();
                    string text = t.text;
                    if (text.IndexOf(":\n") < 0 && text.IndexOf("<color=") > 0)
                    {
                        int fcut = text.IndexOf(">") + 1;
                        string s = text.Substring(fcut, text.Length - fcut - 8);
                        //string s = text.Substring(fcut).Remove(text.LastIndexOf("</") - fcut);
                        t.text = text.Substring(0, fcut) + "需求:\n\n" + LoadLocalization.Instance.GetLocalizedString(s) + "</color>";
                    }
                }
            }
        }

    }

    class UIHintDisplay_Patch
    {

        [HarmonyPatch(typeof(UIHintDisplay), nameof(UIHintDisplay.ActivateInfo))]
        class UIHintDisplay_ActivateInfo_Patch
        {
            public static void Postfix(UIHintDisplay __instance)
            {
                __instance.infoHarvestText.text = LoadLocalization.Instance.GetLocalizedString(__instance.infoHarvestText.text);
                __instance.hardnessLabel.rectTransform.sizeDelta = new Vector2(__instance.hardnessLabel.preferredWidth, __instance.hardnessLabel.preferredHeight);
            }
        }

    }

    //世界自定义设置
    class UniverseOptionsManager_Patch
    {

        [HarmonyPatch(typeof(UniverseOptionsManager), nameof(UniverseOptionsManager.InitializeSettings))]
        class UniverseOptionsManager_InitializeSettings_Patch
        {
            public static void Postfix(UniverseOptionsManager __instance)
            {
                __instance.buttonUpdate.GetComponentInChildren<Text>().text = LoadLocalization.Instance.GetLocalizedString(__instance.buttonUpdate.GetComponentInChildren<Text>().text);
            }
        }

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
        
        [HarmonyPatch(typeof(UniverseOptionsManager.Setting), nameof(UniverseOptionsManager.Setting.Set))]
        class UniverseOptionsManager__Setting_InitializeSettings_Patch
        {
            public static void Postfix(UniverseOptionsManager.Setting __instance, ref float value, ref string detail)
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
        }

    }

    //载具
    class Vehicle_Patch
    {

        [HarmonyPatch(typeof(VehicleGUI), "Awake")]
        class VehicleGUI_Awake_Patch
        {
            public static void Postfix(VehicleGUI __instance)
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
        }

        [HarmonyPatch(typeof(VehicleHUDDisplay), "ActivateHUDInterplanetaryTravel")]
        class VehicleHUDDisplay_ActivateHUDInterplanetaryTravel_Patch
        {
            public static void Postfix(VehicleHUDDisplay __instance)
            {
                __instance.gravity.text = __instance.gravity.text.Replace("Gravity: ", LoadLocalization.Instance.GetLocalizedString("Gravity: "));
                __instance.temperature.text = __instance.temperature.text.Replace("Temperature: ", LoadLocalization.Instance.GetLocalizedString("Temperature: "));
                __instance.atmosphere.text = __instance.atmosphere.text.Replace("Atmosphere: ", LoadLocalization.Instance.GetLocalizedString("Atmosphere: "));
                __instance.surface.text = __instance.surface.text.Replace("Surface: ", LoadLocalization.Instance.GetLocalizedString("Surface: "));
                __instance.faction.text = __instance.faction.text.Replace("Faction: ", LoadLocalization.Instance.GetLocalizedString("Faction: "));
                __instance.distance.text = __instance.distance.text.Replace("Distance: ", LoadLocalization.Instance.GetLocalizedString("Distance: ")).Replace(" Kilometers", LoadLocalization.Instance.GetLocalizedString(" Kilometers"));
            }
        }

        [HarmonyPatch(typeof(VehicleHUDDisplay), "ActivateHUDLowOrbitTravel")]
        class VehicleHUDDisplay_ActivateHUDLowOrbitTravel_Patch
        {
            public static void Postfix(VehicleHUDDisplay __instance)
            {
                if (!string.IsNullOrEmpty(__instance.travelText.text))
                {
                    if (__instance.travelText.text.IndexOf("Enter") >= 0)
                    {
                        string text = __instance.travelText.text.Replace("Press", "").Replace("to Enter Planet", "").Trim();
                        __instance.travelText.text = LoadLocalization.Instance.GetLocalizedString("Press {0} to Enter Planet", text);
                    }
                    else if (__instance.travelText.text.IndexOf("Travel") >= 0)
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
        }

    }

    //玩家状态HUD
    class VitalReadout_Patch
    {

        [HarmonyPatch(typeof(VitalReadout), "Awake")]
        class VitalReadout_Awake_Patch
        {
            public static void Postfix(VitalReadout __instance)
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
        }

        [HarmonyPatch(typeof(VitalReadout), nameof(VitalReadout.UpdateVitalsOxygen))]
        class VitalReadout_UpdateVitalsOxygen_Patch
        {
            public static void Postfix(VitalReadout __instance)
            {
                __instance.vitalExtraText[0].text = __instance.vitalExtraText[0].text.Replace("Atmos O2: ", LoadLocalization.Instance.GetLocalizedString("Atmos O2: "));
            }
        }

    }

    class World_Patch
    {

        [HarmonyPatch(typeof(World), nameof(World.PlanetDisplayName))]
        class World_PlanetDisplayName_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

    }

    class Compatible
    {

        public Compatible(Harmony harmony)
        {
            Assembly gamedll = Assembly.LoadFrom(BepInEx.Paths.ManagedPath + "\\Assembly-CSharp.dll");
            if (gamedll.GetType("UIVitalDisplay") != null)
            {
                harmony.Patch(gamedll.GetType("UIVitalDisplay").GetMethod("ActivateVitalDisplay", new Type[] { typeof(VitalReadout) }), postfix: new HarmonyMethod(GetType().GetMethod("UIVitalDisplay_ActivateVitalDisplay_Patch")));
            }
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

    class Alter
    {

        Harmony harmony;

        static bool VehicleGUI_ontakeoffhint;
        static bool VehicleGUI_onflyhint;

        public Alter(Harmony harmony)
        {
            this.harmony = harmony;
        }

        public void Patch()
        {
            VehicleGUI_ontakeoffhint = true;
            VehicleGUI_onflyhint = true;
            harmony.Patch(typeof(VehicleGUI).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), postfix: new HarmonyMethod(GetType().GetMethod("VehicleGUI_Update_Alter")));
        }

        public void Unpatch()
        {
            VehicleGUI_ontakeoffhint = false;
            VehicleGUI_onflyhint = false;
            harmony.Unpatch(typeof(VehicleGUI).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic), GetType().GetMethod("VehicleGUI_Update_Alter"));
        }

        public static void VehicleGUI_Update_Alter(VehicleGUI __instance)
        {
            if (__instance.sliderThrottle != null && __instance.sliderHover != null)
            {
                if (VehicleGUI_ontakeoffhint && __instance.sliderHover.value > 0f && __instance.sliderThrottle.value <= 0.5f)
                {
                    PlayerGUI.Me.Invoke("TurnOffInstructionMessage", 0f);
                    PlayerGUI.Me.ShowInstructionMessage("[W-A-S-D]水平移动", true);
                    VehicleGUI_ontakeoffhint = false;
                    PlayerGUI.Me.Invoke("TurnOffInstructionMessage", 5f);
                }
                else if (!VehicleGUI_ontakeoffhint && (__instance.sliderHover.value <= 0f || __instance.sliderThrottle.value > 0.5f))
                {
                    VehicleGUI_ontakeoffhint = true;
                }
                if (VehicleGUI_onflyhint && __instance.sliderThrottle.value > 0.5f)
                {
                    PlayerGUI.Me.Invoke("TurnOffInstructionMessage", 0f);
                    PlayerGUI.Me.ShowInstructionMessage("[W-S]控制机头抬降,[A-D]左右拐弯,[Q-E]翻转", true);
                    VehicleGUI_onflyhint = false;
                    PlayerGUI.Me.Invoke("TurnOffInstructionMessage", 5f);
                }
                else if (!VehicleGUI_onflyhint && __instance.sliderThrottle.value <= 0.5f)
                {
                    VehicleGUI_onflyhint = true;
                }
            }
        }

    }

    class Optimize
    {

        Harmony harmony;

        public Optimize(Harmony harmony)
        {
            this.harmony = harmony;
        }

        public void Patch()
        {
            harmony.Patch(typeof(PlayerMovement).GetMethod("CheckGroundStatus", BindingFlags.Instance | BindingFlags.NonPublic), postfix: new HarmonyMethod(GetType().GetMethod("PlayerMovement_CheckGroundStatus_Optimize")));
            harmony.Patch(typeof(VitalReadout).GetMethod("UpdateVitalsOxygen"), postfix: new HarmonyMethod(GetType().GetMethod("VitalReadout_UpdateVitalsOxygen_Optimize")));
            harmony.Patch(typeof(VitalWarning).GetMethod("Activate"), postfix: new HarmonyMethod(GetType().GetMethod("VitalWarning_Activate_Optimize")));
        }

        public void Unpatch()
        {
            harmony.Unpatch(typeof(PlayerMovement).GetMethod("CheckGroundStatus", BindingFlags.Instance | BindingFlags.NonPublic), GetType().GetMethod("PlayerMovement_CheckGroundStatus_Optimize"));
            harmony.Unpatch(typeof(VitalReadout).GetMethod("UpdateVitalsOxygen"), GetType().GetMethod("VitalReadout_UpdateVitalsOxygen_Optimize"));
            harmony.Unpatch(typeof(VitalWarning).GetMethod("Activate"), GetType().GetMethod("VitalWarning_Activate_Optimize"));
        }

        public static void PlayerMovement_CheckGroundStatus_Optimize()
        {
            PlayerGUI.Me.slopeText.fontSize = 20;
        }

        public static void VitalReadout_UpdateVitalsOxygen_Optimize(VitalReadout __instance)
        {
            __instance.vitalExtraText[0].fontSize = 20;
        }

        public static void VitalWarning_Activate_Optimize(VitalWarning __instance)
        {
            __instance.textComponent.fontSize = 20;
        }

    }

}
