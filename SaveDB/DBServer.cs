using IOTLib.SaveDB.Points;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using System;
using UMOD;
using UnityEngine;
using UnityEngine.Assertions;

namespace IOTLib
{
    public static class DBServer
    {
        private static Sqlite3System m_sqlite3 = null;

        internal static Sqlite3System Sqlite3
        {
            get {
                if (m_sqlite3 == null)
                {
                    m_sqlite3 = SystemManager.GetSystem<Sqlite3System>();
                }
                return m_sqlite3;
            }
        }

        #region GetVar
        /// <summary>
        /// 获取一个配置变量，只支持:int,float,double,bool,string,byte等原始类型
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetVar(string varName, string defaultValue)
        {
            var findVarSel = $"select value from vars where key=@key";

            var sp = new SqliteParameter("@key", System.Data.DbType.AnsiString);
            sp.Value = varName;

            if (Sqlite3.QuerySingleValue(findVarSel, out var data, sp))
            {
                return data;
            }

            return defaultValue;
        }

        /// <summary>
        /// 是否存在一个变量
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public static bool HasVar(string varName)
        {
            var findVarSel = $"select value from vars where key=@key";

            var sp = new SqliteParameter("@key", System.Data.DbType.AnsiString);
            sp.Value = varName;

            if (Sqlite3.QuerySingleValue(findVarSel, out var data, sp))
            {
                return true;
            }

            return false;
        }

        public static Vector2 GetVar(string varName, Vector2 defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return text.ToVector2();
        }

        public static Vector3 GetVar(string varName, Vector3 defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return text.ToVector3();
        }

        public static Vector4 GetVar(string varName, Vector4 defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return text.ToVector4();
        }

        public static int GetVar(string varName, int defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return int.Parse(text);
        }

        public static long GetVar(string varName, long defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return long.Parse(text);
        }

        public static double GetVar(string varName, double defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return double.Parse(text);
        }

        public static float GetVar(string varName, float defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return float.Parse(text);
        }

        public static byte GetVar(string varName, byte defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return byte.Parse(text);
        }

        public static bool GetVar(string varName, bool defaultValue)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return bool.Parse(text);
        }

