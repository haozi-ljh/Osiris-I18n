using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Osiris_I18n
{
    
    class TargetHUD_Patch
    {

        public TargetHUD_Patch(Harmony harmony)
        {
            if(typeof(VehicleCollisionObject).GetProperty("hudSecondaryName") != null)
            {
                harmony.Patch(typeof(VehicleCollisionObject).GetProperty("hudSecondaryName").GetGetMethod(), postfix: new HarmonyMethod(GetType().GetMethod("hudSecondaryName")));
            }
            if(typeof(DroidHealth).GetProperty("hudSecondaryName") != null)
            {
                harmony.Patch(typeof(DroidHealth).GetProperty("hudSecondaryName").GetGetMethod(), postfix: new HarmonyMethod(GetType().GetMethod("hudSecondaryName")));
            }
        }

        [HarmonyPatch(typeof(PlayerTargetHUDDisplay), "UpdateHUD")]
        class PlayerTargetHUDDisplay_UpdateHUD_Patch
        {
            public static void Postfix(PlayerTargetHUDDisplay __instance)
            {
                string textName = __instance.textName.text.text;
                string textSubtitle = __instance.textSubtitle.text.text;
                string ownerName = __instance.ownerName.text.text;



                __instance.textName.SetText(LoadLocalization.Instance.GetLocalizedString(textName));

                string str;
                if (textSubtitle.IndexOf("<color=") < 0)
                {
                    str = LoadLocalization.Instance.GetLocalizedString(textSubtitle);
                }
                else
                {
                    int fcut = textSubtitle.IndexOf(">") + 1;
                    string text = textSubtitle.Substring(fcut, textSubtitle.Length - fcut - 8);
                    str = textSubtitle.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString(text) + "</color>";
                }
                __instance.textSubtitle.SetText(LoadLocalization.Instance.GetLocalizedString(str));

                __instance.ownerName.SetText(LoadLocalization.Instance.GetLocalizedString(ownerName));
            }
        }

        public static void hudSecondaryName(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        [HarmonyPatch(typeof(BuildableObject), "displayName", MethodType.Getter)]
        class BuildableObject_displayName_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

        [HarmonyPatch(typeof(BuildableObject), "worldName", MethodType.Getter)]
        class BuildableObject_worldName_Patch
        {
            public static void Postfix(BuildableObject __instance, ref string __result)
            {
                if (__instance.showNickname)
                {
                    __result = LoadLocalization.Instance.GetLocalizedString(__result);
                }
            }
        }

        [HarmonyPatch(typeof(CraftingTable), "hudSubtitle", MethodType.Getter)]
        class CraftingTable_hudSubtitle_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = __result.Replace("Assembling ", LoadLocalization.Instance.GetLocalizedString("Assembling "));
            }
        }

        [HarmonyPatch(typeof(Geyser), "displayName", MethodType.Getter)]
        class Geyser_displayName_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }
        
        [HarmonyPatch(typeof(HarvestableSegment), "hudName", MethodType.Getter)]
        class HarvestableSegment_hudName_Patch
        {
            public static void Postfix(HarvestableSegment __instance, ref string __result)
            {
                if (__instance.creatureHealthParent.creatureLevel - 1 >= 0 && __instance.creatureHealthParent.creatureLevel - 1 < __instance.creatureHealthParent.creatureData.species.Length)
                {
                    __result = LoadLocalization.Instance.GetLocalizedString(__instance.creatureHealthParent.creatureData.species[__instance.creatureHealthParent.creatureLevel - 1].namePrefix) + " " + LoadLocalization.Instance.GetLocalizedString(__instance.creatureHealthParent.creatureData.creatureName);
                }
            }
        }

        [HarmonyPatch(typeof(HarvestableSegment), "hudSubtitle", MethodType.Getter)]
        class HarvestableSegment_hudSubtitle_Patch
        {
            public static void Postfix(ref string __result)
            {
                if(__result.IndexOf(" - ") >= 0)
                {
                    string text = __result.Replace(" - Harvestable", "").Trim();
                    __result = LoadLocalization.Instance.GetLocalizedString(text) + LoadLocalization.Instance.GetLocalizedString(" - Harvestable");
                }
            }
        }

        [HarmonyPatch(typeof(NatureFragment), "hudSubtitle", MethodType.Getter)]
        class NatureFragment_hudSubtitle_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (i > 3)
                    {
                        if (codes[i].opcode == OpCodes.Ldloc_0 && codes[i - 1].opcode == OpCodes.Stloc_0 && codes[i - 2].opcode == OpCodes.Callvirt && codes[i - 3].opcode == OpCodes.Callvirt)
                        {
                            codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(LocalizationManager).GetProperty("Instance").GetGetMethod()));
                            i += 12;
                            codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(LocalizationManager).GetMethod("GetLocalizedString")));
                            break;
                        }
                    }
                }

                return codes.AsEnumerable();
            }

            public static void Postfix(ref string __result)
            {
                if (__result.IndexOf("Hardness: ") >= 0)
                {
                    __result = __result.Replace("Hardness: ", LoadLocalization.Instance.GetLocalizedString("Hardness: "));
                    return;
                }
                if (__result.IndexOf("Harvestable") >= 0)
                {
                    __result = __result.Replace("Harvestable", LoadLocalization.Instance.GetLocalizedString("Harvestable"));
                }
                if (__result.IndexOf("Remaining") >= 0)
                {
                    __result = __result.Replace("Remaining", LoadLocalization.Instance.GetLocalizedString("Remaining"));
                    return;
                }
                if (__result.IndexOf("Already") >= 0)
                {
                    __result = __result.Replace("Already Havested", LoadLocalization.Instance.GetLocalizedString("Already Havested"));
                    return;
                }
                if (__result.IndexOf("Dismantle") >= 0)
                {
                    __result = __result.Replace("Dismantle For Parts", LoadLocalization.Instance.GetLocalizedString("Dismantle For Parts"));
                    return;
                }
                if (__result.IndexOf("Tool") >= 0)
                {
                    __result = __result.Replace("Use Tool", LoadLocalization.Instance.GetLocalizedString("Use Tool"));
                    return;
                }
                if (__result.IndexOf("Barrel") >= 0)
                {
                    __result = __result.Replace("Use Barrel", LoadLocalization.Instance.GetLocalizedString("Use Barrel"));
                    return;
                }
                if (__result.IndexOf("Container") >= 0)
                {
                    __result = __result.Replace("Use Container", LoadLocalization.Instance.GetLocalizedString("Use Container"));
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(NatureObject), "displayName", MethodType.Getter)]
        class NatureObject_displayName_Patch
        {
            public static void Postfix(NatureObject __instance, ref string __result)
            {
                if (!__instance.isRandom && string.IsNullOrEmpty(__instance.specialDisplayName))
                {
                    if (!string.IsNullOrEmpty(__instance.displayNamePrefix))
                    {
                        __result = LoadLocalization.Instance.GetLocalizedString(__instance.displayNamePrefix) + " " + LoadLocalization.Instance.GetLocalizedString(__instance.type) + " " + LoadLocalization.Instance.GetLocalizedString(__instance.displayNamePostfix);
                    }
                    else
                    {
                        __result = LoadLocalization.Instance.GetLocalizedString(__instance.type) + " " + LoadLocalization.Instance.GetLocalizedString(__instance.displayNamePostfix);
                    }
                }
                else
                {
                    __result = LoadLocalization.Instance.GetLocalizedString(__result);
                }
            }
        }

        [HarmonyPatch(typeof(Puddle), "displayName", MethodType.Getter)]
        class Puddle_displayName_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }
        
        [HarmonyPatch(typeof(Puddle), "hudSubtitle", MethodType.Getter)]
        class Puddle_hudSubtitle_Patch
        {
            public static void Postfix(ref string __result)
            {
                string text = __result.Split('-')[0].Trim();
                __result = __result.Replace(text, LoadLocalization.Instance.GetLocalizedString(text));
            }
        }

        [HarmonyPatch(typeof(RepairableObject), "hudSubtitle", MethodType.Getter)]
        class RepairableObject_hudSubtitle_Patch
        {
            public static void Postfix(ref string __result)
            {
                if (__result.IndexOf("<color=") < 0)
                {
                    if (__result.IndexOf("pieces operational") >= 0)
                    {
                        string text = __result.Replace("Needs Repairs -", "").Replace("pieces operational", "").Trim();
                        __result = LoadLocalization.Instance.GetLocalizedString("Needs Repairs - ") + text + LoadLocalization.Instance.GetLocalizedString(" pieces operational");
                    }
                    else
                    {
                        string text = __result.Replace("Needs Repairs -", "").Replace("wreckages cleared", "").Trim();
                        __result = LoadLocalization.Instance.GetLocalizedString("Needs Repairs - ") + text + LoadLocalization.Instance.GetLocalizedString(" wreckages cleared");
                    }
                }
                else
                {
                    int fcut = __result.IndexOf(">") + 1;
                    string text = __result.Substring(fcut, __result.Length - fcut - 8);
                    if (__result.IndexOf("pieces operational") >= 0)
                    {
                        string str = text.Replace("Needs Repairs -", "").Replace("pieces operational", "").Trim();
                        __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Needs Repairs - ") + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                    }
                    else
                    {
                        string str = text.Replace("Needs Repairs -", "").Replace("wreckages cleared", "").Trim();
                        __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Needs Repairs - ") + str + LoadLocalization.Instance.GetLocalizedString(" wreckages cleared") + "</color>";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RepairableWreckage), "hudSubtitle", MethodType.Getter)]
        class RepairableWreckage_hudSubtitle_Patch
        {
            public static void Postfix(ref string __result)
            {
                if (__result.IndexOf("<color=") < 0)
                {
                    if (__result.IndexOf("Use Tool") >= 0)
                    {
                        __result = LoadLocalization.Instance.GetLocalizedString("Salvageable") + LoadLocalization.Instance.GetLocalizedString(" - Use Tool");
                    }
                    else
                    {
                        string text = __result.Replace("Salvageable", "");
                        __result = LoadLocalization.Instance.GetLocalizedString("Salvageable") + text;
                    }
                }
                else
                {
                    int fcut = __result.IndexOf(">") + 1;
                    string text = __result.Substring(fcut, __result.Length - fcut - 8);
                    if (__result.IndexOf("Use Tool") >= 0)
                    {
                        __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Salvageable") + LoadLocalization.Instance.GetLocalizedString(" - Use Tool") + "</color>";
                    }
                    else
                    {
                        string str = text.Replace("Salvageable", "");
                        __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Salvageable") + str + "</color>";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(StructureFragment), "displayName", MethodType.Getter)]
        class StructureFragment_displayName_Patch
        {
            public static void Postfix(ref string __result)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }
        
        [HarmonyPatch(typeof(StructureFrame), "hudSubtitle", MethodType.Getter)]
        class StructureFrame_hudSubtitle_Patch
        {
            public static void Postfix(ref string __result)
            {
                if(__result.Trim().IndexOf(" ") >= 0)
                {
                    string str = __result.Split('/')[0];
                    string text = str.Remove(str.LastIndexOf(" ")).Trim();
                    __result = __result.Replace(text, LoadLocalization.Instance.GetLocalizedString(text));
                }
            }
        }

        [HarmonyPatch(typeof(StructureObject), nameof(StructureObject.IsFunctional))]
        class StructureObject_IsFunctional_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

                bool firstpass = false;
                for (int i = 0; i < codes.Count; i++)
                {
                    if (!firstpass && codes[i].opcode == OpCodes.Ret)
                    {
                        firstpass = true;
                    }
                    if (firstpass && codes[i].opcode == OpCodes.Ldstr)
                    {
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(LocalizationManager).GetProperty("Instance").GetGetMethod()));
                        i += 2;
                        codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(LocalizationManager).GetMethod("GetLocalizedString")));
                        i++;
                    }
                }

                return codes.AsEnumerable();
            }
        }

    }

}
