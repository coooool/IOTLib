using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib.SaveDB.Points
{
    /// <summary>
    /// 一个点
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DBPointDataItem
    {
        [JsonProperty]
        public int id { get; set; }
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public string data { get; set; } = null;

        private DBPointDataItem(string pointName)
        {
            name = pointName;
        }

        public DBPointDataItem(string name, string data) : this(name)
        {
            this.data = data;
        }
    }
}
