using HarmonyLib;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Osiris_I18n
{
    class Map_Patch
    {

        [HarmonyPatch(typeof(bl_MiniMapItem), "CreateIcon")]
        class bl_MiniMapItem_CreateIcon_Patch
        {
            public static void Postfix(bl_MiniMapItem __instance)
            {
                bl_IconItem bl_ii = __instance.Graphic.GetComponent<bl_IconItem>();
                bl_ii.InfoText.text = LoadLocalization.Instance.GetLocalizedString(bl_ii.InfoText.text);
            }
        }

        [HarmonyPatch(typeof(DroidPanelController), nameof(DroidPanelController.OnEnable))]
        class DroidPanelController_OnEnable_Patch
        {
            public static void Postfix(DroidPanelController __instance)
            {
				foreach (MonoBehaviour monoBehaviour in __instance.gameObject.GetComponents<MonoBehaviour>())
				{
					foreach (Text text in monoBehaviour.GetComponentsInChildren<Text>())
					{
						switch (text.text.Replace("\r", "").Trim())
						{
							case "All":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							case "O.M.P.A.":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							case "O.M.M.A.":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							case "O.M.S.U":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							case "Send To Position":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							case "Teleport To Player":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							default:
								break;
						}
					}
				}
			}
        }
		
		[HarmonyPatch(typeof(DroidPanelController), nameof(DroidPanelController.OnSelectDroid))]
        class DroidPanelController_OnSelectDroid_Patch
        {
            public static void Postfix(DroidPanelController __instance)
            {
				__instance.selectedDroidNickname.text = LoadLocalization.Instance.GetLocalizedString(__instance.selectedDroidNickname.text);
				__instance.selectedDroidLocation.text = __instance.selectedDroidLocation.text.Replace("Lat", LoadLocalization.Instance.GetLocalizedString("Lat")).Replace("Long", LoadLocalization.Instance.GetLocalizedString("Long"));
				__instance.selectedDroidHealth.text = __instance.selectedDroidHealth.text.Replace("Health: ", LoadLocalization.Instance.GetLocalizedString("Health: "));
				__instance.selectedDroidEnergy.text = __instance.selectedDroidEnergy.text.Replace("Energy: ", LoadLocalization.Instance.GetLocalizedString("Energy: "));

				foreach (MonoBehaviour monoBehaviour in __instance.gameObject.GetComponents<MonoBehaviour>())
				{
					foreach (Text text in monoBehaviour.GetComponentsInChildren<Text>())
					{
						switch (text.text.Replace("\r", "").Trim())
						{
							case "Patrol":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							case "Stay":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							case "Follow":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							default:
								break;
						}
					}
				}
			}
        }
		
		[HarmonyPatch(typeof(DroidPanelController), "ParseCommandCode")]
        class DroidPanelController_ParseCommandCode_Patch
		{
            public static void Postfix(ref string __result)
            {
                if (__result.IndexOf("Travelling to") >= 0)
                {
					string str = __result.Replace("Travelling to", "").Replace(" ", "").Replace("Lat", "").Replace("°Long", "").Trim();
					string[] texts = str.Split('°');
					__result = LoadLocalization.Instance.GetLocalizedString("Travelling to {0}° Lat {1}° Long", texts);
				}
				else if (__result.IndexOf("Following") >= 0)
                {
					string text = __result.Replace("Following", "").Trim();
					__result = LoadLocalization.Instance.GetLocalizedString("Following {0}.", text);
                }
				else if (__result.IndexOf("Protecting") >= 0)
                {
					string text = __result.Replace("Protecting", "").Trim();
					__result = LoadLocalization.Instance.GetLocalizedString("Protecting {0}.", text);
                }
				else
                {
					__result = LoadLocalization.Instance.GetLocalizedString(__result);
				}
			}
        }
		
		[HarmonyPatch(typeof(DroidPanelController), nameof(DroidPanelController.SendToPosition))]
        class DroidPanelController_SendToPosition_Patch
		{
            public static void Postfix(DroidPanelController __instance)
            {
				foreach (MonoBehaviour monoBehaviour in __instance.gameObject.GetComponents<MonoBehaviour>())
				{
					foreach (Text text in monoBehaviour.GetComponentsInChildren<Text>())
					{
						switch (text.text.Replace("\r", "").Trim())
						{
							case "Set Position Target":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							case "Cancel":
								text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
								break;
							default:
								break;
						}
					}
				}
			}
        }

        [HarmonyPatch(typeof(DroidPanelListElement), nameof(DroidPanelListElement.InitializeElement))]
        class DroidPanelListElement_InitializeElement_Patch
        {
            public static void Postfix(DroidPanelListElement __instance)
            {
                if ((bool)__instance.droidData)
                {
                    __instance.droidNickname.text = LoadLocalization.Instance.GetLocalizedString(__instance.droidNickname.text);
                    __instance.droidType.text = LoadLocalization.Instance.GetLocalizedString(__instance.droidType.text);
                }
            }
        }

        //指南针
        [HarmonyPatch(typeof(PlayerGUI), "Awake")]
        class PlayerGUI_Awake_Patch
        {
            public static void Postfix(PlayerGUI __instance)
            {
                foreach (MonoBehaviour monoBehaviour in __instance.GetComponents<MonoBehaviour>())
                {
                    foreach (Text text in monoBehaviour.GetComponentsInChildren<Text>())
                    {
                        switch (text.text.Replace("\r", "").Trim())
                        {
                            case "N":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            case "NE":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            case "E":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            case "SE":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            case "S":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            case "SW":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            case "W":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            case "NW":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(PlayerGUI), nameof(PlayerGUI.ShowMapStructureInfo))]
        class PlayerGUI_ShowMapStructureInfo_class
        {
            public static void Postfix(PlayerGUI __instance)
            {
                __instance.mapItemInfoText1.text = __instance.mapItemInfoText1.text.Replace("Condition: ", LoadLocalization.Instance.GetLocalizedString("Condition: "));
                if (__instance.mapItemInfoText2.IsActive())
                {
                    __instance.mapItemInfoText2.text = LoadLocalization.Instance.GetLocalizedString(__instance.mapItemInfoText2.text);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerGUI), nameof(PlayerGUI.ShowMapItemInfo))]
        class PlayerGUI_ShowMapItemInfo_class
        {
            public static void Prefix(ref string displayInfo)
            {
                displayInfo = LoadLocalization.Instance.GetLocalizedString(displayInfo);
            }
        }
        
        [HarmonyPatch(typeof(PlayerGUI), nameof(PlayerGUI.ShowMapVehicleInfo))]
        class PlayerGUI_ShowMapVehicleInfo_class
        {
            public static void Postfix(PlayerGUI __instance)
            {
                if (__instance.mapItemInfoText1.IsActive())
                {
                    __instance.mapItemInfoText1.text = __instance.mapItemInfoText1.text.Replace("Owner: ", LoadLocalization.Instance.GetLocalizedString("Owner: "));
                }
                if (__instance.mapItemInfoText2.IsActive())
                {
                    __instance.mapItemInfoText2.text = __instance.mapItemInfoText2.text.Replace("Condition: ", LoadLocalization.Instance.GetLocalizedString("Condition: "));
                }
            }
        }
        
        [HarmonyPatch(typeof(PlayerGUI), "UpdateFilterCoroutine")]
        class PlayerGUI_UpdateFilterCoroutine_class
        {
            public static IEnumerator Postfix(IEnumerator values, PlayerGUI __instance)
            {
                __instance.mapResourcesFoundText.text = LoadLocalization.Instance.GetLocalizedString("map-" + __instance.mapResourcesFoundText.text);
                do
                {
                    yield return values.Current;
                } while (values.MoveNext());
                string text = __instance.mapResourcesFoundText.text.Replace("Found", "").Replace("Resources", "").Trim();
                __instance.mapResourcesFoundText.text = LoadLocalization.Instance.GetLocalizedString("map-Found {0} Resources", text);
            }
        }

        //经纬度
        [HarmonyPatch(typeof(RadarLongitude), "Awake")]
        class RadarLongitude_Awake_Patch
        {
            public static void Postfix(RadarLongitude __instance)
            {
                foreach (MonoBehaviour monoBehaviour in __instance.gameObject.GetComponents<MonoBehaviour>())
                {
                    foreach (Text text in monoBehaviour.GetComponentsInChildren<Text>())
                    {
                        switch (text.text)
                        {
                            case "LAT":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            case "LONG":
                                text.text = LoadLocalization.Instance.GetLocalizedString(text.text);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

    }
}
