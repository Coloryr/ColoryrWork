using ColoryrWork.Lib.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace ColoryrServer.FileSystem;

internal static class FileRam
{
    public static string Local;

    public static void Start()
    {
        Local = ServerMain.RunLocal + @"/FileRam/";
        if (!Directory.Exists(Local))
        {
            Directory.CreateDirectory(Local);
        }
    }

    public static ConcurrentDictionary<string, dynamic> Load(string uuid)
    {
        try
        {
            if (!File.Exists(Local + uuid))
            {
                return null;
            }
            FileStream fs = new(Local + uuid, FileMode.Open);
            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new(typeof(ConcurrentDictionary<string, dynamic>));

            ConcurrentDictionary<string, dynamic> deserializedPerson =
                (ConcurrentDictionary<string, dynamic>)ser.ReadObject(reader, true);
            reader.Close();
            fs.Close();

            return deserializedPerson;
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
            return null;
        }
    }

    public static void Save(string uuid, ConcurrentDictionary<string, dynamic> data)
    {
        try
        {
            if (File.Exists(Local + uuid))
            {
                File.Delete(Local + uuid);
            }
            FileStream writer = new(Local + uuid, FileMode.Create);
            IFormatter formatter = new BinaryFormatter();
            DataContractSerializer ser = new(typeof(ConcurrentDictionary<string, dynamic>));
            ser.WriteObject(writer, data);
            writer.Close();
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }
    public static List<string> GetAll()
    {
        try
        {
            var list = new List<string>();
            var data = Function.GetPathFileName(Local);
            foreach (var item in data)
            {
                list.Add(item.Name);
            }
            return list;
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
            return new();
        }
    }

    public static void Remove(string temp1)
    {
        try
        {
            File.Delete(Local + temp1);
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }
}
