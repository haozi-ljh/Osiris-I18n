using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Osiris_I18n
{
    //图鉴
    public class Discovery_Patch
    {

        public Discovery_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(DiscoveryCategoryLoader), "ConstructUI", PatchType.transpiler, GetType().GetMethod("DiscoveryCategoryLoader_ConstructUI_Patch_Transpiler")));
            PatcherManager.Add(new Patcher(typeof(DiscoveryDetailsPanel), "UpdateDetails", PatchType.postfix, GetType().GetMethod("DiscoveryDetailsPanel_UpdateDetails_Patch")));
            PatcherManager.Add(new Patcher(typeof(PopupMessage), "SetDiscovery", PatchType.transpiler, GetType().GetMethod("PopupMessage_SetDiscovery_Patch_Transpiler")));
        }

        public static IEnumerable<CodeInstruction> DiscoveryCategoryLoader_ConstructUI_Patch_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return DiscoveryDetails_Transpiler(instructions);
        }

        public static void DiscoveryDetailsPanel_UpdateDetails_Patch(DiscoveryDetailsPanel __instance)
        {
            if (!string.IsNullOrEmpty(__instance.dateTimeDiscovered.text))
            {
                if (__instance.dateTimeDiscovered.text.Contains("UTC"))
                {
                    __instance.dateTimeDiscovered.text = __instance.dateTimeDiscovered.text.Replace("Discovered: UTC ", LoadLocalization.Instance.GetLocalizedString("Discovered: UTC "));
                }
                else
                {
                    __instance.dateTimeDiscovered.text = LoadLocalization.Instance.GetLocalizedString(__instance.dateTimeDiscovered.text);
                }
            }
            if (!string.IsNullOrEmpty(__instance.description.text))
            {
                __instance.description.text = __instance.description.text.Replace("Hardness: ", LoadLocalization.Instance.GetLocalizedString("Hardness: ")).Replace("Density: ", LoadLocalization.Instance.GetLocalizedString("Density: "));
            }
        }

        public static IEnumerable<CodeInstruction> PopupMessage_SetDiscovery_Patch_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return DiscoveryDetails_Transpiler(instructions);
        }

        private static IEnumerable<CodeInstruction> DiscoveryDetails_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (i < codes.Count - 1 && codes[i].Is(OpCodes.Callvirt, typeof(System.Xml.XmlNode).GetProperty("InnerText").GetGetMethod()))
                {
                    if (codes[i + 1].Is(OpCodes.Stfld, typeof(DiscoveryDetails).GetField("displayName")))
                    {
                        codes.Insert(i - 1, new CodeInstruction(OpCodes.Call, typeof(LocalizationManager).GetProperty("Instance").GetGetMethod()));
                        i += 2;
                        codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(LocalizationManager).GetMethod("GetLocalizedString")));
                        i++;
                        continue;
                    }
                    if (codes[i + 1].Is(OpCodes.Stfld, typeof(DiscoveryDetails).GetField("description")))
                    {
                        codes.Insert(i - 1, new CodeInstruction(OpCodes.Call, typeof(LocalizationManager).GetProperty("Instance").GetGetMethod()));
                        i += 2;
                        codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(LocalizationManager).GetMethod("GetLocalizedString")));
                        i++;
                        continue;
                    }
                    if (codes[i + 1].Is(OpCodes.Stfld, typeof(DiscoveryDetails).GetField("planet")))
                    {
                        codes.Insert(i - 1, new CodeInstruction(OpCodes.Call, typeof(LocalizationManager).GetProperty("Instance").GetGetMethod()));
                        i += 2;
                        codes.Insert(i, new CodeInstruction(OpCodes.Callvirt, typeof(LocalizationManager).GetMethod("GetLocalizedString")));
                        i++;
                        continue;
                    }
                }
            }

            return codes.AsEnumerable();
        }

    }
}
