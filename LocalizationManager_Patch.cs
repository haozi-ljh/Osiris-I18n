using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Osiris_I18n
{
    public class LocalizationManager_Patch
    {

        private static bool mainmenu_addbutton = true;
        private static bool setoriginalsize = true;
        private static int originalsize;

        public LocalizationManager_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(LocalizationManager), "LoadLanguage", PatchType.postfix, GetType().GetMethod("LocalizationManager_LoadLanguage_Patch")));
            PatcherManager.Add(new Patcher(typeof(LocalizationCSV), "LoadFromCSV", PatchType.prefix, GetType().GetMethod("LocalizationCSV_LoadFromCSV_Patch")));
            PatcherManager.Add(new Patcher(typeof(LocalizationManager), "GetLocalizedString", PatchType.prefix, GetType().GetMethod("LocalizationManager_GetLocalizedString_Patch")));
            PatcherManager.Add(new Patcher(typeof(MainMenu), "BringInOptions", PatchType.postfix, GetType().GetMethod("MainMenu_AddLanguageButton_Patch")));
            PatcherManager.Add(new Patcher(typeof(LanguagePanel), "FindLanguageFiles", PatchType.postfix, GetType().GetMethod("LanguagePanel_AddLanguageButton_Patch")));
        }

        public static void LocalizationManager_LoadLanguage_Patch(LocalizationManager __instance, ref bool __result, string fileNameNoExtension)
        {
            bool flag = LoadLocalization.Instance.LoadLanguage(fileNameNoExtension);

            if (flag)
            {
                Dictionary<string, string> loadedLanguage = Traverse.Create(__instance).Field("loadedLanguage").GetValue<Dictionary<string, string>>();
                loadedLanguage = LoadLocalization.Instance.GetLoadedLanguage();
                Traverse.Create(__instance).Field("loadedLanguage").SetValue(loadedLanguage);
                if (__instance.onLocalizationUpdate != null)
                {
                    __instance.onLocalizationUpdate();
                }

                string _fileNameNoExtension = fileNameNoExtension.LastIndexOf(".csv") == fileNameNoExtension.Length - 4 ? fileNameNoExtension.Remove(fileNameNoExtension.Length - 4) : fileNameNoExtension;
                PlayerPrefs.SetString("SelectedLocalizationFile", _fileNameNoExtension);
                Traverse.Create(__instance).Field("_loadedLanguageFilename").SetValue(_fileNameNoExtension);
            }

            __result = flag;
        }

        public static bool LocalizationCSV_LoadFromCSV_Patch(ref Dictionary<string, string> __result, string streamingAssetsRelativeFilePath)
        {
            __result = LoadLocalization.LoadFromCSV(streamingAssetsRelativeFilePath);
            return false;
        }

        public static bool LocalizationManager_GetLocalizedString_Patch(ref string __result, string englishString)
        {
            __result = LoadLocalization.Instance.GetLocalizedString(englishString);
            return false;
        }

        public static void MainMenu_AddLanguageButton_Patch(MainMenu __instance)
        {
            if (mainmenu_addbutton)
            {
                ScrollRect[] sr = __instance.gameObject.GetComponentsInChildren<ScrollRect>();
                if (sr.Length == 1)
                {
                    LanguageButton[] lbs = sr[0].GetComponentsInChildren<LanguageButton>();
                    LanguageButton template = lbs[0];
                    if (setoriginalsize)
                    {
                        originalsize = lbs.Length;
                        setoriginalsize = false;
                    }
                    int resetsize = lbs.Length - originalsize;

                    foreach (string[] str in I18n.RefreshI18nCSVs())
                    {
                        if (resetsize > 0)
                        {
                            lbs[lbs.Length - resetsize].languageLabel.text = str[0];
                            lbs[lbs.Length - resetsize].languageLabel.color = Color.yellow;
                            lbs[lbs.Length - resetsize].languageFileName = str[1];
                            resetsize--;
                        }
                        else
                        {
                            LanguageButton lb = Object.Instantiate(template, sr[0].content.transform);
                            lb.languageLabel.text = str[0];
                            lb.languageLabel.color = Color.yellow;
                            lb.languageFileName = str[1];
                        }
                    }
                }
                else
                {
                    mainmenu_addbutton = false;
                }
            }
        }

        public static void LanguagePanel_AddLanguageButton_Patch(LanguagePanel __instance)
        {
            if (!mainmenu_addbutton)
            {
                List<LanguageButton> spawnedLanguageButtons = Traverse.Create(__instance).Field("spawnedLanguageButtons").GetValue<List<LanguageButton>>();

                foreach (string[] str in I18n.RefreshI18nCSVs())
                {
                    LanguageButton lb = Object.Instantiate(__instance.languagebuttonPrefab.gameObject, __instance.languagebuttonParent).GetComponent<LanguageButton>();
                    lb.languageLabel.text = str[0];
                    lb.languageLabel.color = Color.yellow;
                    lb.languageFileName = str[1];
                    spawnedLanguageButtons.Add(lb);
                }

                Traverse.Create(__instance).Field("spawnedLanguageButtons").SetValue(spawnedLanguageButtons);
            }
        }

    }
}
