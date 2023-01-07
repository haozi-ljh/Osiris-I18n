using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Osiris_I18n
{
    public class TargetHUD_Patch
    {

        public TargetHUD_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(PlayerTargetHUDDisplay), "UpdateHUD", PatchType.postfix, GetType().GetMethod("PlayerTargetHUDDisplay_UpdateHUD_Patch")));
            PatcherManager.Add(new Patcher(Patcher.Find("BioDomeBin", "hudSubtitle", pt: PropertyType.get), PatchType.postfix, GetType().GetMethod("BioDomeBin_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(BuildableObject), "displayName", PropertyType.get, PatchType.postfix, GetType().GetMethod("BuildableObject_displayName_Patch")));
            PatcherManager.Add(new Patcher(typeof(BuildableObject), "worldName", PropertyType.get, PatchType.postfix, GetType().GetMethod("BuildableObject_worldName_Patch")));
            PatcherManager.Add(new Patcher(typeof(CraftingTable), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("CraftingTable_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(Furniture), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("Furniture_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(Geyser), "displayName", PropertyType.get, PatchType.postfix, GetType().GetMethod("Geyser_displayName_Patch")));
            PatcherManager.Add(new Patcher(typeof(HarvestableSegment), "hudName", PropertyType.get, PatchType.postfix, GetType().GetMethod("HarvestableSegment_hudName_Patch")));
            PatcherManager.Add(new Patcher(typeof(HarvestableSegment), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("HarvestableSegment_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(NatureFragment), "hudSubtitle", PropertyType.get, PatchType.transpiler, GetType().GetMethod("NatureFragment_hudSubtitle_Patch_Transpiler")));
            PatcherManager.Add(new Patcher(typeof(NatureFragment), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("NatureFragment_hudSubtitle_Patch_Postfix")));
            PatcherManager.Add(new Patcher(typeof(NatureObject), "displayName", PropertyType.get, PatchType.postfix, GetType().GetMethod("NatureObject_displayName_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerReplenishTrigger), "hudSubtitle", PropertyType.get, PatchType.transpiler, GetType().GetMethod("PlayerReplenishTrigger_hudSubtitle_Patch_Transpiler")));
            PatcherManager.Add(new Patcher(typeof(PlayerReplenishTrigger), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("PlayerReplenishTrigger_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(Puddle), "displayName", PropertyType.get, PatchType.postfix, GetType().GetMethod("Puddle_displayName_Patch")));
            PatcherManager.Add(new Patcher(typeof(Puddle), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("Puddle_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(RepairableFragment), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("RepairableFragment_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(RepairableObject), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("RepairableObject_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(RepairableWreckage), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("RepairableWreckage_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(StructureFragment), "displayName", PropertyType.get, PatchType.postfix, GetType().GetMethod("StructureFragment_displayName_Patch")));
            PatcherManager.Add(new Patcher(typeof(StructureFrame), "hudSubtitle", PropertyType.get, PatchType.postfix, GetType().GetMethod("StructureFrame_hudSubtitle_Patch")));
            PatcherManager.Add(new Patcher(typeof(StructureObject), "IsFunctional", PatchType.transpiler, GetType().GetMethod("StructureObject_IsFunctional_Patch_Transpiler")));

            PatcherManager.Add(new Patcher(Patcher.Find(typeof(VehicleCollisionObject), "hudSecondaryName", pt: PropertyType.get), PatchType.postfix, GetType().GetMethod("hudSecondaryName_Patch")));
            PatcherManager.Add(new Patcher(Patcher.Find(typeof(DroidHealth), "hudSecondaryName", pt: PropertyType.get), PatchType.postfix, GetType().GetMethod("hudSecondaryName_Patch")));
        }

        public static void hudSecondaryName_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        public static void PlayerTargetHUDDisplay_UpdateHUD_Patch(PlayerTargetHUDDisplay __instance)
        {
            string textName = __instance.textName.text.text;
            string textSubtitle = __instance.textSubtitle.text.text;
            string ownerName = __instance.ownerName.text.text;



            __instance.textName.SetText(LoadLocalization.Instance.GetLocalizedString(textName));

            string str;
            if (!textSubtitle.Contains("<color="))
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

        public static void BioDomeBin_hudSubtitle_Patch(ref string __result)
        {
            if (__result.Contains("- Yield"))
            {
                string seedtype = __result.Split('-')[0].Trim();
                __result = __result.Replace(seedtype, LoadLocalization.Instance.GetLocalizedString(seedtype)).Replace("Yield in", LoadLocalization.Instance.GetLocalizedString("Yield in"));
            }
            else
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

        public static void BuildableObject_displayName_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        public static void BuildableObject_worldName_Patch(BuildableObject __instance, ref string __result)
        {
            if (__instance.showNickname)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__result);
            }
        }

        public static void CraftingTable_hudSubtitle_Patch(ref string __result)
        {
            __result = __result.Replace("Assembling ", LoadLocalization.Instance.GetLocalizedString("Assembling "));
        }

        public static void Furniture_hudSubtitle_Patch(ref string __result)
        {
            if (!string.IsNullOrEmpty(__result))
            {
                if (__result.Contains("%"))
                {
                    __result = __result.Replace("Capacity", LoadLocalization.Instance.GetLocalizedString("Capacity"));
                }
                else
                {
                    __result = LoadLocalization.Instance.GetLocalizedString(__result);
                }
            }
        }

        public static void Geyser_displayName_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        public static void HarvestableSegment_hudName_Patch(HarvestableSegment __instance, ref string __result)
        {
            if (__instance.creatureHealthParent.creatureLevel - 1 >= 0 && __instance.creatureHealthParent.creatureLevel - 1 < __instance.creatureHealthParent.creatureData.species.Length)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(__instance.creatureHealthParent.creatureData.species[__instance.creatureHealthParent.creatureLevel - 1].namePrefix) + " " + LoadLocalization.Instance.GetLocalizedString(__instance.creatureHealthParent.creatureData.creatureName);
            }
        }

        public static void HarvestableSegment_hudSubtitle_Patch(ref string __result)
        {
            if (__result.Contains(" - "))
            {
                string text = __result.Replace(" - Harvestable", "").Trim();
                __result = LoadLocalization.Instance.GetLocalizedString(text) + LoadLocalization.Instance.GetLocalizedString(" - Harvestable");
            }
        }

        public static IEnumerable<CodeInstruction> NatureFragment_hudSubtitle_Patch_Transpiler(IEnumerable<CodeInstruction> instructions)
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

        public static void NatureFragment_hudSubtitle_Patch_Postfix(ref string __result)
        {
            if (__result.Contains("Hardness: "))
            {
                __result = __result.Replace("Hardness: ", LoadLocalization.Instance.GetLocalizedString("Hardness: "));
                return;
            }
            if (__result.Contains("Harvestable"))
            {
                __result = __result.Replace("Harvestable", LoadLocalization.Instance.GetLocalizedString("Harvestable"));
            }
            if (__result.Contains("Remaining"))
            {
                __result = __result.Replace("Remaining", LoadLocalization.Instance.GetLocalizedString("Remaining"));
                return;
            }
            if (__result.Contains("Already"))
            {
                __result = __result.Replace("Already Havested", LoadLocalization.Instance.GetLocalizedString("Already Havested"));
                return;
            }
            if (__result.Contains("Dismantle"))
            {
                __result = __result.Replace("Dismantle For Parts", LoadLocalization.Instance.GetLocalizedString("Dismantle For Parts"));
                return;
            }
            if (__result.Contains("Tool"))
            {
                __result = __result.Replace("Use Tool", LoadLocalization.Instance.GetLocalizedString("Use Tool"));
                return;
            }
            if (__result.Contains("Barrel"))
            {
                __result = __result.Replace("Use Barrel", LoadLocalization.Instance.GetLocalizedString("Use Barrel"));
                return;
            }
            if (__result.Contains("Container"))
            {
                __result = __result.Replace("Use Container", LoadLocalization.Instance.GetLocalizedString("Use Container"));
                return;
            }
        }

        public static void NatureObject_displayName_Patch(NatureObject __instance, ref string __result)
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

        public static IEnumerable<CodeInstruction> PlayerReplenishTrigger_hudSubtitle_Patch_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr)
                {
                    if (codes[i].operand.GetType().Equals(typeof(string)))
                    {
                        i++;
                        codes.Insert(i, new CodeInstruction(OpCodes.Call, typeof(TargetHUD_Patch).GetMethod("ResethudSubtitle")));
                    }
                }
            }
            return codes.AsEnumerable();
        }

        public static void PlayerReplenishTrigger_hudSubtitle_Patch(ref string __result)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("(sec)|(mins?)");
            if (regex.IsMatch(__result))
                __result = regex.Replace(__result, LoadLocalization.Instance.GetLocalizedString(regex.Match(__result).Value));
        }

        public static string ResethudSubtitle(string instr)
        {
            string str = instr;
            if (str.Contains("</color>") && !string.IsNullOrEmpty(str.Replace("</color>", "")))
            {
                string[] texts = str.Split(new string[] { "</color>" }, StringSplitOptions.RemoveEmptyEntries);
                if (texts.Length > 0)
                {
                    str = "";
                    foreach (string s in texts)
                    {
                        if (s[0].Equals('>'))
                        {
                            string text = s.Replace(">", "");
                            str = s.Replace(text, LoadLocalization.Instance.GetLocalizedString(text));
                        }
                        else
                            str += LoadLocalization.Instance.GetLocalizedString(s);
                    }
                    str += "</color>";
                }
                else
                {
                    str = LoadLocalization.Instance.GetLocalizedString(str);
                }
            }
            else if (!string.IsNullOrEmpty(str) && !str.Contains("<color=") && !str.Trim().Equals("+"))
            {
                if (str[0].Equals('>'))
                {
                    string text = str.Replace(">", "");
                    str = str.Replace(text, LoadLocalization.Instance.GetLocalizedString(text));
                }
                else
                    str = LoadLocalization.Instance.GetLocalizedString(str);
            }

            return str;
        }

        public static void Puddle_displayName_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        public static void Puddle_hudSubtitle_Patch(ref string __result)
        {
            string text = __result.Split('-')[0].Trim();
            __result = __result.Replace(text, LoadLocalization.Instance.GetLocalizedString(text));
        }

        public static void RepairableFragment_hudSubtitle_Patch(ref string __result)
        {
            int fcut = __result.IndexOf(">") + 1;
            string text = __result.Substring(fcut, __result.Length - fcut - 8);
            if (__result.Contains(" - "))
            {
                if (__result.Contains("Build with"))
                {
                    string str = text.Replace("Build with Tool -", "").Replace("pieces operational", "").Trim();
                    __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Build with Tool") + " - " + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                }
                else if (__result.Contains("Gather Resources"))
                {
                    if (__result.Contains("Repair"))
                    {
                        string str = text.Replace("Gather Resources To Repair -", "").Replace("pieces operational", "").Trim();
                        __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Gather Resources To Repair") + " - " + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                    }
                    else
                    {
                        string str = text.Replace("Gather Resources To Build -", "").Replace("pieces operational", "").Trim();
                        __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Gather Resources To Build - ") + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                    }
                }
                else if (__result.Contains("Repair with"))
                {
                    string str = text.Replace("Repair with Tool -", "").Replace("pieces operational", "").Trim();
                    __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Repair with Tool - ") + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                }
                else if (__result.Contains("Blocked by"))
                {
                    string str = text.Replace("Blocked by wreckage -", "").Replace("pieces operational", "").Trim();
                    __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Blocked by wreckage") + " - " + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                }
                else
                {
                    string str = text.Replace("Operational -", "").Replace("pieces operational", "").Trim();
                    __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Operational - ") + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                }
            }
            else
            {
                __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString(text) + "</color>";
            }
        }

        public static void RepairableObject_hudSubtitle_Patch(ref string __result)
        {
            if (!__result.Contains("<color="))
            {
                if (__result.Contains("pieces operational"))
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
                if (__result.Contains("pieces operational"))
                {
                    if (__result.Contains("Operational -"))
                    {
                        string str = text.Replace("Operational -", "").Replace("pieces operational", "").Trim();
                        __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Operational - ") + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                    }
                    else
                    {
                        string str = text.Replace("Needs Repairs -", "").Replace("pieces operational", "").Trim();
                        __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Needs Repairs - ") + str + LoadLocalization.Instance.GetLocalizedString(" pieces operational") + "</color>";
                    }
                }
                else
                {
                    string str = text.Replace("Needs Repairs -", "").Replace("wreckages cleared", "").Trim();
                    __result = __result.Substring(0, fcut) + LoadLocalization.Instance.GetLocalizedString("Needs Repairs - ") + str + LoadLocalization.Instance.GetLocalizedString(" wreckages cleared") + "</color>";
                }
            }
        }

        public static void RepairableWreckage_hudSubtitle_Patch(ref string __result)
        {
            if (!__result.Contains("<color="))
            {
                if (__result.Contains("Use Tool"))
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
                if (__result.Contains("Use Tool"))
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

        public static void StructureFragment_displayName_Patch(ref string __result)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(__result);
        }

        public static void StructureFrame_hudSubtitle_Patch(ref string __result)
        {
            if (__result.Trim().Contains(" "))
            {
                string str = __result.Split('/')[0];
                string text = str.Remove(str.LastIndexOf(" ")).Trim();
                __result = __result.Replace(text, LoadLocalization.Instance.GetLocalizedString(text));
            }
        }

        public static IEnumerable<CodeInstruction> StructureObject_IsFunctional_Patch_Transpiler(IEnumerable<CodeInstruction> instructions)
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
