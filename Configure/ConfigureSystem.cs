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
    [SystemDescribe(Author = "吴", Dependent = "无", Describe = "提供统一的跨平台配置系统", Name = "ConfigureSystem", Version = "0.5")]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ConfigureSystem : BaseSystem
    {
        /// <summary>
        /// 配置文件集合
        /// </summary>
        private static Dictionary<string, JObject> ConfigFiles = new ();

        /// <summary>
        /// 配置订阅通知
        /// </summary>
        internal static UnityEvent LoadConfigChanged = new();

        /// <summary>
        /// 有成功读取配置文件则为True
        /// </summary>
        internal static bool HasInitialzed = false;

        static void PushConfig(string key, JObject value)
        {
            if (ConfigFiles.ContainsKey(key))
            {
                throw new InvalidOperationException($"不允许出现相同文件名的配置文件:{key}");
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
                    throw new InvalidOperationException("Assets路径下未发现Config目录,没有配置可以读取");

                var mainFilePath = Path.Combine(basePath, "main.json");
                if (File.Exists(mainFilePath) == false) throw new InvalidOperationException("配置路径下没有main.json根文件");

                var file_data = File.ReadAllText(mainFilePath);
                var file_name = Path.GetFileNameWithoutExtension(mainFilePath);
                var config = JObject.Parse(file_data);

                // 读取includes字段包含了载入文件
                var incs = config.SelectToken("$.includes").Select(s => s.ToString()).ToArray();

                foreach (var subFile in incs)
                {
                    LoadConfig(Path.Combine(basePath, subFile));
                }

                HasInitialzed = true;
                // 通知一次订阅者系统有刷新
                LoadConfigChanged?.Invoke();
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError(e.Message);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("读取配置文件时发生错误:{0},{1}", e.Message,e.StackTrace);
            }
        }

        /// <summary>
        /// 释放本系统
        /// </summary>
        public override void OnDrop()
        {
            ConfigFiles.Clear();
        }

        /// <summary>
        /// 移除一个配置文件，不需要添加后缀名.json
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>如果成功移除返回True</returns>
        public static bool RemoveConfigFile(string fileName)
        {
            if (fileName.EndsWith(".json"))
                throw new InvalidOperationException("移除配置文件时不需要添加后缀名.json");

            return ConfigFiles.Remove(fileName);
        }

        /// <summary>
        /// 加载一个路径下的配置文件。可以是HTTP链接
        /// 如果是多场景下的数据管理。应当调用AddItionalConfig添加附加数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void LoadConfig(string filePath)
        {
            filePath = filePath.EndsWith(".json") ? filePath : $"{filePath}.json";
            var fileName = Path.GetFileNameWithoutExtension(filePath);
                
            if (File.Exists(filePath) == false)
            {
                Debug.LogError($"找不到配置文件:{filePath}");
            }
            else
            {
                var file_data = File.ReadAllText(filePath);
                var sub_config = JObject.Parse(file_data);

                // 压入系统
                PushConfig(fileName, sub_config);
            }
        }

        /// <summary>
        /// 安全检查 
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        private static void NullCheck()
        {
            if (ConfigFiles.Count == 0)
                throw new NullReferenceException("配置还未成功加载,却进行了访问,请先配置数据!");
        }

        /// <summary>
        /// 查的
        /// </summary>
        /// <param name="jsonPath">JsonPath</param>
        /// <param name="findFile">指定搜索的文件(为空则查找所有)</param>
        /// <returns></returns>
        public static JToken? Query(string jsonPath, string findFile = "")
        {
            var needFindFile = !string.IsNullOrEmpty(findFile);

            if(needFindFile)
            {
                if (findFile.EndsWith(".json"))
                    throw new InvalidOperationException("指定文件名称时需要添加后缀名.json");
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
        /// 查找一组配置项，建议使用更高效的QueryAll回调版，更有高效的内存分配。
        /// </summary>
        /// <param name="jsonPath">JsonPath</param>
        /// <param name="findFile">在指定文件中查找</param>
        /// <returns></returns>
        public static List<T> QueryAll<T>(string jsonPath, string findFile = "") where T : new()
        {
            var results = new List<T>();
            QueryAll<T>(jsonPath, a => results.Add(a), findFile);

            return results;
        }

        /// <summary>
        /// 查找一组配置项，这个方法具有高效的内存分配。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPath">JsonPath</param>
        /// <param name="callBack">每条数据都会回调</param>
        /// <param name="findFile">指定在某个文件中查找</param>
        /// <returns>如果找到任意一个都返回True</returns>
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
        /// 根据JSONPATH获取一个值
        /// </summary>
        /// <param name="json_path">JsonPath</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T? GetValue<T>(string json_path)
        {
            return GetValue<T>(json_path, null);
        }

        /// <summary>
        /// 根据路径获取一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json_path">JsonPath</param>
        /// <param name="fileName">在指定文件中查找</param>
        /// <returns>转换后的值</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T? GetValue<T>(string json_path, string fileName)
        {
            NullCheck();

            var token = Query(json_path, fileName);

            if (token == null)
                throw new InvalidOperationException($"找不到配置项:{json_path}");

            return token.Value<T>();
        }

        /// <summary>
        /// 根据JSONPATH获取一组数据,建议使用QueryAll<T>使用更高效的版本
        /// </summary>
        /// <param name="json_path">JsonPath</param>
        /// <returns>返回能成功序列化的值</returns>
        public static List<T> GetValues<T>(string json_path) where T : new()
        {
            NullCheck();

            var result = new List<T>();
            var token = QueryAll<T>(json_path, a => result.Add(a));

            return result;
        }

        /// <summary>
        /// 获取一部分数据根据T类型反序列化
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
                throw new System.InvalidOperationException($"获取配置数据失败:{json_path}");

            return token.ToObject<T>();
        }

        /// <summary>
        /// 获取部分数据并返回一个匿名类型,适合在局部空间中随用随丢
        /// </summary>
        /// <typeparam name="T">转到到目标类型</typeparam>
        /// <param name="json_path">JsonPath</param>
        /// <param name="typeDef">数据模板</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T GetAnonymousSection<T>(string json_path, T typeDef)
        {
            NullCheck();

            var token = Query(json_path);

            if (token == null)
                throw new InvalidOperationException($"获取配置数据失败:{json_path}");

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
                Debug.LogError($"加载config配置文件失败!");
            }
        }*/
    }
}
