using IOTLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;

namespace IOTLib
{
    public class CGPrefabPropertyDrawUtility
    {
        public enum IntTypeEnum
        {
            I32,
            U32,
            I64,
            U64,
            Long,
            ULong
        }

        /// <summary>
        /// 获取所有脚本的公开字段
        /// </summary>
        /// <param name="target"></param>
        /// <param name="callBack"></param>
        public static void GetFields(GameObject target, System.Action<VarGroup> callBack)
        {
            var allScripts = target.GetComponents<MonoBehaviour>();

            var vars = ListPool<CGVar>.Get();

            foreach (var a in allScripts)
            {
                vars.Clear();

                var ce = a.GetType().GetCustomAttribute<CGEditor>(false);

                if (ce == null) continue;

                IteraVars(a, (b) =>
                {
                    vars.Add(b);
                });

                if (vars.Count > 0)
                {
                    CGCustomEditor? customEditor = null;

                    // 自定义编辑器
                    var editor = a.GetType().GetCustomAttribute<CGCustomEditorAttribute>(true);
                    if (editor != null && editor.CEType != null)
                    {
                        customEditor = Activator.CreateInstance(editor.CEType) as CGCustomEditor;
                        customEditor.target = a;
                        customEditor.OnEnable();
                    }

                    var newData = new VarGroup() { Behaviour = a, Vars = vars.ToArray(), ToggleValue = true, CustomEditor = customEditor };
                    newData.m_CGEditorHeaderName = ce.CustomName;

                    callBack?.Invoke(newData);
                }
            }

            ListPool<CGVar>.Release(vars);
        }

        static void IteraVars(System.Object target, System.Action<CGVar> callBack)
        {
            if (target == null) return;

            var targetType = target.GetType();

            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var a in fields)
            {
                var znvar = a.GetCustomAttribute<CGVar>(false);

                if (znvar != null)
                {
                    znvar.FieldInfo = a;
                    callBack?.Invoke(znvar);
                }
                else
                {
                    callBack?.Invoke(new CGVar(a.Name) { FieldInfo = a });
                }
            }
        }

        static bool DrawStringProperty(string label, string? value, out string changedValue)
        {
            bool changed = false;
            changedValue = string.Empty;

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(100));
            var newValue = GUILayout.TextField(value);
            if (newValue != value)
            {
                changedValue = newValue;
                changed = true;
            }
            GUILayout.EndHorizontal();

