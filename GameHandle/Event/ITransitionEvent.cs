using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using IOTLib;

public interface ITransitionEvent : IMonoEventHandler, IEvent, IGameHandleEvent
{

}
