using Cysharp.Threading.Tasks;

public interface IStateEventListener
{
    /// <summary>
    /// 开始侦听
    /// </summary>
    internal void StartListener(IFlow flow);

    /// <summary>
    /// 结束侦听
    /// </summary>
    internal void StopListener(IFlow flow);

    /// <summary>
    /// 已经激活侦听了？
    /// </summary>
    bool IsListener { get; }
}
