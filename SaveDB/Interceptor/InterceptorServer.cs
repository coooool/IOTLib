using Mono.Data.Sqlite;
using System;
using UnityEngine;

namespace IOTLib.SaveDB.Interceptor
{
    internal class InterceptorServer
    {
        #region 拦截器
        public static bool GetInterceptorData(string request_name, string request_url, out InterceptorData interceptorData)
        {
            interceptorData = default(InterceptorData);
            interceptorData.block_url = request_url;

            try
            {
                var sql = $"select return_body,state_code from net_intercept where name=@name and IsOpen=True";

                var returnBody = "";
                long stateCode = 0;
                bool find = false;

                DBServer.Sqlite3.Query(sql, (cmd) =>
                {
                    var burl = new SqliteParameter("name", System.Data.DbType.AnsiString);
                    burl.Value = request_name;

                    cmd.Parameters.Add(burl);
                }, (row) =>
                {
                    returnBody = row.GetString(0);
                    stateCode = row.GetInt64(1);
                    find = true;
                });

                if (find)
                {
                    interceptorData.return_body = returnBody;
                    interceptorData.state_code = stateCode;

                    return true;
                }

                return false;
            } 
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError("获取拦截器时发生错误");
            }

            return false;
        }
        #endregion
    }
}
