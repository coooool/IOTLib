using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// �ӽ�-����ģʽ
    /// </summary>
    public class PlayerModel_Roam : FlowState
    {
        public const string TriggerEventName = "����Ѳ��ģʽ";

        public PlayerModel_Roam() : base("Ѳ��ģʽ")
        {

        }

        public override UniTask Enter(IFlow flow)
        {
            Debug.Log("��������ģʽ");
            return base.Enter(flow);
        }
    }
}