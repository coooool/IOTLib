using Cysharp.Threading.Tasks;
using IOTLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    [DisallowMultipleComponent]
    public class DragGameObject2D : MonoBehaviour
    {
        #region 坐标轴图片
        private Rect AXIS_X_RECT;
        private Rect AXIS_Y_RECT;
        private Rect AXIS_CENTER_RECT;
        #endregion

        #region DRAG
        public enum DragHandleTypeEnum
        {
            None = 0,
            PreGUI,
            GUI,
            Object,
        }

        public enum DragHandleActionEnum
        {
            None,
            X,
            Y,
            All
        }

        private DragHandleTypeEnum DragHandleType { get; set; } = DragHandleTypeEnum.None;

        private DragHandleActionEnum m_DragMethod = DragHandleActionEnum.All;
        private DragHandleActionEnum DragMethod
        {
            get
            {
                return m_DragMethod;
            }
            set
            {
                switch (value)
                {
                    case DragHandleActionEnum.X:
                    case DragHandleActionEnum.Y:
                        OnMouseDown();
                        break;
                }

                m_DragMethod = value;
            }
        }

        private Vector2 MDLastClickCenterWorldPos;
        #endregion

        private ExportCGPrefab target;

        private Vector2 m_Origin2DPos;

        // UI的4个点世界坐标
        //Vector3[] tempCorners = new Vector3[4];

        internal Vector2 m_2DPos = Vector2.zero;

        void Start()
        {
            target = GetComponent<ExportCGPrefab>();

            UpdateEditPos();
        }

        internal void UpdateEditPos()
        {
            if (target.UIRecttransform != null)
                m_2DPos = target.UIRecttransform.position;
            else
                m_2DPos = transform.position;
        }

        private void OnMouseDown()
        {
            IOLockState.OtherIsUse = true;

            if (DragHandleType == DragHandleTypeEnum.GUI)
            {
                return;
            }

            CGPrefabEditorWindow.SelectActiveGameObject = gameObject;

            // 记录初始位置
            MDLastClickCenterWorldPos = Input.mousePosition;
            m_Origin2DPos = m_2DPos;

            // 改变操作操作为默认
            CameraHelpFunc.ToAState();
        }

        void OnDestroy()
        {
            if (CGPrefabEditorWindow.SelectActiveGameObject == gameObject)
                CGPrefabEditorWindow.SelectActiveGameObject = null;
        }

        void UpdateDragPos()
        {
            // 将鼠标位置转换成世界坐标系中的位置
            var mousePosition =new Vector2(Input.mousePosition.x, Input.mousePosition.y);       
            var gap_pos = mousePosition - MDLastClickCenterWorldPos;
            var target_pos = m_Origin2DPos + gap_pos;

            var pos = m_2DPos;

            switch (DragMethod)
            {
                case DragHandleActionEnum.All:
                    pos = target_pos;
                    break;
                case DragHandleActionEnum.X:
                    pos.x = target_pos.x;
                    break;
                case DragHandleActionEnum.Y:
                    pos.y = target_pos.y;
                    break;

                default:
                    return;
            }

            m_2DPos = pos;
            target.transform.position = m_2DPos;

            TriggerDragUpdateEvent(this, m_2DPos);
        }

        private void OnMouseDrag()
        {
            if (DragHandleType == DragHandleTypeEnum.Object)
                UpdateDragPos();
        }

        private void Update()
        {
            if (DragHandleType == DragHandleTypeEnum.GUI)
                UpdateDragPos();
        }

        private void OnMouseUp()
        {
            IOLockState.OtherIsUse = false;

            // 完成拖拽操作
            DragHandleType = DragHandleTypeEnum.None;
            DragMethod = DragHandleActionEnum.None;

            SendMessage("OnDragComplete", SendMessageOptions.DontRequireReceiver);
        }

        void UseDragEvent()
        {
            if (Event.current.button != 0)
            {
                DragMethod = DragHandleActionEnum.None;
                return;
            }

            if (Event.current.type == EventType.MouseDown)
            {
                if (AXIS_X_RECT.Contains(Event.current.mousePosition))
                {
                    DragMethod = DragHandleActionEnum.X;
                    DragHandleType = DragHandleTypeEnum.GUI;

                    CGPrefabEditorWindow.SelectActiveTransform = transform;
                    Event.current.Use();
                }
                else if (AXIS_Y_RECT.Contains(Event.current.mousePosition))
                {
                    DragMethod = DragHandleActionEnum.Y;
                    DragHandleType = DragHandleTypeEnum.GUI;

                    CGPrefabEditorWindow.SelectActiveTransform = transform;
                    Event.current.Use();
                }
                else if (AXIS_CENTER_RECT.Contains(Event.current.mousePosition))
                {  
                    CGPrefabEditorWindow.SelectActiveTransform = transform;

                    DragMethod = DragHandleActionEnum.All;
                    DragHandleType = DragHandleTypeEnum.PreGUI;

                    OnMouseDown();

                    Event.current.Use();
                }
                else
                {
                    DragMethod = DragHandleActionEnum.All;
                    DragHandleType = DragHandleTypeEnum.Object;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && DragHandleType == DragHandleTypeEnum.PreGUI)
            {
                DragHandleType = DragHandleTypeEnum.GUI;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                OnMouseUp();
            }
        }

        private void OnGUI()
        {
            if(target.UIRecttransform != null)
            {
                //target.UIRecttransform.GetWorldCorners(tempCorners);

                var pos = m_2DPos;
                pos.y = Screen.height - pos.y;
                var boxRect = new Rect(pos, target.UIRecttransform.sizeDelta);
                //boxRect.xMin = tempCorners[0].x;
                //boxRect.xMax = tempCorners[3].x;

                //boxRect.yMin = Screen.height - tempCorners[0].y;
                //boxRect.yMax = Screen.height - tempCorners[1].y;

                GUI.Box(boxRect, target.gameObject.name);
                //var pos = target.UIRecttransform.anchoredPosition;
                //pos.y = Screen.height - pos.y;
                //var boxRect = new Rect(pos, target.UIRecttransform.sizeDelta);
                //// 绘制背景
                //GUI.Box(boxRect, target.gameObject.name);
            }

            var OldGUIMatrix = GUI.matrix;
     
            var screenPos = m_2DPos; //RectTransformUtility.WorldToScreenPoint(Camera.main, selfPosition);

            GUI.depth = Mathf.CeilToInt(screenPos.y);

            screenPos.y = Screen.height - screenPos.y;

            GUI.matrix = Matrix4x4.TRS(new Vector3(screenPos.x, screenPos.y, 0), Quaternion.identity, Vector3.one);

            Rect centerAxis = new Rect(screenPos.x - DragGameObject.TEXTURE_AXIS_CENTER.width / 2, screenPos.y - DragGameObject.TEXTURE_AXIS_CENTER.height / 2, DragGameObject.TEXTURE_AXIS_CENTER.width, DragGameObject.TEXTURE_AXIS_CENTER.height);
            AXIS_CENTER_RECT = centerAxis;

            var xAxisPos = new Rect(centerAxis.width / 2, -centerAxis.height / 2, DragGameObject.TEXTURE_AXIS_X.width, DragGameObject.TEXTURE_AXIS_X.height);
            AXIS_X_RECT.Set(centerAxis.center.x + xAxisPos.x, centerAxis.center.y + xAxisPos.y, xAxisPos.width, xAxisPos.height);
            GUI.DrawTexture(xAxisPos, DragGameObject.TEXTURE_AXIS_X);

            var yAxisPos = new Rect(-DragGameObject.TEXTURE_AXIS_Y.width / 2, -(DragGameObject.TEXTURE_AXIS_Y.height + DragGameObject.TEXTURE_AXIS_CENTER.height / 2), DragGameObject.TEXTURE_AXIS_Y.width, DragGameObject.TEXTURE_AXIS_Y.height);
            AXIS_Y_RECT.Set(centerAxis.center.x + yAxisPos.x, centerAxis.center.y + yAxisPos.y, yAxisPos.width, yAxisPos.height);
            GUI.DrawTexture(yAxisPos, DragGameObject.TEXTURE_AXIS_Y);

            GUI.matrix = OldGUIMatrix;

            GUI.DrawTexture(centerAxis, DragGameObject.TEXTURE_AXIS_CENTER);

            UseDragEvent();
        }


        /// <summary>
        /// 触发拖动更新事件
        /// </summary>
        /// <param name="target"></param>
        public static void TriggerDragUpdateEvent(MonoBehaviour target, Vector2 pos)
        {
            target.SendMessage("OnDragUpdate", pos, SendMessageOptions.DontRequireReceiver);
        }
    }
}