using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

namespace IOTLib
{
    /// <summary>
    /// 围绕一个物体进行旋转观看
    /// </summary>
    public class PlayerModelObserve : FlowState
    {
        public const string TriggerEventName = "围绕观察模式";

        // 状态改变，True为进入，Exit为False
        public readonly static UnityEvent<bool> StateChangedEvent = new UnityEvent<bool>();

        private UnityEngine.Camera camera;
        private GameObject ObserverTarget;

        public PlayerModelObserve():base("围绕观察物体模式")
        {
            camera = UnityEngine.Camera.main;
        }

        async UniTaskVoid UpdateMouse(CancellationToken cancellation)
        {
            var mouseScrollWheel = Vector3.zero;

            while (true)
            {
                cancellation.ThrowIfCancellationRequested();

                if (Application.isFocused && !CameraHandle.IsPointerOverGameObject)
                {  
                    var oldPos = camera.transform.position;
                    var oldRotation = camera.transform.rotation;

                    // 鼠标不在UI上，后续需要加入别的输入器
                    if (!CameraHandle.IsPointerHoverGameObject)
                    {
                        mouseScrollWheel.z = Input.GetAxis("Mouse ScrollWheel");
                        camera.transform.Translate(mouseScrollWheel);
                    }

                    if (Input.GetMouseButton(0))
                    {
                        float rotationAroundYAxis = Input.GetAxis("Mouse X") * CameraControlSetting.Setting.boost;
                        float rotationAroundXAxis = -Input.GetAxis("Mouse Y") * CameraControlSetting.Setting.boost;

                        camera.transform.RotateAround(ObserverTarget.transform.position, camera.transform.right, rotationAroundXAxis);
                        camera.transform.RotateAround(ObserverTarget.transform.position, Vector3.up, rotationAroundYAxis);
                    }

                    // 计算碰撞
                    var easyOverlapTest = CameraPhysics.LerpOverlapTest(camera);

                    if (easyOverlapTest)
                    {
                        camera.transform.rotation = oldRotation;
                        camera.transform.position = oldPos;
                    }
                }

                await UniTask.Yield();
            }
        }

        public override UniTask Enter(IFlow flow)
        {
            ObserverTarget = flow.Vars.Get<GameObject>("Target");

            StateChangedEvent?.Invoke(true);

            UniTask.Void(UpdateMouse, DestroyOrExitStateCancelToken);

            return base.Enter(flow);
        }

        public override UniTask Exit(IFlow flow)
        {
            StateChangedEvent?.Invoke(false);

            return base.Exit(flow);
        }
    }
}