using System;
using System.Threading;

namespace RedisUtils
{
    /// <summary>
    /// redis分布锁
    /// </summary>
    public class RedisLock
    {
        /// <summary>
        /// 加锁
        /// </summary>
        /// <param name="key">键，业务键</param>
        /// <param name="value">对应的值 ，为一个guid码</param>
        /// <param name="milliseconds">豪秒</param>
        /// <param name="action">要执行的代码片断</param>
        /// <returns></returns>
        public static bool Lock(
            string key,
            string value,
            int milliseconds,
            Action action)
        {

            if (RedisManager.Instance.GetDatabase().LockTake(key, value, TimeSpan.FromMilliseconds(milliseconds)))
            {
                try
                {
                    Console.WriteLine("正在处理……");
                    action();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {   
                    //处理结束后释放redis进程锁,否则还要阻塞100毫秒
                    RedisManager.Instance.GetDatabase().LockRelease(key, value);
                }
                return true;
            }
            else
            {
                Console.WriteLine("锁被占用……");
                Thread.Sleep(100);
                return false;
            }


        }

        /// <summary>
        /// 锁等待
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="milliseconds"></param>
        /// <param name="action"></param>
        public static void LockAwait(
           string key,
           string value,
           int milliseconds,
           Action action)
        {
            while (true)
            {
                if (Lock(key, value, milliseconds, action))
                {
                    break;
                }
                Thread.Sleep(100);
            }
        }

        }
}
