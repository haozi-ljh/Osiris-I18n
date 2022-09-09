using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Osiris_I18n
{
    class MenuOption_Patch
    {

        public MenuOption_Patch(Harmony harmony)
        {
            HarmonyMethod newMenu = new HarmonyMethod(GetType().GetMethod("newMenu"));
            HarmonyMethod oldMenu = new HarmonyMethod(GetType().GetMethod("oldMenu"));

            harmony.Patch(typeof(MenuOption).GetConstructors()[0], prefix: typeSize() > 3 ? newMenu : oldMenu);
            harmony.Patch(typeof(HoldMenuOption).GetConstructors()[0], prefix: oldMenu);
        }

        public static void oldMenu(ref string name, string msg, MenuOption.ActionType actionType)
        {
            name = menuLocal(name);
        }
        
        public static void newMenu(ref string name, string msg, MenuOption.ActionType actionType, bool trans)
        {
            name = menuLocal(name);
        }

        private static string menuLocal(string str)
        {
            if (str.LastIndexOf("localized") >= 0)
            {
                return str;
            }

            string s;
            if (str.IndexOf("<color=") < 0)
            {
                s = LoadLocalization.Instance.GetLocalizedString("menu-" + str);
            }
            else
            {
                int ccut = str.IndexOf(">") + 1;
                string text = str.Substring(ccut, str.Length - ccut - 8);
                s = str.Substring(0, ccut) + LoadLocalization.Instance.GetLocalizedString("menu-" + text) + "</color>";
            }

            return s;
        }

        private static int typeSize()
        {
            int size = 0;

            Type t = typeof(MenuOption);
            ConstructorInfo info = t.GetConstructors()[0];
            foreach (ParameterInfo pi in info.GetParameters())
            {
                size++;
            }

            return size;
        }

        public class MenuCompatible_Patch
        {

            public MenuCompatible_Patch(Harmony harmony)
            {
                if (typeof(WorldBoundaryTrigger).GetProperty("interactOptions") != null)
                {
                    harmony.Patch(typeof(WorldBoundaryTrigger).GetProperty("interactOptions").GetGetMethod(), postfix: new HarmonyMethod(GetType().GetMethod("WorldBoundaryTrigger_interactOptions_Patch")));
                }
                if (typeof(PlayerInventory).GetMethod("GetContextuaFabricatedInteractOptionsList") != null)
                {
                    harmony.Patch(typeof(PlayerInventory).GetMethod("GetContextuaFabricatedInteractOptionsList"), postfix: new HarmonyMethod(GetType().GetMethod("PlayerInventory_GetContextuaFabricatedInteractOptionsList_Patch")));
                }
            }

            [HarmonyPatch(typeof(DroidBrain), "interactOptions", MethodType.Getter)]
            class DroidBrain_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach(MenuOption mo in __result)
                    {
                        if (mo.menuName.IndexOf("Batteries)") >= 0)
                        {
                            string text = mo.menuName.Replace("menu-Hack (", "").Replace("Batteries)", "").Trim();
                            mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Hack ({0} Batteries)", text);
                        }
                        else if (mo.menuName.IndexOf("Replace (") >= 0)
                        {
                            string text = mo.menuName.Replace("menu-Replace (", "").Replace("Batteries)", "").Trim();
                            mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Replace ({0} Batteries)", text);
                        }
                        else if (mo.menuName.IndexOf("Batteries (") >= 0)
                        {
                            string str = mo.menuName.Replace("menu-Replace ", "").Replace("Batteries ", "").Replace(")", "").Trim();
                            string[] texts = str.Split('(');
                            mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Replace ({0} Batteries)", texts);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(Elevator), "interactOptions", MethodType.Getter)]
            class Elevator_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach (MenuOption mo in __result)
                    {
                        if (mo.menuName.IndexOf("Level ") >= 0)
                        {
                            string text = mo.menuName.Replace("menu-Level ", "").Trim();
                            mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Level {0}", text);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(FuelStation), "interactOptions", MethodType.Getter)]
            class FuelStation_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach (MenuOption mo in __result)
                    {
                        if (mo.menuName.IndexOf("Extract ") >= 0)
                        {
                            if (mo.menuName.IndexOf("<color=") < 0)
                            {
                                string text = mo.menuName.Replace("menu-Extract ", "").Replace("from station","").Trim();
                                mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Extract {0} from station", "<" + LoadLocalization.Instance.GetLocalizedString(text) + ">");
                            }
                            else
                            {
                                int ccut = mo.menuName.IndexOf(">") + 1;
                                string text = mo.menuName.Substring(ccut, mo.menuName.Length - ccut - 8).Replace("menu-Extract ", "").Replace("from station", "").Trim();
                                string color = mo.menuName.Substring(0, ccut);
                                mo.menuName = color + LoadLocalization.Instance.GetLocalizedString("menu-Extract {0} from station", "<" + LoadLocalization.Instance.GetLocalizedString(text) + ">") + "</color>";
                            }
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(Geyser), "interactOptions", MethodType.Getter)]
            class Geyser_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach (MenuOption mo in __result)
                    {
                        if (typeof(MenuOption).Equals(mo.GetType()))
                        {
                            if (mo.menuName.IndexOf("Requires") >= 0)
                            {
                                int ccut = mo.menuName.IndexOf(">") + 1;
                                string text = mo.menuName.Substring(ccut, mo.menuName.Length - ccut - 8).Replace("menu-Requires Barrel to Extract", "").Trim();
                                string color = mo.menuName.Substring(0, ccut);
                                mo.menuName = color + LoadLocalization.Instance.GetLocalizedString("menu-Requires Barrel to Extract {0}", "<" + text + ">") + "</color>";
                            }
                            else
                            {
                                int ccut = mo.menuName.IndexOf(">") + 1;
                                string text = mo.menuName.Substring(ccut, mo.menuName.Length - ccut - 8).Replace("menu-Extract ", "").Trim();
                                string color = mo.menuName.Substring(0, ccut);
                                mo.menuName = color + LoadLocalization.Instance.GetLocalizedString("menu-Extract {0}", "<" + text + ">") + "</color>";
                            }
                        }
                        else
                        {
                            string text = mo.menuName.Replace("menu-Extract ", "").Replace("From Geyser", "").Trim();
                            mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Extract {0} From Geyser", "<" + text + ">");
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(MissionSurviveWave), "interactOptions", MethodType.Getter)]
            class MissionSurviveWave_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach (MenuOption mo in __result)
                    {
                        mo.menuName = mo.menuName.Replace("menu-", "");
                    }
                }
            }

            [HarmonyPatch(typeof(PlantBinManager), "interactOptions", MethodType.Getter)]
            class PlantBinManager_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach(MenuOption mo in __result)
                    {
                        if (typeof(MenuOption).Equals(mo.GetType()))
                        {
                            string text = mo.menuName.Replace("menu-Interact with ", "");
                            mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Interact with {0}", text);
                        }
                        else
                        {
                            if (mo.menuName.IndexOf(":") < 0)
                            {
                                string text = mo.menuName.Replace("menu-Plant ", "");
                                mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Plant {0}", LoadLocalization.Instance.GetLocalizedString(text));
                            }
                            else
                            {
                                string text = mo.menuName.Replace("menu-Bin ", "");
                                string[] arrays = text.Split('\n');
                                string timefix = "";
                                if (arrays.Length > 2)
                                {
                                    if (arrays[2].IndexOf("size=") >= 0)
                                    {
                                        string soldsize = arrays[2].Substring(arrays[2].IndexOf("=") + 1, arrays[2].IndexOf(">") - 6);
                                        int ioldsize = 0;
                                        int.TryParse(soldsize, out ioldsize);
                                        timefix = ioldsize > 0 && ioldsize < 16 ? arrays[2].Replace(soldsize, "16") : arrays[2];
                                    }
                                }
                                mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Bin {0}{1}", arrays[0], LoadLocalization.Instance.GetLocalizedString(arrays[1]) + timefix);
                            }
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(PlayerInventory), "GetContextualHarvestInteractOptionsList", new Type[] { typeof(NatureFragmentType) })]
            class PlayerInventory_GetContextualHarvestInteractOptionsList_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach (MenuOption mo in __result)
                    {
                        if(mo.menuName.IndexOf("<color")<0 && mo.menuName.IndexOf("menu-Use ") >= 0)
                        {
                            string text = mo.menuName.Replace("menu-Use ", "").Trim();
                            mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Use {0}", text);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(Puddle), "interactOptions", MethodType.Getter)]
            class Puddle_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach (MenuOption mo in __result)
                    {
                        if (mo.menuName.IndexOf("Extract ") >= 0)
                        {
                            if (mo.menuName.IndexOf("<color=") < 0)
                            {
                                string text = mo.menuName.Replace("menu-Extract ", "").Trim();
                                mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Extract {0}", "<" + LoadLocalization.Instance.GetLocalizedString(text) + ">");
                            }
                            else
                            {
                                int ccut = mo.menuName.IndexOf(">") + 1;
                                string text = mo.menuName.Substring(ccut, mo.menuName.Length - ccut - 8).Replace("menu-Extract ", "").Trim();
                                string color = mo.menuName.Substring(0, ccut);
                                mo.menuName = color + LoadLocalization.Instance.GetLocalizedString("menu-Extract {0}", "<" + LoadLocalization.Instance.GetLocalizedString(text) + ">") + "</color>";
                            }
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(ShitToilet), "interactOptions", MethodType.Getter)]
            class ShitToilet_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach (MenuOption mo in __result)
                    {
                        if (mo.menuName.IndexOf("...") >= 0)
                        {
                            string text = mo.menuName.Replace("menu-Waiting", "").Replace("...", "").Trim();
                            mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Waiting {0}...", text);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(SpaceStationNavigationPanel), "interactOptions", MethodType.Getter)]
            class SpaceStationNavigationPanel_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach (MenuOption mo in __result)
                    {
                        string text = mo.menuName.Replace("menu-Travel to", "").Trim();
                        mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Travel to {0}", text);
                    }
                }
            }

            [HarmonyPatch(typeof(TotemPole), "GetPuzzleInteractOptions", new Type[] { typeof(List<MenuOption>) })]
            class TotemPole_interactOptions_Patch
            {
                public static void Prefix(ref List<MenuOption> options)
                {
                    foreach (MenuOption mo in options)
                    {
                        string text = mo.menuName.Replace("menu-Rotate Totem", "").Trim();
                        mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Rotate Totem {0}", text);
                    }
                }
            }

            [HarmonyPatch(typeof(VehicleManager), "interactOptions", MethodType.Getter)]
            class VehicleManager_interactOptions_Patch
            {
                public static void Postfix(ref List<MenuOption> __result)
                {
                    foreach(MenuOption mo in __result)
                    {
                        if(mo.menuName.IndexOf("Refuel Vehicle") >= 0)
                        {
                            if (mo.menuName.IndexOf("<color=") < 0)
                            {
                                string text = mo.menuName.Replace("menu-Refuel Vehicle With ", "");
                                mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Refuel Vehicle With {0}", "<" + LoadLocalization.Instance.GetLocalizedString(text) + ">");
                            }
                            else
                            {
                                int ccut = mo.menuName.IndexOf(">") + 1;
                                string text = mo.menuName.Substring(ccut, mo.menuName.Length - ccut - 8).Replace("menu-Refuel Vehicle With ", "");
                                string color = mo.menuName.Substring(0, ccut);
                                mo.menuName = color + LoadLocalization.Instance.GetLocalizedString("menu-Refuel Vehicle With {0}", "<" + LoadLocalization.Instance.GetLocalizedString(text) + ">") + "</color>";
                            }
                        }
                    }
                }
            }

            public static void PlayerInventory_GetContextuaFabricatedInteractOptionsList_Patch(ref List<MenuOption> __result)
            {
                foreach (MenuOption mo in __result)
                {
                    if (mo.menuName.IndexOf("menu-Use") >= 0)
                    {
                        string text = mo.menuName.Replace("menu-Use", "").Trim();
                        mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Use {0}", text);
                    }
                }
            }

            public static void WorldBoundaryTrigger_interactOptions_Patch(ref List<MenuOption> __result)
            {
                foreach (MenuOption mo in __result)
                {
                    if (mo.menuName.IndexOf("Travel to") >= 0)
                    {
                        string text = mo.menuName.Replace("menu-Travel to", "").Trim();
                        mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Travel to {0}", text);
                    }
                }
            }

        }

    }
}
