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
    /// Χ��һ�����������ת�ۿ�
    /// </summary>
    public class PlayerModelObserve : FlowState
    {
        public const string TriggerEventName = "Χ�ƹ۲�ģʽ";

        // ״̬�ı䣬TrueΪ���룬ExitΪFalse
        public readonly static UnityEvent<bool> StateChangedEvent = new UnityEvent<bool>();

        private Camera camera;
        private GameObject ObserverTarget;

        private Vector3 targetPos;
        //private Vector3 cameraOffset;

        public PlayerModelObserve():base("Χ�ƹ۲�����ģʽ")
        {
            camera = Camera.main;
        }

        async UniTaskVoid UpdateMouse(CancellationToken cancellation)
        {
            Vector3 mouseScrollWheel;

            camera = Camera.main;

            while (true)
            {
                cancellation.ThrowIfCancellationRequested();

                mouseScrollWheel = Vector3.zero;

                if (!IOLockState.CGEditorLock())
                {  
                    var oldPos = camera.transform.position;
                    var oldRotation = camera.transform.rotation;

                    // ��겻��UI�ϣ�������Ҫ������������

                    if (Input.GetMouseButton(1))
                    {
                        var mouseMovement = new Vector2(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")) / 2;
                        var mouseSensitivityFactor = CameraControlSetting.Setting.mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                        Vector3 direction = mouseMovement * mouseSensitivityFactor;
                        var dictory = direction * Time.deltaTime;
                        dictory *= Mathf.Pow(1.65f, CameraControlSetting.Setting.boost);
                        camera.transform.Translate(dictory);

                        //cameraOffset += dictory;
                    }

                    //if (!IOLockState.IsPointerHoverGameObject)
                    //{ 
                        mouseScrollWheel.z = Input.GetAxis("Mouse ScrollWheel") * CameraControlSetting.Setting.boost * 6;
                        camera.transform.Translate(mouseScrollWheel);
                    //}

                    if (Input.GetMouseButton(0))
                    {
                        float rotationAroundYAxis = Input.GetAxis("Mouse X") * CameraControlSetting.Setting.boost;
                        float rotationAroundXAxis = -Input.GetAxis("Mouse Y") * CameraControlSetting.Setting.boost;

                        camera.transform.RotateAround(targetPos, camera.transform.right, rotationAroundXAxis);
                        camera.transform.RotateAround(targetPos, Vector3.up, rotationAroundYAxis);
                    }

                    // ������ײ
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
            targetPos = ObserverTarget.transform.position;

            StateChangedEvent?.Invoke(true);

            if (flow.Vars.TryGet<Action>("COMPLETE", out var Complete))
            {
                Complete?.Invoke();
            }

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