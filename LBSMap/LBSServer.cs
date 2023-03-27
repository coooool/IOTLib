using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMOD;
using UnityEngine;

namespace IOTLib
{
    [GameSystem]
    [SystemDescribe(Author = "��",
       Dependent = "��",
       Describe = "����GCJ09(�ȸ衢�ߵ�)����ϵ��GPS��λ����",
       Name = "LBS��λ����",
       Version = "0.2")]
    public class LBSServer : SingleBaseSystem<LBSServer>
    {
        public override void BindData(DataBehaviour monoBehaviour)
        {
            if(monoBehaviour is ILBSMap)
            {
                base.BindData(monoBehaviour);
            }
            else
            {
                Debug.LogWarning("ֻ�ܰ󶨵�ͼ����");
            }
        }

        /// <summary>
        /// ����CBJ-02����ϵ�Ķ�λ����һ��Unity��������λ��
        /// </summary>
        /// <param name="lng">����(Y)</param>
        /// <param name="lat">γ��(X)</param>
        /// <param name="layerName">ָ����λ��</param>
        /// <returns>���ص���һ��3ά���꣬Y����Ϊ0</returns>
        public static Vector3 CalculateWorldPosition(double lng, double lat, string layerName = "")
        {
            if (string.IsNullOrEmpty(layerName))
            {
                var firstMap = Install.FirstOrDefault();
                if(firstMap != null)
                {
                    return (firstMap as ILBSMap).CalculateWorldPoint(lng, lat);
                }
            }
            else
            {
                foreach(var a in Install)
                {
                    if(a.gameObject.name == layerName)
                    {
                        return (a as ILBSMap).CalculateWorldPoint(lng, lat);
                    }
                }
            }

            throw new System.InvalidOperationException($"�����в�����λ���ṩ����");
        }

        /// <summary>
        /// Unity����λ�õ�GCJ09�ľ�γ��,��ʧY����
        /// </summary>
        /// <param name="world_pos">Unity���������λ��</param>
        /// <returns>��γ��</returns>
        public static MapLocation WorldPointToGCJ09Point(Vector3 world_pos, string layerName = "")
        {
            if (string.IsNullOrEmpty(layerName))
            {
                var firstMap = Install.FirstOrDefault();
                if (firstMap != null)
                {
                    return (firstMap as ILBSMap).WorldPointToGCJ09Point(world_pos);
                }
            }
            else
            {
                foreach (var a in Install)
                {
                    if (a.gameObject.name == layerName)
                    {
                        return (a as ILBSMap).WorldPointToGCJ09Point(world_pos);
                    }
                }
            }

            throw new System.InvalidOperationException($"�����в�����λ���ṩ����");
        }
    }
}