using System.Collections.Concurrent;

namespace ColoryrServer.Core.DataBase;

/// <summary>
/// �ڴ����ݿ����ݻ���
/// </summary>
public class RamDataObj
{
    /// <summary>
    /// �Ƿ�̻�
    /// </summary>
    public bool IsSave { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public ConcurrentDictionary<string, dynamic> Data { get; init; }

    public RamDataObj(ConcurrentDictionary<string, dynamic> data = null)
    {
        if (data == null)
            Data = new();
        else
            Data = data;
    }

    /// <summary>
    /// ɾ������
    /// </summary>
    public void Delete()
    {
        Data.Clear();
    }
}