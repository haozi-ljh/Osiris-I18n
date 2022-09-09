using HarmonyLib;

namespace Osiris_I18n
{
    class AbilityNode_Patch
    {

        [HarmonyPatch(typeof(AbilityNode), "abilityRequirement", MethodType.Getter)]
        class AbilityNode_abilityRequirement_Patch
        {
            public static void Postfix(ref string __result)
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
        }

    }

    class MenuBeginnerSkillWeb_Patch
    {

        [HarmonyPatch(typeof(MenuBeginnerSkillWeb), nameof(MenuBeginnerSkillWeb.PayAbilityCost))]
        class MenuBeginnerSkillWeb_PayAbilityCost_Patch
        {
            public static void Postfix(MenuBeginnerSkillWeb __instance)
            {
                string[] combat = __instance.combat.text.Split(':');
                string[] engineering = __instance.engineering.text.Split(':');
                string[] science = __instance.science.text.Split(':');

                __instance.combat.text = LoadLocalization.Instance.GetLocalizedString(combat[0] + ": ") + combat[1].Trim();
                __instance.engineering.text = LoadLocalization.Instance.GetLocalizedString(engineering[0] + ": ") + engineering[1].Trim();
                __instance.science.text = LoadLocalization.Instance.GetLocalizedString(science[0] + ": ") + science[1].Trim();
            }
        }
        
        [HarmonyPatch(typeof(MenuBeginnerSkillWeb), nameof(MenuBeginnerSkillWeb.ResetSkillWeb))]
        class MenuBeginnerSkillWeb_ResetSkillWeb_Patch
        {
            public static void Postfix(MenuBeginnerSkillWeb __instance)
            {
                string[] combat = __instance.combat.text.Split(':');
                string[] engineering = __instance.engineering.text.Split(':');
                string[] science = __instance.science.text.Split(':');

                __instance.combat.text = LoadLocalization.Instance.GetLocalizedString(combat[0] + ": ") + combat[1].Trim();
                __instance.engineering.text = LoadLocalization.Instance.GetLocalizedString(engineering[0] + ": ") + engineering[1].Trim();
                __instance.science.text = LoadLocalization.Instance.GetLocalizedString(science[0] + ": ") + science[1].Trim();
            }
        }

    }

    class UIAbilityDisplay_Patch
    {

        [HarmonyPatch(typeof(UIAbilityDisplay), "activeAbility", MethodType.Setter)]
        class UIAbilityDisplay_activeAbility_Patch
        {
            public static void Postfix(UIAbilityDisplay __instance)
            {
                __instance.title.text = LoadLocalization.Instance.GetLocalizedString(__instance.title.text);
                __instance.description.text = LoadLocalization.Instance.GetLocalizedString(__instance.title.text);
            }
        }
        
        [HarmonyPatch(typeof(UIAbilityDisplay), "SetAbilityStatus")]
        class UIAbilityDisplay_SetAbilityStatus_Patch
        {
            public static void Postfix(UIAbilityDisplay __instance)
            {
                __instance.status.text = LoadLocalization.Instance.GetLocalizedString(__instance.status.text);
            }
        }

    }
}
