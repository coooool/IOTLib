using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

namespace IOTLib
{
    public class PlayerModelGoPoint : FlowState
    {
        public const string TriggerEventName = "移动到一个视点";

        // 状态改变，True为进入，Exit为False
        public readonly static UnityEvent<bool> StateChangedEvent = new UnityEvent<bool>();

        public PlayerModelGoPoint() :base("GoPoint")
        {
            
        }

        Tween GoPointFromName(string name, IFlow flow, float time)
        {
            if (DBServer.TryGetPointData(name!, out string pointDataStr))
            {
                var pointData = new { pos = string.Empty, euler = string.Empty, duration = 1.0f };

                pointData = pointDataStr.ToAnonymousType(pointData);

                // 如果为0则不使用这个数据
                var duration = pointData.duration;
                if (time != 0)
                {
                    duration = time;
                }

                var pos = pointData.pos.ToVector3();
                var euler = pointData.euler.ToVector3();

                return GoPointFromPos(pos, euler, duration);
            }
            else
            {
                Debug.LogError($"找不到目标点:{name}");
            }

            return null;
        }

        Tween GoPointFromPos(Vector3 pos, Vector3 euler, float time)
        {
            var hasDis = Vector3.Distance(pos, Camera.main.transform.position) > 1f ? true : false;    
            var hasEluer = Vector3.Distance(euler, Camera.main.transform.eulerAngles) > 1f ? true: false;
            
            if(!hasDis && !hasEluer)
            {
                return null;
            }
            
            var seq = DOTween.Sequence();

            seq.Join(Camera.main.transform.DOMove(pos, time));
            seq.Join(Camera.main.transform.DORotate(euler, time));

            seq.SetEase(Ease.InCubic);
            seq.WithCancellation(DestroyOrExitStateCancelToken);

            return seq;
        }

        Tween GoPointFromPos(Vector3 pos, Quaternion rotation, float time)
        {
            var hasDis = Vector3.Distance(pos, Camera.main.transform.position) > 1f ? true : false;
            var hasEluer = Vector3.Distance(rotation.eulerAngles, Camera.main.transform.eulerAngles) > 1f ? true : false;

            if (!hasDis && !hasEluer)
            {
                return null;
            }

            var seq = DOTween.Sequence();

            seq.Join(Camera.main.transform.DORotateQuaternion(rotation, time));
            seq.Join(Camera.main.transform.DOMove(pos, time));

            seq.SetEase(Ease.InCubic);
            seq.WithCancellation(DestroyOrExitStateCancelToken);

            return seq;
        }

        public override UniTask Enter(IFlow flow)
        {
            float time = 0;
            string? name;
            Action? Complete;
            Tween? tweenCore = null;

            StateChangedEvent?.Invoke(true);

            if (flow.Vars.TryGet<float>("TIME", out var timeval))
                time = timeval;

            flow.Vars.TryGet<Action>("COMPLETE", out Complete);


            if (flow.Vars.TryGet("NAME", out name))
            {
                tweenCore = GoPointFromName(name!, flow, time);
            }
            else if (flow.Vars.IsDefined("TARGET_POS"))
            {
                var pos = flow.Vars.Get<Vector3>("TARGET_POS");

                if(flow.Vars.TryGet<Quaternion>("QUATERNION", out var roration))
                {
                    tweenCore = GoPointFromPos(pos, roration, time);
                }
                else if(flow.Vars.TryGet<Vector3>("EULER", out var euler))
                {
                    tweenCore = GoPointFromPos(pos, euler, time);
                }
                else
                {
                    Debug.LogError("找不到EULER或者QUATERNION参数");
                } 
            }
            else
            {
                Debug.LogError("NAME或TARGET_POS参数不能为空!无法点位目标");
                throw new OperationCanceledException();
            }

            if (tweenCore != null)
                tweenCore.OnComplete(() => Complete());
            else
                Complete?.Invoke();

            return base.Enter(flow);
        }

        public override UniTask Exit(IFlow flow)
        {
            StateChangedEvent?.Invoke(false);

            return base.Exit(flow);
        }
    }
}
