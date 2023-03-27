using System;
using UMOD;
using UnityEngine;
using System.Data.SQLite;
using System.IO;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using IOTLib.Configure.ValueFactory;

namespace IOTLib
{
    /// <summary>
    /// Sqlite3系统
    /// </summary>
    [SystemDescribe(Author = "吴", Describe = "Sqlite3数据库服务,提供核心存储和查询功能", Name = "Sqlite3Server", Version = "0.1")]
    [GameSystem()]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class Sqlite3System : BaseSystem
    {
        private SQLiteConnection m_Connection;

        public SQLiteConnection Connection => m_Connection;

        public UnityEngine.Events.UnityEvent<Sqlite3System> InitDBCallBack = new();

        /// <summary>
        /// 初始化的时候则创建一个数据库
        /// </summary>
        public override void OnCreate()
        {
            var basePath = Path.Combine(Application.dataPath, "Config");

            if(!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            var baseSqlFilePath = Path.Combine(Application.dataPath, "Config", "appdata");

            if (!File.Exists(baseSqlFilePath))
            {
                SQLiteConnection.CreateFile(baseSqlFilePath);
            }

            m_Connection = new SQLiteConnection();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = baseSqlFilePath;

            m_Connection.ConnectionString = connstr.ToString();

            m_Connection.Open();

            InitDBIfNeed();
            RegisterValueFactory();
        }

        /// <summary>
        /// 初始化数据库如果需要
        /// </summary>
        void InitDBIfNeed()
        {
            // 创建变量配置表 id INTEGER PRIMARY KEY AUTOINCREMENT, 
            // 创建唯一索引CREATE UNIQUE INDEX
            CreatleTableIfNotExists("vars(key VARCHAR(255) PRIMARY KEY, value TEXT)");
            //变量没有组了Excute("CREATE INDEX IF NOT EXISTS group_index ON vars(group_name)");
            //Excute("CREATE INDEX IF NOT EXISTS key_index ON vars(key)");

            CreatleTableIfNotExists("points(id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255),data JSON)");
            Excute("CREATE INDEX IF NOT EXISTS point_name_index ON points(name)");
            //Excute("CREATE INDEX IF NOT EXISTS group_index ON points(group_name)");
            //Excute("CREATE INDEX IF NOT EXISTS idx_index ON points(idxstr)");

            CreatleTableIfNotExists("users(id INTEGER PRIMARY KEY AUTOINCREMENT,guid VARCHAR(255), userData JSON)");
            Excute("CREATE INDEX IF NOT EXISTS guid_index ON users(guid)");

            CreatleTableIfNotExists("net_intercept(id INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR(255), block_url VARCHAR(2588), return_body TEXT, state_code INTEGER,IsOpen BOOLEAN, UNIQUE(name))");
            Excute("CREATE UNIQUE INDEX IF NOT EXISTS url_index ON net_intercept(name)");

            InitDBCallBack?.Invoke(this);
        }

        /// <summary>
        /// 注册值提供器
        /// </summary>
        void RegisterValueFactory()
        {
            ValueProviderFactory.RegisterValueProvider(GetDBValueFromValueProvider);
        }

        string GetDBValueFromValueProvider(string key)
        {
            return DBServer.GetVar(key, string.Empty);
        }

        /// <summary>
        /// 创建表如果不存在，该方法其实只是在前面拼接了CREATE TABLE IF NOT EXISTS ${table}
        /// </summary>
        /// <param name="table">例如:users(id INTEGER,name TEXT)</param>
        /// <returns></returns>
        public int CreatleTableIfNotExists(string table)
        {
            return Excute($"CREATE TABLE IF NOT EXISTS {table}");
        }

        /// <summary>
        /// 查询数据，每一行回调一次
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="rowDataCallback">每行回调，通过x['字段名']拿到列数据</param>
        public void Query(string sql, Action<SQLiteDataReader> rowDataCallback)
        {
            using (var cmd = new SQLiteCommand(m_Connection))
            {
                cmd.CommandText = sql;

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rowDataCallback?.Invoke(reader);
                    }
                }
            }
        }

        public void Query(string sql, System.Action<SQLiteCommand> bindArgs, Action<SQLiteDataReader> rowDataCallback)
        {
            using (var cmd = new SQLiteCommand(m_Connection))
            {
                cmd.CommandText = sql;

                bindArgs?.Invoke(cmd);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rowDataCallback?.Invoke(reader);
                    }
                }
            }
        }

        /// <summary>
        /// 查询一个单一的值 
        /// </summary>
        /// <param name="sql">Sql</param>
        /// <param name="args">参数</param>
        /// <param name="valStr">找到的值</param>
        /// <returns>找到True</returns>
        public bool QuerySingleValue(string sql, out string valStr, params SQLiteParameter[] args)
        {
            valStr = string.Empty;

            using (var cmd = new SQLiteCommand(m_Connection))
            {
                cmd.CommandText = sql;

                if(args != null)
                {
                    cmd.Parameters.AddRange(args);
                }

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        valStr = reader[0].ToString();
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 查执行不读取行数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>影响的行数或列表</returns>
        public int Excute(string sql)
        {
            using (var cmd = new SQLiteCommand(m_Connection))
            {
                cmd.CommandText = sql;
                return cmd.ExecuteNonQuery();
            }
        }

        public int Excute(SQLiteCommand sql)
        {
            sql.Connection = m_Connection;
            return sql.ExecuteNonQuery();
        }

        public override void OnDrop()
        {
            if(m_Connection != null)
            {
                m_Connection.Close();
                m_Connection = null;
            }

            base.OnDrop();
        }
    }
}
