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
    [BepInPlugin("I18n.OsirisNewDawn.Localization.plugin", "Osiris: New Dawn 多国语言翻译", "0.9.3")]
    public class I18n : BaseUnityPlugin
    {

        ConfigEntry<bool> oninitConfig;

        ConfigEntry<string> i18ncsvPathConfig;
        ConfigEntry<string> enoutPathConfig;

        ConfigEntry<bool> onEnOutConfig;
        ConfigEntry<bool> onAutoUpdateConfig;

        ConfigEntry<bool> onTextSizeConfig;
        ConfigEntry<bool> onVehicleHelpConfig;
        ConfigEntry<bool> onDepositoryConfig;
        ConfigEntry<bool> onTerrainConfig;

        private ModifyEnhance modify;

        public static bool onconfigGUI = false;
        private int configGUIstate = 0;
        private int tipstate = 0;
        private Vector2 scroll;
        private float textIndex = 0;
        private bool havenew = false;
        private bool fastmode = false;
        private bool checking = false;
        private bool downloading = false;
        private bool downloaded = false;
        private bool installing = false;

        public static string gamePath = "";
        public static string pluginPath = "";
        public static string dllPath = "";
        public static string i18ncsvPath = "";
        public static string enoutPath = "";
        private Texture2D[] helpimages;
        private string[] versionlist = new string[] { };
        private static List<string[]> i18ncsvs = new List<string[]>();

        private int windowWidth;
        private int windowHeight;
        private int configGUIWidth;
        private int configGUIHeight;

        private bool oninit = false;

        public static bool onEnOut = false;
        private bool onAutoUpdate = false;

        private bool onTextSize = false;
        private bool onVehicleHelp = false;
        private bool onDepository = false;
        private bool onTerrain = false;


        private void Start()
        {
            gamePath = Paths.GameRootPath;
            pluginPath = Paths.PluginPath;
            dllPath = GetType().Assembly.Location;

            oninitConfig = Config.Bind<bool>("config", "初始弹窗", true, "\n第一次使用补丁时的弹窗\n");

            i18ncsvPathConfig = Config.Bind<string>("config", "路径", Path.GetDirectoryName(dllPath) + "\\lang", "\n自定义翻译文件路径(默认为插件所在目录), 补丁会扫描该路径下所有符合格式的翻译文件\n翻译文件名称只有结尾必须为\"_I18n.csv\",推荐格式: \"地区语言名_I18n.csv\"\n输入 <gamepath> 快捷设置路径为游戏默认翻译文件路径\n");
            enoutPathConfig = Config.Bind<string>("config", "输出英语文本路径", Paths.GameRootPath, "\n游戏中的英语文本的输出路径\n");

            onEnOutConfig = Config.Bind<bool>("config", "输出英语文本", false, "\n输出游戏中的英语文本\n");
            onAutoUpdateConfig = Config.Bind<bool>("config", "自动检查更新", false, "\n当游戏启动时自动检查插件更新\n");
            
            onTextSizeConfig = Config.Bind<bool>("config", "文本大小优化", false, "\n对部分文本进行放大\n");
            onVehicleHelpConfig = Config.Bind<bool>("config", "飞船操作提示", false, "\n在操作飞船时增加了提示UI\n");
            onDepositoryConfig = Config.Bind<bool>("config", "箱内物品预览", false, "\n游戏早期版本功能的仿制\n");
            onTerrainConfig = Config.Bind<bool>("config", "关闭建筑地形修改", false, "\n关闭建筑地形修改\n");

            oninit = oninitConfig.Value;

            i18ncsvPath = i18ncsvPathConfig.Value;
            enoutPath = enoutPathConfig.Value;

            onEnOut = onEnOutConfig.Value;
            onAutoUpdate = onAutoUpdateConfig.Value;

            onTextSize = onTextSizeConfig.Value;
            onVehicleHelp = onVehicleHelpConfig.Value;
            onDepository = onDepositoryConfig.Value;
            onTerrain = onTerrainConfig.Value;


            if (!Directory.Exists(i18ncsvPath))
            {
                i18ncsvPathConfig.Value = i18ncsvPath = (string)i18ncsvPathConfig.DefaultValue;
                Config.Save();
                if (!Directory.Exists(i18ncsvPath))
                {
                    new DirectoryInfo(i18ncsvPath).Create();
                }
            }
            if (!Directory.Exists(enoutPath))
            {
                enoutPathConfig.Value = enoutPath = (string)enoutPathConfig.DefaultValue;
                Config.Save();
                if (!Directory.Exists(enoutPath))
                {
                    new DirectoryInfo(enoutPath).Create();
                }
            }

            RefreshI18nCSVs();

            windowWidth = Screen.width;
            windowHeight = Screen.height;
            configGUIWidth = windowWidth / 3 > 500 ? windowWidth / 3 : 500;
            configGUIHeight = windowHeight / 3 > 300 ? windowHeight / 3 : 300;

            Harmony harmony = new Harmony("I18n.OsirisNewDawn.Localization.plugin");

            PatcherManager.init(harmony);
            new Ability_Patch();
            new BackPack_Patch();
            new BuildDetails_Patch();
            new BuildTree_Patch();
            new ControlMapper_Patch();
            new Discovery_Patch();
            new LocalizationManager_Patch();
            new MainMenu_Patch();
            new Map_Patch();
            new MenuOption_Patch();
            new Mission_Patch();
            new TargetHUD_Patch();
            new OptionsMenu_Patch();
            new Other_Patch();
            new AddConfigButton();
            PatcherManager.LoadAll();

            modify = new ModifyEnhance();

            if (onTextSize)
                modify.VehicleHelpPatch();
            if (onVehicleHelp)
                modify.TextSizePatch();
            if (onDepository)
                modify.DepositoryPatch();
            if (onTerrain)
                modify.TerrainPatch();

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

            if (!Path.IsPathRooted(PlayerPrefs.GetString("SelectedLocalizationFile", LocalizationManager.DefaultLanguageFilename)) || !new FileInfo(PlayerPrefs.GetString("SelectedLocalizationFile", LocalizationManager.DefaultLanguageFilename)).DirectoryName.Equals(i18ncsvPath))
            {
                MainMenu.Me.confirmBoxManager.Activate("检测到汉化文件未正确设置, 是否打开设置界面", () => { configGUIstate = 1; onconfigGUI = true; if (onAutoUpdate) AutoUpdate(true); }, () => { if (onAutoUpdate) AutoUpdate(false); }, "是", "否");
            }
            else
            {
                if (onAutoUpdate)
                    AutoUpdate(false);
            }

            helpimages = new Texture2D[] { LoadT2D(Resource.font_before), LoadT2D(Resource.font_after), LoadT2D(Resource.vehicle_takeoff), LoadT2D(Resource.vehicle_flying), LoadT2D(Resource.depository_), LoadT2D(Resource.offterrain) };
        }

        private void AutoUpdate(bool guimode)
        {
            if (checking)
                return;

            UpdateVersionInfo();
            new System.Threading.Thread(() =>
            {
                while (checking)
                {
                    System.Threading.Thread.Sleep(1);
                }
                if (versionlist.Length == 0)
                {
                    if (guimode)
                        onconfigGUI = false;
                    MainMenu.Me.confirmBoxManager.Activate("获取更新失败", () => { if (guimode) onconfigGUI = true; }, () => { if (guimode) onconfigGUI = true; }, "是", "否");
                    return;
                }
                if (havenew)
                {
                    if (guimode)
                        onconfigGUI = false;
                    MainMenu.Me.confirmBoxManager.Activate("检测到有新版本，是否立即更新", () => { DownloadVersion(versionlist[versionlist.Length - 1], guimode); if (guimode) onconfigGUI = true; }, () => { if (guimode) onconfigGUI = true; }, "是", "否");
                }
                else
                {
                    MainMenu.Me.confirmBoxManager.Activate("当前版本已是最新", null, null, "确定", "取消");
                }
            }).Start();
        }

        private void UpdateVersionInfo()
        {
            if (checking)
                return;

            checking = true;
            new System.Threading.Thread(() =>
            {
                versionlist = new string[] { };

                string versioninfo = I18nUpdate.Check();
                versionlist = versioninfo.Split('\n');
                if (!string.IsNullOrEmpty(versioninfo) && !string.IsNullOrEmpty(versionlist[0]))
                {
                    for (int i = 0; i < versionlist.Length; i++)
                    {
                        versionlist[i] = versionlist[i].Trim();
                    }
                    if (versionlist[0].Equals("fastmode"))
                    {
                        fastmode = true;
                        string[] list = new string[versionlist.Length - 1];
                        Array.Copy(versionlist, 1, list, 0, list.Length);
                        versionlist = list;
                    }

                    Version latest = new Version();
                    Version now = Info.Metadata.Version;
                    int versionindex = versionlist.Length - 1;

                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[0-9\\.]");
                    if (regex.Replace(versionlist[versionindex], "").Length > 1)
                    {
                        latest = new Version(versionlist[versionindex].Replace(regex.Replace(versionlist[versionindex], ""), ".1"));
                    }
                    else
                    {
                        latest = new Version(versionlist[versionindex]);
                    }

                    if (latest.CompareTo(now) > 0)
                    {
                        havenew = true;
                    }
                }
                else
                {
                    havenew = false;
                    versionlist = new string[] { };
                }

                lock (this)
                {
                    checking = false;
                }
            }).Start();
        }

        private void DownloadVersion(string version, bool guimode)
        {
            if (downloading)
                return;

            downloading = true;
            new System.Threading.Thread(() =>
            {
                bool end = downloaded || I18nUpdate.Download(fastmode, version);
                if (end)
                {
                    if (guimode)
                        onconfigGUI = false;
                    if (downloaded)
                        MainMenu.Me.confirmBoxManager.Activate("检测到已下载文件, 是否安装", () => { if (guimode) onconfigGUI = true; Install(guimode); }, () => { if (guimode) onconfigGUI = true; lock (this) { downloaded = false; } }, "是", "否");
                    else
                        MainMenu.Me.confirmBoxManager.Activate("下载完成, 是否立即安装", () => { if (guimode) onconfigGUI = true; Install(guimode); }, () => { if (guimode) onconfigGUI = true; lock (this) { downloaded = true; } }, "是", "否");
                }
                else
                {
                    if (guimode)
                        onconfigGUI = false;
                    MainMenu.Me.confirmBoxManager.Activate("下载失败", () => { if (guimode) onconfigGUI = true; }, () => { if (guimode) onconfigGUI = true; }, "是", "否");
                }
                lock (this)
                {
                    downloading = false;
                }
            }).Start();
        }

        private void Install(bool guimode)
        {
            if (installing)
            {
                if (guimode)
                    onconfigGUI = false;
                MainMenu.Me.confirmBoxManager.Activate("正在安装...", () => { if (guimode) onconfigGUI = true; }, () => { if (guimode) onconfigGUI = true; }, "是", "否");
                return;
            }

            if (guimode)
                onconfigGUI = false;
            MainMenu.Me.confirmBoxManager.Activate("开始安装...", () => { if (guimode) onconfigGUI = true; }, () => { if (guimode) onconfigGUI = true; }, "是", "否");
            installing = true;
            new System.Threading.Thread(() => {
                if (I18nUpdate.Install())
                    MainMenu.Me.confirmBoxManager.Activate("安装成功, 新补丁将在游戏重启后生效\n是否立即重启", () => { string gameversion = Paths.ProcessName.Contains("BTSE") ? "843350" : "402710"; System.Diagnostics.Process.Start("cmd.exe", "/c echo 游戏即将重启 & timeout /t 5 & start \"\" steam://run/" + gameversion); Application.Quit(); }, () => { if (guimode) onconfigGUI = true; }, "是", "否");
                else
                    MainMenu.Me.confirmBoxManager.Activate("安装失败", () => { if (guimode) onconfigGUI = true; }, () => { if (guimode) onconfigGUI = true; }, "是", "否");
                lock (this)
                {
                    installing = false;
                }
            }).Start();
        }

        private Texture2D LoadT2D(System.Drawing.Bitmap bitmap)
        {
            Texture2D t2d = new Texture2D(2, 2);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Dispose();
            t2d.LoadImage(bytes);
            return t2d;
        }

        public static List<string[]> RefreshI18nCSVs()
        {
            if (Directory.Exists(i18ncsvPath))
            {
                i18ncsvs.Clear();
                FileSystemInfo[] fsis = new DirectoryInfo(i18ncsvPath).GetFileSystemInfos();
                foreach (FileSystemInfo fsi in fsis)
                {
                    if (fsi.Name.Contains("_I18n.csv"))
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

        private void Update()
        {
            if(new KeyboardShortcut(KeyCode.Escape).IsDown())
            {
                if (configGUIstate == 0)
                {
                    onconfigGUI = false;
                    CancelConfig();
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
                ConfigWindow(ref configwindows);
            }
        }

        private void ConfigWindow(ref Rect rect)
        {
            switch (configGUIstate)
            {
                case 0:
                    rect = GUI.Window(0, rect, MainWindow, "Osiris: New Dawn 多国语言翻译插件配置");
                    break;
                case 1:
                    rect = GUI.Window(0, rect, SelectCSVWindow, "Osiris: New Dawn 多国语言翻译插件配置");
                    break;
                case 2:
                    rect = GUI.Window(0, rect, HelpWindow, "Osiris: New Dawn 多国语言翻译插件配置");
                    break;
                case 3:
                    rect = GUI.Window(0, rect, ASWindow, "Osiris: New Dawn 多国语言翻译插件配置");
                    break;
                case 4:
                    rect = GUI.Window(0, rect, ModifyWindow, "Osiris: New Dawn 多国语言翻译插件配置");
                    break;
                case 5:
                    rect = GUI.Window(0, rect, TipHelpWindow, "Osiris: New Dawn 多国语言翻译插件配置");
                    break;
                case 10:
                    rect = GUI.Window(0, rect, SelectUpdateWindow, "Osiris: New Dawn 多国语言翻译插件配置");
                    break;
                default:
                    break;
            }
        }

        private void MainWindow(int winid)
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
                OpenDialogDir.ShowDialog("请选择存放有翻译文件的文件夹\n\n插件可读取翻译文件的文件名结尾必须为\"_I18n.csv(区分大小写)\"", ref i18ncsvPath);
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

            if (GUILayout.Button("高级设置", GUILayout.MinHeight(30)))
            {
                configGUIstate = 3;
                textIndex = 0;
            }
            if (GUILayout.Button("额外功能", GUILayout.MinHeight(30)))
            {
                configGUIstate = 4;
            }

            GUILayout.Label("", GUILayout.MaxHeight(configGUIHeight - 180));

            if (GUILayout.Button("默认设置", GUILayout.MinHeight(30)))
            {
                DefConfig();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("保存", GUILayout.MinHeight(30)))
            {
                SaveConfig();
                onconfigGUI = false;
            }
            if (GUILayout.Button("取消", GUILayout.MinHeight(30)))
            {
                CancelConfig();
                onconfigGUI = false;
            }
            GUILayout.EndHorizontal();
        }

        private void ASWindow(int winid)
        {
            if (GUILayout.Button("输出游戏过程中的英语文本: " + (onEnOut ? "开启" : "关闭"), GUILayout.MinHeight(30)))
            {
                onEnOut = !onEnOut;
            }
            GUILayout.Label("输出路径:");
            GUIStyle pathstyle = GUI.skin.box;
            pathstyle.alignment = TextAnchor.MiddleLeft;
            pathstyle.wordWrap = false;

            float textwidth = pathstyle.CalcSize(new GUIContent(enoutPath)).x;
            float showwidth = configGUIWidth - 120;
            string text = enoutPath.Substring((int)(textIndex / textwidth * 100));

            GUILayout.BeginHorizontal();
            GUILayout.Box(text, pathstyle, GUILayout.MinHeight(30), GUILayout.MaxWidth(configGUIWidth - 120));
            if (GUILayout.Button("...", GUILayout.MaxWidth(30), GUILayout.MinHeight(30)))
            {
                OpenDialogDir.ShowDialog("请选择英语文本的输出路径", ref enoutPath);
            }
            if (GUILayout.Button("打开文件", GUILayout.MaxWidth(70), GUILayout.MinHeight(30)))
            {
                string filepath = Path.Combine(enoutPath, "EnglishOutput.txt");
                if (!File.Exists(filepath))
                {
                    File.CreateText(filepath).Close();
                }
                System.Diagnostics.Process.Start("notepad.exe", filepath);
            }
            GUILayout.EndHorizontal();
            if (showwidth < textwidth)
            {
                textIndex = GUILayout.HorizontalScrollbar(textIndex, showwidth, 0, textwidth);
            }
            else
            {
                textIndex = 0;
            }

            if (GUILayout.Button("自动检查更新: " + (onAutoUpdate ? "开启" : "关闭"), GUILayout.MinHeight(30)))
            {
                onAutoUpdate = !onAutoUpdate;
            }
            if (GUILayout.Button("立即检查更新", GUILayout.MinHeight(30)))
            {
                if (downloaded)
                {
                    onconfigGUI = false;
                    MainMenu.Me.confirmBoxManager.Activate("检测到已下载文件, 是否安装", () => { onconfigGUI = true; }, () => { onconfigGUI = true; downloaded = false; }, "是", "否");
                }
                else
                    AutoUpdate(true);
            }
            if (GUILayout.Button("查看所有版本", GUILayout.MinHeight(30)))
            {
                if (!checking)
                    UpdateVersionInfo();
                configGUIstate = 10;
            }

            GUILayout.Label("", GUILayout.MaxHeight(configGUIHeight - 150));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("确定", GUILayout.MinHeight(30)))
            {
                configGUIstate = 0;
                textIndex = 0;
            }
            if (GUILayout.Button("取消", GUILayout.MinHeight(30)))
            {
                configGUIstate = 0;
                textIndex = 0;
            }
            GUILayout.EndHorizontal();
        }

        private void ModifyWindow(int winid)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("文本大小优化: " + (onTextSize ? "开启" : "关闭"), GUILayout.MinHeight(30)))
            {
                onTextSize = !onTextSize;
            }
            if (GUILayout.Button("<color=yellow>?</color>", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
            {
                configGUIstate = 5;
                tipstate = 1;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("飞船操作提示: " + (onVehicleHelp ? "开启" : "关闭"), GUILayout.MinHeight(30)))
            {
                onVehicleHelp = !onVehicleHelp;
            }
            if (GUILayout.Button("<color=yellow>?</color>", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
            {
                configGUIstate = 5;
                tipstate = 2;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("箱内物品预览: " + (onDepository ? "开启" : "关闭"), GUILayout.MinHeight(30)))
            {
                onDepository = !onDepository;
            }
            if (GUILayout.Button("<color=yellow>?</color>", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
            {
                configGUIstate = 5;
                tipstate = 3;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("关闭建筑地形修改: " + (onTerrain ? "开启" : "关闭"), GUILayout.MinHeight(30)))
            {
                onTerrain = !onTerrain;
            }
            if (GUILayout.Button("<color=yellow>?</color>", GUILayout.MaxWidth(30), GUILayout.MaxHeight(30)))
            {
                configGUIstate = 5;
                tipstate = 4;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("", GUILayout.MaxHeight(configGUIHeight - 150));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("确定", GUILayout.MinHeight(30)))
            {
                configGUIstate = 0;
                tipstate = 0;
            }
            if (GUILayout.Button("取消", GUILayout.MinHeight(30)))
            {
                configGUIstate = 0;
                tipstate = 0;
            }
            GUILayout.EndHorizontal();
        }

        private void HelpWindow(int winid)
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

        private void TipHelpWindow(int winid)
        {
            switch (tipstate)
            {
                case 1:
                    Font_Help();
                    break;
                case 2:
                    Vehicle_Help();
                    break;
                case 3:
                    Depository_Help();
                    break;
                case 4:
                    Terrain_Help();
                    break;
                default:
                    tipstate = 0;
                    configGUIstate = 0;
                    break;
            }
        }

        private void Font_Help()
        {
            float multiplier = helpimages[0].width / helpimages[0].height;

            GUIStyle texstyle = GUI.skin.box;
            texstyle.alignment = TextAnchor.MiddleCenter;

            scroll = GUILayout.BeginScrollView(scroll, false, false);
            GUILayout.Label("此功能适当的放大了游戏内的部分文字\n\n功能开启前字体大小:");
            GUILayout.Box(helpimages[0], texstyle, GUILayout.MaxWidth(configGUIWidth - 45), GUILayout.MaxHeight((configGUIWidth - 45) * multiplier));
            GUILayout.Label("功能开启后字体大小:");
            GUILayout.Box(helpimages[1], texstyle, GUILayout.MaxWidth(configGUIWidth - 45), GUILayout.MaxHeight((configGUIWidth - 45) * multiplier));
            GUILayout.EndScrollView();

            if (GUILayout.Button("确定", GUILayout.MinHeight(30)))
            {
                configGUIstate = 4;
                tipstate = 0;
                scroll = Vector2.zero;
            }
        }
        
        private void Vehicle_Help()
        {
            float multiplier = helpimages[2].width / helpimages[2].height;

            GUIStyle texstyle = GUI.skin.box;
            texstyle.alignment = TextAnchor.MiddleCenter;

            scroll = GUILayout.BeginScrollView(scroll, false, false);
            GUILayout.Label("此功能为飞船添加了操作提示\n\n飞船起飞时:");
            GUILayout.Box(helpimages[2], texstyle, GUILayout.MaxWidth(configGUIWidth - 45), GUILayout.MaxHeight((configGUIWidth - 45) * multiplier));
            GUILayout.Label("飞船起飞后:");
            GUILayout.Box(helpimages[3], texstyle, GUILayout.MaxWidth(configGUIWidth - 45), GUILayout.MaxHeight((configGUIWidth - 45) * multiplier));
            GUILayout.EndScrollView();

            if (GUILayout.Button("确定", GUILayout.MinHeight(30)))
            {
                configGUIstate = 4;
                tipstate = 0;
                scroll = Vector2.zero;
            }
        }
        
        private void Depository_Help()
        {
            float multiplier = helpimages[4].width / helpimages[4].height;

            GUIStyle texstyle = GUI.skin.box;
            texstyle.alignment = TextAnchor.MiddleCenter;

            scroll = GUILayout.BeginScrollView(scroll, false, false);
            GUILayout.Label("此功能为游戏早期版本功能的仿制, 开启后可预览箱内物品\n\n<color=red>注意!!!\n此功能最好别在线上模式使用, 可能会被识别成作弊\n此功能对性能要求较高, 开启后在箱子附近可能帧数会降低</color>\n\n开启后效果预览:");
            GUILayout.Box(helpimages[4], texstyle, GUILayout.MaxWidth(configGUIWidth - 45), GUILayout.MaxHeight((configGUIWidth - 45) * multiplier));
            GUILayout.EndScrollView();

            if (GUILayout.Button("确定", GUILayout.MinHeight(30)))
            {
                configGUIstate = 4;
                tipstate = 0;
                scroll = Vector2.zero;
            }
        }

        private void Terrain_Help()
        {
            float multiplier = helpimages[5].width / helpimages[5].height;

            GUIStyle texstyle = GUI.skin.box;
            texstyle.alignment = TextAnchor.MiddleCenter;

            scroll = GUILayout.BeginScrollView(scroll, false, false);
            GUILayout.Label("此功能关闭了高版本建筑会修改地形的功能\n\n<color=yellow>注意!\n此功能最好别在线上模式使用, 可能会被识别成作弊</color>\n\n开启后效果预览:");
            GUILayout.Box(helpimages[5], texstyle, GUILayout.MaxWidth(configGUIWidth - 45), GUILayout.MaxHeight((configGUIWidth - 45) * multiplier));
            GUILayout.EndScrollView();

            if (GUILayout.Button("确定", GUILayout.MinHeight(30)))
            {
                configGUIstate = 4;
                tipstate = 0;
                scroll = Vector2.zero;
            }
        }

        private void SelectCSVWindow(int winid)
        {
            RefreshI18nCSVs();
            scroll = GUILayout.BeginScrollView(scroll, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.box, GUILayout.MaxHeight(configGUIHeight - 60));
            for (int i = 0; i < i18ncsvs.Count; i++)
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
                scroll = Vector2.zero;
            }
        }

        private void SelectUpdateWindow(int winid)
        {
            if (!checking)
            {
                if (versionlist.Length > 0)
                {
                    scroll = GUILayout.BeginScrollView(scroll, false, false, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.box, GUILayout.MaxHeight(configGUIHeight - 60));
                    for (int i = versionlist.Length - 1; i >= 0; i--)
                    {
                        if (GUILayout.Button(versionlist[i], GUILayout.MinHeight(30)))
                        {
                            onconfigGUI = false;
                            MainMenu.Me.confirmBoxManager.Activate($"开始下载版本<color=yellow>{versionlist[i]}</color>", () => { onconfigGUI = true; }, () => { onconfigGUI = true; }, "是", "否");
                            DownloadVersion(versionlist[i], true);
                        }
                    }
                    GUILayout.EndScrollView();
                }
                else
                {
                    GUIStyle infostyle = GUI.skin.box;
                    infostyle.alignment = TextAnchor.MiddleCenter;
                    infostyle.wordWrap = false;

                    GUILayout.Box("获取失败", infostyle, GUILayout.MaxHeight(configGUIHeight - 30));
                }
            }
            else
            {
                GUIStyle infostyle = GUI.skin.box;
                infostyle.alignment = TextAnchor.MiddleCenter;
                infostyle.wordWrap = false;

                GUILayout.Box("获取版本信息...", infostyle, GUILayout.MaxHeight(configGUIHeight - 30));
            }
            if (GUILayout.Button("返回", GUILayout.MinHeight(30)))
            {
                configGUIstate = 3;
                scroll = Vector2.zero;
            }
        }

        private void DefConfig()
        {
            i18ncsvPathConfig.Value = (string)i18ncsvPathConfig.DefaultValue;
            enoutPathConfig.Value = (string)enoutPathConfig.DefaultValue;

            onEnOutConfig.Value = (bool)onEnOutConfig.DefaultValue;
            onAutoUpdateConfig.Value = (bool)onAutoUpdateConfig.DefaultValue;

            onTextSizeConfig.Value = (bool)onTextSizeConfig.DefaultValue;
            onVehicleHelpConfig.Value = (bool)onVehicleHelpConfig.DefaultValue;
            onDepositoryConfig.Value = (bool)onDepositoryConfig.DefaultValue;
            onTerrainConfig.Value = (bool)onTerrainConfig.DefaultValue;
            
            Config.Save();
            Config.Reload();

            i18ncsvPath = i18ncsvPathConfig.Value;
            enoutPath = enoutPathConfig.Value;

            onEnOut = onEnOutConfig.Value;
            onAutoUpdate = onAutoUpdateConfig.Value;

            onTextSize = onTextSizeConfig.Value;
            onVehicleHelp = onVehicleHelpConfig.Value;
            onDepository = onDepositoryConfig.Value;
            onTerrain = onTerrainConfig.Value;

            modify.VehicleHelpUnpatch();
            modify.TextSizeUnpatch();
            modify.DepositoryUnpatch();
            modify.TerrainUnpatch();
        }

        private void SaveConfig()
        {
            i18ncsvPathConfig.Value = i18ncsvPath;
            enoutPathConfig.Value = enoutPath;

            onEnOutConfig.Value = onEnOut;
            onAutoUpdateConfig.Value = onAutoUpdate;

            onTextSizeConfig.Value = onTextSize;
            onVehicleHelpConfig.Value = onVehicleHelp;
            onDepositoryConfig.Value = onDepository;
            onTerrainConfig.Value = onTerrain;

            if (onTextSize)
                modify.VehicleHelpPatch();
            else
                modify.VehicleHelpUnpatch();
            if (onVehicleHelp)
                modify.TextSizePatch();
            else
                modify.TextSizeUnpatch();
            if (onDepository)
                modify.DepositoryPatch();
            else
                modify.DepositoryUnpatch();
            if (onTerrain)
                modify.TerrainPatch();
            else
                modify.TerrainUnpatch();

            Config.Save();
            Config.Reload();
        }
        
        private void CancelConfig()
        {
            i18ncsvPath = i18ncsvPathConfig.Value;
            enoutPath = enoutPathConfig.Value;

            onEnOut = onEnOutConfig.Value;
            onAutoUpdate = onAutoUpdateConfig.Value;

            onTextSize = onTextSizeConfig.Value;
            onVehicleHelp = onVehicleHelpConfig.Value;
            onDepository = onDepositoryConfig.Value;
            onTerrain = onTerrainConfig.Value;
        }

        protected class AddConfigButton
        {

            private static Button configbutton;

            public AddConfigButton()
            {
                PatcherManager.Add(new Patcher(typeof(MainMenu), "Start", PatchType.postfix, GetType().GetMethod("MainMenu_Start_Patch")));
                PatcherManager.Add(new Patcher(typeof(MainMenu), "Update", PatchType.postfix, GetType().GetMethod("MainMenu_Update_Patch")));
                PatcherManager.Add(new Patcher(typeof(PlayerGUI), "Start", PatchType.postfix, GetType().GetMethod("PlayerGUI_Start_Patch")));
                PatcherManager.Add(new Patcher(typeof(Button), "Press", PatchType.prefix, GetType().GetMethod("PauseButton")));
            }

            public static bool PauseButton()
            {
                if (onconfigGUI)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public static void MainMenu_Start_Patch(MainMenu __instance)
            {

                Button[] bs = __instance.MainPanel.GetComponentsInChildren<Button>();
                Transform tf = bs[0].transform.parent;
                configbutton = Instantiate(bs[0], tf);
                configbutton.name = "Button Osiris-I18n_Option";

                if (bs[0].transform.localPosition == bs[1].transform.localPosition && bs[0].transform.localPosition == new Vector3(0, 0, 0))
                {
                    bool isVertical = true;
                    for(int i = 0; i < bs.Length - 1; i++)
                    {
                        if (bs[i].transform.position.x != bs[i + 1].transform.position.x)
                        {
                            isVertical = false;
                        }
                    }
                    if (isVertical)
                    {
                        RectTransform rtf = tf.GetComponent<RectTransform>();
                        rtf.sizeDelta = new Vector2(rtf.sizeDelta.x, rtf.sizeDelta.y + configbutton.GetComponent<RectTransform>().sizeDelta.y);
                    }
                    else
                    {
                        RectTransform rtf = tf.GetComponent<RectTransform>();
                        rtf.sizeDelta = new Vector2(rtf.sizeDelta.x + configbutton.GetComponent<RectTransform>().sizeDelta.x, configbutton.GetComponent<RectTransform>().sizeDelta.y);
                    }
                }
                else
                {
                    if (bs[0].transform.localPosition.x == bs[1].transform.localPosition.x && bs[0].transform.localPosition.x != 0)
                    {
                        RectTransform rtf = tf.GetComponent<RectTransform>();
                        rtf.sizeDelta = new Vector2(rtf.sizeDelta.x, rtf.sizeDelta.y + configbutton.GetComponent<RectTransform>().sizeDelta.y);
                    }
                    if (bs[0].transform.localPosition.y == bs[1].transform.localPosition.y && bs[0].transform.localPosition.y != 0)
                    {
                        RectTransform rtf = tf.GetComponent<RectTransform>();
                        rtf.sizeDelta = new Vector2(rtf.sizeDelta.x + configbutton.GetComponent<RectTransform>().sizeDelta.x, configbutton.GetComponent<RectTransform>().sizeDelta.y);
                    }
                }

                for (int i = 0; i < configbutton.onClick.GetPersistentEventCount(); i++)
                {
                    configbutton.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
                }
                configbutton.onClick.AddListener(() => { onconfigGUI = true; });

            }

            public static void MainMenu_Update_Patch()
            {
                Text cbt = configbutton.GetComponentInChildren<Text>();
                cbt.text = "插件设置";
                cbt.color = Color.yellow;
            }

            public static void PlayerGUI_Start_Patch(PlayerGUI __instance)
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

            public static void ShowDialog(string lpszTitle, ref string path)
            {
                OpenDialogDir langdialog = new OpenDialogDir
                {
                    pszDisplayName = string.Empty,
                    lpszTitle = lpszTitle,
                    pidlRoot = IntPtr.Zero,
                    lParam = Marshal.StringToHGlobalUni(path),
                    lpfn = Marshal.GetFunctionPointerForDelegate(bcp),
                    ulFlags = 0x00000040 | 0x00000002 | 0x00000001
                };
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
