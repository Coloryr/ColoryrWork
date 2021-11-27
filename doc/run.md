# ColoryrServer

## 部署&启动
[返回](../README.md)

## 部署
1. 安装[.NET6_SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
```
$ dotnet --version
6.0.100
```
2. 克隆并构建
```
注：仅支持在windows下构建，Linux下构建会无法正常运行
```
```
$ git clone https://github.com/Coloryr/ColoryrWork.git
Cloning into 'ColoryrWork'...
remote: Enumerating objects: 4246, done.
remote: Counting objects: 100% (4246/4246), done.
remote: Compressing objects: 100% (2582/2582), done.
remote: Total 4246 (delta 2516), reused 3242 (delta 1518), pack-reused 0R
Receiving objects: 100% (4246/4246), 15.66 MiB | 3.84 MiB/s, done.
Resolving deltas: 100% (2516/2516), done.
$ cd ColoryrWork
$ dotnet build
用于 .NET 的 Microsoft (R) 生成引擎版本 17.0.0+c9eb9dd64
版权所有(C) Microsoft Corporation。保留所有权利。

  正在确定要还原的项目…
  所有项目均是最新的，无法还原。
  ColoryrServer.Core -> G:\code\ColoryrWork\build_out\Core\net6.0\ColoryrServer.Core.dll
  ColoryrBuild -> G:\code\ColoryrWork\build_out\Build\net6.0-windows7.0\ColoryrBuild.dll
  CodeTest -> G:\code\ColoryrWork\build_out\Test\net6.0\CodeTest.dll
  ......
    23 个警告
    0 个错误

已用时间 00:00:06.52
```
在`ColoryrWork`文件夹下，你会得到一个`build_out`的构建文件夹  
- `ASP`是服务器文件  
- `Build`是编辑器
- `Core`是服务器核心
- ~~`Test`不用管~~

## 启动
Windows下直接双击exe启动  
Linux下
```
$ dotnet ColoryrServer.ASP.dll
```
