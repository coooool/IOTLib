using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace IOTLib.Sqlite3
{
    public static class Sqlite3Utility
    {
        /// <summary>
        /// 执行不关注返回行数据的语句
        /// </summary>
        /// <param name="sql">SQL语法</param>
        /// <param name="changedCallback">添加参数回调</param>
        /// <param name="connection">连接实例</param>
        /// <returns></returns>
        public static bool ExecuteNoQuery(string sql, System.Action<SQLiteCommand> changedCallback, SQLiteConnection connection)
        {
            using(var cmd = new SQLiteCommand(sql, connection))
            {
                changedCallback?.Invoke(cmd);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
