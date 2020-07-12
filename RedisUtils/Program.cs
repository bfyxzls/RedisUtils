using Newtonsoft.Json;
using RedisUtils;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace redis.@lock
{
    class Program
    {
        static void Main(string[] args)
        {
            RedisManager.Instance.GetDatabase(0).StringSet("hello", "你好");
            redisList();
            //redisListGet();
            //sql_insert("beijing", 20);
            //sql_insert("hebei", 25);
            //sql();

            Console.Read();
        }

        static void redisList()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<RedisValue> redisValues = new List<RedisValue>();
            for (int i = 0; i < 10000; i++)
            {
                redisValues.Add(JsonConvert.SerializeObject(new userinfo { id = i, name = "zzl" + i }));
            }
            RedisManager.Instance.GetDatabase().ListLeftPush("orderList", redisValues.ToArray());
            RedisManager.Instance.GetDatabase().KeyExpire("orderList", DateTime.Now.AddMinutes(1));
            stopwatch.Stop();
            Console.WriteLine("10000 record time(ms):" + stopwatch.ElapsedMilliseconds);

        }

        static void redisListGet()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            RedisValue[] redisValues = RedisManager.Instance.GetDatabase().ListRange("orderList");
            stopwatch.Stop();
            Console.WriteLine("pop 10000 record time(ms):" + stopwatch.ElapsedMilliseconds);

        }


        static void redisSet()
        {
            List<userinfo> list = new List<userinfo>();
            list.Add(new userinfo { id = 1, name = "zzl" });
            list.Add(new userinfo { id = 2, name = "lr" });
            list.Add(new userinfo { id = 3, name = "bobo" });
            list.Add(new userinfo { id = 4, name = "zzl2" });
            list.Add(new userinfo { id = 5, name = "lr2" });
            list.Add(new userinfo { id = 6, name = "bobo2" });
            list.Add(new userinfo { id = 7, name = "zzl3" });
            list.Add(new userinfo { id = 8, name = "lr3" });
            list.Add(new userinfo { id = 9, name = "bobo3" });
            list.Add(new userinfo { id = 10, name = "lr4" });
            list.Add(new userinfo { id = 11, name = "bobo4" });
            foreach (userinfo u in list)
            {
                RedisManager.Instance.GetDatabase().SetAdd("hot_data", JsonConvert.SerializeObject(u));
            }
            IEnumerable<RedisValue> slist = RedisManager.Instance.GetDatabase().SetMembers("hot_data");
            foreach (var s in slist)
            {
                Console.WriteLine(JsonConvert.DeserializeObject<userinfo>(s).ToString());
            }

            Console.WriteLine("hot_data is contain zzl:" + RedisManager.Instance.GetDatabase().SetContains("hot_data", JsonConvert.SerializeObject(new userinfo
            {
                id = 1,
                name = "zzl"
            })));

        }
        static void sql()
        {
            IRepository<userinfo> repository = new EFRepository<userinfo>(new testEntities());
            repository.GetModel().ToList().ForEach(i =>
            {
                int total = (int)RedisManager.Instance.GetDatabase(0).StringGet(i.city);
                Console.WriteLine(i.ToString() + "统计结果" + total);
            });
        }
        static void sql_insert(string city, int value)
        {
            RedisManager.Instance.GetDatabase(0).StringIncrement(city, value);
        }
        static void wait()
        {
            //如果锁被占用，第二个线程将等待
            Task.Run(() =>
            {
                RedisLock.LockAwait("a1", "1", 10000, () =>
                {
                    Console.WriteLine("第1个执行");
                    Thread.Sleep(10000);
                });
            });

            Task.Run(() =>
            {
                RedisLock.LockAwait("a1", "1", 10000, () =>
                {
                    Console.WriteLine("第2个执行");
                    Thread.Sleep(10000);
                });
            });
        }
        static void disposeMethod()
        {
            //如果锁被占用，第二个线程将不会被执行
            Task.Run(() =>
            {
                RedisLock.Lock("a1", "1", 10000, () =>
                {
                    Console.WriteLine("第1个执行");
                    Thread.Sleep(10000);
                });
            });

            Task.Run(() =>
            {
                RedisLock.Lock("a1", "1", 10000, () =>
                {
                    Console.WriteLine("第2个执行");
                    Thread.Sleep(10000);
                });
            });
        }
    }
}
