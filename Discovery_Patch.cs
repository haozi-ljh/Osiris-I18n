using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Osiris_I18n
{
    //图鉴
    class Discovery_Patch
    {

        [HarmonyPatch(typeof(DiscoveryCategoryLoader), "ConstructUI")]
        class DiscoveryCategoryLoader_ConstructUI_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return DiscoveryDetails_Patch(instructions);
            }
        }

        [HarmonyPatch(typeof(DiscoveryDetailsPanel), nameof(DiscoveryDetailsPanel.UpdateDetails))]
        class DiscoveryDetailsPanel_UpdateDetails_Patch
        {
            public static void Postfix(DiscoveryDetailsPanel __instance)
            {
                if (!string.IsNullOrEmpty(__instance.dateTimeDiscovered.text))
                {
                    if (__instance.dateTimeDiscovered.text.IndexOf("UTC") >= 0)
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
        }

        [HarmonyPatch(typeof(PopupMessage), "SetDiscovery")]
        class PopupMessage_SetDiscovery_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return DiscoveryDetails_Patch(instructions);
            }
        }

        private static IEnumerable<CodeInstruction> DiscoveryDetails_Patch(IEnumerable<CodeInstruction> instructions)
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
