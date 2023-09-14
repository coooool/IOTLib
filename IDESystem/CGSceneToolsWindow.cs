using IOTLib;
using IOTLib.IDESystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

namespace IOTLib
{
    /// <summary>
    /// CG场景工具箱
    /// </summary>
    public class CGSceneToolsWindow : MonoBehaviour
    {
        private const int WINDOW_WIDTH = 380;
        private Rect m_DragWindowRect;

        private DateTime? m_LastSaveTime;

        // 场景中的所有物体类型
        private string[] m_SceneObjectTags = Array.Empty<string>();
        private int m_lastCgSceneObjCount = 0;
        private int m_SelectTagIndex = -1;

        private void OnEnable()
        {
            m_DragWindowRect.Set(Screen.width / 2 - WINDOW_WIDTH / 2, 80, WINDOW_WIDTH, 240);

            // 自动刷新Tag
            InvokeRepeating("UpdateSceneObjectTag", 1, 1.5f);

            UpdateSceneObjectTag();
        }

        void SaveScene()
        {
            CGSceneManager.Save("json", CGResources.TAGName);
            m_LastSaveTime = DateTime.Now;
        }

        void CloseEditorWindow()
        {
            CGUnityWindowManager.Close();
        }

        void OnSelectTagChanged(string tag)
        {
            CGPrefabEditorWindow.SelectActiveGameObject = null;

            var allObject = TagSystem.Find<DynGameTagAgent>(true, CGResources.TAGName);

            foreach (var obj in allObject)
            {
                if (obj.HasTag(tag))
                {
                    obj.gameObject.GetOrCreateCompoent<DragGameObject>();
                }
                else
                {
                    if (obj.TryGetComponent<DragGameObject>(out var dgo))
                    {
                        DestroyImmediate(dgo);
                    }
                }
            }
        }

        Vector2 m_scrollView;
        void WindowFunc(int windowid)
        {
            if (GUILayout.Button("保存场景"))
            {
                SaveScene();
            }

            if (GUILayout.Button("关闭编辑器"))
            {
                CloseEditorWindow();
            }

            m_scrollView = GUILayout.BeginScrollView(m_scrollView, "CGTagBox");
            var newSelectIndex = GUILayout.SelectionGrid(m_SelectTagIndex, m_SceneObjectTags, 4, "CGTag");
            if(newSelectIndex != m_SelectTagIndex)
            {
                OnSelectTagChanged(m_SceneObjectTags[newSelectIndex]);
            }
            m_SelectTagIndex = newSelectIndex;
            GUILayout.EndScrollView();

            if (m_LastSaveTime.HasValue)
            {
                GUILayout.Label($"最后保存时间: {m_LastSaveTime}", "CenterLabel", GUILayout.ExpandWidth(true));
            }
            else
            {
                GUILayout.Label("最后保存时间:未保存", "CenterLabel", GUILayout.ExpandWidth(true));
            }

            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        void UpdateSceneObjectTag()
        {
            var allObject = TagSystem.Find<DynGameTagAgent>(true, CGResources.TAGName);

            if (m_lastCgSceneObjCount == allObject.Count) return;

            HashSet<string> tags = new HashSet<string>(10);

            foreach(var obj in allObject)
            {
                foreach(var a in obj.m_subTag)
                {
                    if (!string.IsNullOrEmpty(a))
                    {
                        if (tags.Contains(a)) continue;
                        tags.Add(a);
                    }
                }
            }

            m_SceneObjectTags = tags.ToArray();

            m_lastCgSceneObjCount = allObject.Count;
        }

        private void OnGUI()
        {
            GUI.skin = CGPrefabEditorWindow.GUIStyle;

            //GUI.depth = 0;

            if(!CGUnityWindowManager.GSetting.NoHeader)
                m_DragWindowRect = GUI.Window(998, m_DragWindowRect, WindowFunc, "CG场景工具箱");

            GUI.skin = null;
        }
    }
}