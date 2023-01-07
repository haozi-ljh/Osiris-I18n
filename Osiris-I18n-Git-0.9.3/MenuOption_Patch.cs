using System;
using System.Collections.Generic;
using System.Reflection;

namespace Osiris_I18n
{
    public class MenuOption_Patch
    {

        public MenuOption_Patch()
        {
            MethodInfo newMenu = GetType().GetMethod("newMenu");
            MethodInfo oldMenu = GetType().GetMethod("oldMenu");
            PatcherManager.Add(new Patcher(typeof(MenuOption), null, PatchType.prefix, typeSize() > 3 ? 0 : 1, newMenu, oldMenu));
            PatcherManager.Add(new Patcher(typeof(HoldMenuOption), null, PatchType.prefix, oldMenu));
            PatcherManager.Add(new Patcher(Patcher.Find("BioDomeBin", "interactOptions", pt: PropertyType.get), PatchType.postfix, GetType().GetMethod("BioDomeBin_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(DroidBrain), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("DroidBrain_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(Elevator), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("Elevator_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(FuelStation), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("FuelStation_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(Geyser), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("Geyser_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(MissionSurviveWave), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("MissionSurviveWave_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlantBinManager), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("PlantBinManager_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerInventory), "GetContextualHarvestInteractOptionsList", new Type[] { typeof(NatureFragmentType) }, PatchType.postfix, GetType().GetMethod("PlayerInventory_GetContextualHarvestInteractOptionsList_Patch")));
            PatcherManager.Add(new Patcher(typeof(Puddle), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("Puddle_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(ShitToilet), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("ShitToilet_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(SpaceStationNavigationPanel), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("SpaceStationNavigationPanel_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(TotemPole), "GetPuzzleInteractOptions", new Type[] { typeof(List<MenuOption>) }, PatchType.prefix, GetType().GetMethod("TotemPole_GetPuzzleInteractOptions_Patch")));
            PatcherManager.Add(new Patcher(typeof(VehicleManager), "interactOptions", PropertyType.get, PatchType.postfix, GetType().GetMethod("VehicleManager_interactOptions_Patch")));

            PatcherManager.Add(new Patcher(Patcher.Find(typeof(WorldBoundaryTrigger), "interactOptions", pt: PropertyType.get), PatchType.postfix, GetType().GetMethod("WorldBoundaryTrigger_interactOptions_Patch")));
            PatcherManager.Add(new Patcher(Patcher.Find(typeof(PlayerInventory), "GetContextuaFabricatedInteractOptionsList"), PatchType.postfix, GetType().GetMethod("PlayerInventory_GetContextuaFabricatedInteractOptionsList_Patch")));
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
            string s;
            if (!str.Contains("<color="))
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



        public static void BioDomeBin_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if ((mo.menuName.Contains("Plant") && !mo.menuName.Contains("Seed")))
                {
                    string text = mo.menuName.Replace("menu-Plant", "").Trim();
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-new_Plant {0}", LoadLocalization.Instance.GetLocalizedString(text));
                }
                else if (mo.menuName.Contains("Pick"))
                {
                    string text = mo.menuName.Replace("menu-Pick", "").Trim();
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Pick {0}", LoadLocalization.Instance.GetLocalizedString(text));
                }
            }
        }

        public static void DroidBrain_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (mo.menuName.Contains("Batteries)"))
                {
                    string text = mo.menuName.Replace("menu-Hack (", "").Replace("Batteries)", "").Trim();
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Hack ({0} Batteries)", text);
                }
                else if (mo.menuName.Contains("Replace ("))
                {
                    string text = mo.menuName.Replace("menu-Replace (", "").Replace("Batteries)", "").Trim();
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Replace ({0} Batteries)", text);
                }
                else if (mo.menuName.Contains("Batteries ("))
                {
                    string str = mo.menuName.Replace("menu-Replace ", "").Replace("Batteries ", "").Replace(")", "").Trim();
                    string[] texts = str.Split('(');
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Replace ({0} Batteries)", texts);
                }
            }
        }

        public static void Elevator_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (mo.menuName.Contains("Level "))
                {
                    string text = mo.menuName.Replace("menu-Level ", "").Trim();
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Level {0}", text);
                }
            }
        }

        public static void FuelStation_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (mo.menuName.Contains("Extract "))
                {
                    if (!mo.menuName.Contains("<color="))
                    {
                        string text = mo.menuName.Replace("menu-Extract ", "").Replace("from station", "").Trim();
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

        public static void Geyser_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (typeof(MenuOption).Equals(mo.GetType()))
                {
                    if (mo.menuName.Contains("Requires"))
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

        public static void MissionSurviveWave_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                mo.menuName = mo.menuName.Replace("menu-", "");
            }
        }

        public static void PlantBinManager_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (typeof(MenuOption).Equals(mo.GetType()))
                {
                    string text = mo.menuName.Replace("menu-Interact with ", "");
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Interact with {0}", text);
                }
                else
                {
                    if (!mo.menuName.Contains(":"))
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
                            if (arrays[2].Contains("size="))
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

        public static void PlayerInventory_GetContextualHarvestInteractOptionsList_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (!mo.menuName.Contains("<color") && mo.menuName.Contains("menu-Use "))
                {
                    string text = mo.menuName.Replace("menu-Use ", "").Trim();
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Use {0}", text);
                }
            }
        }

        public static void Puddle_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (mo.menuName.Contains("Extract "))
                {
                    if (!mo.menuName.Contains("<color="))
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

        public static void ShitToilet_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (mo.menuName.Contains("..."))
                {
                    string text = mo.menuName.Replace("menu-Waiting", "").Replace("...", "").Trim();
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Waiting {0}...", text);
                }
            }
        }

        public static void SpaceStationNavigationPanel_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                string text = mo.menuName.Replace("menu-Travel to", "").Trim();
                mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Travel to {0}", text);
            }
        }

        public static void TotemPole_GetPuzzleInteractOptions_Patch(ref List<MenuOption> options)
        {
            foreach (MenuOption mo in options)
            {
                string text = mo.menuName.Replace("menu-Rotate Totem", "").Trim();
                mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Rotate Totem {0}", text);
            }
        }

        public static void VehicleManager_interactOptions_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (mo.menuName.Contains("Refuel Vehicle"))
                {
                    if (!mo.menuName.Contains("<color="))
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

        public static void PlayerInventory_GetContextuaFabricatedInteractOptionsList_Patch(ref List<MenuOption> __result)
        {
            foreach (MenuOption mo in __result)
            {
                if (mo.menuName.Contains("menu-Use"))
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
                if (mo.menuName.Contains("Travel to"))
                {
                    string text = mo.menuName.Replace("menu-Travel to", "").Trim();
                    mo.menuName = LoadLocalization.Instance.GetLocalizedString("menu-Travel to {0}", text);
                }
            }
        }

    }
}
