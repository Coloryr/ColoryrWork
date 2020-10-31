using Lib.Build.Object;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ColoryrBuild
{
    class CodeSave
    {
        public static string FilePath = App.RunLocal + @"/CodeTEMP/";
        private static ReaderWriterLockSlim lock1 = new ReaderWriterLockSlim();
        public CodeSave()
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
        }
        public static void Save(string name, string data)
        {
            File.WriteAllText(FilePath + name + ".cs", data);
        }
        public static void Save(CSFileCode code)
        {
            if (string.IsNullOrWhiteSpace(code.Code))
                return;
            try
            {
                lock1.EnterWriteLock();
                if (File.Exists(FilePath + GetFileName(code)))
                {
                    File.Delete(FilePath + GetFileName(code));
                }
                File.Create(FilePath + GetFileName(code)).Close();
                using (FileStream Stream = File.Open(FilePath + GetFileName(code), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (Stream != null && !string.IsNullOrWhiteSpace(code.Code))
                    {
                        var array = Encoding.UTF8.GetBytes(code.Code);
                        Stream.Write(array, 0, array.Length);
                    }
                }
            }
            finally
            {
                lock1.ExitWriteLock();
            }
        }
        public static void Load(CSFileCode code)
        {
            try
            {
                lock1.EnterReadLock();
                using (FileStream Stream = File.Open(FilePath + GetFileName(code), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (Stream != null)
                    {
                        Thread.Sleep(10);
                        byte[] bytsize = new byte[Stream.Length];
                        Stream.Read(bytsize, 0, (int)Stream.Length);
                        code.Code = Encoding.UTF8.GetString(bytsize);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                lock1.ExitReadLock();
            }
        }
        public static string GetFileName(CSFileObj code)
        {
            return code.UUID + ".cs";
        }
        public static CSFileCode GetFile(CSFileObj code)
        {
            try
            {
                lock1.EnterReadLock();
                var code1 = new CSFileCode(code);
                if (!File.Exists(FilePath + GetFileName(code)))
                    return null;
                using (FileStream stream = File.Open(FilePath + GetFileName(code), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    if (stream != null)
                    {
                        byte[] bytsize = new byte[stream.Length];
                        stream.Read(bytsize, 0, (int)stream.Length);
                        code1.Code = Encoding.UTF8.GetString(bytsize);
                    }
                }
                return code1;
            }
            finally
            {
                lock1.ExitReadLock();
            }
        }
    }
}
