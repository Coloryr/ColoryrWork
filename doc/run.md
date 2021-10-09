# ColoyrServer

## 部署&启动
[返回](../README.md)

## 部署
1. 安装[net6](https://dotnet.microsoft.com/download/dotnet/6.0)
```
$ dotnet --version
6.0.100-rc.1.21463.6
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
用于 .NET 的 Microsoft (R) 生成引擎版本 17.0.0-preview-21460-01+8f208e609
版权所有(C) Microsoft Corporation。保留所有权利。

  正在确定要还原的项目…
  已还原 G:\ColoryrWork\ColoryrBuild\ColoryrBuild.csproj (用时 290 ms)。
  已还原 G:\ColoryrWork\CodeTest\CodeTest.csproj (用时 290 ms)。
  已还原 G:\ColoryrWork\ColoryrServer\ASP\ColoryrServer.ASP.csproj (用时 616 ms)。
  已还原 G:\ColoryrWork\ColoryrServer\NoASP\ColoryrServer.NoASP.csproj (用时 615 ms)。
  已还原 G:\ColoryrWork\ColoryrServer\Core\ColoryrServer.Core.csproj (用时 616 ms)。
  你正在使用 .NET 的预览版。请查看 https://aka.ms/dotnet-core-preview
  你正在使用 .NET 的预览版。请查看 https://aka.ms/dotnet-core-preview
  你正在使用 .NET 的预览版。请查看 https://aka.ms/dotnet-core-preview
  你正在使用 .NET 的预览版。请查看 https://aka.ms/dotnet-core-preview
  你正在使用 .NET 的预览版。请查看 https://aka.ms/dotnet-core-preview
  ......
    37 个警告
    0 个错误

已用时间 00:00:07.02
```
在`ColoryrWork`文件夹下，你会得到一个`build_out`的构建文件夹  
- `ASP`和`NoASP`是服务器文件  
- `Build`是编辑器
- `ServerCore`是服务器核心
- ~~`Test`不用管~~

## 启动
Windows下直接双击exe启动  
Linux下
```
$ dotnet ColoryrServer.ASP.dll
```
或
```
$ dotnet ColoryrServer.NoASP.dll
```
