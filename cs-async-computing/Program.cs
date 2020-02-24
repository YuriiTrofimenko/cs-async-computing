using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cs_async_computing
{
    class Program
    {
        // private delegate UInt64 AsyncSumDel(UInt64 n);
        private delegate UInt64 AsyncFibDel(UInt64 n);
        private static volatile int count = 0;
        static void Main(string[] args)
        {
            /*AsyncSumDel del = Sum;
            // AsyncSumDel del = new AsyncSumDel(Sum);
            IAsyncResult ar =
                del.BeginInvoke(1000000000, EndSum, del);
            while (!ar.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }
            Console.ReadKey();*/

            /*for (UInt64 i = 0; i < 40; i++)
            {
                Console.WriteLine(Fib(i));
            }*/

            /*AsyncFibDel del = Fib;
            IAsyncResult ar1 =
                del.BeginInvoke(40, EndSum, del);
            IAsyncResult ar2 =
                del.BeginInvoke(35, EndSum, del);
            IAsyncResult ar3 =
                del.BeginInvoke(30, EndSum, del);*/
            DoWork();
            Console.ReadKey();
        }

        // v1
        public static UInt64 Sum(UInt64 n)
        {
            UInt64 sum = 1;
            for (UInt64 i = 2; i < n; ++i)
                sum += i;
            return sum;
        }

        // v2
        public static UInt64 Fib(UInt64 n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }
            return Fib(n - 1) + Fib(n - 2);
        }

        /*public static UInt64 FibResult(UInt64 n)
        {
            // Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            return Fib(n);
        }*/

        private static IAsyncResult BuildTask(ulong pos, string tag, List<string> localResult)
        {
            AsyncFibDel del = Fib;
            return del.BeginInvoke(pos, (ar) => {
                AsyncFibDel func = (AsyncFibDel)ar.AsyncState;
                UInt64 res = func.EndInvoke(ar);
                Console.WriteLine($"pos = {pos}; tag = {tag}; res = {res}");
                lock (localResult)
                {
                    localResult.Add(tag);
                }
            }, del);
        }

        public static void DoWork()
        {
           
            for (int i = 0; i < 100; i++)
            {
                List<string> localResults = new List<string>();
                IAsyncResult ar1 =
                    BuildTask(30, "A", localResults);
                IAsyncResult ar2 =
                    BuildTask(25, "B", localResults);
                IAsyncResult ar3 =
                    BuildTask(20, "C", localResults);
                while (localResults.Count != 3)
                {
                    Thread.Sleep(10);
                }
                if (localResults[0] == "C" && localResults[1] == "B" && localResults[2] == "A")
                {
                    Interlocked.Increment(ref count);
                    // Console.WriteLine("hit");
                }
            }
            Console.WriteLine($"count = {count}");
        }

        /*private static void EndSum(IAsyncResult ar)
        {
            AsyncSumDel del = (AsyncSumDel)ar.AsyncState;
            UInt64 res = del.EndInvoke(ar);
            Console.WriteLine("Сумма = " + res);
        }*/

        private static void EndSum(IAsyncResult ar)
        {
            AsyncFibDel del = (AsyncFibDel)ar.AsyncState;
            UInt64 res = del.EndInvoke(ar);
            Console.WriteLine("Fib = " + res);
            // Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
        }
    }
}
