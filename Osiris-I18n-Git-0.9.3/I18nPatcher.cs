using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Osiris_I18n
{
    public enum PatchType
    {
        prefix,
        postfix,
        transpiler
    }

    public enum PropertyType
    {
        get,
        set,
        def
    }

    public class Patcher
    {

        private readonly static Assembly gamedll = Assembly.LoadFrom(BepInEx.Paths.ManagedPath + "\\Assembly-CSharp.dll");

        private bool patched = false;
        private MethodInfo patchedMethod = null;

        private MethodBase objectivem;
        private PatchType pt;
        private int runindex = -1;
        private List<MethodInfo> pmlist = new List<MethodInfo>();

        public Patcher(Compatible compatible, PatchType patchType, params MethodInfo[] patchmethods)
        {
            if (compatible == null || compatible.type == null)
            {
                objectivem = null;
                pmlist = new List<MethodInfo>(patchmethods);
            }
            else
            {
                if (compatible.propertyType != PropertyType.def)
                {
                    initPatcher(compatible.type, compatible.method, compatible.propertyType, patchType, patchmethods);
                }
                else if (compatible.types != null)
                {
                    initPatcher(compatible.type, compatible.method, compatible.types, patchType, patchmethods);
                }
                else
                {
                    initPatcher(compatible.type, compatible.method, patchType, patchmethods);
                }
            }
        }

        public Patcher(Type objectiveType, string objectiveMethod, PatchType patchType, params MethodInfo[] patchmethods)
        {
            initPatcher(objectiveType, objectiveMethod, patchType, patchmethods);
        }

        public Patcher(Type objectiveType, string objectiveMethod, Type[] types, PatchType patchType, params MethodInfo[] patchmethods)
        {
            initPatcher(objectiveType, objectiveMethod, types, patchType, patchmethods);
        }

        public Patcher(Type objectiveType, string objectiveMethod, PropertyType propertyType, PatchType patchType, params MethodInfo[] patchmethods)
        {
            initPatcher(objectiveType, objectiveMethod, propertyType, patchType, patchmethods);
        }

        public Patcher(Compatible compatible, PatchType patchType, int selectmethod, params MethodInfo[] patchmethods)
        {
            runindex = selectmethod;
            if (compatible == null || compatible.type == null)
            {
                objectivem = null;
                pmlist = new List<MethodInfo>(patchmethods);
            }
            else
            {
                if (compatible.propertyType != PropertyType.def)
                {
                    initPatcher(compatible.type, compatible.method, compatible.propertyType, patchType, patchmethods);
                }
                else if (compatible.types != null)
                {
                    initPatcher(compatible.type, compatible.method, compatible.types, patchType, patchmethods);
                }
                else
                {
                    initPatcher(compatible.type, compatible.method, patchType, patchmethods);
                }
            }
        }

        public Patcher(Type objectiveType, string objectiveMethod, PatchType patchType, int selectmethod, params MethodInfo[] patchmethods)
        {
            runindex = selectmethod;
            initPatcher(objectiveType, objectiveMethod, patchType, patchmethods);
        }

        public Patcher(Type objectiveType, string objectiveMethod, Type[] types, PatchType patchType, int selectmethod, params MethodInfo[] patchmethods)
        {
            runindex = selectmethod;
            initPatcher(objectiveType, objectiveMethod, types, patchType, patchmethods);
        }

        public Patcher(Type objectiveType, string objectiveMethod, PropertyType propertyType, PatchType patchType, int selectmethod, params MethodInfo[] patchmethods)
        {
            runindex = selectmethod;
            initPatcher(objectiveType, objectiveMethod, propertyType, patchType, patchmethods);
        }

        private void initPatcher(Type objectiveType, string objectiveMethod, PatchType patchType, params MethodInfo[] patchmethods)
        {

            if (string.IsNullOrEmpty(objectiveMethod))
            {
                objectivem = objectiveType.GetConstructors()[0];
            }
            else
            {
                objectivem = objectiveType.GetMethod(objectiveMethod);
            }

            pt = patchType;
            pmlist = new List<MethodInfo>(patchmethods);

            if (objectivem == null)
            {
                if (string.IsNullOrEmpty(objectiveMethod))
                {
                    objectivem = objectiveType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
                }
                else
                {
                    objectivem = objectiveType.GetMethod(objectiveMethod, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
                }
            }

        }

        private void initPatcher(Type objectiveType, string objectiveMethod, Type[] types, PatchType patchType, params MethodInfo[] patchmethods)
        {

            if (string.IsNullOrEmpty(objectiveMethod))
            {
                objectivem = objectiveType.GetConstructor(types);
            }
            else
            {
                objectivem = objectiveType.GetMethod(objectiveMethod, types);
            }

            pt = patchType;
            pmlist = new List<MethodInfo>(patchmethods);

            if (objectivem == null)
            {
                if (string.IsNullOrEmpty(objectiveMethod))
                {
                    objectivem = objectiveType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, types, new ParameterModifier[] { new ParameterModifier(types.Length) });
                }
                else
                {
                    objectivem = objectiveType.GetMethod(objectiveMethod, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static, Type.DefaultBinder, types, new ParameterModifier[] { new ParameterModifier(types.Length) });
                }
            }

        }

        private void initPatcher(Type objectiveType, string objectiveMethod, PropertyType propertyType, PatchType patchType, params MethodInfo[] patchmethods)
        {

            PropertyInfo propertyInfo = objectiveType.GetProperty(objectiveMethod);
            if (propertyInfo == null)
            {
                propertyInfo = objectiveType.GetProperty(objectiveMethod, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            }

            if(propertyInfo != null)
            {
                switch (propertyType)
                {
                    case PropertyType.get:
                        objectivem = propertyInfo.GetGetMethod();
                        if (objectivem == null)
                        {
                            objectivem = propertyInfo.GetGetMethod(true);
                        }
                        break;
                    case PropertyType.set:
                        objectivem = propertyInfo.GetSetMethod();
                        if (objectivem == null)
                        {
                            objectivem = propertyInfo.GetSetMethod(true);
                        }
                        break;
                    default:
                        objectivem = null;
                        break;
                }
            }
            else
            {
                objectivem = null;
            }

            pt = patchType;
            pmlist = new List<MethodInfo>(patchmethods);

        }

        public Patcher Info(string name, bool ismain = false, bool needrestart = true)
        {

            return this;
        }

        public Patcher AddSpare(Patcher patcher)
        {

            return this;
        }

        public void Patch(Harmony harmony)
        {
            
            bool noerror = false;

            if (pmlist.Count > 1)
            {
                if (runindex >= 0 && runindex < pmlist.Count)
                {
                    noerror = Patch(harmony, pmlist[runindex]);
                }
                else
                {
                    for (runindex = 0; runindex < pmlist.Count;)
                    {
                        if(Patch(harmony, pmlist[runindex]))
                        {
                            noerror = true;
                            break;
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning($"模块加载方案{++runindex}加载失败");
                        }
                    }
                }
            }
            else
            {
                noerror = Patch(harmony, pmlist[0]);
            }
            
            if (!noerror) { UnityEngine.Debug.LogError("模块加载失败"); }

        }

        private bool Patch(Harmony harmony, MethodInfo patchmethod)
        {

            if (objectivem == null)
            {
                return false;
            }

            try
            {
                switch (pt)
                {
                    case PatchType.prefix:
                        harmony.Patch(objectivem, prefix: new HarmonyMethod(patchmethod));
                        break;
                    case PatchType.postfix:
                        harmony.Patch(objectivem, postfix: new HarmonyMethod(patchmethod));
                        break;
                    case PatchType.transpiler:
                        harmony.Patch(objectivem, transpiler: new HarmonyMethod(patchmethod));
                        break;
                }

                patchedMethod = patchmethod;
                return patched = true;
            }
            catch
            {
                return false;
            }

        }

        public void UnPatch(Harmony harmony)
        {
            if (patched)
            {
                harmony.Unpatch(objectivem, patchedMethod);
                patched = false;
                patchedMethod = null;
            }
        }

        public static Compatible Find(Type t, string s, Type[] ts = null, PropertyType pt = PropertyType.def)
        {
            return new Compatible(t, s, ts, pt);
        }

        public static Compatible Find(string t, string s, Type[] ts = null, PropertyType pt = PropertyType.def)
        {
            return new Compatible(gamedll.GetType(t), s, ts, pt);
        }

        public class Compatible
        {
            public Type type = null;
            public string method = null;
            public Type[] types = null;
            public PropertyType propertyType = PropertyType.def;

            public Compatible(Type t, string s, Type[] ts, PropertyType pt)
            {
                type = t;
                method = s;
                types = ts;
                propertyType = pt;
            }
        }

    }

    public class PatcherManager
    {

        private static Harmony PMharmony;

        private static List<Patcher> patchers = new List<Patcher>();

        public static void init(Harmony harmony)
        {
            PMharmony = harmony;
            patchers.Clear();
        }

        public static void Load(int index)
        {
            patchers[index].Patch(PMharmony);
        }

        public static void Load(int[] indexs)
        {
            foreach (int index in indexs)
            {
                patchers[index].Patch(PMharmony);
            }
        }

        public static void UnLoad(int index)
        {
            patchers[index].UnPatch(PMharmony);
        }

        public static void UnLoad(int[] indexs)
        {
            foreach (int index in indexs)
            {
                patchers[index].Patch(PMharmony);
            }
        }

        public static void LoadAll()
        {
            foreach(Patcher patcher in patchers)
            {
                patcher.Patch(PMharmony);
            }
        }

        public static void UnLoadAll()
        {
            foreach (Patcher patcher in patchers)
            {
                patcher.UnPatch(PMharmony);
            }
        }

        public static int Add(Patcher patcher)
        {
            patchers.Add(patcher);
            return patchers.Count - 1;
        }

    }
}
