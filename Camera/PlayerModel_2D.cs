using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 视角2D播放模式
    /// </summary>
    public class PlayerModel_2D : FlowState
    {
        public const string TriggerEventName = "进入2D模式";

        public PlayerModel_2D() : base("2D模式")
        {

        }

        public override UniTask Enter(IFlow flow)
        {
            var rotation = Camera.main.transform.eulerAngles;
            rotation.x = 90;

            Camera.main.transform.DORotate(rotation, 0.65f).OnComplete(() =>
            {
                CameraControlSetting.Setting.ControlMethod = ModelControlTypeEnum.Translate | ModelControlTypeEnum.Scale;
                CameraHelpFunc.ToAState(new KeyValuePairs(new { NO_RESET = true }));
            }).SetSafeDestory(this);

            return base.Enter(flow);
        }
    }
}