        /// <summary>
        /// 恢复坐标、旋转、缩放
        /// </summary>
        /// <param name="varName">变量名</param>
        /// <param name="target">目标变换</param>
        /// <returns>如果找到变量成功恢复则为True</returns>
        public static bool GetVar(string varName, Transform target)
        {
            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text)) return false;

            var strs = text.Split('|');
            Assert.IsTrue(strs.Length == 3, "无法转换的Transform类型,错误的存储方式");

            var pos = strs[0].ToVector3();
            var eulerAngle = strs[1].ToVector3();
            var scale = strs[2].ToVector3();

            target.position = pos;
            target.eulerAngles = eulerAngle;
            target.localScale = scale;

            return true;
        }

        /// <summary>
        /// 反序列化一个class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varName">变量名</param>
        /// <param name="value">实例类型</param>
        /// <returns>成功返回True</returns>
        public static bool GetObject<T>(string varName, out T? value)
        {
            value = default(T);

            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text)) 
                return false;

            try
            {
                if(typeof(T) == typeof(string))
                {
                    var strValue = Convert.ToString(text);
                    value = (T)(object)strValue;
                }
                else
                    value = JsonConvert.DeserializeObject<T>(text);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"反序列化时发生错误:{ex.Message}");
            }

            return false;
        }

        /// <summary>
        ///  获取并反序列化一个匿名的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varName">变量名</param>
        /// <param name="value">目标</param>
        /// <returns></returns>
        /*public static bool GetAnonymousObject<T>(string varName, out T? value)
        {
            value = default(T);

            var text = GetVar(varName, string.Empty);

            if (string.IsNullOrEmpty(text))
                return false;

            try
            {
                value = JsonConvert.DeserializeAnonymousType(text, value);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"反序列化匿名对象错误:{ex.Message}");
            }

            return false;
        }*/

        /// <summary>
        /// 获取一个配置变量，只支持:int,float,double,bool,string,byte等原始类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varName">变量名</param>
        /// <param name="defaultValue">如果获取失败,使用这个默认值</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /*public static T GetVar<T>(string varName, T defaultValue) where T: struct
        {
            var value = GetVar(varName, null);

            if(false == string.IsNullOrEmpty(value))
            {
                try
                {
                    var convert = TypeDescriptor.GetConverter(typeof(T));

                    if (convert != null)
                    {
                        return (T)convert.ConvertFromString(value);
                    }
                    else
                    {
                        Debug.LogError($"不支持类型转换:{varName}---{typeof(T)}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"不支持的转换变量类型:{varName}---{typeof(T)} ---> {e.Message}");
                }
            }

            return defaultValue;
        }*/
        #endregion

        #region SetValue
        /// <summary>
        /// 持久化的存储一个值,只支持:int,float,double,bool,string,byte,vector等原始类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">变量名称(最长255个字符)</param>
        /// <param name="value">存储的值</param>
        //public static void SetValue<T>(string key, T value) where T: struct
        //{
        //    SetValue(key, value.ToString());
        //}
        public static void SetValue(string key, int value)
        {
            SetValue(key, value.ToString());
        }

        public static void SetValue(string key, long value)
        {
            SetValue(key, value.ToString());
        }

        public static void SetValue(string key, float value)
        {
            SetValue(key, value.ToString());
        }

        public static void SetValue(string key, double value)
        {
            SetValue(key, value.ToString());
        }
        
        public static void SetValue(string key, byte value)
        {
            SetValue(key, value.ToString());
        }

        public static void SetValue(string key, bool value)
        {
            SetValue(key, value ? "True" : "False");
        }

        public static void SetValue(string key, Vector2 value)
        {
            SetValue(key, value.ToString().RemoveInvalidChars());
        }

        public static void SetValue(string key, Vector3 value)
        {
            SetValue(key, value.ToString().RemoveInvalidChars());
        }

        public static void SetValue(string key, Vector4 value)
        {
            SetValue(key, value.ToString().RemoveInvalidChars());
        }

        public static void SetValue(string key, Transform value)
        {
            SetValue(key, $"{value.position.ToOriginStr()}|{value.eulerAngles.ToOriginStr()}|{value.localScale.ToOriginStr()}");
        }

        public static void SetValue(string key, Quaternion value)
        {
            SetValue(key, value.eulerAngles);
        }

        public static void SetValue(string key, string value)
        {
            var sql = @"INSERT INTO vars VALUES($key,$value) ON CONFLICT(key) DO UPDATE SET value=$value";

            using (var sqlcmd = new SqliteCommand())
            {
                sqlcmd.CommandText = sql;

                var key_arg = new SqliteParameter("$key", System.Data.DbType.AnsiString);
                key_arg.Value = key;

                var value_arg = new SqliteParameter("$value", System.Data.DbType.String);
                value_arg.Value = value.ToString();

                sqlcmd.Parameters.Add(key_arg);
                sqlcmd.Parameters.Add(value_arg);

                Sqlite3.Excute(sqlcmd);
            }
        }

        public static void SetValue(string key, Texture2D texture)
        {
            Assert.IsNotNull(texture, "非法值无法存储");
         
            SetValue(key, texture.GetRawTextureData());
        }

        public static void SetValue(string key, System.Object obj)
        {
            Assert.IsNotNull(obj, "非法值无法存储");

            var str = JsonConvert.SerializeObject(obj);

            Assert.IsFalse(string.IsNullOrEmpty(str), "空值无法存储");

            SetValue(key, obj.ToJson());
        }
        #endregion

        #region 点位数据
        /// <summary>
        /// 获取一个点位原始数据
        /// </summary>
        /// <param name="name">视点名称</param>
        /// <param name="result">输出JSON</param>
        /// <returns>找到则True</returns>
        public static bool TryGetPointData(string name, out string result)
        {
            result = string.Empty;

            var findVarSel = $"select data from points where name=@name";

            var sp = new SqliteParameter("@name", System.Data.DbType.AnsiString);
            sp.Value = name;

            if (Sqlite3.QuerySingleValue(findVarSel, out var data, sp))
            {
                result = data;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 仅更新指定点的位置和欧拉角度
        /// </summary>
        /// <param name="name">点位名称</param>
        /// <param name="pos">位置</param>
        /// <param name="eulur">角度</param>
        /// <returns>成功True</returns>
        public static bool SetPointPosAndEuler(string name, Vector3? pos, Vector3? eulur)
        {
            Assert.IsTrue(pos.HasValue || eulur.HasValue, "必须更新位置和角度其中一项");

            var findVarSel = $"select id from points where name=@name";

            var sp = new SqliteParameter("@name", System.Data.DbType.AnsiString);
            sp.Value = name;

            if (Sqlite3.QuerySingleValue(findVarSel, out var data, sp))
            {
                var pointId = int.Parse(data);
                return DBPointUtility.UpdatePosAndEuler(pointId, pos, eulur, Sqlite3.Connection);
            }

            return false;
        }
        #endregion
    }
}
