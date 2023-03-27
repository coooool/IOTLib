using IOTLib;
using IOTLib.IDESystem.Handles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IOTLib
{
    /// <summary>
    /// 拖放鼠标处理
    /// </summary>
    internal class CGHandleDragMouse : MonoBehaviour
    {
        public static bool MouseInDragWindow { get; private set; }

        private static WeakReference<ExportCGPrefab> m_SetObject = new WeakReference<ExportCGPrefab>(null);

        public static ExportCGPrefab SetObject
        {
            get
            {
                if (m_SetObject.TryGetTarget(out var obj)) return obj;
                return null;
            }
            set
            {
                m_SetObject.SetTarget(value);
            }
        }

        private List<IDragArea> dragAreas = new List<IDragArea>();
        
        /// <summary>
        /// 创建一个新的实例
        /// </summary>
        /// <param name="world_pos"></param>
        void CreateNewCGPrefab(Vector3 world_pos)
        {
            var newGo = CGResources.InstanceCGPrefab(world_pos, SetObject);
            
            newGo.AddComponent<DragGameObject>();
            CGPrefabEditorWindow.SelectActiveGameObject = newGo;

            CameraHandle.F(newGo, null, "D");
        }

        /// <summary>
        /// 注册一个放置事件
        /// </summary>
        /// <param name="unityEvent"></param>
        public void RegisterDragPerformEvent(IDragArea area)
        {
            if(dragAreas.Contains(area)) return;
            dragAreas.Add(area);
        }

        /// <summary>
        /// 删除一个区域
        /// </summary>
        /// <param name="area"></param>
        public void RemoveDragPerformEvent(IDragArea area)
        {
            dragAreas.Remove(area);
        }

        /// <summary>
        /// 测试鼠标是否在任意区域内
        /// </summary>
        /// <returns></returns>
        bool MouseInArea(out IDragArea area)
        {
            area = null;

            foreach(var a in dragAreas)
            {
                if (CGGUIUtility.TestMouseInWindow(a.DragArea))
                {
                    area = a;
                    return true;
                }
            }

            return false;
        }

        void Update()
        {
            if (MouseInArea(out var _))
            {
                MouseInDragWindow = true;
            }
            else
            {
                MouseInDragWindow = false;
            }
        }

        void OnDestroy()
        {
            MouseInDragWindow = false;
        }

        void OnGUI()
        {
            bool inGround;

            if (SetObject == null) 
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, Camera.main.farClipPlane))
            {
                inGround = true;
            }
            else
            {
                inGround = false;
                GUI.color = Color.red;
            }

            if (Event.current.type == EventType.MouseUp)
            {
                if (SetObject != null && Event.current.button == 0)
                {
                    if (MouseInArea(out var area) == false)
                    {
                        if (inGround)
                        {
                            CreateNewCGPrefab(hitInfo.point);
                            Event.current.Use();
                        }
                    }
                    else
                    {
                        area.OnDragPerform(SetObject);
                    }
                }

                SetObject = null;
                return;
            }

            var ico = SetObject.GetIco();

            var icoSize = new Vector2(ico.width, ico.height) * 1.2f;

            //var oldDepth = GUI.depth;
            GUI.depth = 0;
            GUI.DrawTexture(new Rect(Input.mousePosition.x - icoSize.x / 2, Screen.height - (Input.mousePosition.y + icoSize.y / 2), icoSize.x, icoSize.y), ico);
            //GUI.depth = oldDepth;

            GUI.color = Color.white;

            if (!inGround)
            {
                var labelRect = new Rect(Input.mousePosition.x + 30, Screen.height - Input.mousePosition.y, 120, 22);
                GUI.Label(labelRect, "请移到地面内", "box");
            }
        }
    }
}