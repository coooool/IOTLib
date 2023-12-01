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
                Debug.Log($"��ǰ��������������:{save_file_path}");
            }
        }

        public virtual void SaveScene(IEnumerable<ExportCGPrefab> gameObjects)
        {
            Init(OutputFilePath, gameObjects);
        }

        // ���ݾɵ��ļ�
        void BakOldSceneFile(string cg_file_path)
        {
            var bak_folder = Path.Combine(Path.GetDirectoryName(cg_file_path), "bak");
            
            if(!Directory.Exists(bak_folder))
            {
                Directory.CreateDirectory(bak_folder);
            }

            var bak_file_name = string.Format("{0}{1}.cgscene", 
                Path.GetFileNameWithoutExtension(cg_file_path),
                DateTime.Now.ToString("yyyy-M-d-HH-ss-m")
            );

            var bak_full_path = Path.Combine(bak_folder, bak_file_name);
            
            if (File.Exists(bak_full_path))
            {
                Debug.Log("�����ʱ���ڶ�α����ظ��������⽫�滻����ı�������");
                File.Delete(bak_full_path);
                //Debug.LogWarning("���ݳ���ʱ������ͬ�ı��ݣ�����ʹ���µ��ļ���...");

                //int errCount = 0;

                //do
                //{
                //    var new_file_path = string.Format("{0}-{1}", Path.GetFileNameWithoutExtension(bak_full_path), UnityEngine.Random.Range(1, 99999));

                //    if(!File.Exists(new_file_path))
                //    {
                //        File.Move(bak_full_path, new_file_path);
                //        Debug.LogWarning($"�޸��������ݳɹ���");

                //        bak_full_path = new_file_path;
                //        break;
                //    }

                //    if(++errCount > 999)
                //    {
                //        Debug.LogWarning("�����޸������ļ�����ʧ���ˡ�����ʾ��������");
                //        return;
                //    }

                //} while (true);
            }

            File.Move(cg_file_path, bak_full_path);
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
                BakOldSceneFile(save_file_path);

                var save_temp_file_path = Path.Combine(
                    Path.GetDirectoryName(save_file_path), 
                    Path.GetFileNameWithoutExtension(fileName) + ".temp"
                    );

                if (File.Exists(save_temp_file_path))
                {
                    File.Delete(save_temp_file_path);
                }

                using (var sw = new StreamWriter(save_temp_file_path, false, System.Text.Encoding.UTF8))
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

                if (File.Exists(save_file_path))
                {
                    File.Delete(save_file_path);
                }

                File.Move(save_temp_file_path, save_file_path);

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