            return changed;
        }

        static bool DrawBoolProperty(string label, bool value, out bool changedValue)
        {
            bool changed = false;
            changedValue = false;

            var newValue = GUILayout.Toggle(value, label);
            if (newValue != value)
            {
                changedValue = newValue;
                changed = true;
            }

            return changed;
        }

        static bool MakeString(CGVar var, System.Object obj)
        {
            var value = var.FieldInfo.GetValue(obj) as string;

            if (DrawStringProperty(var.VarName, value, out var newText))
            {
                var.FieldInfo.SetValue(obj, newText);
                return true;
            }

            return false;
        }
        static bool MakeFloat(CGVar var, System.Object obj)
        {
            var value = (float)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToString(), out var newText))
            {
                // 还是小数点，在输入中
                if (newText.EndsWith(".")) return false;

                if (float.TryParse(newText.Trim(), out var newFloat))
                {
                    var.FieldInfo.SetValue(obj, newFloat);

                    return true;
                }
            }

            return false;
        }

        static bool MakeNumber(CGVar var, System.Object obj, IntTypeEnum intType)
        {
            var value = var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToString(), out var newText))
            {
                // 还是小数点，在输入中
                if (newText.EndsWith(".")) return false;

                switch (intType)
                {
                    case IntTypeEnum.I32:
                        if (int.TryParse(newText, out var vint))
                        {
                            var.FieldInfo.SetValue(obj, vint);
                        }
                        break;
                    case IntTypeEnum.U32:
                        if (uint.TryParse(newText, out var vuint))
                        {
                            var.FieldInfo.SetValue(obj, vuint);
                        }
                        break;
                    case IntTypeEnum.I64:
                        if (Int64.TryParse(newText, out var vi64))
                        {
                            var.FieldInfo.SetValue(obj, vi64);
                        }
                        break;
                    case IntTypeEnum.U64:
                        if (UInt64.TryParse(newText, out var vu64))
                        {
                            var.FieldInfo.SetValue(obj, vu64);
                        }
                        break;
                    case IntTypeEnum.Long:
                        if (long.TryParse(newText, out var vlong))
                        {
                            var.FieldInfo.SetValue(obj, vlong);
                        }
                        break;
                    case IntTypeEnum.ULong:
                        if (ulong.TryParse(newText, out var vulong))
                        {
                            var.FieldInfo.SetValue(obj, vulong);
                        }
                        break;
                }
            }

            return true;
        }

        static bool MakeVec2(CGVar var, System.Object obj)
        {
            var value = (Vector2)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToOriginStr(), out var newText))
            {
                try
                {
                    var.FieldInfo.SetValue(obj, newText.ToVector2());

                    return true;
                }
                catch (InvalidOperationException) { }
            }

            return false;
        }

        static bool MakeVec3(CGVar var, System.Object obj)
        {
            var value = (Vector3)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToOriginStr(), out var newText))
            {
                try
                {
                    var.FieldInfo.SetValue(obj, newText.ToVector3());

                    return true;
                }
                catch (InvalidOperationException) { }
            }

            return false;
        }

        static bool MakeVec4(CGVar var, System.Object obj)
        {
            var value = (Vector4)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToOriginStr(), out var newText))
            {
                try
                {
                    var.FieldInfo.SetValue(obj, newText.ToVector4());

                    return true;
                }
                catch (InvalidOperationException) { }
            }

            return false;
        }

        static bool MakeByte(CGVar var, System.Object obj)
        {
            var value = (byte)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToString(), out var newText))
            {
                if (byte.TryParse(newText, out var result))
                {
                    var.FieldInfo.SetValue(obj, result);

                    return true;
                }
            }

            return false;
        }

        static bool MakeBool(CGVar var, System.Object obj)
        {
            var value = (bool)var.FieldInfo.GetValue(obj);

            if (DrawBoolProperty(var.VarName, value, out var changedValue))
            {
                var.FieldInfo.SetValue(obj, changedValue);

                return true;
            }

            return false;
        }

        static Vector2 groupScroll = Vector2.zero;
        static bool HasChanged = false;

        // 一些CustomEditor自己绘制了可能要用这个
        public static void SetGUIDirty()
        {
            HasChanged = true;
        }

        /// <summary>
        /// 返回是否修改了字段
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="vars"></param>
        /// <returns></returns>
        internal static bool DrawFields(Component instance, IEnumerable<CGVar> vars)
        {
            bool changed = false;
            HasChanged = false;

            foreach (var a in vars)
            {
                GUILayout.BeginVertical();

                if (a.FieldInfo.FieldType == typeof(string))
                {
                    changed = MakeString(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(float))
                {
                    changed = MakeFloat(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(int))
                {
                    changed = MakeNumber(a, instance, IntTypeEnum.I32);
                }
                else if (a.FieldInfo.FieldType == typeof(uint))
                {
                    changed = MakeNumber(a, instance, IntTypeEnum.U32);
                }
                else if (a.FieldInfo.FieldType == typeof(Vector2))
                {
                    changed = MakeVec2(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(Vector3))
                {
                    changed = MakeVec3(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(Vector4))
                {
                    changed = MakeVec4(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(byte))
                {
                    changed = MakeByte(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(Int64))
                {
                    changed = MakeNumber(a, instance, IntTypeEnum.I64);
                }
                else if (a.FieldInfo.FieldType == typeof(UInt64))
                {
                    changed = MakeNumber(a, instance, IntTypeEnum.U64);
                }
                else if (a.FieldInfo.FieldType == typeof(long))
                {
                    changed = MakeNumber(a, instance, IntTypeEnum.Long);
                }
                else if (a.FieldInfo.FieldType == typeof(ulong))
                {
                    changed = MakeNumber(a, instance, IntTypeEnum.ULong);
                }
                else if(a.FieldInfo.FieldType == typeof(bool))
                {
                    changed = MakeBool(a, instance);
                }

                GUILayout.EndVertical();

                if(changed) HasChanged = true;
            }

            return HasChanged;
        }

        public static void DrawVarGroup(IEnumerable<VarGroup> groups)
        {
            groupScroll = GUILayout.BeginScrollView(groupScroll);

            foreach (var g in groups)
            {
                GUILayout.Box(g.HeaderName, "Header");

                if (g.CustomEditor != null)
                {
                    g.CustomEditor.OnGUI(g.Behaviour, g.Vars);

                    if (HasChanged) g.CustomEditor.OnCGFieldUpdate();
                }
                else
                {
                    DrawFields(g.Behaviour, g.Vars);
                }

                // 通过编辑器有值发生变更了
                if (HasChanged)
                {
                    g.Behaviour.gameObject.SendMessage(
                            nameof(IComponentPropertyUpdate.OnCGFieldUpdate),
                            SendMessageOptions.DontRequireReceiver);
                }

            }

            GUILayout.EndScrollView();
        }
    }
}