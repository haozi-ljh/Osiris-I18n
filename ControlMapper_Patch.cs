using HarmonyLib;
using UnityEngine;

namespace Osiris_I18n
{
    class ControlMapper_Patch
    {

        [HarmonyPatch(typeof(Rewired.UI.ControlMapper.ControlMapper), "CreateLabel", new System.Type[] { typeof(UnityEngine.GameObject), typeof(string), typeof(UnityEngine.Transform), typeof(UnityEngine.Vector2) })]
        class ControlMapper_CreateLabel_Patch
        {
            public static void Prefix(GameObject prefab, ref string labelText, Transform parent, Vector2 offset)
            {
                labelText = LoadLocalization.Instance.GetLocalizedString(labelText);
            }
        }

    }
}
