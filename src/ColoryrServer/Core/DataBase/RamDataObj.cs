using System.Collections.Concurrent;

namespace ColoryrServer.Core.DataBase;

/// <summary>
/// ���ٷ���
/// </summary>
public static class ExtensionMethods
{
    public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
    {
        dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
    }
}

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