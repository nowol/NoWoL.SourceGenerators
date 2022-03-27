using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        internal partial class InnerTestClass
        {
            /// <summary>
            /// The summary
            /// </summary>
            /// <param name = "param">The param</param>
            /// <param name = "param2Async">The 2nd param</param>
            /// <param name = "param3">The 3rd param</param>
            /// <param name = "token">The cancel token</param>
            /// <returns>The return</returns>
            public string MainMethod(int param, Func<int, string, string> param2Async, System.Action<int, string> param3, System.Threading.CancellationToken token)
            {
                string str = param2Async(1, "");
                foreach (var n in SimulateWorkStream())
                {
                    TheMethod(() => SimulateWork(3000));
                    TheMethod(() => System.Threading.Thread.Sleep(3000));
                    AnotherMethod(SimulateWork);
                    void AnotherMethod(System.Action<int> funkyAsync)
                    {
                        funkyAsync(3);
                    }
                }

                foreach (var n in new string[]{"hello"})
                {
                }

                void TheMethod(System.Action funkyAsync)
                {
                    funkyAsync();
                }

                int SimulateWork(int value)
                {
                    System.Threading.Thread.Sleep(3000);
                    return 3;
                    void TheMethodValue(System.Action funkyAsync)
                    {
                        funkyAsync();
                    }

                    void AnotherMethodValue(System.Action<int> funkyAsync)
                    {
                        funkyAsync(3);
                    }

                    int SimulateWorkValue(int value)
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                        return 3;
                    }

                    void ReturnNonGenericTask(System.Action funkyAsync)
                    {
                        funkyAsync();
                    }

                    int ReturnGenericTask(System.Func<int> funkyAsync)
                    {
                        return funkyAsync();
                    }
                }

                return param2Async(1, "");
            }
        }
    }
}