
namespace Osiris_I18n
{
    public class Ability_Patch
    {

        public Ability_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(AbilityNode), "abilityRequirement", PropertyType.get, PatchType.postfix, GetType().GetMethod("AbilityNode_abilityRequirement_Patch")));
            PatcherManager.Add(new Patcher(typeof(MenuBeginnerSkillWeb), "PayAbilityCost", PatchType.postfix, GetType().GetMethod("MenuBeginnerSkillWeb_PayAbilityCostANDResetSkillWeb_Patch")));
            PatcherManager.Add(new Patcher(typeof(MenuBeginnerSkillWeb), "ResetSkillWeb", PatchType.postfix, GetType().GetMethod("MenuBeginnerSkillWeb_PayAbilityCostANDResetSkillWeb_Patch")));
            PatcherManager.Add(new Patcher(typeof(UIAbilityDisplay), "activeAbility", PropertyType.set, PatchType.postfix, GetType().GetMethod("UIAbilityDisplay_activeAbility_Patch")));
            PatcherManager.Add(new Patcher(typeof(UIAbilityDisplay), "SetAbilityStatus", PatchType.postfix, GetType().GetMethod("UIAbilityDisplay_SetAbilityStatus_Patch")));
        }

        public static void AbilityNode_abilityRequirement_Patch(ref string __result)
        {
            int startindex = __result.IndexOf(">") + 1;
            if (startindex <= 0)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
            else
            {
                string text = __result.Substring(startindex, __result.LastIndexOf("</") - startindex);
                __result = __result.Replace(text, LoadLocalization.Instance.GetLocalizedString("p-" + text));
            }
        }

        public static void MenuBeginnerSkillWeb_PayAbilityCostANDResetSkillWeb_Patch(MenuBeginnerSkillWeb __instance)
        {
            string[] combat = __instance.combat.text.Split(':');
            string[] engineering = __instance.engineering.text.Split(':');
            string[] science = __instance.science.text.Split(':');

            __instance.combat.text = LoadLocalization.Instance.GetLocalizedString(combat[0] + ": ") + combat[1].Trim();
            __instance.engineering.text = LoadLocalization.Instance.GetLocalizedString(engineering[0] + ": ") + engineering[1].Trim();
            __instance.science.text = LoadLocalization.Instance.GetLocalizedString(science[0] + ": ") + science[1].Trim();
        }

        public static void UIAbilityDisplay_activeAbility_Patch(UIAbilityDisplay __instance)
        {
            __instance.title.text = LoadLocalization.Instance.GetLocalizedString(__instance.title.text);
            __instance.description.text = LoadLocalization.Instance.GetLocalizedString(__instance.title.text);
        }

        public static void UIAbilityDisplay_SetAbilityStatus_Patch(UIAbilityDisplay __instance)
        {
            __instance.status.text = LoadLocalization.Instance.GetLocalizedString("a-" + __instance.status.text);
        }

    }
}
