using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Osiris_I18n
{
    public class Map_Patch
    {

        public Map_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(bl_MiniMapItem), "CreateIcon", PatchType.postfix, GetType().GetMethod("bl_MiniMapItem_CreateIcon_Patch")));
            PatcherManager.Add(new Patcher(typeof(DroidPanelController), "OnEnable", PatchType.postfix, GetType().GetMethod("DroidPanelController_OnEnable_Patch")));
            PatcherManager.Add(new Patcher(typeof(DroidPanelController), "OnSelectDroid", PatchType.postfix, GetType().GetMethod("DroidPanelController_OnSelectDroid_Patch")));
            PatcherManager.Add(new Patcher(typeof(DroidPanelController), "ParseCommandCode", PatchType.postfix, GetType().GetMethod("DroidPanelController_ParseCommandCode_Patch")));
            PatcherManager.Add(new Patcher(typeof(DroidPanelController), "SendToPosition", PatchType.postfix, GetType().GetMethod("DroidPanelController_SendToPosition_Patch")));
            PatcherManager.Add(new Patcher(typeof(DroidPanelListElement), "InitializeElement", PatchType.postfix, GetType().GetMethod("DroidPanelListElement_InitializeElement_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "Awake", PatchType.postfix, GetType().GetMethod("PlayerGUI_Awake_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "RefreshActiveMapScanners", PatchType.prefix, GetType().GetMethod("PlayerGUI_RefreshActiveMapScanners_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "ShowMapStructureInfo", PatchType.postfix, GetType().GetMethod("PlayerGUI_ShowMapStructureInfo_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "ShowMapItemInfo", PatchType.prefix, GetType().GetMethod("PlayerGUI_ShowMapItemInfo_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "ShowMapVehicleInfo", PatchType.postfix, GetType().GetMethod("PlayerGUI_ShowMapVehicleInfo_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "UpdateFilterCoroutine", PatchType.postfix, GetType().GetMethod("PlayerGUI_UpdateFilterCoroutine_Patch")));
            PatcherManager.Add(new Patcher(typeof(RadarLongitude), "Awake", PatchType.postfix, GetType().GetMethod("RadarLongitude_Awake_Patch")));
        }

        public static void bl_MiniMapItem_CreateIcon_Patch(bl_MiniMapItem __instance)
        {
            bl_IconItem bl_ii = __instance.Graphic.GetComponent<bl_IconItem>();
            bl_ii.InfoText.text = LoadLocalization.Instance.GetLocalizedString(bl_ii.InfoText.text);
        }

        public static void DroidPanelController_OnEnable_Patch(DroidPanelController __instance)
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

        public static void DroidPanelController_OnSelectDroid_Patch(DroidPanelController __instance)
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

        public static void DroidPanelController_ParseCommandCode_Patch(ref string __result)
        {
            if (__result.Contains("Travelling to"))
            {
                string str = __result.Replace("Travelling to", "").Replace(" ", "").Replace("Lat", "").Replace("°Long", "").Trim();
                string[] texts = str.Split('°');
                __result = LoadLocalization.Instance.GetLocalizedString("Travelling to {0}° Lat {1}° Long", texts);
            }
            else if (__result.Contains("Following"))
            {
                string text = __result.Replace("Following", "").Trim();
                __result = LoadLocalization.Instance.GetLocalizedString("Following {0}.", text);
            }
            else if (__result.Contains("Protecting"))
            {
                string text = __result.Replace("Protecting", "").Trim();
                __result = LoadLocalization.Instance.GetLocalizedString("Protecting {0}.", text);
            }
            else
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

        public static void DroidPanelController_SendToPosition_Patch(DroidPanelController __instance)
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

        public static void DroidPanelListElement_InitializeElement_Patch(DroidPanelListElement __instance)
        {
            if ((bool)__instance.droidData)
            {
                __instance.droidNickname.text = LoadLocalization.Instance.GetLocalizedString(__instance.droidNickname.text);
                __instance.droidType.text = LoadLocalization.Instance.GetLocalizedString(__instance.droidType.text);
            }
        }

        //指南针
        public static void PlayerGUI_Awake_Patch(PlayerGUI __instance)
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

        public static void PlayerGUI_RefreshActiveMapScanners_Patch(PlayerGUI __instance)
        {
            foreach (Text t in __instance.GetComponentsInChildren<Text>())
            {
                if (t.text.Contains("Radar Systems:"))
                {
                    t.text = LoadLocalization.Instance.GetLocalizedString(t.text);
                }
            }
        }

        public static void PlayerGUI_ShowMapStructureInfo_Patch(PlayerGUI __instance)
        {
            __instance.mapItemInfoText1.text = __instance.mapItemInfoText1.text.Replace("Condition: ", LoadLocalization.Instance.GetLocalizedString("Condition: "));
            if (__instance.mapItemInfoText2.IsActive())
            {
                __instance.mapItemInfoText2.text = LoadLocalization.Instance.GetLocalizedString(__instance.mapItemInfoText2.text);
            }
        }

        public static void PlayerGUI_ShowMapItemInfo_Patch(ref string displayInfo)
        {
            displayInfo = LoadLocalization.Instance.GetLocalizedString(displayInfo);
        }

        public static void PlayerGUI_ShowMapVehicleInfo_Patch(PlayerGUI __instance)
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

        public static IEnumerator PlayerGUI_UpdateFilterCoroutine_Patch(IEnumerator values, PlayerGUI __instance)
        {
            do
            {
                if (!string.IsNullOrEmpty(__instance.mapResourcesFoundText.text) && __instance.mapResourcesFoundText.text.Contains("Scanning"))
                    __instance.mapResourcesFoundText.text = LoadLocalization.Instance.GetLocalizedString("map-" + __instance.mapResourcesFoundText.text).Replace("map-", "");
                yield return values.Current;
            } while (values.MoveNext());
            string text = __instance.mapResourcesFoundText.text.Replace("Found", "").Replace("Resources", "").Trim();
            __instance.mapResourcesFoundText.text = LoadLocalization.Instance.GetLocalizedString("map-Found {0} Resources", text);
        }

        //经纬度
        public static void RadarLongitude_Awake_Patch(RadarLongitude __instance)
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
