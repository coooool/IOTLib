using IOTLib;
using IOTLib.IDESystem.Handles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IOTLib
{
    public class CGPrefabWindow : MonoBehaviour, IDragArea
    {
        private const int WINDOW_WIDTH = 380;

        private Rect m_DragWindowRect;
        private EasyListPool<CGPrefabGroup> m_Prefabs;
        private Vector2 m_ScrollView;
        private string m_SearchText;

        public Rect DragArea => m_DragWindowRect;

        private void OnEnable()
        {
            m_DragWindowRect.Set(20, 80, WINDOW_WIDTH, Mathf.RoundToInt(Screen.height * 0.60f));
        }

        private void Start()
        {
            GetComponent<CGHandleDragMouse>().RegisterDragPerformEvent(this);

            m_Prefabs = new EasyListPool<CGPrefabGroup> ();
           
            var allItem = Resources.LoadAll<ExportCGPrefab>("");

            var group = allItem.GroupBy(p => p.GetSafeCgType());

            foreach(var a in group)
            {
                m_Prefabs.Add(new CGPrefabGroup() { GroupName = a.Key, Prefabs = a.ToArray()});
            }
        }

        private void OnDestroy()
        {
            m_Prefabs.Dispose();
        }

        void WindowFunc(int windowid)
        {
            GUI.depth = 0;

            m_ScrollView = GUILayout.BeginScrollView(m_ScrollView);

            m_SearchText = GUILayout.TextField(m_SearchText, GUILayout.Height(25));

            bool filterData = string.IsNullOrEmpty(m_SearchText) ? false : true;

            foreach (var g in m_Prefabs)
            {
                GUILayout.BeginHorizontal("CGPrefabGroup");
                g.Toggle = GUILayout.Toggle(filterData ? true : g.Toggle, g.GroupName);
                GUILayout.EndHorizontal();

                if (!filterData && !g.Toggle) continue;

                foreach (var item in g.Prefabs)
                {
                    if (filterData)
                    {
                        if (!item.GetCgName().Contains(m_SearchText)) continue;
                    }
                    GUILayout.BeginHorizontal("Box", GUILayout.ExpandWidth(true));

                    if (CGGUIUtility.AutoLayoutDragButton(item.GetIco(), "CGPrefab", GUILayout.Width(54), GUILayout.MinHeight(54)))
                    {
                        CGHandleDragMouse.SetObject = item;
                    }

                    GUILayout.BeginVertical();
                    GUILayout.Label(item.GetCgName());
                    GUI.contentColor = Color.gray;
                    GUILayout.Label(item.GetCgDescription());
                    GUI.contentColor = Color.white;
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();

            if(Event.current.mousePosition.y < 26)
                GUI.DragWindow(new Rect(0,0, Screen.width, Screen.height));
        }

        private void OnGUI()
        {
            GUI.skin = CGPrefabEditorWindow.GUIStyle;

            GUI.depth = 1;

            m_DragWindowRect = GUI.Window(1, m_DragWindowRect, WindowFunc, "CG×ÊÔ´¿â");
               
            GUI.skin = null;
        }

        public void OnDragPerform(ExportCGPrefab target)
        {
            
        }
    }
}