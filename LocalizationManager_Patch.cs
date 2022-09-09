using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Osiris_I18n
{
    class LocalizationManager_Patch
    {

        private static bool mainmenu_addbutton = true;
        private static bool setoriginalsize = true;
        private static int originalsize;

        [HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.LoadLanguage))]
        class LocalizationManager_LoadLanguage_Patch
        {
            public static void Postfix(LocalizationManager __instance, ref bool __result, string fileNameNoExtension)
            {
                bool flag = LoadLocalization.Instance.LoadLanguage(fileNameNoExtension);

                Dictionary<string, string> loadedLanguage = Traverse.Create(__instance).Field("loadedLanguage").GetValue<Dictionary<string, string>>();
                loadedLanguage = LoadLocalization.Instance.getloadedLanguage();
                Traverse.Create(__instance).Field("loadedLanguage").SetValue(loadedLanguage);
                if (__instance.onLocalizationUpdate != null)
                {
                    __instance.onLocalizationUpdate();
                }

                string _fileNameNoExtension = fileNameNoExtension.IndexOf(".csv") >= 0 ? fileNameNoExtension.Replace(".csv", "") : fileNameNoExtension;
                PlayerPrefs.SetString("SelectedLocalizationFile", _fileNameNoExtension);
                Traverse.Create(__instance).Field("_loadedLanguageFilename").SetValue(_fileNameNoExtension);

                __result = flag;
            }
        }

        [HarmonyPatch(typeof(LocalizationCSV), nameof(LocalizationCSV.LoadFromCSV))]
        class LocalizationCSV_LoadFromCSV_Patch
        {
            public static bool Prefix(ref Dictionary<string, string> __result, string streamingAssetsRelativeFilePath)
            {
                __result = LoadLocalization.LoadFromCSV(streamingAssetsRelativeFilePath);
                return false;
            }
        }

        [HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.GetLocalizedString))]
        class LocalizationManager_GetLocalizedString_Patch
        {
            public static bool Prefix(ref string __result, string englishString)
            {
                __result = LoadLocalization.Instance.GetLocalizedString(englishString);
                return false;
            }
        }

        [HarmonyPatch(typeof(MainMenu), "BringInOptions")]
        class MainMenu_AddLanguageButton_Patch
        {
            public static void Postfix(MainMenu __instance)
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

                        foreach (string[] str in I18n.refreshI18ncsvs())
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
        }

        [HarmonyPatch(typeof(LanguagePanel), "FindLanguageFiles")]
        class LanguagePanel_AddLanguageButton_Patch
        {
            public static void Postfix(LanguagePanel __instance)
            {
                if (!mainmenu_addbutton)
                {
                    List<LanguageButton> spawnedLanguageButtons = Traverse.Create(__instance).Field("spawnedLanguageButtons").GetValue<List<LanguageButton>>();

                    foreach (string[] str in I18n.refreshI18ncsvs())
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
}
