using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 视角-漫游模式
    /// </summary>
    public class PlayerModel_Roam : FlowState
    {
        public const string TriggerEventName = "进入巡航模式";

        public PlayerModel_Roam() : base("巡航模式")
        {

        }

        public override UniTask Enter(IFlow flow)
        {
            Debug.Log("进入漫游模式");
            return base.Enter(flow);
        }
    }
}