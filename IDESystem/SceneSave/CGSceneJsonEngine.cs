using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IOTLib
{
    public class CGSceneJsonEngine : BaseLocalFileSave
    {
        public override string Name => "json";

        public override string OutputFilePath => $"{SceneManager.GetActiveScene().name}.cgscene";

        struct SceneDataTemp
        {
            public CGModelJsonData[] gos;
        }

        #region 存储
        EasyListPool<CGModelJsonData> m_SaveModels;
        protected override void BeginWrite(StreamWriter sw)
        {
            m_SaveModels = new EasyListPool<CGModelJsonData>();
        }

        protected override void EndWrite(StreamWriter sw)
        {
            SceneDataTemp save = new SceneDataTemp()
            {
                gos = m_SaveModels.ToArray(),
            };

            sw.Write(JsonUtility.ToJson(save));

            m_SaveModels.Dispose();
        }

        protected override void WriteGameObject(StreamWriter sw, ExportCGPrefab gameObject, IEnumerable<CGEditor> monoScript)
        {
            var pos = gameObject.transform.position;

            if(gameObject.IsUGUI)
            {
                pos = gameObject.UIRecttransform.anchoredPosition;
            }

            var save_data = new CGModelJsonData
            {
                Id = gameObject.m_PrefabID,
                Name = gameObject.name,
                Position = pos,
                Rotation = gameObject.transform.eulerAngles,
                Scale = gameObject.transform.localScale,
                // 保存脚本数据
                Data = monoScript.Any() ? monoScript.Select(p => new CGModelComponentJsonData { FileId = p.FileId, Data = p.Component ? JsonUtility.ToJson(p.Component) : string.Empty }).ToArray() : Array.Empty<CGModelComponentJsonData>()
            };

            m_SaveModels.Add(save_data);
        }
        #endregion

        #region 读取

        protected override void ReadGameObject(StreamReader sw)
        {
            var jsonScene = sw.ReadToEnd();

            try
            {
                var scene_data = JsonUtility.FromJson<SceneDataTemp>(jsonScene);

                foreach (var node in scene_data.gos)
                {
                    if (CGResources.TryGetCGPrefab(node.Id, out var tmp))
                    {
                        var newItem = GameObject.Instantiate<GameObject>(tmp.gameObject, node.Position, Quaternion.Euler(node.Rotation));
                        newItem.transform.localScale = node.Scale;

                        if(false == string.IsNullOrEmpty(node.Name))
                            newItem.name = node.Name;

                        newItem.AddTag(CGResources.TAGName);

                        foreach (var monoData in node.Data)
                        {
                            var script = newItem.GetComponent(monoData.FileId);
                            if (script == null)
                            {
                                Debug.LogError($"获取目标组件不存在:{monoData.FileId}");
                            }
                            else
                            {
                                JsonUtility.FromJsonOverwrite(monoData.Data, script);
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("读取场景错误");
                Debug.LogException(e);
            }
        }
        #endregion
    }
}