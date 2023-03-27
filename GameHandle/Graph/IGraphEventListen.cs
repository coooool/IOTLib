using IOTLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraphEventListen : IMonoEventHandler
{
    internal bool TestEvent(string name,out IGameHandleEvent flow);
}
