using HarmonyLib;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Osiris_I18n
{
    public class ModifyEnhance
    {
        private readonly int vehiclehelpid, terrainid;
        private readonly int[] textsizeids, depositoryids;
        private bool terrainload = false;
        private static bool onterrain = false;

        private static bool VehicleGUI_ontakeoffhint;
        private static bool VehicleGUI_onflyhint;
        private static List<ObjectInventory> loaded = new List<ObjectInventory>();
        private static UnityEvent goDestroy = new UnityEvent();
        private static Dictionary<string, bool> terrainstate = new Dictionary<string, bool>();

        public ModifyEnhance()
        {
            vehiclehelpid = PatcherManager.Add(new Patcher(typeof(VehicleGUI), "Update", PatchType.postfix, GetType().GetMethod("VehicleGUI_Update_Alter")));

            textsizeids = new int[] { PatcherManager.Add(new Patcher(typeof(PlayerMovement), "CheckGroundStatus", PatchType.postfix, GetType().GetMethod("PlayerMovement_CheckGroundStatus_Optimize"))),
                PatcherManager.Add(new Patcher(typeof(VitalReadout), "UpdateVitalsOxygen", PatchType.postfix, GetType().GetMethod("VitalReadout_UpdateVitalsOxygen_Optimize"))),
                PatcherManager.Add(new Patcher(typeof(VitalWarning), "Activate", PatchType.postfix, GetType().GetMethod("VitalWarning_Activate_Optimize"))) };

            depositoryids = new int[] { PatcherManager.Add(new Patcher(typeof(PlayerTargetHUDDisplay), "Update", PatchType.postfix, GetType().GetMethod("PlayerTargetHUDDisplay_Update_Patch"))),
                PatcherManager.Add(new Patcher(typeof(ObjectInventory), "CloseInteraction", PatchType.postfix, GetType().GetMethod("ObjectInventory_CloseInteraction_Patch"))) };

            terrainid = PatcherManager.Add(new Patcher(typeof(StructureObject), "Start", PatchType.prefix, GetType().GetMethod("StructureObject_Start_Patch")));
        }

        public void VehicleHelpPatch()
        {
            VehicleGUI_ontakeoffhint = true;
            VehicleGUI_onflyhint = true;
            PatcherManager.Load(vehiclehelpid);
        }

        public void VehicleHelpUnpatch()
        {
            VehicleGUI_ontakeoffhint = false;
            VehicleGUI_onflyhint = false;
            PatcherManager.UnLoad(vehiclehelpid);
        }

        public void TextSizePatch()
        {
            PatcherManager.Load(textsizeids);
        }

        public void TextSizeUnpatch()
        {
            PatcherManager.UnLoad(textsizeids);
        }

        public void DepositoryPatch()
        {
            PatcherManager.Load(depositoryids);
        }
        
        public void DepositoryUnpatch()
        {
            PatcherManager.UnLoad(depositoryids);
        }

        public void TerrainPatch()
        {
            if (!terrainload)
            {
                PatcherManager.Load(terrainid);
                terrainload = true;
                onterrain = true;
            }
            else
                onterrain = true;
        }

        public void TerrainUnpatch()
        {
            onterrain = false;
        }

        //飞船操作提示
        public static void VehicleGUI_Update_Alter(VehicleGUI __instance)
        {
            if (__instance.sliderThrottle != null && __instance.sliderHover != null)
            {
                if (VehicleGUI_ontakeoffhint && __instance.sliderHover.value > 0f && __instance.sliderThrottle.value <= 0.5f)
                {
                    PlayerGUI.Me.Invoke("TurnOffInstructionMessage", 0f);
                    PlayerGUI.Me.ShowInstructionMessage("[W-A-S-D]水平移动", true);
                    VehicleGUI_ontakeoffhint = false;
                    PlayerGUI.Me.Invoke("TurnOffInstructionMessage", 5f);
                }
                else if (!VehicleGUI_ontakeoffhint && (__instance.sliderHover.value <= 0f || __instance.sliderThrottle.value > 0.5f))
                {
                    VehicleGUI_ontakeoffhint = true;
                }
                if (VehicleGUI_onflyhint && __instance.sliderThrottle.value > 0.5f)
                {
                    PlayerGUI.Me.Invoke("TurnOffInstructionMessage", 0f);
                    PlayerGUI.Me.ShowInstructionMessage("[W-S]控制机头抬降,[A-D]左右拐弯,[Q-E]翻转", true);
                    VehicleGUI_onflyhint = false;
                    PlayerGUI.Me.Invoke("TurnOffInstructionMessage", 5f);
                }
                else if (!VehicleGUI_onflyhint && __instance.sliderThrottle.value <= 0.5f)
                {
                    VehicleGUI_onflyhint = true;
                }
            }
        }
        //飞船操作提示

        //部分文本放大
        public static void PlayerMovement_CheckGroundStatus_Optimize()
        {
            PlayerGUI.Me.slopeText.fontSize = 20;
        }

        public static void VitalReadout_UpdateVitalsOxygen_Optimize(VitalReadout __instance)
        {
            __instance.vitalExtraText[0].fontSize = 20;
        }

        public static void VitalWarning_Activate_Optimize(VitalWarning __instance)
        {
            __instance.textComponent.fontSize = 20;
        }
        //部分文本放大

        //箱内物品预览
        public static void PlayerTargetHUDDisplay_Update_Patch(PlayerTargetHUDDisplay __instance)
        {
            PlayerTargetHUDDisplay.HUDState hudState = Traverse.Create(__instance).Field("hudState").GetValue<PlayerTargetHUDDisplay.HUDState>();
            if ((hudState == PlayerTargetHUDDisplay.HUDState.FADE_IN || hudState == PlayerTargetHUDDisplay.HUDState.ON) && __instance.currentHUDTarget.interactableComponent != null)
            {
                if (__instance.currentHUDTarget.interactableComponent.GetType().Equals(typeof(ObjectInventory)))
                {
                    Vector3 m_targetPos = Traverse.Create(__instance).Field("m_targetPos").GetValue<Vector3>();
                    float num = Vector3.Distance(Player.Transform.position.SetHeight(m_targetPos.y), m_targetPos);
                    ObjectInventory oi = (ObjectInventory)__instance.currentHUDTarget.interactableComponent;
                    if (!loaded.Contains(oi))
                    {
                        foreach (ObjectInventory objectInventory in loaded)
                        {
                            ObjectInventory_CloseInteraction(objectInventory);
                        }
                        loaded.Clear();
                        goDestroy.Invoke();

                        if (num <= __instance.interactDistance && oi.objectName.Equals("Depository"))
                        {
                            ObjectInventory_OnBeginInteract(oi);
                            CreateItems(oi);
                            loaded.Add(oi);
                        }
                    }
                    else if (num > __instance.interactDistance && loaded.Count > 0)
                    {
                        foreach (ObjectInventory objectInventory in loaded)
                        {
                            ObjectInventory_CloseInteraction(objectInventory);
                        }
                        loaded.Clear();
                        goDestroy.Invoke();
                    }
                }
                else if (loaded.Count > 0)
                {
                    foreach (ObjectInventory objectInventory in loaded)
                    {
                        ObjectInventory_CloseInteraction(objectInventory);
                    }
                    loaded.Clear();
                    goDestroy.Invoke();
                }
            }
            else if (loaded.Count > 0 && !PlayerGUI.Me.panelInventory.GetActive())
            {
                foreach (ObjectInventory objectInventory in loaded)
                {
                    ObjectInventory_CloseInteraction(objectInventory);
                }
                loaded.Clear();
                goDestroy.Invoke();
            }
        }

        public static void ObjectInventory_CloseInteraction_Patch(ObjectInventory __instance)
        {
            if (__instance.objectName.Equals("Depository"))
            {
                ObjectInventory_CloseInteraction(__instance);
                loaded.Clear();
                goDestroy.Invoke();
                CreateItems(__instance);
                loaded.Add(__instance);
            }
        }

        private static void ObjectInventory_OnBeginInteract(ObjectInventory oi)
        {
            if (oi.canvasTrigger != null)
            {
                oi.canvasTrigger.SetActive(false);
            }
            if (oi.audioSource != null && oi.audioClipOpen != null)
            {
                oi.audioSource.PlayOneShot(oi.audioClipOpen, oi.openVolume);
            }
            if (!Traverse.Create(oi).Field("hasPlayedActivateInteraction").GetValue<bool>())
            {
                if (oi.onActivateInteraction != null)
                {
                    oi.onActivateInteraction.Invoke();
                }
                Traverse.Create(oi).Field("hasPlayedActivateInteraction").SetValue(true);
            }
        }
        
        private static void ObjectInventory_CloseInteraction(ObjectInventory oi)
        {
            if (oi.audioSource != null && oi.audioClipClose != null)
            {
                oi.audioSource.PlayOneShot(oi.audioClipClose, oi.closeVolume);
            }
            if (oi.canvasTrigger != null)
            {
                oi.canvasTrigger.SetActive(true);
            }
            if (Traverse.Create(oi).Field("hasPlayedActivateInteraction").GetValue<bool>())
            {
                if (oi.onDeactivateInteraction != null)
                {
                    oi.onDeactivateInteraction.Invoke();
                }
                Traverse.Create(oi).Field("hasPlayedActivateInteraction").SetValue(false);
            }
        }

        private static void CreateItems(ObjectInventory __instance)
        {
            float size = 0.2f, space = 0.018f;
            float x = __instance.gameObject.transform.localPosition.x, y = __instance.gameObject.transform.localPosition.y, z = __instance.gameObject.transform.localPosition.z;
            float cos = (float)Math.Round(Math.Cos(__instance.gameObject.transform.rotation.eulerAngles.y * Math.PI / 180), 15), sin = (float)Math.Round(Math.Sin(__instance.gameObject.transform.rotation.eulerAngles.y * Math.PI / 180), 15);

            float xadd, zadd;
            for (int fi = 0; fi < 10; fi++)
            {
                xadd = (.98f - (size + space) * fi) * cos + .45f * sin;
                zadd = .45f * cos - (.98f - (size + space) * fi) * sin;
                for (int fii = 0; fii < 6; fii++)
                {
                    GameObject gameObject = CreateItem(__instance.items[fi + fii * 10].type, .2f, __instance.gameObject.transform.rotation.eulerAngles);
                    gameObject.transform.localPosition = new Vector3(x + xadd, y + 1.34f - fii * (size + space), z + zadd);

                    Vector3 center = gameObject.GetComponentInChildren<Renderer>().bounds.center - gameObject.transform.localPosition;
                    if (center != new Vector3(0, 0, 0))
                    {
                        if (center.x != 0 && center.x + gameObject.GetComponentInChildren<Renderer>().bounds.size.x > size)
                        {
                            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x - center.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
                        }
                        if (center.y != 0 && center.y + gameObject.GetComponentInChildren<Renderer>().bounds.size.y > size)
                        {
                            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y - center.y, gameObject.transform.localPosition.z);
                        }
                        if (center.z != 0 && center.z + gameObject.GetComponentInChildren<Renderer>().bounds.size.z > size)
                        {
                            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x - center.z, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
                        }
                    }

                }
            }
        }

        private static GameObject CreateItem(string itemname, float scale, Vector3 eulerAngles)
        {
            GameObject initially = Resources.Load<GameObject>("Items/" + itemname);
            GameObject gameObject;
            Quaternion rotation = new Quaternion();

            if (initially == null)
            {
                rotation.eulerAngles = eulerAngles;

                gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObject.transform.localScale = new Vector3(scale, scale, scale);
                gameObject.transform.rotation = rotation;

                Material m = gameObject.GetComponent<Renderer>().material;
                m.shader = Shader.Find("Transparent/Diffuse");
                m.color = string.IsNullOrEmpty(itemname) ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);

                goDestroy.AddListener(() => { UnityEngine.Object.Destroy(gameObject); });

                return gameObject;
            }
            initially.SetActive(false);

            gameObject = UnityEngine.Object.Instantiate(initially);
            UnityEngine.Object.Destroy(gameObject.GetComponent<InventoryItem>());
            UnityEngine.Object.Destroy(gameObject.GetComponent<InventoryPickup>());
            UnityEngine.Object.Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.SetActive(true);
            gameObject.GetComponentInChildren<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            Vector3 boundsize = gameObject.GetComponentInChildren<Renderer>().bounds.size;
            Vector3 boundmeshsize = gameObject.GetComponentInChildren<MeshFilter>().mesh.bounds.size;
            float maxsize = boundsize.x > boundsize.y ? (boundsize.x > boundsize.z ? boundsize.x : boundsize.z) : (boundsize.y > boundsize.z ? boundsize.y : boundsize.z), scalesize = scale / maxsize;
            rotation.eulerAngles = new Vector3(eulerAngles.x, boundmeshsize.x < boundmeshsize.z ? (eulerAngles.y < 90 ? 360 - 90 + eulerAngles.y : eulerAngles.y - 90) : eulerAngles.y, eulerAngles.z);

            gameObject.transform.localScale = new Vector3(scalesize, scalesize, scalesize);
            gameObject.transform.rotation = rotation;

            goDestroy.AddListener(() => { UnityEngine.Object.Destroy(gameObject); });

            return gameObject;
        }
        //箱内物品预览

        //关闭地形修改
        public static void StructureObject_Start_Patch(StructureObject __instance)
        {
            if (onterrain)
            {
                if (!terrainstate.ContainsKey(__instance.type))
                {
                    terrainstate.Add(__instance.type, Traverse.Create(__instance).Field("deformTerrain").GetValue<bool>());
                }
                Traverse.Create(__instance).Field("deformTerrain").SetValue(false);
            }
            else
            {
                if (terrainstate.ContainsKey(__instance.type))
                {
                    Traverse.Create(__instance).Field("deformTerrain").SetValue(terrainstate[__instance.type]);
                }
            }
        }
        //关闭地形修改

    }
}
