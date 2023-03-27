public interface IStateGraph : IGraph
{
    /// <summary>
    /// ���״̬
    /// </summary>
    /// <param name="state"></param>
    /// <returns>�ɹ�Ϊtrue</returns>
    bool AddState(IFlowState state);

    /// <summary>
    /// �Ƴ�һ��״̬
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    bool RemoveState(IFlowState state);

    /// <summary>
    /// �Ƴ�һ��״̬
    /// </summary>
    /// <param name="name">״̬����</param>
    /// <returns></returns>
    bool RemoveState(string name);

    /// <summary>
    /// ����һ��״̬?
    /// </summary>
    /// <param name="name">״̬��</param>
    /// <returns></returns>
    bool HasState(string name);
}
