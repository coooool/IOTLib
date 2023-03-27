using IOTLib.IDESystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// �����ĵ��Ĵ洢����
    /// </summary>
    public abstract class BaseLocalFileSave : ICGSceneEngine
    {
        /// <summary>
        /// ��������
        /// </summary>
        public virtual string Name => throw new System.NotImplementedException();

        /// <summary>
        /// ���·��
        /// </summary>
        public virtual string OutputFilePath => throw new System.NotImplementedException();

        public virtual void ReadScene(params string[] args)
        {
            var save_file_path = Path.Combine(Application.dataPath, "Config", OutputFilePath);

            if (File.Exists(save_file_path))
            {
                try
                {
                    using (var sr = File.OpenText(save_file_path))
                    {
                        ReadGameObject(sr);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
            else
            {
                Debug.LogError($"�Ҳ���Ŀ���ļ�:{save_file_path}");
            }
        }

        public virtual void SaveScene(IEnumerable<ExportCGPrefab> gameObjects)
        {
            Init(OutputFilePath, gameObjects);
        }

        /// <summary>
        /// �Զ���ConfigĿ¼�´���Ŀ���ļ������������ɾ�����´���
        /// </summary>
        /// <param name="fileName"></param>
        protected bool Init(string fileName, IEnumerable<ExportCGPrefab> gameObjects)
        {
            var save_file_path = Path.Combine(Application.dataPath, "Config", fileName);

            try
            {
                if (File.Exists(save_file_path))
                {
                    File.Delete(save_file_path);
                }

                using (var sw = new StreamWriter(save_file_path, false, System.Text.Encoding.UTF8))
                {
                    BeginWrite(sw);

                    foreach (var item in gameObjects)
                    {
                        var scripts = item.GetComponents(typeof(MonoBehaviour));

                        using (var list = new EasyListPool<CGEditor>())
                        {
                            foreach (var script in scripts)
                            {
                                var zn_editor = script.GetType().GetCustomAttribute<CGEditor>(false);

                                if (zn_editor != null)
                                {
                                    zn_editor.Component = script;
                                    list.Add(zn_editor);
                                }

                                if (script is ICGSceneSaveNotify cgsnt) 
                                    cgsnt.OnSceneSaveNotify();
                            }

                            WriteGameObject(sw, item, list);
                        }
                    }

                    EndWrite(sw);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("����ʧ��!");
                Debug.LogException(e);
            }

            return false;
        }

        /// <summary>
        /// ׼����ʼд�룬ͨ������д��һ��ͷ��Ϣ���汾��
        /// </summary>
        /// <param name="sw"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void BeginWrite(StreamWriter sw)
        {
            throw new NotImplementedException();
        }

        protected virtual void EndWrite(StreamWriter sw)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// дһ��GameObject
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="gameObject"></param>
        /// <param name="monoScript">���нű�</param>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual void WriteGameObject(StreamWriter sw, ExportCGPrefab gameObject, IEnumerable<CGEditor> monoScript) { throw new NotImplementedException(); }

        protected virtual void ReadGameObject(StreamReader sw) { throw new NotImplementedException(); }
    }
}