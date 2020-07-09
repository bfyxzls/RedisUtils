using RedisUtils;
using System;
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
            //sql_insert("beijing", 20);
            //sql_insert("hebei", 25);
            sql();

            Console.Read();
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
