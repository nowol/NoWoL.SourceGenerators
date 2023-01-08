using System;
using System.Collections.Generic;
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
            /// <param name="param">The param</param>
            /// <param name="param2">The 2nd param</param>
            /// <param name="param3">The 3rd param</param>
            /// 
            /// <returns>The return</returns>
            [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
            public string MainMethod(int param, System.Func<int, string, string> param2, System.Action<int, string> param3)
            {
                string str = param2(1, "");

                foreach (var n in SimulateWorkStream())
                {

                    TheMethod(() => SimulateWork(3000));
                    TheMethod(() => System.Threading.Thread.Sleep(3000));
                    TheMethod(() => System.Threading.Thread.Sleep(3001));
                    TheMethod(() => System.Threading.Thread.Sleep(3002));

                    AnotherMethod(SimulateWork);
                    
                    void AnotherMethod(System.Action<int> funky)
                    {
                        funky(3);
                    }
                }

                foreach (var n in new string[] { "hello" })
                { }

                void TheMethod(System.Action funky9000)
                {
                    funky9000();
                }

                int SimulateWork(int value)
                {
                    System.Threading.Thread.Sleep(3000);

                    return 3;
                    void TheMethodValue(System.Action funky)
                    {
                        funky();
                    }

                    void AnotherMethodValue(System.Action<int> funky)
                    {
                        funky(3);
                    }

                    int SimulateWorkValue(int value)
                    {
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                        return 3;
                    }

                    void ReturnNonGenericTask(System.Action funky)
                    {
                        funky();
                    }
                    
                    int ReturnGenericTask(System.Func<int> funky)
                    {
                        return funky();
                    }

                    void ReturnTaskWithNormalReturnStatement(System.Action funky)
                    {
						funky();
                        return;
                    }
                }

                return param2(1, "");
            }
        }
    }
}