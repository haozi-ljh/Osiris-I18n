using HarmonyLib;

namespace Osiris_I18n
{
    public class BuildDetails_Patch
    {

        [HarmonyPatch(typeof(BuildDetails), "name", MethodType.Getter)]
        class BuildDetails_name_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

        [HarmonyPatch(typeof(BuildDetails), "title", MethodType.Getter)]
        class BuildDetails_title_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

        [HarmonyPatch(typeof(BuildDetails), "description", MethodType.Getter)]
        class BuildDetails_description_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

    }
}
