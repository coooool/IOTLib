public interface IStateGraph : IGraph
{
    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="state"></param>
    /// <returns>成功为true</returns>
    bool AddState(IFlowState state);

    /// <summary>
    /// 移除一个状态
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    bool RemoveState(IFlowState state);

    /// <summary>
    /// 移除一个状态
    /// </summary>
    /// <param name="name">状态名称</param>
    /// <returns></returns>
    bool RemoveState(string name);

    /// <summary>
    /// 存在一个状态?
    /// </summary>
    /// <param name="name">状态名</param>
    /// <returns></returns>
    bool HasState(string name);
}
