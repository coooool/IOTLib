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
        private static WeakReference<ExportCGPrefab?> m_SetObject = new WeakReference<ExportCGPrefab?>(null);

        private static bool IsUGUIPrefab { get; set; }

        public static ExportCGPrefab SetObject
        {
            get
            {
                if (m_SetObject.TryGetTarget(out var obj))
                    return obj;

                return null;
            }
            set
            {
                // 当前如果有拖拽对象则取消
                if (value == null)
                {
                    IOLockState.OtherIsUse = false;
                }
                else
                {
                    IOLockState.OtherIsUse = true;
                    IsUGUIPrefab = value.m_CgType.ToLower() == "ui" ? true : false;
                }

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
           
            // ugui资源不用聚焦，
            if (!IsUGUIPrefab)
            {
                newGo.AddComponent<DragGameObject>();

                CameraHandle.F(newGo, () =>
                {
                    CameraHelpFunc.ToObserver(newGo);
                }, "D2");
            }
            else
            {
                // UGUI直接使用它的缩放做为大小
                newGo.transform.localScale = new Vector3(480, 320, 0);
                newGo.GetComponent<ExportCGPrefab>();
                newGo.AddComponent<DragGameObject2D>();
            }

            CGPrefabEditorWindow.SelectActiveGameObject = newGo;

            // 默认发送一次事件，因为当前编辑器是打开的
            newGo.SendMessage(
                nameof(IDEStateChangedEvent.OnSceneEditorStateNotify), 
                true, 
                SendMessageOptions.DontRequireReceiver);
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

        bool MouseInArea(IDragArea area)
        {
            if (CGGUIUtility.TestMouseInWindow(area.DragArea))
            {                
                return true;
            }

            return false;
        }

        void Update()
        {
            if (MouseInArea(out var _))
            {
                IOLockState.CGEditorMouseInDragWindow = true;
            }
            else
            {
                IOLockState.CGEditorMouseInDragWindow = false;
            }
        }

        void OnDestroy()
        {
            IOLockState.CGEditorMouseInDragWindow = false;
        }

        void OnGUI()
        {
            bool inGround;

            if (SetObject == null) 
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();

            if (IsUGUIPrefab || Physics.Raycast(ray,out hitInfo, Camera.main.farClipPlane))
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
                        if(IsUGUIPrefab)
                        {
                            CreateNewCGPrefab(Input.mousePosition);
                        }
                        else if (inGround)
                        {
                            CreateNewCGPrefab(hitInfo.point); 
                        }

                        Event.current.Use();
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

            if(!IsUGUIPrefab && !inGround)
            {
                var labelRect = new Rect(Input.mousePosition.x + 30, Screen.height - Input.mousePosition.y, 120, 22);
                GUI.Label(labelRect, "请移到地面内", "box");
            }
        }
    }
}