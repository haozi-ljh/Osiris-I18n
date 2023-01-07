using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Osiris_I18n
{
    public class MainMenu_Patch
    {

		private static UnityAction<Scene, LoadSceneMode> localizationFirstScene = delegate (Scene scene, LoadSceneMode mode)
		{
			if (scene.name.Contains("Intro_Cinematic"))
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

		public MainMenu_Patch()
        {
			PatcherManager.Add(new Patcher(typeof(MainMenu), "JoinServer", PatchType.postfix, GetType().GetMethod("MainMenu_JoinServer_Patch")));
			PatcherManager.Add(new Patcher(typeof(MainMenu), "LoadFirstScene", PatchType.prefix, GetType().GetMethod("MainMenu_LoadFirstScene_Patch")));
		}

		public static void MainMenu_JoinServer_Patch(MainMenu __instance)
		{
			__instance.launchButtonText.text = LoadLocalization.Instance.GetLocalizedString(__instance.launchButtonText.text);
		}

		public static void MainMenu_LoadFirstScene_Patch()
		{
			SceneManager.sceneLoaded += localizationFirstScene;
		}

	}
}
