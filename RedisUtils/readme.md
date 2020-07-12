# redis集群说明
1. redis 主从模式 数据备注 ，主节点会定时向从节点同步数据

2. redis哨兵群集 主要实现redis的高可用

3. redis3.x之后，推出redis cluster ，主要用来做数据的分片，它把请求打到16384多个槽上，然后进行存储。
3主   3从的配置  它redis key的hash值进行计算得到槽位的
比如：3台redis，它的key产生的hashcode，对3取模（余），得到的数一定是0~2之间数，对应我们redis的0到2号服务器

4. StackExchange.Redis里的CommandFlags
```
None = 0;	//默认
HighPriority = 1;	//不用了，废弃
FireAndForget = 2; //对结果不感兴趣，调用者将会立即收到默认值
PreferMaster = 0; //如果主服务器可用，则应在主服务器上执行此操作，但可以执行读操作
DemandMaster = 4; //此操作只应在[主站]上执行
PreferSlave = 8;	//如果可用，则应在[从站]上执行此操作，但将在其上执行
DemandSlave = 12;	//此操作只应在[从站]上执行。 仅适用于读取操作。
NoRedirect = 64;	//表示由于ASK或MOVED响应，不应将此操作转发到其他服务器
NoScriptCache = 512 //表示与脚本相关的操作应使用EVAL，而不是SCRIPT LOAD + EVALSHA
```