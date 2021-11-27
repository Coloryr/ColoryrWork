# ColoyrServer

## 配置文件说明
[返回](../README.md)

## 服务器核心配置文件
```JSON
{
    //反向代理(效率极低)
    "Rotes": {
        "turn": {
            //目标地址
            "Url": "http://127.0.0.1/",
            //添加的请求头
            "Heads": {
                "Rote": "test"
            }
        }
    },
    //SSL模式，启用后会使用https
    "Ssl": false,
    //SSL证书配置
    "Ssls": {
        //默认的SSL证书
        "default": {
            "SslLocal": "./test.sfx",
            "SslPassword": "123456"
        },
        //某个域名的SSL证书
        "www.xxx.xxx":{
            "SslLocal": "./www.xxx.xxx.sfx",
            "SslPassword": "123456"
        }
    },
    //域名转发
    "UrlRotes": {
        //目标域名
        "www.test.com": {
            //目标地址
            "Url": "http://localhost:81/",
            //添加的请求头
            "Heads": {
                "Rote": "test"
            }
        }
    },
    //是否启用反向代理
    "RoteEnable": false,
    //控制台输入无效
    "NoInput": false,
    //Http服务器配置，可以添加监听数量
    "Http": [
        {
            "IP": "127.0.0.1",
            "Port": 25555
        }
    ],
    //Socket服务器配置
    "Socket": {
        "IP": "0.0.0.0",
        "Port": 25556
    },
    //WebSocket服务器配置
    "WebSocket": {
        "IP": "0.0.0.0",
        "Port": 25557
    },
    //机器人链接配置
    "Robot": {
        "IP": "127.0.0.1",
        "Port": 23333
    },
    //Mysql配置
    "Mysql": [
        {
            "Enable": false,
            "IP": "127.0.0.1",
            "Port": 3306,
            "User": "root",
            "Password": "MTIzNDU2",
            "Conn": "SslMode=none;Server={0};Port={1};User ID={2};Password={3};Charset=utf8;"
        }
    ],
    //MS sql设置
    "MSsql": [
        {
            "Enable": false,
            "IP": "127.0.0.1",
            "User": "root",
            "Password": "MTIzNDU2",
            "Conn": "Server={0};UID={1};PWD={2};"
        }
    ],
    //Redis设置
    "Redis": [
        {
            "Enable": false,
            "IP": "127.0.0.1",
            "Port": 6379,
            "Conn": "{0}:{1}"
        }
    ],
    //Oracle配置
    "Oracle": [
        {
            "Enable": false,
            "IP": "127.0.0.1",
            "User": "root",
            "Password": "MTIzNDU2",
            "Conn": "User Id={2};Password={3};Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME=test)))"
        }
    ],
    //编辑用户设置
    "User": [
        {
            "Username": "Admin",
            //SHA1加密的密码
            "Password": "4e7afebcfbae000b22c7c85e5560f89a2a0280b4"
        }
    ],
    //不参与动态编译的dll
    "NotInclude": [
        "sni.dll"
    ],
    //MQTT配置
    "MQTTConfig": {
        "Port": 12345
    },
    //任务配置
    "TaskConfig": {
        "ThreadNumber": 50,
        "MaxTime": 30
    },
    //ffmpeg路径
    "MPGE": null,
    //HttpClient数量
    "HttpClientNumber": 100,
    //请求选项
    "Requset": {
        //后端路径（http://xxx:xxx/{WebAPI}）
        "WebAPI": "/WebAPI",
        //网页路径（http://xxx:xxx/{Web}）
        "Web": "/Web",
        //进入缓存的文件后缀
        "Temp": [
        ".jpg",
        ".png",
        ".mp4",
        ".jpge",
        ".gif"
        ],
        //缓存时间
        "TempTime": 1800,
        //自动流转换
        "Stream": true,
        //自动转换的文件类型
        "StreamType": [
            ".mp4"
        ]
    },
    //编辑器AES加密
    "AES": {
        "Key": "Key",
        "IV": "IV"
    }
}
```
