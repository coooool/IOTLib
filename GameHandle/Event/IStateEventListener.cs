using Cysharp.Threading.Tasks;

public interface IStateEventListener
{
    /// <summary>
    /// ��ʼ����
    /// </summary>
    internal void StartListener(IFlow flow);

    /// <summary>
    /// ��������
    /// </summary>
    internal void StopListener(IFlow flow);

    /// <summary>
    /// �Ѿ����������ˣ�
    /// </summary>
    bool IsListener { get; }
}
