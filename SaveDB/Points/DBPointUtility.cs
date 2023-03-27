using IOTLib.Sqlite3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using UnityEngine;

namespace IOTLib.SaveDB.Points
{
    public static class DBPointUtility
    {
        public static bool Add(DBPointDataItem dBPointData, SQLiteConnection connection)
        {
            //id INTEGER PRIMARY KEY AUTOINCREMENT, group_name VARCHAR(255), name VARCHAR(255), data JSON
            try
            {
                var sql = @"INSERT INTO points VALUES(NULL, @name, @data)";

                using (var sqlcmd = new SQLiteCommand(connection))
                {
                    sqlcmd.CommandText = sql;

                    var value_arg = new SQLiteParameter("@name", System.Data.DbType.AnsiString);
                    value_arg.Value = dBPointData.name;

                    var data_arg = new SQLiteParameter("@data", System.Data.DbType.Object);
                    data_arg.Value = dBPointData.data;

                    sqlcmd.Parameters.Add(value_arg);
                    sqlcmd.Parameters.Add(data_arg);

                    return sqlcmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SQLiteException sqlerr)
            {
                Debug.Log($"创建点失败:{sqlerr.ErrorCode}");

                Debug.Log(sqlerr.ResultCode);
            }

            return false;
        }

        public static bool Remove(DBPointDataItem dBPointData, SQLiteConnection connection)
        {
            var sql = @$"delete from points where id={dBPointData.id}";

            using (var sqlcmd = new SQLiteCommand(connection))
            {
                sqlcmd.CommandText = sql;

                return sqlcmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// 仅更新位置和旋转
        /// </summary>
        /// <param name="pointId">点位ID</param>
        /// <param name="pos">位置</param>
        /// <param name="eulerAngle">欧拉角</param>
        /// <param name="connection">数据连接实例</param>
        /// <returns></returns>
        public static bool UpdatePosAndEuler(int pointId, Vector3? pos, Vector3? eulerAngle, SQLiteConnection connection)
        {
            //id INTEGER PRIMARY KEY AUTOINCREMENT, group_name VARCHAR(255), name VARCHAR(255), data JSON
            try
            {
                using (var batch = connection.BeginTransaction())
                {
                    if (pos.HasValue)
                    {
                        var sql = @$"UPDATE points SET data = (SELECT json_set(points.data,'$.pos',@pos) from points) WHERE id={pointId}";

                        Sqlite3Utility.ExecuteNoQuery(sql, cmd =>
                        {
                            var pos_arg = new SQLiteParameter("@pos", System.Data.DbType.AnsiString);
                            pos_arg.Value = pos.Value.ToOriginStr();

                            cmd.Parameters.Add(pos_arg);
                        }, connection);
                    }

                    if (eulerAngle.HasValue)
                    {
                        var sql = @$"UPDATE points SET data = (SELECT json_set(points.data,'$.euler',@euler) from points) WHERE id={pointId}";

                        Sqlite3Utility.ExecuteNoQuery(sql, cmd =>
                        {
                            var euler_arg = new SQLiteParameter("@euler", System.Data.DbType.AnsiString);
                            euler_arg.Value = eulerAngle.Value.ToOriginStr();

                            cmd.Parameters.Add(euler_arg);
                        }, connection);
                    }

                    batch.Commit();
                }

                return true;
            }
            catch (SQLiteException sqlerr)
            {
                Debug.LogError($"更新失败:{sqlerr.ErrorCode}");
            }

            return false;
        }
    
        /// <summary>
        /// 更新所有数据
        /// </summary>
        /// <param name="dBPointData">点数据</param>
        /// <param name="connection">连接器</param>
        /// <returns></returns>
        public static bool UpdateData(DBPointDataItem dBPointData, SQLiteConnection connection)
        {
            try
            {
                var jsoncheck = JObject.Parse(dBPointData.data);
                if (jsoncheck == null)
                    throw new System.InvalidOperationException("属性检验失败,请检查正确的格式");

                var sql = @$"UPDATE points SET data = @data WHERE id={dBPointData.id}";

                Sqlite3Utility.ExecuteNoQuery(sql, cmd =>
                {
                    var pos_arg = new SQLiteParameter("@data", System.Data.DbType.AnsiString);
                    pos_arg.Value = dBPointData.data;

                    cmd.Parameters.Add(pos_arg);
                }, connection);

                return true;
            }
            catch (SQLiteException sqlerr)
            {
                Debug.LogError($"更新失败:{sqlerr.ErrorCode}");
            }
            catch(InvalidOperationException ex)
            {
                Debug.LogError(ex.Message);
            }

            return false;
        }
    }
}
