using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Osiris_I18n
{
    class MainMenu_Patch
    {

		[HarmonyPatch(typeof(MainMenu), "JoinServer")]
		class MainMenu_JoinServer_Patch
        {
			public static void Postfix(MainMenu __instance)
            {
				__instance.launchButtonText.text = LoadLocalization.Instance.GetLocalizedString(__instance.launchButtonText.text);
            }
        }

		[HarmonyPatch(typeof(MainMenu), "LoadFirstScene")]
		class MainMenu_LoadFirstScene_Patch
		{
			static UnityAction<Scene, LoadSceneMode> localizationFirstScene = delegate (Scene scene, LoadSceneMode mode)
			{
				if (scene.name.IndexOf("Intro_Cinematic") >= 0)
				{
					GameObject[] g = scene.GetRootGameObjects();
					for (int i = 0; i < g.Length; i++)
					{
						MonoBehaviour[] m = g[i].GetComponentsInChildren<MonoBehaviour>();
						for (int ii = 0; ii < m.Length; ii++)
						{
							Text[] t = m[ii].GetComponentsInChildren<Text>();
							for (int iii = 0; iii < t.Length; iii++)
							{
								t[iii].fontSize = 35;
								t[iii].text = LoadLocalization.Instance.GetLocalizedString(t[iii].text);
							}
						}
					}
				}
				SceneManager.sceneLoaded -= localizationFirstScene;
			};

			public static void Prefix()
			{
				SceneManager.sceneLoaded += localizationFirstScene;
			}
		}

	}
}
