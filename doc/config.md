# ColoyrServer

## 文件结构信息
[返回](../README.md)

## 服务器核心配置文件
`MainConfig.json`
```JSON
{
  //Http服务器地址
  "Http": [
    {
      "IP": "127.0.0.1",
      "Port": 80
    }
  ],
  //反向代理设置
  "Routes": {
    "turn": {
      "Url": "http://127.0.0.1/",
      "Heads": {}
    }
  },
  //是否启用Http的SSL
  "UseSsl": false,
  //Http的SSL配置
  "Ssls": {
    "default": {
      "Ssl": "./test.sfx",
      "Password": "123456"
    }
  },
  //域名转发设置
  "UrlRoutes": {
    "www.test.com": {
      "Url": "http://localhost:81/",
      "Heads": {}
    }
  },
  //是否启用转发
  "RouteEnable": false,
  //无控制台输入
  "NoInput": false,
  //Socket服务器地址
  "Socket": {
    "IP": "0.0.0.0",
    "Port": 25556
  },
  //WebSocket服务器地址
  "WebSocket": {
    "UseSsl": false,
    "Ssl": "",
    "Password": "",
    "Socket": {
      "IP": "0.0.0.0",
      "Port": 25557
    }
  },
  //机器人链接地址
  "Robot": {
    "Socket": {
      "IP": "127.0.0.1",
      "Port": 23333
    },
    "Packs": []
  },
  "Mysql": [
    {
      "Enable": false,
      "IP": "127.0.0.1",
      "Port": 3306,
      "User": "root",
      "Password": "MTIzNDU2",
      "TimeOut": 1000,
      "Conn": "SslMode=none;Server={0};Port={1};User ID={2};Password={3};Charset=utf8;"
    }
  ],
  "MSsql": [
    {
      "Enable": false,
      "IP": "127.0.0.1",
      "Port": 0,
      "User": "root",
      "Password": "MTIzNDU2",
      "TimeOut": 1000,
      "Conn": "Server={0};UID={1};PWD={2};"
    }
  ],
  "Oracle": [
    {
      "Enable": false,
      "IP": "",
      "Port": 0,
      "User": "",
      "Password": "",
      "TimeOut": 1000,
      "Conn": "Data Source=MyDatabase.db;Mode=ReadWriteCreate"
    }
  ],
  "SQLite": [
    {
      "Enable": false,
      "IP": "127.0.0.1",
      "Port": 0,
      "User": "root",
      "Password": "MTIzNDU2",
      "TimeOut": 1000,
      "Conn": "User Id={2};Password={3};Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME=test)))"
    }
  ],
  "Redis": [
    {
      "Enable": false,
      "IP": "127.0.0.1",
      "Port": 6379,
      "Conn": "{0}:{1}"
    }
  ],
  //Mqtt服务器设置
  "MqttConfig": {
    "UseSsl": false,
    "Ssl": "",
    "Password": "",
    "Socket": {
      "IP": "0.0.0.0",
      "Port": 12345
    }
  },
  //Task任务设置
  "TaskConfig": {
    "MaxTime": 30
  },
  //Http客户端数量
  "HttpClientNumber": 100,
  "Requset": {
    "Temp": [
      ".jpg",
      ".png",
      ".mp4",
      ".jpge",
      ".gif"
    ],
    "TempTime": 1800,
    "Stream": true,
    "StreamType": [
      ".mp4"
    ]
  },
  //编译工具密钥设置
  "AES": {
    "Key": "Key",
    "IV": "IV"
  },
  //代码设置
  "CodeSetting": {
    "NotInclude": [
      "sni.dll"
    ],
    "CodeLog": true,
    "MinifyHtml": true,
    "MinifyJS": true,
    "MinifyCSS": true
  }
}
```

## 加密证书配置

修改`Ssl`为证书位置  
修改`Password`为证书密码  
修改`UseSsl`为true，重启服务器  
加密版本为`Tls1.3`，需要你的证书支持才行

## 登录数据库

管理员数据，登录信息储存在`ColoryrServer\Login.db`中
- 表`login`储存登录Token
- 表`user`储存账户密码

## 代码储存

代码储存数据库
在文件夹`ColoryrServer\Codes`中
- `Code.db`主要存储代码项目信息和部分代码
- `CodeLog.db`主要储存代码编辑数据
- `WebCode.db`储存网页代码数据
- `WebData.db`储存网页文件数据
- `WebLog.db`储存网页代码编辑数据
- `Static`文件夹用于储存Vue网页项目

## 编译后DLL储存

在文件夹`ColoryrServer\Dll`中，储存所有C#代码编译后的Dll

## 删除代码储存

在文件夹`ColoryrServer\Removes`中，储存所有删除的项目信息

## 静态网页文件夹

在文件夹`ColoryrServer\Static`中，储存静态网页信息  
`index.html`与`404.html`就在里面

## Dll运行与编译结果数据库

在`ColoryrServer\DllLog.db`中  
- 表`error`储存运行错误
- 表`webbuild`储存Vue编译结果

## 其他说明

- `FileRam`内存数据库固化
- `Libs`额外CLR库，可以用于Dll调用，只能在关闭服务器期间更换
- `Notes`导出的说明信息

