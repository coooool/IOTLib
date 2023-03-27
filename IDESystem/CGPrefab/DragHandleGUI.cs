using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static IOTLib.DragGameObject;

namespace IOTLib.IDESystem
{
    /// <summary>
    /// 一个拖放处理GUI
    /// </summary>
    public class DragHandleGUI
    {
        public Vector3 position { get; set; }

        public GameObject parent { get; set; }

        public bool YAxisToGround { get; set; }

        private Rect AXIS_X_RECT;
        private Rect AXIS_Y_RECT;
        private Rect AXIS_Z_RECT;
        private Rect AXIS_CENTER_RECT;

        private DragGameObject.DragHandleActionEnum m_DragMethod = DragHandleActionEnum.None;

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
                    case DragHandleActionEnum.Z:
                        OnMouseDown();
                        break;
                }

                m_DragMethod = value;
            }
        }

        private Vector3 MDLastClickCenterWorldPos;
        private Vector3 m_MoveAxis = Vector3.up;

        public DragHandleGUI()
        {

        }

        private void OnMouseDown()
        {
            // 记录初始位置
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(position).z));
            MDLastClickCenterWorldPos = position - mouseWorldPos;
        }

        private void OnMouseUp()
        {
            // 完成拖拽操作
            DragMethod = DragHandleActionEnum.None;

            if(parent != null)
                parent.SendMessage("OnDragComplete", SendMessageOptions.DontRequireReceiver);

            if (YAxisToGround)
            {
                position = PhysicsFunc.CalculateGroundPosition(position);
            }
        }

        Vector3 GetDirection(int type)
        {
            var camera_eulerAngle = Camera.main.transform.transform.eulerAngles;
            switch (type)
            {
                case 0:
                    camera_eulerAngle.y = 90;
                    camera_eulerAngle.x = 0;
                    break;

                case 1:
                    camera_eulerAngle.x = -90;
                    camera_eulerAngle.y = 0;
                    break;

                case 2:
                    camera_eulerAngle.x = 0;
                    break;
            }

            var fixRoration = Quaternion.Euler(camera_eulerAngle);
            var cameraRotaion = Matrix4x4.TRS(Camera.main.transform.position, fixRoration, Vector3.one);

            Vector3 forward = cameraRotaion.GetColumn(2);

            return forward;
        }

        void UpdateDragPos()
        {
            // 将鼠标位置转换成世界坐标系中的位置
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
                Camera.main.WorldToScreenPoint(position).z));

            var MDCurrentClickCenterWorldPos = position - mousePosition;

            if (MDLastClickCenterWorldPos == MDCurrentClickCenterWorldPos)
                return;

            switch (DragMethod)
            {
                case DragHandleActionEnum.All:
                    var targetPosition = mousePosition + MDLastClickCenterWorldPos;
                    position = targetPosition;
                    break;
                case DragHandleActionEnum.X:
                    m_MoveAxis = Camera.main.transform.TransformDirection(Vector3.right);
                    break;
                case DragHandleActionEnum.Y:
                    m_MoveAxis = Camera.main.transform.TransformDirection(Vector3.up);
                    break;
                case DragHandleActionEnum.Z:
                    m_MoveAxis = GetDirection(2);
                    break;

                default:
                    return;
            }

            if (DragMethod != DragHandleActionEnum.All)
            {
                // 根据轴的方向计算出移动的距离
                var dotValue = MDLastClickCenterWorldPos - MDCurrentClickCenterWorldPos;
                float distance = Vector3.Dot(dotValue, m_MoveAxis);

                if (DragMethod == DragHandleActionEnum.Z)
                    position += m_MoveAxis * dotValue.y;
                else
                    position += m_MoveAxis * distance;
            }

            if(YAxisToGround)
            {
                position = PhysicsFunc.CalculateGroundPosition(position);
            }

            if(parent != null)
                parent.SendMessage("OnDragUpdate", SendMessageOptions.DontRequireReceiver);
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

                    Event.current.Use();
                }
                else if (AXIS_Y_RECT.Contains(Event.current.mousePosition))
                {
                    DragMethod = DragHandleActionEnum.Y;

                    Event.current.Use();
                }
                else if (AXIS_Z_RECT.Contains(Event.current.mousePosition))
                {
                    DragMethod = DragHandleActionEnum.Z;

                    Event.current.Use();
                }
                else if (AXIS_CENTER_RECT.Contains(Event.current.mousePosition))
                {
                    if (Event.current.clickCount == 2)
                    {
                        CameraHandle.F(position, null);
                    }

                    DragMethod = DragHandleActionEnum.All;

                    OnMouseDown();

                    Event.current.Use();
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                UpdateDragPos();
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                OnMouseUp();
            }   
        }

        /// <summary>
        /// 刷新位置
        /// </summary>
        public void OnGUI()
        {
            var OldGUIMatrix = GUI.matrix;
            var selfPosition = position;

            if (!Camera.main.PointInCameraView(selfPosition))
                return;

            var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, selfPosition);

            GUI.depth = Mathf.CeilToInt(screenPos.y);

            screenPos.y = Screen.height - screenPos.y;

            GUI.matrix = Matrix4x4.TRS(new Vector3(screenPos.x, screenPos.y, 0), Quaternion.identity, Vector3.one);

            Rect centerAxis = new Rect(screenPos.x - DragGameObject.TEXTURE_AXIS_CENTER.width / 2, screenPos.y - DragGameObject.TEXTURE_AXIS_CENTER.height / 2, TEXTURE_AXIS_CENTER.width, TEXTURE_AXIS_CENTER.height);
            AXIS_CENTER_RECT = centerAxis;

            var xAxisPos = new Rect(centerAxis.width / 2, -centerAxis.height / 2, DragGameObject.TEXTURE_AXIS_X.width, DragGameObject.TEXTURE_AXIS_X.height);
            AXIS_X_RECT.Set(centerAxis.center.x + xAxisPos.x, centerAxis.center.y + xAxisPos.y, xAxisPos.width, xAxisPos.height);
            GUI.DrawTexture(xAxisPos, DragGameObject.TEXTURE_AXIS_X);

            var yAxisPos = new Rect(-DragGameObject.TEXTURE_AXIS_Y.width / 2, -(DragGameObject.TEXTURE_AXIS_Y.height + DragGameObject.TEXTURE_AXIS_CENTER.height / 2), TEXTURE_AXIS_Y.width, TEXTURE_AXIS_Y.height);
            AXIS_Y_RECT.Set(centerAxis.center.x + yAxisPos.x, centerAxis.center.y + yAxisPos.y, yAxisPos.width, yAxisPos.height);
            GUI.DrawTexture(yAxisPos, DragGameObject.TEXTURE_AXIS_Y);

            var zAxisMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
            GUI.matrix = GUI.matrix * zAxisMatrix;

            var zAxisPos = new Rect(yAxisPos.x, -(DragGameObject.TEXTURE_AXIS_Z.height + DragGameObject.TEXTURE_AXIS_CENTER.height / 2), DragGameObject.TEXTURE_AXIS_Z.width, TEXTURE_AXIS_Z.height);
            AXIS_Z_RECT.Set(centerAxis.center.x + zAxisPos.x, centerAxis.center.y - zAxisPos.y - DragGameObject.TEXTURE_AXIS_Z.height, DragGameObject.TEXTURE_AXIS_Z.width, TEXTURE_AXIS_Z.height);
            GUI.DrawTexture(zAxisPos, DragGameObject.TEXTURE_AXIS_Z);

            GUI.matrix = OldGUIMatrix;

            GUI.DrawTexture(centerAxis, DragGameObject.TEXTURE_AXIS_CENTER);

            UseDragEvent();
        }
    }
}
