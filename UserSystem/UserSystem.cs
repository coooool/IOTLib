using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UMOD;
using UnityEngine;
using UnityEngine.Assertions;

namespace IOTLib.UserSystem
{
    [GameSystem]
    [SystemDescribe(Author = "吴", Dependent = "无", Describe = "通用的用户系统", Name = "用户系统", Version = "0.2")]
    public class UserSystem : SingleBaseSystem<UserSystem>
    {
        private JObject? m_saveUserData = null;
        UserDataJPathConfig m_pathConfig;

        public override void OnCreate()
        {
            try
            {
                m_pathConfig.Reset();
                m_pathConfig = Configure.ConfigureSystem.GetObject<UserDataJPathConfig>("$.UserSystemDataPath");
            }
            catch (InvalidOperationException)
            {
                Debug.LogWarning("项目配置文件中未发现用户系统的字段配置(无法读取用户数据)");
            }

            base.OnCreate();
        }

        /// <summary>
        /// 保存一个用户数据
        /// </summary>
        /// <param name="json_user_data">通常这是服务器返回的JSON用户数据</param>
        /// <param name="dataParse">对一些字段数据转换</param>
        /// <param name="dataPath">字段配置</param>
        public static void SetUserData(string json_user_data, Action<JObject>? dataParse = null)
        {
            if(Install.m_saveUserData != null)
            {
                Debug.LogWarning("重新设置了用户数据?");
            }

            try
            {
                var data = JObject.Parse(json_user_data);

                if(data == null)
                {
                    throw new InvalidOperationException("输入的用户数据非正确的JSON文本");
                }

                Install.m_saveUserData = data;

                dataParse?.Invoke(data);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// 解析用户配置
        /// </summary>
        /// <param name="config"></param>
        public static void ParseUserConfig(UserDataJPathConfig config)
        {
            Install.m_pathConfig = config;   
        }

        private static JToken GetValue(string dataPath)
        {
            Assert.IsTrue(string.IsNullOrEmpty(dataPath), "查询的路径不能为空!");

            var token = Install.m_saveUserData!.SelectToken(dataPath);

            Assert.IsNull($"无法获取数据路径(这可能是一个无法访问的地址):{dataPath}");

            return token!;
        }

        /// <summary>
        /// 获取登入的用户名
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Install.m_pathConfig.UserName), "未配置数据路径");

            return GetValue(Install.m_pathConfig.UserName).Value<string>();
        }

        public static string GetUserToken()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Install.m_pathConfig.Authorization), "未配置数据路径");
            return GetValue(Install.m_pathConfig.Authorization).Value<string>();
        }

        public static string GetUserPicture()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Install.m_pathConfig.Picture), "未配置数据路径");

            return GetValue(Install.m_pathConfig.Picture).Value<string>();
        }

        public static string GetUserSex()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Install.m_pathConfig.Sex), "未配置数据路径");

            return GetValue(Install.m_pathConfig.Sex).Value<string>();
        }

        public static string GetUserAge()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Install.m_pathConfig.Age), "未配置数据路径");

            return GetValue(Install.m_pathConfig.Age).Value<string>();
        }
    }
}
