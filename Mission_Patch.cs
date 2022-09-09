using HarmonyLib;
using UnityEngine;
using System;
using System.Xml;

namespace Osiris_I18n
{

    class MissionData_Patch
    {

        [HarmonyPatch(typeof(MissionData), MethodType.Constructor, new Type[] { typeof(string), typeof(string), typeof(XmlNodeList) })]
        class MissionData_Constructor_Patch
        {
            public static void Postfix(MissionData __instance)
            {
                __instance.title = LoadLocalization.Instance.GetLocalizedString(__instance.title + "");
                __instance.description = LoadLocalization.Instance.GetLocalizedString(__instance.description + "");
                __instance.status = LoadLocalization.Instance.GetLocalizedString(__instance.status + "");
            }
        }

        [HarmonyPatch(typeof(MissionData), "currentStatus", MethodType.Getter)]
        class MissionData_currentStatus_Patch
        {
            public static void Postfix(ref string __result)
            {
                int typeindex = __result.LastIndexOf(" ");
                if (__result.LastIndexOf("harvested") > 0)
                {
                    __result = LoadLocalization.Instance.GetLocalizedString("harvested") + __result.Substring(0, typeindex) + "\n";
                }
                else if (__result.LastIndexOf("killed") > 0)
                {
                    __result = LoadLocalization.Instance.GetLocalizedString("killed") + __result.Substring(0, typeindex) + "\n";
                }
                else if (__result.LastIndexOf("found") > 0)
                {
                    __result = LoadLocalization.Instance.GetLocalizedString("found") + __result.Substring(0, typeindex) + "\n";
                }
                else if (__result.LastIndexOf("crafted") > 0)
                {
                    __result = LoadLocalization.Instance.GetLocalizedString("crafted") + __result.Substring(0, typeindex) + "\n";
                }
            }
        }

    }

    class MissionData__MissionObjective_Patch
    {

        [HarmonyPatch(typeof(MissionData.MissionObjective), nameof(MissionData.MissionObjective.ToString))]
        class MissionData__MissionObjective_ToString_Patch
        {
            public static void Postfix(MissionData.MissionObjective __instance, ref string __result)
            {
                if (__result.IndexOf(__instance.name) > 0)
                {
                    __result = __result.Replace(__instance.name, LoadLocalization.Instance.GetLocalizedString(__instance.name));
                }
                if (__result.LastIndexOf('s') == __result.Length - 1)
                {
                    __result = __result.Remove(__result.Length - 1);
                }
            }
        }

    }

}
