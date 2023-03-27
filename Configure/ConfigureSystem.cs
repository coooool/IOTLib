using UnityEngine;
using UMOD;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Linq;
using UnityEngine.Pool;

namespace IOTLib.Configure
{
    [GameSystem]
    [SystemDescribe(Author = "��", Dependent = "��", Describe = "�ṩͳһ�Ŀ�ƽ̨����ϵͳ", Name = "ConfigureSystem", Version = "0.5")]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ConfigureSystem : BaseSystem
    {
        /// <summary>
        /// �����ļ�����
        /// </summary>
        private static Dictionary<string, JObject> ConfigFiles = new ();

        /// <summary>
        /// ���ö���֪ͨ
        /// </summary>
        internal static UnityEvent LoadConfigChanged = new();

        /// <summary>
        /// �гɹ���ȡ�����ļ���ΪTrue
        /// </summary>
        internal static bool HasInitialzed = false;

        static void PushConfig(string key, JObject value)
        {
            if (ConfigFiles.ContainsKey(key))
            {
                throw new InvalidOperationException($"�����������ͬ�ļ����������ļ�:{key}");
            }

            ConfigFiles[key] = value;
        }

        public static void ReLoad()
        {
            HasInitialzed = false;
            ConfigFiles.Clear();

            var csys = SystemManager.GetSystem<ConfigureSystem>();
            csys.OnCreate();
        }

        public override void OnCreate()
        {
            var basePath = Path.Combine(Application.dataPath, "Config");

            try
            {
                if (false == Directory.Exists(basePath)) 
                    throw new InvalidOperationException("Assets·����δ����ConfigĿ¼,û�����ÿ��Զ�ȡ");

                var mainFilePath = Path.Combine(basePath, "main.json");
                if (File.Exists(mainFilePath) == false) throw new InvalidOperationException("����·����û��main.json���ļ�");

                var file_data = File.ReadAllText(mainFilePath);
                var file_name = Path.GetFileNameWithoutExtension(mainFilePath);
                var config = JObject.Parse(file_data);

                // ��ȡincludes�ֶΰ����������ļ�
                var incs = config.SelectToken("$.includes").Select(s => s.ToString()).ToArray();

                foreach (var subFile in incs)
                {
                    LoadConfig(Path.Combine(basePath, subFile));
                }

                HasInitialzed = true;
                // ֪ͨһ�ζ�����ϵͳ��ˢ��
                LoadConfigChanged?.Invoke();
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError(e.Message);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("��ȡ�����ļ�ʱ��������:{0},{1}", e.Message,e.StackTrace);
            }
        }

        /// <summary>
        /// �ͷű�ϵͳ
        /// </summary>
        public override void OnDrop()
        {
            ConfigFiles.Clear();
        }

        /// <summary>
        /// �Ƴ�һ�������ļ�������Ҫ��Ӻ�׺��.json
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <returns>����ɹ��Ƴ�����True</returns>
        public static bool RemoveConfigFile(string fileName)
        {
            if (fileName.EndsWith(".json"))
                throw new InvalidOperationException("�Ƴ������ļ�ʱ����Ҫ��Ӻ�׺��.json");

            return ConfigFiles.Remove(fileName);
        }

        /// <summary>
        /// ����һ��·���µ������ļ���������HTTP����
        /// ����Ƕೡ���µ����ݹ���Ӧ������AddItionalConfig��Ӹ�������
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void LoadConfig(string filePath)
        {
            filePath = filePath.EndsWith(".json") ? filePath : $"{filePath}.json";
            var fileName = Path.GetFileNameWithoutExtension(filePath);
                
            if (File.Exists(filePath) == false)
            {
                Debug.LogError($"�Ҳ��������ļ�:{filePath}");
            }
            else
            {
                var file_data = File.ReadAllText(filePath);
                var sub_config = JObject.Parse(file_data);

                // ѹ��ϵͳ
                PushConfig(fileName, sub_config);
            }
        }

        /// <summary>
        /// ��ȫ��� 
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        private static void NullCheck()
        {
            if (ConfigFiles.Count == 0)
                throw new NullReferenceException("���û�δ�ɹ�����,ȴ�����˷���,������������!");
        }

        /// <summary>
        /// ���
        /// </summary>
        /// <param name="jsonPath">JsonPath</param>
        /// <param name="findFile">ָ���������ļ�(Ϊ�����������)</param>
        /// <returns></returns>
        public static JToken? Query(string jsonPath, string findFile = "")
        {
            var needFindFile = !string.IsNullOrEmpty(findFile);

            if(needFindFile)
            {
                if (findFile.EndsWith(".json"))
                    throw new InvalidOperationException("ָ���ļ�����ʱ��Ҫ��Ӻ�׺��.json");
            }

            foreach (KeyValuePair<string, JObject> a in ConfigFiles)
            {
                if (needFindFile)
                {
                    if (a.Key != findFile) continue;
                }

                var tv = a.Value.SelectToken(jsonPath);

                if (tv != null) return tv;
            }

            return null;
        }

        /// <summary>
        /// ����һ�����������ʹ�ø���Ч��QueryAll�ص��棬���и�Ч���ڴ���䡣
        /// </summary>
        /// <param name="jsonPath">JsonPath</param>
        /// <param name="findFile">��ָ���ļ��в���</param>
        /// <returns></returns>
        public static List<T> QueryAll<T>(string jsonPath, string findFile = "") where T : new()
        {
            var results = new List<T>();
            QueryAll<T>(jsonPath, a => results.Add(a), findFile);

            return results;
        }

        /// <summary>
        /// ����һ�����������������и�Ч���ڴ���䡣
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPath">JsonPath</param>
        /// <param name="callBack">ÿ�����ݶ���ص�</param>
        /// <param name="findFile">ָ����ĳ���ļ��в���</param>
        /// <returns>����ҵ�����һ��������True</returns>
        public static bool QueryAll<T>(string jsonPath, Action<T> callBack, string findFile = "") where T : new()
        {
            var findCount = 0;
            var needFindFile = !string.IsNullOrEmpty(findFile);

            using (ListPool<T>.Get(out var results))
            {
                foreach (KeyValuePair<string, JObject> a in ConfigFiles)
                {
                    if (needFindFile)
                    {
                        if (a.Key != findFile) continue;
                    }

                    var tv = a.Value.SelectTokens(jsonPath);

                    foreach (var b in tv)
                    {
                        var tmp = b.Value<T>();

                        if (tmp != null)
                        {
                            callBack?.Invoke(tmp);
                            findCount++;
                        }
                    }
                }
            }

            return findCount > 0;
        }

        /// <summary>
        /// ����JSONPATH��ȡһ��ֵ
        /// </summary>
        /// <param name="json_path">JsonPath</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T? GetValue<T>(string json_path)
        {
            return GetValue<T>(json_path, null);
        }

        /// <summary>
        /// ����·����ȡһ��ֵ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json_path">JsonPath</param>
        /// <param name="fileName">��ָ���ļ��в���</param>
        /// <returns>ת�����ֵ</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T? GetValue<T>(string json_path, string fileName)
        {
            NullCheck();

            var token = Query(json_path, fileName);

            if (token == null)
                throw new InvalidOperationException($"�Ҳ���������:{json_path}");

            return token.Value<T>();
        }

        /// <summary>
        /// ����JSONPATH��ȡһ������,����ʹ��QueryAll<T>ʹ�ø���Ч�İ汾
        /// </summary>
        /// <param name="json_path">JsonPath</param>
        /// <returns>�����ܳɹ����л���ֵ</returns>
        public static List<T> GetValues<T>(string json_path) where T : new()
        {
            NullCheck();

            var result = new List<T>();
            var token = QueryAll<T>(json_path, a => result.Add(a));

            return result;
        }

        /// <summary>
        /// ��ȡһ�������ݸ���T���ͷ����л�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json_path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T GetObject<T>(string json_path)
        {
            NullCheck();

            var token = Query(json_path);

            if (token == null) 
                throw new System.InvalidOperationException($"��ȡ��������ʧ��:{json_path}");

            return token.ToObject<T>();
        }

        /// <summary>
        /// ��ȡ�������ݲ�����һ����������,�ʺ��ھֲ��ռ��������涪
        /// </summary>
        /// <typeparam name="T">ת����Ŀ������</typeparam>
        /// <param name="json_path">JsonPath</param>
        /// <param name="typeDef">����ģ��</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T GetAnonymousSection<T>(string json_path, T typeDef)
        {
            NullCheck();

            var token = Query(json_path);

            if (token == null)
                throw new InvalidOperationException($"��ȡ��������ʧ��:{json_path}");

            return token.ToObject<T>();
        }

        /*public async static UniTask LoadConfigFile()
        {
            try
            {
                #if UNITY_STANDALONE
                var file_data = await File.ReadAllTextAsync(Path.Combine(Application.dataPath, "config.json"));
                var config = JsonConvert.DeserializeObject<DefaultConfigData>(file_data);

                HostMap.InitData(config.hosts);
                ApiMap.InitData(config.apimap);
                #endif
            }
            catch(FileNotFoundException)
            {
                Debug.LogError($"����config�����ļ�ʧ��!");
            }
        }*/
    }
}
