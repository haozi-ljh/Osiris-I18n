using System;
using UnityEngine;

namespace Osiris_I18n
{
    public class ControlMapper_Patch
    {

        public ControlMapper_Patch()
        {
            PatcherManager.Add(new Patcher(typeof(Rewired.UI.ControlMapper.ControlMapper), "CreateLabel", new Type[] { typeof(GameObject), typeof(string), typeof(Transform), typeof(Vector2) }, PatchType.prefix, GetType().GetMethod("ControlMapper_CreateLabel_Patch")));
        }

        public static void ControlMapper_CreateLabel_Patch(GameObject prefab, ref string labelText, Transform parent, Vector2 offset)
        {
            labelText = LoadLocalization.Instance.GetLocalizedString(labelText);
        }

    }
}
