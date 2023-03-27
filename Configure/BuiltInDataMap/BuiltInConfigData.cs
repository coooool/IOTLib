using System;
using System.Collections.Generic;
using UMOD;
using UnityEngine;

namespace IOTLib.Configure
{
    [GameSystem]
    [SystemDescribe(Author = "吴", 
        Dependent = "ConfigureSystem", 
        Describe = "框架内置的数据提供器,包含hosts、apimap等。",
        Name = "BuiltInConfigData", 
        Version = "0.2")]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateInBefore(typeof(ConfigureSystem))]
    public class BuiltInConfigData : BaseSystem
    {
        public override void OnCreate()
        {
            ConfigureSystem.LoadConfigChanged.AddListener(UpdateConfig);
        }

        void UpdateConfig()
        {
            ApiMap.InitData();
            HostMap.InitData();
        }
    }
}
