using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOTLib;
using System.Threading;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace IOTLib
{
    /// <summary>
    /// 操作模式A
    /// </summary>
    public class PlayerModelA : FlowState
    {
        public const string TriggerEventName = "FPS默认模式";

        // 状态改变，True为进入，Exit为False
        public readonly static UnityEvent<bool> StateChangedEvent = new UnityEvent<bool>();

        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        // 碰撞体测试
        private Collider[] m_PlayerhitCollider = new Collider[2];
        private RaycastHit[] m_PlayerRaycastHit = new RaycastHit[2];

        private UnityEngine.Camera mainCamera;

        public PlayerModelA() : base("播放模式A")
        {
        }

        void InitStartPos()
        {
            //初始化组件
            m_TargetCameraState.SetFromTransform(mainCamera.transform);
            m_InterpolatingCameraState.SetFromTransform(m_TargetCameraState);
        }

        /// <summary>
        /// 获取控制方向
        /// </summary>
        /// <returns></returns>
        Vector3 GetInputTranslationDirection()
        {
            var PointerOverGameObject = CameraHandle.IsPointerOverGameObject || CGHandleDragMouse.MouseInDragWindow;

            Vector3 direction = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
            else if (!PointerOverGameObject && Input.GetMouseButton(2))
            {
                if (CameraControlSetting.Setting.HasCameraControlMethod(ModelControlTypeEnum.Translate))
                {
                    var mouseMovement = new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) / 2;
                    var mouseSensitivityFactor = CameraControlSetting.Setting.mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                    direction = mouseMovement * mouseSensitivityFactor;
                }
            }
            else if (!PointerOverGameObject && Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                if (CameraControlSetting.Setting.HasCameraControlMethod(ModelControlTypeEnum.Scale))
                {
                    if (Input.mouseScrollDelta.y > 0.00)
                    {
                        direction += Vector3.forward * 7;
                    }
                    else
                    {
                        direction += Vector3.back * 7;
                    }
                }
            }

            return direction;
        }

        /// <summary>
        /// 碰撞测试
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool OverlapTest(ref Vector3 direction)
        {
            var result = false;

            var forward = mainCamera.transform.forward;
            var right = mainCamera.transform.right;
            var up = mainCamera.transform.up;
            
            if (Physics.RaycastNonAlloc(mainCamera.transform.position, forward, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.z > 0)
                {
                    result = true;
                    direction.z = 0;
                }
            }
            else if (Physics.RaycastNonAlloc(mainCamera.transform.position, -forward, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.z < 0)
                {
                    result = true;
                    direction.z = 0;
                }
            }

            if (Physics.RaycastNonAlloc(mainCamera.transform.position, right, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.x > 0) { direction.x = 0; result = true; }
            }
            else if (Physics.RaycastNonAlloc(mainCamera.transform.position, -right, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.x < 0) { direction.x = 0; result = true; }
            }

            if (Physics.RaycastNonAlloc(mainCamera.transform.position, up, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {   
                if (direction.y > 0) { direction.y = 0; result = true; }
            }
            else if (Physics.RaycastNonAlloc(mainCamera.transform.position, -up, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.y < 0) { direction.y = 0; result = true; }
            }

            return result;
        }

        bool LerpOverlapTest(Vector3 newPos)
        {
            int num = Physics.OverlapSphereNonAlloc(newPos, CameraControlSetting.Setting.m_PlayerSphereRadius, m_PlayerhitCollider, CameraControlSetting.Setting.m_LayerMask);

            if (num > 0)
            {
                //var closetPoint = m_PlayerhitCollider[0].ClosestPointOnBounds(camera.transform.position);

                //Debug.Log(m_PlayerhitCollider[0].gameObject);
                //var collider = m_PlayerhitCollider[0];
                //var closestPoint = collider.ClosestPointOnBounds(mainCamera.transform.position);

                return true;
            }

            return false;
        }

        void Move()
        {
            if (!CameraHandle.IsPointerHoverGameObject && Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (!CameraHandle.IsPointerOverGameObject && CameraControlSetting.Setting.HasCameraControlMethod(ModelControlTypeEnum.Rotate))
            {
                if (Input.GetMouseButtonDown(1))
                {
                    //Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    if (Input.GetMouseButtonUp(1))
                    {
                        //Cursor.visible = true;
                        //Cursor.lockState = CursorLockMode.None;
                    }

                    if (Input.GetMouseButton(1))
                    {
                        var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (CameraControlSetting.Setting.invertY ? 1 : -1));

                        var mouseSensitivityFactor = CameraControlSetting.Setting.mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                        m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                        m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;

                        // 限制90度
                        m_TargetCameraState.pitch = Mathf.Clamp(m_TargetCameraState.pitch, -50, 90);
                    }
                }
            }

            var translation = GetInputTranslationDirection() * Time.deltaTime;

            translation *= Mathf.Pow(1.65f, CameraControlSetting.Setting.boost);

            //仿游戏加速移动
            if (Input.GetKey(KeyCode.LeftShift))
            {
                translation *= 2.0f;
            }

            // 使用动态因子
            if (CameraControlSetting.Setting.UseDynamicBoost)
            {
                var forwardRaycastCount = Physics.RaycastNonAlloc(
                    mainCamera.transform.position, 
                    Camera.main.transform.forward, 
                    m_PlayerRaycastHit, 
                    Mathf.Infinity,
                    CameraControlSetting.Setting.m_LayerMask
                    );
                if (forwardRaycastCount > 0)
                {
                    translation *= CameraControlSetting.Setting.DynamicBoostCurve.Evaluate(Vector3.Distance(mainCamera.transform.position, m_PlayerRaycastHit[0].point));
                }
            }

            // 计算碰撞
            var easyOverlapTest = OverlapTest(ref translation);

            m_TargetCameraState.Translate(translation);

            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / CameraControlSetting.Setting.positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / CameraControlSetting.Setting.rotationLerpTime) * Time.deltaTime);

            var oldPos = Vector3.zero;
            if (easyOverlapTest == false)
            {
                oldPos = new Vector3(m_InterpolatingCameraState.x, m_InterpolatingCameraState.y, m_InterpolatingCameraState.z);
            }

            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

            if (!easyOverlapTest)
            {
                if (LerpOverlapTest(m_InterpolatingCameraState.XYZ()))
                {
                    m_InterpolatingCameraState.SetPos(oldPos);
                    m_TargetCameraState.SetPos(oldPos);
                }
            }

            m_InterpolatingCameraState.UpdateTransform(mainCamera.transform);
        }

        async UniTaskVoid UpdateCamera(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                cancellation.ThrowIfCancellationRequested();

                Move();

                await UniTask.Yield();
            }
        }

        public override UniTask Enter(IFlow flow)
        {
            if(!flow.Vars.IsDefined("NO_RESET"))
            {
                CameraControlSetting.Setting.ControlMethod = ModelControlTypeEnum.All;
            }

            mainCamera = UnityEngine.Camera.main;
            InitStartPos();

            StateChangedEvent?.Invoke(true);
            
            UniTask.Void(UpdateCamera, DestroyOrExitStateCancelToken);

            return base.Enter(flow);
        }

        public override UniTask Exit(IFlow flow)
        {
            StateChangedEvent?.Invoke(false);

            return base.Exit(flow);
        }

        public override UniTask Update(IFlow flow)
        {
            return base.Update(flow);
        }
    }
}