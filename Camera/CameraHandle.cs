using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOTLib;
using System.Data;
using System;
using IOTLib.Extend;
using DG.Tweening.Core.Easing;
using IOTLib.GameHandle;

namespace IOTLib
{
    /// <summary>
    /// ����ͷ��ҵϵͳ
    /// </summary>
    public class CameraHandle : FlowStateGraph
    {
        public const string HandleName = "MainCamera";
        /// <summary>
        /// �����UI��
        /// </summary>
        public static bool IsPointerOverGameObject { get; set; }
        public static bool IsPointerHoverGameObject { get; set; }

        private static WeakReference<AnyState> _anyState = null;

        public CameraHandle(bool defaultIsAState = false) : base(HandleName)
        {
            var AnyState = new AnyState("Default");

            if (_anyState != null)
            {
                throw new InvalidOperationException("����߼����ƺ���ι�����CameraHandleʵ��������һ����������");
            }

            _anyState= new WeakReference<AnyState>(AnyState);

            var EmptyState = new EmptyState() { };
            var PlayerModelA = new PlayerModelA() { };
            var PlayerModelF = new PlayerModelF() { };
            var ObserveModel = new PlayerModelObserve() { };
            var GoPointModel = new PlayerModelGoPoint();
            var Player2DModel = new PlayerModel_2D();
            var RoamModel = new PlayerModel_Roam();

            if (defaultIsAState)
            {
                PlayerModelA.IsStart = true;
            }
            else
            {
                //EmptyState.IsStart = true;
            }

            AddState(AnyState);
            AddState(EmptyState);
            AddState(PlayerModelA);
            AddState(PlayerModelF);
            AddState(ObserveModel);
            AddState(GoPointModel);
            AddState(Player2DModel);
            AddState(RoamModel);
          
            TransitionUtility.CreateEvent(AnyState, EmptyState, EmptyState.TriggerEventName).AllowExitSelf = true;
            TransitionUtility.CreateEvent(AnyState, PlayerModelA, PlayerModelA.TriggerEventName).AllowExitSelf = true;
            TransitionUtility.CreateEvent(AnyState, PlayerModelF, PlayerModelF.TriggerEventName).AllowExitSelf = true;
            TransitionUtility.CreateEvent(AnyState, ObserveModel, PlayerModelObserve.TriggerEventName).AllowExitSelf = true;
            TransitionUtility.CreateEvent(AnyState, GoPointModel, PlayerModelGoPoint.TriggerEventName).AllowExitSelf = true;
            TransitionUtility.CreateEvent(AnyState, Player2DModel, PlayerModel_2D.TriggerEventName).AllowExitSelf = true;
            TransitionUtility.CreateEvent(AnyState, RoamModel, PlayerModel_Roam.TriggerEventName).AllowExitSelf = true;
        }

        /// <summary>
        /// ����Ӿ���״̬
        /// </summary>
        /// <param name="anyState"></param>
        public static void AddMirrorState(AnyState anyState)
        {
            if (_anyState == null) throw new InvalidOperationException("CameraHandle��δ��ʼ��");
            if(_anyState.TryGetTarget(out var r))
            {
                r.AddMirrorState(anyState);
            }
            else
            {
                Debug.LogError("CameraHandleĿ���Ѿ����ͷ�");
            }
        }

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static FlowStateShapshot GetThisSnapshot()
        {
            if (_anyState == null) 
                throw new InvalidOperationException("CameraHandle��δ��ʼ��");
            
            if (_anyState.TryGetTarget(out var r))
            {
                return new FlowStateShapshot(r);
            }

            throw new InvalidOperationException("CameraHandleĿ���Ѿ����ͷ�");
        }

        /// <summary>
        /// �Ƴ�����ڵ�
        /// </summary>
        /// <param name="state"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void RemoveMirrorState(AnyState state)
        {
            if (_anyState == null) throw new InvalidOperationException("CameraHandle��δ��ʼ��");
            if (_anyState.TryGetTarget(out var r))
            {
                r.RemoveMirrorState(state);
            }
            else
            {
                Debug.LogError("CameraHandleĿ���Ѿ����ͷ�");
            }
        }

        /// <summary>
        /// �۽�һ�����壬�߱�3�ֲ�ͬ�㷨
        /// </summary>
        /// <param name="target">Ŀ��</param>
        /// <param name="Complete">��ɻص�,Ĭ���ֽ���Aģʽ</param>
        /// <param name="Model">A:����Unityԭ��F��,B:������ƫ��һЩ��C������B������B��������,D:����Ŀ����ӵ��豸�����㷨</param>
        /// <param name="radius">Խ�������Զ</param>
        public static void F(GameObject target, Action Complete = null, string Model = "A", float radius = 1.1f)
        {
            if(Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var arg = new { TARGET = target, RADIUS = radius, COMPLETE = Complete, MODEL = Model };
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelF.TriggerEventName, new KeyValuePairs(arg));
        }

