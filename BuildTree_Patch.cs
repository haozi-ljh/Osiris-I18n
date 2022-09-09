using HarmonyLib;
using System;
using UnityEngine.UI;

namespace Osiris_I18n
{
    class BuildTree_Patch
    {

        [HarmonyPatch(typeof(BuildOption), MethodType.Constructor, new Type[] { typeof(int), typeof(string), typeof(string), typeof(ItemPair[]), typeof(bool) })]
        class BuildOption_Constructor_Patch
        {
            public static void Prefix(int id, ref string name, string description, ItemPair[] build, bool isItem)
            {
                string text = name.Remove(name.LastIndexOf("[") - 1).Trim();
                name = name.Replace(text, LoadLocalization.Instance.GetLocalizedString(text));
            }

            public static void Postfix(BuildOption __instance)
            {
                __instance.description = LoadLocalization.Instance.GetLocalizedString(__instance.description);
            }
        }

        [HarmonyPatch(typeof(BuildTreePanel), nameof(BuildTreePanel.ShowBuild))]
        class BuildTreePanel_ShowBuild_Patch
        {
            public static void Postfix(BuildTreePanel __instance)
            {
                Text[] t = __instance.GetComponentsInChildren<Text>();
                t[t.Length - 1].text = LoadLocalization.Instance.GetLocalizedString(t[t.Length - 1].text);
            }
        }

    }
}
