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
                    callBack?.Invoke(newData);
                }
            }

            ListPool<CGVar>.Release(vars);
        }

        static void IteraVars(System.Object target, System.Action<CGVar> callBack)
        {
            if (target == null) return;

            var targetType = target.GetType();

            if (targetType.GetCustomAttribute<CGEditor>(false) == null) return;

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

        static void MakeString(CGVar var, System.Object obj)
        {
            var value = var.FieldInfo.GetValue(obj) as string;

            if (DrawStringProperty(var.VarName, value, out var newText))
            {
                var.FieldInfo.SetValue(obj, newText);
            }
        }
        static void MakeFloat(CGVar var, System.Object obj)
        {
            var value = (float)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToString(), out var newText))
            {
                // 还是小数点，在输入中
                if (newText.EndsWith(".")) return;

                if (float.TryParse(newText.Trim(), out var newFloat))
                {
                    var.FieldInfo.SetValue(obj, newFloat);
                }
            }
        }

        static void MakeNumber(CGVar var, System.Object obj, IntTypeEnum intType)
        {
            var value = var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToString(), out var newText))
            {
                // 还是小数点，在输入中
                if (newText.EndsWith(".")) return;

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
        }

        static void MakeVec2(CGVar var, System.Object obj)
        {
            var value = (Vector2)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToOriginStr(), out var newText))
            {
                try
                {
                    var.FieldInfo.SetValue(obj, newText.ToVector2());
                }
                catch (InvalidOperationException) { }
            }
        }

        static void MakeVec3(CGVar var, System.Object obj)
        {
            var value = (Vector3)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToOriginStr(), out var newText))
            {
                try
                {
                    var.FieldInfo.SetValue(obj, newText.ToVector3());
                }
                catch (InvalidOperationException) { }
            }
        }

        static void MakeVec4(CGVar var, System.Object obj)
        {
            var value = (Vector4)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToOriginStr(), out var newText))
            {
                try
                {
                    var.FieldInfo.SetValue(obj, newText.ToVector4());
                }
                catch (InvalidOperationException) { }
            }
        }

        static void MakeByte(CGVar var, System.Object obj)
        {
            var value = (byte)var.FieldInfo.GetValue(obj);

            if (DrawStringProperty(var.VarName, value.ToString(), out var newText))
            {
                if (byte.TryParse(newText, out var result))
                {
                    var.FieldInfo.SetValue(obj, result);
                }
            }
        }

        static Vector2 groupScroll = Vector2.zero;

        internal static void DrawFields(Component instance, IEnumerable<CGVar> vars)
        {
            foreach (var a in vars)
            {
                GUILayout.BeginVertical();

                if (a.FieldInfo.FieldType == typeof(string))
                {
                    MakeString(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(float))
                {
                    MakeFloat(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(int))
                {
                    MakeNumber(a, instance, IntTypeEnum.I32);
                }
                else if (a.FieldInfo.FieldType == typeof(uint))
                {
                    MakeNumber(a, instance, IntTypeEnum.U32);
                }
                else if (a.FieldInfo.FieldType == typeof(Vector2))
                {
                    MakeVec2(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(Vector3))
                {
                    MakeVec3(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(Vector4))
                {
                    MakeVec4(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(byte))
                {
                    MakeByte(a, instance);
                }
                else if (a.FieldInfo.FieldType == typeof(Int64))
                {
                    MakeNumber(a, instance, IntTypeEnum.I64);
                }
                else if (a.FieldInfo.FieldType == typeof(UInt64))
                {
                    MakeNumber(a, instance, IntTypeEnum.U64);
                }
                else if (a.FieldInfo.FieldType == typeof(long))
                {
                    MakeNumber(a, instance, IntTypeEnum.Long);
                }
                else if (a.FieldInfo.FieldType == typeof(ulong))
                {
                    MakeNumber(a, instance, IntTypeEnum.ULong);
                }

                GUILayout.EndVertical();
            }
        }

        public static void DrawVarGroup(IEnumerable<VarGroup> groups)
        {
            groupScroll = GUILayout.BeginScrollView(groupScroll);

            foreach (var g in groups)
            {
                GUILayout.Box(g.TypeName, "Header");

                if (g.CustomEditor != null)
                {
                    g.CustomEditor.OnGUI(g.Behaviour, g.Vars);
                }
                else
                {
                    DrawFields(g.Behaviour, g.Vars);
                }
            }

            GUILayout.EndScrollView();
        }
    }
}