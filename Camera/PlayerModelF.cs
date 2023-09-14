using Cysharp.Threading.Tasks;
using DG.Tweening;
using IOTLib.Extend;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace IOTLib
{
    /// <summary>
    /// ����Unity��F�����ۼ�һ������
    /// </summary>
    public class PlayerModelF : FlowState
    {
        // ״̬�ı䣬TrueΪ���룬ExitΪFalse
        public readonly static UnityEvent<bool> StateChangedEvent = new UnityEvent<bool>();

        //private WeakReference<GameObject> LastObserverObject { get; set; } = new WeakReference<GameObject>(null);
        public const string TriggerEventName = "F�ۼ�ģʽ";

        private Transform m_lookAt { get; set; }
        private Vector3? m_lookAtV3 { get; set; }

        private Vector3? LookAt
        {
            get
            {
                if(m_lookAt != null)
                {
                    return m_lookAt.position;
                }
                else if(m_lookAtV3 != null)
                {
                    return m_lookAtV3.Value;
                }

                return null;
            }
        }

        public PlayerModelF():base("F�ۼ�����ģʽ")
        {
        }

        /// <summary>
        /// ��λһ��GameObject
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="focusedObject"></param>
        /// <param name="flow"></param>
        /// <param name="marginPercentage"></param>
        Tween FocusOnGameObject(Camera camera, GameObject focusedObject, IFlow flow, float marginPercentage = 1.1f)
        {
            Bounds bounds = BoundsUtility.GetBoundsWithChildren(focusedObject);
            float virtualsphereRadius = Vector3.Magnitude(bounds.max - bounds.center) * marginPercentage;

            // ��¼ǰ��
            flow.Vars.Set("Forward", focusedObject.transform.forward);
            flow.Vars.Set("WorldLeftDirection", focusedObject.transform.TransformDirection(Vector3.left));

            return FocusOnPosition(camera, bounds, focusedObject.transform.position, flow, virtualsphereRadius);
        }

        // ��ͬ�㷨�ľ۽�
        void FocusStyleA(Camera camera, Sequence seq, Vector3 targetPos, IFlow flow, float virtualsphereRadius)
        {
            float minD = (virtualsphereRadius) / Mathf.Sin(Mathf.Deg2Rad * camera.fieldOfView / 2);

            var direction = (camera.transform.position - targetPos).normalized;
            var newPos = direction * minD + targetPos;

            newPos = CameraPhysics.CalculateCameraBestPoint(newPos);

            #region ����������������
            var rotatioin = Quaternion.LookRotation(targetPos - camera.transform.position);

            var changed = false;
            var _lookAt = LookAt;

            if (_lookAt.HasValue)
            {
                rotatioin = Quaternion.LookRotation(_lookAt.Value - camera.transform.position);

                if (Vector3.Distance(rotatioin.eulerAngles, camera.transform.eulerAngles) > 0.1f)
                {
                    seq.Join(camera.transform.DODynamicLookAt(_lookAt.Value, .65f).SetEase(Ease.InOutQuad));
                    changed = true;
                }
            }
            else
            {
                if (Vector3.Distance(rotatioin.eulerAngles, camera.transform.eulerAngles) > 0.1f)
                {
                    seq.Join(camera.transform.DORotateQuaternion(rotatioin, .65f).SetEase(Ease.InOutQuad));
                    changed = true;
                }
            }

            if (Vector3.Distance(newPos, camera.transform.position) > 0.1f)
            {
                changed = true;
                seq.Join(camera.transform.DOMove(newPos, .65f).SetEase(Ease.InOutQuad));
            }

            if(!changed)
            {
                seq.Complete();
            }
            #endregion
        }

        void FocusStyleB(Camera camera, Bounds? bounds, Sequence seq, Vector3 targetPos, IFlow flow, float virtualsphereRadius)
        {
            Vector3 moveToPos = targetPos;

            if(bounds.HasValue)
                moveToPos.y += bounds.Value.size.y;

            float minD = (virtualsphereRadius) / Mathf.Sin(Mathf.Deg2Rad * camera.fieldOfView / 2);

            var direction = (camera.transform.position - moveToPos).normalized;
            var newPos = direction * minD + moveToPos;
            newPos = CameraPhysics.CalculateCameraBestPoint(newPos);

            #region ����������������
            var rotatioin = Quaternion.LookRotation(targetPos - camera.transform.position);

            var _lookAt = LookAt;
            if (_lookAt.HasValue)
            {
                rotatioin = Quaternion.LookRotation(_lookAt.Value - camera.transform.position);
            }

            var changed = false;

            if (Vector3.Distance(rotatioin.eulerAngles, camera.transform.eulerAngles) > 0.1f)
            {
                seq.Join(camera.transform.DODynamicLookAt(targetPos, 0.75f).SetEase(Ease.InOutQuad));
                changed = true;
            }

            if (Vector3.Distance(newPos, camera.transform.position) > 0.1f)
            {
                changed = true; 
                seq.Join(camera.transform.DOMove(newPos, .75f).SetEase(Ease.InOutQuad));
            }

            if (!changed)
            {
                seq.Complete();
            }
            #endregion
        }

        void FocusStyleC(Camera camera, Bounds? bounds, Sequence seq, Vector3 targetPos, IFlow flow, float virtualsphereRadius)
        {
            var moveToPos = targetPos;

            if (bounds.HasValue)
                moveToPos.y += bounds.Value.size.y;

            float minD = (virtualsphereRadius) / Mathf.Sin(Mathf.Deg2Rad * camera.fieldOfView / 2);

            var direction = (camera.transform.position - moveToPos).normalized;
            var newPos = direction * minD + moveToPos;
            newPos = CameraPhysics.CalculateCameraBestPoint(newPos);

            var oldCameraRotation = camera.transform.rotation;
            var oldCameraPos = camera.transform.position;

            camera.transform.position = newPos;
            camera.transform.LookAt(targetPos);

            var newTargetRotation = camera.transform.rotation;

            camera.transform.rotation = oldCameraRotation;
            camera.transform.position = oldCameraPos;

            seq.Join(camera.transform.DOMove(newPos, .65f).SetEase(Ease.InOutQuad));
            seq.Join(camera.transform.DORotateQuaternion(newTargetRotation, 0.65f).SetEase(Ease.InOutQuad));
        }

        void FocusStyleD(Camera camera, Sequence seq, Vector3 targetPos, IFlow flow, float virtualsphereRadius)
        {
            if (flow.Vars.TryGet<Vector3>("Forward", out var forward) == false)
            {
                Debug.LogError("��ʹ��D�۽��㷨ʱȱ��Forward����,����֤�����ĽǶ���ȷ��ʹ��GameObject��仯����ͨ������");
                forward = Vector3.forward;
            }

            if (flow.Vars.TryGet<Vector3>("WorldLeftDirection", out var world_left_direction) == false)
            {
                Debug.LogError("��ʹ��D�۽��㷨ʱȱ��WorldLeftDirection����,��ͨ������ִ���ĽǶ�");
                world_left_direction = Vector3.left;
            }

            var fortyFiveDegVector = Quaternion.AngleAxis(25, world_left_direction) * forward;

            float minD = (virtualsphereRadius) / Mathf.Sin(Mathf.Deg2Rad * camera.fieldOfView / 2);

            var newPos = fortyFiveDegVector * minD + targetPos;

            newPos = CameraPhysics.CalculateCameraBestPoint(newPos);

            #region ����������������
            var rotatioin = Quaternion.LookRotation(targetPos - camera.transform.position);

            var _lookAt = LookAt;
            if (_lookAt.HasValue)
            {
                rotatioin = Quaternion.LookRotation(_lookAt.Value - camera.transform.position);
            }

            var changed = false;

            if (Vector3.Distance(rotatioin.eulerAngles, camera.transform.eulerAngles) > 0.1f)
            {
                seq.Join(camera.transform.DODynamicLookAt(targetPos, 0.95f).SetEase(Ease.InOutQuad));
                changed = true;
            }

            if (Vector3.Distance(newPos, camera.transform.position) > 0.1f)
            {
                changed = true;
                seq.Join(camera.transform.DOMove(newPos, .95f).SetEase(Ease.InOutQuad));
            }

            if (!changed)
            {
                seq.Complete();
            }
            #endregion
        }

        /// <summary>
        /// ����һ����
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="targetPos"></param>
        /// <param name="flow"></param>
        /// <param name="marginPercentage"></param>
        Tween FocusOnPosition(Camera camera, Bounds? bounds, Vector3 targetPos, IFlow flow, float virtualsphereRadius)
        {
            string? model = "";

            if (flow.Vars.TryGet<string>("MODEL", out model) == false)
                model = "A";

            var seq = DOTween.Sequence();

            if (model == "A")
            {
                FocusStyleA(camera, seq, targetPos, flow, virtualsphereRadius);
            }
            else if (model == "B")
            {
                FocusStyleB(camera, bounds, seq, targetPos, flow, virtualsphereRadius);
            }
            else if(model == "C")
            {
                FocusStyleC(camera, bounds, seq, targetPos, flow, virtualsphereRadius);
            }
            else if (model == "D")
            {
                if (bounds.HasValue)
                {
                    FocusStyleD(camera, seq, bounds.Value.center, flow, virtualsphereRadius);
                }
                else
                {
                    Debug.Log("ʹ��D�㷨��λ��һ��û�������ʸ�����ܻ���ɷ�Ԥ�ڵĽ��");
                    FocusStyleD(camera, seq, targetPos, flow, virtualsphereRadius);
                }   
            }
            else
            {
                Debug.LogError($"Modelֻ֧��A��B��C���־۽��㷨,���������:{model}");
            }

            if (seq.IsActive() == true)
            {
                seq.WithCancellation(DestroyOrExitStateCancelToken);
            }
            else
            {
                return null;
            }

            return seq;
        }

        public override UniTask Enter(IFlow flow)
        {
            float radius;
            Action? Complete;
            Tween tweenCore;

            if (flow.Vars.TryGet<float>("RADIUS", out radius) == false) radius = 1.1f;
            flow.Vars.TryGet<Action>("COMPLETE", out Complete);

            StateChangedEvent?.Invoke(true);

            if (flow.Vars.IsDefined("LOOKATV3"))
                m_lookAtV3 = flow.Vars.Get<Vector3>("LOOKATV3");
            else m_lookAtV3 = null;

            if (flow.Vars.IsDefined("LOOKAT"))
                m_lookAt = flow.Vars.Get<Transform>("LOOKAT");
            else m_lookAt = null;

            if (flow.Vars.IsDefined("TARGET"))
            {
                tweenCore = FocusOnGameObject(Camera.main, flow.Vars.Get<GameObject>("TARGET"), flow, radius);
            }
            else if (flow.Vars.IsDefined("TARGET_POS"))
            {
                tweenCore = FocusOnPosition(Camera.main, null, flow.Vars.Get<Vector3>("TARGET_POS"), flow, radius);
            }
            else
            {
                Debug.LogError("TARGET��TARGET_POS��������Ϊ��!�޷��۽�Ŀ��");
                throw new OperationCanceledException();
            }

            if (tweenCore != null)
            {
                tweenCore.OnComplete(() => Complete());
            }
            else
            {
                Complete();
            }

            return base.Enter(flow);
        }

        public override UniTask Exit(IFlow flow)
        {
            StateChangedEvent?.Invoke(false);

            return base.Exit(flow);
        }
    }
}