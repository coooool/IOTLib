using IOTLib.IDESystem.Handles;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

namespace IOTLib
{
    public class CGPrefabEditorWindow : MonoBehaviour, IDragArea
    {
        #region EDITORWINDOW
        private static Rect DragWindowRect;
        private static GUISkin m_DragGUIStyle;
        public static GUISkin GUIStyle
        {
            get
            {
                if (m_DragGUIStyle == null)
                {
                    m_DragGUIStyle = Resources.Load<GUISkin>("DOS/DOSSkin");
                }

                return m_DragGUIStyle;
            }
        }
        // 窗口宽度
        private const int WINDOW_WIDTH = 380;
        #endregion

        #region PROPERTY
        static bool Is2DEditor = false;
        internal static List<VarGroup>? m_EditVars = null;
        static List<VarGroup> EditVars
        {
            get
            {
                if(m_EditVars == null)
                {
                    m_EditVars = ListPool<VarGroup>.Get();
                }

                return m_EditVars;
            }
            set
            {
                if (value == null)
                {
                    if (m_EditVars != null) ListPool<VarGroup>.Release(m_EditVars);
                }

                m_EditVars = value;
            }
        }

        static WeakReference<ExportCGPrefab> _activeCgPrefab = new WeakReference<ExportCGPrefab>(null);
        static WeakReference<GameObject> _activeGameObject = new WeakReference<GameObject>(null);      
        public static Transform SelectActiveTransform
        {
            get
            {
                if (_activeGameObject.TryGetTarget(out var g))
                    return g.transform;

                return null;
            }
            set
            {
                SelectActiveGameObject = value.gameObject;
            }
        }

        public static ExportCGPrefab SelectActiveCGPrefab
        {
            get
            {
                if (_activeCgPrefab.TryGetTarget(out var g))
                    return g;

                return null;
            }

            set
            {
                if (_activeCgPrefab.TryGetTarget(out var o))
                {
                    if (o == value) return;
                }

                _activeCgPrefab.SetTarget(value);
            }
        }

        public static GameObject SelectActiveGameObject
        {
            get
            {
                if (_activeGameObject.TryGetTarget(out var g))
                    return g;

                return null;
            }
            set
            {
                if (_activeGameObject.TryGetTarget(out var o))
                {
                    if( o == value) return;
                }

                _activeGameObject.SetTarget(value);

                foreach (var a in m_EditVars)
                {
                    if (a.CustomEditor != null)
                    {
                        a.CustomEditor.OnDestroy();
                    }
                }

                InitFields();
            }
        }

        public Rect DragArea => DragWindowRect;
        #endregion

        private void OnDestroy()
        {
            SelectActiveGameObject = null;
            EditVars = null;
        }

        static void InitFields()
        {
            Is2DEditor = false;
            SelectActiveCGPrefab = null;
            EditVars.Clear();

            if(SelectActiveGameObject != null)
            {
                // 处理Prefab 项
                if (SelectActiveGameObject.TryGetComponent<ExportCGPrefab>(out var cg))
                {
                    SelectActiveCGPrefab = cg;
                    Is2DEditor = cg.IsUGUI;
                }
                // ---------

                CGPrefabPropertyDrawUtility.GetFields(SelectActiveGameObject, (a) =>
                {
                    EditVars.Add(a);
                });
            }
        }

        private void Start()
        {
            DragWindowRect.Set(Screen.width - (WINDOW_WIDTH + 30), 80, WINDOW_WIDTH, Mathf.RoundToInt(Screen.height * 0.70f));
            GetComponent<CGHandleDragMouse>().RegisterDragPerformEvent(this);
        }

