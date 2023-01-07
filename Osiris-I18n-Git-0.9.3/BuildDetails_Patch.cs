
namespace Osiris_I18n
{
    public class BuildDetails_Patch
    {

        public BuildDetails_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(BuildDetails), "name", PropertyType.get, PatchType.postfix, GetType().GetMethod("BuildDetails_name_Patch")));
            PatcherManager.Add(new Patcher(typeof(BuildDetails), "title", PropertyType.get, PatchType.postfix, GetType().GetMethod("BuildDetails_title_Patch")));
            PatcherManager.Add(new Patcher(typeof(BuildDetails), "description", PropertyType.get, PatchType.postfix, GetType().GetMethod("BuildDetails_description_Patch")));
        }

        public static void BuildDetails_name_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        public static void BuildDetails_title_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        public static void BuildDetails_description_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

    }
}
