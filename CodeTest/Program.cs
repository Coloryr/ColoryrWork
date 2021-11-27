using ColoryrServer.Core.FileSystem;
using ICSharpCode.SharpZipLib.Zip;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

var dir = AppContext.BaseDirectory + "test\\";
if (!Directory.Exists(dir))
{
	Directory.CreateDirectory(dir);
}
ZipFile zip;
if (!File.Exists(dir + "backup.zip"))
{
	ZipOutputStream s = new(File.Create(dir + "backup.zip"));
	s.Close();
}
zip = new ZipFile(dir + "backup.zip");
var dir1 = AppContext.BaseDirectory + "test\\test\\";
zip.BeginUpdate();
foreach (var item in new DirectoryInfo(dir1).GetFiles())
{
	zip.Add(item.FullName, item.Name);
}
zip.CommitUpdate();
zip.Close();


