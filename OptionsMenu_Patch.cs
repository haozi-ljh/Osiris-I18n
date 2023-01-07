using HarmonyLib;
using UnityEngine.UI;

namespace Osiris_I18n
{
    public class OptionsMenu_Patch
    {

        private static bool isPlayerGUIOpenOptions = false;
        private static bool addtext = false;
        private static bool isnew = true;

        public OptionsMenu_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "BringInOptionsMenu", PatchType.prefix, GetType().GetMethod("PlayerGUI_BringInOptionsMenu_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "InitializeOptions", PatchType.postfix, GetType().GetMethod("OptionsMenu_InitializeOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateFOV", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateFOV_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateThirdFOV", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateThirdFOV_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateFirstPersonCameraBob", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateFirstPersonCameraBob_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateThirdPersonCameraBob", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateThirdPersonCameraBob_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateCameraShake", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateCameraShake_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateGraphicsQuality", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateGraphicsQuality_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateHUDFadeOpacity", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateHUDFadeOpacity_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateLimitFramerate", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateLimitFramerate_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateShadows", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateShadows_Patch")));
            PatcherManager.Add(new Patcher(typeof(OptionsMenu), "UpdateTerrainDetail", PatchType.postfix, GetType().GetMethod("OptionsMenu_UpdateTerrainDetail_Patch")));
        }

        public static void PlayerGUI_BringInOptionsMenu_Patch()
        {
            isPlayerGUIOpenOptions = true;
        }

        public static void OptionsMenu_InitializeOptions_Patch(OptionsMenu __instance)
        {
            isnew = string.IsNullOrEmpty(new System.Text.RegularExpressions.Regex("[0-9\\(\\)]").Replace(__instance.Text_FOV.text, ""));
            addtext = isPlayerGUIOpenOptions && isnew;
            isPlayerGUIOpenOptions = false;

            if (addtext)
            {
                __instance.Text_FOV.text = LoadLocalization.Instance.GetLocalizedString("First Person Field of View") + ": " + __instance.Text_FOV.text;
                __instance.Text_ThirdFOV.text = LoadLocalization.Instance.GetLocalizedString("Third Person Field of View") + ": " + __instance.Text_ThirdFOV.text;
                __instance.Text_GraphicsQuality.text = LoadLocalization.Instance.GetLocalizedString("Preset") + ": " + LoadLocalization.Instance.GetLocalizedString(__instance.Text_GraphicsQuality.text);
                if (__instance.Slider_LimitFramerate != null)
                    __instance.Text_LimitFramerate.text = LoadLocalization.Instance.GetLocalizedString("Max Framerate") + ": " + __instance.Text_LimitFramerate.text;
            }
            else
            {
                if (isnew)
                {
                    __instance.Text_GraphicsQuality.text = LoadLocalization.Instance.GetLocalizedString(__instance.Text_GraphicsQuality.text);
                    __instance.Text_FirstPersonCameraBob.text = LoadLocalization.Instance.GetLocalizedString(__instance.Text_FirstPersonCameraBob.text);
                    __instance.Text_ThirdPersonCameraBob.text = LoadLocalization.Instance.GetLocalizedString(__instance.Text_ThirdPersonCameraBob.text);
                    __instance.Text_CameraShake.text = LoadLocalization.Instance.GetLocalizedString(__instance.Text_CameraShake.text);

                    Text[] ts = __instance.GetComponentsInChildren<Text>();
                    foreach (Text t in ts)
                    {
                        switch (t.text)
                        {
                            case "Max Framerate":
                                t.text = LoadLocalization.Instance.GetLocalizedString(t.text);
                                break;
                            case "Display":
                                t.text = LoadLocalization.Instance.GetLocalizedString(t.text);
                                break;
                            case "Gameplay":
                                t.text = LoadLocalization.Instance.GetLocalizedString(t.text);
                                break;
                            case "Key Binding":
                                t.text = LoadLocalization.Instance.GetLocalizedString(t.text);
                                break;
                        }
                    }
                }
                else
                {
                    string text = __instance.Text_GraphicsQuality.text.Split(':')[1].Trim();
                    __instance.Text_GraphicsQuality.text = LoadLocalization.Instance.GetLocalizedString("Graphics Quality: ") + LoadLocalization.Instance.GetLocalizedString(text);
                    if (__instance.Slider_LimitFramerate != null)
                    {
                        text = __instance.Text_LimitFramerate.text.Split(':')[1].Trim();
                        __instance.Text_LimitFramerate.text = LoadLocalization.Instance.GetLocalizedString("Limit Framerate: ") + (text.Contains("fps") ? text : "(" + LoadLocalization.Instance.GetLocalizedString("Vsync") + ")");
                    }
                    text = __instance.Text_FirstPersonCameraBob.text.Split(':')[1].Trim();
                    __instance.Text_FirstPersonCameraBob.text = LoadLocalization.Instance.GetLocalizedString("First Person Camera Bob: ") + LoadLocalization.Instance.GetLocalizedString(text);
                    text = __instance.Text_ThirdPersonCameraBob.text.Split(':')[1].Trim();
                    __instance.Text_ThirdPersonCameraBob.text = LoadLocalization.Instance.GetLocalizedString("Third Person Camera Bob: ") + LoadLocalization.Instance.GetLocalizedString(text);
                    text = __instance.Text_CameraShake.text.Split(':')[1].Trim();
                    __instance.Text_CameraShake.text = LoadLocalization.Instance.GetLocalizedString("Camera Shake Strength: ") + LoadLocalization.Instance.GetLocalizedString(text);
                }
            }
        }

        public static void OptionsMenu_UpdateFOV_Patch(OptionsMenu __instance)
        {
            if (addtext)
                __instance.Text_FOV.text = LoadLocalization.Instance.GetLocalizedString("First Person Field of View") + ": " + __instance.Text_FOV.text;
        }

        public static void OptionsMenu_UpdateThirdFOV_Patch(OptionsMenu __instance)
        {
            if (addtext)
                __instance.Text_ThirdFOV.text = LoadLocalization.Instance.GetLocalizedString("Third Person Field of View") + ": " + __instance.Text_ThirdFOV.text;
            else if (!isnew)
            {
                string text = __instance.Text_ThirdFOV.text.Split(':')[1].Trim();
                __instance.Text_ThirdFOV.text = LoadLocalization.Instance.GetLocalizedString("Third Person Field of View: ") + (text.Contains("Def") ? text.Replace("Default", LoadLocalization.Instance.GetLocalizedString("Default")) : text);
            }
        }

        public static void OptionsMenu_UpdateFirstPersonCameraBob_Patch(OptionsMenu __instance)
        {
            if (!isnew)
            {
                string text = __instance.Text_FirstPersonCameraBob.text.Split(':')[1].Trim();
                __instance.Text_FirstPersonCameraBob.text = LoadLocalization.Instance.GetLocalizedString("First Person Camera Bob: ") + LoadLocalization.Instance.GetLocalizedString(text);
            }
        }

        public static void OptionsMenu_UpdateThirdPersonCameraBob_Patch(OptionsMenu __instance)
        {
            if (!isnew)
            {
                string text = __instance.Text_ThirdPersonCameraBob.text.Split(':')[1].Trim();
                __instance.Text_ThirdPersonCameraBob.text = LoadLocalization.Instance.GetLocalizedString("Third Person Camera Bob: ") + LoadLocalization.Instance.GetLocalizedString(text);
            }
        }

        public static void OptionsMenu_UpdateCameraShake_Patch(OptionsMenu __instance)
        {
            if (!isnew)
            {
                string text = __instance.Text_CameraShake.text.Split(':')[1].Trim();
                __instance.Text_CameraShake.text = LoadLocalization.Instance.GetLocalizedString("Camera Shake Strength: ") + LoadLocalization.Instance.GetLocalizedString(text);
            }
        }

        public static void OptionsMenu_UpdateGraphicsQuality_Patch(OptionsMenu __instance)
        {
            if (addtext)
                __instance.Text_GraphicsQuality.text = LoadLocalization.Instance.GetLocalizedString("Preset") + ": " + LoadLocalization.Instance.GetLocalizedString(__instance.Text_GraphicsQuality.text);
            else if (isnew)
                __instance.Text_GraphicsQuality.text = LoadLocalization.Instance.GetLocalizedString(__instance.Text_GraphicsQuality.text);
            else
            {
                string text = __instance.Text_GraphicsQuality.text.Split(':')[1].Trim();
                __instance.Text_GraphicsQuality.text = LoadLocalization.Instance.GetLocalizedString("Graphics Quality: ") + LoadLocalization.Instance.GetLocalizedString(text);
            }
        }

        public static void OptionsMenu_UpdateHUDFadeOpacity_Patch(OptionsMenu __instance)
        {
            Traverse Text_HUDFadeOpacity = Traverse.Create(__instance).Field("Text_HUDFadeOpacity");
            if (Text_HUDFadeOpacity != null)
            {
                string str = Text_HUDFadeOpacity.GetValue<Text>().text.Split(':')[1].Trim();
                Text_HUDFadeOpacity.GetValue<Text>().text = LoadLocalization.Instance.GetLocalizedString("HUD Fade Opacity: ") + LoadLocalization.Instance.GetLocalizedString(str);
            }
        }

        public static void OptionsMenu_UpdateLimitFramerate_Patch(OptionsMenu __instance)
        {
            bool m_vsync = Traverse.Create(__instance).Field("m_vsync").GetValue<bool>();
            if (!m_vsync)
            {
                if (addtext)
                    __instance.Text_LimitFramerate.text = LoadLocalization.Instance.GetLocalizedString("Max Framerate") + ": " + __instance.Text_LimitFramerate.text;
                else if (!isnew)
                {
                    string text = __instance.Text_LimitFramerate.text.Split(':')[1].Trim();
                    __instance.Text_LimitFramerate.text = LoadLocalization.Instance.GetLocalizedString("Limit Framerate: ") + (text.Contains("fps") ? text : "(" + LoadLocalization.Instance.GetLocalizedString("Vsync") + ")");
                }
            }
        }

        public static void OptionsMenu_UpdateShadows_Patch(OptionsMenu __instance, float value)
        {
            __instance.Text_Shadows.text = LoadLocalization.Instance.GetLocalizedString("Shadow Distance: ") + LoadLocalization.Instance.GetLocalizedString(__instance.settingsData.shadowsFriendlyNames[(int)value]);
        }

        public static void OptionsMenu_UpdateTerrainDetail_Patch(OptionsMenu __instance, float value)
        {
            __instance.Text_TerrainDetail.text = LoadLocalization.Instance.GetLocalizedString("Terrain Detail: ") + LoadLocalization.Instance.GetLocalizedString(__instance.settingsData.terrainDetailFriendlyNames[(int)value]);
        }

    }
}
