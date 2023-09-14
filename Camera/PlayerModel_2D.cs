using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// �ӽ�2D����ģʽ
    /// </summary>
    public class PlayerModel_2D : FlowState
    {
        public const string TriggerEventName = "����2Dģʽ";

        public PlayerModel_2D() : base("2Dģʽ")
        {

        }

        public override UniTask Enter(IFlow flow)
        {
            var rotation = Camera.main.transform.eulerAngles;
            rotation.x = 90;
            rotation.y = 0;
            rotation.z = 0;
            float view_size = 0;

            Camera.main.transform.DORotate(rotation, 0.65f).OnComplete(() =>
            {
               if(view_size != 0)
                {
                    Camera.main.orthographicSize = view_size;
                    Camera.main.orthographic = true;
                }
                //var fieldOfView = Camera.main.fieldOfView / 2;
                //var view_size = Mathf.Abs(Camera.main.transform.position.y) * Mathf.Tan(fieldOfView * Mathf.Deg2Rad);

                //Camera.main.orthographicSize = view_size;
                //Camera.main.orthographic = true;

                CameraControlSetting.Setting.ControlMethod = ModelControlTypeEnum.Translate | ModelControlTypeEnum.Scale;
                CameraHelpFunc.ToAState(new KeyValuePairs(new { NO_RESET = true }));
            }).SetSafeDestory(this);

            return base.Enter(flow);
        }
    }
}