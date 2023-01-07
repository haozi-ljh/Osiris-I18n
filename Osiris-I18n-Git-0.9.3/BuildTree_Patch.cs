using System;
using UnityEngine.UI;

namespace Osiris_I18n
{
    public class BuildTree_Patch
    {

        public BuildTree_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(BuildOption), null, new Type[] { typeof(int), typeof(string), typeof(string), typeof(ItemPair[]), typeof(bool) }, PatchType.prefix, GetType().GetMethod("BuildOption_Constructor_Patch_prefix")));
            PatcherManager.Add(new Patcher(typeof(BuildOption), null, new Type[] { typeof(int), typeof(string), typeof(string), typeof(ItemPair[]), typeof(bool) }, PatchType.postfix, GetType().GetMethod("BuildOption_Constructor_Patch_postfix")));
            PatcherManager.Add(new Patcher(typeof(BuildTreePanel), "ShowBuild", PatchType.postfix, GetType().GetMethod("BuildTreePanel_ShowBuild_Patch")));
        }

        public static void BuildOption_Constructor_Patch_prefix(int id, ref string name, string description, ItemPair[] build, bool isItem)
        {
            string text = name.Remove(name.LastIndexOf("[") - 1).Trim();
            name = name.Replace(text, LoadLocalization.Instance.GetLocalizedString(text));
        }

        public static void BuildOption_Constructor_Patch_postfix(BuildOption __instance)
        {
            __instance.description = LoadLocalization.Instance.GetLocalizedString(__instance.description);
        }

        public static void BuildTreePanel_ShowBuild_Patch(BuildTreePanel __instance)
        {
            Text[] t = __instance.GetComponentsInChildren<Text>();
            t[t.Length - 1].text = LoadLocalization.Instance.GetLocalizedString(t[t.Length - 1].text);
        }

    }
}
