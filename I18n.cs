using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Osiris_I18n
{

    [BepInPlugin("I18n.OsirisNewDawn.Localization.plugin", "Osiris: New Dawn 多国语言翻译", "0.9.0")]
    public class I18n : BaseUnityPlugin
    {

        float textIndex;

        ConfigEntry<bool> oninitConfig;
        ConfigEntry<string> i18ncsvPathConfig;
        ConfigEntry<bool> onalterConfig;
        ConfigEntry<bool> onoptimizeConfig;

        public static bool onconfigGUI = false;
        private int configGUIstate = 0;
        private Vector2 selectcsvscroll;
        public static bool oninit = false;
        public static string i18ncsvPath = "";
        private static List<string[]> i18ncsvs = new List<string[]>();
        public static bool onalter = false;
        public static bool onoptimize = false;

        private int windowWidth;
        private int windowHeight;
        private int configGUIWidth;
        private int configGUIHeight;
        
        private Alter alter;
        private Optimize optimize;

        void Start()
        {
            oninitConfig = Config.Bind<bool>("config", "初始弹窗", true, "\n第一次使用补丁时的弹窗\n");
            i18ncsvPathConfig = Config.Bind<string>("config", "路径", Path.GetDirectoryName(GetType().Assembly.Location) + "\\lang", "\n自定义翻译文件路径(默认为插件所在目录), 补丁会扫描该路径下所有符合格式的翻译文件\n翻译文件名称只有结尾必须为\"_I18n.csv\",推荐格式: \"地区语言名_I18n.csv\"\n输入 <gamepath> 快捷设置路径为游戏默认翻译文件路径\n");
            onalterConfig = Config.Bind<bool>("config", "魔改开关", false, "\n修改部分UI显示效果\n");
            onoptimizeConfig = Config.Bind<bool>("config", "优化开关", false, "\n在操作飞船时增加了提示UI\n");

            oninit = oninitConfig.Value;
            i18ncsvPath = i18ncsvPathConfig.Value;
            onalter = onalterConfig.Value;
            onoptimize = onoptimizeConfig.Value;

            if (!Directory.Exists(i18ncsvPath))
            {
                i18ncsvPathConfig.Value = i18ncsvPath = (string)i18ncsvPathConfig.DefaultValue;
                Config.Save();
                if (!Directory.Exists(i18ncsvPath))
                {
                    new DirectoryInfo(i18ncsvPath).Create();
                }
            }

            refreshI18ncsvs();

            windowWidth = Screen.width;
            windowHeight = Screen.height;
            configGUIWidth = windowWidth / 3 > 500 ? windowWidth / 3 : 500;
            configGUIHeight = windowHeight / 3 > 300 ? windowHeight / 3 : 300;

            Harmony harmony = new Harmony("I18n.OsirisNewDawn.Localization.plugin");
            harmony.PatchAll();

            new MenuOption_Patch(harmony);
            new TargetHUD_Patch(harmony);
            new MenuOption_Patch.MenuCompatible_Patch(harmony);
            new Compatible(harmony);
            alter = new Alter(harmony);
            optimize = new Optimize(harmony);

            if (onalter)
                alter.Patch();
            if (onoptimize)
                optimize.Patch();

            if (oninit)
            {
                oninitConfig.Value = false;
                Config.Save();
                Action opengui = delegate
                {
                    onconfigGUI = true;
                };
                Action action = delegate
                {
                    MainMenu.Me.confirmBoxManager.Activate("是否打开插件设置", opengui, null, "确定", "取消");
                };
                MainMenu.Me.confirmBoxManager.Activate("由 <color=yellow>高阶领主喵拉纳克 , ljh_haozi , 席兰雅</color> 共同汉化", action, action, "确定", "取消");
            }
        }

        private void Update()
        {
            if(new KeyboardShortcut(KeyCode.Escape).IsDown())
            {
                if (configGUIstate == 0)
                {
                    onconfigGUI = false;
                    cancelConfig();
                }
                else
                {
                    configGUIstate = 0;
                }
            }
        }

        private void OnGUI()
        {
            if (onconfigGUI)
            {
                Rect configwindows = new Rect(windowWidth / 2 - configGUIWidth / 2, windowHeight / 2 - configGUIHeight / 2, configGUIWidth, configGUIHeight);
                configwindows = GUI.Window(0, configwindows, ConfigWindow, "Osiris: New Dawn 多国语言翻译插件配置");
            }
        }

        private void ConfigWindow(int winid)
        {
            switch (configGUIstate)
            {
                case 0:
                    mainWindow(winid);
                    break;
                case 1:
                    selectcsvWindow(winid);
                    break;
                case 2:
                    helpWindow(winid);
                    break;
                default:
                    break;
            }
        }

        private void mainWindow(int winid)
        {
            GUILayout.Label("翻译文件路径(默认为插件所在目录): ");
            GUIStyle pathstyle = GUI.skin.box;
            pathstyle.alignment = TextAnchor.MiddleLeft;
            pathstyle.wordWrap = false;

            float textwidth = pathstyle.CalcSize(new GUIContent(i18ncsvPath)).x;
            float showwidth = configGUIWidth - 20;
            string text = i18ncsvPath.Substring((int)(textIndex / textwidth * 100));

            GUILayout.Box(text, pathstyle, GUILayout.MinHeight(30));
            if (showwidth < textwidth)
            {
                textIndex = GUILayout.HorizontalScrollbar(textIndex, showwidth, 0, textwidth);
            }
            else
            {
                textIndex = 0;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打开翻译文件夹", GUILayout.MinHeight(30)))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/root," + i18ncsvPath);
            }
            if (GUILayout.Button("...", GUILayout.MinHeight(30)))
            {
                OpenDialogDir.showDialog("请选择存放有翻译文件的文件夹\n\n插件可读取翻译文件的文件名结尾必须为\"_I18n.csv(区分大小写)\"", ref i18ncsvPath);
            }
            if (GUILayout.Button("切换翻译", GUILayout.MinHeight(30)))
            {
                configGUIstate = 1;
            }
            if (GUILayout.Button("插件帮助", GUILayout.MinHeight(30)))
            {
                configGUIstate = 2;
                //MainMenu.Me.confirmBoxManager.Activate("本插件会自动读取路径下所有后缀为<color=yellow>_I18n.csv</color>的文件，读取到的所有文件都会集成在游戏的语言设置处<color=yellow>高亮显示</color>\n\n是否立即打开设置", () => { MainMenu.Me.BringInOptions(); }, () => { onconfigGUI = true; }, "确定", "取消");
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("魔改开关: " + (onalter ? "开启" : "关闭"), GUILayout.MinHeight(30)))
            {
                onalter = !onalter;
            }
            if (GUILayout.Button("优化开关: " + (onoptimize ? "开启" : "关闭"), GUILayout.MinHeight(30)))
            {
                onoptimize = !onoptimize;
            }

            GUILayout.Label("", GUILayout.MaxHeight(configGUIHeight - 180));

            if (GUILayout.Button("默认设置", GUILayout.MinHeight(30)))
            {
                defConfig();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("保存", GUILayout.MinHeight(30)))
            {
                saveConfig();
                onconfigGUI = false;
            }
            if (GUILayout.Button("取消", GUILayout.MinHeight(30)))
            {
                cancelConfig();
                onconfigGUI = false;
            }
            GUILayout.EndHorizontal();
        }

        private void helpWindow(int winid)
        {
            string helptext = "插件是怎么读取翻译文件的\n\n是根据特定的文件名读取翻译文件的, 插件会读取翻译文件路径下的所有文件, 其中文件名结尾为<color=yellow>_I18n.csv</color>(区分大小写)的文件能被插件识别\n\n\n我该怎么切换翻译\n\n你可以在插件配置中切换, 或者通过插件集成在游戏的语言设置处<color=yellow>高亮显示</color>的按钮切换";
            GUIStyle helpstyle = GUI.skin.box;
            helpstyle.wordWrap = true;
            GUILayout.Box(helptext, helpstyle, GUILayout.MaxHeight(configGUIHeight - 60));
            if (GUILayout.Button("确定", GUILayout.MinHeight(30)))
            {
                configGUIstate = 0;
            }
        }

        private void selectcsvWindow(int winid)
        {
            selectcsvscroll = GUILayout.BeginScrollView(selectcsvscroll, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.box, GUILayout.MaxHeight(configGUIHeight - 60));
            for (int i = 0; i < refreshI18ncsvs().Count; i++)
            {
                if (GUILayout.Button(i18ncsvs[i][0], GUILayout.MinHeight(30)))
                {
                    LocalizationManager.Instance.LoadLanguage(i18ncsvs[i][1]);
                }
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("返回", GUILayout.MinHeight(30)))
            {
                configGUIstate = 0;
            }
        }

        public static List<string[]> refreshI18ncsvs()
        {
            if (Directory.Exists(i18ncsvPath))
            {
                i18ncsvs.Clear();
                FileSystemInfo[] fsis = new DirectoryInfo(i18ncsvPath).GetFileSystemInfos();
                foreach (FileSystemInfo fsi in fsis)
                {
                    if (fsi.Name.IndexOf("_I18n.csv") >= 0)
                    {
                        if (fsi.Name.Equals("_I18n.csv"))
                        {
                            i18ncsvs.Add(new string[] { fsi.Name, fsi.FullName });
                        }
                        else if (fsi.Name.Substring(fsi.Name.Length - 9).Equals("_I18n.csv"))
                        {
                            i18ncsvs.Add(new string[] { fsi.Name.Substring(0, fsi.Name.Length - 9), fsi.FullName });
                        }
                    }
                }
            }
            else
            {
                i18ncsvs.Clear();
            }
            return i18ncsvs;
        }

        private void defConfig()
        {
            i18ncsvPathConfig.Value = (string)i18ncsvPathConfig.DefaultValue;
            onalterConfig.Value = (bool)onalterConfig.DefaultValue;
            onoptimizeConfig.Value = (bool)onoptimizeConfig.DefaultValue;
            
            Config.Save();
            Config.Reload();

            i18ncsvPath = i18ncsvPathConfig.Value;
            onalter = onalterConfig.Value;
            onoptimize = onoptimizeConfig.Value;

            alter.Unpatch();
            optimize.Unpatch();
        }

        private void saveConfig()
        {
            i18ncsvPathConfig.Value = i18ncsvPath;
            onalterConfig.Value = onalter;
            onoptimizeConfig.Value = onoptimize;
            
            if (onalter)
                alter.Patch();
            else
                alter.Unpatch();
            if (onoptimize)
                optimize.Patch();
            else
                optimize.Unpatch();

            Config.Save();
            Config.Reload();
        }
        
        private void cancelConfig()
        {
            i18ncsvPath = i18ncsvPathConfig.Value;
            onalter = onalterConfig.Value;
            onoptimize = onoptimizeConfig.Value;
        }

        protected class AddConfigButton
        {

            private static Button configbutton;

            [HarmonyPatch(typeof(MainMenu), "Start")]
            class MainMenu_Start_Patch
            {
                public static void Postfix(MainMenu __instance)
                {
                    Button[] bs = __instance.MainPanel.GetComponentsInChildren<Button>();
                    Transform tf = bs[0].transform.parent;
                    configbutton = Instantiate(bs[0], tf);
                    configbutton.name = "Button Osiris-I18n_Option";
                    if (bs[0].transform.localPosition.x == bs[1].transform.localPosition.x)
                    {
                        RectTransform rtf = tf.GetComponent<RectTransform>();
                        rtf.sizeDelta = new Vector2(rtf.sizeDelta.x, rtf.sizeDelta.y + configbutton.GetComponent<RectTransform>().sizeDelta.y);
                    }
                    if (bs[0].transform.localPosition.y == bs[1].transform.localPosition.y)
                    {
                        RectTransform rtf = tf.GetComponent<RectTransform>();
                        rtf.sizeDelta = new Vector2(rtf.sizeDelta.x + configbutton.GetComponent<RectTransform>().sizeDelta.x, configbutton.GetComponent<RectTransform>().sizeDelta.y);
                    }
                    for (int i = 0; i < configbutton.onClick.GetPersistentEventCount(); i++)
                    {
                        configbutton.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
                    }
                    configbutton.onClick.AddListener(() => { onconfigGUI = true; });
                }
            }
            
            [HarmonyPatch(typeof(MainMenu), "Update")]
            class MainMenu_Update_Patch
            {
                public static void Postfix()
                {
                    Text cbt = configbutton.GetComponentInChildren<Text>();
                    cbt.text = "插件设置";
                    cbt.color = Color.yellow;
                }
            }

            [HarmonyPatch(typeof(PlayerGUI), "Start")]
            class PlayerGUI_Start_Patch
            {
                public static void Postfix(PlayerGUI __instance)
                {
                    Button[] bs = __instance.panelOptions.GetComponentsInChildren<Button>();
                    Transform tf = bs[bs.Length - 1].transform.parent;
                    configbutton = Instantiate(bs[0], tf);
                    configbutton.name = "Button Osiris-I18n_Option";
                    if (bs[0].transform.localPosition.x == bs[1].transform.localPosition.x)
                    {
                        RectTransform rtf = tf.GetComponent<RectTransform>();
                        rtf.sizeDelta = new Vector2(rtf.sizeDelta.x, rtf.sizeDelta.y + configbutton.GetComponent<RectTransform>().sizeDelta.y);
                    }
                    if (bs[0].transform.localPosition.y == bs[1].transform.localPosition.y)
                    {
                        RectTransform rtf = tf.GetComponent<RectTransform>();
                        rtf.sizeDelta = new Vector2(rtf.sizeDelta.x + configbutton.GetComponent<RectTransform>().sizeDelta.x, configbutton.GetComponent<RectTransform>().sizeDelta.y);
                    }
                    Text cbt = configbutton.GetComponentInChildren<Text>();
                    cbt.text = "插件设置";
                    cbt.color = Color.yellow;
                    for (int i = 0; i < configbutton.onClick.GetPersistentEventCount(); i++)
                    {
                        configbutton.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
                    }
                    configbutton.onClick.AddListener(() => { onconfigGUI = true; });
                }
            }

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        protected class OpenDialogDir
        {
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr pidlRoot = IntPtr.Zero;
            public String pszDisplayName = "";
            public String lpszTitle = null;
            public UInt32 ulFlags = 0;
            public IntPtr lpfn = IntPtr.Zero;
            public IntPtr lParam = IntPtr.Zero;
            public int iImage = 0;

            private delegate int BrowseCallbackProc(IntPtr hwnd, UInt32 uMsg, IntPtr lParam, IntPtr lpData);
            private static BrowseCallbackProc bcp = new BrowseCallbackProc(BrowseCallback);

            public static void showDialog(string lpszTitle, ref string path)
            {
                OpenDialogDir langdialog = new OpenDialogDir();
                langdialog.pszDisplayName = string.Empty;
                langdialog.lpszTitle = lpszTitle;
                langdialog.pidlRoot = IntPtr.Zero;
                langdialog.lParam = Marshal.StringToHGlobalUni(path);
                langdialog.lpfn = Marshal.GetFunctionPointerForDelegate(bcp);
                langdialog.ulFlags = 0x00000040 | 0x00000002 | 0x00000001;
                IntPtr pidl = DLLWin32.SHBrowseForFolder(langdialog);

                char[] getpath = new char[2048];
                if (DLLWin32.SHGetPathFromIDList(pidl, getpath))
                {
                    string str = new string(getpath);
                    path = str.Remove(str.IndexOf((new char[1])[0]));
                }
            }

            private static int BrowseCallback(IntPtr hwnd, UInt32 uMsg, IntPtr lParam, IntPtr lpData)
            {
                if (uMsg == 1)
                {
                    DLLWin32.SendMessage(hwnd, 0x0400 + 103, 1, lpData);
                }
                return 0;
            }
        }

        protected class DLLWin32
        {
            [DllImport("user32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage([In] IntPtr hWnd, [In] UInt32 Msg, [In] UInt32 wParam, [In] IntPtr lParam);

            [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

            [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);
        }

    }

}
