# ColoyrServer

## 理论性能测试
[返回](../README.md)
### 测试机器：  
![测试环境](./pic/server.png)  
### 测试环境：  
- 系统：Ubuntu20(虚拟机，8G内存 4核心 8线程)  
  （宿主系统：Windows Server 2019）
- .Net版本：6.0.100-rc.1
-------------------
## [wrk](https://github.com/wg/wrk)
![结果](./pic/wrk.png)  
## [postjson](http://coolaf.com/)  
![结果](./pic/postjson.png)  
## [JMeter](https://jmeter.apache.org/)
![配置](./pic/jmeter1.png)  
![结果](./pic/jmeter2.png)  
## [webbench](https://github.com/EZLippi/WebBench)
![结果](./pic/webbench.png)  
## [siege](https://github.com/JoeDog/siege)
```
siege -c 255 -r 10 http://localhost:35555/
```
![结果](./pic/siege.png)  