        string m_t_str, m_r_str, m_s_str;
        void TRSEditorWindow(int windowid)
        {
            if (SelectActiveGameObject == null)
            {
                GUILayout.Label("请选择一个物体进行编辑");
            }
            else
            {
                GUILayout.Label("名称");
                SelectActiveGameObject.name = GUILayout.TextField(SelectActiveGameObject.name);

                GUILayout.Label("位置");
                if(Is2DEditor)
                {
                    m_t_str = GUILayout.TextField(SelectActiveCGPrefab.UIRecttransform.anchoredPosition.ToOriginStr());
                }
                else
                {
                    m_t_str = GUILayout.TextField(SelectActiveTransform.position.ToOriginStr());
                }
               
                GUILayout.Label("旋转(欧拉角XYZ)");
                m_r_str = GUILayout.TextField(SelectActiveTransform.eulerAngles.ToOriginStr());

                GUILayout.Label("缩放");
                if(Is2DEditor)
                    m_s_str = GUILayout.TextField(SelectActiveCGPrefab.UIRecttransform.sizeDelta.ToOriginStr());
                else
                    m_s_str = GUILayout.TextField(SelectActiveTransform.localScale.ToOriginStr());

                if (GUILayout.Button("删除物体"))
                {
                    Destroy(SelectActiveGameObject);
                    SelectActiveGameObject= null;
                    return;
                }

                if (GUI.changed)
                {
                    try
                    {
                        if (Is2DEditor)
                        {
                            SelectActiveCGPrefab.UIRecttransform.anchoredPosition = m_t_str.ToVector2();
                            SelectActiveTransform.position = SelectActiveCGPrefab.UIRecttransform.position;
                            
                            // 保存为保存系统使用的是3D映射物体。所以需要放回去
                            SelectActiveCGPrefab.UIRecttransform.sizeDelta = m_s_str.ToVector2();
                            SelectActiveTransform.localScale = SelectActiveCGPrefab.UIRecttransform.sizeDelta;
                            
                            if (SelectActiveCGPrefab.TryGetComponent<DragGameObject2D>(out var dgo))
                                dgo.UpdateEditPos();
                        }
                        else
                        {
                            SelectActiveTransform.position = m_t_str.ToVector3();
                            SelectActiveTransform.localScale = m_s_str.ToVector3();
                        }
                       
                        SelectActiveTransform.eulerAngles = m_r_str.ToVector3();

                        // 通知值更新
                        SelectActiveTransform.SendMessage(
                            nameof(IDEPropertyEvent.OnTransformUpdate), 
                            SelectActiveTransform,
                            SendMessageOptions.DontRequireReceiver);
                    }
                    catch (Exception)
                    {
                        Debug.LogError("请输入正确的数据格式");
                    }
                }

                GUILayout.Space(6);

                if (EditVars.Count == 0)
                    GUILayout.Button("没有组件可以编辑", "Header");
                else
                    CGPrefabPropertyDrawUtility.DrawVarGroup(EditVars);
            }

            GUI.DragWindow(new Rect(0,0, Screen.width, 30));
        }

        void OnGUI()
        {
            GUI.skin = GUIStyle;

            if (!CGUnityWindowManager.GSetting.NoHeader)
                DragWindowRect = GUI.Window(0, DragWindowRect, TRSEditorWindow, SelectActiveGameObject == null ? "属性编辑器" : SelectActiveGameObject.name);

            // F键聚焦
            if (SelectActiveGameObject != null && Event.current.type == EventType.KeyUp)
            {
                if (!SelectActiveCGPrefab.IsUGUI)
                {
                    if (Event.current.keyCode == KeyCode.F)
                    {
                        CameraHandle.F(SelectActiveGameObject, () => CameraHelpFunc.ToObserver(SelectActiveGameObject), "D2");
                    }
                }
            }
        }

        void Update()
        {
            foreach (var a in EditVars)
            {
                if (a.CustomEditor != null)
                {
                    a.CustomEditor.OnUpdate();
                }
            }
        }

        /// <summary>
        /// 当从CGPREFAB窗口拖过来时使用
        /// </summary>
        /// <param name="target"></param>
        public void OnDragPerform(ExportCGPrefab target)
        {
            foreach(var a in EditVars)
            {
                if(a.CustomEditor != null)
                {
                    a.CustomEditor.OnDragPerform(target);
                }
            }
        }
    }
}