# redis集群说明
1. redis 主从模式 数据备注 ，主节点会定时向从节点同步数据

2. redis哨兵群集 主要实现redis的高可用

3. redis3.x之后，推出redis cluster ，主要用来做数据的分片，它把请求打到16384多个槽上，然后进行存储。
3主   3从的配置  它redis key的hash值进行计算得到槽位的
比如：3台redis，它的key产生的hashcode，对3取模（余），得到的数一定是0~2之间数，对应我们redis的0到2号服务器