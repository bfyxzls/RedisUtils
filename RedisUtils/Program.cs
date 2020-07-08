using RedisUtils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace redis.@lock
{
    class Program
    {
        static void Main(string[] args)
        {
            RedisManager.Instance.GetDatabase(0).StringSet("hello", "你好");

            wait();

            Console.Read();
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
