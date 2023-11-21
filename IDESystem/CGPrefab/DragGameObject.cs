using Cysharp.Threading.Tasks;
using IOTLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    [DisallowMultipleComponent]
    public class DragGameObject : MonoBehaviour
    {
        #region 坐标轴图片
        private static Texture2D m_texture_x;
        private static Texture2D m_texture_y;
        private static Texture2D m_texture_z;
        private static Texture2D m_texture_center;

        private Rect AXIS_X_RECT;
        private Rect AXIS_Y_RECT;
        private Rect AXIS_Z_RECT;
        private Rect AXIS_CENTER_RECT;

        internal static Texture2D TEXTURE_AXIS_X
        {
            get
            {
                if (m_texture_x == null)
                {
                    m_texture_x = Resources.Load<Texture2D>("DOS/axis_img_x");
                }

                return m_texture_x;
            }
        }

        internal static Texture2D TEXTURE_AXIS_Y
        {
            get
            {
                if (m_texture_y == null)
                {
                    m_texture_y = Resources.Load<Texture2D>("DOS/axis_img_y");
                }

                return m_texture_y;
            }
        }

        internal static Texture2D TEXTURE_AXIS_Z
        {
            get
            {
                if (m_texture_z == null)
                {
                    m_texture_z = Resources.Load<Texture2D>("DOS/axis_img_z");
                }

                return m_texture_z;
            }
        }
        internal static Texture2D TEXTURE_AXIS_CENTER
        {
            get
            {
                if (m_texture_center == null)
                {
                    m_texture_center = Resources.Load<Texture2D>("DOS/axis_img_center");
                }

                return m_texture_center;
            }
        }
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
            Z,
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
                    case DragHandleActionEnum.Z:
                        OnMouseDown();
                        break;
                }

                m_DragMethod = value;
            }
        }

        private Vector3 MDLastClickCenterWorldPos;
        private Vector3 m_MoveAxis = Vector3.up;
        // 触发过拖？
        private bool m_TriggerDrag = false;
        #endregion

        private void OnMouseDown()
        {
            CGHandleDragMouse.OtherIsUse = true;

            if (DragHandleType == DragHandleTypeEnum.GUI)
            {
                return;
            }

            // 记录初始位置
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(gameObject.transform.position).z));
            MDLastClickCenterWorldPos = transform.position - mouseWorldPos;

            if(CGPrefabEditorWindow.SelectActiveGameObject != gameObject)
            {
                CameraHandle.F(gameObject, () =>
                {
                    CameraHelpFunc.ToObserver(gameObject);
                }, "D2");
            }

            CGPrefabEditorWindow.SelectActiveGameObject = gameObject;
        }

        void OnDestroy()
        {
            if (CGPrefabEditorWindow.SelectActiveGameObject == gameObject)
                CGPrefabEditorWindow.SelectActiveGameObject = null;
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
            //Vector3 up = cameraRotaion.GetColumn(1);
            //Vector3 right = cameraRotaion.GetColumn(0);
            //Vector3 direction = Vector3.Cross(up, right);

            return forward;
        }

        void UpdateDragPos()
        {
            // 将鼠标位置转换成世界坐标系中的位置
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(gameObject.transform.position).z));
            var MDCurrentClickCenterWorldPos = transform.position - mousePosition;

            if (MDLastClickCenterWorldPos == MDCurrentClickCenterWorldPos)
                return;

            switch (DragMethod)
            {
                case DragHandleActionEnum.All:
                    var targetPosition = mousePosition + MDLastClickCenterWorldPos;
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if(LayerUtility.GetGroundLayer(out var ground_layer))
                    {
                        if (Physics.Raycast(ray, out var hitInfo, Camera.main.farClipPlane, ground_layer))
                        {
                            // 在地面之上
                            targetPosition.y = hitInfo.point.y;
                        }
                    }
                     
                    transform.position = targetPosition;
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
                    transform.position += m_MoveAxis * dotValue.y;
                else
                    transform.position += m_MoveAxis * distance;
            }

            SendMessage("OnDragUpdate", SendMessageOptions.DontRequireReceiver);
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
            CGHandleDragMouse.OtherIsUse = false;

            if (DragMethod == DragHandleActionEnum.All && m_TriggerDrag)
            {
                // 如果点击了圆点则跟踪D号算法，否则只是C算法聚焦
                //var model = DragMethod == DragHandleActionEnum.All ? "D" : "C";

                //UniTask.Void(async () =>
                //{
                //    await UniTask.NextFrame();

                    CameraHandle.F(gameObject, () =>
                    {
                        CameraHelpFunc.ToObserver(gameObject);
                    }, "D2");
                //});

                m_TriggerDrag = false;
            }

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

                    // 暂停Y计算
                    if(gameObject.TryGetComponent<PositionToGroundData>(out var pgd))
                        pgd.enabled= false;

                    CGPrefabEditorWindow.SelectActiveTransform = transform;
                    Event.current.Use();
                }
                else if (AXIS_Z_RECT.Contains(Event.current.mousePosition))
                {
                    DragMethod = DragHandleActionEnum.Z;
                    DragHandleType = DragHandleTypeEnum.GUI;

                    CGPrefabEditorWindow.SelectActiveTransform = transform;
                    Event.current.Use();
                }
                else if (AXIS_CENTER_RECT.Contains(Event.current.mousePosition))
                {
                    //if (Event.current.clickCount == 2)
                    //{
                    //    CGPrefabEditorWindow.SelectActiveTransform = transform;
                    //    CameraHandle.F(gameObject, null, "D2");
                    //}
                    //else if (Event.current.clickCount == 1)
                    //{
                        CGPrefabEditorWindow.SelectActiveTransform = transform;
                    //}

                    DragMethod = DragHandleActionEnum.All;
                    DragHandleType = DragHandleTypeEnum.PreGUI;

                    //if (gameObject.TryGetComponent<PositionToGroundData>(out var pgd))
                    //    pgd.enabled = false;

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
                m_TriggerDrag = true;
                DragHandleType = DragHandleTypeEnum.GUI;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                // 恢复Y轴计算地面
                //if (DragHandleType == DragHandleTypeEnum.GUI && DragMethod == DragHandleActionEnum.Y)
                //if(gameObject.TryGetComponent<PositionToGroundData>(out var pgd))
                //    pgd.enabled = true;

                OnMouseUp();
            }
        }

        private void OnGUI()
        {
            if (CGPrefabEditorWindow.SelectActiveGameObject != gameObject)
                return;

            var OldGUIMatrix = GUI.matrix;
            var selfPosition = transform.position;

            if (!Camera.main.PointInCameraView(selfPosition))
                return;

            var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, selfPosition);

            GUI.depth = Mathf.CeilToInt(screenPos.y);

            screenPos.y = Screen.height - screenPos.y;

            GUI.matrix = Matrix4x4.TRS(new Vector3(screenPos.x, screenPos.y, 0), Quaternion.identity, Vector3.one);

            Rect centerAxis = new Rect(screenPos.x - TEXTURE_AXIS_CENTER.width / 2, screenPos.y - TEXTURE_AXIS_CENTER.height / 2, TEXTURE_AXIS_CENTER.width, TEXTURE_AXIS_CENTER.height);
            AXIS_CENTER_RECT = centerAxis;

            var xAxisPos = new Rect(centerAxis.width / 2, -centerAxis.height / 2, TEXTURE_AXIS_X.width, TEXTURE_AXIS_X.height);
            AXIS_X_RECT.Set(centerAxis.center.x + xAxisPos.x, centerAxis.center.y + xAxisPos.y, xAxisPos.width, xAxisPos.height);
            GUI.DrawTexture(xAxisPos, TEXTURE_AXIS_X);

            var yAxisPos = new Rect(-TEXTURE_AXIS_Y.width / 2, -(TEXTURE_AXIS_Y.height + TEXTURE_AXIS_CENTER.height / 2), TEXTURE_AXIS_Y.width, TEXTURE_AXIS_Y.height);
            AXIS_Y_RECT.Set(centerAxis.center.x + yAxisPos.x, centerAxis.center.y + yAxisPos.y, yAxisPos.width, yAxisPos.height);
            GUI.DrawTexture(yAxisPos, TEXTURE_AXIS_Y);

            var zAxisMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180));
            GUI.matrix = GUI.matrix * zAxisMatrix;

            var zAxisPos = new Rect(yAxisPos.x, -(TEXTURE_AXIS_Z.height + TEXTURE_AXIS_CENTER.height / 2), TEXTURE_AXIS_Z.width, TEXTURE_AXIS_Z.height);
            AXIS_Z_RECT.Set(centerAxis.center.x + zAxisPos.x, centerAxis.center.y - zAxisPos.y - TEXTURE_AXIS_Z.height, TEXTURE_AXIS_Z.width, TEXTURE_AXIS_Z.height);
            GUI.DrawTexture(zAxisPos, TEXTURE_AXIS_Z);

            GUI.matrix = OldGUIMatrix;

            GUI.DrawTexture(centerAxis, TEXTURE_AXIS_CENTER);

            UseDragEvent();
        }
    }
}