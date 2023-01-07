using System;
using System.Xml;
using UnityEngine;

namespace Osiris_I18n
{
    public class Mission_Patch
    {

        private static int tiptype = -1;

        public Mission_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(MissionData), null, new Type[] { typeof(string), typeof(string), typeof(XmlNodeList) }, PatchType.postfix, GetType().GetMethod("MissionData_Constructor_Patch")));
            PatcherManager.Add(new Patcher(typeof(MissionData), "currentStatus", PropertyType.get, PatchType.postfix, GetType().GetMethod("MissionData_currentStatus_Patch")));
            PatcherManager.Add(new Patcher(typeof(MissionData.MissionObjective), "ToString", PatchType.postfix, GetType().GetMethod("MissionData__MissionObjective_ToString_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "QueueShowInstructionMessage", PatchType.postfix, GetType().GetMethod("PlayerGUI_QueueShowInstructionMessage_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerControl), "OnUpdateCompletedMissions", PatchType.prefix, GetType().GetMethod("PlayerControl_OnUpdateCompletedMissions_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerControl), "OnReadyToWalk", PatchType.prefix, GetType().GetMethod("PlayerControl_OnReadyToWalk_Patch")));
            PatcherManager.Add(new Patcher(typeof(PlayerControl), "OnUpdateCompletedMissions", PatchType.postfix, GetType().GetMethod("CleanTipType")));
            PatcherManager.Add(new Patcher(typeof(PlayerControl), "OnReadyToWalk", PatchType.postfix, GetType().GetMethod("CleanTipType")));
            PatcherManager.Add(new Patcher(typeof(PlayerGUI), "ShowInstructionMessage", PatchType.prefix, GetType().GetMethod("PlayerGUI_ShowInstructionMessage_Patch")));
            PatcherManager.Add(new Patcher(typeof(VoiceLogEntry), "SetData", PatchType.prefix, GetType().GetMethod("VoiceLogEntry_SetData_Patch")));
        }

        public static void MissionData_Constructor_Patch(MissionData __instance)
        {
            __instance.title = LoadLocalization.Instance.GetLocalizedString(__instance.title + "");
            __instance.description = LoadLocalization.Instance.GetLocalizedString(__instance.description + "");
            __instance.status = LoadLocalization.Instance.GetLocalizedString(__instance.status + "");
        }

        public static void MissionData_currentStatus_Patch(ref string __result)
        {
            int typeindex = __result.LastIndexOf(" ");
            if (__result.LastIndexOf("harvested") > 0)
            {
                __result = LoadLocalization.Instance.GetLocalizedString("harvested") + __result.Substring(0, typeindex) + "\n";
            }
            else if (__result.LastIndexOf("killed") > 0)
            {
                __result = LoadLocalization.Instance.GetLocalizedString("killed") + __result.Substring(0, typeindex) + "\n";
            }
            else if (__result.LastIndexOf("found") > 0)
            {
                __result = LoadLocalization.Instance.GetLocalizedString("found") + __result.Substring(0, typeindex) + "\n";
            }
            else if (__result.LastIndexOf("crafted") > 0)
            {
                __result = LoadLocalization.Instance.GetLocalizedString("crafted") + __result.Substring(0, typeindex) + "\n";
            }
        }

        public static void MissionData__MissionObjective_ToString_Patch(MissionData.MissionObjective __instance, ref string __result)
        {
            if (__result.IndexOf(__instance.name) > 0)
            {
                __result = __result.Replace(__instance.name, LoadLocalization.Instance.GetLocalizedString(__instance.name));
            }
            if (__result.LastIndexOf('s') == __result.Length - 1)
            {
                __result = __result.Remove(__result.Length - 1);
            }
        }

        public static void PlayerGUI_QueueShowInstructionMessage_Patch(ref string msg, float delay, bool playNotificationSound)
        {
            msg = LoadLocalization.Instance.GetLocalizedString(msg);
        }

        public static void PlayerControl_OnUpdateCompletedMissions_Patch(PlayerControl __instance, MissionComparer comparer)
        {
            if (__instance.isInTutorial)
            {
                if (comparer("ExitPod"))
                {
                    tiptype = 5;
                }
                else if (comparer("BreakPodDoor") || comparer("EjectPodDoor"))
                {
                    tiptype = 4;
                }
                else if (comparer("EscapePod"))
                {
                    tiptype = 6;
                }
                else if (comparer("HealWounds"))
                {
                    tiptype = 3;
                }
                else if (comparer("PickupBandage"))
                {
                    tiptype = 2;
                }
                else if (comparer("FixSuitBreach"))
                {
                    tiptype = 1;
                }
                else if (comparer("PickupPatchTape"))
                {
                    tiptype = 0;
                }
            }
        }

        public static void PlayerControl_OnReadyToWalk_Patch()
        {
            tiptype = 7;
        }

        public static void CleanTipType()
        {
            tiptype = -1;
        }

        public static void PlayerGUI_ShowInstructionMessage_Patch(ref string msg, bool playNotificationSound)
        {
            switch (tiptype)
            {
                case 0:
                    msg = LoadLocalization.Instance.GetLocalizedString(msg);
                    break;
                case 1:
                    msg = LoadLocalization.Instance.GetLocalizedString(msg);
                    break;
                case 2:
                    msg = LoadLocalization.Instance.GetLocalizedString(msg);
                    break;
                case 3:
                    msg = LoadLocalization.Instance.GetLocalizedString(msg);
                    break;
                case 4:
                    msg = LoadLocalization.Instance.GetLocalizedString(msg);
                    break;
                case 5:
                    msg = LoadLocalization.Instance.GetLocalizedString(msg);
                    break;
                case 6:
                    int index;
                    PlayerInventory.Me.HasItemEquipped("SurvivalKnife", out index);
                    msg = LoadLocalization.Instance.GetLocalizedString("Use the Survival Knife [{0}] to break the pod door. ({1})", index, DynamicActionKey.GetKeyName("Fire", true));
                    break;
                case 7:
                    msg = LoadLocalization.Instance.GetLocalizedString("Move with [{0}-{1}-{2}-{3}]", DynamicActionKey.GetKeyName("Move Vertical", Rewired.Pole.Positive, false), DynamicActionKey.GetKeyName("Move Horizontal", Rewired.Pole.Negative, false), DynamicActionKey.GetKeyName("Move Vertical", Rewired.Pole.Negative, false), DynamicActionKey.GetKeyName("Move Horizontal", Rewired.Pole.Positive, false));
                    break;
                default:
                    msg = LoadLocalization.Instance.GetLocalizedString(msg);
                    break;
            }
            tiptype = -1;
        }

        public static void VoiceLogEntry_SetData_Patch(VoiceLogInfo voiceLogInfo, VoiceLogMenu parentMenu)
        {
            voiceLogInfo.author = LoadLocalization.Instance.GetLocalizedString(voiceLogInfo.author);
            voiceLogInfo.date = LoadLocalization.Instance.GetLocalizedString(voiceLogInfo.date);
            voiceLogInfo.subject = LoadLocalization.Instance.GetLocalizedString(voiceLogInfo.subject);
            voiceLogInfo.storyText = LoadLocalization.Instance.GetLocalizedString(voiceLogInfo.storyText);
        }

    }
}
