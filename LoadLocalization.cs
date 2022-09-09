using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Osiris_I18n
{

	public class LoadLocalization : MonoBehaviour
	{
		public delegate void OnLocalizationUpdate();

		private static LoadLocalization _instance;

		public OnLocalizationUpdate onLocalizationUpdate;

		public const string streamingAssetsFolderPath = "Localization";

		private string _loadedLanguageFilename = "";

		[NonSerialized]
		private Dictionary<string, string> loadedLanguage;

		public static LoadLocalization Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GameObject("LoadLocalization").AddComponent<LoadLocalization>();
				}
				return _instance;
			}
		}

		public static string DefaultLanguageFilename
		{
			get
			{
				string result = "en_default";
				switch (Application.systemLanguage)
				{
					case SystemLanguage.Chinese:
					case SystemLanguage.ChineseSimplified:
					case SystemLanguage.ChineseTraditional:
						result = "zh";
						break;
					case SystemLanguage.English:
						result = "en_default";
						break;
					case SystemLanguage.French:
						result = "fr";
						break;
					case SystemLanguage.German:
						result = "de";
						break;
					case SystemLanguage.Italian:
						result = "it";
						break;
					case SystemLanguage.Japanese:
						result = "ja";
						break;
					case SystemLanguage.Russian:
						result = "ru";
						break;
					case SystemLanguage.Spanish:
						result = "es";
						break;
				}
				return result;
			}
		}

		public string LoadedLanguageFilename
		{
			get
			{
				return _loadedLanguageFilename;
			}
			set
			{
				if (value != _loadedLanguageFilename)
				{
					LoadLanguage(_loadedLanguageFilename);
				}
			}
		}

		private void Start()
		{
			if (string.IsNullOrEmpty(LoadedLanguageFilename))
			{
				Init();
			}
		}

		private void Init()
		{
			string @string = PlayerPrefs.GetString("SelectedLocalizationFile", DefaultLanguageFilename);
			LoadLanguage(@string);
		}

		public bool LoadLanguage(string fileNameNoExtension)
		{
			bool flag = true;
			try
			{
				loadedLanguage = LoadFromCSV(Path.Combine("Localization", fileNameNoExtension));
			}
			catch (Exception ex)
			{
				Debug.LogError("Error Loading Localization CSV:" + ex.Message);
				flag = false;
			}
			if (flag)
			{
				PlayerPrefs.SetString("SelectedLocalizationFile", fileNameNoExtension);
				_loadedLanguageFilename = fileNameNoExtension;
				if (onLocalizationUpdate != null)
				{
					onLocalizationUpdate();
				}
				Debug.Log("Successfully loaded language " + fileNameNoExtension);
			}
			return flag;
		}

		public Dictionary<string, string> getloadedLanguage()
		{
			return loadedLanguage;
		}

		public string GetLocalizedString(string englishString, params object[] other)
		{
			return string.Format(GetLocalizedString(englishString), other);
		}

		public string GetLocalizedString(string englishString)
		{
			if (loadedLanguage == null || string.IsNullOrEmpty(LoadedLanguageFilename))
			{
				Init();
			}
			if (LoadedLanguageFilename == "en_default")
			{
				return englishString;
			}
			string key = englishString.Replace("\r", "");
			if (loadedLanguage.ContainsKey(key))
			{
				return loadedLanguage[key];
			}
			/*System.IO.FileStream fs = new System.IO.FileStream("C:\\en.txt", System.IO.FileMode.Append);
			byte[] db = System.Text.Encoding.Default.GetBytes("\"" +englishString +"\"\n");
			fs.Write(db, 0, db.Length);
			fs.Flush();
			fs.Close();*/
			return englishString;
		}

		public static Dictionary<string, string> LoadFromCSV(string streamingAssetsRelativeFilePath)
		{
			string text;
			if(File.Exists(streamingAssetsRelativeFilePath) || File.Exists(streamingAssetsRelativeFilePath + ".csv"))
            {
				text = File.ReadAllText(streamingAssetsRelativeFilePath);
			}
            else
            {
				text = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, streamingAssetsRelativeFilePath + ".csv"));
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string com;
			foreach (string s in text.Split(';'))
			{
				com = s.Replace("\r", "");
				int cut = com.IndexOf("\",\"");
				int node = com.IndexOf('"');
				if (cut - node - 2 < 0)
				{
					Debug.LogError("CSVError-substring1:" + cut + "," + node + ", " + s);
					continue;
				}
				string key = com.Substring(node + 1, cut - node - 1);
				node = com.LastIndexOf('"') - cut - 3;
				if (node - 1 < 0)
				{
					Debug.LogError("CSVError-substring2:" + cut + 3 + "," + node + ", " + s);
				}
				else
				{
					string value = com.Substring(cut + 3).Substring(0, node);
					key = key.Replace("<semicolon>", ";");
					value = value.Replace("<semicolon>", ";");
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, value);
					}
					else
					{
						Debug.LogWarning("Found Duplicate key while loading CSV! " + key + " with value: " + value);
					}
				}
			}
			return dictionary;
		}
	}

}