        public static void F(Vector3 Position, Action Complete = null, string Model = "A", float radius = 1.1f)
        {
            if (Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var arg = new { TARGET_POS = Position, RADIUS = radius, COMPLETE = Complete, MODEL = Model };
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelF.TriggerEventName, new KeyValuePairs(arg)); 
        }

        public static void FAndLookAt(Vector3 Position, Vector3 lookAtV3, Action Complete = null, string Model = "A", float radius = 1.1f)
        {
            if (Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var arg = new { TARGET_POS = Position, RADIUS = radius, COMPLETE = Complete, MODEL = Model, LOOKATV3 = lookAtV3 };
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelF.TriggerEventName, new KeyValuePairs(arg));
        }

        internal static void F(KeyValuePairs args)
        {
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelF.TriggerEventName, args);
        }

        public static void F(IEnumerable<Vector3> Points, Action Complete = null, string Model = "A", float radius = 1.1f)
        {
            if (Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var bounds = BoundsUtility.CalculateBound(Points);
            var virtualRadius = Vector3.Magnitude(bounds.max - bounds.center) * radius;

            var arg = new { TARGET_POS = bounds.center, RADIUS = radius, COMPLETE = Complete, MODEL = Model };
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelF.TriggerEventName, new KeyValuePairs(arg));
        }

        public static void F(IEnumerable<GameObject> Points, Action Complete = null, string Model = "A", float radius = 1.1f)
        {
            if (Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var bounds = BoundsUtility.CalculateBound(Points);

            var virtualRadius = Vector3.Magnitude(bounds.max - bounds.center) * radius;

            var arg = new { TARGET_POS = bounds.center, RADIUS = virtualRadius, COMPLETE = Complete, MODEL = Model };
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelF.TriggerEventName, new KeyValuePairs(arg));
        }

        /// <summary>
        /// �ƶ���һ����
        /// </summary>
        /// <param name="name">������</param>
        /// <param name="time">��Ŀ������ʱ��</param>
        /// <param name="Complete">��ɻص�</param>
        public static void GoPoint(string name, float? time = null, Action Complete = null)
        {
            if (Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var arg = new { NAME = name, COMPLETE = Complete, TIME = time.HasValue ? Mathf.Max(0.01f, time.Value) : 0 };
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelGoPoint.TriggerEventName, new KeyValuePairs(arg));
        }

        public static void GoPoint(Vector3 pos, Vector3 euler, float time, Action Complete = null)
        {
            if (Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var arg = new { TARGET_POS = pos, EULER = euler, COMPLETE = Complete, TIME = Math.Max(0.01f, time) };
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelGoPoint.TriggerEventName, new KeyValuePairs(arg));
        }

        public static void GoPoint(Vector3 pos, float time, System.Action Complete = null)
        {
            if (Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var rotation = Quaternion.LookRotation(pos - Camera.main.transform.position, Vector3.up);

            var arg = new { TARGET_POS = pos, QUATERNION = rotation, COMPLETE = Complete, TIME = Math.Max(0.01f, time) };
            GameHandleEventSystem.TriggerEvent(HandleName, PlayerModelGoPoint.TriggerEventName, new KeyValuePairs(arg));
        }

        /// <summary>
        /// ���õ�ǰλ��
        /// </summary>
        /// <param name="world_pos">�����</param>
        public static void SetCurrentPosition(Vector3 world_pos)
        {
            SetCurrentPosition(world_pos, null);   
        }

        public static void SetCurrentPosition(string name)
        {
            var point = PointSystem.GetPosAndEulerAngle(name);
            SetCurrentPosition(point[0], point[1]);
        }

        public static void SetCurrentPosition(Vector3 world_pos, Vector3? eulerAngles)
        {
            var handle = GameHandleSystem.GetFlowGraphFromName(HandleName);
            if (handle == null)
            {
                Camera.main.transform.position = world_pos;
                if (eulerAngles.HasValue)
                    Camera.main.transform.eulerAngles = eulerAngles.Value;

                //Debug.LogError("ϵͳδ��ʼ�����޷���ȡ�����ҵϵͳ");
                return;
            }

            var cm = handle.GraphRef as CameraHandle;
            var pa = cm.GetUnit<PlayerModelA>();
            if (pa.IsListener)
            {
                pa.SetCustomPos(world_pos);
                if (eulerAngles.HasValue)
                    pa.SetCustomEulerAngles(eulerAngles.Value);
            }
            else
            {
                Camera.main.transform.position = world_pos;
                if(eulerAngles.HasValue)
                    Camera.main.transform.eulerAngles= eulerAngles.Value;
            }
        }

        public static void EmptyAction()
        {

        }
    }
}