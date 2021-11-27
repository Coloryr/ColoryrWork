using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrBuild
{
	internal class ZIPUtils
	{
		public static void Pack(string dir, string file, string name)
		{
			ZipFile zip;
			if (!File.Exists(dir + "backup.zip"))
			{
				ZipOutputStream s = new(File.Create(dir + "backup.zip"));
				s.Close();
			}
			zip = new ZipFile(dir + "backup.zip");
			zip.BeginUpdate();
			zip.Add(file, name);
			zip.CommitUpdate();
			zip.Close();
		}

		public static void Pack1(string dir, string dir1, string name)
		{
			ZipFile zip;
			if (!File.Exists(dir + "backup.zip"))
			{
				ZipOutputStream s = new(File.Create(dir + "backup.zip"));
				s.Close();
			}
			zip = new ZipFile(dir + "backup.zip");
			zip.BeginUpdate();
			zip.AddDirectory(name);
			foreach (var item in new DirectoryInfo(dir1).GetFiles())
			{
				zip.Add(item.FullName, name + "/" + item.Name);
			}
			zip.CommitUpdate();
			zip.Close();
		}
	}
}