using System;
using System.Threading.Tasks;

namespace Test
{
    public partial class TestClass
    {
        [System.CodeDom.Compiler.GeneratedCodeAttribute("AsyncToSyncConverterGenerator", "1.0.0.0")]
        public void MainMethod()
        {
            TheMethod(() => SimulateWork(3000));
            TheMethod(() => SimulateWork(5000));

            AnotherMethod(SimulateWork);

            void TheMethod(System.Action funky)
            {
                funky();
            }

            void AnotherMethod(System.Action<int> funky)
            {
                funky(3);
            }

            int SimulateWork(int value)
            {
                System.Threading.Thread.Sleep(3000);
                return 3;
            }

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
                System.Threading.Thread.Sleep(3000);
                return 3;
            }

            void ReturnNonGenericTask(System.Action funky)
            {
                funky();
            }

            int ReturnGenericTask(System.Func<int> funkyZ)
            {
                return funkyZ();
            }

            int ReturnGenericTaskWhereTheReturnShouldBeKept(System.Func<int> funky)
            {
                return funky();
            }
        }
    }
}