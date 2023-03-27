using IOTLib.UIPlus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UMOD;
using UnityEngine;

namespace IOTLib
{
    [SystemDescribe(Author = "吴", Describe = "世界坐标与屏幕坐标互转换系统", Name = "W2SPosSystem", Version = "0.1")]
    [GameSystem]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class W2SPosSystem : BaseSystem
    {
        public override void OnCreate()
        {      
        }

        void WorldToScreenPos(W2SPosData data, Vector3 position, Transform transform)
        {
            if (data.CullingHide)
            {
                var cpsub = position - UnityEngine.Camera.main.transform.position;
                var cpdot = Vector3.Dot(cpsub.normalized, UnityEngine.Camera.main.transform.forward);

                var viewPos = UnityEngine.Camera.main.WorldToViewportPoint(position);

                if (cpdot < 0 || viewPos.x < 0 || viewPos.x > 1)
                {
                    transform.gameObject.SetActive(false);
                }
                else if (cpdot < 0 || viewPos.y < 0 || viewPos.y > 1)
                {
                    transform.gameObject.SetActive(false);
                }
                else
                {
                    transform.gameObject.SetActive(true);
                }
            }

            var pos = RectTransformUtility.WorldToScreenPoint(UnityEngine.Camera.main, position);
            transform.position = pos;
        }

        void ScreenToWorldPos(W2SPosData data, Vector3 position, Transform transform)
        {
            //var newPos = position;
            //position.z -= UnityEngine.Camera.main.transform.position.z;

            var pos = UnityEngine.Camera.main.ScreenToWorldPoint(position);
            transform.position = pos;

            //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rtf.root as RectTransform,
            //position,
            //null,
            //out var world_pos))
            //{
            //    transform.position = world_pos;
            //}
        }

        public override void OnUpdate()
        {
            foreach(var p in GetBindDatas<W2SPosData>())
            {
                if (p.enabled)
                {
                    switch(p.ConvertMethod)
                    {
                        case W2SPosData.W2SPosTypeEnum.S2W:
                            WorldToScreenPos(p, p.Position, p.transform);               
                            break;

                        case W2SPosData.W2SPosTypeEnum.W2S:
                            ScreenToWorldPos(p, p.Position, p.transform);
                            break;
                    }
                }
            }
        }
    }
}
