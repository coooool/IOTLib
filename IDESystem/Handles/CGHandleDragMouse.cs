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
    /// �Ϸ���괦��
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
                // ��ǰ�������ק������ȡ��
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
        /// ����һ���µ�ʵ��
        /// </summary>
        /// <param name="world_pos"></param>
        void CreateNewCGPrefab(Vector3 world_pos)
        {
            var newGo = CGResources.InstanceCGPrefab(world_pos, SetObject);
           
            // ugui��Դ���þ۽���
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
                // UGUIֱ��ʹ������������Ϊ��С
                newGo.transform.localScale = new Vector3(480, 320, 0);
                newGo.GetComponent<ExportCGPrefab>();
                newGo.AddComponent<DragGameObject2D>();
            }

            CGPrefabEditorWindow.SelectActiveGameObject = newGo;

            // Ĭ�Ϸ���һ���¼�����Ϊ��ǰ�༭���Ǵ򿪵�
            newGo.SendMessage(
                nameof(IDEStateChangedEvent.OnSceneEditorStateNotify), 
                true, 
                SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// ע��һ�������¼�
        /// </summary>
        /// <param name="unityEvent"></param>
        public void RegisterDragPerformEvent(IDragArea area)
        {
            if(dragAreas.Contains(area)) return;
            dragAreas.Add(area);
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        /// <param name="area"></param>
        public void RemoveDragPerformEvent(IDragArea area)
        {
            dragAreas.Remove(area);
        }

        /// <summary>
        /// ��������Ƿ�������������
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
                GUI.Label(labelRect, "���Ƶ�������", "box");
            }
        }
    }
}